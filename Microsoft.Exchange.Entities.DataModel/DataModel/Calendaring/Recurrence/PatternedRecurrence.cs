using System;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring.Recurrence
{
	public class PatternedRecurrence
	{
		public RecurrencePattern Pattern { get; set; }

		public RecurrenceRange Range { get; set; }
	}
}
