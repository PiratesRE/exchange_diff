using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal interface IInspectLog
	{
		bool InspectLog(long logfileNumber, bool fRecopyOnFailure);
	}
}
