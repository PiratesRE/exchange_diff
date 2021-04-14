using System;
using Microsoft.Exchange.Data.Storage.Cluster;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CiSeederClientRpcExceptionWrapper : SeederRpcExceptionWrapper
	{
		private CiSeederClientRpcExceptionWrapper()
		{
		}

		public new static CiSeederClientRpcExceptionWrapper Instance
		{
			get
			{
				return CiSeederClientRpcExceptionWrapper.s_ciSeederRpcWrapper;
			}
		}

		[Obsolete("This method is not supported for this class.", true)]
		public new void ClientRethrowIfFailed(string serverName, RpcErrorExceptionInfo errorInfo)
		{
			throw new NotImplementedException();
		}

		[Obsolete("This method is not supported for this class.", true)]
		public new void ClientRethrowIfFailed(string databaseName, string serverName, RpcErrorExceptionInfo errorInfo)
		{
			throw new NotImplementedException();
		}

		[Obsolete("This method is not supported for this class.", true)]
		public new RpcErrorExceptionInfo RunRpcServerOperation(RpcServerOperation rpcOperation)
		{
			throw new NotImplementedException();
		}

		[Obsolete("This method is not supported for this class.", true)]
		public new RpcErrorExceptionInfo RunRpcServerOperation(string databaseName, RpcServerOperation rpcOperation)
		{
			throw new NotImplementedException();
		}

		protected override SeederServerException GetGenericOperationFailedException(string message)
		{
			return new CiSeederRpcOperationFailedException(message);
		}

		protected override SeederServerException GetGenericOperationFailedException(string message, Exception innerException)
		{
			return new CiSeederRpcOperationFailedException(message, innerException);
		}

		protected override SeederServerException GetServiceDownException(string serverName, Exception innerException)
		{
			return new CiServiceDownException(serverName, innerException.Message, innerException);
		}

		protected override SeederServerException GetGenericOperationFailedWithEcException(int errorCode)
		{
			throw new NotImplementedException();
		}

		protected override SeederServerTransientException GetGenericOperationFailedTransientException(string message, Exception innerException)
		{
			throw new NotImplementedException();
		}

		private static CiSeederClientRpcExceptionWrapper s_ciSeederRpcWrapper = new CiSeederClientRpcExceptionWrapper();
	}
}
