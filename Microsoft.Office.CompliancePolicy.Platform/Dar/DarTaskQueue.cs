using System;
using System.Collections.Generic;

namespace Microsoft.Office.CompliancePolicy.Dar
{
	public abstract class DarTaskQueue
	{
		public abstract IEnumerable<DarTask> Dequeue(int count, DarTaskCategory category, object availableResource = null);

		public abstract void Enqueue(DarTask darTask);

		public abstract IEnumerable<DarTask> GetRunningTasks(DateTime minExecutionStartTime, DateTime maxExecutionStartTime, string taskType = null, string tenantId = null);

		public abstract void UpdateTask(DarTask task);

		public abstract void DeleteTask(DarTask task);

		public abstract void DeleteCompletedTask(DateTime maxCompletionTime, string taskType = null, string tenantId = null);

		public abstract IEnumerable<DarTask> GetCompletedTasks(DateTime minCompletionTime, string taskType = null, string tenantId = null);

		public abstract IEnumerable<DarTask> GetTasks(string tenantId, string taskType = null, DarTaskState? taskState = null, DateTime? minScheduledTime = null, DateTime? maxScheduledTime = null, DateTime? minCompletedTime = null, DateTime? maxCompletedTime = null);

		public abstract IEnumerable<DarTask> GetLastScheduledTasks(string tenantId);
	}
}
