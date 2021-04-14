using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory
{
	internal sealed class MSExchangeResourceLoadInstance : PerformanceCounterInstance
	{
		internal MSExchangeResourceLoadInstance(string instanceName, MSExchangeResourceLoadInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Resource Load")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.ResourceMetric = new ExPerformanceCounter(base.CategoryName, "Resource Metric", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ResourceMetric);
				this.ResourceLoad = new ExPerformanceCounter(base.CategoryName, "Resource Load", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ResourceLoad);
				long num = this.ResourceMetric.RawValue;
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

		internal MSExchangeResourceLoadInstance(string instanceName) : base(instanceName, "MSExchange Resource Load")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.ResourceMetric = new ExPerformanceCounter(base.CategoryName, "Resource Metric", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ResourceMetric);
				this.ResourceLoad = new ExPerformanceCounter(base.CategoryName, "Resource Load", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ResourceLoad);
				long num = this.ResourceMetric.RawValue;
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

		public readonly ExPerformanceCounter ResourceMetric;

		public readonly ExPerformanceCounter ResourceLoad;
	}
}
