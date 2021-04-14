using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal sealed class ShadowRedundancyInstancePerfCountersInstance : PerformanceCounterInstance
	{
		internal ShadowRedundancyInstancePerfCountersInstance(string instanceName, ShadowRedundancyInstancePerfCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeTransport Shadow Redundancy Host Info")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.ShadowQueueLength = new ExPerformanceCounter(base.CategoryName, "Shadow Queue Length", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ShadowQueueLength, new ExPerformanceCounter[0]);
				list.Add(this.ShadowQueueLength);
				this.ShadowHeartbeatLatencyAverageTime = new ExPerformanceCounter(base.CategoryName, "Shadow Heartbeat Latency Average Time", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ShadowHeartbeatLatencyAverageTime, new ExPerformanceCounter[0]);
				list.Add(this.ShadowHeartbeatLatencyAverageTime);
				this.ShadowHeartbeatLatencyAverageTimeBase = new ExPerformanceCounter(base.CategoryName, "Shadow Heartbeat Latency Average Time Base", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ShadowHeartbeatLatencyAverageTimeBase, new ExPerformanceCounter[0]);
				list.Add(this.ShadowHeartbeatLatencyAverageTimeBase);
				this.ShadowFailureCount = new ExPerformanceCounter(base.CategoryName, "Shadow Failure Count", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ShadowFailureCount, new ExPerformanceCounter[0]);
				list.Add(this.ShadowFailureCount);
				this.ResubmittedMessageCount = new ExPerformanceCounter(base.CategoryName, "Resubmitted Message Count", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ResubmittedMessageCount, new ExPerformanceCounter[0]);
				list.Add(this.ResubmittedMessageCount);
				this.HeartbeatFailureCount = new ExPerformanceCounter(base.CategoryName, "Shadow Heartbeat Failure Count", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.HeartbeatFailureCount, new ExPerformanceCounter[0]);
				list.Add(this.HeartbeatFailureCount);
				this.ShadowedMessageCount = new ExPerformanceCounter(base.CategoryName, "Shadowed Message Count", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ShadowedMessageCount, new ExPerformanceCounter[0]);
				list.Add(this.ShadowedMessageCount);
				long num = this.ShadowQueueLength.RawValue;
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

		internal ShadowRedundancyInstancePerfCountersInstance(string instanceName) : base(instanceName, "MSExchangeTransport Shadow Redundancy Host Info")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.ShadowQueueLength = new ExPerformanceCounter(base.CategoryName, "Shadow Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ShadowQueueLength);
				this.ShadowHeartbeatLatencyAverageTime = new ExPerformanceCounter(base.CategoryName, "Shadow Heartbeat Latency Average Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ShadowHeartbeatLatencyAverageTime);
				this.ShadowHeartbeatLatencyAverageTimeBase = new ExPerformanceCounter(base.CategoryName, "Shadow Heartbeat Latency Average Time Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ShadowHeartbeatLatencyAverageTimeBase);
				this.ShadowFailureCount = new ExPerformanceCounter(base.CategoryName, "Shadow Failure Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ShadowFailureCount);
				this.ResubmittedMessageCount = new ExPerformanceCounter(base.CategoryName, "Resubmitted Message Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ResubmittedMessageCount);
				this.HeartbeatFailureCount = new ExPerformanceCounter(base.CategoryName, "Shadow Heartbeat Failure Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.HeartbeatFailureCount);
				this.ShadowedMessageCount = new ExPerformanceCounter(base.CategoryName, "Shadowed Message Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ShadowedMessageCount);
				long num = this.ShadowQueueLength.RawValue;
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

		public readonly ExPerformanceCounter ShadowQueueLength;

		public readonly ExPerformanceCounter ShadowHeartbeatLatencyAverageTime;

		public readonly ExPerformanceCounter ShadowHeartbeatLatencyAverageTimeBase;

		public readonly ExPerformanceCounter ShadowFailureCount;

		public readonly ExPerformanceCounter ResubmittedMessageCount;

		public readonly ExPerformanceCounter HeartbeatFailureCount;

		public readonly ExPerformanceCounter ShadowedMessageCount;
	}
}
