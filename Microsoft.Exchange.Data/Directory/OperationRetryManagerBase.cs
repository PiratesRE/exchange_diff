using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class OperationRetryManagerBase : IOperationRetryManager
	{
		public OperationRetryManagerBase(int maxRetryCount) : this(maxRetryCount, TimeSpan.FromSeconds(1.0), true)
		{
		}

		public OperationRetryManagerBase(int maxRetryCount, TimeSpan retryInterval, bool multiplyDurationByRetryIteration)
		{
			if (maxRetryCount < 0)
			{
				throw new ArgumentOutOfRangeException("maxRetryCount", "maxRetryCount must be greater than or equal to 0.");
			}
			this.maxRetryCount = maxRetryCount;
			this.retryInterval = retryInterval;
			this.multiplyDurationByRetryIteration = multiplyDurationByRetryIteration;
		}

		public void Run(Action operation)
		{
			if (operation == null)
			{
				throw new ArgumentNullException("operation");
			}
			for (int i = 0; i <= this.maxRetryCount; i++)
			{
				if (this.InternalRun(operation, i == this.maxRetryCount))
				{
					return;
				}
				TimeSpan duration;
				if (this.multiplyDurationByRetryIteration)
				{
					duration = TimeSpan.FromMilliseconds((double)(i * (int)this.retryInterval.TotalMilliseconds));
				}
				else
				{
					duration = ((i == 0) ? TimeSpan.Zero : this.retryInterval);
				}
				this.Sleep(duration);
			}
		}

		public OperationRetryManagerResult TryRun(Action operation)
		{
			if (operation == null)
			{
				throw new ArgumentNullException("operation");
			}
			return this.InternalTryRun(operation);
		}

		protected abstract bool InternalRun(Action operation, bool maxRetryReached);

		protected abstract OperationRetryManagerResult InternalTryRun(Action operation);

		protected virtual void Sleep(TimeSpan duration)
		{
			Thread.Sleep(duration);
		}

		private readonly TimeSpan retryInterval;

		private readonly int maxRetryCount;

		private readonly bool multiplyDurationByRetryIteration;
	}
}
