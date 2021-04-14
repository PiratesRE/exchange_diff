using System;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Office.CompliancePolicy.Dar;

namespace Microsoft.Exchange.Data.Storage.Compliance.DAR
{
	[Serializable]
	public class TaskAggregateStoreObject : EwsStoreObject, IStoreObject
	{
		public TaskAggregateStoreObject()
		{
			this.propertyBag.SetField(EwsStoreObjectSchema.Identity, new EwsStoreObjectId(Guid.NewGuid().ToString()));
		}

		public string Id
		{
			get
			{
				return (string)this[TaskAggregateStoreObjectSchema.Id];
			}
			set
			{
				this[TaskAggregateStoreObjectSchema.Id] = value;
			}
		}

		public byte[] ScopeId
		{
			get
			{
				return (byte[])this[TaskAggregateStoreObjectSchema.ScopeId];
			}
			set
			{
				this[TaskAggregateStoreObjectSchema.ScopeId] = value;
			}
		}

		public string TaskType
		{
			get
			{
				return (string)this[TaskAggregateStoreObjectSchema.TaskType];
			}
			set
			{
				this[TaskAggregateStoreObjectSchema.TaskType] = value;
			}
		}

		public bool Enabled
		{
			get
			{
				return (bool)this[TaskAggregateStoreObjectSchema.Enabled];
			}
			set
			{
				this[TaskAggregateStoreObjectSchema.Enabled] = value;
			}
		}

		public int MaxRunningTasks
		{
			get
			{
				return (int)this[TaskAggregateStoreObjectSchema.MaxRunningTasks];
			}
			set
			{
				this[TaskAggregateStoreObjectSchema.MaxRunningTasks] = value;
			}
		}

		public RecurrenceType RecurrenceType
		{
			get
			{
				return (RecurrenceType)((int)this[TaskAggregateStoreObjectSchema.RecurrenceType]);
			}
			set
			{
				this[TaskAggregateStoreObjectSchema.RecurrenceType] = (int)value;
			}
		}

		public RecurrenceFrequency RecurrenceFrequency
		{
			get
			{
				return (RecurrenceFrequency)((int)this[TaskAggregateStoreObjectSchema.RecurrenceFrequency]);
			}
			set
			{
				this[TaskAggregateStoreObjectSchema.RecurrenceFrequency] = (int)value;
			}
		}

		public int RecurrenceInterval
		{
			get
			{
				return (int)this[TaskAggregateStoreObjectSchema.RecurrenceInterval];
			}
			set
			{
				this[TaskAggregateStoreObjectSchema.RecurrenceInterval] = value;
			}
		}

		public int SchemaVersion
		{
			get
			{
				return (int)this[TaskAggregateStoreObjectSchema.SchemaVersion];
			}
			set
			{
				this[TaskAggregateStoreObjectSchema.SchemaVersion] = value;
			}
		}

		public DateTime LastModifiedTime
		{
			get
			{
				return (DateTime)this[TaskAggregateStoreObjectSchema.LastModifiedTime];
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
				return TaskAggregateStoreObject.schema;
			}
		}

		internal override string ItemClass
		{
			get
			{
				return "IPM.Configuration.DarTaskAggregate";
			}
		}

		internal static TaskAggregateStoreObject FromDarTaskAggregate(DarTaskAggregate taskAggregate, StoreObjectProvider objectProvider)
		{
			if (taskAggregate == null)
			{
				throw new ArgumentNullException("taskAggregate");
			}
			if (objectProvider == null)
			{
				throw new ArgumentNullException("objectProvider");
			}
			TaskAggregateStoreObject taskAggregateStoreObject = taskAggregate.WorkloadContext as TaskAggregateStoreObject;
			if (taskAggregateStoreObject == null)
			{
				taskAggregateStoreObject = objectProvider.FindByAlternativeId<TaskAggregateStoreObject>(taskAggregate.Id);
				if (taskAggregateStoreObject == null)
				{
					taskAggregateStoreObject = new TaskAggregateStoreObject();
				}
			}
			if (taskAggregateStoreObject != null)
			{
				taskAggregateStoreObject.UpdateFromDarTaskAggregate(taskAggregate);
			}
			return taskAggregateStoreObject;
		}

