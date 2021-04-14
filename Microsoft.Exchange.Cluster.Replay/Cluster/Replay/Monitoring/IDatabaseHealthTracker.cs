using System;

namespace Microsoft.Exchange.Cluster.Replay.Monitoring
{
	internal interface IDatabaseHealthTracker
	{
		void UpdateHealthInfo(HealthInfoPersisted healthInfo);

		DateTime GetDagHealthInfoUpdateTimeUtc();

		HealthInfoPersisted GetDagHealthInfo();
	}
}
