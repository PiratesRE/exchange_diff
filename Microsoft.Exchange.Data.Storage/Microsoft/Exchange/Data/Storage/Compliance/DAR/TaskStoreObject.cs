using System;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Office.CompliancePolicy.Dar;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Exchange.Data.Storage.Compliance.DAR
{
	[Serializable]
	public class TaskStoreObject : EwsStoreObject, IStoreObject
	{
		public TaskStoreObject()
		{
			this.propertyBag.SetField(EwsStoreObjectSchema.Identity, new EwsStoreObjectId(Guid.NewGuid().ToString()));
		}

		public string Id
		{
			get
			{
				return (string)this[TaskStoreObjectSchema.Id];
			}
			set
			{
				this[TaskStoreObjectSchema.Id] = value;
			}
		}

		public DarTaskCategory Category
		{
			get
			{
				return (DarTaskCategory)((int)this[TaskStoreObjectSchema.Category]);
			}
			set
			{
				this[TaskStoreObjectSchema.Category] = (int)value;
			}
		}

		public int Priority
		{
			get
			{
				return (int)this[TaskStoreObjectSchema.Priority];
			}
			set
			{
				this[TaskStoreObjectSchema.Priority] = value;
			}
		}

		public DarTaskState TaskState
		{
			get
			{
				return (DarTaskState)((int)this[TaskStoreObjectSchema.TaskState]);
			}
			set
			{
				this[TaskStoreObjectSchema.TaskState] = (int)value;
			}
		}

		public string TaskType
		{
			get
			{
				return (string)this[TaskStoreObjectSchema.TaskType];
			}
			set
			{
				this[TaskStoreObjectSchema.TaskType] = value;
			}
		}

		public string SerializedTaskData
		{
			get
			{
				return (string)this[TaskStoreObjectSchema.SerializedTaskData];
			}
			set
			{
				this[TaskStoreObjectSchema.SerializedTaskData] = value;
			}
		}

		public byte[] TenantId
		{
			get
			{
				return (byte[])this[TaskStoreObjectSchema.TenantId];
			}
			set
			{
				this[TaskStoreObjectSchema.TenantId] = value;
			}
		}

		public DateTime MinTaskScheduleTime
		{
			get
			{
				return (DateTime)this[TaskStoreObjectSchema.MinTaskScheduleTime];
			}
			set
			{
				this[TaskStoreObjectSchema.MinTaskScheduleTime] = value;
			}
		}

		public DateTime TaskCompletionTime
		{
			get
			{
				return (DateTime)this[TaskStoreObjectSchema.TaskCompletionTime];
			}
			set
			{
				this[TaskStoreObjectSchema.TaskCompletionTime] = value;
			}
		}

		public DateTime TaskExecutionStartTime
		{
			get
			{
				return (DateTime)this[TaskStoreObjectSchema.TaskExecutionStartTime];
			}
			set
			{
				this[TaskStoreObjectSchema.TaskExecutionStartTime] = value;
			}
		}

		public DateTime TaskQueuedTime
		{
			get
			{
				return (DateTime)this[TaskStoreObjectSchema.TaskQueuedTime];
			}
			set
			{
				this[TaskStoreObjectSchema.TaskQueuedTime] = value;
			}
		}

		public DateTime TaskScheduledTime
		{
			get
			{
				return (DateTime)this[TaskStoreObjectSchema.TaskScheduledTime];
			}
			set
			{
				this[TaskStoreObjectSchema.TaskScheduledTime] = value;
			}
		}

		public TimeSpan TaskRetryInterval
		{
			get
			{
				return TimeSpan.Parse((string)this[TaskStoreObjectSchema.TaskRetryInterval]);
			}
			set
			{
				this[TaskStoreObjectSchema.TaskRetryInterval] = value.ToString();
			}
		}

		public int TaskRetryCurrentCount
		{
			get
			{
				return (int)this[TaskStoreObjectSchema.TaskRetryCurrentCount];
			}
			set
			{
				this[TaskStoreObjectSchema.TaskRetryCurrentCount] = value;
			}
		}

		public int TaskRetryTotalCount
		{
			get
			{
				return (int)this[TaskStoreObjectSchema.TaskRetryTotalCount];
			}
			set
			{
				this[TaskStoreObjectSchema.TaskRetryTotalCount] = value;
			}
		}

		public TaskSynchronizationOption TaskSynchronizationOption
		{
			get
			{
				return (TaskSynchronizationOption)((int)this[TaskStoreObjectSchema.TaskSynchronizationOption]);
			}
			set
			{
				this[TaskStoreObjectSchema.TaskSynchronizationOption] = (int)value;
			}
		}

		public string TaskSynchronizationKey
		{
			get
			{
				return (string)this[TaskStoreObjectSchema.TaskSynchronizationKey];
			}
			set
			{
				this[TaskStoreObjectSchema.TaskSynchronizationKey] = value;
			}
		}

		public string ExecutionContainer
		{
			get
			{
				return (string)this[TaskStoreObjectSchema.ExecutionContainer];
			}
			set
			{
				this[TaskStoreObjectSchema.ExecutionContainer] = value;
			}
		}

		public string ExecutionTarget
		{
			get
			{
				return (string)this[TaskStoreObjectSchema.ExecutionTarget];
			}
			set
			{
				this[TaskStoreObjectSchema.ExecutionTarget] = value;
			}
		}

		public DateTime ExecutionLockExpiryTime
		{
			get
			{
				return (DateTime)this[TaskStoreObjectSchema.ExecutionLockExpiryTime];
			}
			set
			{
				this[TaskStoreObjectSchema.ExecutionLockExpiryTime] = value;
			}
		}

		public DateTime LastModifiedTime
		{
			get
			{
				return (DateTime)this[TaskAggregateStoreObjectSchema.LastModifiedTime];
			}
		}

		public int SchemaVersion
		{
			get
			{
				return (int)this[TaskStoreObjectSchema.SchemaVersion];
			}
			set
			{
				this[TaskStoreObjectSchema.SchemaVersion] = value;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return TaskStoreObject.schema;
			}
		}

		internal override string ItemClass
		{
			get
			{
				return "IPM.Configuration.DarTask";
			}
		}

		internal static TaskStoreObject FromDarTask(DarTask task, StoreObjectProvider objectProvider)
		{
			if (task == null)
			{
				throw new ArgumentNullException("task");
			}
			if (objectProvider == null)
			{
				throw new ArgumentNullException("objectProvider");
			}
			TaskStoreObject taskStoreObject = task.WorkloadContext as TaskStoreObject;
			if (taskStoreObject == null)
			{
				taskStoreObject = objectProvider.FindByAlternativeId<TaskStoreObject>(task.Id);
				if (taskStoreObject == null)
				{
					taskStoreObject = new TaskStoreObject();
				}
			}
			if (taskStoreObject != null)
			{
				taskStoreObject.UpdateFromDarTask(task);
			}
			return taskStoreObject;
		}

		internal static TaskStoreObject FromExistingObject(TaskStoreObject storeObject, StoreObjectProvider objectProvider)
		{
			TaskStoreObject taskStoreObject = objectProvider.FindByAlternativeId<TaskStoreObject>(storeObject.Id);
			taskStoreObject.Id = storeObject.Id;
			taskStoreObject.Category = storeObject.Category;
			taskStoreObject.Priority = storeObject.Priority;
			taskStoreObject.TaskSynchronizationKey = Helper.ToDefaultString(storeObject.TaskSynchronizationKey, null);
			taskStoreObject.TaskSynchronizationOption = storeObject.TaskSynchronizationOption;
			taskStoreObject.TaskType = Helper.ToDefaultString(storeObject.TaskType, null);
			taskStoreObject.TenantId = storeObject.TenantId;
			taskStoreObject.TaskState = storeObject.TaskState;
			taskStoreObject.TaskQueuedTime = storeObject.TaskQueuedTime;
			taskStoreObject.MinTaskScheduleTime = storeObject.MinTaskScheduleTime;
			taskStoreObject.TaskScheduledTime = storeObject.TaskScheduledTime;
			taskStoreObject.TaskExecutionStartTime = storeObject.TaskExecutionStartTime;
			taskStoreObject.TaskCompletionTime = storeObject.TaskCompletionTime;
			taskStoreObject.TaskRetryTotalCount = storeObject.TaskRetryTotalCount;
			taskStoreObject.TaskRetryInterval = storeObject.TaskRetryInterval;
			taskStoreObject.TaskRetryCurrentCount = storeObject.TaskRetryCurrentCount;
			taskStoreObject.SerializedTaskData = Helper.ToDefaultString(storeObject.SerializedTaskData, null);
			return taskStoreObject;
		}

		public DarTask ToDarTask(DarServiceProvider serviceProvider)
		{
			if (serviceProvider == null)
			{
				throw new ArgumentNullException("serviceProvider");
			}
			DarTask darTask = serviceProvider.DarTaskFactory.CreateTask(this.TaskType);
			this.UpdateDarTask(darTask);
			return darTask;
		}

		public void UpdateDarTask(DarTask task)
		{
			task.Id = this.Id;
			task.Category = this.Category;
			task.Priority = this.Priority;
			task.TaskSynchronizationKey = this.TaskSynchronizationKey;
			task.TaskSynchronizationOption = this.TaskSynchronizationOption;
			task.TenantId = Convert.ToBase64String(this.TenantId);
			task.TaskState = this.TaskState;
			task.TaskQueuedTime = this.TaskQueuedTime;
			task.MinTaskScheduleTime = this.MinTaskScheduleTime;
			task.TaskScheduledTime = this.TaskScheduledTime;
			task.TaskExecutionStartTime = this.TaskExecutionStartTime;
			task.TaskCompletionTime = this.TaskCompletionTime;
			task.TaskRetryTotalCount = this.TaskRetryTotalCount;
			task.TaskRetryInterval = this.TaskRetryInterval;
			task.TaskRetryCurrentCount = this.TaskRetryCurrentCount;
			task.SerializedTaskData = this.SerializedTaskData;
			task.WorkloadContext = this;
		}

		public void UpdateFromDarTask(DarTask task)
		{
			if (task == null)
			{
				throw new ArgumentNullException("task");
			}
			if (task.WorkloadContext != null && task.WorkloadContext != this)
			{
				throw new InvalidOperationException("Task is bound to another raw object use it to perform udpate");
			}
			this.Id = Helper.ToDefaultString(task.Id, null);
			this.Category = task.Category;
			this.Priority = task.Priority;
			this.TaskSynchronizationKey = Helper.ToDefaultString(task.TaskSynchronizationKey, null);
			this.TaskSynchronizationOption = task.TaskSynchronizationOption;
			this.TaskType = Helper.ToDefaultString(task.TaskType, null);
			this.TenantId = Convert.FromBase64String(task.TenantId);
			this.TaskState = task.TaskState;
			this.TaskQueuedTime = task.TaskQueuedTime;
			this.MinTaskScheduleTime = task.MinTaskScheduleTime;
			this.TaskScheduledTime = task.TaskScheduledTime;
			this.TaskExecutionStartTime = task.TaskExecutionStartTime;
			this.TaskCompletionTime = task.TaskCompletionTime;
			this.TaskRetryTotalCount = task.TaskRetryTotalCount;
			this.TaskRetryInterval = task.TaskRetryInterval;
			this.TaskRetryCurrentCount = task.TaskRetryCurrentCount;
			this.SerializedTaskData = Helper.ToDefaultString(task.SerializedTaskData, null);
			task.WorkloadContext = this;
		}

		public override string ToString()
		{
			return string.Format("Id:{0}, Tenant:{1}, Type:{2}, State:{3}, Retries:{4}", new object[]
			{
				this.Id,
				this.TenantId,
				this.TaskType,
				this.TaskState,
				this.TaskRetryCurrentCount
			});
		}

		public const string ObjectClass = "IPM.Configuration.DarTask";

		private static readonly TaskStoreObjectSchema schema = new TaskStoreObjectSchema();
	}
}
