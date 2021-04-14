using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	internal class InvalidDateRangeException : Exception
	{
		internal InvalidDateRangeException(ExDateTime rangeStart, ExDateTime rangeEnd) : base(string.Format("Invalid date range {0} to {1}", rangeStart, rangeEnd))
		{
		}
	}
}
