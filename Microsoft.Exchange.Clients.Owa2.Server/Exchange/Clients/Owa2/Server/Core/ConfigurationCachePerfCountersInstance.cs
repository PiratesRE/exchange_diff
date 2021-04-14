using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class ConfigurationCachePerfCountersInstance : PerformanceCounterInstance
	{
		internal ConfigurationCachePerfCountersInstance(string instanceName, ConfigurationCachePerfCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Owa Configuration Cache")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Configuration Cache requests per sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.Requests = new ExPerformanceCounter(base.CategoryName, "Configuration Cache Total requests", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.Requests);
				this.HitRatio = new ExPerformanceCounter(base.CategoryName, "Configuration Cache hit ratio", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.HitRatio);
				this.HitRatio_Base = new ExPerformanceCounter(base.CategoryName, "Configuration Cache hit ratio base counter", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.HitRatio_Base);
				this.CacheSize = new ExPerformanceCounter(base.CategoryName, "Configuration Cache size", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CacheSize);
				this.CacheSizeKB = new ExPerformanceCounter(base.CategoryName, "Configuration Cache size in KB", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CacheSizeKB);
				long num = this.Requests.RawValue;
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

		internal ConfigurationCachePerfCountersInstance(string instanceName) : base(instanceName, "MSExchange Owa Configuration Cache")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Configuration Cache requests per sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.Requests = new ExPerformanceCounter(base.CategoryName, "Configuration Cache Total requests", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.Requests);
				this.HitRatio = new ExPerformanceCounter(base.CategoryName, "Configuration Cache hit ratio", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.HitRatio);
				this.HitRatio_Base = new ExPerformanceCounter(base.CategoryName, "Configuration Cache hit ratio base counter", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.HitRatio_Base);
				this.CacheSize = new ExPerformanceCounter(base.CategoryName, "Configuration Cache size", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CacheSize);
				this.CacheSizeKB = new ExPerformanceCounter(base.CategoryName, "Configuration Cache size in KB", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CacheSizeKB);
				long num = this.Requests.RawValue;
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

		public readonly ExPerformanceCounter Requests;

		public readonly ExPerformanceCounter HitRatio;

		public readonly ExPerformanceCounter HitRatio_Base;

		public readonly ExPerformanceCounter CacheSize;

		public readonly ExPerformanceCounter CacheSizeKB;
	}
}
