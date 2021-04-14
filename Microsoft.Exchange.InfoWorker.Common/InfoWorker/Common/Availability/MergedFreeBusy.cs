using System;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal static class MergedFreeBusy
	{
		public static FreeBusyQueryResult MergeGroupMemberResults(ExTimeZone timeZone, int mergedFreeBusyInterval, ExDateTime startTime, ExDateTime endTime, FreeBusyQuery[] freeBusyQueryMembers, ExchangeVersionType requestSchemaVersion)
		{
			int num = MergedFreeBusy.NumberOfSlots(startTime, endTime, mergedFreeBusyInterval);
			char[] array = new char[num];
			char c = (requestSchemaVersion < ExchangeVersionType.Exchange2012) ? '4' : '5';
			for (int i = 0; i < num; i++)
			{
				array[i] = c;
			}
			foreach (FreeBusyQuery freeBusyQuery in freeBusyQueryMembers)
			{
				if (freeBusyQuery.Result != null)
				{
					string text = freeBusyQuery.Result.MergedFreeBusy;
					if (text == null)
					{
						text = MergedFreeBusy.BuildMergedFreeBusyString(timeZone, mergedFreeBusyInterval, startTime, endTime, num, freeBusyQuery.Result.CalendarEventArray, freeBusyQuery.Result.ExceptionInfo != null, null, requestSchemaVersion);
					}
					for (int k = 0; k < array.Length; k++)
					{
						if (MergedFreeBusy.MergedFreeBusyWeight(array[k], requestSchemaVersion) < MergedFreeBusy.MergedFreeBusyWeight(text[k], requestSchemaVersion))
						{
							array[k] = text[k];
						}
					}
				}
			}
			return new FreeBusyQueryResult(FreeBusyViewType.MergedOnly, null, new string(array), null);
		}

		public static string GenerateMergedFreeBusyString(ExTimeZone timeZone, int mergedFreeBusyInterval, ExDateTime windowStart, ExDateTime windowEnd, CalendarEvent[] calendarEventArray, bool exceptionOccurred, byte[] globalObjectId, ExchangeVersionType requestedSchemaVersion)
		{
			int numberOfFreeBusySlots = MergedFreeBusy.NumberOfSlots(windowStart, windowEnd, mergedFreeBusyInterval);
			return MergedFreeBusy.BuildMergedFreeBusyString(timeZone, mergedFreeBusyInterval, windowStart, windowEnd, numberOfFreeBusySlots, calendarEventArray, exceptionOccurred, globalObjectId, requestedSchemaVersion);
		}

		internal static string GenerateNoMergedFreeBusyString(Duration timeWindow, ExTimeZone timeZone, int mergedFreeBusyIntervalInMinutes)
		{
			int num = (int)(new ExDateTime(timeZone, timeWindow.EndTime) - new ExDateTime(timeZone, timeWindow.StartTime)).TotalMinutes;
			int num2 = num / mergedFreeBusyIntervalInMinutes;
			if (num % mergedFreeBusyIntervalInMinutes > 0)
			{
				num2++;
			}
			return new string('5', num2);
		}

		internal static string GetMergedFreeBusyString(EmailAddress emailAddress, CalendarFolder calendarFolder, ExTimeZone timeZone, ExDateTime windowStart, ExDateTime windowEnd)
		{
			CalendarEvent[] calendarEventArray = null;
			int mergedFreeBusyInterval = 5;
			bool exceptionOccurred = false;
			try
			{
				calendarEventArray = InternalCalendarQuery.GetCalendarEvents(emailAddress, calendarFolder, windowStart, windowEnd, FreeBusyViewType.FreeBusyMerged, true, ExchangeVersionType.Exchange2012);
			}
			catch (ResultSetTooBigException)
			{
				exceptionOccurred = true;
			}
			return MergedFreeBusy.GenerateMergedFreeBusyString(timeZone, mergedFreeBusyInterval, windowStart, windowEnd, calendarEventArray, exceptionOccurred, null, ExchangeVersionType.Exchange2012);
		}

		internal static string FromPublicFolderFreeBusyProperty(ExTimeZone timeZone, int mergedFreeBusyInterval, ExDateTime windowStart, ExDateTime windowEnd, PublicFolderFreeBusy freeBusyData, bool exceptionOccurred)
		{
			int num = MergedFreeBusy.NumberOfSlots(windowStart, windowEnd, mergedFreeBusyInterval);
			char[] array = new char[num];
			char c = exceptionOccurred ? '4' : '0';
			for (int i = 0; i < num; i++)
			{
				array[i] = c;
			}
			if (exceptionOccurred || freeBusyData == null || freeBusyData.Appointments == null || freeBusyData.Appointments.Count == 0)
			{
				return new string(array);
			}
			foreach (PublicFolderFreeBusyAppointment publicFolderFreeBusyAppointment in freeBusyData.Appointments)
			{
				MergedFreeBusy.AddAppointmentToFreeBusyString(windowStart, windowEnd, publicFolderFreeBusyAppointment.StartTime, publicFolderFreeBusyAppointment.EndTime, (BusyType)publicFolderFreeBusyAppointment.BusyType, mergedFreeBusyInterval, array, ExchangeVersionType.Exchange2007);
			}
			return new string(array);
		}

		private static void AddAppointmentToFreeBusyString(ExDateTime windowStart, ExDateTime windowEnd, ExDateTime startTime, ExDateTime endTime, BusyType busyType, int mergedFreeBusyInterval, char[] freeBusyValues, ExchangeVersionType requestedSchemaVersion)
		{
			TimeSpan timeSpan = startTime - windowStart;
			TimeSpan timeSpan2 = endTime - windowStart;
			int num = (int)timeSpan.TotalMinutes / mergedFreeBusyInterval;
			int num2 = (int)(timeSpan2.TotalMinutes / (double)mergedFreeBusyInterval) - 1;
			if (timeSpan2.TotalMinutes % (double)mergedFreeBusyInterval > 0.0)
			{
				num2++;
			}
			if (num < 0)
			{
				num = 0;
			}
			if (num2 >= freeBusyValues.Length)
			{
				num2 = freeBusyValues.Length - 1;
			}
			char c = (char)(busyType + 48);
			for (int i = num; i <= num2; i++)
			{
				if (MergedFreeBusy.MergedFreeBusyWeight(freeBusyValues[i], requestedSchemaVersion) < MergedFreeBusy.MergedFreeBusyWeight(c, requestedSchemaVersion))
				{
					freeBusyValues[i] = c;
				}
			}
		}

		private static int MergedFreeBusyWeight(char c, ExchangeVersionType requestSchemaVersion)
		{
			switch (c)
			{
			case '0':
				return 1;
			case '1':
				return 2;
			case '2':
				return 3;
			case '3':
				return 4;
			case '4':
				if (requestSchemaVersion < ExchangeVersionType.Exchange2012)
				{
					return 0;
				}
				return 1;
			default:
				return 0;
			}
		}

		private static int NumberOfSlots(ExDateTime windowStart, ExDateTime windowEnd, int mergedFreeBusyInterval)
		{
			return (int)Math.Ceiling((windowEnd - windowStart).TotalMinutes / (double)mergedFreeBusyInterval);
		}

		private static string BuildMergedFreeBusyString(ExTimeZone timeZone, int mergedFreeBusyInterval, ExDateTime windowStart, ExDateTime windowEnd, int numberOfFreeBusySlots, CalendarEvent[] calendarEventArray, bool exceptionOccurred, byte[] globalObjectId, ExchangeVersionType requestedSchemaVersion)
		{
			char[] array = new char[numberOfFreeBusySlots];
			char c;
			if (requestedSchemaVersion < ExchangeVersionType.Exchange2012)
			{
				c = (exceptionOccurred ? '4' : '0');
			}
			else
			{
				c = (exceptionOccurred ? '5' : '0');
			}
			for (int i = 0; i < numberOfFreeBusySlots; i++)
			{
				array[i] = c;
			}
			if (!exceptionOccurred && calendarEventArray != null)
			{
				int num = calendarEventArray.Length;
				int j = 0;
				while (j < num)
				{
					if (globalObjectId == null)
					{
						goto IL_72;
					}
					byte[] globalObjectId2 = calendarEventArray[j].GlobalObjectId;
					if (globalObjectId2 == null || !ArrayComparer<byte>.Comparer.Equals(globalObjectId2, globalObjectId))
					{
						goto IL_72;
					}
					IL_B3:
					j++;
					continue;
					IL_72:
					ExDateTime startTime = new ExDateTime(timeZone, calendarEventArray[j].StartTime);
					ExDateTime endTime = new ExDateTime(timeZone, calendarEventArray[j].EndTime);
					BusyType busyType = calendarEventArray[j].BusyType;
					MergedFreeBusy.AddAppointmentToFreeBusyString(windowStart, windowEnd, startTime, endTime, busyType, mergedFreeBusyInterval, array, requestedSchemaVersion);
					goto IL_B3;
				}
			}
			return new string(array);
		}
	}
}
