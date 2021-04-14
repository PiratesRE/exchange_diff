using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net
{
	internal sealed class ServiceProxyPoolCountersInstance : PerformanceCounterInstance
	{
		internal ServiceProxyPoolCountersInstance(string instanceName, ServiceProxyPoolCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange ServiceProxyPool")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.ProxyInstanceCount = new ExPerformanceCounter(base.CategoryName, "Proxy Instance Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ProxyInstanceCount);
				this.OutstandingCalls = new ExPerformanceCounter(base.CategoryName, "Current Outstanding Calls", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OutstandingCalls);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Calls/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.NumberOfCalls = new ExPerformanceCounter(base.CategoryName, "Total Number of Calls", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.NumberOfCalls);
				this.AverageLatency = new ExPerformanceCounter(base.CategoryName, "Average Latency", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageLatency);
				this.AverageLatencyBase = new ExPerformanceCounter(base.CategoryName, "Base for Average Latency", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageLatencyBase);
				long num = this.ProxyInstanceCount.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter2 in list)
					{
						exPerformanceCounter2.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal ServiceProxyPoolCountersInstance(string instanceName) : base(instanceName, "MSExchange ServiceProxyPool")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.ProxyInstanceCount = new ExPerformanceCounter(base.CategoryName, "Proxy Instance Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ProxyInstanceCount);
				this.OutstandingCalls = new ExPerformanceCounter(base.CategoryName, "Current Outstanding Calls", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OutstandingCalls);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Calls/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.NumberOfCalls = new ExPerformanceCounter(base.CategoryName, "Total Number of Calls", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.NumberOfCalls);
				this.AverageLatency = new ExPerformanceCounter(base.CategoryName, "Average Latency", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageLatency);
				this.AverageLatencyBase = new ExPerformanceCounter(base.CategoryName, "Base for Average Latency", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageLatencyBase);
				long num = this.ProxyInstanceCount.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter2 in list)
					{
						exPerformanceCounter2.Close();
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

		public readonly ExPerformanceCounter ProxyInstanceCount;

		public readonly ExPerformanceCounter OutstandingCalls;

		public readonly ExPerformanceCounter NumberOfCalls;

		public readonly ExPerformanceCounter AverageLatency;

		public readonly ExPerformanceCounter AverageLatencyBase;
	}
}
