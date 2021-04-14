using System;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal interface IMockableCluster
	{
		IAmCluster OpenByName(AmServerName serverName);
	}
}
