using System;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal class MockableCluster : IMockableCluster
	{
		public static IMockableCluster Instance
		{
			get
			{
				return MockableCluster.instance;
			}
		}

		public static void SetTestInstance(IMockableCluster testInstance)
		{
			MockableCluster.instance = testInstance;
		}

		public IAmCluster OpenByName(AmServerName serverName)
		{
			return AmCluster.OpenByName(serverName);
		}

		private static IMockableCluster instance = new MockableCluster();
	}
}
