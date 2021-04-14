using System;

namespace Microsoft.Exchange.Inference.Common.Diagnostics
{
	public interface ILogConfig
	{
		bool IsLoggingEnabled { get; }

		string SoftwareName { get; }

		string SoftwareVersion { get; }

		string ComponentName { get; }

		string LogType { get; }

		string LogPrefix { get; }

		string LogPath { get; }

		ulong MaxLogDirectorySize { get; }

		ulong MaxLogFileSize { get; }

		TimeSpan MaxLogAge { get; }

		int LogSessionLineCount { get; }
	}
}
