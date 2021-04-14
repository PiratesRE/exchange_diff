using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Exchange.Diagnostics
{
	internal class RequestLoggerConfig
	{
		public RequestLoggerConfig(string logType, string logFilePrefix, string logComponent, string folderPathAppSettingsKey, string fallbackLogFolderPath, Enum genericInfoColumn, List<KeyValuePair<string, Enum>> columns, int defaultLatencyDictionarySize) : this(logType, logFilePrefix, logComponent, folderPathAppSettingsKey, fallbackLogFolderPath, TimeSpan.FromDays(30.0), 5368709120L, 10485760L, genericInfoColumn, columns, defaultLatencyDictionarySize)
		{
		}

		public RequestLoggerConfig(string logType, string logFilePrefix, string logComponent, string folderPathAppSettingsKey, string fallbackLogFolderPath, TimeSpan maxAge, long maxDirectorySize, long maxLogFileSize, Enum genericInfoColumn, List<KeyValuePair<string, Enum>> columns, int defaultLatencyDictionarySize)
		{
			this.LogType = logType;
			this.LogFilePrefix = logFilePrefix;
			this.LogComponent = logComponent;
			this.FolderPathAppSettingsKey = folderPathAppSettingsKey;
			this.FallbackLogFolderPath = fallbackLogFolderPath;
			this.MaxAge = maxAge;
			this.MaxDirectorySize = maxDirectorySize;
			this.MaxLogFileSize = maxLogFileSize;
			this.GenericInfoColumn = genericInfoColumn;
			this.Columns = new ReadOnlyCollection<KeyValuePair<string, Enum>>(new List<KeyValuePair<string, Enum>>(columns));
			this.DefaultLatencyDictionarySize = defaultLatencyDictionarySize;
		}

		public string LogType { get; private set; }

		public string LogFilePrefix { get; private set; }

		public string LogComponent { get; private set; }

		public string FolderPathAppSettingsKey { get; private set; }

		public string FallbackLogFolderPath { get; private set; }

		public TimeSpan MaxAge { get; private set; }

		public long MaxDirectorySize { get; private set; }

		public long MaxLogFileSize { get; private set; }

		public Enum GenericInfoColumn { get; private set; }

		public ReadOnlyCollection<KeyValuePair<string, Enum>> Columns { get; private set; }

		public int DefaultLatencyDictionarySize { get; private set; }
	}
}
