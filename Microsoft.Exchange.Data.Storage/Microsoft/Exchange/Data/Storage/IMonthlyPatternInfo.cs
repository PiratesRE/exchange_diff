using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IMonthlyPatternInfo
	{
		CalendarType CalendarType { get; }
	}
}
