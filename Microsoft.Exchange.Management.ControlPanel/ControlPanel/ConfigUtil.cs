using System;
using System.Configuration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class ConfigUtil
	{
		internal static int ReadInt(string key, int defaultValue)
		{
			int result;
			if (!int.TryParse(ConfigurationManager.AppSettings[key], out result))
			{
				result = defaultValue;
			}
			return result;
		}

		internal static bool ReadBool(string key, bool defaultValue)
		{
			bool result;
			if (!bool.TryParse(ConfigurationManager.AppSettings[key], out result))
			{
				result = defaultValue;
			}
			return result;
		}
	}
}
