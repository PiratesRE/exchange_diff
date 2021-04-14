using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal static class CalendarItemUtilities
	{
		public static int GetCancelRecurrenceCount(CalendarItem calendarItem, ExDateTime endRange)
		{
			if (calendarItem == null)
			{
				throw new ArgumentNullException("calendarItem");
			}
			if (endRange <= calendarItem.Recurrence.Range.StartDate)
			{
				return 0;
			}
			if (!(calendarItem.Recurrence.Range is NoEndRecurrenceRange))
			{
				OccurrenceInfo lastOccurrence = calendarItem.Recurrence.GetLastOccurrence();
				if (lastOccurrence != null && lastOccurrence.EndTime < endRange)
				{
					return int.MinValue;
				}
			}
			EndDateRecurrenceRange endDateRecurrenceRange = calendarItem.Recurrence.Range as EndDateRecurrenceRange;
			if (endDateRecurrenceRange != null && endDateRecurrenceRange.EndDate < endRange)
			{
				return int.MinValue;
			}
			IList<OccurrenceInfo> occurrenceInfoList = calendarItem.Recurrence.GetOccurrenceInfoList(calendarItem.Recurrence.Range.StartDate, endRange);
			if (0 <= occurrenceInfoList.Count && (!(calendarItem.Recurrence.Range is NumberedRecurrenceRange) || occurrenceInfoList.Count < ((NumberedRecurrenceRange)calendarItem.Recurrence.Range).NumberOfOccurrences))
			{
				return occurrenceInfoList.Count;
			}
			return int.MinValue;
		}

		public static string GenerateWhen(UserContext userContext, ExDateTime startTime, ExDateTime endTime, Recurrence recurrence)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (recurrence == null)
			{
				throw new ArgumentNullException("recurrence");
			}
			string result;
			using (CalendarItem calendarItem = CalendarItem.Create(userContext.MailboxSession, userContext.CalendarFolderId))
			{
				calendarItem.StartTime = startTime;
				calendarItem.EndTime = endTime;
				calendarItem.Recurrence = recurrence;
				result = calendarItem.GenerateWhen();
			}
			return result;
		}

		public static bool BuildSendConfirmDialogPrompt(CalendarItemBase calendarItemBase, out string prompt)
		{
			if (calendarItemBase == null)
			{
				throw new ArgumentNullException("calendarItemBase");
			}
			prompt = null;
			StringBuilder stringBuilder = null;
			ExDateTime localTime = DateTimeUtilities.GetLocalTime();
			if (calendarItemBase.CalendarItemType == CalendarItemType.RecurringMaster)
			{
				CalendarItem calendarItem = calendarItemBase as CalendarItem;
				if (!(calendarItem.Recurrence.Range is NoEndRecurrenceRange))
				{
					OccurrenceInfo lastOccurrence = calendarItem.Recurrence.GetLastOccurrence();
					if (lastOccurrence != null && lastOccurrence.EndTime < localTime)
					{
						if (stringBuilder == null)
						{
							stringBuilder = new StringBuilder();
						}
						stringBuilder.Append("\n\t");
						stringBuilder.Append(LocalizedStrings.GetNonEncoded(2056979915));
					}
				}
			}
			else if (calendarItemBase.EndTime < localTime)
			{
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder();
				}
				stringBuilder.Append("\n\t");
				stringBuilder.Append(LocalizedStrings.GetNonEncoded(839442440));
			}
			if (string.IsNullOrEmpty(calendarItemBase.Subject))
			{
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder();
				}
				stringBuilder.Append("\n\t");
				stringBuilder.Append(LocalizedStrings.GetNonEncoded(-25858033));
			}
			if (string.IsNullOrEmpty(calendarItemBase.Location))
			{
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder();
				}
				stringBuilder.Append("\n\t");
				stringBuilder.Append(LocalizedStrings.GetNonEncoded(-1681723506));
			}
			if (stringBuilder != null)
			{
				stringBuilder.Insert(0, "\n");
				stringBuilder.Insert(0, LocalizedStrings.GetNonEncoded(1040416023));
				stringBuilder.Append("\n\n");
				stringBuilder.Append(LocalizedStrings.GetNonEncoded(105464887));
				prompt = stringBuilder.ToString();
				return true;
			}
			return false;
		}

		public const int DefaultMeetingLengthMinutes = 60;
	}
}
