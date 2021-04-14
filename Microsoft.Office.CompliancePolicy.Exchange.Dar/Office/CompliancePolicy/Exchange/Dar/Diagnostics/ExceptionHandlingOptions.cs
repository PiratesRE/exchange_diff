using System;
using Microsoft.Office.CompliancePolicy.Exchange.Dar.Utility;

namespace Microsoft.Office.CompliancePolicy.Exchange.Dar.Diagnostics
{
	internal class ExceptionHandlingOptions
	{
		public ExceptionHandlingOptions() : this(ExceptionHandlingOptions.minTimeSpan)
		{
		}

		public ExceptionHandlingOptions(TimeSpan operationDuration)
		{
			if (operationDuration < ExceptionHandlingOptions.minTimeSpan)
			{
				throw new ArgumentException("operationDuration");
			}
			this.OperationDuration = operationDuration;
			this.MaxRetries = ExceptionHandlingOptions.maxRetries;
			this.IsRetryEnabled = true;
			this.IsTimeoutEnabled = true;
			this.ClientId = "ExceptionHandler";
			this.Operation = "ExceptionHandling";
			this.CorrelationId = null;
			this.OperationTimeout = Helper.GetTimeSpanPercentage(this.OperationDuration, ExceptionHandlingOptions.timeoutPercentage);
			this.RetrySchedule = new TimeSpan[ExceptionHandlingOptions.retrySchedulePercentages.Length];
			for (int i = 0; i < ExceptionHandlingOptions.retrySchedulePercentages.Length; i++)
			{
				this.RetrySchedule[i] = Helper.GetTimeSpanPercentage(this.OperationDuration, ExceptionHandlingOptions.retrySchedulePercentages[i]);
			}
		}

		public TimeSpan[] RetrySchedule { get; set; }

		public TimeSpan OperationDuration { get; set; }

		public TimeSpan OperationTimeout { get; set; }

		public int MaxRetries { get; set; }

		public string ClientId { get; set; }

		public string CorrelationId { get; set; }

		public string Operation { get; set; }

		public bool IsTimeoutEnabled { get; set; }

		public bool IsRetryEnabled { get; set; }

		private static double[] retrySchedulePercentages = new double[]
		{
			2.0,
			4.0,
			8.0,
			20.0
		};

		private static double timeoutPercentage = 130.0;

		private static int maxRetries = 50;

		private static TimeSpan minTimeSpan = TimeSpan.FromMilliseconds(1.0);
	}
}
