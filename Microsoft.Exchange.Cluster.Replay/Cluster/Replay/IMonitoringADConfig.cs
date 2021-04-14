using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal interface IMonitoringADConfig
	{
		DateTime CreateTimeUtc { get; }

		List<AmServerName> AmServerNames { get; }

		IADDatabaseAvailabilityGroup Dag { get; }

		Dictionary<AmServerName, IEnumerable<IADDatabase>> DatabaseMap { get; }

		Dictionary<Guid, IADDatabase> DatabaseByGuidMap { get; }

		Dictionary<AmServerName, IEnumerable<IADDatabase>> DatabasesIncludingMisconfiguredMap { get; }

		IADServer TargetMiniServer { get; }

		AmServerName TargetServerName { get; }

		IADServer LookupMiniServerByName(AmServerName serverName);

		MonitoringServerRole ServerRole { get; }

		List<IADServer> Servers { get; }
	}
}
