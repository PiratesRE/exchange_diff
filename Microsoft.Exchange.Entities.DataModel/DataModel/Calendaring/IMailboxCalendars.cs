using System;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring
{
	public interface IMailboxCalendars : ICalendars, IEntitySet<Calendar>
	{
		ICalendarReference Default { get; }
	}
}
