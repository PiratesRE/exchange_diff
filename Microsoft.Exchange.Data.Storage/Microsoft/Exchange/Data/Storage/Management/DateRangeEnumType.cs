using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	public enum DateRangeEnumType
	{
		[LocDescription(ServerStrings.IDs.DateRangeOneDay)]
		OneDay,
		[LocDescription(ServerStrings.IDs.DateRangeThreeDays)]
		ThreeDays,
		[LocDescription(ServerStrings.IDs.DateRangeOneWeek)]
		OneWeek,
		[LocDescription(ServerStrings.IDs.DateRangeOneMonth)]
		OneMonth,
		[LocDescription(ServerStrings.IDs.DateRangeThreeMonths)]
		ThreeMonths,
		[LocDescription(ServerStrings.IDs.DateRangeSixMonths)]
		SixMonths,
		[LocDescription(ServerStrings.IDs.DateRangeOneYear)]
		OneYear
	}
}
