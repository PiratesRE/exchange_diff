using System;
using System.Xml;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	internal class CustomWorkItem : DiscoveryContext
	{
		public CustomWorkItem(XmlNode node, TracingContext traceContext) : this(node, traceContext, null)
		{
		}

		public CustomWorkItem(XmlNode node, TracingContext traceContext, MaintenanceResult result) : base(node, traceContext, result)
		{
		}

		internal override void ProcessDefinitions(IMaintenanceWorkBroker broker)
		{
			if (!base.CheckEnvironment())
			{
				return;
			}
			base.ProcessProbeDefinition();
			base.ProcessMonitorDefinitions();
			base.ProcessResponderDefinitions();
			base.AddDefinitions(broker);
		}
	}
}
