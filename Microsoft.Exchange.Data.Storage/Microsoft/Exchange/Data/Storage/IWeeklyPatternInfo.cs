using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IWeeklyPatternInfo
	{
		DaysOfWeek DaysOfWeek { get; }

		DayOfWeek FirstDayOfWeek { get; }
	}
}
