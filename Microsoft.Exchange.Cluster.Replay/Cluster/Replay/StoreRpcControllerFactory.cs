using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class StoreRpcControllerFactory : IStoreRpcFactory
	{
		public IStoreRpc Construct(string serverNameOrFqdn, string clientTypeId)
		{
			return new StoreRpcController(serverNameOrFqdn, clientTypeId);
		}

		public IStoreRpc ConstructWithNoTimeout(string serverNameOrFqdn)
		{
			return new StoreRpcControllerNoTimeout(serverNameOrFqdn);
		}

		public IListMDBStatus ConstructListMDBStatus(string serverNameOrFqdn, string clientTypeId)
		{
			return new StoreRpcController(serverNameOrFqdn, clientTypeId);
		}

		public IStoreMountDismount ConstructMountDismount(string serverNameOrFqdn)
		{
			return new StoreRpcController(serverNameOrFqdn, null);
		}
	}
}
