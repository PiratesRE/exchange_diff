using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Internal
{
	internal static class ApplicationServices
	{
		public static IApplicationServices Provider
		{
			get
			{
				return ApplicationServices.provider;
			}
		}

		public static CtsConfigurationSetting GetSimpleConfigurationSetting(string subSectionName, string settingName)
		{
			CtsConfigurationSetting ctsConfigurationSetting = null;
			IList<CtsConfigurationSetting> configuration = ApplicationServices.Provider.GetConfiguration(subSectionName);
			foreach (CtsConfigurationSetting ctsConfigurationSetting2 in configuration)
			{
				if (string.Equals(ctsConfigurationSetting2.Name, settingName, StringComparison.OrdinalIgnoreCase))
				{
					if (ctsConfigurationSetting != null)
					{
						ApplicationServices.Provider.LogConfigurationErrorEvent();
						break;
					}
					ctsConfigurationSetting = ctsConfigurationSetting2;
				}
			}
			return ctsConfigurationSetting;
		}

		internal static int ParseIntegerSetting(CtsConfigurationSetting setting, int defaultValue, int min, bool kilobytes)
		{
			if (setting.Arguments.Count != 1 || !setting.Arguments[0].Name.Equals("Value", StringComparison.OrdinalIgnoreCase))
			{
				ApplicationServices.Provider.LogConfigurationErrorEvent();
				return defaultValue;
			}
			if (setting.Arguments[0].Value.Trim().Equals("unlimited", StringComparison.OrdinalIgnoreCase))
			{
				return int.MaxValue;
			}
			int num;
			if (!int.TryParse(setting.Arguments[0].Value.Trim(), out num))
			{
				ApplicationServices.Provider.LogConfigurationErrorEvent();
				return defaultValue;
			}
			if (num < min)
			{
				ApplicationServices.Provider.LogConfigurationErrorEvent();
				return defaultValue;
			}
			if (kilobytes)
			{
				if (num > 2097151)
				{
					num = int.MaxValue;
				}
				else
				{
					num *= 1024;
				}
			}
			return num;
		}

		private static IApplicationServices LoadServices()
		{
			return new DefaultApplicationServices();
		}

		private static IApplicationServices provider = ApplicationServices.LoadServices();
	}
}
