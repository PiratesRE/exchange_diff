using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	internal class GSCalendarItem
	{
		public ExDateTime StartTime { get; set; }

		public ExDateTime EndTime { get; set; }

		public BusyTypeWrapper BusyType { get; set; }

		public string Subject { get; set; }

		public string Location { get; set; }

		public bool IsMeeting { get; set; }

		public CalendarItemTypeWrapper CalendarItemType { get; set; }

		public bool IsPrivate { get; set; }
	}
}
