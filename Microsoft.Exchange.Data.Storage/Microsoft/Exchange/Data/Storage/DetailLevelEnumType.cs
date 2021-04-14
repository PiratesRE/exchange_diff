using System;

namespace Microsoft.Exchange.Data.Storage
{
	public enum DetailLevelEnumType
	{
		[LocDescription(ServerStrings.IDs.AvailabilityOnly)]
		AvailabilityOnly = 1,
		[LocDescription(ServerStrings.IDs.LimitedDetails)]
		LimitedDetails,
		[LocDescription(ServerStrings.IDs.FullDetails)]
		FullDetails,
		[LocDescription(ServerStrings.IDs.Editor)]
		Editor
	}
}
