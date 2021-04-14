using System;

namespace Microsoft.Exchange.Hygiene.Data.BackgroundJobBackend
{
	internal sealed class ScheduleItemActiveUpdate : BackgroundJobBackendBase
	{
		public ScheduleItemActiveUpdate(Guid backgroundJobId, Guid scheduleId, bool active)
		{
			this.ScheduleId = scheduleId;
			this.BackgroundJobId = backgroundJobId;
			this.Active = active;
		}

		public Guid ScheduleId
		{
			get
			{
				return (Guid)this[ScheduleItemActiveUpdate.ScheduleIdProperty];
			}
			set
			{
				this[ScheduleItemActiveUpdate.ScheduleIdProperty] = value;
			}
		}

		public Guid BackgroundJobId
		{
			get
			{
				return (Guid)this[ScheduleItemActiveUpdate.BackgroundJobIdProperty];
			}
			set
			{
				this[ScheduleItemActiveUpdate.BackgroundJobIdProperty] = value;
			}
		}

		public bool Active
		{
			get
			{
				return (bool)this[ScheduleItemActiveUpdate.ActiveProperty];
			}
			set
			{
				this[ScheduleItemActiveUpdate.ActiveProperty] = value;
			}
		}

		internal static readonly BackgroundJobBackendPropertyDefinition ScheduleIdProperty = ScheduleItemProperties.ScheduleIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition BackgroundJobIdProperty = ScheduleItemProperties.BackgroundJobIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition ActiveProperty = ScheduleItemProperties.ActiveProperty;
	}
}
