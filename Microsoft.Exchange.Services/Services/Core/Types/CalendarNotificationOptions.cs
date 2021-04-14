using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CalendarNotificationOptions : OptionsPropertyChangeTracker
	{
		[DataMember]
		public bool CalendarUpdateNotification
		{
			get
			{
				return this.calendarUpdateNotification;
			}
			set
			{
				this.calendarUpdateNotification = value;
				base.TrackPropertyChanged("CalendarUpdateNotification");
			}
		}

		[DataMember]
		public int NextDays
		{
			get
			{
				return this.nextDays;
			}
			set
			{
				this.nextDays = value;
				base.TrackPropertyChanged("NextDays");
			}
		}

		[DataMember]
		public bool CalendarUpdateSendDuringWorkHour
		{
			get
			{
				return this.calendarUpdateSendDuringWorkHour;
			}
			set
			{
				this.calendarUpdateSendDuringWorkHour = value;
				base.TrackPropertyChanged("CalendarUpdateSendDuringWorkHour");
			}
		}

		[DataMember]
		public bool MeetingReminderNotification
		{
			get
			{
				return this.meetingReminderNotification;
			}
			set
			{
				this.meetingReminderNotification = value;
				base.TrackPropertyChanged("MeetingReminderNotification");
			}
		}

		[DataMember]
		public bool MeetingReminderSendDuringWorkHour
		{
			get
			{
				return this.meetingReminderSendDuringWorkHour;
			}
			set
			{
				this.meetingReminderSendDuringWorkHour = value;
				base.TrackPropertyChanged("MeetingReminderSendDuringWorkHour");
			}
		}

		[DataMember]
		public bool DailyAgendaNotification
		{
			get
			{
				return this.dailyAgendaNotification;
			}
			set
			{
				this.dailyAgendaNotification = value;
				base.TrackPropertyChanged("DailyAgendaNotification");
			}
		}

		[DataMember]
		public int DailyAgendaNotificationSendTime
		{
			get
			{
				return this.dailyAgendaNotificationSendTime;
			}
			set
			{
				this.dailyAgendaNotificationSendTime = value;
				base.TrackPropertyChanged("DailyAgendaNotificationSendTime");
			}
		}

		private bool calendarUpdateNotification;

		private int nextDays;

		private bool calendarUpdateSendDuringWorkHour;

		private bool meetingReminderNotification;

		private bool meetingReminderSendDuringWorkHour;

		private bool dailyAgendaNotification;

		private int dailyAgendaNotificationSendTime;
	}
}
