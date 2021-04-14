using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Diagnostics.LatencyDetection
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class WindowsErrorReportingLogger : ILatencyDetectionLogger
	{
		private WindowsErrorReportingLogger()
		{
		}

		public LoggingType Type
		{
			get
			{
				return LoggingType.WindowsErrorReporting;
			}
		}

		internal static ILatencyDetectionLogger Instance
		{
			get
			{
				return WindowsErrorReportingLogger.singletonInstance;
			}
		}

		public void Log(LatencyReportingThreshold threshold, LatencyDetectionContext trigger, ICollection<LatencyDetectionContext> context, LatencyDetectionException exception)
		{
			if (threshold == null)
			{
				throw new ArgumentNullException("threshold");
			}
			if (trigger == null)
			{
				throw new ArgumentNullException("trigger");
			}
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			DateTime utcNow = DateTime.UtcNow;
			if (ExWatson.LastWatsonReport + TimeSpan.FromMinutes(1.0) < utcNow && this.lastReport + PerformanceReportingOptions.Instance.WatsonThrottle < utcNow)
			{
				this.lastReport = DateTime.UtcNow;
				WindowsErrorReportingLogger.CreateReport(threshold, trigger, context, exception);
			}
		}

		private static void CreateReport(LatencyReportingThreshold threshold, LatencyDetectionContext trigger, ICollection<LatencyDetectionContext> dataToLog, LatencyDetectionException exception)
		{
			StringBuilder stringBuilder = new StringBuilder(Math.Min(LatencyDetectionContext.EstimatedStringCapacity * (dataToLog.Count + 1), 42000));
			stringBuilder.Append("Latency Threshold: ").Append(threshold.Threshold.TotalMilliseconds).AppendLine(" ms");
			stringBuilder.AppendLine("Trigger").AppendLine(trigger.ToString());
			if (dataToLog.Count > 0)
			{
				stringBuilder.Append(dataToLog.Count).AppendLine(" Backlog Entries");
				foreach (LatencyDetectionContext latencyDetectionContext in dataToLog)
				{
					stringBuilder.AppendLine(latencyDetectionContext.ToString());
				}
			}
			stringBuilder.AppendLine(exception.StackTrace);
			string text = trigger.Version;
			if (string.IsNullOrEmpty(text))
			{
				text = "00.00.0000.000";
			}
			string callstack = (trigger.StackTraceContext ?? string.Empty) + Environment.NewLine + exception.StackTrace;
			ExWatson.SendLatencyWatsonReport(text, trigger.Location.Identity, exception.WatsonExceptionName, callstack, exception.WatsonMethodName, stringBuilder.ToString());
		}

		private static readonly ILatencyDetectionLogger singletonInstance = new WindowsErrorReportingLogger();

		private DateTime lastReport = DateTime.MinValue;
	}
}
