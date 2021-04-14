using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Storage.Compliance.DAR;
using Microsoft.Exchange.Management.CompliancePolicy.DarTasks.Protocol;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Office.CompliancePolicy.Dar;

namespace Microsoft.Office.CompliancePolicy.Exchange.Dar.Utility
{
	internal static class TaskHelper
	{
		public static HashSet<DarTaskState> CompletedTaskStates
		{
			get
			{
				return TaskHelper.completedTaskStates;
			}
		}

		public static bool IsActiveTask(DarTask task)
		{
			return task != null && !TaskHelper.CompletedTaskStates.Contains(task.TaskState);
		}

		public static SearchFilter GetCompletedTaskFilter(string taskType = null, DateTime? minCompletionDate = null, DateTime? maxCompletionDate = null, string taskId = null)
		{
			List<SearchFilter> list = new List<SearchFilter>();
			list.Add(new SearchFilter.SearchFilterCollection(1, (from t in TaskHelper.CompletedTaskStates
			select new SearchFilter.IsEqualTo(TaskStoreObjectExtendedStoreSchema.TaskState, (int)t)).ToArray<SearchFilter.IsEqualTo>()));
			if (maxCompletionDate != null)
			{
				list.Add(new SearchFilter.IsLessThanOrEqualTo(TaskStoreObjectExtendedStoreSchema.TaskCompletionTime, maxCompletionDate.Value));
			}
			if (minCompletionDate != null)
			{
				list.Add(new SearchFilter.IsGreaterThanOrEqualTo(TaskStoreObjectExtendedStoreSchema.TaskCompletionTime, minCompletionDate.Value));
			}
			if (!string.IsNullOrEmpty(taskType))
			{
				list.Add(new SearchFilter.IsEqualTo(TaskStoreObjectExtendedStoreSchema.TaskType, taskType));
			}
			if (!string.IsNullOrEmpty(taskId))
			{
				list.Add(new SearchFilter.IsEqualTo(TaskAggregateStoreObjectSchema.Id.StorePropertyDefinition, taskId));
			}
			return new SearchFilter.SearchFilterCollection(0, list.ToArray());
		}

		public static SearchFilter GetTaskFilter(string taskType = null, DarTaskState? taskState = null, DateTime? minScheduledTime = null, DateTime? maxScheduledTime = null, DateTime? minCompletedTime = null, DateTime? maxCompletedTime = null)
		{
			DarTaskParams darTaskParams = new DarTaskParams();
			if (taskType != null)
			{
				darTaskParams.TaskType = taskType;
			}
			if (taskState != null)
			{
				darTaskParams.TaskState = taskState.Value;
			}
			if (minScheduledTime != null)
			{
				darTaskParams.MinQueuedTime = minScheduledTime.Value;
			}
			if (maxScheduledTime != null)
			{
				darTaskParams.MaxQueuedTime = maxScheduledTime.Value;
			}
			if (minCompletedTime != null)
			{
				darTaskParams.MinCompletionTime = minCompletedTime.Value;
			}
			if (maxCompletedTime != null)
			{
				darTaskParams.MaxCompletionTime = maxCompletedTime.Value;
			}
			return TaskHelper.GetTaskFilter(darTaskParams);
		}

