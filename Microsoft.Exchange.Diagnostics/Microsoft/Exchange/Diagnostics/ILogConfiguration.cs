using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal interface ILogConfiguration
	{
		bool IsLoggingEnabled { get; }

		bool IsActivityEventHandler { get; }

		string LogPath { get; }

		string LogPrefix { get; }

		string LogComponent { get; }

		string LogType { get; }

		long MaxLogDirectorySizeInBytes { get; }

		long MaxLogFileSizeInBytes { get; }

		TimeSpan MaxLogAge { get; }
	}
}
