using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class MailGatewaySchema : SendConnectorSchema
	{
		public static readonly ADPropertyDefinition AddressSpaces = new ADPropertyDefinition("AddressSpaces", ExchangeObjectVersion.Exchange2003, typeof(AddressSpace), "routingList", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ConnectedDomains = new ADPropertyDefinition("ConnectedDomains", ExchangeObjectVersion.Exchange2003, typeof(ConnectedDomain), "connectedDomains", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DeliveryMechanism = new ADPropertyDefinition("DeliveryMechanism", ExchangeObjectVersion.Exchange2003, typeof(int), "deliveryMechanism", ADPropertyDefinitionFlags.PersistDefaultValue, 2, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 3)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Comment = new ADPropertyDefinition("Comment", ExchangeObjectVersion.Exchange2003, typeof(string), "adminDescription", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IsScopedConnector = new ADPropertyDefinition("IsScopedConnector", ExchangeObjectVersion.Exchange2003, typeof(bool?), null, ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IsSmtpConnector = new ADPropertyDefinition("IsSmtpConnector", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.ObjectClass
		}, null, new GetterDelegate(MailGateway.IsSmtpConnectorGetter), null, null, null);
	}
}
