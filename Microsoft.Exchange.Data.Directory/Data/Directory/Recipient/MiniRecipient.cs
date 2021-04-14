using System;
using System.Globalization;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class MiniRecipient : MiniObject
	{
		internal MiniRecipient(IRecipientSession session, PropertyBag propertyBag)
		{
			this.m_Session = session;
			this.propertyBag = (ADPropertyBag)propertyBag;
		}

		public MiniRecipient()
		{
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ADRecipient.PublicFolderMailboxObjectVersion;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return MiniRecipient.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				throw new InvalidADObjectOperationException(DirectoryStrings.ExceptionMostDerivedOnBase("MiniRecipient"));
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return Filters.DefaultRecipientFilter;
			}
		}

		public ADObjectId ArchiveDatabase
		{
			get
			{
				return (ADObjectId)this[MiniRecipientSchema.ArchiveDatabase];
			}
		}

		public Guid ArchiveGuid
		{
			get
			{
				return (Guid)this[MiniRecipientSchema.ArchiveGuid];
			}
		}

		public MultiValuedProperty<string> ArchiveName
		{
			get
			{
				return (MultiValuedProperty<string>)this[MiniRecipientSchema.ArchiveName];
			}
		}

		public ArchiveState ArchiveState
		{
			get
			{
				return (ArchiveState)this[MiniRecipientSchema.ArchiveState];
			}
		}

		public SmtpAddress JournalArchiveAddress
		{
			get
			{
				return (SmtpAddress)this[MiniRecipientSchema.JournalArchiveAddress];
			}
		}

		public ADObjectId Database
		{
			get
			{
				return (ADObjectId)this[MiniRecipientSchema.Database];
			}
		}

		public string DisplayName
		{
			get
			{
				return (string)this[MiniRecipientSchema.DisplayName];
			}
		}

		public Guid ExchangeGuid
		{
			get
			{
				return (Guid)this[MiniRecipientSchema.ExchangeGuid];
			}
		}

		public Guid? MailboxContainerGuid
		{
			get
			{
				return (Guid?)this[ADUserSchema.MailboxContainerGuid];
			}
			set
			{
				this[ADUserSchema.MailboxContainerGuid] = value;
			}
		}

		public MultiValuedProperty<Guid> AggregatedMailboxGuids
		{
			get
			{
				return (MultiValuedProperty<Guid>)this[MiniRecipientSchema.AggregatedMailboxGuids];
			}
		}

		public RawSecurityDescriptor ExchangeSecurityDescriptor
		{
			get
			{
				return (RawSecurityDescriptor)this[MiniRecipientSchema.ExchangeSecurityDescriptor];
			}
		}

		public ProxyAddress ExternalEmailAddress
		{
			get
			{
				return (ProxyAddress)this[MiniRecipientSchema.ExternalEmailAddress];
			}
		}

		public MultiValuedProperty<ADObjectId> GrantSendOnBehalfTo
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[MiniRecipientSchema.GrantSendOnBehalfTo];
			}
		}

		public MultiValuedProperty<CultureInfo> Languages
		{
			get
			{
				return (MultiValuedProperty<CultureInfo>)this[MiniRecipientSchema.Languages];
			}
		}

		public string LegacyExchangeDN
		{
			get
			{
				return (string)this[MiniRecipientSchema.LegacyExchangeDN];
			}
		}

		public SecurityIdentifier MasterAccountSid
		{
			get
			{
				return (SecurityIdentifier)this[MiniRecipientSchema.MasterAccountSid];
			}
		}

		public bool MAPIEnabled
		{
			get
			{
				return (bool)this[MiniRecipientSchema.MAPIEnabled];
			}
		}

		public bool OWAEnabled
		{
			get
			{
				return (bool)this[MiniRecipientSchema.OWAEnabled];
			}
		}

		public bool MOWAEnabled
		{
			get
			{
				return (bool)this[MiniRecipientSchema.MOWAEnabled];
			}
		}

		public ADObjectId OwaMailboxPolicy
		{
			get
			{
				return (ADObjectId)this[ADUserSchema.OwaMailboxPolicy];
			}
		}

		public ADObjectId MobileDeviceMailboxPolicy
		{
			get
			{
				return (ADObjectId)this[MiniRecipientSchema.MobileDeviceMailboxPolicy];
			}
		}

		public ADObjectId AddressBookPolicy
		{
			get
			{
				return (ADObjectId)this[ADRecipientSchema.AddressBookPolicy];
			}
		}

		public SmtpAddress PrimarySmtpAddress
		{
			get
			{
				return (SmtpAddress)this[MiniRecipientSchema.PrimarySmtpAddress];
			}
		}

		public ADObjectId QueryBaseDN
		{
			get
			{
				return (ADObjectId)this[MiniRecipientSchema.QueryBaseDN];
			}
		}

		public RecipientType RecipientType
		{
			get
			{
				return (RecipientType)this[MiniRecipientSchema.RecipientType];
			}
		}

		public RecipientTypeDetails RecipientTypeDetails
		{
			get
			{
				return (RecipientTypeDetails)this[MiniRecipientSchema.RecipientTypeDetails];
			}
		}

		public bool IsResource
		{
			get
			{
				return (bool)this[MiniRecipientSchema.IsResource];
			}
		}

		public ADObjectId DefaultPublicFolderMailbox
		{
			get
			{
				return (ADObjectId)this[MiniRecipientSchema.DefaultPublicFolderMailbox];
			}
		}

		public string ServerLegacyDN
		{
			get
			{
				return (string)this[MiniRecipientSchema.ServerLegacyDN];
			}
		}

		public SecurityIdentifier Sid
		{
			get
			{
				return (SecurityIdentifier)this[MiniRecipientSchema.Sid];
			}
		}

		public MultiValuedProperty<SecurityIdentifier> SidHistory
		{
			get
			{
				return (MultiValuedProperty<SecurityIdentifier>)this[MiniRecipientSchema.SidHistory];
			}
		}

		public string UserPrincipalName
		{
			get
			{
				return (string)this[MiniRecipientSchema.UserPrincipalName];
			}
		}

		public SmtpAddress WindowsLiveID
		{
			get
			{
				return (SmtpAddress)this[MiniRecipientSchema.WindowsLiveID];
			}
		}

		public NetID NetID
		{
			get
			{
				return (NetID)this[MiniRecipientSchema.NetID];
			}
		}

		public bool IsPersonToPersonTextMessagingEnabled
		{
			get
			{
				return (bool)this[MiniRecipientSchema.IsPersonToPersonTextMessagingEnabled];
			}
		}

		public bool IsMachineToPersonTextMessagingEnabled
		{
			get
			{
				return (bool)this[MiniRecipientSchema.IsMachineToPersonTextMessagingEnabled];
			}
		}

		public MultiValuedProperty<Capability> PersistedCapabilities
		{
			get
			{
				return (MultiValuedProperty<Capability>)this[MiniRecipientSchema.PersistedCapabilities];
			}
			set
			{
				this[MiniRecipientSchema.PersistedCapabilities] = value;
			}
		}

		internal string ExternalDirectoryObjectId
		{
			get
			{
				return (string)this[MiniRecipientSchema.ExternalDirectoryObjectId];
			}
		}

		internal Capability? SKUCapability
		{
			get
			{
				return CapabilityHelper.GetSKUCapability(this.PersistedCapabilities);
			}
		}

		public bool? SKUAssigned
		{
			get
			{
				return (bool?)this[MiniRecipientSchema.SKUAssigned];
			}
			set
			{
				this[MiniRecipientSchema.SKUAssigned] = value;
			}
		}

		public Uri SharePointUrl
		{
			get
			{
				return (Uri)this[MiniRecipientSchema.SharePointUrl];
			}
		}

		public DateTime? WhenMailboxCreated
		{
			get
			{
				return (DateTime?)this[MiniRecipientSchema.WhenMailboxCreated];
			}
		}

		public bool MailboxAuditEnabled
		{
			get
			{
				return (bool)this[MiniRecipientSchema.AuditEnabled];
			}
		}

		public EnhancedTimeSpan MailboxAuditLogAgeLimit
		{
			get
			{
				return (EnhancedTimeSpan)this[MiniRecipientSchema.AuditLogAgeLimit];
			}
		}

		public MailboxAuditOperations AuditAdminOperations
		{
			get
			{
				return (MailboxAuditOperations)this[MiniRecipientSchema.AuditAdminFlags];
			}
		}

		public MailboxAuditOperations AuditDelegateOperations
		{
			get
			{
				return (MailboxAuditOperations)this[MiniRecipientSchema.AuditDelegateFlags];
			}
		}

		public MailboxAuditOperations AuditDelegateAdminOperations
		{
			get
			{
				return (MailboxAuditOperations)this[MiniRecipientSchema.AuditDelegateAdminFlags];
			}
		}

		public MailboxAuditOperations AuditOwnerOperations
		{
			get
			{
				return (MailboxAuditOperations)this[MiniRecipientSchema.AuditOwnerFlags];
			}
		}

		public bool BypassAudit
		{
			get
			{
				return (bool)this[MiniRecipientSchema.AuditBypassEnabled];
			}
		}

		public DateTime? AuditLastAdminAccess
		{
			get
			{
				return (DateTime?)this[MiniRecipientSchema.AuditLastAdminAccess];
			}
		}

		public DateTime? AuditLastDelegateAccess
		{
			get
			{
				return (DateTime?)this[MiniRecipientSchema.AuditLastDelegateAccess];
			}
		}

		public DateTime? AuditLastExternalAccess
		{
			get
			{
				return (DateTime?)this[MiniRecipientSchema.AuditLastExternalAccess];
			}
		}

		public ProxyAddressCollection EmailAddresses
		{
			get
			{
				return (ProxyAddressCollection)this[MiniRecipientSchema.EmailAddresses];
			}
		}

		public ModernGroupObjectType ModernGroupType
		{
			get
			{
				return (ModernGroupObjectType)this[MiniRecipientSchema.ModernGroupType];
			}
		}

		public ReleaseTrack? ReleaseTrack
		{
			get
			{
				ReleaseTrack? result;
				try
				{
					result = (this.propertyBag.Contains(MiniRecipientSchema.ConfigurationXML) ? ((ReleaseTrack?)this[MiniRecipientSchema.ReleaseTrack]) : null);
				}
				catch (Exception ex)
				{
					if (!(ex is DataValidationException) && !(ex is ValueNotPresentException))
					{
						throw;
					}
					result = null;
				}
				return result;
			}
		}

		public MultiValuedProperty<SecurityIdentifier> PublicToGroupSids
		{
			get
			{
				return (MultiValuedProperty<SecurityIdentifier>)this[MiniRecipientSchema.PublicToGroupSids];
			}
		}

		public ADObjectId ThrottlingPolicy
		{
			get
			{
				return (ADObjectId)this[MiniRecipientSchema.ThrottlingPolicy];
			}
		}

		internal IThrottlingPolicy ReadThrottlingPolicy()
		{
			if (this.ThrottlingPolicy != null)
			{
				return ThrottlingPolicyCache.Singleton.Get(base.OrganizationId, this.ThrottlingPolicy);
			}
			return this.ReadDefaultThrottlingPolicy();
		}

		internal IThrottlingPolicy ReadDefaultThrottlingPolicy()
		{
			return ThrottlingPolicyCache.Singleton.Get(base.OrganizationId);
		}

		private static readonly MiniRecipientSchema schema = ObjectSchema.GetInstance<MiniRecipientSchema>();
	}
}
