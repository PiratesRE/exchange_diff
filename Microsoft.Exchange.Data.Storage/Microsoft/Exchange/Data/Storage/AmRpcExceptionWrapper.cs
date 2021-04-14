using System;
using Microsoft.Exchange.Data.Storage.Cluster;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AmRpcExceptionWrapper : HaRpcExceptionWrapperBase<AmServerException, AmServerTransientException>
	{
		private AmRpcExceptionWrapper()
		{
		}

		public static AmRpcExceptionWrapper Instance
		{
			get
			{
				return AmRpcExceptionWrapper.s_amRpcWrapper;
			}
		}

		internal override bool IsKnownException(Exception ex)
		{
			return ex is TaskServerException || ex is TaskServerTransientException || base.IsKnownException(ex);
		}

		protected override AmServerException GetGenericOperationFailedException(string message)
		{
			return new AmOperationFailedException(message);
		}

		protected override AmServerException GetGenericOperationFailedException(string message, Exception innerException)
		{
			return new AmOperationFailedException(message, innerException);
		}

		protected override AmServerException GetGenericOperationFailedWithEcException(int errorCode)
		{
			return new AmOperationFailedWithEcException(errorCode);
		}

		protected override AmServerException GetServiceDownException(string serverName, Exception innerException)
		{
			return new AmReplayServiceDownException(serverName, innerException.Message, innerException);
		}

		protected override AmServerTransientException GetGenericOperationFailedTransientException(string message, Exception innerException)
		{
			return new AmServerTransientException(message, innerException);
		}

		private static AmRpcExceptionWrapper s_amRpcWrapper = new AmRpcExceptionWrapper();
	}
}
