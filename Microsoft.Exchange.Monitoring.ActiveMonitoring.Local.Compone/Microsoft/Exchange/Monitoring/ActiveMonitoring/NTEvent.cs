using System;
using System.Xml;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	internal class NTEvent : DiscoveryContext
	{
		public NTEvent(XmlNode node, TracingContext traceContext) : this(node, traceContext, null)
		{
		}

		public NTEvent(XmlNode node, TracingContext traceContext, MaintenanceResult result) : base(node, traceContext, result)
		{
		}

		internal bool IsInstrumented { get; set; }

		internal override void ProcessDefinitions(IMaintenanceWorkBroker broker)
		{
			if (!base.CheckEnvironment())
			{
				return;
			}
			if (base.GetDescendantCount("Probe") == 0)
			{
				this.IsInstrumented = true;
			}
			else
			{
				this.IsInstrumented = false;
				base.ProcessProbeDefinition();
			}
			base.ProcessMonitorDefinitions();
			base.ProcessResponderDefinitions();
			base.AddDefinitions(broker);
		}
	}
}
