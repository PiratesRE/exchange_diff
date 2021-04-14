using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.AgentTasks
{
	internal class TransportEventSchema : SimpleProviderObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition EventTopic = new SimpleProviderPropertyDefinition("EventTopic", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TransportAgentIdentities = new SimpleProviderPropertyDefinition("TransportAgentIdentities", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
