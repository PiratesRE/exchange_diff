using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.BackgroundJobBackend
{
	internal class SaveTaskItemBatch : BackgroundJobBackendBase, IEnumerable<TaskItem>, IEnumerable
	{
		public SaveTaskItemBatch()
		{
			this.batch = new List<TaskItem>(5);
			this.InsertedTasks = false;
			this[SaveTaskItemBatch.NextActiveJobIdProperty] = null;
		}

		public SaveTaskItemBatch(int initialCapacity)
		{
			if (initialCapacity < 0)
			{
				throw new ArgumentOutOfRangeException("initialCapacity < 0");
			}
			this.batch = new List<TaskItem>(initialCapacity);
			this.InsertedTasks = false;
			this[SaveTaskItemBatch.NextActiveJobIdProperty] = null;
		}

		public SaveTaskItemBatch(List<TaskItem> taskItems, Guid backgroundJobId, DateTime lastScheduledTime, Guid? nextActiveJobId = null)
		{
			if (taskItems != null)
			{
				this.batch = taskItems;
			}
			else
			{
				this.batch = new List<TaskItem>();
			}
			this.BackgroundJobId = backgroundJobId;
			this.LastScheduledTime = lastScheduledTime;
			this.InsertedTasks = false;
			if (nextActiveJobId != null)
			{
				this.NextActiveJobId = nextActiveJobId.Value;
				return;
			}
			this[SaveTaskItemBatch.NextActiveJobIdProperty] = null;
		}

		public int TaskCount
		{
			get
			{
				return this.batch.Count;
			}
		}

		public Guid BackgroundJobId
		{
			get
			{
				return (Guid)this[SaveTaskItemBatch.BackgroundJobIdProperty];
			}
			set
			{
				this[SaveTaskItemBatch.BackgroundJobIdProperty] = value;
			}
		}

		public Guid ActiveJobId
		{
			get
			{
				if (this.batch.Count == 0)
				{
					throw new ArgumentException("Unable to determine active job id because the batch of task items is empty.");
				}
				return this.batch[0].ActiveJobId;
			}
		}

		public Guid ScheduleId
		{
			get
			{
				if (this.batch.Count == 0)
				{
					throw new ArgumentException("Unable to determine schedule id because the batch of task items is empty.");
				}
				return this.batch[0].ScheduleId;
			}
		}

		public DateTime LastScheduledTime
		{
			get
			{
				return (DateTime)this[SaveTaskItemBatch.LastScheduledTimeProperty];
			}
			set
			{
				this[SaveTaskItemBatch.LastScheduledTimeProperty] = value;
			}
		}

		public bool HasNextActiveJobId
		{
			get
			{
				return this[SaveTaskItemBatch.NextActiveJobIdProperty] != null;
			}
		}

		public Guid NextActiveJobId
		{
			get
			{
				return (Guid)this[SaveTaskItemBatch.NextActiveJobIdProperty];
			}
			set
			{
				this[SaveTaskItemBatch.NextActiveJobIdProperty] = value;
			}
		}

		public bool InsertedTasks
		{
			get
			{
				return (bool)this[SaveTaskItemBatch.InsertedTasksProperty];
			}
			set
			{
				this[SaveTaskItemBatch.InsertedTasksProperty] = value;
			}
		}

		public void Clear()
		{
			this.batch.Clear();
			this.InsertedTasks = false;
			this.ClearNextActiveJobId();
		}

		public void ClearNextActiveJobId()
		{
			this[SaveTaskItemBatch.NextActiveJobIdProperty] = null;
		}

		public IEnumerator<TaskItem> GetEnumerator()
		{
			return this.batch.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void Add(TaskItem taskItem)
		{
			if (this.batch.Count > 0)
			{
				if (this.batch[0].ActiveJobId != taskItem.ActiveJobId)
				{
					throw new ArgumentException("Unable to add a task item to the batch because it is for a active job id.", "taskItem");
				}
				if (this.batch[0].ScheduleId != taskItem.ScheduleId)
				{
					throw new ArgumentException("Unable to add a task item to the batch because it is for a diffrent schedule id.", "taskItem");
				}
			}
			else
			{
				if (taskItem.ActiveJobId.Equals(Guid.Empty))
				{
					throw new ArgumentException("Unable to add task item to the batch because the ActiveJobId is not specified.", "taskItem");
				}
				if (taskItem.ScheduleId.Equals(Guid.Empty))
				{
					throw new ArgumentException("Unable to add task item to the batch because the ScheduleId is not specified.", "taskItem");
				}
			}
			this.batch.Add(taskItem);
		}

		internal static readonly BackgroundJobBackendPropertyDefinition BackgroundJobIdProperty = JobDefinition.BackgroundJobIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition NextActiveJobIdProperty = ScheduleItemProperties.NextActiveJobIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition LastScheduledTimeProperty = ScheduleItemProperties.LastScheduledTimeProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition InsertedTasksProperty = new BackgroundJobBackendPropertyDefinition("InsertedTasks", typeof(bool), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, false);

		private List<TaskItem> batch;
	}
}
