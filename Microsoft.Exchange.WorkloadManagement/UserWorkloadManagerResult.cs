using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.WorkloadManagement
{
	public class UserWorkloadManagerResult
	{
		public int MaxTasksQueued { get; set; }

		public int MaxThreadCount { get; set; }

		public string MaxDelayCacheTime { get; set; }

		public int CurrentWorkerThreads { get; set; }

		public bool IsQueueFull { get; set; }

		public bool Canceled { get; set; }

		public int TotalTaskCount { get; set; }

		public int QueuedTaskCount { get; set; }

		public string SyncToAsyncRatio { get; set; }

		public bool SynchronousExecutionAllowed { get; set; }

		public int DelayedTaskCount { get; set; }

		public int TaskSubmissionFailuresPerMinute { get; set; }

		public int TasksCompletedPerMinute { get; set; }

		public int TaskTimeoutsPerMinute { get; set; }

		public List<WLMTaskDetails> QueuedTasks { get; set; }

		public List<WLMTaskDetails> DelayedTasks { get; set; }

		public List<WLMTaskDetails> ExecutingTasks { get; set; }
	}
}
