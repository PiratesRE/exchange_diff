using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.HolidayCalendars.Configuration
{
	internal interface IDateTimeFactory
	{
		ExDateTime GetUtcNow();
	}
}
