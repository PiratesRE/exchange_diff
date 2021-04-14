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
	internal sealed class ADSystemMailboxGenericWrapper : IGenericADUser, IFederatedIdentityParameters
	{
		public ADSystemMailboxGenericWrapper(ADSystemMailbox systemMailbox)
		{
			ArgumentValidator.ThrowIfNull("systemMailbox", systemMailbox);
			this.systemMailbox = systemMailbox;
		}

		public string DisplayName
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<string>(() => this.systemMailbox.DisplayName);
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
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<string>(() => this.systemMailbox.LegacyExchangeDN);
			}
		}

		public string Alias
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<string>(() => this.systemMailbox.Alias);
			}
		}

		public ADObjectId DefaultPublicFolderMailbox
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ADObjectId>(() => this.systemMailbox.DefaultPublicFolderMailbox);
			}
		}

		public SecurityIdentifier Sid
		{
			get
			{
				throw new InvalidOperationException("Property not supported");
			}
		}

		public SecurityIdentifier MasterAccountSid
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<SecurityIdentifier>(() => this.systemMailbox.MasterAccountSid);
			}
		}

		public IEnumerable<SecurityIdentifier> SidHistory
		{
			get
			{
				throw new InvalidOperationException("Property not supported");
			}
		}

		public IEnumerable<ADObjectId> GrantSendOnBehalfTo
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MultiValuedProperty<ADObjectId>>(() => this.systemMailbox.GrantSendOnBehalfTo);
			}
		}

		public IEnumerable<CultureInfo> Languages
		{
			get
			{
				throw new InvalidOperationException("Property not supported");
			}
		}

		public ADObjectId Id
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ADObjectId>(() => this.systemMailbox.Id);
			}
		}

		public RecipientType RecipientType
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<RecipientType>(() => this.systemMailbox.RecipientType);
			}
		}

		public RecipientTypeDetails RecipientTypeDetails
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<RecipientTypeDetails>(() => this.systemMailbox.RecipientTypeDetails);
			}
		}

		public bool? IsResource
		{
			get
			{
				return new bool?(DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.systemMailbox.IsResource));
			}
		}

		public SmtpAddress PrimarySmtpAddress
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<SmtpAddress>(() => this.systemMailbox.PrimarySmtpAddress);
			}
		}

		public ProxyAddress ExternalEmailAddress
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ProxyAddress>(() => this.systemMailbox.ExternalEmailAddress);
			}
		}

		public ProxyAddressCollection EmailAddresses
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ProxyAddressCollection>(() => this.systemMailbox.EmailAddresses);
			}
		}

		public ADObjectId ObjectId
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ADObjectId>(() => this.systemMailbox.Id);
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<OrganizationId>(() => this.systemMailbox.OrganizationId);
			}
		}

		public string ImmutableId
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<string>(() => this.systemMailbox.ImmutableId);
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<Guid>(() => this.systemMailbox.ExchangeGuid);
			}
		}

		public ADObjectId MailboxDatabase
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ADObjectId>(() => this.systemMailbox.Database);
			}
		}

		public DateTime? WhenMailboxCreated
		{
			get
			{
				throw new InvalidOperationException("Property not supported");
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
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<string>(() => this.systemMailbox.ImmutableIdPartial);
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
				throw new InvalidOperationException("Property not supported");
			}
		}

		public IEnumerable<SecurityIdentifier> PublicToGroupSids
		{
			get
			{
				throw new InvalidOperationException("Property not supported");
			}
		}

		public string ExternalDirectoryObjectId
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<string>(() => this.systemMailbox.ExternalDirectoryObjectId);
			}
		}

		public ADObjectId ArchiveDatabase
		{
			get
			{
				throw new InvalidOperationException("Property not supported");
			}
		}

		public Guid ArchiveGuid
		{
			get
			{
				throw new InvalidOperationException("Property not supported");
			}
		}

		public IEnumerable<string> ArchiveName
		{
			get
			{
				throw new InvalidOperationException("Property not supported");
			}
		}

		public ArchiveState ArchiveState
		{
			get
			{
				throw new InvalidOperationException("Property not supported");
			}
		}

		public ArchiveStatusFlags ArchiveStatus
		{
			get
			{
				throw new InvalidOperationException("Property not supported");
			}
		}

		public SmtpDomain ArchiveDomain
		{
			get
			{
				throw new InvalidOperationException("Property not supported");
			}
		}

		public IEnumerable<Guid> AggregatedMailboxGuids
		{
			get
			{
				throw new InvalidOperationException("Property not supported");
			}
		}

		public Uri SharePointUrl
		{
			get
			{
				throw new InvalidOperationException("Property not supported");
			}
		}

		public bool IsMapiEnabled
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.systemMailbox.MAPIEnabled);
			}
		}

		public bool IsOwaEnabled
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.systemMailbox.OWAEnabled);
			}
		}

		public bool IsMowaEnabled
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.systemMailbox.MOWAEnabled);
			}
		}

		public ADObjectId ThrottlingPolicy
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ADObjectId>(() => this.systemMailbox.ThrottlingPolicy);
			}
		}

		public ADObjectId OwaMailboxPolicy
		{
			get
			{
				throw new InvalidOperationException("Property not supported");
			}
		}

		public ADObjectId MobileDeviceMailboxPolicy
		{
			get
			{
				throw new InvalidOperationException("Property not supported");
			}
		}

		public ADObjectId AddressBookPolicy
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<ADObjectId>(() => this.systemMailbox.AddressBookPolicy);
			}
		}

		public bool IsPersonToPersonMessagingEnabled
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.systemMailbox.IsPersonToPersonTextMessagingEnabled);
			}
		}

		public bool IsMachineToPersonMessagingEnabled
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.systemMailbox.IsMachineToPersonTextMessagingEnabled);
			}
		}

		public Capability? SkuCapability
		{
			get
			{
				throw new InvalidOperationException("Property not supported");
			}
		}

		public bool? SkuAssigned
		{
			get
			{
				throw new InvalidOperationException("Property not supported");
			}
		}

		public bool IsMailboxAuditEnabled
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.systemMailbox.MailboxAuditEnabled);
			}
		}

		public bool BypassAudit
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<bool>(() => this.systemMailbox.BypassAudit);
			}
		}

		public EnhancedTimeSpan MailboxAuditLogAgeLimit
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<EnhancedTimeSpan>(() => this.systemMailbox.MailboxAuditLogAgeLimit);
			}
		}

		public MailboxAuditOperations AuditAdminOperations
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MailboxAuditOperations>(() => this.systemMailbox.AuditAdminOperations);
			}
		}

		public MailboxAuditOperations AuditDelegateOperations
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MailboxAuditOperations>(() => this.systemMailbox.AuditDelegateOperations);
			}
		}

		public MailboxAuditOperations AuditDelegateAdminOperations
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MailboxAuditOperations>(() => this.systemMailbox.AuditDelegateAdminOperations);
			}
		}

		public MailboxAuditOperations AuditOwnerOperations
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<MailboxAuditOperations>(() => this.systemMailbox.AuditOwnerOperations);
			}
		}

		public DateTime? AuditLastAdminAccess
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<DateTime?>(() => this.systemMailbox.AuditLastAdminAccess);
			}
		}

		public DateTime? AuditLastDelegateAccess
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<DateTime?>(() => this.systemMailbox.AuditLastDelegateAccess);
			}
		}

		public DateTime? AuditLastExternalAccess
		{
			get
			{
				return DirectoryExtensions.GetWithDirectoryExceptionTranslation<DateTime?>(() => this.systemMailbox.AuditLastExternalAccess);
			}
		}

		public ADObjectId QueryBaseDN
		{
			get
			{
				throw new InvalidOperationException("Property not supported");
			}
		}

		public SmtpAddress GetFederatedSmtpAddress()
		{
			throw new InvalidOperationException("Operation not supported");
		}

		public FederatedIdentity GetFederatedIdentity()
		{
			throw new InvalidOperationException("Operation not supported");
		}

		public IEnumerable<IMailboxLocationInfo> MailboxLocations
		{
			get
			{
				return Enumerable.Empty<IMailboxLocationInfo>();
			}
		}

		private readonly ADSystemMailbox systemMailbox;
	}
}
