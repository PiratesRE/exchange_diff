using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MiniRecipientGenericWrapper : IGenericADUser, IFederatedIdentityParameters
	{
		public MiniRecipientGenericWrapper(StorageMiniRecipient storageMiniRecipient)
		{
			ArgumentValidator.ThrowIfNull("storageMiniRecipient", storageMiniRecipient);
			this.storageMiniRecipient = storageMiniRecipient;
		}

		public string DisplayName
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<string>(() => this.storageMiniRecipient.DisplayName);
			}
		}

		public string UserPrincipalName
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<string>(() => this.storageMiniRecipient.UserPrincipalName);
			}
		}

		public string LegacyDn
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<string>(() => this.storageMiniRecipient.LegacyExchangeDN);
			}
		}

		public string Alias
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<string>(() => this.storageMiniRecipient.Alias);
			}
		}

		public ADObjectId DefaultPublicFolderMailbox
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ADObjectId>(() => this.storageMiniRecipient.DefaultPublicFolderMailbox);
			}
		}

		public SecurityIdentifier Sid
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<SecurityIdentifier>(() => this.storageMiniRecipient.Sid);
			}
		}

		public SecurityIdentifier MasterAccountSid
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<SecurityIdentifier>(() => this.storageMiniRecipient.MasterAccountSid);
			}
		}

		public IEnumerable<SecurityIdentifier> SidHistory
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MultiValuedProperty<SecurityIdentifier>>(() => this.storageMiniRecipient.SidHistory);
			}
		}

		public IEnumerable<ADObjectId> GrantSendOnBehalfTo
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MultiValuedProperty<ADObjectId>>(() => this.storageMiniRecipient.GrantSendOnBehalfTo);
			}
		}

		public IEnumerable<CultureInfo> Languages
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MultiValuedProperty<CultureInfo>>(() => this.storageMiniRecipient.Languages);
			}
		}

		public RecipientType RecipientType
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<RecipientType>(() => this.storageMiniRecipient.RecipientType);
			}
		}

		public RecipientTypeDetails RecipientTypeDetails
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<RecipientTypeDetails>(() => this.storageMiniRecipient.RecipientTypeDetails);
			}
		}

		public bool? IsResource
		{
			get
			{
				return new bool?(DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.storageMiniRecipient.IsResource));
			}
		}

		public SmtpAddress PrimarySmtpAddress
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<SmtpAddress>(() => this.storageMiniRecipient.PrimarySmtpAddress);
			}
		}

		public ProxyAddress ExternalEmailAddress
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ProxyAddress>(() => this.storageMiniRecipient.ExternalEmailAddress);
			}
		}

		public ProxyAddressCollection EmailAddresses
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ProxyAddressCollection>(() => this.storageMiniRecipient.EmailAddresses);
			}
		}

		public ADObjectId ObjectId
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ADObjectId>(() => this.storageMiniRecipient.Id);
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<OrganizationId>(() => this.storageMiniRecipient.OrganizationId);
			}
		}

		public string ImmutableId
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<string>(() => this.storageMiniRecipient.ImmutableId);
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<Guid>(() => this.storageMiniRecipient.ExchangeGuid);
			}
		}

		public ADObjectId MailboxDatabase
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ADObjectId>(() => this.storageMiniRecipient.Database);
			}
		}

		public DateTime? WhenMailboxCreated
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<DateTime?>(() => this.storageMiniRecipient.WhenMailboxCreated);
			}
		}

		public SmtpAddress WindowsLiveID
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<SmtpAddress>(() => this.storageMiniRecipient.WindowsLiveID);
			}
		}

		public string ImmutableIdPartial
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<string>(() => this.storageMiniRecipient.ImmutableIdPartial);
			}
		}

		public NetID NetId
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<NetID>(() => this.storageMiniRecipient.NetID);
			}
		}

		public ModernGroupObjectType ModernGroupType
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ModernGroupObjectType>(() => this.storageMiniRecipient.ModernGroupType);
			}
		}

		public IEnumerable<SecurityIdentifier> PublicToGroupSids
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MultiValuedProperty<SecurityIdentifier>>(() => this.storageMiniRecipient.PublicToGroupSids);
			}
		}

		public string ExternalDirectoryObjectId
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<string>(() => this.storageMiniRecipient.ExternalDirectoryObjectId);
			}
		}

		public ADObjectId ArchiveDatabase
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ADObjectId>(() => this.storageMiniRecipient.ArchiveDatabase);
			}
		}

		public Guid ArchiveGuid
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<Guid>(() => this.storageMiniRecipient.ArchiveGuid);
			}
		}

		public IEnumerable<string> ArchiveName
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MultiValuedProperty<string>>(() => this.storageMiniRecipient.ArchiveName);
			}
		}

		public ArchiveState ArchiveState
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ArchiveState>(() => this.storageMiniRecipient.ArchiveState);
			}
		}

		public ArchiveStatusFlags ArchiveStatus
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ArchiveStatusFlags>(() => this.storageMiniRecipient.ArchiveStatus);
			}
		}

		public SmtpDomain ArchiveDomain
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<SmtpDomain>(() => this.storageMiniRecipient.ArchiveDomain);
			}
		}

		public IEnumerable<Guid> AggregatedMailboxGuids
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MultiValuedProperty<Guid>>(() => this.storageMiniRecipient.AggregatedMailboxGuids);
			}
		}

		public Uri SharePointUrl
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<Uri>(() => this.storageMiniRecipient.SharePointUrl);
			}
		}

		public bool IsMapiEnabled
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.storageMiniRecipient.MAPIEnabled);
			}
		}

		public bool IsOwaEnabled
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.storageMiniRecipient.OWAEnabled);
			}
		}

		public bool IsMowaEnabled
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.storageMiniRecipient.MOWAEnabled);
			}
		}

		public ADObjectId ThrottlingPolicy
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ADObjectId>(() => this.storageMiniRecipient.ThrottlingPolicy);
			}
		}

		public ADObjectId OwaMailboxPolicy
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ADObjectId>(() => this.storageMiniRecipient.OwaMailboxPolicy);
			}
		}

		public ADObjectId MobileDeviceMailboxPolicy
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ADObjectId>(() => this.storageMiniRecipient.MobileDeviceMailboxPolicy);
			}
		}

		public ADObjectId AddressBookPolicy
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ADObjectId>(() => this.storageMiniRecipient.AddressBookPolicy);
			}
		}

		public bool IsPersonToPersonMessagingEnabled
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.storageMiniRecipient.IsPersonToPersonTextMessagingEnabled);
			}
		}

		public bool IsMachineToPersonMessagingEnabled
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.storageMiniRecipient.IsMachineToPersonTextMessagingEnabled);
			}
		}

		public Capability? SkuCapability
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<Capability?>(() => this.storageMiniRecipient.SKUCapability);
			}
		}

		public bool? SkuAssigned
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool?>(() => this.storageMiniRecipient.SKUAssigned);
			}
		}

		public bool IsMailboxAuditEnabled
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.storageMiniRecipient.MailboxAuditEnabled);
			}
		}

		public bool BypassAudit
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.storageMiniRecipient.BypassAudit);
			}
		}

		public EnhancedTimeSpan MailboxAuditLogAgeLimit
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<EnhancedTimeSpan>(() => this.storageMiniRecipient.MailboxAuditLogAgeLimit);
			}
		}

		public MailboxAuditOperations AuditAdminOperations
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MailboxAuditOperations>(() => this.storageMiniRecipient.AuditAdminOperations);
			}
		}

		public MailboxAuditOperations AuditDelegateOperations
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MailboxAuditOperations>(() => this.storageMiniRecipient.AuditDelegateOperations);
			}
		}

		public MailboxAuditOperations AuditDelegateAdminOperations
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MailboxAuditOperations>(() => this.storageMiniRecipient.AuditDelegateAdminOperations);
			}
		}

		public MailboxAuditOperations AuditOwnerOperations
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MailboxAuditOperations>(() => this.storageMiniRecipient.AuditOwnerOperations);
			}
		}

		public DateTime? AuditLastAdminAccess
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<DateTime?>(() => this.storageMiniRecipient.AuditLastAdminAccess);
			}
		}

		public DateTime? AuditLastDelegateAccess
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<DateTime?>(() => this.storageMiniRecipient.AuditLastDelegateAccess);
			}
		}

		public DateTime? AuditLastExternalAccess
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<DateTime?>(() => this.storageMiniRecipient.AuditLastExternalAccess);
			}
		}

		public ADObjectId QueryBaseDN
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ADObjectId>(() => this.storageMiniRecipient.QueryBaseDN);
			}
		}

		public SmtpAddress GetFederatedSmtpAddress()
		{
			return DirectoryExtensions.GetWithDirectoryExceptionTranslation<SmtpAddress>(new Func<SmtpAddress>(this.storageMiniRecipient.GetFederatedSmtpAddress));
		}

		public FederatedIdentity GetFederatedIdentity()
		{
			return DirectoryExtensions.GetWithDirectoryExceptionTranslation<FederatedIdentity>(new Func<FederatedIdentity>(this.storageMiniRecipient.GetFederatedIdentity));
		}

		public IEnumerable<IMailboxLocationInfo> MailboxLocations
		{
			get
			{
				return Enumerable.Empty<IMailboxLocationInfo>();
			}
		}

		private readonly StorageMiniRecipient storageMiniRecipient;
	}
}
