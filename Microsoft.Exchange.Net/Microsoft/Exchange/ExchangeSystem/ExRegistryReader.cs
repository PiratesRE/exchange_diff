using System;
using System.Collections.Generic;
using Microsoft.Exchange.Win32;
using Microsoft.Win32;

namespace Microsoft.Exchange.ExchangeSystem
{
	internal sealed class ExRegistryReader
	{
		internal static IList<RegistryTimeZoneInformation> ReadTimeZones()
		{
			List<RegistryTimeZoneInformation> list = new List<RegistryTimeZoneInformation>(256);
			using (RegistryKey localMachine = Registry.LocalMachine)
			{
				using (RegistryKey registryKey = localMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones"))
				{
					if (registryKey != null)
					{
						string[] subKeyNames = registryKey.GetSubKeyNames();
						foreach (string text in subKeyNames)
						{
							using (RegistryKey registryKey2 = registryKey.OpenSubKey(text))
							{
								try
								{
									RegistryTimeZoneInformation item = ExRegistryReader.ReadTimeZoneInfoFromRegistry(text, registryKey2);
									list.Add(item);
								}
								catch (InvalidTimeZoneException)
								{
								}
							}
						}
					}
				}
			}
			return list;
		}

		public static RegistryTimeZoneInformation ReadTimeZoneInfoFromRegistry(string timeZoneKeyName, RegistryKey timeZoneKey)
		{
			RegistryTimeZoneInformation result;
			try
			{
				string displayName = timeZoneKey.GetValue("Display") as string;
				string standardName = timeZoneKey.GetValue("Std") as string;
				string daylightName = timeZoneKey.GetValue("Dlt") as string;
				string muiStandardName = timeZoneKey.GetValue("MUI_Std") as string;
				REG_TIMEZONE_INFO regTimeZoneRule = ExRegistryReader.GetRegTimeZoneRule(timeZoneKey, "TZI");
				RegistryTimeZoneInformation registryTimeZoneInformation = new RegistryTimeZoneInformation(timeZoneKeyName, displayName, standardName, daylightName, muiStandardName, regTimeZoneRule);
				using (RegistryKey registryKey = timeZoneKey.OpenSubKey("Dynamic DST"))
				{
					if (registryKey != null)
					{
						registryTimeZoneInformation.Rules = ExRegistryReader.LoadDynamicTimeZoneRules(registryKey);
					}
					else
					{
						registryTimeZoneInformation.Rules = new List<RegistryTimeZoneRule>(1);
						registryTimeZoneInformation.Rules.Add(new RegistryTimeZoneRule(1, registryTimeZoneInformation.RegInfo));
					}
				}
				result = registryTimeZoneInformation;
			}
			catch (ArgumentOutOfRangeException ex)
			{
				throw new InvalidTimeZoneException(ex.Message, ex);
			}
			return result;
		}

		public static string GetCurrentTimeZoneName()
		{
			return ExRegistryReader.GetCurrentTimeZoneInformation("TimeZoneKeyName");
		}

		public static string GetCurrentTimeZoneMuiStandardName()
		{
			return ExRegistryReader.GetCurrentTimeZoneInformation("StandardName");
		}

		private static string GetCurrentTimeZoneInformation(string valueName)
		{
			string result = null;
			using (RegistryKey localMachine = Registry.LocalMachine)
			{
				using (RegistryKey registryKey = localMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\TimeZoneInformation"))
				{
					if (registryKey != null)
					{
						result = (registryKey.GetValue(valueName) as string);
					}
				}
			}
			return result;
		}

		private static IList<RegistryTimeZoneRule> LoadDynamicTimeZoneRules(RegistryKey dynamicRulesKey)
		{
			string[] valueNames = dynamicRulesKey.GetValueNames();
			List<int> list = new List<int>(valueNames.Length);
			for (int i = 0; i < valueNames.Length; i++)
			{
				int item;
				if (int.TryParse(valueNames[i], out item))
				{
					list.Add(item);
				}
			}
			list.Sort();
			List<RegistryTimeZoneRule> list2 = new List<RegistryTimeZoneRule>(list.Count);
			for (int j = 0; j < list.Count; j++)
			{
				REG_TIMEZONE_INFO regTimeZoneRule = ExRegistryReader.GetRegTimeZoneRule(dynamicRulesKey, list[j].ToString());
				list2.Add(new RegistryTimeZoneRule(list[j], regTimeZoneRule));
			}
			return list2;
		}

		private unsafe static REG_TIMEZONE_INFO GetRegTimeZoneRule(RegistryKey timeZoneKey, string subKeyName)
		{
			byte[] array = timeZoneKey.GetValue(subKeyName, null) as byte[];
			if (array == null || array.Length != sizeof(REG_TIMEZONE_INFO))
			{
				throw new InvalidTimeZoneException("Invalid time zone");
			}
			REG_TIMEZONE_INFO result;
			fixed (IntPtr* ptr = array)
			{
				result = *(REG_TIMEZONE_INFO*)ptr;
			}
			ExRegistryReader.NormalizeTimeZoneInfo(ref result);
			return result;
		}

		private static void NormalizeTimeZoneInfo(ref REG_TIMEZONE_INFO timeZoneInfo)
		{
			if (timeZoneInfo.StandardDate == timeZoneInfo.DaylightDate)
			{
				timeZoneInfo.StandardDate = default(NativeMethods.SystemTime);
				timeZoneInfo.DaylightDate = default(NativeMethods.SystemTime);
			}
			if (timeZoneInfo.StandardDate.Month == 0)
			{
				timeZoneInfo.DaylightBias = timeZoneInfo.StandardBias;
			}
		}

		public const string RegistryTimeZoneRoot = "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones";

		public const string IndexRegName = "Index";

		public const string DltRegName = "Dlt";

		public const string StdRegName = "Std";

		public const string DisplayRegName = "Display";

		public const string TziRegName = "TZI";

		public const string MuiStdRegName = "MUI_Std";

		public const string DynamicDstRegName = "Dynamic DST";

		public const string DaylightName = "Daylight";

		public const string StandardName = "Standard";

		public const string CurrentTimeZoneKey = "SYSTEM\\CurrentControlSet\\Control\\TimeZoneInformation";

		public const string CurrentTimeZoneName = "TimeZoneKeyName";

		public const string CurrentTimeZoneStandardName = "StandardName";
	}
}
