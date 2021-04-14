using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Inference.Performance
{
	internal sealed class StatefulComponentPerformanceCountersInstance : PerformanceCounterInstance
	{
		internal StatefulComponentPerformanceCountersInstance(string instanceName, StatefulComponentPerformanceCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeInference StatefulComponent")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.AverageSignalDispatchingLatency = new ExPerformanceCounter(base.CategoryName, "Average Signal Dispatching Latency (msec)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageSignalDispatchingLatency);
				this.AverageSignalDispatchingLatencyBase = new ExPerformanceCounter(base.CategoryName, "Average Signal Dispatching Latency Base", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageSignalDispatchingLatencyBase);
				long num = this.AverageSignalDispatchingLatency.RawValue;
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

		internal StatefulComponentPerformanceCountersInstance(string instanceName) : base(instanceName, "MSExchangeInference StatefulComponent")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.AverageSignalDispatchingLatency = new ExPerformanceCounter(base.CategoryName, "Average Signal Dispatching Latency (msec)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageSignalDispatchingLatency);
				this.AverageSignalDispatchingLatencyBase = new ExPerformanceCounter(base.CategoryName, "Average Signal Dispatching Latency Base", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageSignalDispatchingLatencyBase);
				long num = this.AverageSignalDispatchingLatency.RawValue;
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

		public readonly ExPerformanceCounter AverageSignalDispatchingLatency;

		public readonly ExPerformanceCounter AverageSignalDispatchingLatencyBase;
	}
}
