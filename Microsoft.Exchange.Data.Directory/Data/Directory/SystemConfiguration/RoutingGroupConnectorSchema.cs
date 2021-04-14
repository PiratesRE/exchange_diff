using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class RoutingGroupConnectorSchema : SendConnectorSchema
	{
		public static readonly ADPropertyDefinition TargetRoutingGroup = new ADPropertyDefinition("TargetRoutingGroup", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "msExchDestinationRGDN", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Cost = new ADPropertyDefinition("Cost", ExchangeObjectVersion.Exchange2003, typeof(int), "cost", ADPropertyDefinitionFlags.PersistDefaultValue, 1, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 100)
		}, null, null);

		public static readonly ADPropertyDefinition TargetTransportServerVsis = new ADPropertyDefinition("TargetTransportServerVsis", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "msExchTargetBridgeheadServersDN", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExchangeLegacyDN = new ADPropertyDefinition("ExchangeLegacyDN", ExchangeObjectVersion.Exchange2003, typeof(string), "legacyExchangeDN", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PublicFolderReferralsDisabled = new ADPropertyDefinition("PublicFolderReferralsDisabled", ExchangeObjectVersion.Exchange2003, typeof(bool), "msExchNoPFConnection", ADPropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition VersionNumber = new ADPropertyDefinition("VersionNumber", ExchangeObjectVersion.Exchange2003, typeof(int), "versionNumber", ADPropertyDefinitionFlags.PersistDefaultValue, 7638, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TargetTransportServers = new ADPropertyDefinition("TargetTransportServers", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			RoutingGroupConnectorSchema.TargetTransportServerVsis
		}, null, new GetterDelegate(RoutingGroupConnector.TargetTransportServersGetter), new SetterDelegate(RoutingGroupConnector.TargetTransportServersSetter), null, null);

		public static readonly ADPropertyDefinition PublicFolderReferralsEnabled = new ADPropertyDefinition("PublicFolderReferralsEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			RoutingGroupConnectorSchema.PublicFolderReferralsDisabled
		}, null, new GetterDelegate(RoutingGroupConnector.PFReferralsEnabledGetter), new SetterDelegate(RoutingGroupConnector.PFReferralsEnabledSetter), null, null);
	}
}
