using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.Cluster.DirectoryServices;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal interface IReplayAdObjectLookup
	{
		ITopologyConfigurationSession AdSession { get; }

		IFindAdObject<IADDatabaseAvailabilityGroup> DagLookup { get; }

		IFindAdObject<IADDatabase> DatabaseLookup { get; }

		IFindAdObject<IADServer> ServerLookup { get; }

		IFindMiniServer MiniServerLookup { get; }

		void Clear();
	}
}
