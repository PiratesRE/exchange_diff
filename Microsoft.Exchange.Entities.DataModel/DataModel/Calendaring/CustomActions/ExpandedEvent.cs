using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions
{
	public class ExpandedEvent
	{
		public Event RecurrenceMaster { get; set; }

		public IList<Event> Occurrences { get; set; }

		public IList<string> CancelledOccurrences { get; set; }
	}
}
