using System;

namespace Microsoft.Exchange.Inference.Common.Diagnostics
{
	public class DiagnosticLogConfig : LogConfig, IDiagnosticLogConfig, ILogConfig
	{
		public DiagnosticLogConfig(bool isLoggingEnabled, string logType, string logPrefix, string logPath, ulong? maxLogDirectorySize, ulong? maxLogFileSize, TimeSpan? maxLogAge, LoggingLevel loggingLevel) : base(isLoggingEnabled, logType, logPrefix, logPath, maxLogDirectorySize, maxLogFileSize, maxLogAge, 4096)
		{
			this.LoggingLevel = loggingLevel;
		}

		public LoggingLevel LoggingLevel { get; private set; }
	}
}
