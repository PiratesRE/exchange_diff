using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Serializable]
	public class ADSystemAttendantMailbox : ADRecipient, IADMailStorage
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADSystemAttendantMailbox.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADSystemAttendantMailbox.MostDerivedClass;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, this.MostDerivedObjectClass);
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2007;
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			errors.AddRange(Microsoft.Exchange.Data.Directory.SystemConfiguration.Database.ValidateAscendingQuotas(this.propertyBag, new ProviderPropertyDefinition[]
			{
				IADMailStorageSchema.IssueWarningQuota,
				IADMailStorageSchema.ProhibitSendQuota,
				IADMailStorageSchema.ProhibitSendReceiveQuota
			}, this.Identity));
		}

		internal ADSystemAttendantMailbox(IRecipientSession session, PropertyBag propertyBag) : base(session, propertyBag)
		{
		}

		internal ADSystemAttendantMailbox(IRecipientSession session, string commonName, ADObjectId containerId)
		{
			this.m_Session = session;
			base.SetId(containerId.GetChildId("CN", commonName));
			base.SetObjectClass(this.MostDerivedObjectClass);
		}

		public ADSystemAttendantMailbox()
		{
		}

		public ADObjectId Database
		{
			get
			{
				return (ADObjectId)this[IADMailStorageSchema.Database];
			}
			set
			{
				this[IADMailStorageSchema.Database] = value;
			}
		}

		public DeletedItemRetention DeletedItemFlags
		{
			get
			{
				return (DeletedItemRetention)this[IADMailStorageSchema.DeletedItemFlags];
			}
			set
			{
				this[IADMailStorageSchema.DeletedItemFlags] = value;
			}
		}

		public bool DeliverToMailboxAndForward
		{
			get
			{
				return (bool)this[IADMailStorageSchema.DeliverToMailboxAndForward];
			}
			set
			{
				this[IADMailStorageSchema.DeliverToMailboxAndForward] = value;
			}
		}

		public Guid ExchangeGuid
		{
			get
			{
				return (Guid)this[IADMailStorageSchema.ExchangeGuid];
			}
			set
			{
				this[IADMailStorageSchema.ExchangeGuid] = value;
			}
		}

		public RawSecurityDescriptor ExchangeSecurityDescriptor
		{
			get
			{
				return (RawSecurityDescriptor)this[IADMailStorageSchema.ExchangeSecurityDescriptor];
			}
			set
			{
				this[IADMailStorageSchema.ExchangeSecurityDescriptor] = value;
			}
		}

		public ExternalOofOptions ExternalOofOptions
		{
			get
			{
				return (ExternalOofOptions)this[IADMailStorageSchema.ExternalOofOptions];
			}
			set
			{
				this[IADMailStorageSchema.ExternalOofOptions] = value;
			}
		}

		public EnhancedTimeSpan RetainDeletedItemsFor
		{
			get
			{
				return (EnhancedTimeSpan)this[IADMailStorageSchema.RetainDeletedItemsFor];
			}
			set
			{
				this[IADMailStorageSchema.RetainDeletedItemsFor] = value;
			}
		}

		public bool IsMailboxEnabled
		{
			get
			{
				return (bool)this[IADMailStorageSchema.IsMailboxEnabled];
			}
		}

		public ADObjectId OfflineAddressBook
		{
			get
			{
				return (ADObjectId)this[IADMailStorageSchema.OfflineAddressBook];
			}
			set
			{
				this[IADMailStorageSchema.OfflineAddressBook] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> ProhibitSendQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[IADMailStorageSchema.ProhibitSendQuota];
			}
			set
			{
				this[IADMailStorageSchema.ProhibitSendQuota] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> ProhibitSendReceiveQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[IADMailStorageSchema.ProhibitSendReceiveQuota];
			}
			set
			{
				this[IADMailStorageSchema.ProhibitSendReceiveQuota] = value;
			}
		}

		public string ServerLegacyDN
		{
			get
			{
				return (string)this[IADMailStorageSchema.ServerLegacyDN];
			}
			set
			{
				this[IADMailStorageSchema.ServerLegacyDN] = value;
			}
		}

		public string ServerName
		{
			get
			{
				return (string)this[IADMailStorageSchema.ServerName];
			}
		}

		public bool? UseDatabaseQuotaDefaults
		{
			get
			{
				return (bool?)this[IADMailStorageSchema.UseDatabaseQuotaDefaults];
			}
			set
			{
				this[IADMailStorageSchema.UseDatabaseQuotaDefaults] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> IssueWarningQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[IADMailStorageSchema.IssueWarningQuota];
			}
			set
			{
				this[IADMailStorageSchema.IssueWarningQuota] = value;
			}
		}

		public ByteQuantifiedSize RulesQuota
		{
			get
			{
				return (ByteQuantifiedSize)this[IADMailStorageSchema.RulesQuota];
			}
			set
			{
				this[IADMailStorageSchema.RulesQuota] = value;
			}
		}

		private static readonly ADSystemAttendantMailboxSchema schema = ObjectSchema.GetInstance<ADSystemAttendantMailboxSchema>();

		internal static string MostDerivedClass = "exchangeAdminService";
	}
}
