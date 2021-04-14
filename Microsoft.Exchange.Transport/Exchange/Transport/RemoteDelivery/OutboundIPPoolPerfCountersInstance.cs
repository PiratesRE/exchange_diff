using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.RemoteDelivery
{
	internal sealed class OutboundIPPoolPerfCountersInstance : PerformanceCounterInstance
	{
		internal OutboundIPPoolPerfCountersInstance(string instanceName, OutboundIPPoolPerfCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeTransport Outbound IP Pools")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.NormalRisk = new ExPerformanceCounter(base.CategoryName, "Percentage of Normal Risk queues with failures", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NormalRisk);
				this.BulkRisk = new ExPerformanceCounter(base.CategoryName, "Percentage of Bulk Risk queues with failures", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.BulkRisk);
				this.HighRisk = new ExPerformanceCounter(base.CategoryName, "Percentage of High Risk queues with failures", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.HighRisk);
				this.LowRisk = new ExPerformanceCounter(base.CategoryName, "Percentage of Low Risk queues with failures", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.LowRisk);
				long num = this.NormalRisk.RawValue;
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

		internal OutboundIPPoolPerfCountersInstance(string instanceName) : base(instanceName, "MSExchangeTransport Outbound IP Pools")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.NormalRisk = new ExPerformanceCounter(base.CategoryName, "Percentage of Normal Risk queues with failures", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NormalRisk);
				this.BulkRisk = new ExPerformanceCounter(base.CategoryName, "Percentage of Bulk Risk queues with failures", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.BulkRisk);
				this.HighRisk = new ExPerformanceCounter(base.CategoryName, "Percentage of High Risk queues with failures", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.HighRisk);
				this.LowRisk = new ExPerformanceCounter(base.CategoryName, "Percentage of Low Risk queues with failures", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.LowRisk);
				long num = this.NormalRisk.RawValue;
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

		public readonly ExPerformanceCounter NormalRisk;

		public readonly ExPerformanceCounter BulkRisk;

		public readonly ExPerformanceCounter HighRisk;

		public readonly ExPerformanceCounter LowRisk;
	}
}
