using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal interface IADConfig
	{
		IADDatabaseAvailabilityGroup GetLocalDag();

		IADServer GetLocalServer();

		IADServer GetServer(string nodeOrFqdn);

		IADServer GetServer(AmServerName serverName);

		IADDatabase GetDatabase(Guid dbGuid);

		IEnumerable<IADDatabase> GetDatabasesOnServer(AmServerName serverName);

		IEnumerable<IADDatabase> GetDatabasesOnServer(IADServer server);

		IEnumerable<IADDatabase> GetDatabasesOnLocalServer();

		IEnumerable<IADDatabase> GetDatabasesInLocalDag();

		void Refresh(string reason);
	}
}
