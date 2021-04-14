using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.VersionedXml;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public sealed class CalendarNotification : VersionedXmlConfigurationObject
	{
		internal override XsoMailboxConfigurationObjectSchema Schema
		{
			get
			{
				return CalendarNotification.schema;
			}
		}

		public CalendarNotification()
		{
			((SimplePropertyBag)this.propertyBag).SetObjectIdentityPropertyDefinition(VersionedXmlConfigurationObjectSchema.Identity);
		}

		public override ObjectId Identity
		{
			get
			{
				return (ADObjectId)this[VersionedXmlConfigurationObjectSchema.Identity];
			}
		}

		[Parameter]
		public bool CalendarUpdateNotification
		{
			get
			{
				return (bool)this[CalendarNotificationSchema.CalendarUpdateNotification];
			}
			set
			{
				this[CalendarNotificationSchema.CalendarUpdateNotification] = value;
			}
		}

		[Parameter]
		public int NextDays
		{
			get
			{
				return (int)this[CalendarNotificationSchema.NextDays];
			}
			set
			{
				this[CalendarNotificationSchema.NextDays] = value;
			}
		}

		[Parameter]
		public bool CalendarUpdateSendDuringWorkHour
		{
			get
			{
				return (bool)this[CalendarNotificationSchema.CalendarUpdateSendDuringWorkHour];
			}
			set
			{
				this[CalendarNotificationSchema.CalendarUpdateSendDuringWorkHour] = value;
			}
		}

		[Parameter]
		public bool MeetingReminderNotification
		{
			get
			{
				return (bool)this[CalendarNotificationSchema.MeetingReminderNotification];
			}
			set
			{
				this[CalendarNotificationSchema.MeetingReminderNotification] = value;
			}
		}

		[Parameter]
		public bool MeetingReminderSendDuringWorkHour
		{
			get
			{
				return (bool)this[CalendarNotificationSchema.MeetingReminderSendDuringWorkHour];
			}
			set
			{
				this[CalendarNotificationSchema.MeetingReminderSendDuringWorkHour] = value;
			}
		}

		[Parameter]
		public bool DailyAgendaNotification
		{
			get
			{
				return (bool)this[CalendarNotificationSchema.DailyAgendaNotification];
			}
			set
			{
				this[CalendarNotificationSchema.DailyAgendaNotification] = value;
			}
		}

		[Parameter]
		public TimeSpan DailyAgendaNotificationSendTime
		{
			get
			{
				return (TimeSpan)this[CalendarNotificationSchema.DailyAgendaNotificationSendTime];
			}
			set
			{
				this[CalendarNotificationSchema.DailyAgendaNotificationSendTime] = value;
			}
		}

		internal E164Number TextMessagingPhoneNumber
		{
			get
			{
				return (E164Number)this[CalendarNotificationSchema.TextMessagingPhoneNumber];
			}
			set
			{
				this[CalendarNotificationSchema.TextMessagingPhoneNumber] = value;
			}
		}

		internal CalendarNotificationSettingsVersion1Point0 CalendarNotificationSettings
		{
			get
			{
				return (CalendarNotificationSettingsVersion1Point0)this[CalendarNotificationSchema.CalendarNotificationSettings];
			}
			set
			{
				this[CalendarNotificationSchema.CalendarNotificationSettings] = value;
			}
		}

		internal static object CalendarNotificationSettingsGetter(IPropertyBag propertyBag)
		{
			if (propertyBag[CalendarNotificationSchema.RawCalendarNotificationSettings] == null)
			{
				bool flag = ((PropertyBag)propertyBag).IsChanged(CalendarNotificationSchema.RawCalendarNotificationSettings);
				propertyBag[CalendarNotificationSchema.RawCalendarNotificationSettings] = new CalendarNotificationSettingsVersion1Point0(new TimeSlotMonitoringSettings(false, false, DateTime.MinValue, DateTime.MinValue + TimeSpan.FromDays(1.0) - TimeSpan.FromTicks(1L), new Duration(DurationType.Days, 1U, false, DateTime.MinValue, DateTime.MinValue + TimeSpan.FromDays(1.0) - TimeSpan.FromTicks(1L), false)), new TimeSlotMonitoringSettings(false, false, DateTime.MinValue, DateTime.MinValue + TimeSpan.FromDays(1.0) - TimeSpan.FromTicks(1L), new Duration(DurationType.Days, 1U, false, DateTime.MinValue, DateTime.MinValue + TimeSpan.FromDays(1.0) - TimeSpan.FromTicks(1L), false)), new TimePointScaningSettings(false, DateTime.MinValue + TimeSpan.FromHours(8.0), new Duration(DurationType.Days, 1U, false, DateTime.MinValue, DateTime.MinValue + TimeSpan.FromDays(1.0) - TimeSpan.FromTicks(1L), false), new Recurrence(RecurrenceType.Daily, 1U, 0U, DaysOfWeek.None, WeekOrderInMonth.None, 0U)), null);
				if (!flag)
				{
					((PropertyBag)propertyBag).ResetChangeTracking(CalendarNotificationSchema.RawCalendarNotificationSettings);
				}
			}
			return (CalendarNotificationSettingsVersion1Point0)propertyBag[CalendarNotificationSchema.RawCalendarNotificationSettings];
		}

		internal static void CalendarNotificationSettingsSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[CalendarNotificationSchema.CalendarNotificationSettings] = CloneHelper.SerializeObj(value);
		}

		internal static object CalendarUpdateNotificationGetter(IPropertyBag propertyBag)
		{
			CalendarNotificationSettingsVersion1Point0 calendarNotificationSettingsVersion1Point = (CalendarNotificationSettingsVersion1Point0)propertyBag[CalendarNotificationSchema.CalendarNotificationSettings];
			return calendarNotificationSettingsVersion1Point.UpdateSettings.Enabled;
		}

		internal static void CalendarUpdateNotificationSetter(object value, IPropertyBag propertyBag)
		{
			CalendarNotificationSettingsVersion1Point0 calendarNotificationSettingsVersion1Point = (CalendarNotificationSettingsVersion1Point0)propertyBag[CalendarNotificationSchema.CalendarNotificationSettings];
			calendarNotificationSettingsVersion1Point.UpdateSettings.Enabled = (bool)value;
		}

		internal static object NextDaysGetter(IPropertyBag propertyBag)
		{
			CalendarNotificationSettingsVersion1Point0 calendarNotificationSettingsVersion1Point = (CalendarNotificationSettingsVersion1Point0)propertyBag[CalendarNotificationSchema.CalendarNotificationSettings];
			if (DurationType.Days != calendarNotificationSettingsVersion1Point.UpdateSettings.Duration.Type)
			{
				throw new DataValidationException(new PropertyValidationError(ServerStrings.ErrorCorruptedData(CalendarNotificationSchema.NextDays.Name), CalendarNotificationSchema.NextDays, calendarNotificationSettingsVersion1Point.UpdateSettings.Duration.Type));
			}
			return (int)calendarNotificationSettingsVersion1Point.UpdateSettings.Duration.Interval;
		}

		internal static void NextDaysSetter(object value, IPropertyBag propertyBag)
		{
			CalendarNotificationSettingsVersion1Point0 calendarNotificationSettingsVersion1Point = (CalendarNotificationSettingsVersion1Point0)propertyBag[CalendarNotificationSchema.CalendarNotificationSettings];
			if (DurationType.Days != calendarNotificationSettingsVersion1Point.UpdateSettings.Duration.Type)
			{
				throw new DataValidationException(new PropertyValidationError(ServerStrings.ErrorCorruptedData(CalendarNotificationSchema.NextDays.Name), CalendarNotificationSchema.NextDays, calendarNotificationSettingsVersion1Point.UpdateSettings.Duration.Type));
			}
			calendarNotificationSettingsVersion1Point.UpdateSettings.Duration.Interval = (uint)((int)value);
		}

		internal static object CalendarUpdateSendDuringWorkHourGetter(IPropertyBag propertyBag)
		{
			CalendarNotificationSettingsVersion1Point0 calendarNotificationSettingsVersion1Point = (CalendarNotificationSettingsVersion1Point0)propertyBag[CalendarNotificationSchema.CalendarNotificationSettings];
			return calendarNotificationSettingsVersion1Point.UpdateSettings.Duration.NonWorkHoursExcluded;
		}

		internal static void CalendarUpdateSendDuringWorkHourSetter(object value, IPropertyBag propertyBag)
		{
			CalendarNotificationSettingsVersion1Point0 calendarNotificationSettingsVersion1Point = (CalendarNotificationSettingsVersion1Point0)propertyBag[CalendarNotificationSchema.CalendarNotificationSettings];
			calendarNotificationSettingsVersion1Point.UpdateSettings.Duration.NonWorkHoursExcluded = (bool)value;
		}

		internal static object MeetingReminderNotificationGetter(IPropertyBag propertyBag)
		{
			CalendarNotificationSettingsVersion1Point0 calendarNotificationSettingsVersion1Point = (CalendarNotificationSettingsVersion1Point0)propertyBag[CalendarNotificationSchema.CalendarNotificationSettings];
			return calendarNotificationSettingsVersion1Point.ReminderSettings.Enabled;
		}

		internal static void MeetingReminderNotificationSetter(object value, IPropertyBag propertyBag)
		{
			CalendarNotificationSettingsVersion1Point0 calendarNotificationSettingsVersion1Point = (CalendarNotificationSettingsVersion1Point0)propertyBag[CalendarNotificationSchema.CalendarNotificationSettings];
			calendarNotificationSettingsVersion1Point.ReminderSettings.Enabled = (bool)value;
		}

		internal static object MeetingReminderSendDuringWorkHourGetter(IPropertyBag propertyBag)
		{
			CalendarNotificationSettingsVersion1Point0 calendarNotificationSettingsVersion1Point = (CalendarNotificationSettingsVersion1Point0)propertyBag[CalendarNotificationSchema.CalendarNotificationSettings];
			return calendarNotificationSettingsVersion1Point.ReminderSettings.Duration.NonWorkHoursExcluded;
		}

		internal static void MeetingReminderSendDuringWorkHourSetter(object value, IPropertyBag propertyBag)
		{
			CalendarNotificationSettingsVersion1Point0 calendarNotificationSettingsVersion1Point = (CalendarNotificationSettingsVersion1Point0)propertyBag[CalendarNotificationSchema.CalendarNotificationSettings];
			calendarNotificationSettingsVersion1Point.ReminderSettings.Duration.NonWorkHoursExcluded = (bool)value;
		}

		internal static object DailyAgendaNotificationGetter(IPropertyBag propertyBag)
		{
			CalendarNotificationSettingsVersion1Point0 calendarNotificationSettingsVersion1Point = (CalendarNotificationSettingsVersion1Point0)propertyBag[CalendarNotificationSchema.CalendarNotificationSettings];
			return calendarNotificationSettingsVersion1Point.SummarySettings.Enabled;
		}

		internal static void DailyAgendaNotificationSetter(object value, IPropertyBag propertyBag)
		{
			CalendarNotificationSettingsVersion1Point0 calendarNotificationSettingsVersion1Point = (CalendarNotificationSettingsVersion1Point0)propertyBag[CalendarNotificationSchema.CalendarNotificationSettings];
			calendarNotificationSettingsVersion1Point.SummarySettings.Enabled = (bool)value;
		}

		internal static object DailyAgendaNotificationSendTimeGetter(IPropertyBag propertyBag)
		{
			CalendarNotificationSettingsVersion1Point0 calendarNotificationSettingsVersion1Point = (CalendarNotificationSettingsVersion1Point0)propertyBag[CalendarNotificationSchema.CalendarNotificationSettings];
			return calendarNotificationSettingsVersion1Point.SummarySettings.NotifyingTimeInDay - calendarNotificationSettingsVersion1Point.SummarySettings.NotifyingTimeInDay.Date;
		}

		internal static void DailyAgendaNotificationSendTimeSetter(object value, IPropertyBag propertyBag)
		{
			CalendarNotificationSettingsVersion1Point0 calendarNotificationSettingsVersion1Point = (CalendarNotificationSettingsVersion1Point0)propertyBag[CalendarNotificationSchema.CalendarNotificationSettings];
			calendarNotificationSettingsVersion1Point.SummarySettings.NotifyingTimeInDay = DateTime.MinValue.Date + (TimeSpan)value;
		}

		internal static object TextMessagingPhoneNumberGetter(IPropertyBag propertyBag)
		{
			CalendarNotificationSettingsVersion1Point0 calendarNotificationSettingsVersion1Point = (CalendarNotificationSettingsVersion1Point0)propertyBag[CalendarNotificationSchema.CalendarNotificationSettings];
			Emitter emitter = null;
			foreach (Emitter emitter2 in calendarNotificationSettingsVersion1Point.Emitters)
			{
				if (EmitterType.TextMessaging == emitter2.Type)
				{
					emitter = emitter2;
					break;
				}
			}
			if (emitter == null || emitter.PhoneNumbers.Count == 0)
			{
				return null;
			}
			return emitter.PhoneNumbers[0];
		}

		internal static void TextMessagingPhoneNumberSetter(object value, IPropertyBag propertyBag)
		{
			CalendarNotificationSettingsVersion1Point0 calendarNotificationSettingsVersion1Point = (CalendarNotificationSettingsVersion1Point0)propertyBag[CalendarNotificationSchema.CalendarNotificationSettings];
			Emitter emitter = null;
			foreach (Emitter emitter2 in calendarNotificationSettingsVersion1Point.Emitters)
			{
				if (EmitterType.TextMessaging == emitter2.Type)
				{
					emitter = emitter2;
					break;
				}
			}
			if (value == null)
			{
				if (emitter != null)
				{
					calendarNotificationSettingsVersion1Point.Emitters.Remove(emitter);
					return;
				}
			}
			else
			{
				if (emitter == null)
				{
					calendarNotificationSettingsVersion1Point.Emitters.Add(new Emitter(EmitterType.TextMessaging, 0, true, new E164Number[]
					{
						(E164Number)value
					}));
					return;
				}
				emitter.PhoneNumbers.Clear();
				emitter.PhoneNumbers.Add((E164Number)value);
			}
		}

		public override string ToString()
		{
			if (this.Identity != null)
			{
				return this.Identity.ToString();
			}
			return base.ToString();
		}

		internal override string UserConfigurationName
		{
			get
			{
				return "CalendarNotification.001";
			}
		}

		internal override ProviderPropertyDefinition RawVersionedXmlPropertyDefinition
		{
			get
			{
				return CalendarNotificationSchema.RawCalendarNotificationSettings;
			}
		}

		internal const string ConfigurationName = "CalendarNotification.001";

		private static XsoMailboxConfigurationObjectSchema schema = ObjectSchema.GetInstance<CalendarNotificationSchema>();
	}
}
