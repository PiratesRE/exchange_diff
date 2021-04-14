using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	internal sealed class WTFPerfCountersInstance : PerformanceCounterInstance
	{
		internal WTFPerfCountersInstance(string instanceName, WTFPerfCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeWorkerTaskFramework")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.WorkItemExecutionRate = new ExPerformanceCounter(base.CategoryName, "Workitem Execution Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.WorkItemExecutionRate);
				this.WorkItemCount = new ExPerformanceCounter(base.CategoryName, "Active Workitem Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.WorkItemCount);
				this.TimedOutWorkItemCount = new ExPerformanceCounter(base.CategoryName, "Timed Out Workitem Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TimedOutWorkItemCount);
				this.WorkItemRetrievalRate = new ExPerformanceCounter(base.CategoryName, "Workitem Retrieval Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.WorkItemRetrievalRate);
				this.ThrottleRate = new ExPerformanceCounter(base.CategoryName, "Throttle Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ThrottleRate);
				this.QueryRate = new ExPerformanceCounter(base.CategoryName, "Query Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.QueryRate);
				this.SchedulingLatency = new ExPerformanceCounter(base.CategoryName, "Scheduling Latency", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.SchedulingLatency);
				this.PoisonedWorkItemCount = new ExPerformanceCounter(base.CategoryName, "Poisoned Workitem Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PoisonedWorkItemCount);
				long num = this.WorkItemExecutionRate.RawValue;
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

		internal WTFPerfCountersInstance(string instanceName) : base(instanceName, "MSExchangeWorkerTaskFramework")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.WorkItemExecutionRate = new ExPerformanceCounter(base.CategoryName, "Workitem Execution Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.WorkItemExecutionRate);
				this.WorkItemCount = new ExPerformanceCounter(base.CategoryName, "Active Workitem Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.WorkItemCount);
				this.TimedOutWorkItemCount = new ExPerformanceCounter(base.CategoryName, "Timed Out Workitem Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TimedOutWorkItemCount);
				this.WorkItemRetrievalRate = new ExPerformanceCounter(base.CategoryName, "Workitem Retrieval Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.WorkItemRetrievalRate);
				this.ThrottleRate = new ExPerformanceCounter(base.CategoryName, "Throttle Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ThrottleRate);
				this.QueryRate = new ExPerformanceCounter(base.CategoryName, "Query Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.QueryRate);
				this.SchedulingLatency = new ExPerformanceCounter(base.CategoryName, "Scheduling Latency", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.SchedulingLatency);
				this.PoisonedWorkItemCount = new ExPerformanceCounter(base.CategoryName, "Poisoned Workitem Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PoisonedWorkItemCount);
				long num = this.WorkItemExecutionRate.RawValue;
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

		public readonly ExPerformanceCounter WorkItemExecutionRate;

		public readonly ExPerformanceCounter WorkItemCount;

		public readonly ExPerformanceCounter TimedOutWorkItemCount;

		public readonly ExPerformanceCounter WorkItemRetrievalRate;

		public readonly ExPerformanceCounter ThrottleRate;

		public readonly ExPerformanceCounter QueryRate;

		public readonly ExPerformanceCounter SchedulingLatency;

		public readonly ExPerformanceCounter PoisonedWorkItemCount;
	}
}
