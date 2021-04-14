using System;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Search.Core.Diagnostics
{
	internal interface IDiagnosticsLogConfig : IConfig
	{
		Guid EventLogComponentGuid { get; }

		bool IsEnabled { get; }

		string ServiceName { get; }

		string LogTypeName { get; }

		string LogFilePath { get; }

		string LogFilePrefix { get; }

		string LogComponent { get; }

		int MaxAge { get; }

		int MaxDirectorySize { get; }

		int MaxFileSize { get; }

		bool IncludeExtendedLogging { get; }

		int ExtendedLoggingSize { get; }

		DiagnosticsLoggingTag DiagnosticsLoggingTag { get; }
	}
}
