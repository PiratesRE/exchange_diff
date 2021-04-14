﻿using System;

namespace Microsoft.Exchange.Data
{
	public enum CustomScheduleName
	{
		[LocDescription(DataStrings.IDs.CustomScheduleDaily10PM)]
		Daily10PM,
		[LocDescription(DataStrings.IDs.CustomScheduleDaily11PM)]
		Daily11PM,
		[LocDescription(DataStrings.IDs.CustomScheduleDaily12PM)]
		Daily12PM,
		[LocDescription(DataStrings.IDs.CustomScheduleDailyAtMidnight)]
		DailyAtMidnight,
		[LocDescription(DataStrings.IDs.CustomScheduleDaily1AM)]
		Daily1AM,
		[LocDescription(DataStrings.IDs.CustomScheduleDaily2AM)]
		Daily2AM,
		[LocDescription(DataStrings.IDs.CustomScheduleDaily3AM)]
		Daily3AM,
		[LocDescription(DataStrings.IDs.CustomScheduleDaily4AM)]
		Daily4AM,
		[LocDescription(DataStrings.IDs.CustomScheduleDaily5AM)]
		Daily5AM,
		[LocDescription(DataStrings.IDs.CustomScheduleDailyFrom11PMTo3AM)]
		DailyFrom11PMTo3AM,
		[LocDescription(DataStrings.IDs.CustomScheduleDailyFrom11PMTo6AM)]
		DailyFrom11PMTo6AM,
		[LocDescription(DataStrings.IDs.CustomScheduleDailyFrom1AMTo5AM)]
		DailyFrom1AMTo5AM,
		[LocDescription(DataStrings.IDs.CustomScheduleDailyFrom2AMTo6AM)]
		DailyFrom2AMTo6AM,
		[LocDescription(DataStrings.IDs.CustomScheduleDailyFromMidnightTo4AM)]
		DailyFromMidnightTo4AM,
		[LocDescription(DataStrings.IDs.CustomScheduleFridayAtMidnight)]
		FridayAtMidnight,
		[LocDescription(DataStrings.IDs.CustomScheduleSaturdayAtMidnight)]
		SaturdayAtMidnight,
		[LocDescription(DataStrings.IDs.CustomScheduleSundayAtMidnight)]
		SundayAtMidnight,
		[LocDescription(DataStrings.IDs.CustomScheduleEveryHalfHour)]
		EveryHalfHour,
		[LocDescription(DataStrings.IDs.CustomScheduleEveryHour)]
		EveryHour,
		[LocDescription(DataStrings.IDs.CustomScheduleEveryTwoHours)]
		EveryTwoHours,
		[LocDescription(DataStrings.IDs.CustomScheduleEveryFourHours)]
		EveryFourHours,
		[LocDescription(DataStrings.IDs.CustomScheduleDailyFrom8AMTo5PMAtWeekDays)]
		DailyFrom8AMTo5PMAtWeekDays,
		[LocDescription(DataStrings.IDs.CustomScheduleDailyFrom9AMTo5PMAtWeekDays)]
		DailyFrom9AMTo5PMAtWeekDays,
		[LocDescription(DataStrings.IDs.CustomScheduleDailyFrom9AMTo6PMAtWeekDays)]
		DailyFrom9AMTo6PMAtWeekDays,
		[LocDescription(DataStrings.IDs.CustomScheduleDailyFrom8AMTo12PMAnd1PMTo5PMAtWeekDays)]
		DailyFrom8AMTo12PMAnd1PMTo5PMAtWeekDays,
		[LocDescription(DataStrings.IDs.CustomScheduleDailyFrom9AMTo12PMAnd1PMTo6PMAtWeekDays)]
		DailyFrom9AMTo12PMAnd1PMTo6PMAtWeekDays
	}
}
