using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Authentication;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SharingProviderHandlerExternal : SharingProviderHandler
	{
		internal SharingProviderHandlerExternal()
		{
		}

		internal override void FillSharingMessageProvider(SharingContext context, SharingMessageProvider sharingMessageProvider)
		{
			sharingMessageProvider.FolderId = context.FolderEwsId;
			sharingMessageProvider.EncryptedSharedFolderDataCollection = context.EncryptedSharedFolderDataCollection;
		}

		internal override void ParseSharingMessageProvider(SharingContext context, SharingMessageProvider sharingMessageProvider)
		{
			context.FolderEwsId = sharingMessageProvider.FolderId;
			context.EncryptedSharedFolderDataCollection = sharingMessageProvider.EncryptedSharedFolderDataCollection;
		}

		protected override bool InternalValidateCompatibility(Folder folderToShare)
		{
			SharingDataType sharingDataType = SharingDataType.FromContainerClass(folderToShare.ClassName);
			return sharingDataType != null && sharingDataType.IsExternallySharable;
		}

		protected override ValidRecipient InternalCheckOneRecipient(ADRecipient mailboxOwner, string recipient, IRecipientSession recipientSession)
		{
			ADRecipient adrecipient = recipientSession.FindByProxyAddress(new SmtpProxyAddress(recipient, false));
			if (adrecipient == null)
			{
				ExTraceGlobals.SharingTracer.TraceDebug<ADRecipient, string>((long)this.GetHashCode(), "{0}: {1} is NOT found from AD in {0}'s organization.", mailboxOwner, recipient);
			}
			else
			{
				ExTraceGlobals.SharingTracer.TraceDebug<ADRecipient, string>((long)this.GetHashCode(), "{0}: {1} is found from AD in {0}'s organization.", mailboxOwner, recipient);
			}
			if (this.IsValidExternalRecipient(mailboxOwner, recipient, adrecipient))
			{
				return new ValidRecipient(recipient, adrecipient);
			}
			return null;
		}

		protected override PerformInvitationResults InternalPerformInvitation(MailboxSession mailboxSession, SharingContext context, ValidRecipient[] recipients, IFrontEndLocator frontEndLocator)
		{
			ExternalAuthentication current = ExternalAuthentication.GetCurrent();
			if (!current.Enabled)
			{
				ExTraceGlobals.SharingTracer.TraceError<string>((long)this.GetHashCode(), "{0}: The organization is not federated for external sharing.", context.InitiatorSmtpAddress);
				return new PerformInvitationResults(new InvalidSharingRecipientsException(ValidRecipient.ConvertToStringArray(recipients), new OrganizationNotFederatedException()));
			}
			SharedFolderDataEncryption sharedFolderDataEncryption = new SharedFolderDataEncryption(current);
			string text = StoreId.StoreIdToEwsId(mailboxSession.MailboxOwner.MailboxInfo.MailboxGuid, context.FolderId);
			PerformInvitationResults result;
			using (ExternalUserCollection externalUsers = mailboxSession.GetExternalUsers())
			{
				PerformInvitationResults performInvitationResults = null;
				EncryptionResults encryptionResults = null;
				Exception ex = null;
				try
				{
					encryptionResults = sharedFolderDataEncryption.Encrypt(mailboxSession.MailboxOwner, mailboxSession.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid), externalUsers, recipients, context.InitiatorSmtpAddress, context.FolderClass, text, frontEndLocator);
				}
				catch (UserWithoutFederatedProxyAddressException ex2)
				{
					ex = ex2;
				}
				catch (InvalidFederatedOrganizationIdException ex3)
				{
					ex = ex3;
				}
				catch (StoragePermanentException ex4)
				{
					ex = ex4;
				}
				if (ex != null)
				{
					ExTraceGlobals.SharingTracer.TraceError<string, Exception>((long)this.GetHashCode(), "{0}: Error occurred when trying to encrypt. Exception = {1}", context.InitiatorSmtpAddress, ex);
					result = new PerformInvitationResults(new InvalidSharingRecipientsException(ValidRecipient.ConvertToStringArray(recipients), ex));
				}
				else
				{
					if (encryptionResults.InvalidRecipients != null && encryptionResults.InvalidRecipients.Length > 0)
					{
						InvalidSharingRecipientsException exception = new InvalidSharingRecipientsException(encryptionResults.InvalidRecipients);
						if (encryptionResults.InvalidRecipients.Length == recipients.Length)
						{
							return new PerformInvitationResults(exception);
						}
						performInvitationResults = new PerformInvitationResults(recipients, exception);
						recipients = performInvitationResults.SucceededRecipients;
					}
					else
					{
						performInvitationResults = new PerformInvitationResults(recipients);
					}
					PermissionLevel permissionLevel = this.GetPermissionLevel(context);
					FreeBusyAccess? freeBusy = this.GetFreeBusy(context);
					using (FolderPermissionContext current2 = FolderPermissionContext.GetCurrent(mailboxSession, context))
					{
						foreach (ValidRecipient validRecipient in recipients)
						{
							PermissionSecurityPrincipal principal = this.CreatePermissionSecurityPrincipal(validRecipient.SmtpAddress, externalUsers);
							current2.AddOrChangePermission(principal, permissionLevel, freeBusy);
							ExternalUser externalUser = externalUsers.FindReachUserWithOriginalSmtpAddress(new SmtpAddress(validRecipient.SmtpAddress));
							if (externalUser != null)
							{
								current2.RemovePermission(new PermissionSecurityPrincipal(externalUser));
							}
						}
					}
					context.FolderEwsId = text;
					context.EncryptedSharedFolderDataCollection = encryptionResults.EncryptedSharedFolderDataCollection;
					result = performInvitationResults;
				}
			}
			return result;
		}

		protected override void InternalPerformRevocation(MailboxSession mailboxSession, SharingContext context)
		{
			using (FolderPermissionContext current = FolderPermissionContext.GetCurrent(mailboxSession, context))
			{
				using (ExternalUserCollection externalUsers = mailboxSession.GetExternalUsers())
				{
					PermissionSecurityPrincipal permissionSecurityPrincipal = this.CreatePermissionSecurityPrincipal(context.InitiatorSmtpAddress, externalUsers);
					if (permissionSecurityPrincipal != null)
					{
						current.RemovePermission(permissionSecurityPrincipal);
					}
				}
			}
		}

		protected override SubscribeResults InternalPerformSubscribe(MailboxSession mailboxSession, SharingContext context)
		{
			ADRecipient mailboxOwner = DirectoryHelper.ReadADRecipient(mailboxSession.MailboxOwner.MailboxInfo.MailboxGuid, mailboxSession.MailboxOwner.MailboxInfo.IsArchive, mailboxSession.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid));
			SharedFolderData matchingSharedFolderData = this.GetMatchingSharedFolderData(context, mailboxOwner);
			if (context.InitiatorSmtpAddress == null)
			{
				throw new InvalidSharingDataException("InitiatorSmtpAddress", string.Empty);
			}
			IdAndName idAndName = null;
			StoreObjectId storeObjectId = null;
			using (SharingSubscriptionManager sharingSubscriptionManager = new SharingSubscriptionManager(mailboxSession))
			{
				SharingSubscriptionData sharingSubscriptionData = this.CreateSubscriptionData(mailboxOwner, context, matchingSharedFolderData);
				SharingFolderManager sharingFolderManager = new SharingFolderManager(mailboxSession);
				SharingSubscriptionData existing = sharingSubscriptionManager.GetExisting(sharingSubscriptionData.Key);
				if (existing != null)
				{
					existing.CopyFrom(sharingSubscriptionData);
				}
				SharingSubscriptionData sharingSubscriptionData2 = existing ?? sharingSubscriptionData;
				idAndName = sharingFolderManager.EnsureFolder(sharingSubscriptionData2);
				if (sharingSubscriptionData2.LocalFolderId == null || !sharingSubscriptionData2.LocalFolderId.Equals(idAndName.Id))
				{
					storeObjectId = (sharingSubscriptionData2.LocalFolderId = idAndName.Id);
				}
				SharingSubscriptionData sharingSubscriptionData3 = sharingSubscriptionManager.CreateOrUpdate(sharingSubscriptionData2, false);
				if (!sharingSubscriptionData2.LocalFolderId.Equals(sharingSubscriptionData3.LocalFolderId))
				{
					idAndName = sharingFolderManager.GetFolder(sharingSubscriptionData3);
				}
			}
			return new SubscribeResultsExternal(context.DataType, context.InitiatorSmtpAddress, context.InitiatorName, context.FolderName, matchingSharedFolderData.FolderId, idAndName.Id, storeObjectId != null, idAndName.Name);
		}

		private static SharedFolderDataRecipient TryFindMatchingRecipient(SharedFolderData sharedFolderData, ADRecipient mailboxOwner)
		{
			foreach (SharedFolderDataRecipient sharedFolderDataRecipient in sharedFolderData.Recipients)
			{
				if (mailboxOwner.IsAnyAddressMatched(new string[]
				{
					sharedFolderDataRecipient.SmtpAddress
				}))
				{
					return sharedFolderDataRecipient;
				}
			}
			return null;
		}

		private static bool IsOrganizationHasOrganizationRelationshipWithDomain(OrganizationId organizationId, string domain)
		{
			OrganizationIdCacheValue organizationIdCacheValue = OrganizationIdCache.Singleton.Get(organizationId);
			OrganizationRelationship organizationRelationship = organizationIdCacheValue.GetOrganizationRelationship(domain);
			return organizationRelationship != null;
		}

		private static bool IsOrganizationHasOrganizationRelationshipWithRecipient(OrganizationId organizationId, ADRecipient recipient)
		{
			if (!(recipient.ExternalEmailAddress != null))
			{
				foreach (ProxyAddress proxyAddress in recipient.EmailAddresses)
				{
					SmtpProxyAddress smtpProxyAddress = proxyAddress as SmtpProxyAddress;
					if (smtpProxyAddress != null && SharingProviderHandlerExternal.IsOrganizationHasOrganizationRelationshipWithDomain(organizationId, new SmtpAddress(smtpProxyAddress.SmtpAddress).Domain))
					{
						return true;
					}
				}
				return false;
			}
			SmtpProxyAddress smtpProxyAddress2 = recipient.ExternalEmailAddress as SmtpProxyAddress;
			if (smtpProxyAddress2 != null && SharingProviderHandlerExternal.IsOrganizationHasOrganizationRelationshipWithDomain(organizationId, new SmtpAddress(smtpProxyAddress2.SmtpAddress).Domain))
			{
				return true;
			}
			return false;
		}

		private bool IsValidExternalRecipient(ADRecipient mailboxOwner, string recipient, ADRecipient adRecipient)
		{
			string domain = new SmtpAddress(recipient).Domain;
			if (adRecipient == null)
			{
				if (SharingProviderHandlerExternal.IsOrganizationHasOrganizationRelationshipWithDomain(mailboxOwner.OrganizationId, domain))
				{
					ExTraceGlobals.SharingTracer.TraceDebug<ADRecipient, string, string>((long)this.GetHashCode(), "{0}: Found OrganizationRelationship with domain {1}, so {2} this is valid external recipient.", mailboxOwner, domain, recipient);
					return true;
				}
			}
			else if (SharingProviderHandlerExternal.IsOrganizationHasOrganizationRelationshipWithRecipient(mailboxOwner.OrganizationId, adRecipient))
			{
				ExTraceGlobals.SharingTracer.TraceDebug<ADRecipient, string>((long)this.GetHashCode(), "{0}: Found OrganizationRelationship that matches one of proxy addresses of {1}, this is valid external recipient.", mailboxOwner, recipient);
				return true;
			}
			ExTraceGlobals.SharingTracer.TraceDebug<ADRecipient, string>((long)this.GetHashCode(), "{0}: Found NO OrganizationRelationship with {1}.", mailboxOwner, recipient);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.FullyConsistent, mailboxOwner.OrganizationId.ToADSessionSettings(), 485, "IsValidExternalRecipient", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Sharing\\SharingProviderHandlerExternal.cs");
			AcceptedDomain acceptedDomainByDomainName = tenantOrTopologyConfigurationSession.GetAcceptedDomainByDomainName(domain);
			if (acceptedDomainByDomainName != null)
			{
				ExTraceGlobals.SharingTracer.TraceDebug<ADRecipient, string>((long)this.GetHashCode(), "{0}: found AcceptedDomain for {1}'s domain, this is NOT valid external recipient.", mailboxOwner, recipient);
				return false;
			}
			ExTraceGlobals.SharingTracer.TraceDebug<ADRecipient, string>((long)this.GetHashCode(), "{0}: found no AcceptedDomain for {1}'1 domain, this is a valid external recipient.", mailboxOwner, recipient);
			return true;
		}

		private PermissionSecurityPrincipal CreatePermissionSecurityPrincipal(string recipient, ExternalUserCollection externalUserCollection)
		{
			ExternalUser externalUser = externalUserCollection.FindExternalUser(new SmtpAddress(recipient));
			if (externalUser == null)
			{
				return null;
			}
			return new PermissionSecurityPrincipal(externalUser);
		}

		private PermissionLevel GetPermissionLevel(SharingContext context)
		{
			if (context.DataType != SharingDataType.Calendar || context.SharingDetail == SharingContextDetailLevel.FullDetails || context.SharingDetail == SharingContextDetailLevel.Editor)
			{
				return PermissionLevel.Reviewer;
			}
			return PermissionLevel.None;
		}

		private FreeBusyAccess? GetFreeBusy(SharingContext context)
		{
			if (context.DataType != SharingDataType.Calendar)
			{
				return null;
			}
			SharingContextDetailLevel sharingDetail = context.SharingDetail;
			switch (sharingDetail)
			{
			case SharingContextDetailLevel.AvailabilityOnly:
				return new FreeBusyAccess?(FreeBusyAccess.Basic);
			case SharingContextDetailLevel.Limited:
			case SharingContextDetailLevel.FullDetails:
				break;
			case (SharingContextDetailLevel)3:
				goto IL_48;
			default:
				if (sharingDetail != SharingContextDetailLevel.Editor)
				{
					goto IL_48;
				}
				break;
			}
			return new FreeBusyAccess?(FreeBusyAccess.Details);
			IL_48:
			throw new ArgumentOutOfRangeException("context");
		}

		private SharedFolderData GetMatchingSharedFolderData(SharingContext context, ADRecipient mailboxOwner)
		{
			SharedFolderDataEncryption sharedFolderDataEncryption = new SharedFolderDataEncryption(ExternalAuthentication.GetCurrent());
			bool flag = false;
			ExTraceGlobals.SharingTracer.TraceDebug<ADRecipient>((long)this.GetHashCode(), "{0}: Decrypting the encrypted shared folder data.", mailboxOwner);
			foreach (EncryptedSharedFolderData encryptedSharedFolderData in context.EncryptedSharedFolderDataCollection)
			{
				SharedFolderData sharedFolderData = sharedFolderDataEncryption.TryDecrypt(encryptedSharedFolderData);
				if (sharedFolderData != null)
				{
					if (SharingProviderHandlerExternal.TryFindMatchingRecipient(sharedFolderData, mailboxOwner) != null)
					{
						ExTraceGlobals.SharingTracer.TraceDebug<ADRecipient>((long)this.GetHashCode(), "{0}: Get the decrypted shared folder data.", mailboxOwner);
						return sharedFolderData;
					}
					flag = true;
				}
			}
			ExTraceGlobals.SharingTracer.TraceError<ADRecipient>((long)this.GetHashCode(), "{0}: Failed to decrypt shared folder data.", mailboxOwner);
			if (flag)
			{
				throw new InvalidExternalSharingSubscriberException();
			}
			throw new InvalidSharingDataException("SharedFolderData", string.Empty);
		}

		private SharingSubscriptionData CreateSubscriptionData(ADRecipient mailboxOwner, SharingContext context, SharedFolderData sharedFolderData)
		{
			SharedFolderDataRecipient matchingRecipient = this.GetMatchingRecipient(sharedFolderData, mailboxOwner);
			if (sharedFolderData.FederationUri == null)
			{
				throw new InvalidSharingDataException("FederationUri", string.Empty);
			}
			if (!Uri.IsWellFormedUriString(sharedFolderData.FederationUri, UriKind.Absolute))
			{
				throw new InvalidSharingDataException("FederationUri", sharedFolderData.FederationUri);
			}
			if (sharedFolderData.SharingUrl == null)
			{
				throw new InvalidSharingDataException("SharingUrl", string.Empty);
			}
			if (!Uri.IsWellFormedUriString(sharedFolderData.SharingUrl, UriKind.Absolute))
			{
				throw new InvalidSharingDataException("SharingUrl", sharedFolderData.SharingUrl);
			}
			return new SharingSubscriptionData(sharedFolderData.DataType, context.InitiatorSmtpAddress, context.InitiatorName, sharedFolderData.FolderId, context.FolderName, context.IsPrimary, new Uri(sharedFolderData.FederationUri), new Uri(sharedFolderData.SharingUrl), null, matchingRecipient.SharingKey, matchingRecipient.SmtpAddress);
		}

		private SharedFolderDataRecipient GetMatchingRecipient(SharedFolderData sharedFolderData, ADRecipient mailboxOwner)
		{
			SharedFolderDataRecipient sharedFolderDataRecipient = SharingProviderHandlerExternal.TryFindMatchingRecipient(sharedFolderData, mailboxOwner);
			if (sharedFolderDataRecipient != null)
			{
				return sharedFolderDataRecipient;
			}
			ExTraceGlobals.SharingTracer.TraceError<ADRecipient>((long)this.GetHashCode(), "{0}: The sharing message is not intended to this subscriber.", mailboxOwner);
			throw new InvalidExternalSharingSubscriberException();
		}
	}
}
