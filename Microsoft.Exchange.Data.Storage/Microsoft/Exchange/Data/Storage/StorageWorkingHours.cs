using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class StorageWorkingHours
	{
		public static StorageWorkingHours LoadFrom(MailboxSession session, StoreId folderId)
		{
			WorkHoursInCalendar fromCalendar = WorkHoursInCalendar.GetFromCalendar(session, folderId);
			if (fromCalendar == null || fromCalendar.WorkHoursVersion1 == null)
			{
				return null;
			}
			if (fromCalendar.WorkHoursVersion1.TimeSlot == null)
			{
				throw new WorkingHoursXmlMalformedException(ServerStrings.NullWorkHours);
			}
			ExTimeZone exTimeZone = null;
			try
			{
				if (!string.IsNullOrEmpty(fromCalendar.WorkHoursVersion1.TimeZone.Name))
				{
					if (ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(fromCalendar.WorkHoursVersion1.TimeZone.Name, out exTimeZone))
					{
						WorkHoursTimeZone workHoursTimeZone = fromCalendar.WorkHoursVersion1.TimeZone;
						if (!workHoursTimeZone.IsSameTimeZoneInfo(TimeZoneHelper.RegTimeZoneInfoFromExTimeZone(exTimeZone)))
						{
							exTimeZone = null;
						}
					}
					else
					{
						exTimeZone = null;
					}
				}
				if (exTimeZone == null)
				{
					exTimeZone = TimeZoneHelper.CreateCustomExTimeZoneFromRegTimeZoneInfo(fromCalendar.WorkHoursVersion1.TimeZone.TimeZoneInfo, "tzone://Microsoft/Custom", "Customized Time Zone");
				}
			}
			catch (InvalidTimeZoneException ex)
			{
				throw new WorkingHoursXmlMalformedException(ServerStrings.MalformedTimeZoneWorkingHours(session.MailboxOwner.MailboxInfo.DisplayName, ex.ToString()), ex);
			}
			return new StorageWorkingHours(exTimeZone, fromCalendar.WorkHoursVersion1.WorkDays, fromCalendar.WorkHoursVersion1.TimeSlot.StartTimeInMinutes, fromCalendar.WorkHoursVersion1.TimeSlot.EndTimeInMinutes);
		}

		public static StorageWorkingHours Create(ExTimeZone timeZone)
		{
			return new StorageWorkingHours(timeZone, DaysOfWeek.Weekdays, 480, 1020);
		}

		public static StorageWorkingHours Create(ExTimeZone timeZone, int daysOfWeek, int startTimeInMinutes, int endTimeInMinutes)
		{
			EnumValidator.IsValidValue<DaysOfWeek>((DaysOfWeek)daysOfWeek);
			return new StorageWorkingHours(timeZone, (DaysOfWeek)daysOfWeek, startTimeInMinutes, endTimeInMinutes);
		}

		public static bool RemoveWorkingHoursFrom(MailboxSession session, StoreId folderId)
		{
			return WorkHoursInCalendar.DeleteFromCalendar(session, folderId);
		}

		public int StartTimeInMinutes
		{
			get
			{
				return this.startTimeInMinutes;
			}
		}

		public DaysOfWeek DaysOfWeek
		{
			get
			{
				return this.daysOfWeek;
			}
		}

		public void UpdateWorkingPeriod(DaysOfWeek daysOfWeek, int startTimeInMinutes, int endTimeInMinutes)
		{
			EnumValidator.ThrowIfInvalid<DaysOfWeek>(daysOfWeek, "daysOfWeek");
			this.startTimeInMinutes = startTimeInMinutes;
			this.endTimeInMinutes = endTimeInMinutes;
			this.daysOfWeek = daysOfWeek;
		}

		public int EndTimeInMinutes
		{
			get
			{
				return this.endTimeInMinutes;
			}
		}

		public void SaveTo(MailboxSession session, StoreId folderId)
		{
			WorkHoursInCalendar workHoursInCalendar = WorkHoursInCalendar.Create(this.timeZone, (int)this.daysOfWeek, this.startTimeInMinutes, this.endTimeInMinutes);
			workHoursInCalendar.SaveToCalendar(session, folderId);
		}

		private StorageWorkingHours(ExTimeZone timeZone, DaysOfWeek daysOfWeek, int startTimeInMinutes, int endTimeInMinutes)
		{
			if (timeZone == null)
			{
				throw new ArgumentException("timeZone");
			}
			this.TimeZone = timeZone;
			this.daysOfWeek = daysOfWeek;
			this.startTimeInMinutes = startTimeInMinutes;
			this.endTimeInMinutes = endTimeInMinutes;
		}

		public ExTimeZone TimeZone
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

		public bool IsWorkingDay(DayOfWeek dayOfWeek)
		{
			return StorageWorkingHours.IsBitOn((int)StorageWorkingHours.ToDaysOfWeek(dayOfWeek), (int)this.daysOfWeek);
		}

		public bool IsInWorkingHours(ExDateTime dateTime)
		{
			TimeSpan timeOfDay = this.TimeZone.ConvertDateTime(dateTime).TimeOfDay;
			return this.IsWorkingDay(dateTime.DayOfWeek) && timeOfDay.TotalMinutes >= (double)this.StartTimeInMinutes && timeOfDay.TotalMinutes <= (double)this.EndTimeInMinutes;
		}

		public override string ToString()
		{
			return string.Format("TimeZone = {0}; WorkingPeriod: {1} {2} {3}", new object[]
			{
				this.timeZone,
				this.daysOfWeek,
				this.startTimeInMinutes,
				this.endTimeInMinutes
			});
		}

		internal static bool IsBitOn(int val, int mask)
		{
			return (val & mask) == val;
		}

		internal static DaysOfWeek ToDaysOfWeek(DayOfWeek dayOfWeek)
		{
			switch (dayOfWeek)
			{
			case DayOfWeek.Sunday:
				return DaysOfWeek.Sunday;
			case DayOfWeek.Monday:
				return DaysOfWeek.Monday;
			case DayOfWeek.Tuesday:
				return DaysOfWeek.Tuesday;
			case DayOfWeek.Wednesday:
				return DaysOfWeek.Wednesday;
			case DayOfWeek.Thursday:
				return DaysOfWeek.Thursday;
			case DayOfWeek.Friday:
				return DaysOfWeek.Friday;
			case DayOfWeek.Saturday:
				return DaysOfWeek.Saturday;
			default:
				return DaysOfWeek.None;
			}
		}

		private ExTimeZone timeZone;

		private DaysOfWeek daysOfWeek;

		private int startTimeInMinutes;

		private int endTimeInMinutes;
	}
}