		internal static TaskAggregateStoreObject FromExistingObject(TaskAggregateStoreObject storeObject, StoreObjectProvider objectProvider)
		{
			TaskAggregateStoreObject taskAggregateStoreObject = objectProvider.FindByAlternativeId<TaskAggregateStoreObject>(storeObject.Id);
			taskAggregateStoreObject.Id = storeObject.Id;
			taskAggregateStoreObject.Enabled = storeObject.Enabled;
			taskAggregateStoreObject.TaskType = storeObject.TaskType;
			taskAggregateStoreObject.ScopeId = storeObject.ScopeId;
			taskAggregateStoreObject.MaxRunningTasks = storeObject.MaxRunningTasks;
			taskAggregateStoreObject.RecurrenceType = storeObject.RecurrenceType;
			taskAggregateStoreObject.RecurrenceFrequency = storeObject.RecurrenceFrequency;
			taskAggregateStoreObject.RecurrenceInterval = storeObject.RecurrenceInterval;
			return taskAggregateStoreObject;
		}

		public DarTaskAggregate ToDarTaskAggregate(DarServiceProvider serviceProvider)
		{
			if (serviceProvider == null)
			{
				throw new ArgumentNullException("serviceProvider");
			}
			DarTaskAggregate darTaskAggregate = serviceProvider.DarTaskFactory.CreateTaskAggregate(this.TaskType);
			this.UpdateDarTaskAggregate(darTaskAggregate);
			return darTaskAggregate;
		}

		public void UpdateDarTaskAggregate(DarTaskAggregate taskAggregate)
		{
			taskAggregate.Id = this.Id;
			taskAggregate.Enabled = this.Enabled;
			taskAggregate.TaskType = this.TaskType;
			taskAggregate.ScopeId = Convert.ToBase64String(this.ScopeId);
			taskAggregate.MaxRunningTasks = this.MaxRunningTasks;
			taskAggregate.RecurrenceType = this.RecurrenceType;
			taskAggregate.RecurrenceFrequency = this.RecurrenceFrequency;
			taskAggregate.RecurrenceInterval = this.RecurrenceInterval;
			taskAggregate.WorkloadContext = this;
		}

		public void UpdateFromDarTaskAggregate(DarTaskAggregate taskAggregate)
		{
			if (taskAggregate == null)
			{
				throw new ArgumentNullException("task");
			}
			if (taskAggregate.WorkloadContext != null && taskAggregate.WorkloadContext != this)
			{
				throw new InvalidOperationException("Task is bound to another raw object use it to perform udpate");
			}
			this.Id = Helper.ToDefaultString(taskAggregate.Id, null);
			this.Enabled = taskAggregate.Enabled;
			this.TaskType = Helper.ToDefaultString(taskAggregate.TaskType, null);
			this.ScopeId = Convert.FromBase64String(taskAggregate.ScopeId);
			this.MaxRunningTasks = taskAggregate.MaxRunningTasks;
			this.RecurrenceType = taskAggregate.RecurrenceType;
			this.RecurrenceFrequency = taskAggregate.RecurrenceFrequency;
			this.RecurrenceInterval = taskAggregate.RecurrenceInterval;
			taskAggregate.WorkloadContext = this;
		}

		public override string ToString()
		{
			return string.Format("Id:{0}, Tenant:{1}, Type:{2}, Max:{3}", new object[]
			{
				this.Id,
				this.ScopeId,
				this.TaskType,
				this.MaxRunningTasks
			});
		}

		public const string ObjectClass = "IPM.Configuration.DarTaskAggregate";

		private static readonly TaskAggregateStoreObjectSchema schema = new TaskAggregateStoreObjectSchema();
	}
}
