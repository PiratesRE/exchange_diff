using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class WorkingHours
	{
		internal static WorkingHours CreateForSession(MailboxSession mailBoxSession, ExTimeZone timeZone)
		{
			if (mailBoxSession == null)
			{
				throw new ArgumentNullException("mailBoxSession");
			}
			if (timeZone == null)
			{
				throw new ArgumentNullException("timeZone");
			}
			WorkingHours result;
			if (WorkingHours.LoadFromMailbox(mailBoxSession, out result) != WorkingHours.LoadResult.Success)
			{
				result = new WorkingHours(0, 1440, 127, timeZone);
			}
			return result;
		}

		public static WorkingHours CreateFromAvailabilityWorkingHours(ISessionContext sessionContext, WorkingHours availabilityWorkingHours)
		{
			if (availabilityWorkingHours != null)
			{
				return new WorkingHours(availabilityWorkingHours.StartTimeInMinutes, availabilityWorkingHours.EndTimeInMinutes, (int)availabilityWorkingHours.DaysOfWeek, availabilityWorkingHours.ExTimeZone);
			}
			return new WorkingHours(0, 1440, 127, sessionContext.TimeZone);
		}

		internal static bool StampOnMailboxIfMissing(MailboxSession mailboxSession, string timeZoneKeyName)
		{
			WorkingHours workingHours;
			if (WorkingHours.LoadFromMailbox(mailboxSession, out workingHours) != WorkingHours.LoadResult.Missing)
			{
				return true;
			}
			ExTimeZone exTimeZone;
			if (!ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(timeZoneKeyName, out exTimeZone))
			{
				return false;
			}
			workingHours = new WorkingHours(480, 1020, 62, exTimeZone);
			return workingHours.CommitChanges(mailboxSession);
		}

		private static ExDateTime GetDateTimeFromMinutes(int minutes, ExDateTime date)
		{
			return date.Date.AddMinutes((double)minutes);
		}

		private static int GetMinutesFromDateTime(ExDateTime time, ExDateTime date)
		{
			return (int)(time - date.Date).TotalMinutes;
		}

		private static WorkingHours.LoadResult LoadFromMailbox(MailboxSession mailboxSession, out WorkingHours workingHours)
		{
			StorageWorkingHours storageWorkingHours = null;
			try
			{
				storageWorkingHours = StorageWorkingHours.LoadFrom(mailboxSession, mailboxSession.GetDefaultFolderId(DefaultFolderType.Calendar));
			}
			catch (AccessDeniedException)
			{
				workingHours = null;
				return WorkingHours.LoadResult.AccessDenied;
			}
			catch (ArgumentNullException)
			{
				workingHours = null;
				return WorkingHours.LoadResult.AccessDenied;
			}
			catch (ObjectNotFoundException)
			{
				workingHours = null;
				return WorkingHours.LoadResult.AccessDenied;
			}
			catch (WorkingHoursXmlMalformedException)
			{
				workingHours = null;
				return WorkingHours.LoadResult.Corrupt;
			}
			if (storageWorkingHours == null)
			{
				workingHours = null;
				return WorkingHours.LoadResult.Missing;
			}
			workingHours = new WorkingHours(storageWorkingHours.StartTimeInMinutes, storageWorkingHours.EndTimeInMinutes, (int)storageWorkingHours.DaysOfWeek, storageWorkingHours.TimeZone);
			return WorkingHours.LoadResult.Success;
		}

		private WorkingHours(int startTime, int endTime, int workDays, ExTimeZone timeZone)
		{
			this.SetWorkDayTimesInWorkingHoursTimeZone(startTime, endTime);
			this.WorkDays = workDays;
			this.TimeZone = timeZone;
		}

		public int GetWorkDayStartTime(ExDateTime day)
		{
			return this.ConvertToUserTimeZone(this.workDayStartTimeInWorkingHoursTimeZone, day);
		}

		public int GetWorkDayEndTime(ExDateTime day)
		{
			return this.ConvertToUserTimeZone(this.workDayEndTimeInWorkingHoursTimeZone, day);
		}

		public int WorkDayStartTimeInWorkingHoursTimeZone
		{
			get
			{
				return this.workDayStartTimeInWorkingHoursTimeZone;
			}
		}

		public int WorkDayEndTimeInWorkingHoursTimeZone
		{
			get
			{
				return this.workDayEndTimeInWorkingHoursTimeZone;
			}
		}

		public void SetWorkDayTimesInWorkingHoursTimeZone(int startTime, int endTime)
		{
			int num = startTime;
			int num2 = endTime;
			if (startTime < 0 || startTime > 1439 || endTime < 1 || endTime > 1440 || startTime > endTime)
			{
				num = 0;
				num2 = 1440;
			}
			if (num != this.workDayStartTimeInWorkingHoursTimeZone || num2 != this.workDayEndTimeInWorkingHoursTimeZone)
			{
				this.workDayStartTimeInWorkingHoursTimeZone = num;
				this.workDayEndTimeInWorkingHoursTimeZone = num2;
			}
		}

		public int WorkDays
		{
			get
			{
				return this.workDays;
			}
			set
			{
				if (value < 0 || value > 127)
				{
					value = 127;
				}
				if (value != this.workDays)
				{
					this.workDays = value;
				}
			}
		}

		public bool IsWorkDay(DayOfWeek day)
		{
			return (this.WorkDays >> (int)day & 1) > 0;
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

		public bool IsTimeZoneDifferent
		{
			get
			{
				return TimeZoneHelper.RegTimeZoneInfoFromExTimeZone(OwaContext.Current.SessionContext.TimeZone) != TimeZoneHelper.RegTimeZoneInfoFromExTimeZone(this.timeZone);
			}
		}

		internal bool CommitChanges(MailboxSession mailboxSession)
		{
			StorageWorkingHours storageWorkingHours = StorageWorkingHours.Create(this.timeZone, this.workDays, this.workDayStartTimeInWorkingHoursTimeZone, this.workDayEndTimeInWorkingHoursTimeZone);
			try
			{
				StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Calendar);
				storageWorkingHours.SaveTo(mailboxSession, defaultFolderId);
			}
			catch (WorkingHoursSaveFailedException)
			{
				return false;
			}
			return true;
		}

		public WorkingHours.WorkingPeriod[] GetWorkingHoursOnDay(ExDateTime day)
		{
			List<WorkingHours.WorkingPeriod> list = new List<WorkingHours.WorkingPeriod>(2);
			this.AddWorkingPeriodForDay(list, day, -2);
			this.AddWorkingPeriodForDay(list, day, -1);
			this.AddWorkingPeriodForDay(list, day, 0);
			this.AddWorkingPeriodForDay(list, day, 1);
			this.AddWorkingPeriodForDay(list, day, 2);
			return list.ToArray();
		}

		private void AddWorkingPeriodForDay(List<WorkingHours.WorkingPeriod> workingPeriods, ExDateTime day, int offset)
		{
			ExDateTime day2 = day.IncrementDays(offset);
			if (!this.IsWorkDay(day2.DayOfWeek))
			{
				return;
			}
			int val = this.GetWorkDayStartTime(day2) + offset * 1440;
			int val2 = this.GetWorkDayEndTime(day2) + offset * 1440;
			WorkingHours.WorkingPeriod workingPeriod = new WorkingHours.WorkingPeriod();
			workingPeriod.Start = day.AddMinutes((double)Math.Max(0, Math.Min(1440, val)));
			workingPeriod.End = day.AddMinutes((double)Math.Max(0, Math.Min(1440, val2)));
			if (workingPeriod.Start < workingPeriod.End)
			{
				workingPeriods.Add(workingPeriod);
			}
		}

		private int ConvertToUserTimeZone(int minutesPastMidnight, ExDateTime day)
		{
			if (!this.IsTimeZoneDifferent)
			{
				return minutesPastMidnight;
			}
			ExDateTime exDateTime = WorkingHours.GetDateTimeFromMinutes(minutesPastMidnight, day.Date);
			exDateTime = new ExDateTime(this.timeZone, (DateTime)exDateTime);
			exDateTime = UserContextManager.GetUserContext().TimeZone.ConvertDateTime(exDateTime);
			return WorkingHours.GetMinutesFromDateTime(exDateTime, day.Date);
		}

		public string CreateHomeTimeZoneString()
		{
			return this.TimeZone.LocalizableDisplayName.ToString(Thread.CurrentThread.CurrentUICulture);
		}

		private const int MinutesPerDay = 1440;

		private const int FirstTimeDefaultWorkDayStartTime = 480;

		private const int FirstTimeDefaultWorkDayEndTime = 1020;

		private const int FirstTimeDefaultWorkDays = 62;

		private const int SessionDefaultWorkDayStartTime = 0;

		private const int SessionDefaultWorkDayEndTime = 1440;

		private const int SessionDefaultWorkDays = 127;

		private int workDayStartTimeInWorkingHoursTimeZone;

		private int workDayEndTimeInWorkingHoursTimeZone;

		private int workDays;

		private ExTimeZone timeZone;

		public class WorkingPeriod
		{
			public ExDateTime Start;

			public ExDateTime End;
		}

		private enum LoadResult
		{
			Success,
			Missing,
			AccessDenied,
			Corrupt
		}
	}
}
