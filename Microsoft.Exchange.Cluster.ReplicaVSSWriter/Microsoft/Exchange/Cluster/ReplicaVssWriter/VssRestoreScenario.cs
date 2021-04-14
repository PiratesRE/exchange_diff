using System;

namespace Microsoft.Exchange.Cluster.ReplicaVssWriter
{
	internal enum VssRestoreScenario
	{
		rstscenUnknown,
		rstscenOriginalDB,
		rstscenAlternateDB,
		rstscenAlternateLoc
	}
}
