using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum CalendarInconsistencyGroup
	{
		None,
		StartTime,
		EndTime,
		Recurrence,
		Location,
		Cancellation,
		MissingItem,
		Duplicate
	}
}
