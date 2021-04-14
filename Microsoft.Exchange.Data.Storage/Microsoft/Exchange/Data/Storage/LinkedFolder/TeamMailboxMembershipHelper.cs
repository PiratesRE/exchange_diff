using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TeamMailboxMembershipHelper
	{
		public TeamMailbox TeamMailbox { get; private set; }

		public IRecipientSession DataSession { get; private set; }

		public TeamMailboxMembershipHelper(TeamMailbox tm, IRecipientSession dataSession)
		{
			if (tm == null)
			{
				throw new ArgumentNullException("tm");
			}
			if (dataSession == null)
			{
				throw new ArgumentNullException("dataSession");
			}
			this.TeamMailbox = tm;
			this.DataSession = dataSession;
		}

		public static bool IsUserQualifiedType(ADUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			return TeamMailboxMembershipHelper.IsUserQualifiedType(user);
		}

		public static bool IsUserQualifiedType(ADRawEntry user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			return user[ADRecipientSchema.RecipientTypeDetails] != null && (user[ADRecipientSchema.RecipientTypeDetails].Equals(RecipientTypeDetails.UserMailbox) || user[ADRecipientSchema.RecipientTypeDetails].Equals(RecipientTypeDetails.LinkedMailbox) || user[ADRecipientSchema.RecipientTypeDetails].Equals((RecipientTypeDetails)((ulong)int.MinValue)) || user[ADRecipientSchema.RecipientTypeDetails].Equals(RecipientTypeDetails.MailUser));
		}

		public bool UpdateTeamMailboxUserList(IList<ADObjectId> currentUserList, IList<ADObjectId> newUserList, out IList<ADObjectId> usersToAdd, out IList<ADObjectId> usersToRemove)
		{
			if (currentUserList == null)
			{
				throw new ArgumentNullException("currentList");
			}
			if (newUserList == null)
			{
				throw new ArgumentNullException("newUserList");
			}
			usersToRemove = TeamMailbox.DiffUsers(currentUserList, newUserList);
			usersToAdd = TeamMailbox.DiffUsers(newUserList, currentUserList);
			foreach (ADObjectId item in usersToRemove)
			{
				currentUserList.Remove(item);
			}
			foreach (ADObjectId item2 in usersToAdd)
			{
				currentUserList.Add(item2);
			}
			return usersToRemove.Count > 0 || usersToAdd.Count > 0;
		}

		public bool ClearClosedAndDeletedTeamMailboxesInShowInMyClientList(ADUser userMailbox)
		{
			if (userMailbox == null)
			{
				throw new ArgumentNullException("userMailbox");
			}
			bool result = false;
			List<ADObjectId> list = new List<ADObjectId>();
			foreach (ADObjectId item in userMailbox.TeamMailboxShowInClientList)
			{
				list.Add(item);
			}
			foreach (ADObjectId adobjectId in list)
			{
				Exception ex;
				ADUser aduser = TeamMailboxADUserResolver.Resolve(this.DataSession, adobjectId, out ex);
				if (ex == null && (aduser == null || !TeamMailbox.FromDataObject(aduser).Active))
				{
					userMailbox.TeamMailboxShowInClientList.Remove(adobjectId);
					result = true;
				}
			}
			return result;
		}

		public bool SetShowInMyClient(ADObjectId userId, bool show, out bool maxPinnedReached, out Exception ex)
		{
			if (userId == null)
			{
				throw new ArgumentNullException("userId");
			}
			bool flag = false;
			maxPinnedReached = false;
			ex = null;
			ADUser aduser = TeamMailboxADUserResolver.ResolveBypassCache(this.DataSession, userId, out ex);
			if (ex == null && aduser != null && !aduser.RecipientTypeDetails.Equals(RecipientTypeDetails.MailUser))
			{
				ADObjectId id = this.TeamMailbox.Id;
				try
				{
					flag = this.ClearClosedAndDeletedTeamMailboxesInShowInMyClientList(aduser);
					maxPinnedReached = (show && aduser.TeamMailboxShowInClientList.Count >= 10);
					if (!maxPinnedReached)
					{
						if (show && !aduser.TeamMailboxShowInClientList.Contains(id))
						{
							aduser.TeamMailboxShowInClientList.Add(id);
							flag = true;
						}
						else if (!show && aduser.TeamMailboxShowInClientList.Contains(id))
						{
							aduser.TeamMailboxShowInClientList.Remove(id);
							flag = true;
						}
					}
					if (flag)
					{
						try
						{
							this.DataSession.Save(aduser);
						}
						catch (TransientException arg)
						{
							ex = new Exception(string.Format("When setting ShowInMyClient for user {0}, an error happened: {1}", aduser.DisplayName, arg));
						}
						catch (DataSourceOperationException arg2)
						{
							ex = new Exception(string.Format("When setting ShowInMyClient for user {0}, an error happened: {1}", aduser.DisplayName, arg2));
						}
					}
				}
				catch (InvalidOperationException arg3)
				{
					ex = new Exception(string.Format("When setting ShowInMyClient for user {0}, an error happened: {1}", aduser.DisplayName, arg3));
				}
			}
			return flag;
		}

		public void SetShowInMyClient(IList<ADObjectId> usersToAdd, IList<ADObjectId> usersToRemove, out IList<Exception> exceptions)
		{
			exceptions = new List<Exception>();
			if (usersToAdd != null)
			{
				foreach (ADObjectId userId in usersToAdd)
				{
					bool flag;
					Exception ex;
					this.SetShowInMyClient(userId, true, out flag, out ex);
					if (ex != null)
					{
						exceptions.Add(ex);
					}
				}
			}
			if (usersToRemove != null)
			{
				foreach (ADObjectId userId2 in usersToRemove)
				{
					bool flag;
					Exception ex;
					this.SetShowInMyClient(userId2, false, out flag, out ex);
					if (ex != null)
					{
						exceptions.Add(ex);
					}
				}
			}
		}

		public void SetTeamMailboxUserPermissions(IList<ADObjectId> usersToAdd, IList<ADObjectId> usersToRemove, SecurityIdentifier[] additionalSids, bool save = true)
		{
			ADUser aduser = (ADUser)this.TeamMailbox.DataObject;
			RawSecurityDescriptor exchangeSecurityDescriptor = aduser.ExchangeSecurityDescriptor;
			ActiveDirectorySecurity activeDirectorySecurity = SecurityDescriptorConverter.ConvertToActiveDirectorySecurity(exchangeSecurityDescriptor);
			if (usersToAdd != null)
			{
				foreach (ADObjectId userId in usersToAdd)
				{
					SecurityIdentifier userSid = this.GetUserSid(userId);
					if (userSid != null)
					{
						activeDirectorySecurity.AddAccessRule(new ActiveDirectoryAccessRule(userSid, ActiveDirectoryRights.CreateChild, AccessControlType.Allow, Guid.Empty, ActiveDirectorySecurityInheritance.All, Guid.Empty));
					}
				}
			}
			if (usersToRemove != null)
			{
				foreach (ADObjectId userId2 in usersToRemove)
				{
					SecurityIdentifier userSid2 = this.GetUserSid(userId2);
					if (userSid2 != null)
					{
						activeDirectorySecurity.RemoveAccessRule(new ActiveDirectoryAccessRule(userSid2, ActiveDirectoryRights.CreateChild, AccessControlType.Allow, Guid.Empty, ActiveDirectorySecurityInheritance.All, Guid.Empty));
					}
				}
			}
			if (additionalSids != null)
			{
				foreach (SecurityIdentifier identity in additionalSids)
				{
					activeDirectorySecurity.AddAccessRule(new ActiveDirectoryAccessRule(identity, ActiveDirectoryRights.CreateChild, AccessControlType.Allow, Guid.Empty, ActiveDirectorySecurityInheritance.All, Guid.Empty));
				}
			}
			aduser.ExchangeSecurityDescriptor = new RawSecurityDescriptor(activeDirectorySecurity.GetSecurityDescriptorBinaryForm(), 0);
			if (save)
			{
				this.DataSession.Save(aduser);
			}
		}

		private SecurityIdentifier GetUserSid(ADObjectId userId)
		{
			if (userId == null)
			{
				throw new ArgumentNullException("userId");
			}
			Exception ex;
			ADUser aduser = TeamMailboxADUserResolver.Resolve(this.DataSession, userId, out ex);
			if (aduser == null)
			{
				return null;
			}
			if (null != aduser.MasterAccountSid && !aduser.MasterAccountSid.IsWellKnown(WellKnownSidType.SelfSid))
			{
				return aduser.MasterAccountSid;
			}
			return aduser.Sid;
		}

		private ADUser ResolveUserFunction(ADObjectId id, out Exception ex)
		{
			return TeamMailboxADUserResolver.Resolve(this.DataSession, id, out ex);
		}

		private const ActiveDirectoryRights MailboxRightsFullAccess = ActiveDirectoryRights.CreateChild;
	}
}
