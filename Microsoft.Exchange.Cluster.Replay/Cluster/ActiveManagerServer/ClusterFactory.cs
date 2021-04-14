using System;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class ClusterFactory : IClusterFactory, IMockableCluster
	{
		public static IClusterFactory Instance
		{
			get
			{
				return ClusterFactory.instance;
			}
		}

		public static void SetTestClusterFactory(IClusterFactory testClusterFactory, IMockableCluster amClusterOverride)
		{
			ClusterFactory.instance = testClusterFactory;
			MockableCluster.SetTestInstance(amClusterOverride);
		}

		public IAmCluster Open()
		{
			return AmCluster.Open();
		}

		IAmCluster IMockableCluster.OpenByName(AmServerName serverName)
		{
			return AmCluster.OpenByName(serverName);
		}

		IAmCluster IClusterFactory.OpenByName(AmServerName serverName)
		{
			return AmCluster.OpenByName(serverName);
		}

		public IAmDbState CreateClusterDbState(IAmCluster cluster)
		{
			return new AmClusterDbState(cluster);
		}

		public bool IsRunning()
		{
			return AmCluster.IsRunning();
		}

		public bool IsEvicted(AmServerName machineName)
		{
			return AmCluster.IsEvicted(machineName);
		}

		public IAmCluster CreateExchangeCluster(string clusterName, AmServerName firstNodeName, string[] ipAddress, uint[] ipPrefixLength, IClusterSetupProgress setupProgress, IntPtr context, out Exception failureException, bool throwExceptionOnFailure)
		{
			return AmCluster.CreateExchangeCluster(clusterName, firstNodeName, ipAddress, ipPrefixLength, setupProgress, context, out failureException, throwExceptionOnFailure);
		}

		private static IClusterFactory instance = new ClusterFactory();
	}
}
