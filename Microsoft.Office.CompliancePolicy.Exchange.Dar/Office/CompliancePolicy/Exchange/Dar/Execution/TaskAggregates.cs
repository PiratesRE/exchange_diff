using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Storage.Compliance.DAR;
using Microsoft.Exchange.Management.CompliancePolicy.DarTasks.Protocol;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Office.CompliancePolicy.Dar;
using Microsoft.Office.CompliancePolicy.Exchange.Dar.LocStrings;
using Microsoft.Office.CompliancePolicy.Exchange.Dar.Utility;

namespace Microsoft.Office.CompliancePolicy.Exchange.Dar.Execution
{
	public class TaskAggregates
	{
		public DarTaskAggregate Get(string tenantId, string type, string correlationId)
		{
			return this.taskAggregateList.GetOrAdd(new TaskAggregates.TaskAggregateKey
			{
				TenantId = tenantId,
				TaskType = type
			}, delegate(TaskAggregates.TaskAggregateKey key)
			{
				SearchFilter taskAggregateFilter = TaskHelper.GetTaskAggregateFilter(new DarTaskAggregateParams
				{
					TaskType = type
				});
				DarTaskAggregate darTaskAggregate = (from ta in TenantStore.Find<TaskAggregateStoreObject>(key.TenantId, taskAggregateFilter, false, correlationId)
				select ta.ToDarTaskAggregate(InstanceManager.Current.Provider)).FirstOrDefault<DarTaskAggregate>();
				if (darTaskAggregate == null)
				{
					darTaskAggregate = TaskAggregates.CreateDefaultTaskAggregate(tenantId, type);
					TenantStore.SaveTaskAggregate(darTaskAggregate, correlationId);
				}
				return darTaskAggregate;
			});
		}

		public IEnumerable<DarTaskAggregate> GetAll(string tenantId, string correlationId)
		{
			return from kvp in this.taskAggregateList
			where kvp.Key.TenantId == tenantId
			select kvp.Value;
		}

		public void Set(DarTaskAggregate taskAggregate, string correlationId)
		{
			TenantStore.SaveTaskAggregate(taskAggregate, correlationId);
			TaskAggregates.TaskAggregateKey key = new TaskAggregates.TaskAggregateKey
			{
				TenantId = taskAggregate.ScopeId,
				TaskType = taskAggregate.TaskType
			};
			this.taskAggregateList[key] = taskAggregate;
		}

		public bool Remove(string tenantId, string type, string correlationId)
		{
			TaskAggregates.TaskAggregateKey key = new TaskAggregates.TaskAggregateKey
			{
				TenantId = tenantId,
				TaskType = type
			};
			SearchFilter searchFilter = new SearchFilter.IsEqualTo(TaskAggregateStoreObjectSchema.TaskType.StorePropertyDefinition, type);
			TenantStore.DeleteTaskAggregates(tenantId, searchFilter, correlationId);
			DarTaskAggregate darTaskAggregate;
			return this.taskAggregateList.TryRemove(key, out darTaskAggregate);
		}

		public void ValidateEnqueue(DarTask darTask, string correlationId)
		{
			if (!this.Get(darTask.TenantId, darTask.TaskType, correlationId).Enabled)
			{
				throw new ApplicationException(Strings.TaskIsDisabled);
			}
		}

		private static DarTaskAggregate CreateDefaultTaskAggregate(string tenantId, string type)
		{
			DarTaskAggregate darTaskAggregate = InstanceManager.Current.Provider.DarTaskFactory.CreateTaskAggregate(type);
			darTaskAggregate.ScopeId = tenantId;
			darTaskAggregate.Enabled = true;
			return darTaskAggregate;
		}

		private readonly ConcurrentDictionary<TaskAggregates.TaskAggregateKey, DarTaskAggregate> taskAggregateList = new ConcurrentDictionary<TaskAggregates.TaskAggregateKey, DarTaskAggregate>();

		private struct TaskAggregateKey
		{
			public string TenantId;

			public string TaskType;
		}
	}
}
