using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class ADMicrosoftExchangeRecipient : ADRecipient, IADMailStorage
	{
		internal static ADObjectId GetDefaultId(IConfigurationSession configurationSession)
		{
			if (configurationSession == null)
			{
				throw new ArgumentNullException("configurationSession");
			}
			return configurationSession.GetOrgContainerId().GetChildId("Transport Settings").GetChildId(ADMicrosoftExchangeRecipient.DefaultName);
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ADMicrosoftExchangeRecipient.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADMicrosoftExchangeRecipient.MostDerivedClass;
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

		internal ADMicrosoftExchangeRecipient(IRecipientSession session, PropertyBag propertyBag) : base(session, propertyBag)
		{
		}

		internal ADMicrosoftExchangeRecipient(IRecipientSession session, string commonName, ADObjectId containerId)
		{
			this.m_Session = session;
			base.SetId(containerId.GetChildId("CN", commonName));
			base.SetObjectClass(this.MostDerivedObjectClass);
		}

		public ADMicrosoftExchangeRecipient()
		{
			base.SetObjectClass(this.MostDerivedObjectClass);
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (!ADMicrosoftExchangeRecipient.DefaultDisplayName.Equals(base.DisplayName, StringComparison.OrdinalIgnoreCase))
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.MicrosoftExchangeRecipientDisplayNameError(ADMicrosoftExchangeRecipient.DefaultDisplayName), this.Identity, string.Empty));
			}
		}

		public ADObjectId Database
		{
			get
			{
				return (ADObjectId)this[IADMailStorageSchema.Database];
			}
			internal set
			{
				this[IADMailStorageSchema.Database] = value;
			}
		}

		ADObjectId IADMailStorage.Database
		{
			get
			{
				return this.Database;
			}
			set
			{
				this.Database = value;
			}
		}

		public DeletedItemRetention DeletedItemFlags
		{
			get
			{
				return (DeletedItemRetention)this[IADMailStorageSchema.DeletedItemFlags];
			}
			internal set
			{
				this[IADMailStorageSchema.DeletedItemFlags] = value;
			}
		}

		DeletedItemRetention IADMailStorage.DeletedItemFlags
		{
			get
			{
				return this.DeletedItemFlags;
			}
			set
			{
				this.DeletedItemFlags = value;
			}
		}

		public bool DeliverToMailboxAndForward
		{
			get
			{
				return (bool)this[IADMailStorageSchema.DeliverToMailboxAndForward];
			}
			internal set
			{
				this[IADMailStorageSchema.DeliverToMailboxAndForward] = value;
			}
		}

		bool IADMailStorage.DeliverToMailboxAndForward
		{
			get
			{
				return this.DeliverToMailboxAndForward;
			}
			set
			{
				this.DeliverToMailboxAndForward = value;
			}
		}

		public Guid ExchangeGuid
		{
			get
			{
				return (Guid)this[IADMailStorageSchema.ExchangeGuid];
			}
			internal set
			{
				this[IADMailStorageSchema.ExchangeGuid] = value;
			}
		}

		Guid IADMailStorage.ExchangeGuid
		{
			get
			{
				return this.ExchangeGuid;
			}
			set
			{
				this.ExchangeGuid = value;
			}
		}

		public RawSecurityDescriptor ExchangeSecurityDescriptor
		{
			get
			{
				return (RawSecurityDescriptor)this[IADMailStorageSchema.ExchangeSecurityDescriptor];
			}
			internal set
			{
				this[IADMailStorageSchema.ExchangeSecurityDescriptor] = value;
			}
		}

		RawSecurityDescriptor IADMailStorage.ExchangeSecurityDescriptor
		{
			get
			{
				return this.ExchangeSecurityDescriptor;
			}
			set
			{
				this.ExchangeSecurityDescriptor = value;
			}
		}

		public ExternalOofOptions ExternalOofOptions
		{
			get
			{
				return (ExternalOofOptions)this[IADMailStorageSchema.ExternalOofOptions];
			}
			internal set
			{
				this[IADMailStorageSchema.ExternalOofOptions] = value;
			}
		}

		ExternalOofOptions IADMailStorage.ExternalOofOptions
		{
			get
			{
				return this.ExternalOofOptions;
			}
			set
			{
				this.ExternalOofOptions = value;
			}
		}

		public EnhancedTimeSpan RetainDeletedItemsFor
		{
			get
			{
				return (EnhancedTimeSpan)this[IADMailStorageSchema.RetainDeletedItemsFor];
			}
			internal set
			{
				this[IADMailStorageSchema.RetainDeletedItemsFor] = value;
			}
		}

		EnhancedTimeSpan IADMailStorage.RetainDeletedItemsFor
		{
			get
			{
				return this.RetainDeletedItemsFor;
			}
			set
			{
				this.RetainDeletedItemsFor = value;
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
			internal set
			{
				this[IADMailStorageSchema.OfflineAddressBook] = value;
			}
		}

		ADObjectId IADMailStorage.OfflineAddressBook
		{
			get
			{
				return this.OfflineAddressBook;
			}
			set
			{
				this.OfflineAddressBook = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> ProhibitSendQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[IADMailStorageSchema.ProhibitSendQuota];
			}
			internal set
			{
				this[IADMailStorageSchema.ProhibitSendQuota] = value;
			}
		}

		Unlimited<ByteQuantifiedSize> IADMailStorage.ProhibitSendQuota
		{
			get
			{
				return this.ProhibitSendQuota;
			}
			set
			{
				this.ProhibitSendQuota = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> ProhibitSendReceiveQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[IADMailStorageSchema.ProhibitSendReceiveQuota];
			}
			internal set
			{
				this[IADMailStorageSchema.ProhibitSendReceiveQuota] = value;
			}
		}

		Unlimited<ByteQuantifiedSize> IADMailStorage.ProhibitSendReceiveQuota
		{
			get
			{
				return this.ProhibitSendReceiveQuota;
			}
			set
			{
				this.ProhibitSendReceiveQuota = value;
			}
		}

		public string ServerLegacyDN
		{
			get
			{
				return (string)this[IADMailStorageSchema.ServerLegacyDN];
			}
			internal set
			{
				this[IADMailStorageSchema.ServerLegacyDN] = value;
			}
		}

		string IADMailStorage.ServerLegacyDN
		{
			get
			{
				return this.ServerLegacyDN;
			}
			set
			{
				this.ServerLegacyDN = value;
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
			internal set
			{
				this[IADMailStorageSchema.UseDatabaseQuotaDefaults] = value;
			}
		}

		bool? IADMailStorage.UseDatabaseQuotaDefaults
		{
			get
			{
				return this.UseDatabaseQuotaDefaults;
			}
			set
			{
				this.UseDatabaseQuotaDefaults = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> IssueWarningQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[IADMailStorageSchema.IssueWarningQuota];
			}
			internal set
			{
				this[IADMailStorageSchema.IssueWarningQuota] = value;
			}
		}

		Unlimited<ByteQuantifiedSize> IADMailStorage.IssueWarningQuota
		{
			get
			{
				return this.IssueWarningQuota;
			}
			set
			{
				this.IssueWarningQuota = value;
			}
		}

		public ByteQuantifiedSize RulesQuota
		{
			get
			{
				return (ByteQuantifiedSize)this[IADMailStorageSchema.RulesQuota];
			}
			internal set
			{
				this[IADMailStorageSchema.RulesQuota] = value;
			}
		}

		ByteQuantifiedSize IADMailStorage.RulesQuota
		{
			get
			{
				return this.RulesQuota;
			}
			set
			{
				this.RulesQuota = value;
			}
		}

		private static ADMicrosoftExchangeRecipientSchema schema = ObjectSchema.GetInstance<ADMicrosoftExchangeRecipientSchema>();

		internal static string MostDerivedClass = "msExchExchangeServerRecipient";

		public static readonly string DefaultName = "MicrosoftExchange329e71ec88ae4615bbc36ab6ce41109e";

		public static readonly string DefaultDisplayName = "Microsoft Outlook";
	}
}
