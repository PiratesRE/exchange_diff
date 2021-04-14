using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Monitoring.Reporting
{
	internal class MessageStatisticsReportSchema : TransportReportSchema
	{
		public static readonly SimpleProviderPropertyDefinition TotalMessagesSent = new SimpleProviderPropertyDefinition("TotalMessagesSent", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TotalMessagesReceived = new SimpleProviderPropertyDefinition("TotalMessagesReceived", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TotalMessagesSentToForeign = new SimpleProviderPropertyDefinition("TotalMessagesSentToForeign", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TotalMessagesReceivedFromForeign = new SimpleProviderPropertyDefinition("TotalMessagesReceivedFromForeign", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
