using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Search.Performance
{
	internal sealed class MdbCachePerfCountersInstance : PerformanceCounterInstance
	{
		internal MdbCachePerfCountersInstance(string instanceName, MdbCachePerfCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeSearch MailboxSession Cache")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.NumberOfRequest = new ExPerformanceCounter(base.CategoryName, "Cache Requests", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfRequest);
				this.NumberOfCacheHit = new ExPerformanceCounter(base.CategoryName, "Cache Hits", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfCacheHit);
				this.NumberOfCacheMiss = new ExPerformanceCounter(base.CategoryName, "Cache Misses", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfCacheMiss);
				this.CacheHitRatio = new ExPerformanceCounter(base.CategoryName, "Cache Hit Ratio", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CacheHitRatio);
				this.CacheHitRatioBase = new ExPerformanceCounter(base.CategoryName, "Cache Hit Ratio Base", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CacheHitRatioBase);
				this.CacheMissRatio = new ExPerformanceCounter(base.CategoryName, "Cache Miss Ratio", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CacheMissRatio);
				this.CacheMissRatioBase = new ExPerformanceCounter(base.CategoryName, "Cache Miss Ratio Base", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CacheMissRatioBase);
				long num = this.NumberOfRequest.RawValue;
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

		internal MdbCachePerfCountersInstance(string instanceName) : base(instanceName, "MSExchangeSearch MailboxSession Cache")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.NumberOfRequest = new ExPerformanceCounter(base.CategoryName, "Cache Requests", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfRequest);
				this.NumberOfCacheHit = new ExPerformanceCounter(base.CategoryName, "Cache Hits", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfCacheHit);
				this.NumberOfCacheMiss = new ExPerformanceCounter(base.CategoryName, "Cache Misses", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfCacheMiss);
				this.CacheHitRatio = new ExPerformanceCounter(base.CategoryName, "Cache Hit Ratio", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CacheHitRatio);
				this.CacheHitRatioBase = new ExPerformanceCounter(base.CategoryName, "Cache Hit Ratio Base", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CacheHitRatioBase);
				this.CacheMissRatio = new ExPerformanceCounter(base.CategoryName, "Cache Miss Ratio", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CacheMissRatio);
				this.CacheMissRatioBase = new ExPerformanceCounter(base.CategoryName, "Cache Miss Ratio Base", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CacheMissRatioBase);
				long num = this.NumberOfRequest.RawValue;
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

		public readonly ExPerformanceCounter NumberOfRequest;

		public readonly ExPerformanceCounter NumberOfCacheHit;

		public readonly ExPerformanceCounter NumberOfCacheMiss;

		public readonly ExPerformanceCounter CacheHitRatio;

		public readonly ExPerformanceCounter CacheHitRatioBase;

		public readonly ExPerformanceCounter CacheMissRatio;

		public readonly ExPerformanceCounter CacheMissRatioBase;
	}
}
