using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.LinkedFolder;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class SetTeamMailbox : SingleStepServiceCommand<SetTeamMailboxRequest, ServiceResultNone>
	{
		public SetTeamMailbox(CallContext callContext, SetTeamMailboxRequest request) : base(callContext, request)
		{
			this.request = request;
			OAuthIdentity oauthIdentity = CallContext.Current.HttpContext.User.Identity as OAuthIdentity;
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(oauthIdentity.OrganizationId);
			this.readWriteSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, null, CultureInfo.InvariantCulture.LCID, false, ConsistencyMode.IgnoreInvalid, null, sessionSettings, 62, ".ctor", "f:\\15.00.1497\\sources\\dev\\services\\src\\Core\\servicecommands\\SetTeamMailbox.cs");
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new SetTeamMailboxResponseMessage(base.Result.Code, base.Result.Error);
		}

		internal override ServiceResult<ServiceResultNone> Execute()
		{
			ServiceResult<ServiceResultNone> result = new ServiceResult<ServiceResultNone>(new ServiceResultNone());
			string empty = string.Empty;
			Exception ex = null;
			base.CallContext.ProtocolLog.AppendGenericInfo("TeamMailboxLifecycleState", this.request.State);
			ADUser aduser;
			if (!this.TryResolveUser(this.request.EmailAddress.EmailAddress, out aduser, out ex) || !TeamMailbox.IsLocalTeamMailbox(aduser) || TeamMailbox.IsPendingDeleteSiteMailbox(aduser))
			{
				return new ServiceResult<ServiceResultNone>(new ServiceError((ex == null) ? string.Empty : ex.Message, ResponseCodeType.ErrorTeamMailboxNotFound, 0, ExchangeVersion.Exchange2012));
			}
			TeamMailbox teamMailbox = TeamMailbox.FromDataObject(aduser);
			ExchangePrincipal fromCache = ExchangePrincipalCache.GetFromCache(teamMailbox.PrimarySmtpAddress.ToString(), base.CallContext.ADRecipientSessionContext);
			base.CallContext.ProtocolLog.AppendGenericInfo("TeamMailboxGuid", fromCache.MailboxInfo.MailboxGuid);
			base.CallContext.ProtocolLog.AppendGenericInfo("TeamMailboxWebId", teamMailbox.WebId);
			base.CallContext.ProtocolLog.AppendGenericInfo("TeamMailboxWebCollectionURL", teamMailbox.WebCollectionUrl);
			base.CallContext.ProtocolLog.AppendGenericInfo("TeamMailboxRequestSPURL", this.request.SharePointSiteUrl);
			if (teamMailbox.SharePointUrl == null)
			{
				return new ServiceResult<ServiceResultNone>(new ServiceError(string.Empty, ResponseCodeType.ErrorTeamMailboxNotLinkedToSharePoint, 0, ExchangeVersion.Exchange2012));
			}
			if (!SetTeamMailbox.IsRequestUrlValid(this.request.SharePointSiteUrl, teamMailbox))
			{
				return new ServiceResult<ServiceResultNone>(new ServiceError(string.Empty, ResponseCodeType.ErrorTeamMailboxUrlValidationFailed, 0, ExchangeVersion.Exchange2012));
			}
			if (!base.CallContext.AuthZBehavior.IsAllowedToPrivilegedOpenMailbox(fromCache))
			{
				throw new ServiceAccessDeniedException((CoreResources.IDs)2435663882U);
			}
			try
			{
				switch (this.request.State)
				{
				case TeamMailboxLifecycleState.Active:
					if (!teamMailbox.Active)
					{
						teamMailbox.ClosedTime = null;
						this.readWriteSession.Save(aduser);
						TeamMailboxNotificationHelper teamMailboxNotificationHelper = new TeamMailboxNotificationHelper(teamMailbox, this.readWriteSession);
						List<Exception> list = new List<Exception>();
						try
						{
							list = teamMailboxNotificationHelper.SendNotification(teamMailbox.Owners, TeamMailboxNotificationType.Reactivated);
						}
						catch (StorageTransientException item)
						{
							list.Add(item);
						}
						catch (StoragePermanentException item2)
						{
							list.Add(item2);
						}
						if (list != null && list.Count > 0)
						{
							StringBuilder stringBuilder = new StringBuilder();
							foreach (Exception ex2 in list)
							{
								stringBuilder.Append(ex2.Message);
							}
							result = new ServiceResult<ServiceResultNone>(new ServiceError(stringBuilder.ToString(), ResponseCodeType.ErrorTeamMailboxFailedSendingNotifications, 0, ExchangeVersion.Exchange2012));
						}
					}
					break;
				case TeamMailboxLifecycleState.Closed:
					if (teamMailbox.Active)
					{
						teamMailbox.ClosedTime = new DateTime?(DateTime.UtcNow);
						this.readWriteSession.Save(aduser);
						TeamMailboxNotificationHelper teamMailboxNotificationHelper2 = new TeamMailboxNotificationHelper(teamMailbox, this.readWriteSession);
						List<Exception> list2 = new List<Exception>();
						try
						{
							list2 = teamMailboxNotificationHelper2.SendNotification(teamMailbox.Owners, TeamMailboxNotificationType.Closed);
						}
						catch (StorageTransientException item3)
						{
							list2.Add(item3);
						}
						catch (StoragePermanentException item4)
						{
							list2.Add(item4);
						}
						if (list2 != null && list2.Count > 0)
						{
							StringBuilder stringBuilder2 = new StringBuilder();
							foreach (Exception ex3 in list2)
							{
								stringBuilder2.Append(ex3.Message);
							}
							result = new ServiceResult<ServiceResultNone>(new ServiceError(stringBuilder2.ToString(), ResponseCodeType.ErrorTeamMailboxFailedSendingNotifications, 0, ExchangeVersion.Exchange2012));
						}
					}
					break;
				case TeamMailboxLifecycleState.Unlinked:
					teamMailbox.SetSharePointSiteInfo(null, Guid.Empty);
					teamMailbox.SharePointUrl = null;
					teamMailbox.SharePointLinkedBy = null;
					this.readWriteSession.Save(aduser);
					break;
				case TeamMailboxLifecycleState.PendingDelete:
					teamMailbox.ClosedTime = TeamMailbox.ClosedTimeOfMarkedForDeletion;
					aduser.HiddenFromAddressListsEnabled = true;
					if (aduser.AddressListMembership != null)
					{
						aduser.AddressListMembership.Clear();
					}
					aduser.DisplayName = "MDEL:" + aduser.DisplayName;
					teamMailbox.SetSharePointSiteInfo(null, Guid.Empty);
					teamMailbox.SharePointUrl = null;
					teamMailbox.SharePointLinkedBy = null;
					this.readWriteSession.Save(aduser);
					this.DeleteSiteMailbox(teamMailbox, fromCache, ref result);
					break;
				default:
					result = new ServiceResult<ServiceResultNone>(new ServiceError(string.Empty, ResponseCodeType.ErrorTeamMailboxErrorUnknown, 0, ExchangeVersion.Exchange2012));
					break;
				}
			}
			catch (TransientException ex4)
			{
				ex = ex4;
			}
			catch (DataSourceOperationException ex5)
			{
				ex = ex5;
			}
			if (ex != null)
			{
				result = new ServiceResult<ServiceResultNone>(new ServiceError(ex.Message, ResponseCodeType.ErrorTeamMailboxErrorUnknown, 0, ExchangeVersion.Exchange2012));
			}
			return result;
		}

		private static bool IsRequestUrlValid(string requestUrl, TeamMailbox teamMailbox)
		{
			bool flag = false;
			Uri uri;
			if (Uri.TryCreate(requestUrl, UriKind.Absolute, out uri))
			{
				Uri uri2;
				if (teamMailbox.WebCollectionUrl != null && Uri.TryCreate(teamMailbox.WebCollectionUrl.AbsoluteUri + "?WebId=" + teamMailbox.WebId, UriKind.Absolute, out uri2))
				{
					flag = (Uri.Compare(uri, uri2, UriComponents.AbsoluteUri, UriFormat.UriEscaped, StringComparison.OrdinalIgnoreCase) == 0);
				}
				if (!flag)
				{
					flag = (Uri.Compare(uri, teamMailbox.SharePointUrl, UriComponents.AbsoluteUri, UriFormat.UriEscaped, StringComparison.OrdinalIgnoreCase) == 0);
				}
			}
			return flag;
		}

		private bool TryResolveUser(string emailAddress, out ADUser user, out Exception ex)
		{
			ex = null;
			user = null;
			if (string.IsNullOrEmpty(emailAddress))
			{
				ex = new ArgumentNullException("emailAddress");
			}
			else if (!SmtpAddress.IsValidSmtpAddress(emailAddress))
			{
				ex = new ArgumentException("Cannot get internal address for caller; identity: " + emailAddress);
			}
			else
			{
				user = (ADUser)this.readWriteSession.FindByProxyAddress(ProxyAddress.Parse(emailAddress));
			}
			return user != null;
		}

		private ADUser ResolveReadOnlyUser(ADObjectId id)
		{
			if (id != null)
			{
				return base.CallContext.ADRecipientSessionContext.GetADRecipientSession().FindADUserByObjectId(id);
			}
			return null;
		}

		private void DeleteSiteMailbox(TeamMailbox teamMailbox, ExchangePrincipal teamMailboxExchangePrincipal, ref ServiceResult<ServiceResultNone> result)
		{
			if (!Globals.IsDatacenter)
			{
				return;
			}
			using (PSLocalTask<RemoveMailbox, Mailbox> pslocalTask = CmdletTaskFactory.Instance.CreateRemoveMailboxTask(teamMailboxExchangePrincipal, "Identity"))
			{
				pslocalTask.Task.Identity = new MailboxIdParameter(teamMailbox.Identity.ToString());
				pslocalTask.Task.Execute();
				if (pslocalTask.Error != null)
				{
					base.CallContext.ProtocolLog.AppendGenericError("TeamMailboxRemoveMailboxError", pslocalTask.ErrorMessage);
				}
			}
		}

		private readonly SetTeamMailboxRequest request;

		private readonly IRecipientSession readWriteSession;
	}
}
