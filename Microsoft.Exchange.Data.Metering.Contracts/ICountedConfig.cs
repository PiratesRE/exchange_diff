using System;

namespace Microsoft.Exchange.Data.Metering
{
	internal interface ICountedConfig
	{
		bool IsPromotable { get; }

		int MinActivityThreshold { get; }

		TimeSpan TimeToLive { get; }

		TimeSpan IdleTimeToLive { get; }

		bool IsRemovable { get; }

		TimeSpan IdleCleanupInterval { get; }
	}
}
