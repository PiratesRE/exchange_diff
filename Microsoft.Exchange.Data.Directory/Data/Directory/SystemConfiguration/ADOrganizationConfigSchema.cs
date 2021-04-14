using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ADOrganizationConfigSchema : OrganizationSchema
	{
		public static readonly ADPropertyDefinition ServicePlan = new ADPropertyDefinition("ServicePlan", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.TaskPopulated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TargetServicePlan = new ADPropertyDefinition("TargetServicePlan", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.TaskPopulated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SharePointUrl = new ADPropertyDefinition("SharePointUrl", ExchangeObjectVersion.Exchange2003, typeof(Uri), "wWWHomePage", "msExchShadowWWWHomePage", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute),
			new UIImpactStringLengthConstraint(0, 2048)
		}, SimpleProviderPropertyDefinition.None, null, null, null, null, null);
	}
}
