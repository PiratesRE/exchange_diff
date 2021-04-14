using System;
using System.Diagnostics;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AnchorService
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class TimedOperationRunner
	{
		public TimedOperationRunner(ILogger logger, TimeSpan slowOperationThreshold)
		{
			this.logger = logger;
			this.slowOperationThreshold = slowOperationThreshold;
		}

		public TResult RunOperation<TResult>(Func<TResult> operation, object debugInfo)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			TResult result;
			try
			{
				result = operation();
			}
			finally
			{
				stopwatch.Stop();
				if (this.slowOperationThreshold < stopwatch.Elapsed)
				{
					this.logger.Log(MigrationEventType.Error, "SLOW Operation: took {0}s using '{1}' stack trace {2}", new object[]
					{
						stopwatch.Elapsed.Seconds,
						debugInfo,
						AnchorUtil.GetCurrentStackTrace()
					});
				}
				else
				{
					this.logger.Log(MigrationEventType.Instrumentation, "Operation: took {0} using '{1}'", new object[]
					{
						stopwatch.Elapsed,
						debugInfo
					});
				}
			}
			return result;
		}

		private readonly ILogger logger;

		private readonly TimeSpan slowOperationThreshold;
	}
}
