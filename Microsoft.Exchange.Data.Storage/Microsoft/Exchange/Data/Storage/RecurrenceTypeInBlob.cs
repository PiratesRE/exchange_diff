using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum RecurrenceTypeInBlob
	{
		Minute,
		Week,
		Month,
		MonthNth,
		MonthEnd,
		Year,
		YearNth,
		HjMonth = 10,
		HjMonthNth,
		HjMonthEnd
	}
}
