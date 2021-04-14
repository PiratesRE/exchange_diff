using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	public class PropertyUpdateTracker
	{
		public LastTimeUpdateTimeAndValueTracker<DateTime> LastTimeCopierTimeUpdateTracker = new LastTimeUpdateTimeAndValueTracker<DateTime>();
	}
}
