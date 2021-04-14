using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ForeignConnectorSchema : MailGatewaySchema
	{
		public static readonly ADPropertyDefinition MailGatewayFlags = new ADPropertyDefinition("MailGatewayFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchMailGatewayFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DropDirectory = new ADPropertyDefinition("DropDirectory", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchTransportDropDirectoryName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DropDirectoryQuota = new ADPropertyDefinition("DropDirectoryQuota", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<ByteQuantifiedSize>), ByteQuantifiedSize.MegabyteQuantifierProvider, "msExchTransportDropDirectoryQuota", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromMB(1UL), ByteQuantifiedSize.FromMB(2147483647UL))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RelayDsnRequired = new ADPropertyDefinition("RelayDsnRequired", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ForeignConnectorSchema.MailGatewayFlags
		}, null, ADObject.FlagGetterDelegate(ForeignConnectorSchema.MailGatewayFlags, 1), ADObject.FlagSetterDelegate(ForeignConnectorSchema.MailGatewayFlags, 1), null, null);

		public static readonly ADPropertyDefinition Disabled = new ADPropertyDefinition("Disabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ForeignConnectorSchema.MailGatewayFlags
		}, null, ADObject.FlagGetterDelegate(ForeignConnectorSchema.MailGatewayFlags, 2), ADObject.FlagSetterDelegate(ForeignConnectorSchema.MailGatewayFlags, 2), null, null);
	}
}
