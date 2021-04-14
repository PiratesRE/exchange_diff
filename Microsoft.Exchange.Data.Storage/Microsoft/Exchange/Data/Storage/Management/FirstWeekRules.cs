using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	public enum FirstWeekRules
	{
		[LocDescription(ServerStrings.IDs.FirstDay)]
		LegacyNotSet,
		[LocDescription(ServerStrings.IDs.FirstDay)]
		FirstDay,
		[LocDescription(ServerStrings.IDs.FirstFourDayWeek)]
		FirstFourDayWeek,
		[LocDescription(ServerStrings.IDs.FirstFullWeek)]
		FirstFullWeek
	}
}
