using System;

namespace Microsoft.Exchange.AirSync
{
	internal enum AirSyncV25FilterTypes
	{
		InvalidFilter = -1,
		NoFilter,
		OneDayFilter,
		ThreeDayFilter,
		OneWeekFilter,
		TwoWeekFilter,
		OneMonthFilter,
		ThreeMonthFilter,
		SixMonthFilter,
		IncompleteFilter,
		MinValid = 0,
		MaxValid = 8
	}
}
