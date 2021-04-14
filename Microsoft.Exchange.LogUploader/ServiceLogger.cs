using System;
using System.Configuration;
using System.Threading;
using Microsoft.Exchange.LogUploaderProxy;
using Microsoft.Office.Compliance.Audit.Common;

namespace Microsoft.Exchange.LogUploader
{
	internal class ServiceLogger : DisposableBase
	{
		public static TimeSpan MaximumLogAge { get; private set; }

		public static long MaximumLogDirectorySize { get; private set; }

		public static long MaximumLogFileSize { get; private set; }

		public static string FileLocation { get; private set; }

		public static ServiceLogger.LogLevel ServiceLogLevel { get; private set; }

		public static void ReadConfiguration()
		{
			string fileLocation = "d:\\MessageTracingServiceLogs";
			ServiceLogger.ServiceLogLevel = ServiceLogger.LogLevel.Error;
			ServiceLogger.MaximumLogAge = TimeSpan.Parse("5.00:00:00");
			ServiceLogger.MaximumLogDirectorySize = 500000000L;
			ServiceLogger.MaximumLogFileSize = 5000000L;
			try
			{
				fileLocation = ConfigurationManager.AppSettings["LogFilePath"].Trim();
				ServiceLogger.ServiceLogLevel = (ServiceLogger.LogLevel)Convert.ToInt32(ConfigurationManager.AppSettings["LogLevel"]);
				ServiceLogger.MaximumLogAge = TimeSpan.Parse(ConfigurationManager.AppSettings["LogFileMaximumLogAge"]);
				ServiceLogger.MaximumLogDirectorySize = Convert.ToInt64(ConfigurationManager.AppSettings["LogFileMaximumLogDirectorySize"]);
				ServiceLogger.MaximumLogFileSize = Convert.ToInt64(ConfigurationManager.AppSettings["LogFileMaximumLogFileSize"]);
			}
			catch (Exception ex)
			{
				string text = string.Format("Fail to read config value, default values are used. The error is {0}", ex.ToString());
				EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_ConfigSettingNotFound, Thread.CurrentThread.Name, new object[]
				{
					text
				});
				EventNotificationItem.Publish(ExchangeComponent.Name, "BadServiceLoggerConfig", null, text, ResultSeverityLevel.Error, false);
				if (RetryHelper.IsSystemFatal(ex))
				{
					throw;
				}
			}
			finally
			{
				ServiceLogger.FileLocation = fileLocation;
			}
		}

		public static void Initialize(ILogWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			ServiceLogger.serviceLog = writer;
		}

		public static void LogDebug(ServiceLogger.Component componentName, LogUploaderEventLogConstants.Message message, string customData = "", string logFileType = "", string logFilePath = "")
		{
			ServiceLogger.Log(ServiceLogger.LogLevel.Debug, componentName, message, customData, logFileType, logFilePath);
		}

		public static void LogInfo(ServiceLogger.Component componentName, LogUploaderEventLogConstants.Message message, string customData = "", string logFileType = "", string logFilePath = "")
		{
			ServiceLogger.Log(ServiceLogger.LogLevel.Info, componentName, message, customData, logFileType, logFilePath);
		}

		public static void LogWarning(ServiceLogger.Component componentName, LogUploaderEventLogConstants.Message message, string customData = "", string logFileType = "", string logFilePath = "")
		{
			ServiceLogger.Log(ServiceLogger.LogLevel.Warning, componentName, message, customData, logFileType, logFilePath);
		}

		public static void LogError(ServiceLogger.Component componentName, LogUploaderEventLogConstants.Message message, string customData = "", string logFileType = "", string logFilePath = "")
		{
			ServiceLogger.Log(ServiceLogger.LogLevel.Error, componentName, message, customData, logFileType, logFilePath);
		}

		public static void Log(ServiceLogger.LogLevel logLevel, ServiceLogger.Component componentName, LogUploaderEventLogConstants.Message message, string customData, string logFileType, string logFilePath)
		{
			ServiceLogger.LogCommon(logLevel, message.ToString(), customData, componentName, logFileType, logFilePath);
		}

