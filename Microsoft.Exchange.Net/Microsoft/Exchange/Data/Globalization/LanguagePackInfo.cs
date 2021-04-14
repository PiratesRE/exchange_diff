using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Security;
using System.Threading;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Globalization
{
	internal static class LanguagePackInfo
	{
		private static ExEventLog EventLog
		{
			get
			{
				if (LanguagePackInfo.eventlog == null)
				{
					LanguagePackInfo.eventlog = new ExEventLog(ExTraceGlobals.LanguagePackInfoTracer.Category, "MSExchange Common");
				}
				return LanguagePackInfo.eventlog;
			}
		}

		public static string[] GetInstalledLanguagePackCultureNames(LanguagePackType type)
		{
			CultureInfo[] installedLanguagePackCultures = LanguagePackInfo.GetInstalledLanguagePackCultures(type);
			List<string> list = new List<string>(installedLanguagePackCultures.Length);
			foreach (CultureInfo cultureInfo in installedLanguagePackCultures)
			{
				list.Add(cultureInfo.Name);
			}
			return list.ToArray();
		}

		public static CultureInfo[] GetInstalledLanguagePackCultures(LanguagePackType type)
		{
			CultureInfo[] array = null;
			if (type != LanguagePackType.UnifiedMessaging)
			{
				string name = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\";
				string name2 = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Language Packs\\" + type.ToString();
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name2))
				{
					if (registryKey != null)
					{
						string[] subKeyNames = registryKey.GetSubKeyNames();
						new List<string>(subKeyNames.Length);
						array = LanguagePackInfo.CreateCultureInfosFromCultureNames(subKeyNames);
						registryKey.Close();
					}
					else
					{
						using (RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey(name))
						{
							if (registryKey2.SubKeyCount != 0)
							{
								LanguagePackInfo.EventLog.LogEvent(CommonEventLogConstants.Tuple_ErrorOpeningLanguagePackRegistryKey, null, new object[0]);
							}
						}
					}
				}
				if (array == null)
				{
					array = new CultureInfo[]
					{
						CultureInfo.CreateSpecificCulture("en")
					};
				}
			}
			else
			{
				array = LanguagePackInfo.GetInstalledUMLanguagePackCultures();
			}
			return array;
		}

		public static CultureInfo[] GetInstalledLanguagePackSpecificCultures(LanguagePackType type)
		{
			List<CultureInfo> list = new List<CultureInfo>(LanguagePackInfo.GetInstalledLanguagePackCultures(type));
			List<CultureInfo> list2 = new List<CultureInfo>(list.Count);
			List<CultureInfo> list3 = new List<CultureInfo>(list.Count);
			foreach (CultureInfo cultureInfo in list)
			{
				CultureInfo cultureInfo2 = cultureInfo;
				if (!cultureInfo.IsNeutralCulture)
				{
					if (cultureInfo.Parent != CultureInfo.InvariantCulture)
					{
						cultureInfo2 = cultureInfo.Parent;
						if (string.Compare(cultureInfo2.Name, "zh-CHS", StringComparison.OrdinalIgnoreCase) == 0)
						{
							cultureInfo2 = CultureInfo.GetCultureInfo("zh-Hans");
						}
						else if (string.Compare(cultureInfo2.Name, "zh-CHT", StringComparison.OrdinalIgnoreCase) == 0)
						{
							cultureInfo2 = CultureInfo.GetCultureInfo("zh-Hant");
						}
						else if (cultureInfo2.Name.StartsWith("sr-cyrl", StringComparison.OrdinalIgnoreCase))
						{
							cultureInfo2 = CultureInfo.GetCultureInfo("sr-Cyrl-CS");
						}
						else if (cultureInfo2.Name.StartsWith("sr-", StringComparison.OrdinalIgnoreCase))
						{
							cultureInfo2 = CultureInfo.GetCultureInfo("sr-Latn-CS");
						}
					}
				}
				else if (string.Equals(cultureInfo.Name, "sr", StringComparison.OrdinalIgnoreCase) || string.Equals(cultureInfo.Name, "sr-latn", StringComparison.OrdinalIgnoreCase))
				{
					cultureInfo2 = CultureInfo.GetCultureInfo("sr-Latn-CS");
				}
				else if (string.Equals(cultureInfo.Name, "sr-cyrl", StringComparison.OrdinalIgnoreCase))
				{
					cultureInfo2 = CultureInfo.GetCultureInfo("sr-Cyrl-CS");
				}
				if (!list3.Contains(cultureInfo2))
				{
					list3.Add(cultureInfo2);
					List<CultureInfo> list4;
					if (LanguagePackInfo.specificCultures.TryGetValue(cultureInfo2, out list4))
					{
						using (List<CultureInfo>.Enumerator enumerator2 = list4.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								CultureInfo cultureInfo3 = enumerator2.Current;
								if (!LanguagePackInfo.UnsupportedCultures.Contains(cultureInfo3.LCID))
								{
									list2.Add(cultureInfo3);
								}
							}
							continue;
						}
					}
					string[] array = new string[]
					{
						cultureInfo2.Name
					};
					foreach (string name in array)
					{
						CultureInfo cultureInfo4 = CultureInfo.CreateSpecificCulture(name);
						if (!list2.Contains(cultureInfo4) && !LanguagePackInfo.UnsupportedCultures.Contains(cultureInfo4.LCID))
						{
							list2.Add(cultureInfo4);
						}
					}
				}
			}
			if (LanguagePackInfo.addPseudoLocalizedLocales)
			{
				list2.AddRange(LanguagePackInfo.CreateCultureInfosFromCultureNames(LanguagePackInfo.PseudoLocalizedLocales));
			}
			return list2.ToArray();
		}

		public static string[] GetLanguagePackBundleCultureNames(string extractedLanguagePackDir, LanguagePackType type)
		{
			CultureInfo[] languagePackBundleCultures = LanguagePackInfo.GetLanguagePackBundleCultures(extractedLanguagePackDir, type);
			if (languagePackBundleCultures != null)
			{
				List<string> list = new List<string>(languagePackBundleCultures.Length);
				foreach (CultureInfo cultureInfo in languagePackBundleCultures)
				{
					list.Add(cultureInfo.Name);
				}
				return list.ToArray();
			}
			return null;
		}

		public static CultureInfo[] GetLanguagePackBundleCultures(string extractedLanguagePackDir, LanguagePackType type)
		{
			try
			{
				string[] directories = Directory.GetDirectories(extractedLanguagePackDir);
				new List<CultureInfo>(directories.Length);
				List<string> list = new List<string>(directories.Length);
				foreach (string path in directories)
				{
					if (Directory.GetFiles(path, (type == LanguagePackType.Client) ? "ClientLanguagePack.msi" : "ServerLanguagePack.msi").Length > 0)
					{
						list.Add(Path.GetFileName(path));
					}
				}
				return LanguagePackInfo.CreateCultureInfosFromCultureNames(list.ToArray());
			}
			catch (UnauthorizedAccessException ex)
			{
				LanguagePackInfo.EventLog.LogEvent(CommonEventLogConstants.Tuple_ErrorScanningLanguagePackFolders, ex.Message, new object[0]);
			}
			catch (ArgumentException ex2)
			{
				LanguagePackInfo.EventLog.LogEvent(CommonEventLogConstants.Tuple_ErrorScanningLanguagePackFolders, ex2.Message, new object[0]);
			}
			catch (DirectoryNotFoundException ex3)
			{
				LanguagePackInfo.EventLog.LogEvent(CommonEventLogConstants.Tuple_ErrorScanningLanguagePackFolders, ex3.Message, new object[0]);
			}
			catch (IOException ex4)
			{
				LanguagePackInfo.EventLog.LogEvent(CommonEventLogConstants.Tuple_ErrorScanningLanguagePackFolders, ex4.Message, new object[0]);
			}
			return null;
		}

		private static CultureInfo GetNeutralCulture(string cultureName)
		{
			int num = 0;
			bool flag = true;
			CultureInfo result = null;
			while (flag)
			{
				try
				{
					result = CultureInfo.GetCultureInfo(cultureName);
					flag = false;
				}
				catch (FileLoadException ex)
				{
					num++;
					if (num > 5)
					{
						throw ex;
					}
					Thread.Sleep(5);
				}
			}
			return result;
		}

		private static Dictionary<CultureInfo, List<CultureInfo>> GetSpecificCultures()
		{
			CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
			Dictionary<CultureInfo, List<CultureInfo>> dictionary = new Dictionary<CultureInfo, List<CultureInfo>>(CultureInfo.GetCultures(CultureTypes.NeutralCultures).Length);
			foreach (CultureInfo cultureInfo in cultures)
			{
				CultureInfo cultureInfo2 = cultureInfo.Parent;
				if (string.Compare(cultureInfo2.Name, "zh-CHS", StringComparison.OrdinalIgnoreCase) == 0)
				{
					cultureInfo2 = LanguagePackInfo.GetNeutralCulture("zh-Hans");
				}
				else if (string.Compare(cultureInfo2.Name, "zh-CHT", StringComparison.OrdinalIgnoreCase) == 0)
				{
					cultureInfo2 = LanguagePackInfo.GetNeutralCulture("zh-Hant");
				}
				if (dictionary.ContainsKey(cultureInfo2))
				{
					dictionary[cultureInfo2].Add(cultureInfo);
				}
				else
				{
					dictionary.Add(cultureInfo2, new List<CultureInfo>(3)
					{
						cultureInfo
					});
				}
			}
			return dictionary;
		}

		private static CultureInfo[] GetInstalledUMLanguagePackCultures()
		{
			CultureInfo[] array = null;
			Exception ex = null;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\UnifiedMessagingRole\\LanguagePacks\\"))
				{
					if (registryKey != null)
					{
						array = LanguagePackInfo.CreateCultureInfosFromCultureNames(registryKey.GetValueNames());
					}
				}
			}
			catch (IOException ex2)
			{
				ex = ex2;
			}
			catch (SecurityException ex3)
			{
				ex = ex3;
			}
			catch (UnauthorizedAccessException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				LanguagePackInfo.EventLog.LogEvent(CommonEventLogConstants.Tuple_ErrorOpeningUMLanguagePackRegistryKey, null, new object[]
				{
					ex.Message
				});
				ExTraceGlobals.LanguagePackInfoTracer.TraceError(0L, string.Format("Error occurred while opening UM language pack registry key : {0}", ex));
			}
			if (array == null || array.Length == 0)
			{
				array = new CultureInfo[0];
			}
			return array;
		}

		private static CultureInfo[] CreateCultureInfosFromCultureNames(string[] cultureNames)
		{
			List<CultureInfo> list = new List<CultureInfo>(cultureNames.Length);
			foreach (string name in cultureNames)
			{
				try
				{
					list.Add(new CultureInfo(name));
				}
				catch (ArgumentException ex)
				{
					LanguagePackInfo.EventLog.LogEvent(CommonEventLogConstants.Tuple_InvalidCultureIdentifier, ex.Message, new object[0]);
				}
			}
			return list.ToArray();
		}

		private const string FailoverCultureId = "en";

		private static Dictionary<CultureInfo, List<CultureInfo>> specificCultures = LanguagePackInfo.GetSpecificCultures();

		private static ExEventLog eventlog;

		private static readonly bool addPseudoLocalizedLocales = StringComparer.OrdinalIgnoreCase.Equals("true", ConfigurationManager.AppSettings["AddPseudoLocalizedLocales"]);

		private static string[] PseudoLocalizedLocales = new string[]
		{
			"qps-ploc",
			"qps-ploca",
			"qps-plocm"
		};

		private static List<int> UnsupportedCultures = new List<int>
		{
			2117,
			2121,
			15369,
			22538,
			11276,
			9228,
			12300,
			15372,
			13324,
			14348,
			8204,
			10252
		};

		public static readonly Microsoft.Exchange.Collections.HashSet<int> expectedCultureLcids = new Microsoft.Exchange.Collections.HashSet<int>
		{
			4,
			9,
			12,
			7,
			17,
			31748,
			16,
			18,
			22,
			25,
			10,
			1,
			5,
			6,
			19,
			11,
			8,
			13,
			14,
			20,
			21,
			2070,
			29,
			31,
			24,
			30,
			1124,
			57,
			33,
			38,
			62,
			34,
			42,
			2,
			26,
			37,
			39,
			31770,
			27,
			36,
			45,
			3,
			3076,
			41,
			86,
			15,
			63,
			3098,
			32,
			54,
			28,
			1118,
			43,
			1101,
			1093,
			2117,
			8218,
			5146,
			55,
			71,
			1128,
			1136,
			2141,
			2108,
			1076,
			1077,
			75,
			1107,
			1158,
			1159,
			65,
			87,
			64,
			1108,
			1134,
			47,
			2110,
			1100,
			1082,
			1153,
			78,
			1121,
			2068,
			1096,
			1123,
			70,
			3179,
			1132,
			1074,
			1115,
			73,
			68,
			74,
			67,
			1106,
			1160,
			1130
		};
	}
}
