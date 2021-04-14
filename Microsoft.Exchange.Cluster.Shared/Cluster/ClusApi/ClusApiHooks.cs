using System;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal enum ClusApiHooks
	{
		OpenCluster = 1,
		CloseCluster,
		GetClusterKey,
		ClusterRegBatchAddCommand,
		ClusterRegCreateBatch,
		ClusterRegCloseBatch,
		ClusterRegOpenKey,
		ClusterRegCreateKey,
		ClusterRegQueryValue,
		ClusterRegSetValue,
		ClusterRegDeleteValue,
		ClusterRegDeleteKey,
		ClusterRegCloseKey
	}
}
