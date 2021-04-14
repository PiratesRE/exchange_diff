using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class CalendarOriginatorIdProperty
	{
		internal static bool TryCreate(MailboxSession session, out string calendarOriginatorId)
		{
			return CalendarOriginatorIdProperty.TryCreate(session.MailboxGuid, session.MailboxOwnerLegacyDN, out calendarOriginatorId);
		}

		internal static bool TryCreate(Guid mailboxGuid, string mailboxOwnerLegacyDN, out string calendarOriginatorId)
		{
			calendarOriginatorId = null;
			if (string.IsNullOrEmpty(mailboxOwnerLegacyDN))
			{
				return false;
			}
			calendarOriginatorId = mailboxGuid + CalendarOriginatorIdProperty.CalendarOriginatorIdSeparatorToken + mailboxOwnerLegacyDN;
			return true;
		}

		internal static bool TryParse(string calendarOriginatorId, out string[] calendarOriginatorIdParts)
		{
			calendarOriginatorIdParts = calendarOriginatorId.Split(new string[]
			{
				CalendarOriginatorIdProperty.CalendarOriginatorIdSeparatorToken
			}, 2, StringSplitOptions.RemoveEmptyEntries);
			return calendarOriginatorIdParts != null && calendarOriginatorIdParts.Length == 2;
		}

		internal static string GetPreferredCalendarOriginatorId(string calendarOriginatorIdToCheck, string storedCalendarOriginatorId)
		{
			bool flag = string.IsNullOrWhiteSpace(calendarOriginatorIdToCheck);
			bool flag2 = string.IsNullOrWhiteSpace(storedCalendarOriginatorId);
			if (flag && flag2)
			{
				return null;
			}
			if (flag)
			{
				string[] array;
				if (!CalendarOriginatorIdProperty.TryParse(storedCalendarOriginatorId, out array))
				{
					return null;
				}
				return storedCalendarOriginatorId;
			}
			else
			{
				if (flag2)
				{
					return calendarOriginatorIdToCheck;
				}
				int? num = CalendarOriginatorIdProperty.Compare(storedCalendarOriginatorId, calendarOriginatorIdToCheck);
				if (num != null && num >= 0)
				{
					return storedCalendarOriginatorId;
				}
				return calendarOriginatorIdToCheck;
			}
		}

		internal static int? Compare(string originatorId1, string originatorId2)
		{
			if (string.Compare(originatorId1, originatorId2, StringComparison.OrdinalIgnoreCase) == 0)
			{
				return new int?(0);
			}
			if (string.IsNullOrWhiteSpace(originatorId1) || string.IsNullOrWhiteSpace(originatorId2))
			{
				return null;
			}
			string[] array;
			if (!CalendarOriginatorIdProperty.TryParse(originatorId1, out array))
			{
				return null;
			}
			string[] array2;
			if (!CalendarOriginatorIdProperty.TryParse(originatorId2, out array2))
			{
				return null;
			}
			if (string.Compare(array[CalendarOriginatorIdProperty.CalendarOriginatorIdGuidIndex], array2[CalendarOriginatorIdProperty.CalendarOriginatorIdGuidIndex], StringComparison.OrdinalIgnoreCase) == 0)
			{
				return new int?(1);
			}
			if (string.Compare(array[CalendarOriginatorIdProperty.CalendarOriginatorIdLegDNIndex], array2[CalendarOriginatorIdProperty.CalendarOriginatorIdLegDNIndex], StringComparison.OrdinalIgnoreCase) == 0)
			{
				return new int?(1);
			}
			return new int?(-1);
		}

		private static readonly string CalendarOriginatorIdSeparatorToken = ";";

		internal static readonly int CalendarOriginatorIdGuidIndex = 0;

		internal static readonly int CalendarOriginatorIdLegDNIndex = 1;
	}
}
