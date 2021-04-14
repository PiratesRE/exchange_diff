using System;
using System.ComponentModel;
using Microsoft.Exchange.Data.Storage.Cluster;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SeederRpcExceptionWrapper : HaRpcExceptionWrapperBase<SeederServerException, SeederServerTransientException>
	{
		protected SeederRpcExceptionWrapper()
		{
		}

		public static SeederRpcExceptionWrapper Instance
		{
			get
			{
				return SeederRpcExceptionWrapper.s_seederRpcWrapper;
			}
		}

		protected override SeederServerException GetGenericOperationFailedException(string message)
		{
			return new SeederOperationFailedException(message);
		}

		protected override SeederServerException GetGenericOperationFailedException(string message, Exception innerException)
		{
			return new SeederOperationFailedException(message, innerException);
		}

		protected override SeederServerException GetGenericOperationFailedWithEcException(int errorCode)
		{
			Win32Exception ex = new Win32Exception(errorCode);
			return new SeederOperationFailedWithEcException(errorCode, ex.Message);
		}

		protected override SeederServerException GetServiceDownException(string serverName, Exception innerException)
		{
			return new SeederReplayServiceDownException(serverName, innerException.Message, innerException);
		}

		protected override SeederServerTransientException GetGenericOperationFailedTransientException(string message, Exception innerException)
		{
			return new SeederServerTransientException(message, innerException);
		}

		private static SeederRpcExceptionWrapper s_seederRpcWrapper = new SeederRpcExceptionWrapper();
	}
}
