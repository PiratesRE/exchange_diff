using System;
using System.CodeDom.Compiler;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Assistants
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	public interface ICalendarRepairLoggerSettings : ISettings
	{
		bool InsightLogEnabled { get; }

		string InsightLogDirectoryName { get; }

		TimeSpan InsightLogFileAgeInDays { get; }

		ulong InsightLogDirectorySizeLimit { get; }

		ulong InsightLogFileSize { get; }

		ulong InsightLogCacheSize { get; }

		TimeSpan InsightLogFlushIntervalInSeconds { get; }
	}
}
