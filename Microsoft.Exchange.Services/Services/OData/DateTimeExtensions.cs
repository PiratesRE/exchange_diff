using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Services.OData
{
	internal static class DateTimeExtensions
	{
		internal static ExDateTime ToExDateTime(this DateTimeOffset dateTimeOffset)
		{
			return (ExDateTime)dateTimeOffset.UtcDateTime;
		}

		internal static DateTimeOffset ToDateTimeOffset(this ExDateTime exDateTime)
		{
			return new DateTimeOffset((DateTime)exDateTime, exDateTime.Bias);
		}
	}
}
