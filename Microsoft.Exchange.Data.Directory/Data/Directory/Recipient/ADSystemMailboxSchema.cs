using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class ADSystemMailboxSchema : ADRecipientSchema
	{
		public static readonly ADPropertyDefinition DeliveryMechanism = new ADPropertyDefinition("DeliveryMechanism", ExchangeObjectVersion.Exchange2003, typeof(DeliveryMechanisms), "deliveryMechanism", ADPropertyDefinitionFlags.PersistDefaultValue, DeliveryMechanisms.MessageStore, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(DeliveryMechanisms))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RulesQuota = new ADPropertyDefinition("systemMailboxRulesQuota", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<ByteQuantifiedSize>), ByteQuantifiedSize.KilobyteQuantifierProvider, "msExchMDBRulesQuota", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RetainDeletedItemsFor = new ADPropertyDefinition("systemMailboxRetainDeletedItemsFor", ExchangeObjectVersion.Exchange2003, typeof(EnhancedTimeSpan?), "garbageCollPeriod", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.Zero, EnhancedTimeSpan.FromSeconds(2147483647.0)),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Database = IADMailStorageSchema.Database;

		public static readonly ADPropertyDefinition DeletedItemFlags = IADMailStorageSchema.DeletedItemFlags;

		public static readonly ADPropertyDefinition DeliverToMailboxAndForward = IADMailStorageSchema.DeliverToMailboxAndForward;

		public static readonly ADPropertyDefinition ExchangeGuid = IADMailStorageSchema.ExchangeGuid;

		public static readonly ADPropertyDefinition MailboxContainerGuid = IADMailStorageSchema.MailboxContainerGuid;

		public static readonly ADPropertyDefinition ExchangeSecurityDescriptor = IADMailStorageSchema.ExchangeSecurityDescriptor;

		public static readonly ADPropertyDefinition ExternalOofOptions = IADMailStorageSchema.ExternalOofOptions;

		public static readonly ADPropertyDefinition IsMailboxEnabled = IADMailStorageSchema.IsMailboxEnabled;

		public static readonly ADPropertyDefinition OfflineAddressBook = IADMailStorageSchema.OfflineAddressBook;

		public static readonly ADPropertyDefinition ProhibitSendQuota = IADMailStorageSchema.ProhibitSendQuota;

		public static readonly ADPropertyDefinition ProhibitSendReceiveQuota = IADMailStorageSchema.ProhibitSendReceiveQuota;

		public static readonly ADPropertyDefinition ServerLegacyDN = IADMailStorageSchema.ServerLegacyDN;

		public static readonly ADPropertyDefinition ServerName = IADMailStorageSchema.ServerName;

		public static readonly ADPropertyDefinition UseDatabaseQuotaDefaults = IADMailStorageSchema.UseDatabaseQuotaDefaults;

		public static readonly ADPropertyDefinition IssueWarningQuota = IADMailStorageSchema.IssueWarningQuota;
	}
}
