using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Monitoring
{
	internal class TransactionOutcomeBaseSchema : ObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition ClientAccessServer = new SimpleProviderPropertyDefinition("ClientAccessServer", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Latency = new SimpleProviderPropertyDefinition("Latency", ExchangeObjectVersion.Exchange2010, typeof(EnhancedTimeSpan), PropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.Zero, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ScenarioDescription = new SimpleProviderPropertyDefinition("ScenarioDescription", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ScenarioName = new SimpleProviderPropertyDefinition("ScenarioName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition PerformanceCounterName = new SimpleProviderPropertyDefinition("PerformanceCounterName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Result = new SimpleProviderPropertyDefinition("Result", ExchangeObjectVersion.Exchange2010, typeof(CasTransactionResult), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition AdditionalInformation = new SimpleProviderPropertyDefinition("AdditionalInformation", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition StartTime = new SimpleProviderPropertyDefinition("StartTime", ExchangeObjectVersion.Exchange2010, typeof(ExDateTime), PropertyDefinitionFlags.None, ExDateTime.Now, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition UserName = new SimpleProviderPropertyDefinition("UserName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition EventType = new SimpleProviderPropertyDefinition("EventType", ExchangeObjectVersion.Exchange2010, typeof(EventTypeEnumeration), PropertyDefinitionFlags.None, EventTypeEnumeration.Success, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
