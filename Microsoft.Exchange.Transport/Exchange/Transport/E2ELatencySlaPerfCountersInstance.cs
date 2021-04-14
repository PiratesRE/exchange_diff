using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal sealed class E2ELatencySlaPerfCountersInstance : PerformanceCounterInstance
	{
		internal E2ELatencySlaPerfCountersInstance(string instanceName, E2ELatencySlaPerfCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeTransport E2E Latency SLA")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.DeliverPercentMeetingSla = new ExPerformanceCounter(base.CategoryName, "Deliver Percent Meeting SLA", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DeliverPercentMeetingSla);
				this.SendToExternalPercentMeetingSla = new ExPerformanceCounter(base.CategoryName, "Send To External Percent Meeting SLA", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.SendToExternalPercentMeetingSla);
				long num = this.DeliverPercentMeetingSla.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter in list)
					{
						exPerformanceCounter.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal E2ELatencySlaPerfCountersInstance(string instanceName) : base(instanceName, "MSExchangeTransport E2E Latency SLA")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.DeliverPercentMeetingSla = new ExPerformanceCounter(base.CategoryName, "Deliver Percent Meeting SLA", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DeliverPercentMeetingSla);
				this.SendToExternalPercentMeetingSla = new ExPerformanceCounter(base.CategoryName, "Send To External Percent Meeting SLA", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.SendToExternalPercentMeetingSla);
				long num = this.DeliverPercentMeetingSla.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter in list)
					{
						exPerformanceCounter.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		public override void GetPerfCounterDiagnosticsInfo(XElement topElement)
		{
			XElement xelement = null;
			foreach (ExPerformanceCounter exPerformanceCounter in this.counters)
			{
				try
				{
					if (xelement == null)
					{
						xelement = new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.InstanceName));
						topElement.Add(xelement);
					}
					xelement.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					topElement.Add(content);
				}
			}
		}

		public readonly ExPerformanceCounter DeliverPercentMeetingSla;

		public readonly ExPerformanceCounter SendToExternalPercentMeetingSla;
	}
}
