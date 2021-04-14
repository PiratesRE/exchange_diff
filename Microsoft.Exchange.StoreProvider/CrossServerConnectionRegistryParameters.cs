using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Win32;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CrossServerConnectionRegistryParameters
	{
		public static bool IsCrossServerLoggingEnabled()
		{
			return CrossServerConnectionRegistryParameters.CheckBooleanValue("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\CrossServerConnectionPolicy", "EnableCrossServerConnectionLog", true);
		}

		public static bool IsCrossServerBlockEnabled()
		{
			return CrossServerConnectionRegistryParameters.CheckBooleanValue("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\CrossServerConnectionPolicy", "EnableCrossServerConnectionBlock", true);
		}

		public static bool IsCrossServerMonitoringBlockEnabled()
		{
			return CrossServerConnectionRegistryParameters.CheckBooleanValue("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\CrossServerConnectionPolicy", "EnableCrossServerMonitoringBlock", true);
		}

		public static TimeSpan GetInfoWatsonThrottlingInterval()
		{
			return CrossServerConnectionRegistryParameters.ReadTimeSpanFromRegistry("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\CrossServerConnectionPolicy", "CrossServerInfoWatsonThrottlingInterval", CrossServerConnectionRegistryParameters.defaultInfoWatsonThrottlingInterval);
		}

		public static bool TryGetClientSpecificOverrides(string clientId, CrossServerBehavior crossServerBehaviorDefaults, out CrossServerBehavior crossServerBehavior)
		{
			crossServerBehavior = null;
			string text = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\CrossServerConnectionPolicy\\ClientIdOverrides\\" + clientId;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(text))
			{
				if (registryKey != null)
				{
					bool shouldTrace = CrossServerConnectionRegistryParameters.CheckBooleanValue(text, "ShouldTrace", crossServerBehaviorDefaults.ShouldTrace);
					bool shouldLogInfoWatson = CrossServerConnectionRegistryParameters.CheckBooleanValue(text, "ShouldLogInfoWatson", crossServerBehaviorDefaults.ShouldLogInfoWatson);
					bool shouldBlock = CrossServerConnectionRegistryParameters.CheckBooleanValue(text, "ShouldBlock", crossServerBehaviorDefaults.ShouldBlock);
					crossServerBehavior = new CrossServerBehavior(clientId, crossServerBehaviorDefaults.PreExchange15, shouldTrace, shouldLogInfoWatson, shouldBlock);
				}
			}
			return crossServerBehavior != null;
		}

		public static bool ConvertBooleanRegistryValue(object registryValue, bool defaultValue)
		{
			bool result = defaultValue;
			if (registryValue is int)
			{
				switch ((int)registryValue)
				{
				case 0:
					result = false;
					break;
				case 1:
					result = true;
					break;
				}
			}
			return result;
		}

		public static TimeSpan ConvertTimeSpanRegistryValue(object registryValue, TimeSpan defaultValue)
		{
			TimeSpan result = defaultValue;
			TimeSpan timeSpan;
			if (registryValue != null && registryValue is string && TimeSpan.TryParse(registryValue as string, out timeSpan))
			{
				result = timeSpan;
			}
			return result;
		}

		private static bool CheckBooleanValue(string keyPath, string valueName, bool defaultValue)
		{
			return CrossServerConnectionRegistryParameters.ConvertBooleanRegistryValue(CrossServerConnectionRegistryParameters.ReadRegistryKey(keyPath, valueName), defaultValue);
		}

		private static TimeSpan ReadTimeSpanFromRegistry(string keyPath, string valueName, TimeSpan defaultValue)
		{
			return CrossServerConnectionRegistryParameters.ConvertTimeSpanRegistryValue(CrossServerConnectionRegistryParameters.ReadRegistryKey(keyPath, valueName), defaultValue);
		}

		private static object ReadRegistryKey(string keyPath, string valueName)
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(keyPath))
			{
				if (registryKey != null)
				{
					return registryKey.GetValue(valueName, null);
				}
			}
			return null;
		}

		public const string ExchangeServerCrossServerConnectionPolicyRegistryKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\CrossServerConnectionPolicy";

		public const string ExchangeServerClientIdOverridesRegistryKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\CrossServerConnectionPolicy\\ClientIdOverrides";

		public const string EnableCrossServerLogRegistryValue = "EnableCrossServerConnectionLog";

		public const string EnableCrossServerBlockRegistryValue = "EnableCrossServerConnectionBlock";

		public const string EnableCrossServerMonitoringBlockRegistryValue = "EnableCrossServerMonitoringBlock";

		public const string CrossServerInfoWatsonThrottlingIntervalRegistryValue = "CrossServerInfoWatsonThrottlingInterval";

		public const string ShouldTraceRegistryValue = "ShouldTrace";

		public const string ShouldLogInfoWatsonRegistryValue = "ShouldLogInfoWatson";

		public const string ShouldBlockRegistryValue = "ShouldBlock";

		private static TimeSpan defaultInfoWatsonThrottlingInterval = TimeSpan.FromDays(7.0);
	}
}
