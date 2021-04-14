using System;

namespace Microsoft.Exchange.Hygiene.Data.BackgroundJobBackend
{
	internal sealed class TaskItem : BackgroundJobBackendBase
	{
		public Guid ActiveJobId
		{
			get
			{
				return (Guid)this[TaskItem.ActiveJobIdProperty];
			}
			set
			{
				this[TaskItem.ActiveJobIdProperty] = value;
			}
		}

		public Guid TaskId
		{
			get
			{
				return (Guid)this[TaskItem.TaskIdProperty];
			}
			set
			{
				this[TaskItem.TaskIdProperty] = value;
			}
		}

		public Guid ScheduleId
		{
			get
			{
				return (Guid)this[TaskItem.ScheduleIdProperty];
			}
			set
			{
				this[TaskItem.ScheduleIdProperty] = value;
			}
		}

		public Guid BackgroundJobId
		{
			get
			{
				return (Guid)this[TaskItem.BackgroundJobIdProperty];
			}
			set
			{
				this[TaskItem.BackgroundJobIdProperty] = value;
			}
		}

		public Guid RoleId
		{
			get
			{
				return (Guid)this[TaskItem.RoleIdProperty];
			}
			set
			{
				this[TaskItem.RoleIdProperty] = value;
			}
		}

		public int InstanceId
		{
			get
			{
				return (int)this[TaskItem.InstanceIdProperty];
			}
			set
			{
				this[TaskItem.InstanceIdProperty] = value;
			}
		}

		public TaskExecutionStateType TaskExecutionState
		{
			get
			{
				return (TaskExecutionStateType)this[TaskItem.TaskExecutionStateProperty];
			}
			set
			{
				this[TaskItem.TaskExecutionStateProperty] = (byte)value;
			}
		}

		public bool HasTaskCompletionStatus
		{
			get
			{
				return this[TaskItem.TaskCompletionStatusProperty] != null;
			}
		}

		public TaskCompletionStatusType TaskCompletionStatus
		{
			get
			{
				return (TaskCompletionStatusType)this[TaskItem.TaskCompletionStatusProperty];
			}
			set
			{
				this[TaskItem.TaskCompletionStatusProperty] = (byte)value;
			}
		}

		public Guid ParentTaskId
		{
			get
			{
				return (Guid)this[TaskItem.ParentTaskIdProperty];
			}
			set
			{
				this[TaskItem.ParentTaskIdProperty] = value;
			}
		}

		public short ExecutionAttempt
		{
			get
			{
				return (short)this[TaskItem.ExecutionAttemptProperty];
			}
			set
			{
				this[TaskItem.ExecutionAttemptProperty] = value;
			}
		}

		public bool HasBJMOwnerId
		{
			get
			{
				return this[TaskItem.BJMOwnerIdProperty] != null;
			}
		}

		public Guid BJMOwnerId
		{
			get
			{
				return (Guid)this[TaskItem.BJMOwnerIdProperty];
			}
			set
			{
				this[TaskItem.BJMOwnerIdProperty] = value;
			}
		}

		public string TargetMachineName
		{
			get
			{
				return (string)this[TaskItem.TargetMachineNameProperty];
			}
		}

		public bool HasOwnerFitnessScore
		{
			get
			{
				return this[TaskItem.OwnerFitnessScoreProperty] != null;
			}
		}

		public int OwnerFitnessScore
		{
			get
			{
				return (int)this[TaskItem.OwnerFitnessScoreProperty];
			}
			set
			{
				this[TaskItem.OwnerFitnessScoreProperty] = value;
			}
		}

		public bool HasStartTime
		{
			get
			{
				return this[TaskItem.StartTimeProperty] != null;
			}
		}

		public DateTime StartTime
		{
			get
			{
				return (DateTime)this[TaskItem.StartTimeProperty];
			}
			set
			{
				this[TaskItem.StartTimeProperty] = value;
			}
		}

		public bool HasEndTime
		{
			get
			{
				return this[TaskItem.EndTimeProperty] != null;
			}
		}

		public DateTime EndTime
		{
			get
			{
				return (DateTime)this[TaskItem.EndTimeProperty];
			}
			set
			{
				this[TaskItem.EndTimeProperty] = value;
			}
		}

		public bool HasHeartBeat
		{
			get
			{
				return this[TaskItem.HeartBeatProperty] != null;
			}
		}

		public DateTime HeartBeat
		{
			get
			{
				return (DateTime)this[TaskItem.HeartBeatProperty];
			}
			set
			{
				this[TaskItem.HeartBeatProperty] = value;
			}
		}

		public DateTime InsertTimeStamp
		{
			get
			{
				return (DateTime)this[TaskItem.InsertTimeStampProperty];
			}
			set
			{
				this[TaskItem.InsertTimeStampProperty] = value;
			}
		}

		internal static readonly BackgroundJobBackendPropertyDefinition ActiveJobIdProperty = TaskItemProperties.ActiveJobIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition TaskIdProperty = TaskItemProperties.TaskIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition ScheduleIdProperty = TaskItemProperties.ScheduleIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition BackgroundJobIdProperty = TaskItemProperties.BackgroundJobIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition RoleIdProperty = TaskItemProperties.RoleIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition InstanceIdProperty = TaskItemProperties.InstanceIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition TaskExecutionStateProperty = TaskItemProperties.TaskExecutionStateProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition TaskCompletionStatusProperty = TaskItemProperties.TaskCompletionStatusProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition ParentTaskIdProperty = TaskItemProperties.ParentTaskIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition TargetMachineNameProperty = ScheduleItemProperties.TargetMachineNameProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition ExecutionAttemptProperty = TaskItemProperties.ExecutionAttemptProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition BJMOwnerIdProperty = TaskItemProperties.BJMOwnerIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition OwnerFitnessScoreProperty = TaskItemProperties.OwnerFitnessScoreProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition StartTimeProperty = TaskItemProperties.StartTimeProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition EndTimeProperty = TaskItemProperties.EndTimeProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition HeartBeatProperty = TaskItemProperties.HeartBeatProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition InsertTimeStampProperty = TaskItemProperties.InsertTimeStampProperty;
	}
}
