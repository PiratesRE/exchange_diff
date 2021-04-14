using System;

namespace Microsoft.Exchange.Data
{
	[Flags]
	public enum DaysOfWeek
	{
		[LocDescription(DataStrings.IDs.DaysOfWeek_None)]
		None = 0,
		[LocDescription(DataStrings.IDs.Sunday)]
		Sunday = 1,
		[LocDescription(DataStrings.IDs.Monday)]
		Monday = 2,
		[LocDescription(DataStrings.IDs.Tuesday)]
		Tuesday = 4,
		[LocDescription(DataStrings.IDs.Wednesday)]
		Wednesday = 8,
		[LocDescription(DataStrings.IDs.Thursday)]
		Thursday = 16,
		[LocDescription(DataStrings.IDs.Friday)]
		Friday = 32,
		[LocDescription(DataStrings.IDs.Saturday)]
		Saturday = 64,
		[LocDescription(DataStrings.IDs.Weekdays)]
		Weekdays = 62,
		[LocDescription(DataStrings.IDs.WeekendDays)]
		WeekendDays = 65,
		[LocDescription(DataStrings.IDs.AllDays)]
		AllDays = 127
	}
}
