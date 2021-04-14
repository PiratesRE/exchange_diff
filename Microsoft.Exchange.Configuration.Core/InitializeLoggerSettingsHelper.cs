using System;
using Microsoft.Exchange.Configuration.Core.EventLog;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Configuration.Core
{
	internal static class InitializeLoggerSettingsHelper
	{
		public static void InitLoggerSettings()
		{
			if (!LoggerSettings.IsInitialized)
			{
				LoggerSettings.LogEnabled = AppSettings.Current.LogEnabled;
				LoggerSettings.IsPowerShellWebService = Constants.IsPowerShellWebService;
				LoggerSettings.IsRemotePS = Constants.IsRemotePS;
				LoggerSettings.ThresholdToLogActivityLatency = AppSettings.Current.ThresholdToLogActivityLatency;
				LoggerSettings.LogSubFolderName = AppSettings.Current.LogSubFolderName;
				LoggerSettings.LogFileAgeInDays = AppSettings.Current.LogFileAgeInDays;
				LoggerSettings.MaxLogDirectorySizeInGB = AppSettings.Current.MaxLogDirectorySizeInGB;
				LoggerSettings.MaxLogFileSizeInMB = AppSettings.Current.MaxLogFileSizeInMB;
				LoggerSettings.EventLog = Constants.CoreEventLogger;
				LoggerSettings.EventTuple = TaskEventLogConstants.Tuple_UnhandledException;
				LoggerSettings.ProcessName = Constants.ProcessName;
				LoggerSettings.ProcessId = Constants.ProcessId;
				LoggerSettings.IsInitialized = true;
			}
		}
	}
}
