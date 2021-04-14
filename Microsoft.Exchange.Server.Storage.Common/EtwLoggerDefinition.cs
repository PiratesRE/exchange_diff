using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public struct EtwLoggerDefinition
	{
		public LoggerType LoggerType;

		public string LogFilePrefixName;

		public Guid ProviderGuid;

		public uint LogFileSizeMB;

		public uint MemoryBufferSizeKB;

		public uint MinimumNumberOfMemoryBuffers;

		public uint NumberOfMemoryBuffers;

		public uint MaximumTotalFilesSizeMB;

		public bool FileModeCreateNew;

		public TimeSpan FlushTimer;

		public TimeSpan RetentionLimit;
	}
}
