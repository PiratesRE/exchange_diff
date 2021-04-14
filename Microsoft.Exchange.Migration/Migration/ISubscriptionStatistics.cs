using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ISubscriptionStatistics : IStepSnapshot, IMigrationSerializable
	{
		ExDateTime? LastSyncTime { get; }

		long NumItemsSkipped { get; }

		long NumItemsSynced { get; }
	}
}
