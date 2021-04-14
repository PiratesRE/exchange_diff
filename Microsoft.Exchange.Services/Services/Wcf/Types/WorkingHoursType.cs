using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class WorkingHoursType
	{
		internal WorkingHoursType(int startTime, int endTime, int workDays, ExTimeZone displayTimeZone, ExTimeZone originalTimeZone)
		{
			this.displayTimeZone = displayTimeZone;
			this.originalTimeZone = originalTimeZone;
			this.WorkHoursStartTimeInMinutes = this.ConvertToDisplayTimeZone(startTime, ExDateTime.Now);
			this.WorkHoursEndTimeInMinutes = this.ConvertToDisplayTimeZone(endTime, ExDateTime.Now);
			this.WorkDays = workDays;
		}

		[DataMember]
		public int WorkHoursStartTimeInMinutes
		{
			get
			{
				return this.workHoursStartTimeInMinutes;
			}
			private set
			{
				this.workHoursStartTimeInMinutes = value;
			}
		}

		[DataMember]
		public int WorkHoursEndTimeInMinutes
		{
			get
			{
				return this.workHoursEndTimeInMinutes;
			}
			private set
			{
				this.workHoursEndTimeInMinutes = value;
			}
		}

		[DataMember]
		public int WorkDays
		{
			get
			{
				return this.workDays;
			}
			private set
			{
				this.workDays = value;
			}
		}

		public ExTimeZone WorkingHoursTimeZone
		{
			get
			{
				return this.originalTimeZone;
			}
		}

		public bool IsTimeZoneDifferent
		{
			get
			{
				return this.displayTimeZone != this.originalTimeZone && TimeZoneHelper.RegTimeZoneInfoFromExTimeZone(this.displayTimeZone) != TimeZoneHelper.RegTimeZoneInfoFromExTimeZone(this.originalTimeZone);
			}
		}

		internal static WorkingHoursType Load(MailboxSession mailboxSession, string displayTimeZoneKey)
		{
			if (string.IsNullOrEmpty(displayTimeZoneKey))
			{
				throw new ArgumentException("mailboxTimeZoneKey");
			}
			ExTimeZone exTimeZone = null;
			if (!ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(displayTimeZoneKey, out exTimeZone))
			{
				ExTraceGlobals.UserOptionsTracer.TraceError<string>(0L, "Failed to resolve target time zone in a call to WorkingHoursType.Load. {0}. - Will revert to default working hours.", displayTimeZoneKey);
				return WorkingHoursType.GetDefaultWorkingHoursInTimeZone(ExTimeZone.UtcTimeZone);
			}
			StorageWorkingHours storageWorkingHours;
			WorkingHoursType.LoadResult loadResult = WorkingHoursType.LoadInternal(mailboxSession, out storageWorkingHours);
			WorkingHoursType workingHoursType;
			if (loadResult != WorkingHoursType.LoadResult.Success)
			{
				ExTraceGlobals.UserOptionsTracer.TraceDebug<IExchangePrincipal>(0L, "Could not retrieve working hours - returning defaults instead.User {0}", mailboxSession.MailboxOwner);
				workingHoursType = WorkingHoursType.GetDefaultWorkingHoursInTimeZone(exTimeZone);
				if (loadResult == WorkingHoursType.LoadResult.Corrupt)
				{
					WorkingHoursType.DeleteWorkingHoursMessage(mailboxSession);
				}
				if (loadResult == WorkingHoursType.LoadResult.Missing || loadResult == WorkingHoursType.LoadResult.Corrupt)
				{
					ExTraceGlobals.UserOptionsTracer.TraceDebug<IExchangePrincipal>(0L, "Working hours are missing or corrupted. Performing recovery. User:{0}", mailboxSession.MailboxOwner);
					workingHoursType.CommitChanges(mailboxSession);
				}
			}
			else
			{
				workingHoursType = new WorkingHoursType(storageWorkingHours.StartTimeInMinutes, storageWorkingHours.EndTimeInMinutes, (int)storageWorkingHours.DaysOfWeek, exTimeZone, storageWorkingHours.TimeZone ?? exTimeZone);
			}
			return workingHoursType;
		}

		internal static WorkingHoursType GetDefaultWorkingHoursInTimeZone(ExTimeZone timeZone)
		{
			return new WorkingHoursType(480, 1020, 62, timeZone, timeZone);
		}

		internal static void MoveWorkingHoursToTimeZone(MailboxSession mailboxSession, ExTimeZone newTimeZone)
		{
			StorageWorkingHours storageWorkingHours = null;
			if (WorkingHoursType.LoadInternal(mailboxSession, out storageWorkingHours) == WorkingHoursType.LoadResult.Success)
			{
				storageWorkingHours.TimeZone = newTimeZone;
				storageWorkingHours.SaveTo(mailboxSession, mailboxSession.GetDefaultFolderId(DefaultFolderType.Calendar));
			}
		}

		private static void DeleteWorkingHoursMessage(MailboxSession mailboxSession)
		{
			try
			{
				ExTraceGlobals.UserOptionsTracer.TraceDebug<IExchangePrincipal>(0L, "Working hours are corrupted. Deleting old message before saving new working hours. User:{0}", mailboxSession.MailboxOwner);
				StorageWorkingHours.RemoveWorkingHoursFrom(mailboxSession, mailboxSession.GetDefaultFolderId(DefaultFolderType.Calendar));
			}
			catch (StorageTransientException arg)
			{
				ExTraceGlobals.UserOptionsTracer.TraceError<IExchangePrincipal, StorageTransientException>(0L, "Issues while trying to delete working hours message. StorageTransient. User:{0} Exception:{1}", mailboxSession.MailboxOwner, arg);
			}
			catch (StoragePermanentException arg2)
			{
				ExTraceGlobals.UserOptionsTracer.TraceError<IExchangePrincipal, StoragePermanentException>(0L, "Issues while trying to delete working hours message. StoragePermanent. User:{0} Exception:{1}", mailboxSession.MailboxOwner, arg2);
			}
		}

		private static WorkingHoursType.LoadResult LoadInternal(MailboxSession mailboxSession, out StorageWorkingHours workingHours)
		{
			workingHours = null;
			try
			{
				workingHours = StorageWorkingHours.LoadFrom(mailboxSession, mailboxSession.GetDefaultFolderId(DefaultFolderType.Calendar));
			}
			catch (AccessDeniedException ex)
			{
				ExTraceGlobals.UserOptionsTracer.TraceError<string>(0L, "AccessDenied in a call of loading working hours : {0}", ex.Message);
				return WorkingHoursType.LoadResult.AccessDenied;
			}
			catch (ArgumentNullException ex2)
			{
				ExTraceGlobals.UserOptionsTracer.TraceError<string>(0L, "Argument exception in a call of loading working hours : {0}", ex2.Message);
				return WorkingHoursType.LoadResult.AccessDenied;
			}
			catch (ObjectNotFoundException ex3)
			{
				ExTraceGlobals.UserOptionsTracer.TraceError<string>(0L, "ObjectNotFoundException exception in a call of loading working hours : {0}", ex3.Message);
				return WorkingHoursType.LoadResult.AccessDenied;
			}
			catch (WorkingHoursXmlMalformedException ex4)
			{
				ExTraceGlobals.UserOptionsTracer.TraceError<string>(0L, "WorkingHoursXmlMalformedException exception in a call of loading working hours : {0}", ex4.Message);
				return WorkingHoursType.LoadResult.Corrupt;
			}
			catch (CorruptDataException ex5)
			{
				ExTraceGlobals.UserOptionsTracer.TraceError<string>(0L, "CorruptDataException exception in a call of loading working hours : {0}", ex5.Message);
				return WorkingHoursType.LoadResult.Corrupt;
			}
			if (workingHours == null)
			{
				return WorkingHoursType.LoadResult.Missing;
			}
			if (!WorkingHoursType.ValidateWorkingHours(workingHours.StartTimeInMinutes, workingHours.EndTimeInMinutes))
			{
				return WorkingHoursType.LoadResult.Corrupt;
			}
			return WorkingHoursType.LoadResult.Success;
		}

		private static bool ValidateWorkingHours(int startTime, int endTime)
		{
			return startTime >= 0 && startTime <= 1439 && endTime >= 1 && endTime <= 1440;
		}

		private static ExDateTime GetDateTimeFromMinutes(int minutes, ExDateTime date)
		{
			return date.Date.AddMinutes((double)minutes);
		}

		private static int GetMinutesFromDateTime(DateTime time, DateTime date)
		{
			return (int)(time - date.Date).TotalMinutes;
		}

		private int ConvertToDisplayTimeZone(int minutesPastMidnight, ExDateTime day)
		{
			if (!this.IsTimeZoneDifferent)
			{
				return minutesPastMidnight;
			}
			ExDateTime dateTimeFromMinutes = WorkingHoursType.GetDateTimeFromMinutes(minutesPastMidnight, day.Date);
			dateTimeFromMinutes = new ExDateTime(this.originalTimeZone, (DateTime)dateTimeFromMinutes);
			ExDateTime exDateTime = this.displayTimeZone.ConvertDateTime(dateTimeFromMinutes);
			return WorkingHoursType.GetMinutesFromDateTime((DateTime)exDateTime, (DateTime)day.Date);
		}

		private bool CommitChanges(MailboxSession mailboxSession)
		{
			StorageWorkingHours storageWorkingHours = StorageWorkingHours.Create(this.displayTimeZone, this.WorkDays, this.workHoursStartTimeInMinutes, this.WorkHoursEndTimeInMinutes);
			try
			{
				StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Calendar);
				storageWorkingHours.SaveTo(mailboxSession, defaultFolderId);
			}
			catch (WorkingHoursSaveFailedException ex)
			{
				ExTraceGlobals.UserOptionsTracer.TraceError<string>(0L, "Failed to save working hours in WorkingHour.Commit call: {0}", ex.Message);
				return false;
			}
			return true;
		}

		private const int FirstTimeDefaultWorkDayStartTime = 480;

		private const int FirstTimeDefaultWorkDayEndTime = 1020;

		private const int FirstTimeDefaultWorkDays = 62;

		private int workHoursStartTimeInMinutes;

		private int workHoursEndTimeInMinutes;

		private int workDays;

		private ExTimeZone displayTimeZone;

		private ExTimeZone originalTimeZone;

		private enum LoadResult
		{
			Success,
			Missing,
			AccessDenied,
			Corrupt
		}
	}
}
