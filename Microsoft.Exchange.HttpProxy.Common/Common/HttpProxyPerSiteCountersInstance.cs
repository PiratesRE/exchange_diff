using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.HttpProxy.Common
{
	internal sealed class HttpProxyPerSiteCountersInstance : PerformanceCounterInstance
	{
		internal HttpProxyPerSiteCountersInstance(string instanceName, HttpProxyPerSiteCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange HttpProxy Per Site")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.RoutingLatency90thPercentile = new ExPerformanceCounter(base.CategoryName, "Routing Latency 90th Percentile", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.RoutingLatency90thPercentile);
				this.RoutingLatency95thPercentile = new ExPerformanceCounter(base.CategoryName, "Routing Latency 95th Percentile", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.RoutingLatency95thPercentile);
				this.RoutingLatency99thPercentile = new ExPerformanceCounter(base.CategoryName, "Routing Latency 99th Percentile", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.RoutingLatency99thPercentile);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Failed Requests/Sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.TotalFailedRequests = new ExPerformanceCounter(base.CategoryName, "Total Failed Requests", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.TotalFailedRequests);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Proxy Requests with Latency Data/Sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.TotalProxyWithLatencyRequests = new ExPerformanceCounter(base.CategoryName, "Total Proxy with Latency Data Requests", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.TotalProxyWithLatencyRequests);
				this.MovingPercentageRoutingFailure = new ExPerformanceCounter(base.CategoryName, "Routing Failure Percentage", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MovingPercentageRoutingFailure);
				long num = this.RoutingLatency90thPercentile.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter3 in list)
					{
						exPerformanceCounter3.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal HttpProxyPerSiteCountersInstance(string instanceName) : base(instanceName, "MSExchange HttpProxy Per Site")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.RoutingLatency90thPercentile = new ExPerformanceCounter(base.CategoryName, "Routing Latency 90th Percentile", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.RoutingLatency90thPercentile);
				this.RoutingLatency95thPercentile = new ExPerformanceCounter(base.CategoryName, "Routing Latency 95th Percentile", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.RoutingLatency95thPercentile);
				this.RoutingLatency99thPercentile = new ExPerformanceCounter(base.CategoryName, "Routing Latency 99th Percentile", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.RoutingLatency99thPercentile);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Failed Requests/Sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.TotalFailedRequests = new ExPerformanceCounter(base.CategoryName, "Total Failed Requests", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.TotalFailedRequests);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Proxy Requests with Latency Data/Sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.TotalProxyWithLatencyRequests = new ExPerformanceCounter(base.CategoryName, "Total Proxy with Latency Data Requests", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.TotalProxyWithLatencyRequests);
				this.MovingPercentageRoutingFailure = new ExPerformanceCounter(base.CategoryName, "Routing Failure Percentage", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MovingPercentageRoutingFailure);
				long num = this.RoutingLatency90thPercentile.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter3 in list)
					{
						exPerformanceCounter3.Close();
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

		public readonly ExPerformanceCounter RoutingLatency90thPercentile;

		public readonly ExPerformanceCounter RoutingLatency95thPercentile;

		public readonly ExPerformanceCounter RoutingLatency99thPercentile;

		public readonly ExPerformanceCounter TotalFailedRequests;

		public readonly ExPerformanceCounter TotalProxyWithLatencyRequests;

		public readonly ExPerformanceCounter MovingPercentageRoutingFailure;
	}
}
