using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Office.CompliancePolicy.Dar
{
	public class DarTaskManager
	{
		public DarTaskManager(DarServiceProvider darServiceProvider)
		{
			this.darServiceProvider = darServiceProvider;
		}

		public DarServiceProvider ServiceProvider
		{
			get
			{
				return this.darServiceProvider;
			}
		}

		public ExecutionLog ExecutionLog
		{
			get
			{
				if (this.darServiceProvider == null)
				{
					return null;
				}
				return this.darServiceProvider.ExecutionLog;
			}
		}

		public static string GetPerfCounterFromTaskState(DarTaskState state)
		{
			switch (state)
			{
			case DarTaskState.None:
				return "DarTasksInStateNone";
			case DarTaskState.Ready:
				return "DarTasksInStateReady";
			case DarTaskState.Running:
				return "DarTasksInStateRunning";
			case DarTaskState.Completed:
				return "DarTasksInStateCompleted";
			case DarTaskState.Failed:
				return "DarTasksInStateFailed";
			case DarTaskState.Cancelled:
				return "DarTasksInStateCancelled";
			default:
				return string.Empty;
			}
		}

		public void Enqueue(DarTask task)
		{
			task.TaskState = DarTaskState.Ready;
			task.TaskQueuedTime = DateTime.UtcNow;
			task.SaveStateToSerializedData(this);
			this.darServiceProvider.ExecutionLog.LogInformation("DarTaskManager", null, task.CorrelationId, string.Format("Enqueuing {0}", task), new KeyValuePair<string, object>[]
			{
				new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.DarTaskManager0.ToString())
			});
			this.darServiceProvider.DarTaskQueue.Enqueue(task);
			this.darServiceProvider.PerformanceCounter.Increment("DarTasksInStateReady");
		}

		public void Cancel(string taskId)
		{
		}

		public void Pause(string taskId)
		{
		}

		public void Resume(string taskId)
		{
		}

		public IEnumerable<DarTask> Dequeue(int taskCount, DarTaskCategory category, object availableResource = null)
		{
			IEnumerable<DarTask> enumerable = this.DeserializeTaskData(this.darServiceProvider.DarTaskQueue.Dequeue(taskCount, category, availableResource));
			this.darServiceProvider.ExecutionLog.LogInformation("DarTaskManager", null, null, string.Format("Dequeued {0}/{1} task(s), category:{2}, availableResources:{3}. Dequeued Tasks: {4}", new object[]
			{
				enumerable.Count<DarTask>(),
				taskCount.ToString(),
				category.ToString(),
				availableResource ?? "<null>",
				string.Join<DarTask>(",", enumerable = enumerable.ToArray<DarTask>())
			}), new KeyValuePair<string, object>[]
			{
				new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.DarTaskManager1.ToString())
			});
			return enumerable;
		}

		public DarTaskExecutionCommand ShouldContinue(DarTask task, out string additionalInformation)
		{
			return this.darServiceProvider.DarWorkloadHost.ShouldContinue(task, out additionalInformation);
		}

		public void UpdateTaskState(DarTask darTask)
		{
			this.UpdateTaskState(darTask, (DarTaskExecutionResult)(-1));
		}

		public void UpdateTaskState(DarTask darTask, DarTaskExecutionResult executionResult)
		{
			if (darTask == null)
			{
				throw new ArgumentNullException("darTask");
			}
			this.darServiceProvider.ExecutionLog.LogInformation("DarTaskManager", null, darTask.CorrelationId, string.Format("Updating task {0} state to {1} and serializedTaskData is {2}", darTask.Id, darTask.TaskState, darTask.SerializedTaskData), new KeyValuePair<string, object>[]
			{
				new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.DarTaskManager2.ToString())
			});
			this.darServiceProvider.DarTaskQueue.UpdateTask(darTask);
			if (darTask.TaskState == DarTaskState.Completed)
			{
				TimeSpan timeSpan = darTask.TaskCompletionTime.Subtract(darTask.TaskQueuedTime);
				this.darServiceProvider.ExecutionLog.LogInformation("DarTaskManager", null, darTask.CorrelationId, string.Format("From queue time to finish task took {0}", timeSpan), new KeyValuePair<string, object>[]
				{
					new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.DarTaskManager3.ToString())
				});
			}
			if (darTask.PreviousTaskState == DarTaskState.Ready && darTask.TaskState == DarTaskState.Running && darTask.TaskRetryCurrentCount == 0)
			{
				this.ServiceProvider.PerformanceCounter.IncrementBy("DarTasksInStateReady", -1L);
				this.ServiceProvider.PerformanceCounter.Increment("DarTasksInStateRunning");
			}
			if (darTask.PreviousTaskState == DarTaskState.Running && (darTask.TaskState == DarTaskState.Cancelled || darTask.TaskState == DarTaskState.Failed || darTask.TaskState == DarTaskState.Completed))
			{
				this.ServiceProvider.PerformanceCounter.IncrementBy("DarTasksInStateRunning", -1L);
				this.ServiceProvider.PerformanceCounter.Increment(DarTaskManager.GetPerfCounterFromTaskState(darTask.TaskState));
				if (darTask.TaskState == DarTaskState.Completed)
				{
					this.ServiceProvider.PerformanceCounter.IncrementBy("DarTaskAverageDuration", (darTask.TaskCompletionTime - darTask.TaskQueuedTime).Ticks, "DarTaskAverageDurationBase");
				}
			}
			if (darTask.PreviousTaskState != darTask.TaskState && executionResult == DarTaskExecutionResult.TransientError && darTask.TaskRetryCurrentCount == 1)
			{
				this.ServiceProvider.PerformanceCounter.Increment("DarTasksTransientFailed");
			}
		}

		public IEnumerable<DarTask> GetCompletedTasks(DateTime minCompletionTime, string taskType = null, string tenantId = null)
		{
			return this.DeserializeTaskData(this.darServiceProvider.DarTaskQueue.GetCompletedTasks(minCompletionTime, taskType, tenantId));
		}

		private IEnumerable<DarTask> DeserializeTaskData(IEnumerable<DarTask> rawTasks)
		{
			foreach (DarTask task in rawTasks)
			{
				task.RestoreStateFromSerializedData(this);
				yield return task;
			}
			yield break;
		}

		private const string LoggingClientId = "DarTaskManager";

		private readonly DarServiceProvider darServiceProvider;
	}
}
