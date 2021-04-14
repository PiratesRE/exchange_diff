using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal sealed class ResourceThrottlingPerfCountersInstance : PerformanceCounterInstance
	{
		internal ResourceThrottlingPerfCountersInstance(string instanceName, ResourceThrottlingPerfCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeTransport ResourceThrottling")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.BackPressureTime = new ExPerformanceCounter(base.CategoryName, "Backpressure: Current Sustained Time in seconds", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.BackPressureTime);
				this.ResourceMeterLongestCallDuration = new ExPerformanceCounter(base.CategoryName, "Backpressure: Longest Resource Meter call in milliseconds", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ResourceMeterLongestCallDuration);
				this.ResourceObserverLongestCallDuration = new ExPerformanceCounter(base.CategoryName, "Backpressure: Longest Resource Observer call in milliseconds", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ResourceObserverLongestCallDuration);
				long num = this.BackPressureTime.RawValue;
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

		internal ResourceThrottlingPerfCountersInstance(string instanceName) : base(instanceName, "MSExchangeTransport ResourceThrottling")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.BackPressureTime = new ExPerformanceCounter(base.CategoryName, "Backpressure: Current Sustained Time in seconds", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.BackPressureTime);
				this.ResourceMeterLongestCallDuration = new ExPerformanceCounter(base.CategoryName, "Backpressure: Longest Resource Meter call in milliseconds", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ResourceMeterLongestCallDuration);
				this.ResourceObserverLongestCallDuration = new ExPerformanceCounter(base.CategoryName, "Backpressure: Longest Resource Observer call in milliseconds", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ResourceObserverLongestCallDuration);
				long num = this.BackPressureTime.RawValue;
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

		public readonly ExPerformanceCounter BackPressureTime;

		public readonly ExPerformanceCounter ResourceMeterLongestCallDuration;

		public readonly ExPerformanceCounter ResourceObserverLongestCallDuration;
	}
}
