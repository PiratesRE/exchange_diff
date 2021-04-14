using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal sealed class SchedulerPerfCountersInstance : PerformanceCounterInstance
	{
		internal SchedulerPerfCountersInstance(string instanceName, SchedulerPerfCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeTransport Processing Scheduler")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.TotalReceived = new ExPerformanceCounter(base.CategoryName, "Total Received Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalReceived);
				this.TotalScheduled = new ExPerformanceCounter(base.CategoryName, "Total Scheduled Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalScheduled);
				this.ProcessingVelocity = new ExPerformanceCounter(base.CategoryName, "Processing Velocity", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ProcessingVelocity);
				this.TotalScopedQueues = new ExPerformanceCounter(base.CategoryName, "Total Scoped Queues", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalScopedQueues);
				this.ScopedQueuesCreationRate = new ExPerformanceCounter(base.CategoryName, "Scoped Queues created/min", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ScopedQueuesCreationRate);
				this.ScopedQueuesRemovalRate = new ExPerformanceCounter(base.CategoryName, "Scoped Queues removed/min", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ScopedQueuesRemovalRate);
				this.ThrottlingRate = new ExPerformanceCounter(base.CategoryName, "Items Throttled/min", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ThrottlingRate);
				this.OldestScopedQueueAge = new ExPerformanceCounter(base.CategoryName, "Oldest Scoped Queue Age", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.OldestScopedQueueAge);
				this.OldestLockAge = new ExPerformanceCounter(base.CategoryName, "Oldest Lock Age", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.OldestLockAge);
				long num = this.TotalReceived.RawValue;
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

		internal SchedulerPerfCountersInstance(string instanceName) : base(instanceName, "MSExchangeTransport Processing Scheduler")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.TotalReceived = new ExPerformanceCounter(base.CategoryName, "Total Received Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalReceived);
				this.TotalScheduled = new ExPerformanceCounter(base.CategoryName, "Total Scheduled Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalScheduled);
				this.ProcessingVelocity = new ExPerformanceCounter(base.CategoryName, "Processing Velocity", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ProcessingVelocity);
				this.TotalScopedQueues = new ExPerformanceCounter(base.CategoryName, "Total Scoped Queues", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalScopedQueues);
				this.ScopedQueuesCreationRate = new ExPerformanceCounter(base.CategoryName, "Scoped Queues created/min", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ScopedQueuesCreationRate);
				this.ScopedQueuesRemovalRate = new ExPerformanceCounter(base.CategoryName, "Scoped Queues removed/min", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ScopedQueuesRemovalRate);
				this.ThrottlingRate = new ExPerformanceCounter(base.CategoryName, "Items Throttled/min", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ThrottlingRate);
				this.OldestScopedQueueAge = new ExPerformanceCounter(base.CategoryName, "Oldest Scoped Queue Age", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.OldestScopedQueueAge);
				this.OldestLockAge = new ExPerformanceCounter(base.CategoryName, "Oldest Lock Age", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.OldestLockAge);
				long num = this.TotalReceived.RawValue;
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

		public readonly ExPerformanceCounter TotalReceived;

		public readonly ExPerformanceCounter TotalScheduled;

		public readonly ExPerformanceCounter ProcessingVelocity;

		public readonly ExPerformanceCounter TotalScopedQueues;

		public readonly ExPerformanceCounter ScopedQueuesCreationRate;

		public readonly ExPerformanceCounter ScopedQueuesRemovalRate;

		public readonly ExPerformanceCounter ThrottlingRate;

		public readonly ExPerformanceCounter OldestScopedQueueAge;

		public readonly ExPerformanceCounter OldestLockAge;
	}
}
