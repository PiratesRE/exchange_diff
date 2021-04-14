using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	internal sealed class MSExchangeADAccessCacheCountersInstance : PerformanceCounterInstance
	{
		internal MSExchangeADAccessCacheCountersInstance(string instanceName, MSExchangeADAccessCacheCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange ADAccess Cache")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.CacheHit = new ExPerformanceCounter(base.CategoryName, "Percentage of Cache Hit Last Minute", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CacheHit);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Cache Requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.NumberOfCacheRequests = new ExPerformanceCounter(base.CategoryName, "Total Cache Requests", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.NumberOfCacheRequests);
				long num = this.CacheHit.RawValue;
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

		internal MSExchangeADAccessCacheCountersInstance(string instanceName) : base(instanceName, "MSExchange ADAccess Cache")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.CacheHit = new ExPerformanceCounter(base.CategoryName, "Percentage of Cache Hit Last Minute", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CacheHit);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Cache Requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.NumberOfCacheRequests = new ExPerformanceCounter(base.CategoryName, "Total Cache Requests", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.NumberOfCacheRequests);
				long num = this.CacheHit.RawValue;
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

		public readonly ExPerformanceCounter CacheHit;

		public readonly ExPerformanceCounter NumberOfCacheRequests;
	}
}
