using System;

namespace Microsoft.Exchange.Data.Reporting
{
	internal class TenantThrottleInfoSchema : ObjectSchema
	{
		internal static readonly SimpleProviderPropertyDefinition TenantIdProperty = PropertyDefinitionsHelper.CreatePropertyDefinition("TenantId", typeof(Guid));

		internal static readonly SimpleProviderPropertyDefinition TimeStampProperty = PropertyDefinitionsHelper.CreatePropertyDefinition("TimeStamp", typeof(DateTime), DateTime.MinValue, PropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly SimpleProviderPropertyDefinition ThrottleStateProperty = PropertyDefinitionsHelper.CreatePropertyDefinition("ThrottleState", typeof(TenantThrottleState));

		internal static readonly SimpleProviderPropertyDefinition MessageCountProperty = PropertyDefinitionsHelper.CreatePropertyDefinition("MessageCount", typeof(int), 0, PropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly SimpleProviderPropertyDefinition AvgMessageSizeKbProperty = PropertyDefinitionsHelper.CreatePropertyDefinition("AvgMessageSizeKb", typeof(double));

		internal static readonly SimpleProviderPropertyDefinition AvgMessageCostMsProperty = PropertyDefinitionsHelper.CreatePropertyDefinition("AvgMessageCostMs", typeof(double));

		internal static readonly SimpleProviderPropertyDefinition ThrottlingFactorProperty = PropertyDefinitionsHelper.CreatePropertyDefinition("ThrottleFactor", typeof(double));

		internal static readonly SimpleProviderPropertyDefinition PartitionTenantCountProperty = PropertyDefinitionsHelper.CreatePropertyDefinition("PartitionTenantCount", typeof(int), 0, PropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly SimpleProviderPropertyDefinition PartitionMessageCountProperty = PropertyDefinitionsHelper.CreatePropertyDefinition("PartitionMessageCount", typeof(int), 0, PropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly SimpleProviderPropertyDefinition PartitionAvgMessageSizeKbProperty = PropertyDefinitionsHelper.CreatePropertyDefinition("PartitionAvgMessageSizeKb", typeof(double));

		internal static readonly SimpleProviderPropertyDefinition PartitionAvgMessageCostMsProperty = PropertyDefinitionsHelper.CreatePropertyDefinition("PartitionAvgMessageCostMs", typeof(double));

		internal static readonly SimpleProviderPropertyDefinition StandardDeviationProperty = PropertyDefinitionsHelper.CreatePropertyDefinition("StandardDeviation", typeof(double));

		internal static readonly SimpleProviderPropertyDefinition OverriddenOnlyProperty = PropertyDefinitionsHelper.CreatePropertyDefinition("OverriddenOnly", typeof(bool?));

		internal static readonly SimpleProviderPropertyDefinition ThrottledOnlyProperty = PropertyDefinitionsHelper.CreatePropertyDefinition("ThrottledOnly", typeof(bool?));

		internal static readonly SimpleProviderPropertyDefinition DataCountProperty = PropertyDefinitionsHelper.CreatePropertyDefinition("DataCount", typeof(int?));
	}
}