		public static SearchFilter GetTaskFilter(DarTaskParams parameters)
		{
			List<SearchFilter> list = new List<SearchFilter>();
			if (parameters.TaskType != null)
			{
				list.Add(new SearchFilter.IsEqualTo(TaskAggregateStoreObjectSchema.Id.StorePropertyDefinition, parameters.TaskType));
			}
			if (parameters.TaskId != null)
			{
				list.Add(new SearchFilter.IsEqualTo(TaskAggregateStoreObjectSchema.Id.StorePropertyDefinition, parameters.TaskId));
			}
			if (parameters.MinCompletionTime != default(DateTime))
			{
				list.Add(new SearchFilter.IsGreaterThanOrEqualTo(TaskStoreObjectSchema.TaskCompletionTime.StorePropertyDefinition, parameters.MinCompletionTime));
			}
			if (parameters.MaxCompletionTime != default(DateTime))
			{
				list.Add(new SearchFilter.IsLessThanOrEqualTo(TaskStoreObjectSchema.TaskCompletionTime.StorePropertyDefinition, parameters.MaxCompletionTime));
			}
			if (parameters.MinQueuedTime != default(DateTime))
			{
				list.Add(new SearchFilter.IsGreaterThanOrEqualTo(TaskStoreObjectSchema.TaskQueuedTime.StorePropertyDefinition, parameters.MinQueuedTime));
			}
			if (parameters.MaxQueuedTime != default(DateTime))
			{
				list.Add(new SearchFilter.IsLessThanOrEqualTo(TaskStoreObjectSchema.TaskQueuedTime.StorePropertyDefinition, parameters.MaxQueuedTime));
			}
			if (parameters.TaskState != DarTaskState.None)
			{
				list.Add(new SearchFilter.IsEqualTo(TaskStoreObjectSchema.TaskState.StorePropertyDefinition, (int)parameters.TaskState));
			}
			if (!string.IsNullOrEmpty(parameters.TaskType))
			{
				list.Add(new SearchFilter.IsEqualTo(TaskStoreObjectSchema.TaskType.StorePropertyDefinition, parameters.TaskType));
			}
			return TaskHelper.GetSearchFilterFromCollection(list);
		}

		public static SearchFilter GetTaskAggregateFilter(DarTaskAggregateParams parameters)
		{
			List<SearchFilter> list = new List<SearchFilter>();
			if (parameters.TaskId != null)
			{
				list.Add(new SearchFilter.IsEqualTo(TaskAggregateStoreObjectSchema.Id.StorePropertyDefinition, parameters.TaskId));
			}
			if (!string.IsNullOrEmpty(parameters.TaskType))
			{
				list.Add(new SearchFilter.IsEqualTo(TaskAggregateStoreObjectSchema.TaskType.StorePropertyDefinition, parameters.TaskType));
			}
			return TaskHelper.GetSearchFilterFromCollection(list);
		}

		public static SearchFilter GetActiveTaskFilter()
		{
			return new SearchFilter.Not(new SearchFilter.SearchFilterCollection(1, (from t in TaskHelper.CompletedTaskStates
			select new SearchFilter.IsEqualTo(TaskStoreObjectExtendedStoreSchema.TaskState, (int)t)).ToArray<SearchFilter.IsEqualTo>()));
		}

		public static void Validate(DarTask task, DarServiceProvider serviceProvider)
		{
			if (string.IsNullOrEmpty(task.Id))
			{
				throw new ArgumentException("task.Id");
			}
			if (task.TenantId == null)
			{
				throw new ArgumentException("task.TenantId");
			}
			if (string.IsNullOrEmpty(task.TaskType))
			{
				throw new ArgumentException("task.TaskType");
			}
			if (!serviceProvider.DarTaskFactory.GetAllTaskTypes().Any((string t) => t.Equals(task.TaskType, StringComparison.InvariantCultureIgnoreCase)))
			{
				throw new ArgumentException("task.TaskType");
			}
		}

		public static bool IsValid(DarTask task)
		{
			return !string.IsNullOrEmpty(task.Id) && !string.IsNullOrEmpty(task.TaskType) && task.TenantId != null;
		}

		public static void Validate(DarTaskAggregate taskAggregate, DarServiceProvider serviceProvider)
		{
			if (string.IsNullOrEmpty(taskAggregate.Id))
			{
				throw new ArgumentException("taskAggregate.Id");
			}
			if (taskAggregate.ScopeId == null)
			{
				throw new ArgumentException("taskAggregate.ScopeId");
			}
			if (string.IsNullOrEmpty(taskAggregate.TaskType))
			{
				throw new ArgumentException("taskAggregate.TaskType");
			}
		}

		private static SearchFilter GetSearchFilterFromCollection(List<SearchFilter> filters)
		{
			if (filters.Count == 0)
			{
				return null;
			}
			if (filters.Count == 1)
			{
				return filters[0];
			}
			return new SearchFilter.SearchFilterCollection(0, filters);
		}

		private static HashSet<DarTaskState> completedTaskStates = new HashSet<DarTaskState>
		{
			DarTaskState.Completed,
			DarTaskState.Failed,
			DarTaskState.Cancelled
		};
	}
}
