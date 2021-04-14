using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	public enum DayOfWeek
	{
		[LocDescription(ServerStrings.IDs.Sunday)]
		Sunday,
		[LocDescription(ServerStrings.IDs.Monday)]
		Monday,
		[LocDescription(ServerStrings.IDs.Tuesday)]
		Tuesday,
		[LocDescription(ServerStrings.IDs.Wednesday)]
		Wednesday,
		[LocDescription(ServerStrings.IDs.Thursday)]
		Thursday,
		[LocDescription(ServerStrings.IDs.Friday)]
		Friday,
		[LocDescription(ServerStrings.IDs.Saturday)]
		Saturday
	}
}
