using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Fips.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	internal class CollectFIPSLogsResponderHelper : ResponderDefinitionHelper
	{
		internal override ResponderDefinition CreateDefinition()
		{
			int optionalXmlAttribute = base.GetOptionalXmlAttribute<int>("RecurrenceIntervalSeconds", 900);
			int optionalXmlAttribute2 = base.GetOptionalXmlAttribute<int>("WaitIntervalSeconds", 28800);
			int optionalXmlAttribute3 = base.GetOptionalXmlAttribute<int>("TimeoutSeconds", 600);
			int optionalXmlAttribute4 = base.GetOptionalXmlAttribute<int>("MaxRetryAttempts", 1);
			string optionalValue = base.GetOptionalValue<string>("LogDestination", CollectFIPSLogsResponder.DefaultValues.LogDestination);
			int optionalValue2 = base.GetOptionalValue<int>("LifeTimeSeconds", 432000);
			bool optionalXmlAttribute5 = base.GetOptionalXmlAttribute<bool>("Enabled", true);
			return CollectFIPSLogsResponder.CreateDefinition(base.Name, base.ServiceName, base.AlertTypeId, base.AlertMask, base.TargetResource, base.TargetHealthState, optionalXmlAttribute, optionalXmlAttribute2, optionalXmlAttribute3, optionalXmlAttribute4, optionalValue, optionalValue2, optionalXmlAttribute5);
		}
	}
}
