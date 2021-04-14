using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.ExchangeTopology
{
	internal sealed class ExchangeTopologyPerformanceCountersInstance : PerformanceCounterInstance
	{
		internal ExchangeTopologyPerformanceCountersInstance(string instanceName, ExchangeTopologyPerformanceCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Topology")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.LastExchangeTopologyDiscoveryTimeSeconds = new ExPerformanceCounter(base.CategoryName, "Latest Exchange Topology Discovery Time in Seconds", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.LastExchangeTopologyDiscoveryTimeSeconds);
				this.ExchangeTopologyDiscoveriesPerformed = new ExPerformanceCounter(base.CategoryName, "Number of Exchange Topology Discoveries", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ExchangeTopologyDiscoveriesPerformed);
				this.SitelessServers = new ExPerformanceCounter(base.CategoryName, "Number of Siteless Servers", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.SitelessServers);
				long num = this.LastExchangeTopologyDiscoveryTimeSeconds.RawValue;
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

		internal ExchangeTopologyPerformanceCountersInstance(string instanceName) : base(instanceName, "MSExchange Topology")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.LastExchangeTopologyDiscoveryTimeSeconds = new ExPerformanceCounter(base.CategoryName, "Latest Exchange Topology Discovery Time in Seconds", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.LastExchangeTopologyDiscoveryTimeSeconds);
				this.ExchangeTopologyDiscoveriesPerformed = new ExPerformanceCounter(base.CategoryName, "Number of Exchange Topology Discoveries", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ExchangeTopologyDiscoveriesPerformed);
				this.SitelessServers = new ExPerformanceCounter(base.CategoryName, "Number of Siteless Servers", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.SitelessServers);
				long num = this.LastExchangeTopologyDiscoveryTimeSeconds.RawValue;
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

		public readonly ExPerformanceCounter LastExchangeTopologyDiscoveryTimeSeconds;

		public readonly ExPerformanceCounter ExchangeTopologyDiscoveriesPerformed;

		public readonly ExPerformanceCounter SitelessServers;
	}
}
