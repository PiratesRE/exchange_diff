using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Connections.Eas.Model.Common.AirSyncBase;
using Microsoft.Exchange.Connections.Eas.Model.Common.Email;
using Microsoft.Exchange.Connections.Eas.Model.Request.AirSync;
using Microsoft.Exchange.Connections.Eas.Model.Request.AirSyncBase;
using Microsoft.Exchange.Connections.Eas.Model.Request.Calendar;
using Microsoft.Exchange.Connections.Eas.Model.Response.Calendar;
using Microsoft.Exchange.Connections.Eas.Model.Response.ItemOperations;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring.Recurrence;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.TypeConversion.Converters;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal static class SyncCalendarUtils
	{
		public static PropertyTag GlobalObjectId
		{
			get
			{
				return SyncCalendarUtils.GlobalObjectIdPropertyTag;
			}
		}

		public static PropertyTag Start
		{
			get
			{
				return SyncCalendarUtils.StartPropertyTag;
			}
		}

		public static PropertyTag End
		{
			get
			{
				return SyncCalendarUtils.EndPropertyTag;
			}
		}

		public static PropertyTag AllDayEvent
		{
			get
			{
				return SyncCalendarUtils.AllDayEventPropertyTag;
			}
		}

		public static PropertyTag BusyStatus
		{
			get
			{
				return SyncCalendarUtils.BusyStatusPropertyTag;
			}
		}

		public static PropertyTag Location
		{
			get
			{
				return SyncCalendarUtils.LocationPropertyTag;
			}
		}

		public static PropertyTag Reminder
		{
			get
			{
				return SyncCalendarUtils.ReminderPropertyTag;
			}
		}

		public static PropertyTag TimeZoneBlob
		{
			get
			{
				return SyncCalendarUtils.TimeZoneBlobPropertyTag;
			}
		}

		public static PropertyTag Sensitivity
		{
			get
			{
				return SyncCalendarUtils.SensitivityPropertyTag;
			}
		}

		public static PropertyTag SentRepresentingName
		{
			get
			{
				return SyncCalendarUtils.SentRepresentingNamePropertyTag;
			}
		}

		public static PropertyTag SentRepresentingEmailAddress
		{
			get
			{
				return SyncCalendarUtils.SentRepresentingEmailAddressPropertyTag;
			}
		}

		public static PropertyTag MeetingStatus
		{
			get
			{
				return SyncCalendarUtils.MeetingStatusPropertyTag;
			}
		}

		public static PropertyTag AppointmentRecurrenceBlob
		{
			get
			{
				return SyncCalendarUtils.AppointmentRecurrenceBlobPropertyTag;
			}
		}

		public static PropertyTag ResponseType
		{
			get
			{
				return SyncCalendarUtils.ResponseTypePropertyTag;
			}
		}

		public static PropertyTag RecipientTrackStatus
		{
			get
			{
				return SyncCalendarUtils.RecipientTrackStatusPropertyTag;
			}
		}

		public static PropertyTag RecipientType
		{
			get
			{
				return SyncCalendarUtils.RecipientTypePropertyTag;
			}
		}

		public static PropertyTag EmailAddress
		{
			get
			{
				return SyncCalendarUtils.EmailAddressPropertyTag;
			}
		}

		public static PropertyTag DisplayName
		{
			get
			{
				return SyncCalendarUtils.DisplayNamePropertyTag;
			}
		}

		public static PropertyTag RowId
		{
			get
			{
				return SyncCalendarUtils.RowIdPropertyTag;
			}
		}

		internal static ExDateTime ToExDateTime(string value)
		{
			ExDateTime result;
			if (!ExDateTime.TryParseExact(value, "yyyyMMdd\\THHmmss\\Z", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result))
			{
				throw new EasFetchFailedPermanentException("ToExDateTime: " + value);
			}
			return result;
		}

		internal static string ToStringDateTime(ExDateTime value)
		{
			return value.ToUtc().ToString("yyyyMMdd\\THHmmss\\Z");
		}

		internal static ExDateTime ToUtcExDateTime(string value)
		{
			return ExTimeZone.UtcTimeZone.ConvertDateTime(SyncCalendarUtils.ToExDateTime(value));
		}

		internal static ExTimeZone ToExTimeZone(string timezone)
		{
			byte[] array = Convert.FromBase64String(timezone);
			if (array.Length != 172)
			{
				throw new EasFetchFailedPermanentException("ToExTimeZone: " + timezone);
			}
			int num = 0;
			char[] array2 = new char[32];
			REG_TIMEZONE_INFO regInfo;
			regInfo.Bias = BitConverter.ToInt32(array, num);
			num += 4;
			for (int i = 0; i < 32; i++)
			{
				array2[i] = BitConverter.ToChar(array, num);
				num += 2;
			}
			string keyName = new string(array2);
			regInfo.StandardDate.Year = (ushort)BitConverter.ToInt16(array, num);
			num += 2;
			regInfo.StandardDate.Month = (ushort)BitConverter.ToInt16(array, num);
			num += 2;
			regInfo.StandardDate.DayOfWeek = (ushort)BitConverter.ToInt16(array, num);
			num += 2;
			regInfo.StandardDate.Day = (ushort)BitConverter.ToInt16(array, num);
			num += 2;
			regInfo.StandardDate.Hour = (ushort)BitConverter.ToInt16(array, num);
			num += 2;
			regInfo.StandardDate.Minute = (ushort)BitConverter.ToInt16(array, num);
			num += 2;
			regInfo.StandardDate.Second = (ushort)BitConverter.ToInt16(array, num);
			num += 2;
			regInfo.StandardDate.Milliseconds = (ushort)BitConverter.ToInt16(array, num);
			num += 2;
			regInfo.StandardBias = BitConverter.ToInt32(array, num);
			num += 4;
			for (int j = 0; j < 32; j++)
			{
				array2[j] = BitConverter.ToChar(array, num);
				num += 2;
			}
			regInfo.DaylightDate.Year = (ushort)BitConverter.ToInt16(array, num);
			num += 2;
			regInfo.DaylightDate.Month = (ushort)BitConverter.ToInt16(array, num);
			num += 2;
			regInfo.DaylightDate.DayOfWeek = (ushort)BitConverter.ToInt16(array, num);
			num += 2;
			regInfo.DaylightDate.Day = (ushort)BitConverter.ToInt16(array, num);
			num += 2;
			regInfo.DaylightDate.Hour = (ushort)BitConverter.ToInt16(array, num);
			num += 2;
			regInfo.DaylightDate.Minute = (ushort)BitConverter.ToInt16(array, num);
			num += 2;
			regInfo.DaylightDate.Second = (ushort)BitConverter.ToInt16(array, num);
			num += 2;
			regInfo.DaylightDate.Milliseconds = (ushort)BitConverter.ToInt16(array, num);
			num += 2;
			regInfo.DaylightBias = BitConverter.ToInt32(array, num);
			num += 4;
			if (num != 172)
			{
				return ExTimeZone.CurrentTimeZone;
			}
			ExTimeZone result;
			try
			{
				result = TimeZoneHelper.CreateExTimeZoneFromRegTimeZoneInfo(regInfo, keyName);
			}
			catch (InvalidTimeZoneException innerException)
			{
				throw new EasFetchFailedPermanentException("ToExTimeZone: " + timezone, innerException);
			}
			return result;
		}

		public static string ToTimeZoneString(ExTimeZone srcTimeZone)
		{
			if (srcTimeZone == null)
			{
				throw new EasSyncFailedPermanentException("Null source time zone");
			}
			byte[] array = new byte[172];
			int num = 0;
			REG_TIMEZONE_INFO reg_TIMEZONE_INFO = TimeZoneHelper.RegTimeZoneInfoFromExTimeZone(srcTimeZone);
			byte[] bytes = BitConverter.GetBytes(reg_TIMEZONE_INFO.Bias);
			bytes.CopyTo(array, num);
			num += Marshal.SizeOf(reg_TIMEZONE_INFO.Bias);
			char[] array2 = srcTimeZone.LocalizableDisplayName.ToString(CultureInfo.InvariantCulture).ToCharArray();
			int num2 = Math.Min(array2.Length, 32);
			for (int i = 0; i < num2; i++)
			{
				BitConverter.GetBytes(array2[i]).CopyTo(array, num);
				num += 2;
			}
			for (int j = num2; j < 32; j++)
			{
				array[num++] = 0;
				array[num++] = 0;
			}
			BitConverter.GetBytes(reg_TIMEZONE_INFO.StandardDate.Year).CopyTo(array, num);
			num += 2;
			BitConverter.GetBytes(reg_TIMEZONE_INFO.StandardDate.Month).CopyTo(array, num);
			num += 2;
			BitConverter.GetBytes(reg_TIMEZONE_INFO.StandardDate.DayOfWeek).CopyTo(array, num);
			num += 2;
			BitConverter.GetBytes(reg_TIMEZONE_INFO.StandardDate.Day).CopyTo(array, num);
			num += 2;
			BitConverter.GetBytes(reg_TIMEZONE_INFO.StandardDate.Hour).CopyTo(array, num);
			num += 2;
			BitConverter.GetBytes(reg_TIMEZONE_INFO.StandardDate.Minute).CopyTo(array, num);
			num += 2;
			BitConverter.GetBytes(reg_TIMEZONE_INFO.StandardDate.Second).CopyTo(array, num);
			num += 2;
			BitConverter.GetBytes(reg_TIMEZONE_INFO.StandardDate.Milliseconds).CopyTo(array, num);
			num += 2;
			bytes = BitConverter.GetBytes(reg_TIMEZONE_INFO.StandardBias);
			bytes.CopyTo(array, num);
			num += Marshal.SizeOf(reg_TIMEZONE_INFO.StandardBias);
			array2 = srcTimeZone.LocalizableDisplayName.ToString(CultureInfo.InvariantCulture).ToCharArray();
			num2 = Math.Min(array2.Length, 32);
			for (int k = 0; k < num2; k++)
			{
				BitConverter.GetBytes(array2[k]).CopyTo(array, num);
				num += 2;
			}
			for (int l = num2; l < 32; l++)
			{
				array[num++] = 0;
				array[num++] = 0;
			}
			BitConverter.GetBytes(reg_TIMEZONE_INFO.DaylightDate.Year).CopyTo(array, num);
			num += 2;
			BitConverter.GetBytes(reg_TIMEZONE_INFO.DaylightDate.Month).CopyTo(array, num);
			num += 2;
			BitConverter.GetBytes(reg_TIMEZONE_INFO.DaylightDate.DayOfWeek).CopyTo(array, num);
			num += 2;
			BitConverter.GetBytes(reg_TIMEZONE_INFO.DaylightDate.Day).CopyTo(array, num);
			num += 2;
			BitConverter.GetBytes(reg_TIMEZONE_INFO.DaylightDate.Hour).CopyTo(array, num);
			num += 2;
			BitConverter.GetBytes(reg_TIMEZONE_INFO.DaylightDate.Minute).CopyTo(array, num);
			num += 2;
			BitConverter.GetBytes(reg_TIMEZONE_INFO.DaylightDate.Second).CopyTo(array, num);
			num += 2;
			BitConverter.GetBytes(reg_TIMEZONE_INFO.DaylightDate.Milliseconds).CopyTo(array, num);
			num += 2;
			bytes = BitConverter.GetBytes(reg_TIMEZONE_INFO.DaylightBias);
			bytes.CopyTo(array, num);
			num += Marshal.SizeOf(reg_TIMEZONE_INFO.DaylightBias);
			if (num != 172)
			{
				throw new EasSyncFailedPermanentException("Failed to convert Timezone into bytes. Length=" + num);
			}
			return Convert.ToBase64String(array);
		}

		public static ApplicationData ConvertEventToAppData(Event theEvent, IList<Event> exceptionalEvents, IList<string> deletedOccurrences, UserSmtpAddress userSmtpAddress)
		{
			ExTimeZone utcTimeZone;
			if (theEvent.IntendedStartTimeZoneId == "tzone://Microsoft/Utc" || theEvent.IntendedStartTimeZoneId == null)
			{
				utcTimeZone = ExTimeZone.UtcTimeZone;
			}
			else
			{
				ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(theEvent.IntendedStartTimeZoneId, out utcTimeZone);
			}
			bool flag = false;
			byte b = 0;
			if (theEvent.HasAttendees)
			{
				b |= 1;
				flag = true;
			}
			if (theEvent.IsCancelled)
			{
				b |= 4;
			}
			if (((IEventInternal)theEvent).IsReceived)
			{
				b |= 2;
			}
			ApplicationData applicationData = new ApplicationData();
			SyncCalendarUtils.CopyCommonEventData(applicationData, theEvent, flag, userSmtpAddress);
			applicationData.TimeZone = ((utcTimeZone != null) ? SyncCalendarUtils.ToTimeZoneString(utcTimeZone) : null);
			applicationData.MeetingStatus = new byte?(b);
			applicationData.Uid = (string.IsNullOrEmpty(((IEventInternal)theEvent).GlobalObjectId) ? null : new GlobalObjectId(((IEventInternal)theEvent).GlobalObjectId).Uid);
			applicationData.Recurrence = SyncCalendarUtils.GetRecurrenceData(theEvent.PatternedRecurrence);
			applicationData.Exceptions = SyncCalendarUtils.GetExceptionData(exceptionalEvents, deletedOccurrences, flag, theEvent.Start, userSmtpAddress);
			if (flag)
			{
				applicationData.OrganizerEmail = theEvent.Organizer.EmailAddress;
				applicationData.OrganizerName = theEvent.Organizer.Name;
				applicationData.ResponseRequested = new byte?(Convert.ToByte(theEvent.ResponseRequested));
			}
			return applicationData;
		}

		public static byte[] ToRecurrenceBlob(Properties easCalendarItem, ExDateTime start, ExDateTime end, ExTimeZone targetTimeZone)
		{
			Microsoft.Exchange.Connections.Eas.Model.Response.Calendar.Recurrence recurrence = easCalendarItem.Recurrence;
			RecurrencePattern pattern = SyncCalendarUtils.CreateRecurrencePattern(recurrence);
			RecurrenceRange range = SyncCalendarUtils.CreateRecurrenceRange(start, recurrence);
			ExDateTime dt = targetTimeZone.ConvertDateTime(start);
			ExDateTime dt2 = targetTimeZone.ConvertDateTime(end);
			TimeSpan startOffset = dt - dt.Date;
			TimeSpan endOffset = dt2 - dt2.Date;
			InternalRecurrence internalRecurrence = new InternalRecurrence(pattern, range, null, targetTimeZone, ExTimeZone.UtcTimeZone, startOffset, endOffset);
			if (easCalendarItem.Exceptions != null)
			{
				foreach (Microsoft.Exchange.Connections.Eas.Model.Response.Calendar.Exception ex in easCalendarItem.Exceptions)
				{
					ExDateTime originalStartTime = SyncCalendarUtils.ToUtcExDateTime(ex.ExceptionStartTime);
					ExDateTime date = originalStartTime.Date;
					if (ex.Deleted)
					{
						internalRecurrence.TryDeleteOccurrence(date);
					}
					else
					{
						ModificationType modificationType = (ModificationType)0;
						MemoryPropertyBag memoryPropertyBag = new MemoryPropertyBag();
						memoryPropertyBag.SetAllPropertiesLoaded();
						if (ex.Subject != easCalendarItem.CalendarSubject)
						{
							modificationType |= ModificationType.Subject;
							memoryPropertyBag[ItemSchema.Subject] = ex.Subject;
						}
						if (ex.Reminder != easCalendarItem.Reminder)
						{
							modificationType |= ModificationType.ReminderDelta;
							memoryPropertyBag[ItemSchema.ReminderMinutesBeforeStartInternal] = ex.Reminder;
						}
						if (ex.Location != easCalendarItem.Location)
						{
							modificationType |= ModificationType.Location;
							memoryPropertyBag[CalendarItemBaseSchema.Location] = ex.Location;
						}
						if (ex.BusyStatus != easCalendarItem.BusyStatus)
						{
							modificationType |= ModificationType.BusyStatus;
							memoryPropertyBag[CalendarItemBaseSchema.FreeBusyStatus] = ex.BusyStatus;
						}
						if (ex.AllDayEvent != easCalendarItem.AllDayEvent)
						{
							modificationType |= ModificationType.SubType;
							memoryPropertyBag[CalendarItemBaseSchema.MapiIsAllDayEvent] = ex.AllDayEvent;
						}
						ExDateTime startTime = targetTimeZone.ConvertDateTime(SyncCalendarUtils.ToUtcExDateTime(ex.StartTime));
						ExDateTime endTime = targetTimeZone.ConvertDateTime(SyncCalendarUtils.ToUtcExDateTime(ex.EndTime));
						ExceptionInfo exceptionInfo = new ExceptionInfo(null, date, originalStartTime, startTime, endTime, modificationType, memoryPropertyBag);
						internalRecurrence.ModifyOccurrence(exceptionInfo);
					}
				}
			}
			return internalRecurrence.ToByteArray();
		}

		public static Microsoft.Exchange.Connections.Eas.Model.Request.Calendar.Recurrence GetRecurrenceData(PatternedRecurrence recurrence)
		{
			if (recurrence == null)
			{
				return null;
			}
			Microsoft.Exchange.Connections.Eas.Model.Request.Calendar.Recurrence recurrence2 = new Microsoft.Exchange.Connections.Eas.Model.Request.Calendar.Recurrence();
			recurrence2.Interval = new ushort?((ushort)recurrence.Pattern.Interval);
			RecurrencePatternType type = recurrence.Pattern.Type;
			switch (type)
			{
			case RecurrencePatternType.Daily:
			{
				DailyRecurrencePattern dailyRecurrencePattern = (DailyRecurrencePattern)recurrence.Pattern;
				recurrence2.Type = 0;
				break;
			}
			case RecurrencePatternType.Weekly:
			{
				WeeklyRecurrencePattern weeklyRecurrencePattern = (WeeklyRecurrencePattern)recurrence.Pattern;
				recurrence2.Type = 1;
				recurrence2.DayOfWeek = new ushort?(SyncCalendarUtils.GetDayOfWeekValue(weeklyRecurrencePattern.DaysOfWeek));
				break;
			}
			case RecurrencePatternType.AbsoluteMonthly:
			{
				AbsoluteMonthlyRecurrencePattern absoluteMonthlyRecurrencePattern = (AbsoluteMonthlyRecurrencePattern)recurrence.Pattern;
				recurrence2.Type = 2;
				recurrence2.DayOfMonth = new byte?((byte)absoluteMonthlyRecurrencePattern.DayOfMonth);
				break;
			}
			case RecurrencePatternType.RelativeMonthly:
			{
				RelativeMonthlyRecurrencePattern relativeMonthlyRecurrencePattern = (RelativeMonthlyRecurrencePattern)recurrence.Pattern;
				recurrence2.Type = 3;
				recurrence2.DayOfWeek = new ushort?(SyncCalendarUtils.GetDayOfWeekValue(relativeMonthlyRecurrencePattern.DaysOfWeek));
				recurrence2.WeekOfMonth = new byte?((byte)relativeMonthlyRecurrencePattern.Index);
				break;
			}
			case RecurrencePatternType.AbsoluteYearly:
			{
				AbsoluteYearlyRecurrencePattern absoluteYearlyRecurrencePattern = (AbsoluteYearlyRecurrencePattern)recurrence.Pattern;
				recurrence2.Type = 5;
				recurrence2.DayOfMonth = new byte?((byte)absoluteYearlyRecurrencePattern.DayOfMonth);
				recurrence2.MonthOfYear = new byte?((byte)absoluteYearlyRecurrencePattern.Month);
				break;
			}
			case RecurrencePatternType.RelativeYearly:
			{
				RelativeYearlyRecurrencePattern relativeYearlyRecurrencePattern = (RelativeYearlyRecurrencePattern)recurrence.Pattern;
				recurrence2.Type = 6;
				recurrence2.DayOfWeek = new ushort?(SyncCalendarUtils.GetDayOfWeekValue(relativeYearlyRecurrencePattern.DaysOfWeek));
				recurrence2.WeekOfMonth = new byte?((byte)relativeYearlyRecurrencePattern.Index);
				recurrence2.MonthOfYear = new byte?((byte)relativeYearlyRecurrencePattern.Month);
				break;
			}
			default:
				throw new EasSyncFailedPermanentException("Invalid recurrence type: " + type);
			}
			RecurrenceRangeType type2 = recurrence.Range.Type;
			switch (type2)
			{
			case RecurrenceRangeType.EndDate:
			{
				EndDateRecurrenceRange endDateRecurrenceRange = (EndDateRecurrenceRange)recurrence.Range;
				recurrence2.Until = SyncCalendarUtils.ToStringDateTime(endDateRecurrenceRange.EndDate);
				break;
			}
			case RecurrenceRangeType.NoEnd:
				break;
			case RecurrenceRangeType.Numbered:
			{
				NumberedRecurrenceRange numberedRecurrenceRange = (NumberedRecurrenceRange)recurrence.Range;
				recurrence2.Occurrences = new ushort?((ushort)numberedRecurrenceRange.NumberOfOccurrences);
				break;
			}
			default:
				throw new EasSyncFailedPermanentException("Invalid recurrence range type: {0}" + type2);
			}
			return recurrence2;
		}

		public static List<Microsoft.Exchange.Connections.Eas.Model.Request.Calendar.Exception> GetExceptionData(IList<Event> exceptionalEvents, IList<string> deletedOccurrences, bool isMeeting, ExDateTime masterStart, UserSmtpAddress userSmtpAddress)
		{
			if (exceptionalEvents == null && deletedOccurrences == null)
			{
				return null;
			}
			List<Microsoft.Exchange.Connections.Eas.Model.Request.Calendar.Exception> list = new List<Microsoft.Exchange.Connections.Eas.Model.Request.Calendar.Exception>();
			if (exceptionalEvents != null)
			{
				foreach (Event exceptionalEvent in exceptionalEvents)
				{
					list.Add(SyncCalendarUtils.GetExceptionData(exceptionalEvent, isMeeting, masterStart, userSmtpAddress));
				}
			}
			if (deletedOccurrences != null)
			{
				foreach (string deletedOccurrenceId in deletedOccurrences)
				{
					list.Add(SyncCalendarUtils.GetDeletedExceptionData(deletedOccurrenceId, masterStart));
				}
			}
			return list;
		}

		public static void CopyCommonEventData(ICalendarData calendarData, Event theEvent, bool isMeeting, UserSmtpAddress userSmtpAddress)
		{
			EventSchema schema = theEvent.Schema;
			int num = (theEvent.PopupReminderSettings != null && theEvent.PopupReminderSettings.Count > 0) ? theEvent.PopupReminderSettings[0].ReminderMinutesBeforeStart : 0;
			if (theEvent.IsPropertySet(schema.BodyProperty) && theEvent.Body != null && theEvent.Body.Content != null)
			{
				calendarData.Body = new Body
				{
					Data = theEvent.Body.Content,
					Type = new byte?((byte)SyncCalendarUtils.GetEasBodyType(theEvent.Body.ContentType))
				};
			}
			if (theEvent.IsPropertySet(schema.StartProperty))
			{
				calendarData.StartTime = SyncCalendarUtils.ToStringDateTime(theEvent.Start);
			}
			if (theEvent.IsPropertySet(schema.EndProperty))
			{
				calendarData.EndTime = SyncCalendarUtils.ToStringDateTime(theEvent.End);
			}
			if (theEvent.IsPropertySet(schema.SubjectProperty))
			{
				calendarData.CalendarSubject = theEvent.Subject;
			}
			if (theEvent.IsPropertySet(schema.LocationProperty))
			{
				calendarData.Location = ((theEvent.Location != null) ? ((!string.IsNullOrEmpty(theEvent.Location.DisplayName)) ? theEvent.Location.DisplayName : null) : null);
			}
			if (theEvent.IsPropertySet(schema.PopupReminderSettingsProperty))
			{
				calendarData.Reminder = ((num > 0) ? new uint?((uint)num) : null);
			}
			if (theEvent.IsPropertySet(schema.IsAllDayProperty))
			{
				calendarData.AllDayEvent = new byte?(Convert.ToByte(theEvent.IsAllDay));
			}
			if (theEvent.IsPropertySet(schema.ShowAsProperty))
			{
				EasBusyStatus? busyStatus = SyncCalendarUtils.GetBusyStatus(theEvent.ShowAs);
				calendarData.BusyStatus = ((busyStatus != null) ? new byte?((byte)busyStatus.GetValueOrDefault()) : null);
			}
			if (theEvent.IsPropertySet(schema.SensitivityProperty))
			{
				calendarData.Sensitivity = new byte?((byte)theEvent.Sensitivity);
			}
			if (theEvent.IsPropertySet(schema.LastModifiedTimeProperty))
			{
				calendarData.DtStamp = SyncCalendarUtils.ToStringDateTime(theEvent.LastModifiedTime);
			}
			if (theEvent.IsPropertySet(schema.CategoriesProperty))
			{
				calendarData.CalendarCategories = SyncCalendarUtils.GetCategories(theEvent.Categories);
			}
			if (isMeeting && theEvent.IsPropertySet(schema.AttendeesProperty))
			{
				calendarData.Attendees = SyncCalendarUtils.GetAttendees(theEvent.Attendees, userSmtpAddress, theEvent.ResponseStatus);
			}
		}

		private static Microsoft.Exchange.Connections.Eas.Model.Common.AirSyncBase.BodyType GetEasBodyType(Microsoft.Exchange.Entities.DataModel.Items.BodyType contentType)
		{
			switch (contentType)
			{
			case Microsoft.Exchange.Entities.DataModel.Items.BodyType.Text:
				return Microsoft.Exchange.Connections.Eas.Model.Common.AirSyncBase.BodyType.PlainText;
			case Microsoft.Exchange.Entities.DataModel.Items.BodyType.Html:
				return Microsoft.Exchange.Connections.Eas.Model.Common.AirSyncBase.BodyType.HTML;
			default:
				throw new EasSyncFailedPermanentException("Invalid contentType value : " + contentType);
			}
		}

		private static EasBusyStatus? GetBusyStatus(FreeBusyStatus showAs)
		{
			switch (showAs)
			{
			case FreeBusyStatus.Unknown:
			case FreeBusyStatus.WorkingElsewhere:
				return null;
			case FreeBusyStatus.Free:
				return new EasBusyStatus?(EasBusyStatus.Free);
			case FreeBusyStatus.Tentative:
				return new EasBusyStatus?(EasBusyStatus.Tentative);
			case FreeBusyStatus.Busy:
				return new EasBusyStatus?(EasBusyStatus.Busy);
			case FreeBusyStatus.Oof:
				return new EasBusyStatus?(EasBusyStatus.OutOfOffice);
			default:
				throw new EasSyncFailedPermanentException("Invalid showAs status : " + showAs);
			}
		}

		private static List<Category> GetCategories(List<string> edmCategories)
		{
			List<Category> list = null;
			if (edmCategories != null && edmCategories.Count > 0)
			{
				list = new List<Category>();
				foreach (string name in edmCategories)
				{
					list.Add(new Category
					{
						Name = name
					});
				}
			}
			return list;
		}

		private static List<Microsoft.Exchange.Connections.Eas.Model.Request.Calendar.Attendee> GetAttendees(IList<Microsoft.Exchange.Entities.DataModel.Calendaring.Attendee> edmAttendees, UserSmtpAddress userSmtpAddress, ResponseStatus status)
		{
			List<Microsoft.Exchange.Connections.Eas.Model.Request.Calendar.Attendee> list = null;
			if (edmAttendees != null && edmAttendees.Count > 0)
			{
				list = new List<Microsoft.Exchange.Connections.Eas.Model.Request.Calendar.Attendee>();
				foreach (Microsoft.Exchange.Entities.DataModel.Calendaring.Attendee attendee in edmAttendees)
				{
					Microsoft.Exchange.Connections.Eas.Model.Request.Calendar.Attendee attendee2 = new Microsoft.Exchange.Connections.Eas.Model.Request.Calendar.Attendee
					{
						AttendeeType = new byte?((byte)attendee.Type),
						Email = attendee.EmailAddress,
						Name = attendee.Name
					};
					if (attendee.Status != null)
					{
						attendee2.AttendeeStatus = new byte?((byte)attendee.Status.Response);
					}
					else if (status != null && userSmtpAddress.Address.Equals(attendee.EmailAddress))
					{
						attendee2.AttendeeStatus = new byte?((byte)status.Response);
					}
					list.Add(attendee2);
				}
			}
			return list;
		}

		private static Microsoft.Exchange.Connections.Eas.Model.Request.Calendar.Exception GetExceptionData(Event exceptionalEvent, bool isMeeting, ExDateTime masterStart, UserSmtpAddress userSmtpAddress)
		{
			Microsoft.Exchange.Connections.Eas.Model.Request.Calendar.Exception ex = new Microsoft.Exchange.Connections.Eas.Model.Request.Calendar.Exception();
			ex.ExceptionStartTime = SyncCalendarUtils.GetExceptionStartDate(exceptionalEvent.Id, masterStart);
			SyncCalendarUtils.CopyCommonEventData(ex, exceptionalEvent, isMeeting, userSmtpAddress);
			return ex;
		}

		private static Microsoft.Exchange.Connections.Eas.Model.Request.Calendar.Exception GetDeletedExceptionData(string deletedOccurrenceId, ExDateTime masterStart)
		{
			return new Microsoft.Exchange.Connections.Eas.Model.Request.Calendar.Exception
			{
				ExceptionStartTime = SyncCalendarUtils.GetExceptionStartDate(deletedOccurrenceId, masterStart),
				Deleted = new byte?(1)
			};
		}

		private static string GetExceptionStartDate(string exceptionId, ExDateTime masterStart)
		{
			StoreObjectId storeObjectId = IdConverter.Instance.ToStoreObjectId(exceptionId);
			OccurrenceStoreObjectId occurrenceStoreObjectId = storeObjectId as OccurrenceStoreObjectId;
			if (occurrenceStoreObjectId == null)
			{
				throw new EasSyncFailedPermanentException("Exception id is not an occurrence id: " + exceptionId);
			}
			return SyncCalendarUtils.ToStringDateTime(occurrenceStoreObjectId.OccurrenceId.Add(masterStart.UniversalTime.TimeOfDay));
		}

		private static RecurrencePattern CreateRecurrencePattern(Microsoft.Exchange.Connections.Eas.Model.Response.Calendar.Recurrence easRecurrence)
		{
			try
			{
				EasRecurrenceType type = (EasRecurrenceType)easRecurrence.Type;
				switch (type)
				{
				case EasRecurrenceType.Daily:
					if (easRecurrence.DayOfWeek != 0)
					{
						return new WeeklyRecurrencePattern((DaysOfWeek)easRecurrence.DayOfWeek);
					}
					return new DailyRecurrencePattern(easRecurrence.Interval);
				case EasRecurrenceType.Weekly:
					return new WeeklyRecurrencePattern((DaysOfWeek)easRecurrence.DayOfWeek, easRecurrence.Interval);
				case EasRecurrenceType.Monthly:
					return new MonthlyRecurrencePattern(easRecurrence.DayOfMonth, easRecurrence.Interval);
				case EasRecurrenceType.MonthlyTh:
				{
					RecurrenceOrderType order = SyncCalendarUtils.RecurrenceOrderTypeFromWeekOfMonth(easRecurrence.WeekOfMonth);
					return new MonthlyThRecurrencePattern((DaysOfWeek)easRecurrence.DayOfWeek, order, easRecurrence.Interval);
				}
				case EasRecurrenceType.Yearly:
					return new YearlyRecurrencePattern(easRecurrence.DayOfMonth, easRecurrence.MonthOfYear);
				case EasRecurrenceType.YearlyTh:
				{
					RecurrenceOrderType order2 = SyncCalendarUtils.RecurrenceOrderTypeFromWeekOfMonth(easRecurrence.WeekOfMonth);
					return new YearlyThRecurrencePattern((DaysOfWeek)easRecurrence.DayOfWeek, order2, easRecurrence.MonthOfYear);
				}
				}
				throw new EasFetchFailedPermanentException("Invalid recurrence type: " + type);
			}
			catch (ArgumentOutOfRangeException innerException)
			{
				throw new EasFetchFailedPermanentException("Invalid recurrence", innerException);
			}
			RecurrencePattern result;
			return result;
		}

		private static RecurrenceOrderType RecurrenceOrderTypeFromWeekOfMonth(int weekOfMonth)
		{
			if (weekOfMonth != 5)
			{
				return (RecurrenceOrderType)weekOfMonth;
			}
			return RecurrenceOrderType.Last;
		}

		private static RecurrenceRange CreateRecurrenceRange(ExDateTime start, Microsoft.Exchange.Connections.Eas.Model.Response.Calendar.Recurrence easRecurrence)
		{
			if (easRecurrence.Occurrences != 0)
			{
				return new NumberedRecurrenceRange(start, easRecurrence.Occurrences);
			}
			if (!string.IsNullOrEmpty(easRecurrence.Until))
			{
				ExDateTime endDate = SyncCalendarUtils.ToExDateTime(easRecurrence.Until);
				return new EndDateRecurrenceRange(start, endDate);
			}
			return new NoEndRecurrenceRange(start);
		}

		private static ushort GetDayOfWeekValue(ISet<DayOfWeek> daysOfWeek)
		{
			ushort num = 0;
			foreach (DayOfWeek day in daysOfWeek)
			{
				num += SyncCalendarUtils.GetEasDayOfWeekValue(day);
			}
			return num;
		}

		private static ushort GetEasDayOfWeekValue(DayOfWeek day)
		{
			return (ushort)(1 << (int)day);
		}

		private const string DateTimeFormat = "yyyyMMdd\\THHmmss\\Z";

		private const int CharArrrayLength = 32;

		private const int TimeZoneInformationStructSize = 172;

		private const int EasLastWeekOfAMonth = 5;

		private const int BasePropId = 32768;

		private static readonly PropertyTag GlobalObjectIdPropertyTag = new PropertyTag((uint)PropTagHelper.PropTagFromIdAndType(32768, PropType.Binary));

		private static readonly PropertyTag StartPropertyTag = new PropertyTag((uint)PropTagHelper.PropTagFromIdAndType(32769, PropType.SysTime));

		private static readonly PropertyTag EndPropertyTag = new PropertyTag((uint)PropTagHelper.PropTagFromIdAndType(32770, PropType.SysTime));

		private static readonly PropertyTag AllDayEventPropertyTag = new PropertyTag((uint)PropTagHelper.PropTagFromIdAndType(32771, PropType.Boolean));

		private static readonly PropertyTag BusyStatusPropertyTag = new PropertyTag((uint)PropTagHelper.PropTagFromIdAndType(32772, PropType.Int));

		private static readonly PropertyTag LocationPropertyTag = new PropertyTag((uint)PropTagHelper.PropTagFromIdAndType(32773, PropType.AnsiString));

		private static readonly PropertyTag MeetingStatusPropertyTag = new PropertyTag((uint)PropTagHelper.PropTagFromIdAndType(32774, PropType.Int));

		private static readonly PropertyTag ReminderPropertyTag = new PropertyTag((uint)PropTagHelper.PropTagFromIdAndType(32775, PropType.Int));

		private static readonly PropertyTag TimeZoneBlobPropertyTag = new PropertyTag((uint)PropTagHelper.PropTagFromIdAndType(32776, PropType.Binary));

		private static readonly PropertyTag AppointmentRecurrenceBlobPropertyTag = new PropertyTag((uint)PropTagHelper.PropTagFromIdAndType(32777, PropType.Binary));

		private static readonly PropertyTag ResponseTypePropertyTag = new PropertyTag((uint)PropTagHelper.PropTagFromIdAndType(32778, PropType.Int));

		private static readonly PropertyTag RecipientTrackStatusPropertyTag = new PropertyTag(1610547203U);

		private static readonly PropertyTag RecipientTypePropertyTag = new PropertyTag(202702851U);

		private static readonly PropertyTag EmailAddressPropertyTag = new PropertyTag(805503007U);

		private static readonly PropertyTag DisplayNamePropertyTag = new PropertyTag(805371935U);

		private static readonly PropertyTag RowIdPropertyTag = new PropertyTag(805306371U);

		private static readonly PropertyTag SensitivityPropertyTag = new PropertyTag(3538947U);

		private static readonly PropertyTag SentRepresentingNamePropertyTag = new PropertyTag(4325407U);

		private static readonly PropertyTag SentRepresentingEmailAddressPropertyTag = new PropertyTag(6619167U);

		public static readonly Dictionary<PropertyTag, NamedProperty> CalendarItemPropertyTagsToNamedProperties = new Dictionary<PropertyTag, NamedProperty>
		{
			{
				SyncCalendarUtils.GlobalObjectId,
				new NamedProperty(WellKnownPropertySet.Meeting, 3U)
			},
			{
				SyncCalendarUtils.Start,
				new NamedProperty(WellKnownPropertySet.Appointment, 33293U)
			},
			{
				SyncCalendarUtils.End,
				new NamedProperty(WellKnownPropertySet.Appointment, 33294U)
			},
			{
				SyncCalendarUtils.AllDayEvent,
				new NamedProperty(WellKnownPropertySet.Appointment, 33301U)
			},
			{
				SyncCalendarUtils.BusyStatus,
				new NamedProperty(WellKnownPropertySet.Appointment, 33285U)
			},
			{
				SyncCalendarUtils.Location,
				new NamedProperty(WellKnownPropertySet.Appointment, 33288U)
			},
			{
				SyncCalendarUtils.Reminder,
				new NamedProperty(WellKnownPropertySet.Common, 34049U)
			},
			{
				SyncCalendarUtils.TimeZoneBlob,
				new NamedProperty(WellKnownPropertySet.Appointment, 33331U)
			},
			{
				SyncCalendarUtils.MeetingStatus,
				new NamedProperty(WellKnownPropertySet.Appointment, 33303U)
			},
			{
				SyncCalendarUtils.AppointmentRecurrenceBlob,
				new NamedProperty(WellKnownPropertySet.Appointment, 33302U)
			},
			{
				SyncCalendarUtils.ResponseType,
				new NamedProperty(WellKnownPropertySet.Appointment, 33304U)
			}
		};

		public static readonly Dictionary<PropertyTag, NamedProperty> AttendeePropertyTagsToNamedProperties = new Dictionary<PropertyTag, NamedProperty>(0);
	}
}
