using System;

namespace Microsoft.Exchange.Hygiene.Data.BackgroundJobBackend
{
	internal sealed class TaskStatusUpdate : BackgroundJobBackendBase
	{
		public TaskStatusUpdate(Guid backgroundJobId, Guid taskId, TaskExecutionStateType taskExecutionState, TaskCompletionStatusType taskCompletionStatus)
		{
			this[TaskStatusUpdate.BackgroundJobIdProperty] = backgroundJobId;
			this[TaskStatusUpdate.TaskIdProperty] = taskId;
			this[TaskStatusUpdate.TaskExecutionStateProperty] = taskExecutionState;
			this[TaskStatusUpdate.TaskCompletionStatusProperty] = taskCompletionStatus;
		}

		public Guid BackgroundJobId
		{
			get
			{
				return (Guid)this[TaskStatusUpdate.BackgroundJobIdProperty];
			}
		}

		public Guid TaskId
		{
			get
			{
				return (Guid)this[TaskStatusUpdate.TaskIdProperty];
			}
		}

		public TaskExecutionStateType TaskExecutionState
		{
			get
			{
				return (TaskExecutionStateType)this[TaskStatusUpdate.TaskExecutionStateProperty];
			}
		}

		public TaskCompletionStatusType TaskCompletionStatus
		{
			get
			{
				return (TaskCompletionStatusType)this[TaskStatusUpdate.TaskCompletionStatusProperty];
			}
		}

		public bool HasEndTime
		{
			get
			{
				return this[TaskStatusUpdate.EndTimeProperty] != null;
			}
		}

		public DateTime EndTime
		{
			get
			{
				return (DateTime)this[TaskStatusUpdate.EndTimeProperty];
			}
			set
			{
				this[TaskStatusUpdate.EndTimeProperty] = value;
			}
		}

		public bool HasHeartBeat
		{
			get
			{
				return this[TaskStatusUpdate.HeartBeatProperty] != null;
			}
		}

		public DateTime HeartBeat
		{
			get
			{
				return (DateTime)this[TaskStatusUpdate.HeartBeatProperty];
			}
			set
			{
				this[TaskStatusUpdate.HeartBeatProperty] = value;
			}
		}

		internal static readonly BackgroundJobBackendPropertyDefinition BackgroundJobIdProperty = ScheduleItemProperties.BackgroundJobIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition TaskIdProperty = TaskItemProperties.TaskIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition TaskExecutionStateProperty = TaskItemProperties.TaskExecutionStateProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition TaskCompletionStatusProperty = TaskItemProperties.TaskCompletionStatusProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition HeartBeatProperty = TaskItemProperties.HeartBeatProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition EndTimeProperty = TaskItemProperties.EndTimeProperty;
	}
}
