using System;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Setup.Common
{
	internal static class SetupEventLog
	{
		internal static void LogEvent(long eventId, int categoryId, EventLogEntryType type, params object[] messageArgs)
		{
			EventInstance instance = new EventInstance(eventId, categoryId, type);
			try
			{
				SetupEventLog.setupEventLog.WriteEvent(instance, messageArgs);
			}
			catch (Win32Exception)
			{
			}
		}

		internal static void LogStartEvent(string installableUnit)
		{
			SetupEventLog.LogEvent(1000L, 1, EventLogEntryType.Information, new object[]
			{
				SetupEventLog.GetVersionAndLocalizedInstallableUnitName(installableUnit)
			});
		}

		internal static void LogSuccessEvent(string installableUnit)
		{
			SetupEventLog.LogEvent(1001L, 1, EventLogEntryType.Information, new object[]
			{
				SetupEventLog.GetVersionAndLocalizedInstallableUnitName(installableUnit)
			});
		}

		internal static void LogFailureEvent(string installableUnit, string error)
		{
			SetupEventLog.LogEvent(1002L, 1, EventLogEntryType.Error, new object[]
			{
				SetupEventLog.GetLocalizedInstallableUnitName(installableUnit),
				error
			});
		}

		private static string GetVersionAndLocalizedInstallableUnitName(string installableUnit)
		{
			return ConfigurationContext.Setup.GetExecutingVersion().ToString() + ":" + SetupEventLog.GetLocalizedInstallableUnitName(installableUnit);
		}

		private static string GetLocalizedInstallableUnitName(string installableUnit)
		{
			return InstallableUnitConfigurationInfoManager.GetInstallableUnitConfigurationInfoByName(installableUnit).DisplayName;
		}

		private static string eventSourceName = "MSExchangeSetup";

		private static EventLog setupEventLog = new EventLog("Application", ".", SetupEventLog.eventSourceName);
	}
}
