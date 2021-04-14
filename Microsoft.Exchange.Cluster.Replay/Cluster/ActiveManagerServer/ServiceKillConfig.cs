using System;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class ServiceKillConfig
	{
		internal string ServiceName { get; set; }

		internal DateTime LastKillTime { get; set; }

		internal bool IsDisabled { get; set; }

		internal TimeSpan DurationBetweenKill { get; set; }

		internal ServiceKillConfig(string serviceName)
		{
			this.ServiceName = serviceName;
		}

		internal static void WithKey(string serviceName, bool isWritable, Action<RegistryKey> operation)
		{
			string text = string.Format("{0}\\{1}", "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters\\ServiceKill", serviceName);
			using (RegistryKey registryKey = isWritable ? Registry.LocalMachine.CreateSubKey(text) : Registry.LocalMachine.OpenSubKey(text))
			{
				if (registryKey != null)
				{
					operation(registryKey);
				}
			}
		}

		internal static T GetValue<T>(RegistryKey key, string propertyName, T defaultValue)
		{
			T result = defaultValue;
			if (key != null)
			{
				result = (T)((object)key.GetValue(propertyName, defaultValue));
			}
			return result;
		}

		internal static ServiceKillConfig Read(string serviceName)
		{
			ServiceKillConfig skc = new ServiceKillConfig(serviceName);
			ServiceKillConfig.WithKey(serviceName, false, delegate(RegistryKey key)
			{
				string value = ServiceKillConfig.GetValue<string>(key, "LastKillTime", null);
				if (value != null)
				{
					skc.LastKillTime = DateTime.Parse(value);
				}
				skc.IsDisabled = (ServiceKillConfig.GetValue<int>(key, "IsDisabled", 0) > 0);
				skc.DurationBetweenKill = TimeSpan.FromSeconds((double)ServiceKillConfig.GetValue<int>(key, "DurationBetweenKillInSec", 14400));
			});
			return skc;
		}

		internal void UpdateKillTime(DateTime killTime)
		{
			this.LastKillTime = killTime;
			ServiceKillConfig.WithKey(this.ServiceName, true, delegate(RegistryKey key)
			{
				string value = killTime.ToString("o");
				key.SetValue("LastKillTime", value);
			});
		}

		internal const string ConfigKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters\\ServiceKill";

		internal const string LastKillTimeProperty = "LastKillTime";

		internal const string IsDisabledProperty = "IsDisabled";

		internal const string DurationBetweenKillProperty = "DurationBetweenKillInSec";

		internal const int DefaultMinimumDurationBetweenKillInSec = 14400;
	}
}
