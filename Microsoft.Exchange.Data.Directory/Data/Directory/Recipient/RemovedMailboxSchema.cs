using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class RemovedMailboxSchema : DeletedObjectSchema
	{
		public new static readonly ADPropertyDefinition Name = new ADPropertyDefinition("Name", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.RawName
		}, new CustomFilterBuilderDelegate(RemovedMailbox.NameFilterBuilderDelegate), new GetterDelegate(RemovedMailbox.NameGetter), null, null, null);

		public static readonly ADPropertyDefinition PreviousDatabase = IADMailStorageSchema.PreviousDatabase;

		public static readonly ADPropertyDefinition EmailAddresses = ADRecipientSchema.EmailAddresses;

		public static readonly ADPropertyDefinition ExchangeGuid = IADMailStorageSchema.ExchangeGuid;

		public static readonly ADPropertyDefinition LegacyExchangeDN = ADRecipientSchema.LegacyExchangeDN;

		public static readonly ADPropertyDefinition SamAccountName = IADSecurityPrincipalSchema.SamAccountName;

		public static readonly ADPropertyDefinition WindowsLiveID = ADRecipientSchema.WindowsLiveID;

		public static readonly ADPropertyDefinition NetID = new ADPropertyDefinition("NetID", ExchangeObjectVersion.Exchange2010, typeof(NetID), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADRecipientSchema.EmailAddresses
		}, null, new GetterDelegate(RemovedMailbox.NetIDGetter), null, null, null);

		public static readonly ADPropertyDefinition ConsumerNetID = new ADPropertyDefinition("ConsumerNetID", ExchangeObjectVersion.Exchange2010, typeof(NetID), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADRecipientSchema.EmailAddresses
		}, null, new GetterDelegate(RemovedMailbox.ConsumerNetIDGetter), null, null, null);

		public static readonly ADPropertyDefinition IsPasswordResetRequired = new ADPropertyDefinition("IsPasswordResetRequired", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADRecipientSchema.EmailAddresses
		}, null, new GetterDelegate(RemovedMailbox.IsPasswordResetRequiredGetter), null, null, null);
	}
}
