using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace Microsoft.Exchange.Diagnostics.LatencyDetection
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PerformanceReporter
	{
		private PerformanceReporter()
		{
			this.SetLogger(WindowsErrorReportingLogger.Instance);
			if (PerformanceReportingOptions.Instance.EnableLatencyEventLogging)
			{
				this.SetLogger(new LatencyEventLogger());
			}
		}

		public static PerformanceReporter Instance
		{
			get
			{
				return PerformanceReporter.singletonInstance;
			}
		}

		public void ClearHistory()
		{
			this.checker.ClearHistory();
		}

		public void ClearThresholds()
		{
			LatencyReportingThresholdContainer.Instance.Clear();
			this.ClearHistory();
		}

		public bool HasHistory(string locationID)
		{
			if (string.IsNullOrEmpty(locationID))
			{
				throw new ArgumentNullException("locationID");
			}
			bool flag = false;
			LatencyDetectionLocation latencyDetectionLocation;
			if (this.container.Locations.TryGetValue(locationID, out latencyDetectionLocation))
			{
				foreach (object obj in Enum.GetValues(typeof(LoggingType)))
				{
					LoggingType type = (LoggingType)obj;
					BackLog backLog = latencyDetectionLocation.GetBackLog(type);
					flag = (backLog.Count > 0);
					if (flag)
					{
						break;
					}
				}
			}
			return flag;
		}

		public void SetLogger(ILatencyDetectionLogger logger)
		{
			if (logger == null)
			{
				throw new ArgumentNullException("logger");
			}
			LoggingType type = logger.Type;
			if (this.loggers.ContainsKey(type))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Can't set logger, because one is already set. You must first call RemoveLogger({0}.{1}).", new object[]
				{
					typeof(LoggingType),
					type
				}));
			}
			this.loggers.Add(type, logger);
		}

		public void SetDefaultWindowsErrorReportingLogger()
		{
			this.RemoveLogger(LoggingType.WindowsErrorReporting);
			this.SetLogger(WindowsErrorReportingLogger.Instance);
		}

		public void RemoveLogger(LoggingType type)
		{
			this.loggers.Remove(type);
		}

		public bool HasLogger(LoggingType type)
		{
			return this.loggers.ContainsKey(type);
		}

		internal void Log(LatencyDetectionContext context)
		{
			foreach (KeyValuePair<LoggingType, ILatencyDetectionLogger> keyValuePair in this.loggers)
			{
				ILatencyDetectionLogger value = keyValuePair.Value;
				LoggingType key = keyValuePair.Key;
				LatencyDetectionContext latencyDetectionContext;
				LatencyReportingThreshold threshold;
				ICollection<LatencyDetectionContext> backlog;
				bool flag = this.checker.ShouldCreateReport(context, key, out latencyDetectionContext, out threshold, out backlog);
				if (flag)
				{
					LatencyDetectionException exception = latencyDetectionContext.CreateLatencyDetectionException();
					PerformanceReporter.LogData state = new PerformanceReporter.LogData(value, threshold, latencyDetectionContext, backlog, exception);
					ThreadPool.QueueUserWorkItem(PerformanceReporter.LogReportDelegate, state);
				}
			}
		}

		private static void ReportingWorker(object state)
		{
			PerformanceReporter.LogData logData = (PerformanceReporter.LogData)state;
			logData.Logger.Log(logData.Threshold, logData.Trigger, logData.Backlog, logData.Exception);
		}

		private static readonly WaitCallback LogReportDelegate = new WaitCallback(PerformanceReporter.ReportingWorker);

		private static readonly PerformanceReporter singletonInstance = new PerformanceReporter();

		private readonly IDictionary<LoggingType, ILatencyDetectionLogger> loggers = new Dictionary<LoggingType, ILatencyDetectionLogger>(Enum.GetValues(typeof(LoggingType)).Length);

		private readonly LatencyReportingThresholdChecker checker = LatencyReportingThresholdChecker.Instance;

		private readonly LatencyReportingThresholdContainer container = LatencyReportingThresholdContainer.Instance;

		private class LogData
		{
			internal LogData(ILatencyDetectionLogger logger, LatencyReportingThreshold threshold, LatencyDetectionContext trigger, ICollection<LatencyDetectionContext> backlog, LatencyDetectionException exception)
			{
				this.logger = logger;
				this.threshold = threshold;
				this.trigger = trigger;
				this.backlog = backlog;
				this.exception = exception;
			}

			internal LatencyReportingThreshold Threshold
			{
				get
				{
					return this.threshold;
				}
			}

			internal LatencyDetectionContext Trigger
			{
				get
				{
					return this.trigger;
				}
			}

			internal ICollection<LatencyDetectionContext> Backlog
			{
				get
				{
					return this.backlog;
				}
			}

			internal ILatencyDetectionLogger Logger
			{
				get
				{
					return this.logger;
				}
			}

			internal LatencyDetectionException Exception
			{
				get
				{
					return this.exception;
				}
			}

			private LatencyReportingThreshold threshold;

			private LatencyDetectionContext trigger;

			private ICollection<LatencyDetectionContext> backlog;

			private ILatencyDetectionLogger logger;

			private LatencyDetectionException exception;
		}
	}
}
