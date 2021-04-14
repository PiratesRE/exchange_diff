using System;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Inference.Common.Diagnostics
{
	public class LogConfig : ILogConfig
	{
		public LogConfig(bool isLoggingEnabled, string logType, string logPrefix, string logPath, ulong? maxLogDirectorySize, ulong? maxLogFileSize, TimeSpan? maxLogAge, int logSessionLineCount = 4096)
		{
			ArgumentValidator.ThrowIfNull("logType", logType);
			ArgumentValidator.ThrowIfNull("logPrefix", logPrefix);
			ArgumentValidator.ThrowIfNull("logPath", logPath);
			ArgumentValidator.ThrowIfNegative("logSessionLineCount", logSessionLineCount);
			this.IsLoggingEnabled = isLoggingEnabled;
			this.SoftwareName = LogConfig.DefaultSoftwareName;
			this.SoftwareVersion = LogConfig.DefaultSoftwareVersion;
			this.ComponentName = LogConfig.DefaultComponentName;
			this.LogType = logType;
			this.LogPrefix = logPrefix;
			this.LogPath = logPath;
			this.MaxLogDirectorySize = ((maxLogDirectorySize != null) ? maxLogDirectorySize.Value : LogConfig.DefaultMaxLogDirectorySize);
			this.MaxLogFileSize = ((maxLogFileSize != null) ? maxLogFileSize.Value : LogConfig.DefaultMaxLogFileSize);
			this.MaxLogAge = ((maxLogAge != null) ? maxLogAge.Value : LogConfig.DefaultMaxLogAge);
			this.LogSessionLineCount = logSessionLineCount;
		}

		public bool IsLoggingEnabled { get; protected set; }

		public string SoftwareName { get; private set; }

		public string SoftwareVersion { get; private set; }

		public string ComponentName { get; private set; }

		public string LogType { get; protected set; }

		public string LogPrefix { get; protected set; }

		public string LogPath { get; protected set; }

		public ulong MaxLogDirectorySize { get; protected set; }

		public ulong MaxLogFileSize { get; protected set; }

		public TimeSpan MaxLogAge { get; protected set; }

		public int LogSessionLineCount { get; protected set; }

		public static readonly string DefaultSoftwareName = "Microsoft Exchange Server";

		public static readonly string DefaultSoftwareVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

		public static readonly string DefaultComponentName = "Inference";

		public static readonly ulong DefaultMaxLogDirectorySize = 1073741824UL;

		public static readonly ulong DefaultMaxLogFileSize = 10485760UL;

		public static readonly TimeSpan DefaultMaxLogAge = TimeSpan.FromDays(1.0);
	}
}
