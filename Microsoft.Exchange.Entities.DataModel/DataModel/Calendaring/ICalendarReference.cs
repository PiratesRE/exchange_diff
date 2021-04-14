using System;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring
{
	public interface ICalendarReference : IEntityReference<Calendar>
	{
		IEvents Events { get; }
	}
}
