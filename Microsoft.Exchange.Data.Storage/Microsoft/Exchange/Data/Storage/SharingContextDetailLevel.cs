using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum SharingContextDetailLevel
	{
		None,
		AvailabilityOnly,
		Limited,
		FullDetails = 4,
		Editor = 8
	}
}
