using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Office.CompliancePolicy.Exchange.Dar.Diagnostics
{
	internal class ExceptionHandler
	{
		protected ExceptionHandler(Action code, ExceptionGroupHandler groupHandler, ExceptionHandlingOptions options)
		{
			this.code = code;
			this.groupHandler = groupHandler;
			this.options = options;
		}

		public static void Handle(Action code, ExceptionGroupHandler groupHandler, ExceptionHandlingOptions options)
		{
			try
			{
				new ExceptionHandler(code, groupHandler, options).Execute();
			}
			catch (AggregateException)
			{
				throw;
			}
			catch (Exception ex)
			{
				LogItem.Publish(options.ClientId, string.Format("{0}{1}", options.Operation, "UnhandledException"), ex.ToString(), options.CorrelationId, ResultSeverityLevel.Error);
				throw;
			}
		}

		private void Execute()
		{
			this.operationStartTime = DateTime.UtcNow;
			try
			{
				IL_0B:
				this.retryCount++;
				this.code();
			}
			catch (Exception ex)
			{
				this.exceptions.Add(ex);
				TimeSpan timeSpan = DateTime.UtcNow - this.operationStartTime;
				if (this.groupHandler(ex) == ExceptionAction.RetryWait && this.ShouldRetry())
				{
					TimeSpan timeSpan2 = this.options.RetrySchedule[Math.Min(this.retryCount, this.options.RetrySchedule.Length - 1)];
					bool flag = !this.options.IsTimeoutEnabled || timeSpan2 + timeSpan < this.options.OperationDuration;
					if (flag)
					{
						LogItem.Publish(this.options.ClientId, string.Format("{0}{1}", this.options.Operation, "ErrorHandling"), string.Format("Operation failed, retrying {0}/{1} in {2}, duration: {3}/{4}, error: {5}", new object[]
						{
							this.retryCount,
							this.options.MaxRetries,
							timeSpan2,
							timeSpan,
							this.options.OperationDuration,
							ex.ToString()
						}), this.options.CorrelationId, ResultSeverityLevel.Informational);
						Thread.Sleep(timeSpan2);
						goto IL_0B;
					}
					this.exceptions.Add(new TimeoutException("Operation would took longer than expected with next retry: " + this.options.OperationDuration));
				}
				LogItem.Publish(this.options.ClientId, string.Format("{0}{1}", this.options.Operation, "FatalError"), string.Format("Operation failed, retried {0}/{1}, duration: {2}/{3}, error: {4}", new object[]
				{
					this.retryCount,
					this.options.MaxRetries,
					timeSpan,
					this.options.OperationDuration,
					ex.ToString()
				}), this.options.CorrelationId, ResultSeverityLevel.Error);
				throw new AggregateException(this.exceptions);
			}
		}

		private bool ShouldRetry()
		{
			return this.options.IsRetryEnabled && this.options.RetrySchedule != null && this.options.RetrySchedule.Length > 0 && this.retryCount < this.options.MaxRetries;
		}

		private readonly Action code;

		private readonly ExceptionGroupHandler groupHandler;

		private readonly ExceptionHandlingOptions options;

		private readonly List<Exception> exceptions = new List<Exception>();

		private DateTime operationStartTime;

		private int retryCount;
	}
}
