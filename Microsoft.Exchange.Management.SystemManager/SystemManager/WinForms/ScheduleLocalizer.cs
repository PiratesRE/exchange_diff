using System;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public static class ScheduleLocalizer
	{
		private static readonly DateTime MiniDateTime = DateTime.MinValue;

		public static readonly string RunWeekDaysFrom0800AMTo0500PM = Strings.FormatWeekDaysInterval(ScheduleLocalizer.MiniDateTime.AddHours(8.0), ScheduleLocalizer.MiniDateTime.AddHours(17.0));

		public static readonly string RunWeekDaysFrom0900AMTo0500PM = Strings.FormatWeekDaysInterval(ScheduleLocalizer.MiniDateTime.AddHours(9.0), ScheduleLocalizer.MiniDateTime.AddHours(17.0));

		public static readonly string RunWeekDaysFrom0900AMTo0600PM = Strings.FormatWeekDaysInterval(ScheduleLocalizer.MiniDateTime.AddHours(9.0), ScheduleLocalizer.MiniDateTime.AddHours(18.0));

		public static readonly string RunWeekDaysFrom0800AMTo1200PMAnd0100PMTo0500PM = Strings.FormatWeekDaysTwoIntervals(ScheduleLocalizer.MiniDateTime.AddHours(8.0), ScheduleLocalizer.MiniDateTime.AddHours(12.0), ScheduleLocalizer.MiniDateTime.AddHours(13.0), ScheduleLocalizer.MiniDateTime.AddHours(17.0));

		public static readonly string RunWeekDaysFrom0900AMTo1200PMAnd0100PMTo0600PM = Strings.FormatWeekDaysTwoIntervals(ScheduleLocalizer.MiniDateTime.AddHours(9.0), ScheduleLocalizer.MiniDateTime.AddHours(12.0), ScheduleLocalizer.MiniDateTime.AddHours(13.0), ScheduleLocalizer.MiniDateTime.AddHours(18.0));

		public static readonly string RunDailyFrom1100PMTo0300AM = Strings.FormatRunDailyFromTo(ScheduleLocalizer.MiniDateTime.AddHours(23.0), ScheduleLocalizer.MiniDateTime.AddHours(3.0));

		public static readonly string RunDailyFromMidnightTo0400AM = Strings.FormatRunDailyFromMidnightTo(ScheduleLocalizer.MiniDateTime.AddHours(4.0));

		public static readonly string RunDailyFrom0100AMTo0500AM = Strings.FormatRunDailyFromTo(ScheduleLocalizer.MiniDateTime.AddHours(1.0), ScheduleLocalizer.MiniDateTime.AddHours(5.0));

		public static readonly string RunDailyFrom0200AMTo0600AM = Strings.FormatRunDailyFromTo(ScheduleLocalizer.MiniDateTime.AddHours(2.0), ScheduleLocalizer.MiniDateTime.AddHours(6.0));

		public static readonly string RunDailyAt0100AM = Strings.RunDailyAt(ScheduleLocalizer.MiniDateTime.AddHours(1.0));

		public static readonly string RunDailyAt0200AM = Strings.RunDailyAt(ScheduleLocalizer.MiniDateTime.AddHours(2.0));

		public static readonly string RunDailyAt0300AM = Strings.RunDailyAt(ScheduleLocalizer.MiniDateTime.AddHours(3.0));

		public static readonly string RunDailyAt0400AM = Strings.RunDailyAt(ScheduleLocalizer.MiniDateTime.AddHours(4.0));

		public static readonly string RunDailyAt0500AM = Strings.RunDailyAt(ScheduleLocalizer.MiniDateTime.AddHours(5.0));
	}
}
