using System;
using System.Collections.Specialized;
using System.Configuration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Monitoring;

namespace Microsoft.Exchange.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class MonitoringHelper
	{
		internal static bool RunningInMonitoringService
		{
			get
			{
				return MonitoringHelper.runningInMonitoringService;
			}
			set
			{
				MonitoringHelper.runningInMonitoringService = value;
			}
		}

		static MonitoringHelper()
		{
			try
			{
				MonitoringHelper.appSettingsCollection = ConfigurationManager.AppSettings;
			}
			catch (ConfigurationErrorsException arg)
			{
				ExTraceGlobals.MonitoringHelperTracer.TraceError<ConfigurationErrorsException>(0L, "Error trying to get the appSettings secion of the configuration file. Error: {0}.", arg);
				MonitoringHelper.appSettingsCollection = new NameValueCollection();
			}
			MonitoringHelper.reportFilteredExceptions = MonitoringHelper.ReadBoolAppSettingKey("ReportFilteredExceptions", false);
		}

		internal static int ReadIntAppSettingKey(string key, int min, int max, int defaultValue)
		{
			int num = defaultValue;
			string text = MonitoringHelper.appSettingsCollection[key];
			if (string.IsNullOrEmpty(text))
			{
				ExTraceGlobals.MonitoringHelperTracer.TraceDebug<string>(0L, "The appSetting value for key '{0}' is null or empty, using the default value.", key);
			}
			else if (!int.TryParse(text, out num))
			{
				ExTraceGlobals.MonitoringHelperTracer.TraceDebug<string>(0L, "The appSetting value for key '{0}', '{1}', cannot be converted to int; using the default value.", key);
				num = defaultValue;
			}
			if (num < min || num > max)
			{
				ExTraceGlobals.MonitoringHelperTracer.TraceDebug<string, int, int>(0L, "The appSetting value for key '{0}' is out of the valid range [{1}, {2}]", key, min, max);
				num = defaultValue;
			}
			ExTraceGlobals.MonitoringHelperTracer.TraceDebug<int, string>(0L, "Using {0} as the value for the appSetting key '{1}'.", num, key);
			return num;
		}

		internal static bool ReadBoolAppSettingKey(string key, bool defaultValue)
		{
			bool flag = defaultValue;
			string value = MonitoringHelper.appSettingsCollection[key];
			if (string.IsNullOrEmpty(value))
			{
				ExTraceGlobals.MonitoringHelperTracer.TraceDebug<string>(0L, "The appSetting value for key '{0}' is null or empty, using the default value.", key);
			}
			else if (!bool.TryParse(value, out flag))
			{
				ExTraceGlobals.MonitoringHelperTracer.TraceDebug<string>(0L, "The appSetting value for key '{0}', '{1}', cannot be converted to bool; using the default value.", key);
				flag = defaultValue;
			}
			ExTraceGlobals.MonitoringHelperTracer.TraceDebug<bool, string>(0L, "Using {0} as the value for the appSetting key '{1}'.", flag, key);
			return flag;
		}

		internal static string ReadStringAppSettingKey(string key, string defaultValue)
		{
			string text = defaultValue;
			string text2 = MonitoringHelper.appSettingsCollection[key];
			if (string.IsNullOrEmpty(text2) || string.IsNullOrEmpty(text2.Trim()))
			{
				ExTraceGlobals.MonitoringHelperTracer.TraceDebug<string>(0L, "The appSetting value for key '{0}' is null or empty, using the default value.", key);
			}
			else
			{
				text = text2.Trim();
			}
			ExTraceGlobals.MonitoringHelperTracer.TraceDebug<string, string>(0L, "Using {0} as the value for the appSetting key '{1}'.", text, key);
			return text;
		}

		internal static bool IsKnownExceptionForMonitoring(Exception e)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			ExTraceGlobals.MonitoringHelperTracer.TraceDebug<Exception, bool, bool>(0L, "IsKnownExceptionForMonitoring was called to check if exception {0} is known in the current context (runningInMonitoringService = {1}, reportFilteredExceptions = {2})", e, MonitoringHelper.runningInMonitoringService, MonitoringHelper.reportFilteredExceptions);
			bool flag = MonitoringHelper.runningInMonitoringService && !MonitoringHelper.reportFilteredExceptions && !(e is AccessViolationException) && !(e is DataMisalignedException) && !(e is TypeLoadException) && !(e is TypeInitializationException) && !(e is EntryPointNotFoundException) && !(e is InsufficientMemoryException) && !(e is OutOfMemoryException) && !(e is BadImageFormatException) && !(e is StackOverflowException) && !(e is InvalidProgramException);
			ExTraceGlobals.MonitoringHelperTracer.TraceDebug<bool>(0L, "IsKnownExceptionForMonitoring result = {0}", flag);
			return flag;
		}

		private static NameValueCollection appSettingsCollection;

		private static bool reportFilteredExceptions;

		private static bool runningInMonitoringService;

		internal static class Config
		{
			internal static class RaiseCannotUnloadAppDomain
			{
				internal const string Key = "RaiseCannotUnloadAppDomain";

				internal const bool Default = false;
			}

			internal static class DelayBeforeAppDomainUnloadSeconds
			{
				internal const string Key = "DelayBeforeAppDomainUnloadSeconds";

				internal const int Min = 0;

				internal const int Max = 2147483647;

				internal const int Default = 180;
			}

			internal static class TestUserCacheRefreshMinutes
			{
				internal const string Key = "TestUserCacheRefreshMinutes";

				internal const int Min = 5;

				internal const int Max = 1440;

				internal const int Default = 60;
			}

			internal static class ReportFilteredExceptions
			{
				internal const string Key = "ReportFilteredExceptions";

				internal const bool Default = false;
			}

			internal static class IncludeStackTraceInReportError
			{
				internal const string Key = "IncludeStackTraceInReportError";

				internal const bool Default = false;
			}

			internal static class EnableLogging
			{
				internal const string Key = "EnableLogging";

				internal const bool Default = true;
			}

			internal static class MaxLogDays
			{
				internal const string Key = "MaxLogDays";

				internal const int Min = 0;

				internal const int Max = 2147483647;

				internal const int Default = 7;
			}

			internal static class LogVerbose
			{
				internal const string Key = "LogVerbose";

				internal const bool Default = false;
			}
		}
	}
}
