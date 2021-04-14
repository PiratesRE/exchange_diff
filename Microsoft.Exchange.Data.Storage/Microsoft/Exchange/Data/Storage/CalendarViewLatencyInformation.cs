using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CalendarViewLatencyInformation
	{
		public bool IsNewView { get; set; }

		public long ViewTime { get; set; }

		public long SingleItemTotalTime { get; set; }

		public long SingleItemQueryTime { get; set; }

		public long SingleQuerySeekToTime { get; set; }

		public long SingleItemGetRowsTime { get; set; }

		public int SingleItemQueryCount { get; set; }

		public long RecurringItemTotalTime { get; set; }

		public long RecurringItemQueryTime { get; set; }

		public long RecurringItemGetRowsTime { get; set; }

		public long RecurringExpansionTime { get; set; }

		public RecurringItemLatencyInformation MaxRecurringItemLatencyInformation { get; set; }

		public long RecurringItemQueryCount { get; set; }

		public long RecurringItemsNoInstancesInWindow { get; set; }
	}
}
