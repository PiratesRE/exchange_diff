using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class WorkHoursVersion1
	{
		public WorkHoursTimeZone TimeZone
		{
			get
			{
				return this.timeZone;
			}
			set
			{
				this.timeZone = value;
			}
		}

		public TimeSlot TimeSlot
		{
			get
			{
				return this.timeSlot;
			}
			set
			{
				this.timeSlot = value;
			}
		}

		public DaysOfWeek WorkDays
		{
			get
			{
				return this.workDays;
			}
			set
			{
				this.workDays = value;
			}
		}

		public WorkHoursVersion1()
		{
		}

		internal WorkHoursVersion1(WorkHoursTimeZone timeZone, TimeSlot timeSlot, DaysOfWeek workDays)
		{
			this.timeZone = timeZone;
			this.timeSlot = timeSlot;
			this.workDays = workDays;
		}

		private WorkHoursTimeZone timeZone;

		private TimeSlot timeSlot;

		private DaysOfWeek workDays;
	}
}
