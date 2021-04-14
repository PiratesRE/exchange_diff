using System;

namespace Microsoft.Exchange.Data.Metering
{
	internal enum MeteringEvent
	{
		EntityAdded,
		EntityRemoved,
		MeasureAdded,
		MeasureRemoved,
		MeasurePromoted,
		MeasureExpired
	}
}
