using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class UMHuntGroupSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition UMDialPlan = new ADPropertyDefinition("UMDialPlan", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchUMHuntGroupDialPlanLink", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PilotIdentifier = new ADPropertyDefinition("PilotIdentifier", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchUMPilotIdentifier", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 64)
		}, null, null);

		public static readonly ADPropertyDefinition UMIPGateway = new ADPropertyDefinition("UMIPGateway", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id
		}, null, new GetterDelegate(UMHuntGroup.UMIPGatewayGetter), null, null, null);
	}
}
