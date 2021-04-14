using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.ApplicationLogic.SyncCalendar
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SyncCalendarItemType
	{
		public string UID { get; set; }

		public StoreId ItemId { get; set; }

		public CalendarItemType CalendarItemType { get; set; }

		public ExDateTime? Start { get; set; }

		public ExDateTime? End { get; set; }

		public ExDateTime? StartWallClock { get; set; }

		public ExDateTime? EndWallClock { get; set; }

		public Dictionary<PropertyDefinition, object> RowData { get; set; }
	}
}
