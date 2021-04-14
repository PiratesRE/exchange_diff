using System;
using System.Xml;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	internal class PerfCounter : DiscoveryContext
	{
		public PerfCounter(XmlNode node, TracingContext traceContext) : this(node, traceContext, null)
		{
		}

		public PerfCounter(XmlNode node, TracingContext traceContext, MaintenanceResult result) : base(node, traceContext, result)
		{
		}

		internal string Object { get; set; }

		internal string Counter { get; set; }

		internal string Instance { get; set; }

		internal string PerfCounterName { get; set; }

		internal override void ProcessDefinitions(IMaintenanceWorkBroker broker)
		{
			if (!base.CheckEnvironment())
			{
				return;
			}
			this.Object = DefinitionHelperBase.GetMandatoryXmlAttribute<string>(base.ContextNode, "Object", base.TraceContext);
			this.Counter = DefinitionHelperBase.GetMandatoryXmlAttribute<string>(base.ContextNode, "Counter", base.TraceContext);
			this.Instance = DefinitionHelperBase.GetMandatoryXmlAttribute<string>(base.ContextNode, "Instance", base.TraceContext);
			if (string.IsNullOrEmpty(this.Counter))
			{
				throw new XmlException("The Counter attribute of the Counter node cannot be null or an empty string.");
			}
			if (string.IsNullOrEmpty(this.Object))
			{
				throw new XmlException("The Object attribute of the Counter node cannot be null or an empty string.");
			}
			if (string.IsNullOrEmpty(this.Instance) || this.Instance.Equals("*"))
			{
				this.PerfCounterName = string.Format("{0}\\{1}", this.Object, this.Counter);
			}
			else
			{
				this.PerfCounterName = string.Format("{0}\\{1}\\{2}", this.Object, this.Counter, this.Instance);
			}
			base.ProcessMonitorDefinitions();
			base.ProcessResponderDefinitions();
			base.AddDefinitions(broker);
		}
	}
}
