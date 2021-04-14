using System;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PhotoRequestLog
	{
		public PhotoRequestLog(PhotosConfiguration configuration, string logFileNamePrefix, string build)
		{
			ArgumentValidator.ThrowIfNull("configuration", configuration);
			ArgumentValidator.ThrowIfNullOrEmpty("logFileNamePrefix", logFileNamePrefix);
			ArgumentValidator.ThrowIfNullOrEmpty("build", build);
			this.build = build;
			this.log = new Log(logFileNamePrefix, new LogHeaderFormatter(PhotoRequestLog.Schema), "photos");
			this.log.Configure(configuration.PhotoRequestLoggingPath, configuration.PhotoRequestLogFileMaxAge, configuration.PhotoRequestLogDirectoryMaxSize, configuration.PhotoRequestLogFileMaxSize);
		}

		public void Log(DateTime timestamp, string requestId, string eventType, string message)
		{
			LogRowFormatter logRowFormatter = new LogRowFormatter(PhotoRequestLog.Schema, true);
			logRowFormatter[1] = this.build;
			logRowFormatter[2] = Environment.MachineName;
			logRowFormatter[3] = requestId;
			logRowFormatter[4] = eventType;
			logRowFormatter[5] = message;
			this.log.Append(logRowFormatter, 0, timestamp);
		}

		private const string LogComponentName = "photos";

		private static readonly string[] LogColumns = new string[]
		{
			"Time",
			"Build",
			"Server",
			"RequestId",
			"EventType",
			"Message"
		};

		private static readonly LogSchema Schema = new LogSchema("Microsoft Exchange Server", Assembly.GetExecutingAssembly().GetName().Version.ToString(), "Photo Request log", PhotoRequestLog.LogColumns);

		private readonly string build;

		private Log log;

		private static class LogColumnIndices
		{
			public const int Time = 0;

			public const int Build = 1;

			public const int Server = 2;

			public const int RequestId = 3;

			public const int EventType = 4;

			public const int Message = 5;
		}
	}
}
