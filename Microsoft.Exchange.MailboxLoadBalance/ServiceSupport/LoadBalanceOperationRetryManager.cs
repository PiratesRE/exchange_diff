using System;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Config;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.MailboxLoadBalance.ServiceSupport
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LoadBalanceOperationRetryManager : OperationRetryManagerBase
	{
		private LoadBalanceOperationRetryManager(int retryCount, TimeSpan retryInterval, ILogger logger) : base(retryCount, retryInterval, true)
		{
			this.logger = logger;
		}

		public static IOperationRetryManager Create(int retryCount, TimeSpan retryInterval, ILogger logger)
		{
			return new LoadBalanceOperationRetryManager(retryCount, retryInterval, logger);
		}

		public static IOperationRetryManager Create(ILogger logger)
		{
			ILoadBalanceSettings value = LoadBalanceADSettings.Instance.Value;
			return LoadBalanceOperationRetryManager.Create(value.TransientFailureMaxRetryCount, value.TransientFailureRetryDelay, logger);
		}

		protected override bool InternalRun(Action operation, bool maxRetryReached)
		{
			bool success = true;
			CommonUtils.ProcessKnownExceptionsWithoutTracing(operation, delegate(Exception exception)
			{
				this.logger.Log(MigrationEventType.Verbose, exception, "Error running operation {0}. Last retry: {1}.", new object[]
				{
					operation.Method.Name,
					maxRetryReached
				});
				if (!CommonUtils.IsTransientException(exception))
				{
					return false;
				}
				success = false;
				return !maxRetryReached;
			});
			return success;
		}

		protected override OperationRetryManagerResult InternalTryRun(Action operation)
		{
			OperationRetryManagerResult result = OperationRetryManagerResult.Success;
			Action operationToRun = operation;
			CommonUtils.ProcessKnownExceptionsWithoutTracing(delegate
			{
				this.Run(operationToRun);
			}, delegate(Exception exception)
			{
				this.logger.Log(MigrationEventType.Verbose, exception, "Error running operation {0}.", new object[]
				{
					operation.Method.Name
				});
				OperationRetryManagerResultCode resultCode = CommonUtils.IsTransientException(exception) ? OperationRetryManagerResultCode.RetryableError : OperationRetryManagerResultCode.PermanentError;
				result = new OperationRetryManagerResult(resultCode, exception);
				return true;
			});
			return result;
		}

		private readonly ILogger logger;
	}
}
