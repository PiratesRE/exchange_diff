using System;

namespace Microsoft.Exchange.Diagnostics.CmdletInfra
{
	internal static class LoggerSettings
	{
		internal static bool IsInitialized { get; set; }

		internal static bool LogEnabled { get; set; } = true;

		internal static bool IsPowerShellWebService { get; set; }

		internal static bool IsRemotePS { get; set; }

		internal static int ThresholdToLogActivityLatency { get; set; } = 1000;

		internal static string LogSubFolderName { get; set; } = "Others";

		internal static TimeSpan LogFileAgeInDays { get; set; } = TimeSpan.FromDays(30.0);

		internal static int MaxLogDirectorySizeInGB { get; set; } = 1;

		internal static int MaxLogFileSizeInMB { get; set; } = 10;

		internal static ExEventLog EventLog { get; set; }

		internal static ExEventLog.EventTuple EventTuple { get; set; }

		internal static string ProcessName { get; set; }

		internal static int ProcessId { get; set; }

		internal static int? MaxAppendableColumnLength { get; set; } = new int?(16384);

		internal static int? ErrorMessageLengthThreshold { get; set; } = new int?(250);

		internal static bool ProcessExceptionMessage { get; set; } = true;

		internal const string DefaultLogSubFolderName = "Others";

		internal const string CustomLogFolderPathAppSettingsKey = "ConfigurationCoreLogger.LogFolder";
	}
}
