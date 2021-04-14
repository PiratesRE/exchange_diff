using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class CalendarNotificationSettings : BaseRow
	{
		public CalendarNotificationSettings(CalendarNotification calendarNotification) : base(calendarNotification)
		{
			this.CalendarNotification = calendarNotification;
		}

		private CalendarNotification CalendarNotification { get; set; }

		[DataMember]
		public bool DailyAgendaNotification
		{
			get
			{
				return this.CalendarNotification.DailyAgendaNotification;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public int DailyAgendaNotificationSendTime
		{
			get
			{
				return (int)this.CalendarNotification.DailyAgendaNotificationSendTime.TotalMinutes;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool CalendarUpdateNotification
		{
			get
			{
				return this.CalendarNotification.CalendarUpdateNotification;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool CalendarUpdateSendDuringWorkHour
		{
			get
			{
				return this.CalendarNotification.CalendarUpdateSendDuringWorkHour;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public int NextDays
		{
			get
			{
				return this.CalendarNotification.NextDays;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool MeetingReminderNotification
		{
			get
			{
				return this.CalendarNotification.MeetingReminderNotification;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool MeetingReminderSendDuringWorkHour
		{
			get
			{
				return this.CalendarNotification.MeetingReminderSendDuringWorkHour;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool NotificationEnabled { get; set; }

		[DataMember]
		public bool EasEnabled { get; set; }
	}
}
