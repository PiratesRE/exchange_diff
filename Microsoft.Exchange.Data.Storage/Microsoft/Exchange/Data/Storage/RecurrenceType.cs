using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum RecurrenceType
	{
		None,
		Daily,
		Weekly,
		Monthly,
		Yearly,
		DailyRegenerating = 100,
		WeeklyRegenerating,
		MonthlyRegenerating,
		YearlyRegenerating
	}
}
