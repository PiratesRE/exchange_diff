using System;

namespace Microsoft.Exchange.Data
{
	internal class TransportProcessingQuotaConfigSchema : ObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition Id = PropertyDefinitionsHelper.CreatePropertyDefinition("Id", typeof(Guid));

		public static readonly SimpleProviderPropertyDefinition SettingName = PropertyDefinitionsHelper.CreatePropertyDefinition("RawName", typeof(string));

		public static readonly SimpleProviderPropertyDefinition ThrottlingEnabled = PropertyDefinitionsHelper.CreatePropertyDefinition("ThrottlingEnabled", typeof(bool), true, PropertyDefinitionFlags.PersistDefaultValue);

		public static readonly SimpleProviderPropertyDefinition CalculationEnabled = PropertyDefinitionsHelper.CreatePropertyDefinition("CalculationEnabled", typeof(bool), true, PropertyDefinitionFlags.PersistDefaultValue);

		public static readonly SimpleProviderPropertyDefinition AmWeight = PropertyDefinitionsHelper.CreatePropertyDefinition("AmWeight", typeof(double), 0.0, PropertyDefinitionFlags.PersistDefaultValue);

		public static readonly SimpleProviderPropertyDefinition AsWeight = PropertyDefinitionsHelper.CreatePropertyDefinition("AsWeight", typeof(double), 0.0, PropertyDefinitionFlags.PersistDefaultValue);

		public static readonly SimpleProviderPropertyDefinition CalculationFrequency = PropertyDefinitionsHelper.CreatePropertyDefinition("CalculationFrequency", typeof(int), 15, PropertyDefinitionFlags.PersistDefaultValue);

		public static readonly SimpleProviderPropertyDefinition CostThreshold = PropertyDefinitionsHelper.CreatePropertyDefinition("CostThreshold", typeof(int), 100, PropertyDefinitionFlags.PersistDefaultValue);

		public static readonly SimpleProviderPropertyDefinition EtrWeight = PropertyDefinitionsHelper.CreatePropertyDefinition("EtrWeight", typeof(double), 1.0, PropertyDefinitionFlags.PersistDefaultValue);

		public static readonly SimpleProviderPropertyDefinition TimeWindow = PropertyDefinitionsHelper.CreatePropertyDefinition("TimeWindow", typeof(int), 15, PropertyDefinitionFlags.PersistDefaultValue);

		public static readonly SimpleProviderPropertyDefinition ThrottleFactor = PropertyDefinitionsHelper.CreatePropertyDefinition("ThrottleFactor", typeof(double), 0.01, PropertyDefinitionFlags.PersistDefaultValue);

		public static readonly SimpleProviderPropertyDefinition RelativeCostThreshold = PropertyDefinitionsHelper.CreatePropertyDefinition("RelativeCostThreshold", typeof(double), 5.0, PropertyDefinitionFlags.PersistDefaultValue);
	}
}
