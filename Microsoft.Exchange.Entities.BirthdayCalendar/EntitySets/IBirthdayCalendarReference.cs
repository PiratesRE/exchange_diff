using System;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Entities.BirthdayCalendar.EntitySets
{
	internal interface IBirthdayCalendarReference : IEntityReference<IBirthdayCalendar>
	{
		IBirthdayEvents Events { get; }
	}
}
