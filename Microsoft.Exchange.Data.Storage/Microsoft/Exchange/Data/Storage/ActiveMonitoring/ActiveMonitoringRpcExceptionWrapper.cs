using System;
using Microsoft.Exchange.Data.Storage.Cluster;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.ActiveMonitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ActiveMonitoringRpcExceptionWrapper : HaRpcExceptionWrapperBase<ActiveMonitoringServerException, ActiveMonitoringServerTransientException>
	{
		protected ActiveMonitoringRpcExceptionWrapper()
		{
		}

		public static ActiveMonitoringRpcExceptionWrapper Instance
		{
			get
			{
				return ActiveMonitoringRpcExceptionWrapper.rpcWrapper;
			}
		}

		protected override ActiveMonitoringServerException GetGenericOperationFailedException(string message)
		{
			return new ActiveMonitoringOperationFailedException(message);
		}

		protected override ActiveMonitoringServerException GetGenericOperationFailedException(string message, Exception innerException)
		{
			return new ActiveMonitoringOperationFailedException(message, innerException);
		}

		protected override ActiveMonitoringServerException GetGenericOperationFailedWithEcException(int errorCode)
		{
			return new ActiveMonitoringOperationFailedWithEcException(errorCode);
		}

		protected override ActiveMonitoringServerException GetServiceDownException(string serverName, Exception innerException)
		{
			return new ActiveMonitoringServiceDownException(serverName, innerException.Message, innerException);
		}

		protected override ActiveMonitoringServerTransientException GetGenericOperationFailedTransientException(string message, Exception innerException)
		{
			return new ActiveMonitoringServerTransientException(message, innerException);
		}

		private static ActiveMonitoringRpcExceptionWrapper rpcWrapper = new ActiveMonitoringRpcExceptionWrapper();
	}
}
