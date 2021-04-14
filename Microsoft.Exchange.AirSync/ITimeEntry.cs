using System;

namespace Microsoft.Exchange.AirSync
{
	internal interface ITimeEntry : IDisposable
	{
		TimeId TimeId { get; }

		DateTime StartTime { get; }

		DateTime EndTime { get; }

		TimeSpan ElapsedInclusive { get; }

		TimeSpan ElapsedExclusive { get; }
	}
}
