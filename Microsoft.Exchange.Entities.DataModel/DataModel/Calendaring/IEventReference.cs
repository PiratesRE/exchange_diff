using System;
using Microsoft.Exchange.Entities.DataModel.Items;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring
{
	public interface IEventReference : IItemReference<Event>, IEntityReference<Event>
	{
		ICalendarReference Calendar { get; }
	}
}
