using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.ApplicationLogic.SyncCalendar
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SyncCalendarResponse
	{
		public CalendarViewQueryResumptionPoint QueryResumptionPoint { get; set; }

		public ExDateTime? OldWindowEnd { get; set; }

		public bool IncludesLastItemInRange { get; set; }

		public IList<StoreId> DeletedItems { get; set; }

		public IList<SyncCalendarItemType> UpdatedItems { get; set; }

		public IList<SyncCalendarItemType> RecurrenceMastersWithInstances { get; set; }

		public IList<SyncCalendarItemType> RecurrenceMastersWithoutInstances { get; set; }

		public IList<SyncCalendarItemType> UnchangedRecurrenceMastersWithInstances { get; set; }
	}
}
