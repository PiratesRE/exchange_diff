using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetCalendarNotificationSettings : SetObjectProperties
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-CalendarNotification";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Self";
			}
		}

		[DataMember]
		public bool DailyAgendaNotification
		{
			get
			{
				return (bool)(base[CalendarNotificationSchema.DailyAgendaNotification] ?? false);
			}
			set
			{
				base[CalendarNotificationSchema.DailyAgendaNotification] = value;
			}
		}

		[DataMember]
		public int DailyAgendaNotificationSendTime
		{
			get
			{
				return (int)((TimeSpan)(base[CalendarNotificationSchema.DailyAgendaNotificationSendTime] ?? TimeSpan.Zero)).TotalMinutes;
			}
			set
			{
				base[CalendarNotificationSchema.DailyAgendaNotificationSendTime] = TimeSpan.FromMinutes((double)value);
			}
		}

		[DataMember]
		public bool CalendarUpdateNotification
		{
			get
			{
				return (bool)(base[CalendarNotificationSchema.CalendarUpdateNotification] ?? false);
			}
			set
			{
				base[CalendarNotificationSchema.CalendarUpdateNotification] = value;
			}
		}

		[DataMember]
		public bool CalendarUpdateSendDuringWorkHour
		{
			get
			{
				return (bool)(base[CalendarNotificationSchema.CalendarUpdateSendDuringWorkHour] ?? false);
			}
			set
			{
				base[CalendarNotificationSchema.CalendarUpdateSendDuringWorkHour] = value;
			}
		}

		[DataMember]
		public int NextDays
		{
			get
			{
				return (int)(base[CalendarNotificationSchema.NextDays] ?? 1);
			}
			set
			{
				base[CalendarNotificationSchema.NextDays] = value;
			}
		}

		[DataMember]
		public bool MeetingReminderNotification
		{
			get
			{
				return (bool)(base[CalendarNotificationSchema.MeetingReminderNotification] ?? false);
			}
			set
			{
				base[CalendarNotificationSchema.MeetingReminderNotification] = value;
			}
		}

		[DataMember]
		public bool MeetingReminderSendDuringWorkHour
		{
			get
			{
				return (bool)(base[CalendarNotificationSchema.MeetingReminderSendDuringWorkHour] ?? false);
			}
			set
			{
				base[CalendarNotificationSchema.MeetingReminderSendDuringWorkHour] = value;
			}
		}
	}
}
