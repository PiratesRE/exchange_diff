using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ADOperationRetryManager : OperationRetryManagerBase
	{
		public ADOperationRetryManager(int maxRetryCount) : base(maxRetryCount, TimeSpan.FromSeconds(1.0), true)
		{
		}

		public ADOperationRetryManager(int maxRetryCount, TimeSpan retryInterval, bool multiplyDurationByRetryIteration) : base(maxRetryCount, retryInterval, multiplyDurationByRetryIteration)
		{
		}

		protected override bool InternalRun(Action operation, bool maxRetryReached)
		{
			try
			{
				operation();
				return true;
			}
			catch (ADInvalidCredentialException ex)
			{
				if (maxRetryReached)
				{
					ExTraceGlobals.RetryManagerTracer.TraceError(0L, "ADInvalidCredentialException: Max retry count reached, throw transient exception.");
					throw new ADTransientException(ex.LocalizedString, ex);
				}
				ExTraceGlobals.RetryManagerTracer.TraceError(0L, "ADInvalidCredentialException: Will keep retrying.");
			}
			catch (TransientException arg)
			{
				if (maxRetryReached)
				{
					ExTraceGlobals.RetryManagerTracer.TraceError<TransientException>(0L, "{0}: Max retry count reached, will re-throw.", arg);
					throw;
				}
			}
			return false;
		}

		protected override OperationRetryManagerResult InternalTryRun(Action operation)
		{
			try
			{
				base.Run(operation);
			}
			catch (TransientException ex)
			{
				ExTraceGlobals.RetryManagerTracer.TraceError<TransientException>(0L, "AD operation failed with exception: {0}", ex);
				return new OperationRetryManagerResult(OperationRetryManagerResultCode.RetryableError, ex);
			}
			catch (DataSourceOperationException ex2)
			{
				ExTraceGlobals.RetryManagerTracer.TraceError<DataSourceOperationException>(0L, "AD operation failed with exception: {0}", ex2);
				return new OperationRetryManagerResult(OperationRetryManagerResultCode.PermanentError, ex2);
			}
			catch (DataValidationException ex3)
			{
				ExTraceGlobals.RetryManagerTracer.TraceError<DataValidationException>(0L, "AD operation failed with exception: {0}", ex3);
				return new OperationRetryManagerResult(OperationRetryManagerResultCode.PermanentError, ex3);
			}
			return OperationRetryManagerResult.Success;
		}
	}
}
