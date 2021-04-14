using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ADEcpVirtualDirectorySchema : ExchangeWebAppVirtualDirectorySchema
	{
		public static readonly ADPropertyDefinition ADFeatureSet = new ADPropertyDefinition("ADFeatureSet", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchManagementSettings", ADPropertyDefinitionFlags.PersistDefaultValue, -1, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AdminEnabled = new ADPropertyDefinition("AdminEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADEcpVirtualDirectorySchema.ADFeatureSet
		}, null, ADObject.FlagGetterDelegate(1, ADEcpVirtualDirectorySchema.ADFeatureSet), ADObject.FlagSetterDelegate(1, ADEcpVirtualDirectorySchema.ADFeatureSet), null, null);

		public static readonly ADPropertyDefinition OwaOptionsEnabled = new ADPropertyDefinition("OwaOptionsEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADEcpVirtualDirectorySchema.ADFeatureSet
		}, null, ADObject.FlagGetterDelegate(2, ADEcpVirtualDirectorySchema.ADFeatureSet), ADObject.FlagSetterDelegate(2, ADEcpVirtualDirectorySchema.ADFeatureSet), null, null);
	}
}
