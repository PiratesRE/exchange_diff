using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum CalendarAgeFilterType
	{
		[LocDescription(DirectoryStrings.IDs.CalendarAgeFilterAll)]
		All,
		[LocDescription(DirectoryStrings.IDs.CalendarAgeFilterTwoWeeks)]
		TwoWeeks = 4,
		[LocDescription(DirectoryStrings.IDs.CalendarAgeFilterOneMonth)]
		OneMonth,
		[LocDescription(DirectoryStrings.IDs.CalendarAgeFilterThreeMonths)]
		ThreeMonths,
		[LocDescription(DirectoryStrings.IDs.CalendarAgeFilterSixMonths)]
		SixMonths
	}
}
