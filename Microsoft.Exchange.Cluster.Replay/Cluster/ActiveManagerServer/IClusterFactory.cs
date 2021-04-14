using System;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal interface IClusterFactory
	{
		IAmCluster Open();

		IAmDbState CreateClusterDbState(IAmCluster cluster);

		IAmCluster OpenByName(AmServerName serverName);

		bool IsRunning();

		bool IsEvicted(AmServerName machineName);

		IAmCluster CreateExchangeCluster(string clusterName, AmServerName firstNodeName, string[] ipAddress, uint[] ipPrefixLength, IClusterSetupProgress setupProgress, IntPtr context, out Exception failureException, bool throwExceptionOnFailure);
	}
}
