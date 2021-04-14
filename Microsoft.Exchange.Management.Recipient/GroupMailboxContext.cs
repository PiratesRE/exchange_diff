using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;
using Microsoft.Exchange.Diagnostics.Performance;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.SoapWebClient.EWS;
using Microsoft.Exchange.UnifiedGroups;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	internal sealed class GroupMailboxContext
	{
		internal GroupMailboxContext(ADUser groupMailbox, OrganizationId currentOrgId, IRecipientSession adSession, DataAccessHelper.CategorizedGetDataObjectDelegate getUserDataObject, DataAccessHelper.CategorizedGetDataObjectDelegate getGroupDataObject, Task.TaskVerboseLoggingDelegate verboseHandler, Task.TaskWarningLoggingDelegate warningHandler, Task.ErrorLoggerDelegate errorHandler)
		{
			this.groupMailbox = groupMailbox;
			this.currentOrganizationId = currentOrgId;
			this.adSession = adSession;
			this.getUserDataObject = getUserDataObject;
			this.getGroupDataObject = getGroupDataObject;
			this.warningHandler = warningHandler;
			this.errorHandler = errorHandler;
			this.verboseHandler = verboseHandler;
		}

		internal ADUser ExecutingUser { get; private set; }

		internal static string EnsureGroupIsInDirectoryCache(string perfTrackerPrefix, IRecipientSession readWriteSession, ADUser groupAdUser)
		{
			string result;
			using (new StopwatchPerformanceTracker(perfTrackerPrefix + ".EnsureGroupIsInDirectoryCache", GenericCmdletInfoDataLogger.Instance))
			{
				IRecipientSession tenantOrRootRecipientReadOnlySession = DirectorySessionFactory.Default.GetTenantOrRootRecipientReadOnlySession(readWriteSession, groupAdUser.OriginatingServer, 204, "EnsureGroupIsInDirectoryCache", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\GroupMailbox\\GroupMailboxContext.cs");
				ProxyAddress proxyAddress = new SmtpProxyAddress(groupAdUser.PrimarySmtpAddress.ToString(), true);
				ADUser aduser = tenantOrRootRecipientReadOnlySession.FindByProxyAddress(proxyAddress) as ADUser;
				OWAMiniRecipient owaminiRecipient = tenantOrRootRecipientReadOnlySession.FindMiniRecipientByProxyAddress<OWAMiniRecipient>(proxyAddress, OWAMiniRecipientSchema.AdditionalProperties);
				string text = string.Format("Querying AD for group. ProxyAddress={0}, DomainController={1}, FoundADUser={2}, FoundOwaMiniRecipient={3}", new object[]
				{
					proxyAddress,
					groupAdUser.OriginatingServer,
					aduser != null,
					owaminiRecipient != null
				});
				GroupMailboxContext.Tracer.TraceDebug<string>(0L, "{0}", text);
				result = text;
			}
			return result;
		}

		internal void EnsureGroupIsInDirectoryCache(string perfTrackerPrefix)
		{
			string message = GroupMailboxContext.EnsureGroupIsInDirectoryCache(perfTrackerPrefix, this.adSession, this.groupMailbox);
			this.AddVerboseLog(message);
		}

		internal void SetPermissionsVersion(int permissionsVersion)
		{
			this.permissionsVersion = new int?(permissionsVersion);
		}

		internal void SetGroupAdUser(ADUser groupAdUser)
		{
			ArgumentValidator.ThrowIfNull("groupAdUser", groupAdUser);
			ArgumentValidator.ThrowIfInvalidValue<ADUser>("groupAdUser", groupAdUser, (ADUser group) => group.RecipientTypeDetails == RecipientTypeDetails.GroupMailbox);
			this.groupMailbox = groupAdUser;
		}

		internal void AddPublicToGroups(IEnumerable<RecipientIdParameter> ids)
		{
			ArgumentValidator.ThrowIfNull("ids", ids);
			using (new StopwatchPerformanceTracker("GroupMailboxContext.AddPublicToGroups", GenericCmdletInfoDataLogger.Instance))
			{
				this.AddVerboseLog("Start: AddPublicToGroups");
				Exception ex = null;
				foreach (RecipientIdParameter recipientIdParameter in ids)
				{
					ADGroup adgroup = this.ResolveGroup(recipientIdParameter, out ex);
					if (ex != null)
					{
						this.errorHandler(new GroupMailboxFailedToResolvePublicToGroupException(Strings.ErrorUnableToResolvePublicToGroup(recipientIdParameter.ToString(), ex.Message)), ExchangeErrorCategory.Client, null);
					}
					if (adgroup != null && !this.groupMailbox.DelegateListLink.Contains(adgroup.OriginalId))
					{
						this.groupMailbox.DelegateListLink.Add(adgroup.OriginalId);
					}
				}
				this.AddVerboseLog("End: AddPublicToGroups");
			}
		}

		internal void SetMembers(RecipientIdParameter[] setMembers)
		{
			this.setMembers = setMembers;
		}

		internal void AddAndRemoveMembers(RecipientIdParameter[] addMembers, RecipientIdParameter[] removeMembers)
		{
			this.addMembers = addMembers;
			this.removeMembers = removeMembers;
		}

		internal void SetExecutingUser(RecipientIdParameter executingUser)
		{
			using (new StopwatchPerformanceTracker("GroupMailboxContext.SetExecutingUser", GenericCmdletInfoDataLogger.Instance))
			{
				Exception ex;
				this.ExecutingUser = this.ResolveUser(executingUser, out ex);
				if (ex != null)
				{
					this.errorHandler(new GroupMailboxFailedToResolveExecutingUserException(Strings.WarningUnableToResolveUser(executingUser.ToString(), ex.Message)), ExchangeErrorCategory.Client, null);
				}
			}
		}

		internal ADUser ResolveUser(RecipientIdParameter id, out Exception exception)
		{
			this.AddVerboseLog("Start: ResolveUser");
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			ADUser user;
			if (this.resolveUserCache.TryGetValue(id.RawIdentity, out user))
			{
				exception = null;
				return user;
			}
			exception = GroupMailboxContext.ExecuteADOperationAndHandleException(delegate
			{
				user = (ADUser)this.getUserDataObject(id, this.adSession, this.currentOrganizationId.OrganizationalUnit, null, new LocalizedString?(Strings.ErrorRecipientNotFound((string)id)), new LocalizedString?(Strings.ErrorRecipientNotUnique((string)id)), ExchangeErrorCategory.Client);
			});
			Guid guid;
			if (exception is ManagementObjectNotFoundException && Guid.TryParse(id.RawIdentity, out guid))
			{
				GroupMailboxContext.ExecuteADOperationAndHandleException(delegate
				{
					user = this.adSession.FindADUserByExternalDirectoryObjectId(id.RawIdentity);
				});
				if (user != null)
				{
					exception = null;
				}
			}
			if (exception == null && user != null)
			{
				this.resolveUserCache.Add(id.RawIdentity, user);
			}
			this.AddVerboseLog("End: ResolveUser");
			return user;
		}

		internal ADGroup ResolveGroup(RecipientIdParameter id, out Exception exception)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			ADGroup group = null;
			exception = GroupMailboxContext.ExecuteADOperationAndHandleException(delegate
			{
				group = (ADGroup)this.getGroupDataObject(id, this.adSession, this.currentOrganizationId.OrganizationalUnit, null, new LocalizedString?(Strings.ErrorRecipientNotFound((string)id)), new LocalizedString?(Strings.ErrorRecipientNotUnique((string)id)), ExchangeErrorCategory.Client);
			});
			return group;
		}

		internal static Exception ExecuteADOperationAndHandleException(Action action)
		{
			try
			{
				action();
			}
			catch (ADTransientException result)
			{
				return result;
			}
			catch (ADExternalException result2)
			{
				return result2;
			}
			catch (ADOperationException result3)
			{
				return result3;
			}
			catch (InvalidCastException result4)
			{
				return result4;
			}
			catch (ManagementObjectNotFoundException result5)
			{
				return result5;
			}
			return null;
		}

		internal void NewGroupMailbox(DatabaseLocationInfo databaseLocationInfo, out Exception exception, out ExchangeErrorCategory? errorCategory)
		{
			errorCategory = null;
			try
			{
				this.AddVerboseLog("Start: NewGroupMailbox");
				this.EnsureGroupIsInDirectoryCache("GroupMailboxContext.NewGroupMailbox");
				this.UpdateGroupMailbox(databaseLocationInfo, (GroupMailboxConfigurationActionType)0, this.GetValidGroupMembersFromFromRecipientIds(this.setMembers), null);
				if (this.invalidUsers.Length > 0)
				{
					CmdletLogger.SafeAppendGenericError("UsersWithoutSmtpAddress_New", this.invalidUsers.ToString(), false);
					this.invalidUsers.Clear();
				}
				exception = null;
			}
			catch (Exception ex)
			{
				exception = ex;
				errorCategory = new ExchangeErrorCategory?(ExchangeErrorCategory.ServerTransient);
				if (this.IsWellKnownClientException(ex))
				{
					errorCategory = new ExchangeErrorCategory?(ExchangeErrorCategory.Client);
				}
			}
			finally
			{
				this.AddVerboseLog("End: NewGroupMailbox");
			}
		}

		internal void SetGroupMailbox(GroupMailboxConfigurationActionType forceConfigurationActionMask, out Exception exception, out ExchangeErrorCategory? errorCategory)
		{
			errorCategory = null;
			try
			{
				this.AddVerboseLog("Start: SetGroupMailbox");
				if (this.addMembers != null || this.removeMembers != null || !this.groupMailbox.IsGroupMailboxConfigured || this.permissionsVersion != null || forceConfigurationActionMask != (GroupMailboxConfigurationActionType)0)
				{
					this.UpdateGroupMailbox(null, forceConfigurationActionMask, this.GetValidGroupMembersFromFromRecipientIds(this.addMembers), this.ResolveUsers(this.removeMembers, new Action<RecipientIdParameter, Exception>(this.ReportResolveUserWarning)));
					if (this.invalidUsers.Length > 0)
					{
						CmdletLogger.SafeAppendGenericError("UsersWithoutSmtpAddress_Set", this.invalidUsers.ToString(), false);
						this.invalidUsers.Clear();
					}
				}
				else
				{
					this.AddVerboseLog("No members to update on GroupMailbox or configuration action to execute.");
				}
				exception = null;
			}
			catch (LocalizedException ex)
			{
				exception = ex;
				errorCategory = new ExchangeErrorCategory?(ExchangeErrorCategory.ServerTransient);
				if (this.IsWellKnownClientException(ex))
				{
					errorCategory = new ExchangeErrorCategory?(ExchangeErrorCategory.Client);
				}
			}
			finally
			{
				this.AddVerboseLog("End: SetGroupMailbox");
			}
		}

		internal bool SetOwners(RecipientIdParameter[] ownerIds)
		{
			ArgumentValidator.ThrowIfNull("ownerIds", ownerIds);
			bool result;
			using (new StopwatchPerformanceTracker("GroupMailboxContext.SetOwners", GenericCmdletInfoDataLogger.Instance))
			{
				this.AddVerboseLog("Start: SetOwners");
				List<ADUser> list = this.FilterOutInvalidMembers(this.ResolveUsers(ownerIds, new Action<RecipientIdParameter, Exception>(this.ReportResolveOwnerError)));
				HashSet<ADObjectId> existingOwnersSet = new HashSet<ADObjectId>(this.groupMailbox.Owners);
				List<ADUser> list2 = list.FindAll((ADUser owner) => !existingOwnersSet.Contains(owner.ObjectId));
				List<ADObjectId> list3 = list.ConvertAll<ADObjectId>((ADUser owner) => owner.ObjectId);
				existingOwnersSet.ExceptWith(list3);
				ADObjectId[] array = existingOwnersSet.ToArray<ADObjectId>();
				List<ADUser> list4 = this.ResolveUsers(Array.ConvertAll<ADObjectId, RecipientIdParameter>(array, (ADObjectId ownerId) => new RecipientIdParameter(ownerId)), new Action<RecipientIdParameter, Exception>(this.ReportResolveOwnerError));
				this.groupMailbox.Owners = new MultiValuedProperty<ADObjectId>(list3);
				this.ApplyPermissionToOwners(list2, list4);
				this.AddVerboseLog("End: SetOwners");
				result = (list2.Count > 0 || list4.Count > 0);
			}
			return result;
		}

		internal bool AddOwners(RecipientIdParameter[] ownerIds)
		{
			ArgumentValidator.ThrowIfNull("ownerIds", ownerIds);
			this.AddVerboseLog("Start: AddOwners");
			List<ADUser> list = this.FilterOutInvalidMembers(this.ResolveUsers(ownerIds, new Action<RecipientIdParameter, Exception>(this.ReportResolveOwnerError)));
			List<ADUser> list2 = list.FindAll(delegate(ADUser user)
			{
				if (!this.groupMailbox.Owners.Contains(user.ObjectId))
				{
					this.groupMailbox.Owners.Add(user.ObjectId);
					return true;
				}
				return false;
			});
			this.ApplyPermissionToOwners(list2, null);
			this.AddVerboseLog("End: AddOwners");
			return list2.Count > 0;
		}

		internal bool RemoveOwners(RecipientIdParameter[] ownerIds)
		{
			ArgumentValidator.ThrowIfNull("ownerIds", ownerIds);
			this.AddVerboseLog("Start: RemoveOwners");
			List<ADUser> list = this.ResolveUsers(ownerIds, new Action<RecipientIdParameter, Exception>(this.ReportResolveOwnerError));
			List<ADUser> list2 = list.FindAll(delegate(ADUser user)
			{
				if (this.groupMailbox.Owners.Contains(user.ObjectId))
				{
					if (this.groupMailbox.Owners.Count > 1)
					{
						this.groupMailbox.Owners.Remove(user.ObjectId);
						return true;
					}
					this.errorHandler(new GroupMailboxInvalidOperationException(Strings.ErrorSetGroupMailboxNoOwners), ExchangeErrorCategory.Client, null);
				}
				return false;
			});
			this.ApplyPermissionToOwners(null, list2);
			this.AddVerboseLog("End: RemoveOwners");
			return list2.Count > 0;
		}

		internal void RefreshStoreCache()
		{
			this.AddVerboseLog("Start: RefreshStoreCache");
			bool flag = this.groupMailbox.WhenMailboxCreated == null || this.groupMailbox.WhenMailboxCreated.Value.AddMinutes(15.0).ToUniversalTime() > DateTime.UtcNow;
			if (flag)
			{
				return;
			}
			string text = null;
			try
			{
				ActiveManager activeManagerInstance = ActiveManager.GetActiveManagerInstance();
				DatabaseLocationInfo serverForDatabase = activeManagerInstance.GetServerForDatabase(this.groupMailbox.Database.ObjectGuid);
				text = serverForDatabase.ServerFqdn;
				using (MapiMessageStoreSession mapiMessageStoreSession = new MapiMessageStoreSession(serverForDatabase.ServerLegacyDN, serverForDatabase.ServerLegacyDN + "/cn=Microsoft System Attendant", Fqdn.Parse(serverForDatabase.ServerFqdn)))
				{
					MailboxId mailboxId = new MailboxId(MapiTaskHelper.ConvertDatabaseADObjectIdToDatabaseId(this.groupMailbox.Database), this.groupMailbox.ExchangeGuid);
					this.AddVerboseLog(Strings.VerboseSaveStoreMailboxSecurityDescriptor(mailboxId.ToString(), mapiMessageStoreSession.ServerName));
					mapiMessageStoreSession.Administration.PurgeCachedMailboxObject(mailboxId.MailboxGuid);
				}
			}
			catch (DatabaseNotFoundException)
			{
				this.AddVerboseLog(Strings.ErrorMailboxDatabaseNotFound(this.groupMailbox.Database.ToString()));
			}
			catch (MapiExceptionNetworkError)
			{
				this.AddVerboseLog(Strings.ErrorFailedToConnectToStore((text != null) ? text : string.Empty));
			}
			catch (MailboxNotFoundException)
			{
				this.AddVerboseLog(Strings.VerboseMailboxNotExistInStore(this.groupMailbox.DistinguishedName));
			}
			this.AddVerboseLog("End: RefreshStoreCache");
		}

		internal void SetExternalResources(bool failOnError)
		{
			if (!AADClientFactory.IsAADEnabled())
			{
				this.AddVerboseLog("AADClient is not enabled. Skipping SetExternalResources step.");
				return;
			}
			using (new StopwatchPerformanceTracker("GroupMailboxContext.PublishExchangeResources", GenericCmdletInfoDataLogger.Instance))
			{
				try
				{
					new GroupMailboxExchangeResourcesPublisher(this.groupMailbox, this.GetActivityId()).Publish(new int?(0));
					this.AddVerboseLog("Published ExternalResources to AAD.");
					if (!this.groupMailbox.GroupMailboxExternalResourcesSet)
					{
						this.groupMailbox.GroupMailboxExternalResourcesSet = true;
						this.adSession.Save(this.groupMailbox);
					}
					this.AddVerboseLog("Set GroupMailboxExternalResourcesSet to true");
				}
				catch (LocalizedException ex)
				{
					if (failOnError)
					{
						this.errorHandler(new LocalizedException(new LocalizedString("Failed to set exchangeResources in AAD: " + ex.ToString()), ex), ExchangeErrorCategory.ServerTransient, null);
					}
					else
					{
						this.warningHandler(new LocalizedString("Failed to set exchangeResources in AAD: " + ex.ToString()));
					}
				}
			}
		}

		private List<ADUser> FilterOutInvalidMembers(List<ADUser> membersOrOwnersToAdd)
		{
			List<ADUser> list = new List<ADUser>(membersOrOwnersToAdd.Count);
			foreach (ADUser aduser in membersOrOwnersToAdd)
			{
				if (MailboxLocatorValidator.IsValidUserLocator(aduser))
				{
					list.Add(aduser);
				}
				else
				{
					LocalizedString message = new LocalizedString(string.Format("Cannot add User:{0} with Type:{1}", aduser.ExternalDirectoryObjectId, aduser.RecipientTypeDetails));
					this.warningHandler(message);
				}
			}
			return list;
		}

		private void UpdateGroupMailbox(DatabaseLocationInfo databaseLocationInfo, GroupMailboxConfigurationActionType forceConfigurationActionMask, List<ADUser> addedMembersList = null, List<ADUser> removedMembersList = null)
		{
			ADUser[] addedMembers = (addedMembersList != null && addedMembersList.Count > 0) ? addedMembersList.ToArray() : null;
			ADUser[] removedMembers = (removedMembersList != null && removedMembersList.Count > 0) ? removedMembersList.ToArray() : null;
			this.UpdateMembershipInAD(this.groupMailbox, addedMembers, removedMembers);
			try
			{
				this.AddVerboseLog("Start: UpdateGroupMailbox EWS Call");
				using (new StopwatchPerformanceTracker("GroupMailboxContext.UpdateGroupMailbox", GenericCmdletInfoDataLogger.Instance))
				{
					UpdateGroupMailboxBase updateGroupMailboxBase = UpdateGroupMailboxFactory.CreateUpdateGroupMailbox(this.adSession, this.groupMailbox, databaseLocationInfo, this.ExecutingUser, forceConfigurationActionMask, addedMembers, removedMembers, this.permissionsVersion);
					updateGroupMailboxBase.Execute();
					if (updateGroupMailboxBase.Error != null)
					{
						if (updateGroupMailboxBase.ResponseCode == null)
						{
							throw new LocalizedException(new LocalizedString(updateGroupMailboxBase.Error));
						}
						if (updateGroupMailboxBase.ResponseCode.Value == ResponseCodeType.ErrorOrganizationAccessBlocked)
						{
							throw new TenantAccessBlockedException(new LocalizedString(updateGroupMailboxBase.Error));
						}
						if (updateGroupMailboxBase.ResponseCode.Value == ResponseCodeType.ErrorInvalidLicense)
						{
							throw new InvalidLicenseException(new LocalizedString(updateGroupMailboxBase.Error));
						}
						throw new LocalizedException(new LocalizedString(updateGroupMailboxBase.Error));
					}
					else if (!this.groupMailbox.IsGroupMailboxConfigured)
					{
						this.groupMailbox.IsGroupMailboxConfigured = true;
					}
				}
			}
			finally
			{
				this.AddVerboseLog("End: UpdateGroupMailbox EWS Call");
			}
		}

		private void UpdateMembershipInAD(ADUser groupADUser, ADUser[] addedMembers, ADUser[] removedMembers)
		{
			try
			{
				this.AddVerboseLog("Start: UpdateMembers in AD");
				using (new StopwatchPerformanceTracker("GroupMailboxContext.UpdateMembersInAD", GenericCmdletInfoDataLogger.Instance))
				{
					UnifiedGroupADAccessLayer unifiedGroupADAccessLayer = new UnifiedGroupADAccessLayer(groupADUser, this.adSession.DomainController);
					unifiedGroupADAccessLayer.UpdateMembership(addedMembers, removedMembers);
				}
			}
			finally
			{
				this.AddVerboseLog("End: UpdateMembers in AD");
			}
		}

		private List<ADUser> ResolveUsers(RecipientIdParameter[] userIds, Action<RecipientIdParameter, Exception> errorAction)
		{
			if (userIds == null)
			{
				return new List<ADUser>(0);
			}
			List<ADUser> result;
			using (new StopwatchPerformanceTracker("GroupMailboxContext.ResolveUsers", GenericCmdletInfoDataLogger.Instance))
			{
				this.AddVerboseLog("Start: ResolveUsers");
				List<ADUser> list = new List<ADUser>(userIds.Length);
				foreach (RecipientIdParameter recipientIdParameter in userIds)
				{
					Exception ex;
					ADUser aduser = this.ResolveUser(recipientIdParameter, out ex);
					if (ex != null)
					{
						errorAction(recipientIdParameter, ex);
					}
					if (aduser != null)
					{
						if (aduser.PrimarySmtpAddress.IsValidAddress)
						{
							list.Add(aduser);
						}
						else
						{
							this.warningHandler(Strings.NoSmtpAddressForRecipientId(recipientIdParameter.ToString()));
							this.invalidUsers.Append(aduser.ObjectId).Append("|");
						}
					}
				}
				this.AddVerboseLog("End: ResolveUsers");
				result = list;
			}
			return result;
		}

		private void ApplyPermissionToOwners(List<ADUser> ownersToAdd, List<ADUser> ownersToRemove)
		{
			this.AddVerboseLog("Start: ApplyPermissionToOwners");
			Exception ex = null;
			try
			{
				ADUser aduser = this.groupMailbox;
				RawSecurityDescriptor exchangeSecurityDescriptor = aduser.ExchangeSecurityDescriptor;
				ActiveDirectorySecurity activeDirectorySecurity = SecurityDescriptorConverter.ConvertToActiveDirectorySecurity(exchangeSecurityDescriptor);
				bool flag = false;
				if (ownersToAdd != null)
				{
					foreach (ADUser aduser2 in ownersToAdd)
					{
						SecurityIdentifier userSid = this.GetUserSid(aduser2);
						if (userSid != null)
						{
							this.AddOwnerPermission(aduser2.PrimarySmtpAddress.ToString(), activeDirectorySecurity, userSid);
							flag = true;
						}
						else
						{
							this.ReportResolveUserSidError(aduser2);
						}
					}
				}
				if (ownersToRemove != null)
				{
					foreach (ADUser aduser3 in ownersToRemove)
					{
						SecurityIdentifier userSid2 = this.GetUserSid(aduser3);
						if (userSid2 != null)
						{
							this.RemoveOwnerPermission(aduser3.PrimarySmtpAddress.ToString(), activeDirectorySecurity, userSid2);
							flag = true;
						}
						else
						{
							this.ReportResolveUserSidError(aduser3);
						}
					}
				}
				if (flag)
				{
					aduser.ExchangeSecurityDescriptor = new RawSecurityDescriptor(activeDirectorySecurity.GetSecurityDescriptorBinaryForm(), 0);
				}
			}
			catch (OverflowException ex2)
			{
				ex = ex2;
			}
			catch (COMException ex3)
			{
				ex = ex3;
			}
			catch (UnauthorizedAccessException ex4)
			{
				ex = ex4;
			}
			catch (TransientException ex5)
			{
				ex = ex5;
			}
			catch (DataSourceOperationException ex6)
			{
				ex = ex6;
			}
			if (ex != null)
			{
				this.errorHandler(new GroupMailboxFailedToSetPermissionException(Strings.ErrorSetGroupMailboxUserPermissions(this.groupMailbox.ExchangeGuid, ex.Message)), ExchangeErrorCategory.ServerTransient, null);
			}
			this.AddVerboseLog("End: ApplyPermissionToOwners");
		}

		private SecurityIdentifier GetUserSid(ADUser user)
		{
			SecurityIdentifier result;
			if (null != user.MasterAccountSid && !user.MasterAccountSid.IsWellKnown(WellKnownSidType.SelfSid))
			{
				result = user.MasterAccountSid;
			}
			else
			{
				result = user.Sid;
			}
			return result;
		}

		private void AddVerboseLog(string message)
		{
			this.verboseHandler(new LocalizedString(message));
		}

		private List<ADUser> GetValidGroupMembersFromFromRecipientIds(RecipientIdParameter[] ids)
		{
			if (ids == null)
			{
				return null;
			}
			return this.FilterOutInvalidMembers(this.ResolveUsers(ids, new Action<RecipientIdParameter, Exception>(this.ReportResolveUserWarning)));
		}

		private void AddOwnerPermission(string user, ActiveDirectorySecurity adSecurity, SecurityIdentifier sid)
		{
			adSecurity.AddAccessRule(new ActiveDirectoryAccessRule(sid, GroupMailboxPermissionHandler.MailboxRightsFullAccess, AccessControlType.Allow, Guid.Empty, ActiveDirectorySecurityInheritance.All, Guid.Empty));
			this.AddVerboseLog(string.Format("Granted full mailbox access to {0}", user));
		}

		private void RemoveOwnerPermission(string user, ActiveDirectorySecurity adSecurity, SecurityIdentifier sid)
		{
			adSecurity.RemoveAccess(sid, AccessControlType.Allow);
			this.AddVerboseLog(string.Format("Removed full mailbox access to {0}", user));
		}

		private void ReportResolveOwnerError(RecipientIdParameter id, Exception ex)
		{
			this.errorHandler(new GroupMailboxFailedToResolveOwnerException(Strings.ErrorUnableToResolveOwner(id.ToString(), ex.Message)), ExchangeErrorCategory.Client, null);
		}

		private void ReportResolveUserWarning(RecipientIdParameter id, Exception ex)
		{
			this.warningHandler(Strings.WarningUnableToResolveUser(id.ToString(), ex.Message));
		}

		private void ReportResolveUserSidError(ADUser user)
		{
			this.errorHandler(new GroupMailboxFailedToResolveUserSidException(Strings.ErrorUnableToGetUserSid(user.Name)), ExchangeErrorCategory.Client, null);
		}

		private Guid GetActivityId()
		{
			IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
			Guid guid = (currentActivityScope != null) ? currentActivityScope.ActivityId : Guid.NewGuid();
			this.AddVerboseLog("ActivityId: " + guid);
			return guid;
		}

		private bool IsWellKnownClientException(Exception e)
		{
			return e is TenantAccessBlockedException || e is InvalidLicenseException;
		}

		private const string SystemAttendant = "/cn=Microsoft System Attendant";

		private static readonly Trace Tracer = ExTraceGlobals.CmdletsTracer;

		private readonly OrganizationId currentOrganizationId;

		private readonly IRecipientSession adSession;

		private readonly DataAccessHelper.CategorizedGetDataObjectDelegate getUserDataObject;

		private readonly DataAccessHelper.CategorizedGetDataObjectDelegate getGroupDataObject;

		private readonly Task.TaskWarningLoggingDelegate warningHandler;

		private readonly Task.ErrorLoggerDelegate errorHandler;

		private readonly Task.TaskVerboseLoggingDelegate verboseHandler;

		private readonly Dictionary<string, ADUser> resolveUserCache = new Dictionary<string, ADUser>(StringComparer.OrdinalIgnoreCase);

		private ADUser groupMailbox;

		private RecipientIdParameter[] setMembers;

		private RecipientIdParameter[] addMembers;

		private RecipientIdParameter[] removeMembers;

		private int? permissionsVersion;

		private StringBuilder invalidUsers = new StringBuilder();
	}
}
