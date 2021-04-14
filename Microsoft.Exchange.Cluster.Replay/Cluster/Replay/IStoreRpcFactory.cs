using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal interface IStoreRpcFactory
	{
		IStoreRpc Construct(string serverNameOrFqdn, string clientTypeId);

		IStoreRpc ConstructWithNoTimeout(string serverNameOrFqdn);

		IListMDBStatus ConstructListMDBStatus(string serverNameOrFqdn, string clientTypeId);

		IStoreMountDismount ConstructMountDismount(string serverNameOrFqdn);
	}
}
