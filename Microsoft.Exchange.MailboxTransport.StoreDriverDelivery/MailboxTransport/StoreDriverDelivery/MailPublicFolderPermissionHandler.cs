using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal static class MailPublicFolderPermissionHandler
	{
		internal static AccessCheckResult CheckAccessForEmailDelivery(MailItemDeliver mailItemDeliver, Folder mailPublicFolder)
		{
			if (mailItemDeliver == null)
			{
				throw new ArgumentNullException("MailItemDeliver");
			}
			if (mailPublicFolder == null)
			{
				throw new ArgumentNullException("MailPublicFolder");
			}
			if (mailItemDeliver.ReplayItem == null)
			{
				MailPublicFolderPermissionHandler.Diag.TraceError(0L, "ReplayItem for the message appears to be null.");
				return AccessCheckResult.NotAllowedInternalSystemError;
			}
			if (mailItemDeliver.ReplayItem.From == null)
			{
				MailPublicFolderPermissionHandler.Diag.TraceError(0L, "From attribute of the ReplayItem for the given message appears to be null.");
				return AccessCheckResult.NotAllowedInternalSystemError;
			}
			AccessCheckResult accessCheckResult = AccessCheckResult.NotAllowedAuthenticated;
			ClientSecurityContext context = null;
			bool isAnonymous = false;
			ADRecipientCache<TransportMiniRecipient> recipientCache = mailItemDeliver.MbxTransportMailItem.ADRecipientCache;
			IRecipientSession recipientSession = (recipientCache != null) ? recipientCache.ADSession : null;
			if (recipientSession != null)
			{
				recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(recipientSession.SessionSettings.CurrentOrganizationId), 146, "CheckAccessForEmailDelivery", "f:\\15.00.1497\\sources\\dev\\MailboxTransport\\src\\MailboxTransportDelivery\\StoreDriver\\MailPublicFolderPermissionHandler.cs");
				try
				{
					GrayException.MapAndReportGrayExceptions(delegate()
					{
						Participant from = mailItemDeliver.ReplayItem.From;
						if (from.RoutingType != "EX")
						{
							MailPublicFolderPermissionHandler.Diag.TraceDebug(0L, "Determined the sent user as an anonymous entity");
							isAnonymous = true;
							context = MailPublicFolderPermissionHandler.GetAnonymousClientSecurityContext();
							MailPublicFolderPermissionHandler.Diag.TraceDebug(0L, "Constructed clientSecurityContext for anonymous user");
						}
						else
						{
							MailPublicFolderPermissionHandler.Diag.TraceDebug(0L, "Determined the sent user as an authorized entity");
							byte[] valueOrDefault = from.GetValueOrDefault<byte[]>(ParticipantSchema.ParticipantSID);
							SecurityIdentifier securityIdentifier = (valueOrDefault == null) ? null : new SecurityIdentifier(valueOrDefault, 0);
							TransportMiniRecipient data = recipientCache.FindAndCacheRecipient(ProxyAddress.Parse(mailItemDeliver.MbxTransportMailItem.From.ToString())).Data;
							if (securityIdentifier == null)
							{
								context = MailPublicFolderPermissionHandler.GetUserClientSecurityContext(MailPublicFolderPermissionHandler.EveryoneSID, null);
							}
							else if (data == null)
							{
								context = MailPublicFolderPermissionHandler.GetUserClientSecurityContext(securityIdentifier, null);
							}
							else
							{
								context = MailPublicFolderPermissionHandler.GetUserClientSecurityContext(securityIdentifier, recipientSession.GetTokenSids((ADObjectId)data[ADObjectSchema.Id], AssignmentMethod.S4U));
							}
							MailPublicFolderPermissionHandler.Diag.TraceDebug(0L, "Constructed clientSecurityContext for user {0}.", new object[]
							{
								(data != null) ? data[ADRecipientSchema.PrimarySmtpAddress] : context.UserSid
							});
						}
						if (MailPublicFolderPermissionHandler.CanPostItemsToPublicFolder(mailPublicFolder, context))
						{
							accessCheckResult = AccessCheckResult.Allowed;
							return;
						}
						if (isAnonymous)
						{
							accessCheckResult = AccessCheckResult.NotAllowedAnonymous;
						}
					});
				}
				catch (GrayException ex)
				{
					string arg = string.Empty;
					if (ex.InnerException != null)
					{
						arg = ex.InnerException.Message;
					}
					accessCheckResult = AccessCheckResult.NotAllowedInternalSystemError;
					MailPublicFolderPermissionHandler.Diag.TraceError<ClientSecurityContext, string>(0L, "Access check failed on ClientSecurityContext {0} with {1}.", context, arg);
				}
				finally
				{
					if (context != null)
					{
						context.Dispose();
						context = null;
					}
				}
			}
			return accessCheckResult;
		}

		internal static ClientSecurityContext GetAnonymousClientSecurityContext()
		{
			return new ClientSecurityContext(MailPublicFolderPermissionHandler.AnonymousSecurityAccessToken, AuthzFlags.AuthzSkipTokenGroups);
		}

		internal static ClientSecurityContext GetUserClientSecurityContext(SecurityIdentifier userSid, List<string> groupSids)
		{
			if (userSid == null)
			{
				throw new ArgumentNullException("UserSid");
			}
			SecurityAccessToken securityAccessToken = new SecurityAccessToken();
			securityAccessToken.UserSid = userSid.ToString();
			if (groupSids != null)
			{
				securityAccessToken.GroupSids = (from s in groupSids
				select new SidStringAndAttributes(s, 4U)).ToArray<SidStringAndAttributes>();
			}
			else
			{
				securityAccessToken.GroupSids = MailPublicFolderPermissionHandler.DefaultSidStringAndAttributes;
			}
			return new ClientSecurityContext(securityAccessToken, AuthzFlags.AuthzSkipTokenGroups);
		}

		private static bool CanPostItemsToPublicFolder(Folder publicFolder, ClientSecurityContext userContext)
		{
			RawSecurityDescriptor rawSecurityDescriptor = publicFolder.TryGetProperty(FolderSchema.SecurityDescriptor) as RawSecurityDescriptor;
			int grantedAccess = userContext.GetGrantedAccess(rawSecurityDescriptor, AccessMask.DeleteChild);
			MailPublicFolderPermissionHandler.Diag.TraceDebug<int, Folder>(0L, "Granted access {0} for user on public folder {1}", grantedAccess, publicFolder);
			return (grantedAccess & 2) != 0;
		}

		private static readonly Trace Diag = ExTraceGlobals.MapiDeliverTracer;

		private static readonly SecurityIdentifier AnonymousSID = new SecurityIdentifier(WellKnownSidType.AnonymousSid, null);

		private static readonly SecurityIdentifier EveryoneSID = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

		private static readonly SidStringAndAttributes[] DefaultSidStringAndAttributes = new SidStringAndAttributes[]
		{
			new SidStringAndAttributes(MailPublicFolderPermissionHandler.EveryoneSID.ToString(), 4U)
		};

		private static readonly SidStringAndAttributes[] AnonymousSidStringAndAttributes = new SidStringAndAttributes[]
		{
			new SidStringAndAttributes(MailPublicFolderPermissionHandler.AnonymousSID.ToString(), 4U)
		};

		private static readonly SecurityAccessToken AnonymousSecurityAccessToken = new SecurityAccessToken
		{
			UserSid = MailPublicFolderPermissionHandler.AnonymousSID.ToString(),
			GroupSids = MailPublicFolderPermissionHandler.AnonymousSidStringAndAttributes
		};
	}
}
