using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal sealed class MSExchangeWorkloadManagementWorkloadInstance : PerformanceCounterInstance
	{
		internal MSExchangeWorkloadManagementWorkloadInstance(string instanceName, MSExchangeWorkloadManagementWorkloadInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange WorkloadManagement Workloads")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.ActiveTasks = new ExPerformanceCounter(base.CategoryName, "ActiveTasks", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ActiveTasks);
				this.QueuedTasks = new ExPerformanceCounter(base.CategoryName, "QueuedTasks", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.QueuedTasks);
				this.BlockedTasks = new ExPerformanceCounter(base.CategoryName, "BlockedTasks", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.BlockedTasks);
				this.CompletedTasks = new ExPerformanceCounter(base.CategoryName, "CompletedTasks", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CompletedTasks);
				this.YieldedTasks = new ExPerformanceCounter(base.CategoryName, "YieldedTasks", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.YieldedTasks);
				this.TasksPerMinute = new ExPerformanceCounter(base.CategoryName, "Tasks Per Minute", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TasksPerMinute);
				this.AverageTaskStepLength = new ExPerformanceCounter(base.CategoryName, "Average Step Execution Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageTaskStepLength);
				this.Active = new ExPerformanceCounter(base.CategoryName, "Active", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.Active);
				long num = this.ActiveTasks.RawValue;
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

		internal MSExchangeWorkloadManagementWorkloadInstance(string instanceName) : base(instanceName, "MSExchange WorkloadManagement Workloads")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.ActiveTasks = new ExPerformanceCounter(base.CategoryName, "ActiveTasks", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ActiveTasks);
				this.QueuedTasks = new ExPerformanceCounter(base.CategoryName, "QueuedTasks", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.QueuedTasks);
				this.BlockedTasks = new ExPerformanceCounter(base.CategoryName, "BlockedTasks", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.BlockedTasks);
				this.CompletedTasks = new ExPerformanceCounter(base.CategoryName, "CompletedTasks", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CompletedTasks);
				this.YieldedTasks = new ExPerformanceCounter(base.CategoryName, "YieldedTasks", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.YieldedTasks);
				this.TasksPerMinute = new ExPerformanceCounter(base.CategoryName, "Tasks Per Minute", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TasksPerMinute);
				this.AverageTaskStepLength = new ExPerformanceCounter(base.CategoryName, "Average Step Execution Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageTaskStepLength);
				this.Active = new ExPerformanceCounter(base.CategoryName, "Active", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.Active);
				long num = this.ActiveTasks.RawValue;
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

		public readonly ExPerformanceCounter ActiveTasks;

		public readonly ExPerformanceCounter QueuedTasks;

		public readonly ExPerformanceCounter BlockedTasks;

		public readonly ExPerformanceCounter CompletedTasks;

		public readonly ExPerformanceCounter YieldedTasks;

		public readonly ExPerformanceCounter TasksPerMinute;

		public readonly ExPerformanceCounter AverageTaskStepLength;

		public readonly ExPerformanceCounter Active;
	}
}
