using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Monitoring
{
	internal class TestMailflowOutputSchema : SimpleProviderObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition TestMailflowResult = new SimpleProviderPropertyDefinition("TestMailflowResult", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition MessageLatencyTime = new SimpleProviderPropertyDefinition("MessageLatencyTime", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), PropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromSeconds(0.0), PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition IsRemoteTest = new SimpleProviderPropertyDefinition("IsRemoteTest", ExchangeObjectVersion.Exchange2007, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
