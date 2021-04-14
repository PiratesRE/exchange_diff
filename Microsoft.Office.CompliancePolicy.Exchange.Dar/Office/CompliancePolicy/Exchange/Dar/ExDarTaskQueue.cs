using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Storage.Compliance.DAR;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Office.CompliancePolicy.Dar;
using Microsoft.Office.CompliancePolicy.Exchange.Dar.Diagnostics;
using Microsoft.Office.CompliancePolicy.Exchange.Dar.Execution;
using Microsoft.Office.CompliancePolicy.Exchange.Dar.Utility;

namespace Microsoft.Office.CompliancePolicy.Exchange.Dar
{
	internal class ExDarTaskQueue : DarTaskQueue
	{
		public ExDarTaskQueue(DarServiceProvider provider)
		{
			this.provider = provider;
		}

		public override void DeleteCompletedTask(DateTime maxCompletionTime, string taskType = null, string tenantId = null)
		{
			if (tenantId == null)
			{
				throw new ArgumentNullException("tenantId must be specified for exchange workload");
			}
			SearchFilter completedTaskFilter = TaskHelper.GetCompletedTaskFilter(taskType, null, new DateTime?(maxCompletionTime), null);
			TenantStore.DeleteTasks(tenantId, completedTaskFilter, OperationContext.CorrelationId);
		}

		public override void DeleteTask(DarTask task)
		{
			if (task == null)
			{
				throw new ArgumentNullException("Task must be specified to delete");
			}
			SearchFilter completedTaskFilter = TaskHelper.GetCompletedTaskFilter(null, null, null, task.Id);
			TenantStore.DeleteTasks(task.TenantId, completedTaskFilter, OperationContext.CorrelationId);
		}

		public override IEnumerable<DarTask> GetCompletedTasks(DateTime minCompletionTime, string taskType = null, string tenantId = null)
		{
			if (tenantId == null)
			{
				throw new ArgumentNullException("tenantId must be specified for exchange workload");
			}
			SearchFilter completedTaskFilter = TaskHelper.GetCompletedTaskFilter(taskType, new DateTime?(minCompletionTime), null, null);
			return from task in TenantStore.Find<TaskStoreObject>(tenantId, completedTaskFilter, false, OperationContext.CorrelationId)
			select task.ToDarTask(this.provider);
		}

		public override IEnumerable<DarTask> GetTasks(string tenantId, string taskType = null, DarTaskState? taskState = null, DateTime? minScheduledTime = null, DateTime? maxScheduledTime = null, DateTime? minCompletedTime = null, DateTime? maxCompletedTime = null)
		{
			if (tenantId == null)
			{
				throw new ArgumentNullException("tenantId must be specified for exchange workload");
			}
			SearchFilter taskFilter = TaskHelper.GetTaskFilter(taskType, taskState, minScheduledTime, maxScheduledTime, minCompletedTime, maxCompletedTime);
			return from task in TenantStore.Find<TaskStoreObject>(tenantId, taskFilter, false, OperationContext.CorrelationId)
			select task.ToDarTask(this.provider);
		}

		public override IEnumerable<DarTask> Dequeue(int count, DarTaskCategory category, object availableResource = null)
		{
			return (from t in (from t in InstanceManager.Current.GetReadyTaskList()
			orderby t.Priority
			select t).ThenBy((DarTask darTask) => darTask.TaskLastExecutionTime)
			where t.Category == category
			select t).Take(count);
		}

		public override void Enqueue(DarTask darTask)
		{
			TaskHelper.Validate(darTask, this.provider);
			InstanceManager.Current.TaskAggregates.ValidateEnqueue(darTask, darTask.CorrelationId);
			darTask.TaskState = DarTaskState.Ready;
			TenantStore.SaveTask(darTask, darTask.CorrelationId);
		}

		public override IEnumerable<DarTask> GetRunningTasks(DateTime minExecutionStartTime, DateTime maxExecutionStartTime, string taskType = null, string tenantId = null)
		{
			return from t in InstanceManager.Current.GetActiveTaskList(tenantId)
			where t.TaskExecutionStartTime > minExecutionStartTime && t.TaskExecutionStartTime < maxExecutionStartTime && t.TaskState == DarTaskState.Running && (string.IsNullOrEmpty(taskType) || taskType.Equals(taskType, StringComparison.InvariantCultureIgnoreCase))
			select t;
		}

		public override void UpdateTask(DarTask darTask)
		{
			TenantStore.SaveTask(darTask, darTask.CorrelationId);
		}

		public override IEnumerable<DarTask> GetLastScheduledTasks(string tenantId)
		{
			throw new NotImplementedException();
		}

		private DarServiceProvider provider;
	}
}
