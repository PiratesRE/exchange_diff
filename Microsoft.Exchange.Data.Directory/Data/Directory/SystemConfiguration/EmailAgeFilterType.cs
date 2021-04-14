using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum EmailAgeFilterType
	{
		[LocDescription(DirectoryStrings.IDs.EmailAgeFilterAll)]
		All,
		[LocDescription(DirectoryStrings.IDs.EmailAgeFilterOneDay)]
		OneDay,
		[LocDescription(DirectoryStrings.IDs.EmailAgeFilterThreeDays)]
		ThreeDays,
		[LocDescription(DirectoryStrings.IDs.EmailAgeFilterOneWeek)]
		OneWeek,
		[LocDescription(DirectoryStrings.IDs.EmailAgeFilterTwoWeeks)]
		TwoWeeks,
		[LocDescription(DirectoryStrings.IDs.EmailAgeFilterOneMonth)]
		OneMonth
	}
}
