using System;
using Microsoft.Exchange.Data.Storage.Cluster;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TasksRpcExceptionWrapper : HaRpcExceptionWrapperBase<TaskServerException, TaskServerTransientException>
	{
		public static TasksRpcExceptionWrapper Instance
		{
			get
			{
				return TasksRpcExceptionWrapper.s_tasksRpcWrapper;
			}
		}

		protected TasksRpcExceptionWrapper()
		{
		}

		protected override TaskServerException GetGenericOperationFailedException(string message)
		{
			return new TaskOperationFailedException(message);
		}

		protected override TaskServerException GetGenericOperationFailedException(string message, Exception innerException)
		{
			return new TaskOperationFailedException(message, innerException);
		}

		protected override TaskServerException GetGenericOperationFailedWithEcException(int errorCode)
		{
			return new TaskOperationFailedWithEcException(errorCode);
		}

		protected override TaskServerException GetServiceDownException(string serverName, Exception innerException)
		{
			return new ReplayServiceDownException(serverName, innerException.Message, innerException);
		}

		protected override TaskServerTransientException GetGenericOperationFailedTransientException(string message, Exception innerException)
		{
			return new TaskServerTransientException(message, innerException);
		}

		private static TasksRpcExceptionWrapper s_tasksRpcWrapper = new TasksRpcExceptionWrapper();
	}
}
