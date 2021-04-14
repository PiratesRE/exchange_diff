using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal interface ICopyStatusClientLookup
	{
		CopyStatusClientCachedEntry GetCopyStatus(Guid dbGuid, AmServerName server, CopyStatusClientLookupFlags flags);

		IEnumerable<CopyStatusClientCachedEntry> GetCopyStatusesByDatabase(Guid dbGuid, IEnumerable<AmServerName> servers, CopyStatusClientLookupFlags flags);

		IEnumerable<CopyStatusClientCachedEntry> GetCopyStatusesByServer(AmServerName server, IEnumerable<IADDatabase> expectedDatabases, CopyStatusClientLookupFlags flags);
	}
}
