using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IMonthlyThPatternInfo : IMonthlyPatternInfo
	{
		RecurrenceOrderType Order { get; }
	}
}
