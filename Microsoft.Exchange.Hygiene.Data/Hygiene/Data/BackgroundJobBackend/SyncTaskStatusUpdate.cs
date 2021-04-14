using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.BackgroundJobBackend
{
	internal sealed class SyncTaskStatusUpdate : BackgroundJobBackendBase
	{
		public SyncTaskStatusUpdate(Guid backgroundJobId, Guid taskId, Guid ownerId, TaskExecutionStateType taskExecutionState, TaskExecutionStateType newTaskExecutionState, TaskCompletionStatusType newTaskCompletionStatus)
		{
			this[SyncTaskStatusUpdate.BackgroundJobIdProperty] = backgroundJobId;
			this[SyncTaskStatusUpdate.TaskIdProperty] = taskId;
			this[SyncTaskStatusUpdate.BJMOwnerIdProperty] = ownerId;
			this[SyncTaskStatusUpdate.TaskExecutionStateProperty] = taskExecutionState;
			this[SyncTaskStatusUpdate.NewTaskExecutionStateProperty] = newTaskExecutionState;
			this[SyncTaskStatusUpdate.NewTaskCompletionStatusProperty] = newTaskCompletionStatus;
			this.UpdatedTaskStatus = false;
		}

		public Guid BackgroundJobId
		{
			get
			{
				return (Guid)this[SyncTaskStatusUpdate.BackgroundJobIdProperty];
			}
		}

		public Guid TaskId
		{
			get
			{
				return (Guid)this[SyncTaskStatusUpdate.TaskIdProperty];
			}
		}

		public TaskExecutionStateType TaskExecutionState
		{
			get
			{
				return (TaskExecutionStateType)this[SyncTaskStatusUpdate.TaskExecutionStateProperty];
			}
			set
			{
				this[SyncTaskStatusUpdate.TaskExecutionStateProperty] = (byte)value;
			}
		}

		public TaskExecutionStateType NewTaskExecutionState
		{
			get
			{
				return (TaskExecutionStateType)this[SyncTaskStatusUpdate.NewTaskExecutionStateProperty];
			}
			set
			{
				this[SyncTaskStatusUpdate.NewTaskExecutionStateProperty] = (byte)value;
			}
		}

		public TaskCompletionStatusType NewTaskCompletionStatus
		{
			get
			{
				return (TaskCompletionStatusType)this[SyncTaskStatusUpdate.NewTaskCompletionStatusProperty];
			}
			set
			{
				this[SyncTaskStatusUpdate.NewTaskCompletionStatusProperty] = (byte)value;
			}
		}

		public Guid BJMOwnerId
		{
			get
			{
				return (Guid)this[SyncTaskStatusUpdate.BJMOwnerIdProperty];
			}
		}

		public bool HasEndTime
		{
			get
			{
				return this[SyncTaskStatusUpdate.EndTimeProperty] != null;
			}
		}

		public DateTime EndTime
		{
			get
			{
				return (DateTime)this[SyncTaskStatusUpdate.EndTimeProperty];
			}
			set
			{
				this[SyncTaskStatusUpdate.EndTimeProperty] = value;
			}
		}

		public bool HasHeartBeat
		{
			get
			{
				return this[SyncTaskStatusUpdate.HeartBeatProperty] != null;
			}
		}

		public DateTime HeartBeat
		{
			get
			{
				return (DateTime)this[SyncTaskStatusUpdate.HeartBeatProperty];
			}
			set
			{
				this[SyncTaskStatusUpdate.HeartBeatProperty] = value;
			}
		}

		public bool UpdatedTaskStatus
		{
			get
			{
				return (bool)this[SyncTaskStatusUpdate.UpdatedTaskStatusProperty];
			}
			set
			{
				this[SyncTaskStatusUpdate.UpdatedTaskStatusProperty] = value;
			}
		}

		internal static readonly BackgroundJobBackendPropertyDefinition BackgroundJobIdProperty = ScheduleItemProperties.BackgroundJobIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition TaskIdProperty = TaskItemProperties.TaskIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition TaskExecutionStateProperty = TaskItemProperties.TaskExecutionStateProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition BJMOwnerIdProperty = TaskItemProperties.BJMOwnerIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition HeartBeatProperty = TaskItemProperties.HeartBeatProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition EndTimeProperty = TaskItemProperties.EndTimeProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition NewTaskExecutionStateProperty = new BackgroundJobBackendPropertyDefinition("NewTaskExecutionState", typeof(byte), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, 0);

		internal static readonly BackgroundJobBackendPropertyDefinition NewTaskCompletionStatusProperty = new BackgroundJobBackendPropertyDefinition("NewTaskCompletionStatus", typeof(byte), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, 0);

		internal static readonly BackgroundJobBackendPropertyDefinition UpdatedTaskStatusProperty = new BackgroundJobBackendPropertyDefinition("UpdatedTaskStatus", typeof(bool), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, false);
	}
}
