using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Data.Storage.Cluster
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IHaRpcExceptionWrapper
	{
		void ClientRetryableOperation(string serverName, RpcClientOperation rpcOperation);

		void ClientRethrowIfFailed(string serverName, RpcErrorExceptionInfo errorInfo);

		void ClientRethrowIfFailed(string databaseName, string serverName, RpcErrorExceptionInfo errorInfo);

		RpcErrorExceptionInfo RunRpcServerOperation(RpcServerOperation rpcOperation);

		RpcErrorExceptionInfo RunRpcServerOperation(string databaseName, RpcServerOperation rpcOperation);
	}
}
