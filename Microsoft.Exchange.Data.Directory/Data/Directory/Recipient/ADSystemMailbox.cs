using System;
using System.Security.AccessControl;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Serializable]
	public sealed class ADSystemMailbox : ADRecipient, IADMailStorage
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADSystemMailbox.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADSystemMailbox.MostDerivedClass;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, this.MostDerivedObjectClass);
			}
		}

		internal ADSystemMailbox(IRecipientSession session, PropertyBag propertyBag) : base(session, propertyBag)
		{
		}

		internal ADSystemMailbox(IRecipientSession session, string commonName, ADObjectId containerId)
		{
			this.m_Session = session;
			base.SetId(containerId.GetChildId("CN", commonName));
			base.SetObjectClass(this.MostDerivedObjectClass);
		}

		public ADSystemMailbox()
		{
		}

		public DeliveryMechanisms DeliveryMechanism
		{
			get
			{
				return (DeliveryMechanisms)this[ADSystemMailboxSchema.DeliveryMechanism];
			}
			internal set
			{
				this[ADSystemMailboxSchema.DeliveryMechanism] = value;
			}
		}

		public ADObjectId Database
		{
			get
			{
				return (ADObjectId)this[ADSystemMailboxSchema.Database];
			}
			set
			{
				this[ADSystemMailboxSchema.Database] = value;
			}
		}

		public DeletedItemRetention DeletedItemFlags
		{
			get
			{
				return (DeletedItemRetention)this[ADSystemMailboxSchema.DeletedItemFlags];
			}
			set
			{
				this[ADSystemMailboxSchema.DeletedItemFlags] = value;
			}
		}

		public bool DeliverToMailboxAndForward
		{
			get
			{
				return (bool)this[ADSystemMailboxSchema.DeliverToMailboxAndForward];
			}
			set
			{
				this[ADSystemMailboxSchema.DeliverToMailboxAndForward] = value;
			}
		}

		public Guid ExchangeGuid
		{
			get
			{
				return (Guid)this[ADSystemMailboxSchema.ExchangeGuid];
			}
			set
			{
				this[ADSystemMailboxSchema.ExchangeGuid] = value;
			}
		}

		public RawSecurityDescriptor ExchangeSecurityDescriptor
		{
			get
			{
				return (RawSecurityDescriptor)this[ADSystemMailboxSchema.ExchangeSecurityDescriptor];
			}
			set
			{
				this[ADSystemMailboxSchema.ExchangeSecurityDescriptor] = value;
			}
		}

		public ExternalOofOptions ExternalOofOptions
		{
			get
			{
				return (ExternalOofOptions)this[ADSystemMailboxSchema.ExternalOofOptions];
			}
			set
			{
				this[ADSystemMailboxSchema.ExternalOofOptions] = value;
			}
		}

		public EnhancedTimeSpan RetainDeletedItemsFor
		{
			get
			{
				return (EnhancedTimeSpan)this[ADSystemMailboxSchema.RetainDeletedItemsFor];
			}
			set
			{
				this[ADSystemMailboxSchema.RetainDeletedItemsFor] = value;
			}
		}

		public bool IsMailboxEnabled
		{
			get
			{
				return (bool)this[ADSystemMailboxSchema.IsMailboxEnabled];
			}
		}

		public ADObjectId OfflineAddressBook
		{
			get
			{
				return (ADObjectId)this[ADSystemMailboxSchema.OfflineAddressBook];
			}
			set
			{
				this[ADSystemMailboxSchema.OfflineAddressBook] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> ProhibitSendQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ADSystemMailboxSchema.ProhibitSendQuota];
			}
			set
			{
				this[ADSystemMailboxSchema.ProhibitSendQuota] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> ProhibitSendReceiveQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ADSystemMailboxSchema.ProhibitSendReceiveQuota];
			}
			set
			{
				this[ADSystemMailboxSchema.ProhibitSendReceiveQuota] = value;
			}
		}

		public string ServerLegacyDN
		{
			get
			{
				return (string)this[ADSystemMailboxSchema.ServerLegacyDN];
			}
			set
			{
				this[ADSystemMailboxSchema.ServerLegacyDN] = value;
			}
		}

		public string ServerName
		{
			get
			{
				return (string)this[ADSystemMailboxSchema.ServerName];
			}
		}

		public bool? UseDatabaseQuotaDefaults
		{
			get
			{
				return (bool?)this[ADSystemMailboxSchema.UseDatabaseQuotaDefaults];
			}
			set
			{
				this[ADSystemMailboxSchema.UseDatabaseQuotaDefaults] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> IssueWarningQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ADSystemMailboxSchema.IssueWarningQuota];
			}
			set
			{
				this[ADSystemMailboxSchema.IssueWarningQuota] = value;
			}
		}

		public ByteQuantifiedSize RulesQuota
		{
			get
			{
				return (ByteQuantifiedSize)this[ADSystemMailboxSchema.RulesQuota];
			}
			set
			{
				this[ADSystemMailboxSchema.RulesQuota] = value;
			}
		}

		internal static string MostDerivedClass = "msExchSystemMailbox";

		private static readonly ADSystemMailboxSchema schema = ObjectSchema.GetInstance<ADSystemMailboxSchema>();
	}
}
