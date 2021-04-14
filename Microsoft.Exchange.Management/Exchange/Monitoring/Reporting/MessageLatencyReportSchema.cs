using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Monitoring.Reporting
{
	internal class MessageLatencyReportSchema : TransportReportSchema
	{
		public static readonly SimpleProviderPropertyDefinition PercentOfMessageInGivenSla = new SimpleProviderPropertyDefinition("PercentOfMessageInGivenSla", ExchangeObjectVersion.Exchange2010, typeof(decimal), PropertyDefinitionFlags.PersistDefaultValue, 0m, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition SlaTargetInSeconds = new SimpleProviderPropertyDefinition("SlaTargetInSeconds", ExchangeObjectVersion.Exchange2010, typeof(short), PropertyDefinitionFlags.WriteOnce, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
