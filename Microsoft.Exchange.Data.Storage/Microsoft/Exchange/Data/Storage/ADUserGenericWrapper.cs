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
	internal sealed class ADUserGenericWrapper : IGenericADUser, IFederatedIdentityParameters
	{
		public ADUserGenericWrapper(IADUser adUser)
		{
			ArgumentValidator.ThrowIfNull("adUser", adUser);
			this.adUser = adUser;
		}

		public string DisplayName
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<string>(() => this.adUser.DisplayName);
			}
		}

		public string UserPrincipalName
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<string>(() => this.adUser.UserPrincipalName);
			}
		}

		public string LegacyDn
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<string>(() => this.adUser.LegacyExchangeDN);
			}
		}

		public string Alias
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<string>(() => this.adUser.Alias);
			}
		}

		public ADObjectId DefaultPublicFolderMailbox
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ADObjectId>(() => this.adUser.DefaultPublicFolderMailbox);
			}
		}

		public SecurityIdentifier Sid
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<SecurityIdentifier>(() => this.adUser.Sid);
			}
		}

		public SecurityIdentifier MasterAccountSid
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<SecurityIdentifier>(() => this.adUser.MasterAccountSid);
			}
		}

		public IEnumerable<SecurityIdentifier> SidHistory
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MultiValuedProperty<SecurityIdentifier>>(() => this.adUser.SidHistory);
			}
		}

		public IEnumerable<ADObjectId> GrantSendOnBehalfTo
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MultiValuedProperty<ADObjectId>>(() => this.adUser.GrantSendOnBehalfTo);
			}
		}

		public IEnumerable<CultureInfo> Languages
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MultiValuedProperty<CultureInfo>>(() => this.adUser.Languages);
			}
		}

		public ADObjectId Id
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ADObjectId>(() => this.adUser.Id);
			}
		}

		public RecipientType RecipientType
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<RecipientType>(() => this.adUser.RecipientType);
			}
		}

		public RecipientTypeDetails RecipientTypeDetails
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<RecipientTypeDetails>(() => this.adUser.RecipientTypeDetails);
			}
		}

		public bool? IsResource
		{
			get
			{
				return new bool?(DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.adUser.IsResource));
			}
		}

		public SmtpAddress PrimarySmtpAddress
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<SmtpAddress>(() => this.adUser.PrimarySmtpAddress);
			}
		}

		public ProxyAddress ExternalEmailAddress
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ProxyAddress>(() => this.adUser.ExternalEmailAddress);
			}
		}

		public ProxyAddressCollection EmailAddresses
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ProxyAddressCollection>(() => this.adUser.EmailAddresses);
			}
		}

		public ADObjectId ObjectId
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ADObjectId>(() => this.adUser.Id);
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<OrganizationId>(() => this.adUser.OrganizationId);
			}
		}

		public string ImmutableId
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<string>(() => this.adUser.ImmutableId);
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<Guid>(() => this.adUser.ExchangeGuid);
			}
		}

		public ADObjectId MailboxDatabase
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ADObjectId>(() => this.adUser.Database);
			}
		}

		public DateTime? WhenMailboxCreated
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<DateTime?>(() => this.adUser.WhenMailboxCreated);
			}
		}

		public SmtpAddress WindowsLiveID
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<SmtpAddress>(() => this.adUser.WindowsLiveID);
			}
		}

		public string ImmutableIdPartial
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<string>(() => this.adUser.ImmutableIdPartial);
			}
		}

		public NetID NetId
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<NetID>(() => this.adUser.NetID);
			}
		}

		public ModernGroupObjectType ModernGroupType
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ModernGroupObjectType>(() => this.adUser.ModernGroupType);
			}
		}

		public IEnumerable<SecurityIdentifier> PublicToGroupSids
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MultiValuedProperty<SecurityIdentifier>>(() => this.adUser.PublicToGroupSids);
			}
		}

		public string ExternalDirectoryObjectId
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<string>(() => this.adUser.ExternalDirectoryObjectId);
			}
		}

		public ADObjectId ArchiveDatabase
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ADObjectId>(() => this.adUser.ArchiveDatabase);
			}
		}

		public Guid ArchiveGuid
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<Guid>(() => this.adUser.ArchiveGuid);
			}
		}

		public IEnumerable<string> ArchiveName
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MultiValuedProperty<string>>(() => this.adUser.ArchiveName);
			}
		}

		public ArchiveState ArchiveState
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ArchiveState>(() => this.adUser.ArchiveState);
			}
		}

		public ArchiveStatusFlags ArchiveStatus
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ArchiveStatusFlags>(() => this.adUser.ArchiveStatus);
			}
		}

		public SmtpDomain ArchiveDomain
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<SmtpDomain>(() => this.adUser.ArchiveDomain);
			}
		}

		public IEnumerable<Guid> AggregatedMailboxGuids
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MultiValuedProperty<Guid>>(() => this.adUser.AggregatedMailboxGuids);
			}
		}

		public Uri SharePointUrl
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<Uri>(() => this.adUser.SharePointUrl);
			}
		}

		public bool IsMapiEnabled
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.adUser.MAPIEnabled);
			}
		}

		public bool IsOwaEnabled
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.adUser.OWAEnabled);
			}
		}

		public bool IsMowaEnabled
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.adUser.MOWAEnabled);
			}
		}

		public ADObjectId ThrottlingPolicy
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ADObjectId>(() => this.adUser.ThrottlingPolicy);
			}
		}

		public ADObjectId OwaMailboxPolicy
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ADObjectId>(() => this.adUser.OwaMailboxPolicy);
			}
		}

		public ADObjectId MobileDeviceMailboxPolicy
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ADObjectId>(() => this.adUser.ActiveSyncMailboxPolicy);
			}
		}

		public ADObjectId AddressBookPolicy
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ADObjectId>(() => this.adUser.AddressBookPolicy);
			}
		}

		public bool IsPersonToPersonMessagingEnabled
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.adUser.IsPersonToPersonTextMessagingEnabled);
			}
		}

		public bool IsMachineToPersonMessagingEnabled
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.adUser.IsMachineToPersonTextMessagingEnabled);
			}
		}

		public Capability? SkuCapability
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<Capability?>(() => this.adUser.SKUCapability);
			}
		}

		public bool? SkuAssigned
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool?>(() => this.adUser.SKUAssigned);
			}
		}

		public bool IsMailboxAuditEnabled
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.adUser.MailboxAuditEnabled);
			}
		}

		public bool BypassAudit
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.adUser.BypassAudit);
			}
		}

		public EnhancedTimeSpan MailboxAuditLogAgeLimit
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<EnhancedTimeSpan>(() => this.adUser.MailboxAuditLogAgeLimit);
			}
		}

		public MailboxAuditOperations AuditAdminOperations
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MailboxAuditOperations>(() => this.adUser.AuditAdminOperations);
			}
		}

		public MailboxAuditOperations AuditDelegateOperations
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MailboxAuditOperations>(() => this.adUser.AuditDelegateOperations);
			}
		}

		public MailboxAuditOperations AuditDelegateAdminOperations
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MailboxAuditOperations>(() => this.adUser.AuditDelegateAdminOperations);
			}
		}

		public MailboxAuditOperations AuditOwnerOperations
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MailboxAuditOperations>(() => this.adUser.AuditOwnerOperations);
			}
		}

		public DateTime? AuditLastAdminAccess
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<DateTime?>(() => this.adUser.AuditLastAdminAccess);
			}
		}

		public DateTime? AuditLastDelegateAccess
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<DateTime?>(() => this.adUser.AuditLastDelegateAccess);
			}
		}

		public DateTime? AuditLastExternalAccess
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<DateTime?>(() => this.adUser.AuditLastExternalAccess);
			}
		}

		public ADObjectId QueryBaseDN
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ADObjectId>(() => this.adUser.QueryBaseDN);
			}
		}

		public SmtpAddress GetFederatedSmtpAddress()
		{
			return DirectoryExtensions.GetWithDirectoryExceptionTranslation<SmtpAddress>(new Func<SmtpAddress>(this.adUser.GetFederatedSmtpAddress));
		}

		public FederatedIdentity GetFederatedIdentity()
		{
			return DirectoryExtensions.GetWithDirectoryExceptionTranslation<FederatedIdentity>(() => FederatedIdentityHelper.GetFederatedIdentity(this.adUser));
		}

		public IEnumerable<IMailboxLocationInfo> MailboxLocations
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<IEnumerable<IMailboxLocationInfo>>(delegate
				{
					if (this.adUser.MailboxLocations == null)
					{
						return Enumerable.Empty<IMailboxLocationInfo>();
					}
					return this.adUser.MailboxLocations.GetMailboxLocations();
				});
			}
		}

		private readonly IADUser adUser;
	}
}
