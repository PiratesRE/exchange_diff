using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.HolidayCalendars.Configuration
{
	internal class DateTimeFactory : IDateTimeFactory
	{
		public ExDateTime GetUtcNow()
		{
			return ExDateTime.UtcNow;
		}
	}
}
