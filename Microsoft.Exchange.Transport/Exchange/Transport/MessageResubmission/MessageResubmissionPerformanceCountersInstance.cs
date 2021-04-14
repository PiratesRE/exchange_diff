using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.MessageResubmission
{
	internal sealed class MessageResubmissionPerformanceCountersInstance : PerformanceCounterInstance
	{
		internal MessageResubmissionPerformanceCountersInstance(string instanceName, MessageResubmissionPerformanceCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeTransport Safety Net")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.SafetyNetResubmissionCount = new ExPerformanceCounter(base.CategoryName, "Safety Net Resubmission Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.SafetyNetResubmissionCount);
				this.ShadowSafetyNetResubmissionCount = new ExPerformanceCounter(base.CategoryName, "Shadow Safety Net Resubmission Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ShadowSafetyNetResubmissionCount);
				this.ResubmitLatencyAverageTime = new ExPerformanceCounter(base.CategoryName, "Resubmit Latency Average Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ResubmitLatencyAverageTime);
				this.ResubmitLatencyAverageTimeBase = new ExPerformanceCounter(base.CategoryName, "Resubmit Latency Average Time Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ResubmitLatencyAverageTimeBase);
				this.ResubmitRequestCount = new ExPerformanceCounter(base.CategoryName, "Resubmit Request Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ResubmitRequestCount);
				this.RecentResubmitRequestCount = new ExPerformanceCounter(base.CategoryName, "Safety Net Resubmit Request Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.RecentResubmitRequestCount);
				this.RecentShadowResubmitRequestCount = new ExPerformanceCounter(base.CategoryName, "Shadow Safety Net Resubmit Request Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.RecentShadowResubmitRequestCount);
				this.AverageResubmitRequestTimeSpan = new ExPerformanceCounter(base.CategoryName, "Average Safety Net Resubmit Request Time Span", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageResubmitRequestTimeSpan);
				long num = this.SafetyNetResubmissionCount.RawValue;
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

		internal MessageResubmissionPerformanceCountersInstance(string instanceName) : base(instanceName, "MSExchangeTransport Safety Net")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.SafetyNetResubmissionCount = new ExPerformanceCounter(base.CategoryName, "Safety Net Resubmission Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.SafetyNetResubmissionCount);
				this.ShadowSafetyNetResubmissionCount = new ExPerformanceCounter(base.CategoryName, "Shadow Safety Net Resubmission Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ShadowSafetyNetResubmissionCount);
				this.ResubmitLatencyAverageTime = new ExPerformanceCounter(base.CategoryName, "Resubmit Latency Average Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ResubmitLatencyAverageTime);
				this.ResubmitLatencyAverageTimeBase = new ExPerformanceCounter(base.CategoryName, "Resubmit Latency Average Time Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ResubmitLatencyAverageTimeBase);
				this.ResubmitRequestCount = new ExPerformanceCounter(base.CategoryName, "Resubmit Request Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ResubmitRequestCount);
				this.RecentResubmitRequestCount = new ExPerformanceCounter(base.CategoryName, "Safety Net Resubmit Request Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.RecentResubmitRequestCount);
				this.RecentShadowResubmitRequestCount = new ExPerformanceCounter(base.CategoryName, "Shadow Safety Net Resubmit Request Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.RecentShadowResubmitRequestCount);
				this.AverageResubmitRequestTimeSpan = new ExPerformanceCounter(base.CategoryName, "Average Safety Net Resubmit Request Time Span", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageResubmitRequestTimeSpan);
				long num = this.SafetyNetResubmissionCount.RawValue;
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

		public readonly ExPerformanceCounter SafetyNetResubmissionCount;

		public readonly ExPerformanceCounter ShadowSafetyNetResubmissionCount;

		public readonly ExPerformanceCounter ResubmitLatencyAverageTime;

		public readonly ExPerformanceCounter ResubmitLatencyAverageTimeBase;

		public readonly ExPerformanceCounter ResubmitRequestCount;

		public readonly ExPerformanceCounter RecentResubmitRequestCount;

		public readonly ExPerformanceCounter RecentShadowResubmitRequestCount;

		public readonly ExPerformanceCounter AverageResubmitRequestTimeSpan;
	}
}
