using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Flags]
	public enum OwaRecurrenceType
	{
		None = 1,
		Daily = 2,
		Weekly = 4,
		Monthly = 8,
		Yearly = 16,
		CoreTypeMask = 255,
		DailyEveryWeekday = 256,
		MonthlyTh = 512,
		YearlyTh = 1024,
		DailyRegenerating = 2048,
		WeeklyRegenerating = 4096,
		MonthlyRegenerating = 8192,
		YearlyRegenerating = 16384,
		ValidValuesMask = 32543
	}
}
