using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal sealed class MSExchangeUserWorkloadManagerInstance : PerformanceCounterInstance
	{
		internal MSExchangeUserWorkloadManagerInstance(string instanceName, MSExchangeUserWorkloadManagerInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange User WorkloadManager")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.AverageTaskWaitTime = new ExPerformanceCounter(base.CategoryName, "Average Task Wait Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageTaskWaitTime);
				this.MaxTaskQueueLength = new ExPerformanceCounter(base.CategoryName, "Max Task Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MaxTaskQueueLength);
				this.MaxWorkerThreadCount = new ExPerformanceCounter(base.CategoryName, "Max Worker Thread Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MaxWorkerThreadCount);
				this.TaskQueueLength = new ExPerformanceCounter(base.CategoryName, "Task Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TaskQueueLength);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "New Task Rejections/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.TotalNewTaskRejections = new ExPerformanceCounter(base.CategoryName, "New Task Rejections", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.TotalNewTaskRejections);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "New Tasks Submitted/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.TotalNewTasks = new ExPerformanceCounter(base.CategoryName, "Total New Tasks", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.TotalNewTasks);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Task Failures/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.TotalTaskExecutionFailures = new ExPerformanceCounter(base.CategoryName, "Total Task Failures", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(this.TotalTaskExecutionFailures);
				this.CurrentDelayedTasks = new ExPerformanceCounter(base.CategoryName, "Current Delayed Tasks", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CurrentDelayedTasks);
				this.MaxDelayPerMinute = new ExPerformanceCounter(base.CategoryName, "Max Delay Per Minute", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MaxDelayPerMinute);
				this.UsersDelayedXMillisecondsPerMinute = new ExPerformanceCounter(base.CategoryName, "Users Delayed X Milliseconds", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.UsersDelayedXMillisecondsPerMinute);
				this.DelayTimeThreshold = new ExPerformanceCounter(base.CategoryName, "Delay Time Threshold", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DelayTimeThreshold);
				this.TaskTimeoutsPerMinute = new ExPerformanceCounter(base.CategoryName, "Task Timeouts Per Minute", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TaskTimeoutsPerMinute);
				this.MaxDelayedTasks = new ExPerformanceCounter(base.CategoryName, "Max Delayed Tasks", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MaxDelayedTasks);
				long num = this.AverageTaskWaitTime.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter4 in list)
					{
						exPerformanceCounter4.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal MSExchangeUserWorkloadManagerInstance(string instanceName) : base(instanceName, "MSExchange User WorkloadManager")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.AverageTaskWaitTime = new ExPerformanceCounter(base.CategoryName, "Average Task Wait Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageTaskWaitTime);
				this.MaxTaskQueueLength = new ExPerformanceCounter(base.CategoryName, "Max Task Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MaxTaskQueueLength);
				this.MaxWorkerThreadCount = new ExPerformanceCounter(base.CategoryName, "Max Worker Thread Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MaxWorkerThreadCount);
				this.TaskQueueLength = new ExPerformanceCounter(base.CategoryName, "Task Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TaskQueueLength);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "New Task Rejections/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.TotalNewTaskRejections = new ExPerformanceCounter(base.CategoryName, "New Task Rejections", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.TotalNewTaskRejections);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "New Tasks Submitted/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.TotalNewTasks = new ExPerformanceCounter(base.CategoryName, "Total New Tasks", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.TotalNewTasks);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Task Failures/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.TotalTaskExecutionFailures = new ExPerformanceCounter(base.CategoryName, "Total Task Failures", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(this.TotalTaskExecutionFailures);
				this.CurrentDelayedTasks = new ExPerformanceCounter(base.CategoryName, "Current Delayed Tasks", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CurrentDelayedTasks);
				this.MaxDelayPerMinute = new ExPerformanceCounter(base.CategoryName, "Max Delay Per Minute", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MaxDelayPerMinute);
				this.UsersDelayedXMillisecondsPerMinute = new ExPerformanceCounter(base.CategoryName, "Users Delayed X Milliseconds", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.UsersDelayedXMillisecondsPerMinute);
				this.DelayTimeThreshold = new ExPerformanceCounter(base.CategoryName, "Delay Time Threshold", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DelayTimeThreshold);
				this.TaskTimeoutsPerMinute = new ExPerformanceCounter(base.CategoryName, "Task Timeouts Per Minute", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TaskTimeoutsPerMinute);
				this.MaxDelayedTasks = new ExPerformanceCounter(base.CategoryName, "Max Delayed Tasks", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MaxDelayedTasks);
				long num = this.AverageTaskWaitTime.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter4 in list)
					{
						exPerformanceCounter4.Close();
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

		public readonly ExPerformanceCounter AverageTaskWaitTime;

		public readonly ExPerformanceCounter MaxTaskQueueLength;

		public readonly ExPerformanceCounter MaxWorkerThreadCount;

		public readonly ExPerformanceCounter TaskQueueLength;

		public readonly ExPerformanceCounter TotalNewTaskRejections;

		public readonly ExPerformanceCounter TotalNewTasks;

		public readonly ExPerformanceCounter TotalTaskExecutionFailures;

		public readonly ExPerformanceCounter CurrentDelayedTasks;

		public readonly ExPerformanceCounter MaxDelayPerMinute;

		public readonly ExPerformanceCounter UsersDelayedXMillisecondsPerMinute;

		public readonly ExPerformanceCounter DelayTimeThreshold;

		public readonly ExPerformanceCounter TaskTimeoutsPerMinute;

		public readonly ExPerformanceCounter MaxDelayedTasks;
	}
}
