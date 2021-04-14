using System;
using Microsoft.Exchange.Data.Storage.Cluster;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DagRpcExceptionWrapper : HaRpcExceptionWrapperBase<DagTaskServerException, DagTaskServerTransientException>
	{
		public static DagRpcExceptionWrapper Instance
		{
			get
			{
				return DagRpcExceptionWrapper.s_dagRpcWrapper;
			}
		}

		protected DagRpcExceptionWrapper()
		{
		}

		protected override DagTaskServerException GetGenericOperationFailedException(string message)
		{
			return new DagTaskOperationFailedException(message);
		}

		protected override DagTaskServerException GetGenericOperationFailedException(string message, Exception innerException)
		{
			return new DagTaskOperationFailedException(message, innerException);
		}

		protected override DagTaskServerException GetGenericOperationFailedWithEcException(int errorCode)
		{
			return new DagTaskOperationFailedWithEcException(errorCode);
		}

		protected override DagTaskServerException GetServiceDownException(string serverName, Exception innerException)
		{
			return new DagReplayServiceDownException(serverName, innerException.Message, innerException);
		}

		protected override DagTaskServerTransientException GetGenericOperationFailedTransientException(string message, Exception innerException)
		{
			return new DagTaskServerTransientException(message, innerException);
		}

		private static DagRpcExceptionWrapper s_dagRpcWrapper = new DagRpcExceptionWrapper();
	}
}
