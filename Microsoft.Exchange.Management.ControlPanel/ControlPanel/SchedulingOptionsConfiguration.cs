using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SchedulingOptionsConfiguration : ResourceConfigurationBase
	{
		public SchedulingOptionsConfiguration(CalendarConfiguration calendarConfiguration) : base(calendarConfiguration)
		{
		}

		public MailboxCalendarConfiguration MailboxCalendarConfiguration { get; set; }

		[DataMember]
		public bool AutoAcceptAutomateProcessing
		{
			get
			{
				return base.CalendarConfiguration.AutomateProcessing == CalendarProcessingFlags.AutoAccept;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool DisableReminders
		{
			get
			{
				return this.MailboxCalendarConfiguration != null && !this.MailboxCalendarConfiguration.RemindersEnabled;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public int BookingWindowInDays
		{
			get
			{
				return base.CalendarConfiguration.BookingWindowInDays;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool EnforceSchedulingHorizon
		{
			get
			{
				return base.CalendarConfiguration.EnforceSchedulingHorizon;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool LimitDuration
		{
			get
			{
				return this.MaximumDurationInMinutes != 0;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public int MaximumDurationInMinutes
		{
			get
			{
				return base.CalendarConfiguration.MaximumDurationInMinutes;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool ScheduleOnlyDuringWorkHours
		{
			get
			{
				return base.CalendarConfiguration.ScheduleOnlyDuringWorkHours;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool AllowRecurringMeetings
		{
			get
			{
				return base.CalendarConfiguration.AllowRecurringMeetings;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool AllowConflicts
		{
			get
			{
				return base.CalendarConfiguration.AllowConflicts;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public int MaximumConflictInstances
		{
			get
			{
				return base.CalendarConfiguration.MaximumConflictInstances;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public int ConflictPercentageAllowed
		{
			get
			{
				return base.CalendarConfiguration.ConflictPercentageAllowed;
			}
			set
			{
				throw new NotSupportedException();
			}
		}
	}
}
