using System;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Serializable]
	public abstract class ADMailboxRecipient : ADRecipient, IADMailboxRecipient, IADRecipient, IADObject, IADRawEntry, IConfigurable, IPropertyBag, IReadOnlyPropertyBag, IADMailStorage, IADSecurityPrincipal
	{
		internal ADMailboxRecipient(IRecipientSession session, PropertyBag propertyBag) : base(session, propertyBag)
		{
		}

		internal ADMailboxRecipient()
		{
		}

		public ADObjectId Database
		{
			get
			{
				return (ADObjectId)this[ADMailboxRecipientSchema.Database];
			}
			set
			{
				this[ADMailboxRecipientSchema.Database] = value;
			}
		}

		public DeletedItemRetention DeletedItemFlags
		{
			get
			{
				return (DeletedItemRetention)this[ADMailboxRecipientSchema.DeletedItemFlags];
			}
			set
			{
				this[ADMailboxRecipientSchema.DeletedItemFlags] = value;
			}
		}

		public bool DeliverToMailboxAndForward
		{
			get
			{
				return (bool)this[ADMailboxRecipientSchema.DeliverToMailboxAndForward];
			}
			set
			{
				this[ADMailboxRecipientSchema.DeliverToMailboxAndForward] = value;
			}
		}

		public Guid ExchangeGuid
		{
			get
			{
				return (Guid)this[ADMailboxRecipientSchema.ExchangeGuid];
			}
			set
			{
				this[ADMailboxRecipientSchema.ExchangeGuid] = value;
			}
		}

		public RawSecurityDescriptor ExchangeSecurityDescriptor
		{
			get
			{
				return (RawSecurityDescriptor)this[ADMailboxRecipientSchema.ExchangeSecurityDescriptor];
			}
			set
			{
				this[ADMailboxRecipientSchema.ExchangeSecurityDescriptor] = value;
			}
		}

		public ExternalOofOptions ExternalOofOptions
		{
			get
			{
				return (ExternalOofOptions)this[ADMailboxRecipientSchema.ExternalOofOptions];
			}
			set
			{
				this[ADMailboxRecipientSchema.ExternalOofOptions] = value;
			}
		}

		public bool IsMailboxEnabled
		{
			get
			{
				return (bool)this[ADMailboxRecipientSchema.IsMailboxEnabled];
			}
		}

		public Unlimited<ByteQuantifiedSize> IssueWarningQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ADMailboxRecipientSchema.IssueWarningQuota];
			}
			set
			{
				this[ADMailboxRecipientSchema.IssueWarningQuota] = value;
			}
		}

		public ADObjectId OfflineAddressBook
		{
			get
			{
				return (ADObjectId)this[ADMailboxRecipientSchema.OfflineAddressBook];
			}
			set
			{
				this[ADMailboxRecipientSchema.OfflineAddressBook] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> ProhibitSendQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ADMailboxRecipientSchema.ProhibitSendQuota];
			}
			set
			{
				this[ADMailboxRecipientSchema.ProhibitSendQuota] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> ProhibitSendReceiveQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ADMailboxRecipientSchema.ProhibitSendReceiveQuota];
			}
			set
			{
				this[ADMailboxRecipientSchema.ProhibitSendReceiveQuota] = value;
			}
		}

		public EnhancedTimeSpan RetainDeletedItemsFor
		{
			get
			{
				return (EnhancedTimeSpan)this[ADMailboxRecipientSchema.RetainDeletedItemsFor];
			}
			set
			{
				this[ADMailboxRecipientSchema.RetainDeletedItemsFor] = value;
			}
		}

		public ByteQuantifiedSize RulesQuota
		{
			get
			{
				return (ByteQuantifiedSize)this[ADMailboxRecipientSchema.RulesQuota];
			}
			set
			{
				this[ADMailboxRecipientSchema.RulesQuota] = value;
			}
		}

		public string ServerLegacyDN
		{
			get
			{
				return (string)this[ADMailboxRecipientSchema.ServerLegacyDN];
			}
			set
			{
				this[ADMailboxRecipientSchema.ServerLegacyDN] = value;
			}
		}

		public string ServerName
		{
			get
			{
				return (string)this[ADMailboxRecipientSchema.ServerName];
			}
		}

		public bool? UseDatabaseQuotaDefaults
		{
			get
			{
				return (bool?)this[ADMailboxRecipientSchema.UseDatabaseQuotaDefaults];
			}
			set
			{
				this[ADMailboxRecipientSchema.UseDatabaseQuotaDefaults] = value;
			}
		}

		public bool IsSecurityPrincipal
		{
			get
			{
				return (bool)this[ADMailboxRecipientSchema.IsSecurityPrincipal];
			}
		}

		public string SamAccountName
		{
			get
			{
				return (string)this[ADMailboxRecipientSchema.SamAccountName];
			}
			set
			{
				this[ADMailboxRecipientSchema.SamAccountName] = value;
			}
		}

		public SecurityIdentifier Sid
		{
			get
			{
				return (SecurityIdentifier)this[ADMailboxRecipientSchema.Sid];
			}
		}

		public MultiValuedProperty<SecurityIdentifier> SidHistory
		{
			get
			{
				return (MultiValuedProperty<SecurityIdentifier>)this[ADMailboxRecipientSchema.SidHistory];
			}
		}

		public ModernGroupObjectType ModernGroupType
		{
			get
			{
				return (ModernGroupObjectType)this[ADRecipientSchema.ModernGroupType];
			}
			set
			{
				this[ADRecipientSchema.ModernGroupType] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> DelegateListLink
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADMailboxRecipientSchema.DelegateListLink];
			}
			internal set
			{
				this[ADMailboxRecipientSchema.DelegateListLink] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> DelegateListBL
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADMailboxRecipientSchema.DelegateListBL];
			}
			internal set
			{
				this[ADMailboxRecipientSchema.DelegateListBL] = value;
			}
		}

		public MultiValuedProperty<SecurityIdentifier> PublicToGroupSids
		{
			get
			{
				return (MultiValuedProperty<SecurityIdentifier>)this[ADMailboxRecipientSchema.PublicToGroupSids];
			}
		}

		public MultiValuedProperty<string> SharePointResources
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADMailboxRecipientSchema.SharePointResources];
			}
			internal set
			{
				this[ADMailboxRecipientSchema.SharePointResources] = value;
			}
		}

		public string SharePointSiteUrl
		{
			get
			{
				return (string)this[ADMailboxRecipientSchema.GroupMailboxSharePointSiteUrl];
			}
		}

		public string SharePointDocumentsUrl
		{
			get
			{
				return (string)this[ADMailboxRecipientSchema.GroupMailboxSharePointDocumentsUrl];
			}
		}

		public Uri SharePointUrl
		{
			get
			{
				return (Uri)this[ADMailboxRecipientSchema.SharePointUrl];
			}
			internal set
			{
				this[ADMailboxRecipientSchema.SharePointUrl] = value;
			}
		}

		public DateTime? WhenMailboxCreated
		{
			get
			{
				return (DateTime?)this[ADMailboxRecipientSchema.WhenMailboxCreated];
			}
		}

		public string YammerGroupAddress
		{
			get
			{
				return this[ADMailboxRecipientSchema.YammerGroupAddress] as string;
			}
			set
			{
				this[ADMailboxRecipientSchema.YammerGroupAddress] = value;
			}
		}

		internal bool SetWhenMailboxCreatedIfNotSet()
		{
			if (this.WhenMailboxCreated == null)
			{
				this[ADMailboxRecipientSchema.WhenMailboxCreated] = DateTime.UtcNow;
				return true;
			}
			return false;
		}
	}
}
