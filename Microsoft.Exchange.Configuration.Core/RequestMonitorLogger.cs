using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.Core
{
	internal class RequestMonitorLogger
	{
		internal RequestMonitorLogger(string logFolderPath = null)
		{
			this.LogFolderPath = logFolderPath;
			this.InitializeLogger();
		}

		internal string LogFolderPath { get; set; }

		internal void InitializeLogger()
		{
			string[] names = Enum.GetNames(typeof(RequestMonitorMetadata));
			this.logSchema = new LogSchema("Microsoft Exchange Server", "15.00.1497.015", "Request Monitor Logs", names);
			this.log = new Log("Request_Monitor_", new LogHeaderFormatter(this.logSchema, true), "RequestMonitor");
			string logFolderPath = this.LogFolderPath;
			this.log.Configure(logFolderPath, RequestMonitorLogger.maxAge, (long)(RequestMonitorLogger.maxDirectorySize * 1024 * 1024 * 1024), (long)(RequestMonitorLogger.maxLogFileSize * 1024 * 1024), true);
		}

		internal void Commit(RequestMonitorContext context)
		{
			if (context == null)
			{
				return;
			}
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.logSchema);
			for (int i = 0; i < context.Fields.Length; i++)
			{
				logRowFormatter[i] = context[i];
			}
			this.log.Append(logRowFormatter, -1);
		}

		private const string LogType = "Request Monitor Logs";

		private const string LogFilePrefix = "Request_Monitor_";

		private const string LogComponent = "RequestMonitor";

		private static TimeSpan maxAge = TimeSpan.FromDays(30.0);

		private static int maxDirectorySize = 1;

		private static int maxLogFileSize = 10;

		private Log log;

		private LogSchema logSchema;
	}
}
