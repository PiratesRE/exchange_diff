using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Storage.ResourceHealth;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric.Scheduling
{
	internal class ResourceBasedTaskScheduler : TaskScheduler
	{
		static ResourceBasedTaskScheduler()
		{
			int maxThreadCount;
			int num;
			ThreadPool.GetMaxThreads(out maxThreadCount, out num);
			UserWorkloadManager.Initialize(maxThreadCount, TaskDistributionSettings.MaxQueuePerBlock, TaskDistributionSettings.MaxQueuePerBlock, TimeSpan.FromHours(4.0), null);
		}

		private ResourceBasedTaskScheduler()
		{
		}

		public static TaskScheduler Instance
		{
			get
			{
				return ResourceBasedTaskScheduler.instance;
			}
		}

		protected override IEnumerable<Task> GetScheduledTasks()
		{
			yield break;
		}

		protected override void QueueTask(Task task)
		{
			UserWorkloadManager.Singleton.TrySubmitNewTask(new ResourceBasedTaskScheduler.TaskDistributionWrappedTask(task, new Func<Task, bool>(base.TryExecuteTask)));
		}

		protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
		{
			return false;
		}

		private static TaskScheduler instance = new ResourceBasedTaskScheduler();

		private class TaskDistributionWrappedTask : ITask
		{
			public TaskDistributionWrappedTask(Task task, Func<Task, bool> tryExecuteTask)
			{
				this.task = task;
				this.input = (task.AsyncState as ComplianceMessage);
				this.tryExecuteTask = tryExecuteTask;
			}

			string ITask.Description
			{
				get
				{
					return base.GetType().Name;
				}
				set
				{
				}
			}

			TimeSpan ITask.MaxExecutionTime
			{
				get
				{
					return TaskDistributionSettings.ApplicationExecutionTime;
				}
			}

			object ITask.State { get; set; }

			WorkloadSettings ITask.WorkloadSettings
			{
				get
				{
					return new WorkloadSettings(WorkloadType.DarRuntime, true);
				}
			}

			IBudget ITask.Budget
			{
				get
				{
					return StandardBudget.Acquire(new UnthrottledBudgetKey("TaskDistribution", BudgetType.Anonymous));
				}
			}

			TaskExecuteResult ITask.CancelStep(LocalizedException exception)
			{
				return TaskExecuteResult.ProcessingComplete;
			}

			void ITask.Complete(TimeSpan queueAndDelayTime, TimeSpan totalTime)
			{
			}

			TaskExecuteResult ITask.Execute(TimeSpan queueAndDelayTime, TimeSpan totalTime)
			{
				if (this.tryExecuteTask(this.task))
				{
					return TaskExecuteResult.ProcessingComplete;
				}
				return TaskExecuteResult.Undefined;
			}

			IActivityScope ITask.GetActivityScope()
			{
				return ActivityContext.Start(null);
			}

			void ITask.Timeout(TimeSpan queueAndDelayTime, TimeSpan totalTime)
			{
			}

			void ITask.Cancel()
			{
			}

			ResourceKey[] ITask.GetResources()
			{
				if (this.input != null && this.input.MessageTarget != null && this.input.MessageTarget.Database != Guid.Empty)
				{
					return new ResourceKey[]
					{
						ProcessorResourceKey.Local,
						new MdbResourceHealthMonitorKey(this.input.MessageTarget.Database),
						new MdbReplicationResourceHealthMonitorKey(this.input.MessageTarget.Database),
						new CiAgeOfLastNotificationResourceKey(this.input.MessageTarget.Database)
					};
				}
				return new ResourceKey[]
				{
					ProcessorResourceKey.Local
				};
			}

			private Task task;

			private Func<Task, bool> tryExecuteTask;

			private ComplianceMessage input;
		}
	}
}
