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
	internal sealed class ADGroupGenericWrapper : IGenericADUser, IFederatedIdentityParameters
	{
		public ADGroupGenericWrapper(IADGroup adGroup)
		{
			ArgumentValidator.ThrowIfNull("adGroup", adGroup);
			this.adGroup = adGroup;
		}

		public string DisplayName
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<string>(() => this.adGroup.DisplayName);
			}
		}

		public string UserPrincipalName
		{
			get
			{
				throw new InvalidOperationException("Property not supported");
			}
		}

		public string LegacyDn
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<string>(() => this.adGroup.LegacyExchangeDN);
			}
		}

		public string Alias
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<string>(() => this.adGroup.Alias);
			}
		}

		public ADObjectId DefaultPublicFolderMailbox
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ADObjectId>(() => this.adGroup.DefaultPublicFolderMailbox);
			}
		}

		public SecurityIdentifier Sid
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<SecurityIdentifier>(() => this.adGroup.Sid);
			}
		}

		public SecurityIdentifier MasterAccountSid
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<SecurityIdentifier>(() => this.adGroup.MasterAccountSid);
			}
		}

		public IEnumerable<SecurityIdentifier> SidHistory
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MultiValuedProperty<SecurityIdentifier>>(() => this.adGroup.SidHistory);
			}
		}

		public IEnumerable<ADObjectId> GrantSendOnBehalfTo
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MultiValuedProperty<ADObjectId>>(() => this.adGroup.GrantSendOnBehalfTo);
			}
		}

		public IEnumerable<CultureInfo> Languages
		{
			get
			{
				return new CultureInfo[]
				{
					new CultureInfo("en-us")
				};
			}
		}

		public ADObjectId Id
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ADObjectId>(() => this.adGroup.Id);
			}
		}

		public RecipientType RecipientType
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<RecipientType>(() => this.adGroup.RecipientType);
			}
		}

		public RecipientTypeDetails RecipientTypeDetails
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<RecipientTypeDetails>(() => this.adGroup.RecipientTypeDetails);
			}
		}

		public bool? IsResource
		{
			get
			{
				return new bool?(DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.adGroup.IsResource));
			}
		}

		public SmtpAddress PrimarySmtpAddress
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<SmtpAddress>(() => this.adGroup.PrimarySmtpAddress);
			}
		}

		public ProxyAddress ExternalEmailAddress
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ProxyAddress>(() => this.adGroup.ExternalEmailAddress);
			}
		}

		public ProxyAddressCollection EmailAddresses
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ProxyAddressCollection>(() => this.adGroup.EmailAddresses);
			}
		}

		public ADObjectId ObjectId
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ADObjectId>(() => this.adGroup.Id);
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<OrganizationId>(() => this.adGroup.OrganizationId);
			}
		}

		public string ImmutableId
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<string>(() => this.adGroup.ImmutableId);
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<Guid>(() => this.adGroup.ExchangeGuid);
			}
		}

		public ADObjectId MailboxDatabase
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ADObjectId>(() => this.adGroup.Database);
			}
		}

		public DateTime? WhenMailboxCreated
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<DateTime?>(() => this.adGroup.WhenMailboxCreated);
			}
		}

		public SmtpAddress WindowsLiveID
		{
			get
			{
				throw new InvalidOperationException("Property not supported");
			}
		}

		public string ImmutableIdPartial
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<string>(() => this.adGroup.ImmutableIdPartial);
			}
		}

		public NetID NetId
		{
			get
			{
				throw new InvalidOperationException("Property not supported");
			}
		}

		public ModernGroupObjectType ModernGroupType
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ModernGroupObjectType>(() => this.adGroup.ModernGroupType);
			}
		}

		public IEnumerable<SecurityIdentifier> PublicToGroupSids
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MultiValuedProperty<SecurityIdentifier>>(() => this.adGroup.PublicToGroupSids);
			}
		}

		public string ExternalDirectoryObjectId
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<string>(() => this.adGroup.ExternalDirectoryObjectId);
			}
		}

		public ADObjectId ArchiveDatabase
		{
			get
			{
				return null;
			}
		}

		public Guid ArchiveGuid
		{
			get
			{
				return Guid.Empty;
			}
		}

		public IEnumerable<string> ArchiveName
		{
			get
			{
				return null;
			}
		}

		public ArchiveState ArchiveState
		{
			get
			{
				return ArchiveState.None;
			}
		}

		public ArchiveStatusFlags ArchiveStatus
		{
			get
			{
				return ArchiveStatusFlags.None;
			}
		}

		public SmtpDomain ArchiveDomain
		{
			get
			{
				return null;
			}
		}

		public IEnumerable<Guid> AggregatedMailboxGuids
		{
			get
			{
				return null;
			}
		}

		public Uri SharePointUrl
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<Uri>(() => this.adGroup.SharePointUrl);
			}
		}

		public bool IsMapiEnabled
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.adGroup.MAPIEnabled);
			}
		}

		public bool IsOwaEnabled
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.adGroup.OWAEnabled);
			}
		}

		public bool IsMowaEnabled
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.adGroup.MOWAEnabled);
			}
		}

		public ADObjectId ThrottlingPolicy
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ADObjectId>(() => this.adGroup.ThrottlingPolicy);
			}
		}

		public ADObjectId OwaMailboxPolicy
		{
			get
			{
				return null;
			}
		}

		public ADObjectId MobileDeviceMailboxPolicy
		{
			get
			{
				return null;
			}
		}

		public ADObjectId AddressBookPolicy
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ADObjectId>(() => this.adGroup.AddressBookPolicy);
			}
		}

		public bool IsPersonToPersonMessagingEnabled
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.adGroup.IsPersonToPersonTextMessagingEnabled);
			}
		}

		public bool IsMachineToPersonMessagingEnabled
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.adGroup.IsMachineToPersonTextMessagingEnabled);
			}
		}

		public Capability? SkuCapability
		{
			get
			{
				return null;
			}
		}

		public bool? SkuAssigned
		{
			get
			{
				return null;
			}
		}

		public bool IsMailboxAuditEnabled
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.adGroup.MailboxAuditEnabled);
			}
		}

		public bool BypassAudit
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.adGroup.BypassAudit);
			}
		}

		public EnhancedTimeSpan MailboxAuditLogAgeLimit
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<EnhancedTimeSpan>(() => this.adGroup.MailboxAuditLogAgeLimit);
			}
		}

		public MailboxAuditOperations AuditAdminOperations
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MailboxAuditOperations>(() => this.adGroup.AuditAdminOperations);
			}
		}

		public MailboxAuditOperations AuditDelegateOperations
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MailboxAuditOperations>(() => this.adGroup.AuditDelegateOperations);
			}
		}

		public MailboxAuditOperations AuditDelegateAdminOperations
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MailboxAuditOperations>(() => this.adGroup.AuditDelegateAdminOperations);
			}
		}

		public MailboxAuditOperations AuditOwnerOperations
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MailboxAuditOperations>(() => this.adGroup.AuditOwnerOperations);
			}
		}

		public DateTime? AuditLastAdminAccess
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<DateTime?>(() => this.adGroup.AuditLastAdminAccess);
			}
		}

		public DateTime? AuditLastDelegateAccess
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<DateTime?>(() => this.adGroup.AuditLastDelegateAccess);
			}
		}

		public DateTime? AuditLastExternalAccess
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<DateTime?>(() => this.adGroup.AuditLastExternalAccess);
			}
		}

		public ADObjectId QueryBaseDN
		{
			get
			{
				return null;
			}
		}

		public SmtpAddress GetFederatedSmtpAddress()
		{
			return SmtpAddress.Empty;
		}

		public FederatedIdentity GetFederatedIdentity()
		{
			return null;
		}

		public IEnumerable<IMailboxLocationInfo> MailboxLocations
		{
			get
			{
				if (this.adGroup.ExchangeGuid != Guid.Empty && this.adGroup.Database != null)
				{
					return DirectoryExtensions.GetWithDirectoryExceptionTranslation<IMailboxLocationInfo[]>(() => new IMailboxLocationInfo[]
					{
						new MailboxLocationInfo(this.adGroup.ExchangeGuid, this.adGroup.Database, MailboxLocationType.Primary)
					});
				}
				return Enumerable.Empty<IMailboxLocationInfo>();
			}
		}

		private readonly IADGroup adGroup;
	}
}
