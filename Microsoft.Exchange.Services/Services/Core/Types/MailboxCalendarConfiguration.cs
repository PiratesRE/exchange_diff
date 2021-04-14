using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class MailboxCalendarConfiguration : OptionsPropertyChangeTracker
	{
		[DataMember]
		public TimeZoneInformation CurrentTimeZone { get; set; }

		[DataMember]
		public CalendarReminder DefaultReminderTime
		{
			get
			{
				return this.defaultReminderTime;
			}
			set
			{
				this.defaultReminderTime = value;
				base.TrackPropertyChanged("DefaultReminderTime");
			}
		}

		[DataMember]
		public FirstWeekRules FirstWeekOfYear
		{
			get
			{
				return this.firstWeekOfYear;
			}
			set
			{
				this.firstWeekOfYear = value;
				base.TrackPropertyChanged("FirstWeekOfYear");
			}
		}

		[DataMember]
		public TimeZoneInformation WorkingHoursTimeZone
		{
			get
			{
				return this.workingHoursTimeZone;
			}
			set
			{
				this.workingHoursTimeZone = value;
				base.TrackPropertyChanged("WorkingHoursTimeZone");
			}
		}

		[DataMember]
		public bool RemindersEnabled
		{
			get
			{
				return this.remindersEnabled;
			}
			set
			{
				this.remindersEnabled = value;
				base.TrackPropertyChanged("RemindersEnabled");
			}
		}

		[DataMember]
		public bool ReminderSoundEnabled
		{
			get
			{
				return this.reminderSoundEnabled;
			}
			set
			{
				this.reminderSoundEnabled = value;
				base.TrackPropertyChanged("ReminderSoundEnabled");
			}
		}

		[DataMember]
		public bool ShowWeekNumbers
		{
			get
			{
				return this.showWeekNumbers;
			}
			set
			{
				this.showWeekNumbers = value;
				base.TrackPropertyChanged("ShowWeekNumbers");
			}
		}

		[DataMember]
		public HourIncrement TimeIncrement
		{
			get
			{
				return this.timeIncrement;
			}
			set
			{
				this.timeIncrement = value;
				base.TrackPropertyChanged("TimeIncrement");
			}
		}

		[DataMember]
		public Microsoft.Exchange.Data.Storage.Management.DayOfWeek WeekStartDay
		{
			get
			{
				return this.weekStartDay;
			}
			set
			{
				this.weekStartDay = value;
				base.TrackPropertyChanged("WeekStartDay");
			}
		}

		[DataMember]
		public DaysOfWeek WorkDays
		{
			get
			{
				return this.workDays;
			}
			set
			{
				this.workDays = value;
				base.TrackPropertyChanged("WorkDays");
			}
		}

		[DataMember]
		public int WorkingHoursStartTime
		{
			get
			{
				return this.workingHoursStartTime;
			}
			set
			{
				this.workingHoursStartTime = value;
				base.TrackPropertyChanged("WorkingHoursStartTime");
			}
		}

		[DataMember]
		public int WorkingHoursEndTime
		{
			get
			{
				return this.workingHoursEndTime;
			}
			set
			{
				this.workingHoursEndTime = value;
				base.TrackPropertyChanged("WorkingHoursEndTime");
			}
		}

		private CalendarReminder defaultReminderTime;

		private FirstWeekRules firstWeekOfYear;

		private bool remindersEnabled;

		private bool reminderSoundEnabled;

		private bool showWeekNumbers;

		private HourIncrement timeIncrement;

		private Microsoft.Exchange.Data.Storage.Management.DayOfWeek weekStartDay;

		private DaysOfWeek workDays;

		private int workingHoursEndTime;

		private int workingHoursStartTime;

		private TimeZoneInformation workingHoursTimeZone;
	}
}
