using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	public enum HourIncrement
	{
		[LocDescription(ServerStrings.IDs.FifteenMinutes)]
		FifteenMinutes = 15,
		[LocDescription(ServerStrings.IDs.ThirtyMinutes)]
		ThirtyMinutes = 30
	}
}
