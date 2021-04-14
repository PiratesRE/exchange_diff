using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Security.Authentication
{
	internal sealed class AdfsAuthCountersInstance : PerformanceCounterInstance
	{
		internal AdfsAuthCountersInstance(string instanceName, AdfsAuthCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange AdfsAuth")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.AdfsFedAuthModuleKeyCacheSize = new ExPerformanceCounter(base.CategoryName, "AdfsFedAuth Module Key Cache Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AdfsFedAuthModuleKeyCacheSize);
				this.AdfsFedAuthModuleKeyCacheHitsRate = new ExPerformanceCounter(base.CategoryName, "AdfsFedAuth Module Key Cache Hits Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AdfsFedAuthModuleKeyCacheHitsRate);
				this.AdfsFedAuthModuleCacheHitsRateBase = new ExPerformanceCounter(base.CategoryName, "AdfsFedAuth Module Key Cache Hits Rate Base", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AdfsFedAuthModuleCacheHitsRateBase);
				long num = this.AdfsFedAuthModuleKeyCacheSize.RawValue;
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

		internal AdfsAuthCountersInstance(string instanceName) : base(instanceName, "MSExchange AdfsAuth")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.AdfsFedAuthModuleKeyCacheSize = new ExPerformanceCounter(base.CategoryName, "AdfsFedAuth Module Key Cache Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AdfsFedAuthModuleKeyCacheSize);
				this.AdfsFedAuthModuleKeyCacheHitsRate = new ExPerformanceCounter(base.CategoryName, "AdfsFedAuth Module Key Cache Hits Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AdfsFedAuthModuleKeyCacheHitsRate);
				this.AdfsFedAuthModuleCacheHitsRateBase = new ExPerformanceCounter(base.CategoryName, "AdfsFedAuth Module Key Cache Hits Rate Base", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AdfsFedAuthModuleCacheHitsRateBase);
				long num = this.AdfsFedAuthModuleKeyCacheSize.RawValue;
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

		public readonly ExPerformanceCounter AdfsFedAuthModuleKeyCacheSize;

		public readonly ExPerformanceCounter AdfsFedAuthModuleKeyCacheHitsRate;

		public readonly ExPerformanceCounter AdfsFedAuthModuleCacheHitsRateBase;
	}
}
