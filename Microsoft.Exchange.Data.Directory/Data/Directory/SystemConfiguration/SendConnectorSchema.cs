using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class SendConnectorSchema : ADLegacyVersionableObjectSchema
	{
		public static readonly ADPropertyDefinition SourceTransportServerVsis = new ADPropertyDefinition("SourceTransportServerVsis", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "msExchSourceBridgeheadServersDN", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SourceTransportServers = new ADPropertyDefinition("SourceTransportServers", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			SendConnectorSchema.SourceTransportServerVsis
		}, null, new GetterDelegate(SendConnector.SourceTransportServersGetter), new SetterDelegate(SendConnector.SourceTransportServersSetter), null, null);

		public static readonly ADPropertyDefinition SourceRoutingGroup = new ADPropertyDefinition("SourceRoutingGroup", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id
		}, null, new GetterDelegate(SendConnector.SourceRoutingGroupGetter), null, null, null);

		public static readonly ADPropertyDefinition MaxMessageSize = new ADPropertyDefinition("MaxMessageSize", ExchangeObjectVersion.Exchange2003, typeof(Unlimited<ByteQuantifiedSize>), ByteQuantifiedSize.KilobyteQuantifierProvider, "delivContLength", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromKB(0UL), ByteQuantifiedSize.FromKB(2147483647UL))
		}, null, null);

		public static readonly ADPropertyDefinition HomeMTA = ADRecipientSchema.HomeMTA;

		public static readonly ADPropertyDefinition HomeMtaServerId = ADGroupSchema.HomeMtaServerId;
	}
}
