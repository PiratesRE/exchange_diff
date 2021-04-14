using System;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Microsoft.Exchange.Diagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class DisposeTrackerOptions
	{
		static DisposeTrackerOptions()
		{
			DisposeTrackerOptions.RefreshNow();
			DisposeTrackerOptions.lastConfigRefreshTicks = (uint)Environment.TickCount;
		}

		public static bool DontStopCollecting
		{
			get
			{
				return DisposeTrackerOptions.dontStopCollecting;
			}
		}

		public static int NumberOfStackTracesToCollect
		{
			get
			{
				return DisposeTrackerOptions.numberOfStackTracesToCollect;
			}
		}

		public static int PercentageOfStackTracesToCollect
		{
			get
			{
				return DisposeTrackerOptions.percentageOfStackTracesToCollect;
			}
		}

		public static bool UseFullSymbols
		{
			get
			{
				return DisposeTrackerOptions.useFullSymbols;
			}
		}

		public static int ThrottleMilliseconds
		{
			get
			{
				return DisposeTrackerOptions.throttleMilliseconds;
			}
		}

		public static int MaximumWatsonsPerSecond
		{
			get
			{
				return DisposeTrackerOptions.maximumWatsonsPerSecond;
			}
		}

		public static bool TerminateOnReport
		{
			get
			{
				return DisposeTrackerOptions.terminateOnReport;
			}
		}

		public static bool Enabled
		{
			get
			{
				return DisposeTrackerOptions.enabled;
			}
		}

		public static bool DebugBreakOnLeak
		{
			get
			{
				return DisposeTrackerOptions.debugBreakOnLeak;
			}
		}

		public static bool CollectStackTracesForLeakDetection
		{
			get
			{
				return DisposeTrackerOptions.collectStackTracesForLeakDetection;
			}
		}

		internal static int NumberOfStackTracesToSkip
		{
			get
			{
				return DisposeTrackerOptions.numberOfStackTracesToSkip;
			}
		}

		public static void ScheduleRefreshIfNecessary()
		{
			if ((ulong)(Environment.TickCount - (int)DisposeTrackerOptions.lastConfigRefreshTicks) >= (ulong)((long)DisposeTrackerOptions.minConfigChangeMilliseconds))
			{
				Task.Factory.StartNew(new Action(DisposeTrackerOptions.OptionsThreadProc));
				DisposeTrackerOptions.lastConfigRefreshTicks = (uint)Environment.TickCount;
			}
		}

		public static void RefreshNowIfNecessary()
		{
			if ((ulong)(Environment.TickCount - (int)DisposeTrackerOptions.lastConfigRefreshTicks) >= (ulong)((long)DisposeTrackerOptions.minConfigChangeMilliseconds))
			{
				DisposeTrackerOptions.RefreshNow();
				DisposeTrackerOptions.lastConfigRefreshTicks = (uint)Environment.TickCount;
			}
		}

		public static void RefreshNow()
		{
			DisposeTrackerOptions.ReloadRegistryData();
			DisposeTrackerOptions.RefreshCalculatedProperties();
		}

		private static void OptionsThreadProc()
		{
			DisposeTrackerOptions.RefreshNow();
		}

		private static void ReloadRegistryData()
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\DisposeTrackerOptions", false))
			{
				DisposeTrackerOptions.dontStopCollecting = DisposeTrackerOptions.GetRegistryBoolean(registryKey, "DontStopCollecting", false);
				DisposeTrackerOptions.numberOfStackTracesToCollect = Math.Min(DisposeTrackerOptions.GetRegistryInt(registryKey, "NumberOfStackTracesToCollect", 0), 65535);
				DisposeTrackerOptions.percentageOfStackTracesToCollect = Math.Min(DisposeTrackerOptions.GetRegistryInt(registryKey, "PercentageOfStackTracesToCollect", 25), 100);
				DisposeTrackerOptions.useFullSymbols = DisposeTrackerOptions.GetRegistryBoolean(registryKey, "UseFullSymbols", true);
				DisposeTrackerOptions.throttleMilliseconds = Math.Min(DisposeTrackerOptions.GetRegistryInt(registryKey, "ThrottleMilliseconds", 33), 300000);
				DisposeTrackerOptions.maximumWatsonsPerSecond = Math.Min(DisposeTrackerOptions.GetRegistryInt(registryKey, "MaximumWatsonsPerSecond", 25), 1000);
				DisposeTrackerOptions.minConfigChangeMilliseconds = Math.Min(DisposeTrackerOptions.GetRegistryInt(registryKey, "MinConfigChangeMilliseconds", 30000), 3600000);
				DisposeTrackerOptions.terminateOnReport = DisposeTrackerOptions.GetRegistryBoolean(registryKey, "TerminateOnReport", false);
				DisposeTrackerOptions.debugBreakOnLeak = DisposeTrackerOptions.GetRegistryBoolean(registryKey, "DebugBreakOnLeak", false);
				DisposeTrackerOptions.collectStackTracesForLeakDetection = DisposeTrackerOptions.GetRegistryBoolean(registryKey, "CollectStackTracesForLeakDetection", false);
			}
		}

		private static bool GetRegistryBoolean(RegistryKey key, string name, bool defaultValue)
		{
			if (key == null)
			{
				return defaultValue;
			}
			object value = key.GetValue(name, defaultValue);
			if (!(value is int))
			{
				return defaultValue;
			}
			return (int)value != 0;
		}

		private static int GetRegistryInt(RegistryKey key, string name, int defaultValue)
		{
			if (key == null)
			{
				return defaultValue;
			}
			object value = key.GetValue(name, defaultValue);
			if (!(value is int))
			{
				return defaultValue;
			}
			if ((int)value >= 0)
			{
				return (int)value;
			}
			return defaultValue;
		}

		private static void RefreshCalculatedProperties()
		{
			if (DisposeTrackerOptions.percentageOfStackTracesToCollect > 0)
			{
				DisposeTrackerOptions.numberOfStackTracesToSkip = (100 - DisposeTrackerOptions.percentageOfStackTracesToCollect) / DisposeTrackerOptions.percentageOfStackTracesToCollect;
				DisposeTrackerOptions.enabled = true;
			}
			else
			{
				DisposeTrackerOptions.numberOfStackTracesToSkip = 0;
				DisposeTrackerOptions.enabled = false;
			}
			if (DisposeTrackerOptions.numberOfStackTracesToCollect == 0)
			{
				DisposeTrackerOptions.enabled = false;
			}
			if (DisposeTrackerOptions.maximumWatsonsPerSecond == 0)
			{
				DisposeTrackerOptions.enabled = false;
			}
		}

		internal const string ConfigRegistryKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\DisposeTrackerOptions";

		private const bool DefaultDontStopCollecting = false;

		private const int DefaultPercentageOfStackTracesToCollect = 25;

		private const int DefaultNumberOfStackTracesToCollect = 0;

		private const int MaxNumberOfStackTracesToCollect = 65535;

		private const bool DefaultUseFullSymbols = true;

		private const int DefaultThrottleMilliseconds = 33;

		private const int MaxThrottleMilliseconds = 300000;

		private const int DefaultMaximumWatsonsPerSecond = 25;

		private const int MaxMaximumWatsonsPerSecond = 1000;

		private const int DefaultMinConfigChangeMilliseconds = 30000;

		private const int MaxMinConfigChangeMilliseconds = 3600000;

		private const bool DefaultTerminateOnReport = false;

		private const bool DefaultDebugBreakOnLeak = false;

		private const bool DefaultCollectStackTracesForLeakDetection = false;

		private static bool dontStopCollecting;

		private static int numberOfStackTracesToCollect;

		private static int percentageOfStackTracesToCollect;

		private static int numberOfStackTracesToSkip;

		private static bool useFullSymbols;

		private static int throttleMilliseconds;

		private static int maximumWatsonsPerSecond;

		private static int minConfigChangeMilliseconds;

		private static bool enabled = true;

		private static bool terminateOnReport;

		private static uint lastConfigRefreshTicks;

		private static bool debugBreakOnLeak;

		private static bool collectStackTracesForLeakDetection;
	}
}
