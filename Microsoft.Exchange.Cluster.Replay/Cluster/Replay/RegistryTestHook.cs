using System;
using Microsoft.Exchange.Cluster.Common.Registry;
using Microsoft.Exchange.Cluster.Replay.Dumpster;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class RegistryTestHook
	{
		internal static int TargetServerVersionOverride
		{
			get
			{
				RegistryTestHook.LoadRegistryValues();
				return RegistryTestHook.s_targetServerVersionOverride;
			}
		}

		internal static SafetyNetVersionCheckerOverrideEnum SafetyNetVersionCheckerOverride
		{
			get
			{
				RegistryTestHook.LoadRegistryValues();
				return (SafetyNetVersionCheckerOverrideEnum)RegistryTestHook.s_safetyNetVersionCheckerOverride;
			}
		}

		internal static int IncReseedDelayInSecs
		{
			get
			{
				RegistryTestHook.LoadRegistryValues();
				return RegistryTestHook.s_incReseedDelayInSecs;
			}
		}

		internal static int SeedDelayPerCallbackInMilliSeconds
		{
			get
			{
				int tempValue = 0;
				RegistryParameters.TryGetRegistryParameters("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\TestHook", delegate(IRegistryKey key)
				{
					tempValue = (int)key.GetValue("SeedDelayPerCallbackInMilliSeconds", 0);
				});
				return tempValue;
			}
		}

		internal static int SeedFailAfterProgressPercent
		{
			get
			{
				int tempValue = 0;
				RegistryParameters.TryGetRegistryParameters("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\TestHook", delegate(IRegistryKey key)
				{
					tempValue = (int)key.GetValue("SeedFailAfterProgressPercent", 0);
				});
				return tempValue;
			}
		}

		internal static bool SeedDisableTruncationCoordination
		{
			get
			{
				bool tempValue = false;
				RegistryParameters.TryGetRegistryParameters("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\TestHook", delegate(IRegistryKey key)
				{
					tempValue = ((int)key.GetValue("SeedDisableTruncationCoordination", 0) != 0);
				});
				return tempValue;
			}
		}

		private static void LoadRegistryValues()
		{
			if (RegistryTestHook.s_loadedRegistryValues)
			{
				return;
			}
			RegistryParameters.TryGetRegistryParameters("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\TestHook", delegate(IRegistryKey key)
			{
				RegistryTestHook.s_targetServerVersionOverride = (int)key.GetValue("TargetServerVersionOverride", RegistryTestHook.s_targetServerVersionOverride);
				RegistryTestHook.s_incReseedDelayInSecs = (int)key.GetValue("IncReseedDelayInSecs", RegistryTestHook.s_incReseedDelayInSecs);
				RegistryTestHook.s_safetyNetVersionCheckerOverride = (int)key.GetValue("SafetyNetVersionCheckerOverride", RegistryTestHook.s_safetyNetVersionCheckerOverride);
			});
			RegistryTestHook.s_loadedRegistryValues = true;
		}

		private const string TestHookKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\TestHook";

		private static int s_targetServerVersionOverride = 0;

		private static int s_safetyNetVersionCheckerOverride = 0;

		private static int s_incReseedDelayInSecs = 0;

		private static bool s_loadedRegistryValues;
	}
}
