using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Rpc.AdminRpc;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.RpcProxy
{
	internal interface IRpcInstanceManager
	{
		event OnPoolNotificationsReceivedCallback NotificationsReceived;

		event OnRpcInstanceClosedCallback RpcInstanceClosed;

		void StopAcceptingCalls();

		ErrorCode StartInstance(Guid instanceId, uint flags, ref bool isNewInstanceStarted, CancellationToken cancellationToken);

		void StopInstance(Guid instanceId, bool terminate);

		bool IsInstanceStarted(Guid instanceId);

		string GetInstanceDisplayName(Guid instanceId);

		RpcInstanceManager.AdminCallGuard GetAdminRpcClient(Guid instanceId, string functionName, out AdminRpcClient adminRpc);

		RpcInstanceManager.RpcClient<RpcInstancePool> GetPoolRpcClient(Guid instanceId, ref int generation, out RpcInstancePool rpcClient);

		IEnumerable<Guid> GetActiveInstances();
	}
}