		public static void LogCommon(ServiceLogger.LogLevel logLevel, string message, string customData, ServiceLogger.Component componentName = ServiceLogger.Component.None, string logFileType = "", string logFilePath = "")
		{
			if (ServiceLogger.serviceLog == null)
			{
				ServiceLogger.InitializeMtrtLog();
			}
			if (ServiceLogger.ServiceLogLevel == ServiceLogger.LogLevel.None || logLevel < ServiceLogger.ServiceLogLevel)
			{
				return;
			}
			LogRowFormatter logRowFormatter = new LogRowFormatter(ServiceLogger.MessageTracingServiceLogSchema);
			logRowFormatter[1] = logLevel.ToString();
			if (!string.IsNullOrEmpty(logFileType))
			{
				string[] array = logFileType.Split(new char[]
				{
					'_'
				});
				logRowFormatter[2] = array[0];
			}
			if (!string.IsNullOrEmpty(logFilePath))
			{
				logRowFormatter[3] = logFilePath;
			}
			logRowFormatter[4] = componentName.ToString();
			logRowFormatter[5] = message;
			if (!string.IsNullOrEmpty(customData))
			{
				logRowFormatter[6] = customData;
			}
			ServiceLogger.serviceLog.Append(logRowFormatter, 0);
		}

		protected override IDisposable InternalGetDisposeTracker()
		{
			return DisposeTrackerFactory.Get<ServiceLogger>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && ServiceLogger.serviceLog != null)
			{
				ServiceLogger.serviceLog.Close();
			}
		}

		private static void InitializeMtrtLog()
		{
			try
			{
				ServiceLogger.ReadConfiguration();
			}
			finally
			{
				Log log = new Log("MessageTracingService_", new LogHeaderFormatter(ServiceLogger.MessageTracingServiceLogSchema), "Microsoft.ForeFront.Hygiene.MessageTracing");
				log.Configure(ServiceLogger.FileLocation, ServiceLogger.MaximumLogAge, ServiceLogger.MaximumLogDirectorySize, ServiceLogger.MaximumLogFileSize, false, 0, TimeSpan.MaxValue, LogFileRollOver.Hourly);
				ServiceLogger.Initialize(log);
			}
		}

		private const string FormattedLogPrefix = "MessageTracingService";

		private const string Version = "1.0";

		private const string LogComponentName = "Microsoft.ForeFront.Hygiene.MessageTracing";

		private static readonly string[] Fields = new string[]
		{
			ServiceLogger.MessageTracingServiceLogFields.Timestamp.ToString(),
			ServiceLogger.MessageTracingServiceLogFields.LogLevel.ToString(),
			ServiceLogger.MessageTracingServiceLogFields.LogFileType.ToString(),
			ServiceLogger.MessageTracingServiceLogFields.LogFilePath.ToString(),
			ServiceLogger.MessageTracingServiceLogFields.ComponentName.ToString(),
			ServiceLogger.MessageTracingServiceLogFields.Message.ToString(),
			ServiceLogger.MessageTracingServiceLogFields.CustomData.ToString()
		};

		private static readonly LogSchema MessageTracingServiceLogSchema = new LogSchema("Microsoft.ForeFront.Hygiene.MessageTracing", "1.0", "Message Tracing Service Log", ServiceLogger.Fields);

		private static ILogWriter serviceLog;

		public enum LogLevel
		{
			None,
			Debug,
			Info,
			Warning,
			Error
		}

		public enum Component
		{
			ADConfigReader,
			AsyncQueueDBWriter,
			DatabaseWriter,
			DualWriteTenantSettingBatchDBWriter,
			GlobalLocationService,
			InputBuffer,
			LogDataBatch,
			LogFileInfo,
			LogMonitor,
			LogParser,
			LogReader,
			Message,
			MessageBatchDBWriter,
			PersistentStoreDetails,
			SpamDigestDBWriter,
			SplitLogMonitor,
			System,
			TenantSettingBatchDBWriter,
			TransferRawDataToFFOFileWriter,
			TransportQueueLogDatabaseWriter,
			TransportQueueLogDataBatch,
			WatermarkFile,
			WebServiceWriter,
			SpamEngineOpticsWriter,
			None
		}

		public enum MessageTracingServiceLogFields
		{
			Timestamp,
			LogLevel,
			LogFileType,
			LogFilePath,
			ComponentName,
			Message,
			CustomData
		}
	}
}
