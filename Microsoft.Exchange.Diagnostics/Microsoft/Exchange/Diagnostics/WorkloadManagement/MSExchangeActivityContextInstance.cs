using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Exchange.Diagnostics.WorkloadManagement
{
	internal sealed class MSExchangeActivityContextInstance : PerformanceCounterInstance
	{
		internal MSExchangeActivityContextInstance(string instanceName, MSExchangeActivityContextInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Activity Context Resources")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.TimeInResourcePerSecond = new ExPerformanceCounter(base.CategoryName, "Time in Resource per second", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TimeInResourcePerSecond);
				long num = this.TimeInResourcePerSecond.RawValue;
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

		internal MSExchangeActivityContextInstance(string instanceName) : base(instanceName, "MSExchange Activity Context Resources")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.TimeInResourcePerSecond = new ExPerformanceCounter(base.CategoryName, "Time in Resource per second", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TimeInResourcePerSecond);
				long num = this.TimeInResourcePerSecond.RawValue;
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

		public readonly ExPerformanceCounter TimeInResourcePerSecond;
	}
}
