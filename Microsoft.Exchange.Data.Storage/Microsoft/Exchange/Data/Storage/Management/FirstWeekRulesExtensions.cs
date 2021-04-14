using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class FirstWeekRulesExtensions
	{
		public static CalendarWeekRule ToCalendarWeekRule(this FirstWeekRules firstWeekRule)
		{
			switch (firstWeekRule)
			{
			case FirstWeekRules.FirstFourDayWeek:
				return CalendarWeekRule.FirstFourDayWeek;
			case FirstWeekRules.FirstFullWeek:
				return CalendarWeekRule.FirstFullWeek;
			}
			return CalendarWeekRule.FirstDay;
		}

		public static FirstWeekRules ToFirstWeekRules(this CalendarWeekRule calendarWeekRule)
		{
			switch (calendarWeekRule)
			{
			case CalendarWeekRule.FirstFullWeek:
				return FirstWeekRules.FirstFullWeek;
			case CalendarWeekRule.FirstFourDayWeek:
				return FirstWeekRules.FirstFourDayWeek;
			}
			return FirstWeekRules.FirstDay;
		}
	}
}
