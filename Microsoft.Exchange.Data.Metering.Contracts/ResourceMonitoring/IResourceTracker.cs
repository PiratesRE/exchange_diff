using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Exchange.Data.Metering.ResourceMonitoring
{
	internal interface IResourceTracker
	{
		event ResourceUseChangedHandler ResourceUseChanged;

		ResourceUse AggregateResourceUse { get; }

		bool IsTracking { get; }

		IEnumerable<ResourceUse> ResourceUses { get; }

		DateTime LastUpdateTime { get; }

		TimeSpan TrackingInterval { get; }

		IEnumerable<IResourceMeter> ResourceMeters { get; }

		Task StartResourceTrackingAsync(CancellationToken cancellationToken);

		ResourceTrackerDiagnosticsData GetDiagnosticsData();
	}
}
