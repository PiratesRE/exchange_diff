using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal sealed class LatencyTrackerEndToEndPerfCountersInstance : PerformanceCounterInstance
	{
		internal LatencyTrackerEndToEndPerfCountersInstance(string instanceName, LatencyTrackerEndToEndPerfCountersInstance autoUpdateTotalInstance) : base(instanceName, LatencyTrackerEndToEndPerfCounters.CategoryName)
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.Percentile50 = new ExPerformanceCounter(base.CategoryName, "Percentile50", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.Percentile50);
				this.Percentile80 = new ExPerformanceCounter(base.CategoryName, "Percentile80", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.Percentile80);
				this.Percentile90 = new ExPerformanceCounter(base.CategoryName, "Percentile90", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.Percentile90);
				this.Percentile95 = new ExPerformanceCounter(base.CategoryName, "Percentile95", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.Percentile95);
				this.Percentile99 = new ExPerformanceCounter(base.CategoryName, "Percentile99", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.Percentile99);
				this.Percentile50Samples = new ExPerformanceCounter(base.CategoryName, "Percentile50Samples", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.Percentile50Samples);
				this.Percentile80Samples = new ExPerformanceCounter(base.CategoryName, "Percentile80Samples", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.Percentile80Samples);
				this.Percentile90Samples = new ExPerformanceCounter(base.CategoryName, "Percentile90Samples", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.Percentile90Samples);
				this.Percentile95Samples = new ExPerformanceCounter(base.CategoryName, "Percentile95Samples", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.Percentile95Samples);
				this.Percentile99Samples = new ExPerformanceCounter(base.CategoryName, "Percentile99Samples", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.Percentile99Samples);
				long num = this.Percentile50.RawValue;
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

		internal LatencyTrackerEndToEndPerfCountersInstance(string instanceName) : base(instanceName, LatencyTrackerEndToEndPerfCounters.CategoryName)
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.Percentile50 = new ExPerformanceCounter(base.CategoryName, "Percentile50", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.Percentile50);
				this.Percentile80 = new ExPerformanceCounter(base.CategoryName, "Percentile80", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.Percentile80);
				this.Percentile90 = new ExPerformanceCounter(base.CategoryName, "Percentile90", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.Percentile90);
				this.Percentile95 = new ExPerformanceCounter(base.CategoryName, "Percentile95", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.Percentile95);
				this.Percentile99 = new ExPerformanceCounter(base.CategoryName, "Percentile99", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.Percentile99);
				this.Percentile50Samples = new ExPerformanceCounter(base.CategoryName, "Percentile50Samples", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.Percentile50Samples);
				this.Percentile80Samples = new ExPerformanceCounter(base.CategoryName, "Percentile80Samples", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.Percentile80Samples);
				this.Percentile90Samples = new ExPerformanceCounter(base.CategoryName, "Percentile90Samples", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.Percentile90Samples);
				this.Percentile95Samples = new ExPerformanceCounter(base.CategoryName, "Percentile95Samples", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.Percentile95Samples);
				this.Percentile99Samples = new ExPerformanceCounter(base.CategoryName, "Percentile99Samples", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.Percentile99Samples);
				long num = this.Percentile50.RawValue;
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

		public readonly ExPerformanceCounter Percentile50;

		public readonly ExPerformanceCounter Percentile80;

		public readonly ExPerformanceCounter Percentile90;

		public readonly ExPerformanceCounter Percentile95;

		public readonly ExPerformanceCounter Percentile99;

		public readonly ExPerformanceCounter Percentile50Samples;

		public readonly ExPerformanceCounter Percentile80Samples;

		public readonly ExPerformanceCounter Percentile90Samples;

		public readonly ExPerformanceCounter Percentile95Samples;

		public readonly ExPerformanceCounter Percentile99Samples;
	}
}
