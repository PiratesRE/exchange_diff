using System;

namespace Microsoft.Exchange.Cluster.Replay.Dumpster
{
	internal interface ISafetyNetVersionCheck
	{
		bool IsVersionCheckSatisfied();
	}
}
