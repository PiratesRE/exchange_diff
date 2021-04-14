using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.BackgroundJobBackend
{
	internal sealed class TakeTaskOwnership : BackgroundJobBackendBase
	{
		public TakeTaskOwnership(Guid backgroundJobId, Guid taskId, Guid ownerId, int ownerFitnessScore, DateTime startTime)
		{
			this[TakeTaskOwnership.BackgroundJobIdProperty] = backgroundJobId;
			this[TakeTaskOwnership.TaskIdProperty] = taskId;
			this[TakeTaskOwnership.BJMOwnerIdProperty] = ownerId;
			this[TakeTaskOwnership.OwnerFitnessScoreProperty] = ownerFitnessScore;
			this[TakeTaskOwnership.StartTimeProperty] = startTime;
			this[TakeTaskOwnership.OwnershipTakenProperty] = false;
		}

		public Guid BackgroundJobId
		{
			get
			{
				return (Guid)this[TakeTaskOwnership.BackgroundJobIdProperty];
			}
		}

		public Guid TaskId
		{
			get
			{
				return (Guid)this[TakeTaskOwnership.TaskIdProperty];
			}
		}

		public Guid BJMOwnerId
		{
			get
			{
				return (Guid)this[TakeTaskOwnership.BJMOwnerIdProperty];
			}
		}

		public int OwnerFitnessScore
		{
			get
			{
				return (int)this[TakeTaskOwnership.OwnerFitnessScoreProperty];
			}
		}

		public DateTime StartTime
		{
			get
			{
				return (DateTime)this[TakeTaskOwnership.StartTimeProperty];
			}
		}

		public bool OwnershipTaken
		{
			get
			{
				return (bool)this[TakeTaskOwnership.OwnershipTakenProperty];
			}
			set
			{
				this[TakeTaskOwnership.OwnershipTakenProperty] = value;
			}
		}

		internal static readonly BackgroundJobBackendPropertyDefinition BackgroundJobIdProperty = ScheduleItemProperties.BackgroundJobIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition TaskIdProperty = TaskItemProperties.TaskIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition BJMOwnerIdProperty = TaskItemProperties.BJMOwnerIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition OwnerFitnessScoreProperty = TaskItemProperties.OwnerFitnessScoreProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition StartTimeProperty = TaskItemProperties.StartTimeProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition OwnershipTakenProperty = new BackgroundJobBackendPropertyDefinition("OwnershipTaken", typeof(bool), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, false);
	}
}
