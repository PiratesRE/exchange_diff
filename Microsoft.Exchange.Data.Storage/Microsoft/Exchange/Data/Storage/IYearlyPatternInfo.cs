using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IYearlyPatternInfo : IMonthlyPatternInfo
	{
		bool IsLeapMonth { get; }

		int Month { get; }
	}
}
