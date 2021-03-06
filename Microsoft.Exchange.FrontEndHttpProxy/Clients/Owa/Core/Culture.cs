using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Data.Globalization;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public static class Culture
	{
		public static bool IsRtl
		{
			get
			{
				return Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft;
			}
		}

		public static string GetCssFontFileNameFromCulture()
		{
			return Culture.GetCssFontFileNameFromCulture(false);
		}

		public static string GetCssFontFileNameFromCulture(bool isBasicExperience)
		{
			CultureInfo userCulture = Culture.GetUserCulture();
			string text = Culture.GetCssFontFileNameFromCulture(userCulture);
			if (isBasicExperience)
			{
				text = "basic_" + text;
			}
			return text;
		}

		public static CultureInfo GetBrowserDefaultCulture(HttpRequest httpRequest)
		{
			string[] userLanguages = httpRequest.UserLanguages;
			if (userLanguages != null)
			{
				int num = Math.Min(5, userLanguages.Length);
				for (int i = 0; i < num; i++)
				{
					string text = Culture.ValidateLanguageTag(userLanguages[i]);
					if (text != null)
					{
						CultureInfo supportedBrowserLanguage = Culture.GetSupportedBrowserLanguage(text);
						if (supportedBrowserLanguage != null)
						{
							return supportedBrowserLanguage;
						}
					}
				}
			}
			return null;
		}

		public static string GetCssFontFileNameFromCulture(CultureInfo culture)
		{
			string text = null;
			Culture.fontFileNameTable.TryGetValue(culture.LCID, out text);
			if (string.IsNullOrEmpty(text))
			{
				return "owafont.css";
			}
			return text;
		}

		public static Culture.SingularPluralRegularExpression GetSingularPluralRegularExpressions(int lcid)
		{
			if (Culture.regularExpressionMap.ContainsKey(lcid))
			{
				return Culture.regularExpressionMap[lcid];
			}
			return Culture.defaultRegularExpression;
		}

		public static CultureInfo[] GetSupportedCultures()
		{
			return Culture.GetSupportedCultures(false);
		}

		public static CultureInfo[] GetSupportedCultures(bool sortByName)
		{
			if (!sortByName)
			{
				return Culture.supportedCultureInfosSortedByLcid;
			}
			return Culture.CreateSortedSupportedCultures(sortByName);
		}

		public static bool IsSupportedCulture(CultureInfo culture)
		{
			return Culture.supportedCultureInfos.Contains(culture);
		}

		public static bool IsSupportedCulture(int lcid)
		{
			return lcid > 0 && Culture.IsSupportedCulture(CultureInfo.GetCultureInfo(lcid));
		}

		public static CultureInfo GetSupportedBrowserLanguage(string language)
		{
			if (Culture.languageMap.ContainsKey(language) && Array.BinarySearch<string>(Culture.supportedBrowserLanguages, CultureInfo.GetCultureInfo(Culture.languageMap[language]).Name, StringComparer.OrdinalIgnoreCase) >= 0)
			{
				return Culture.GetCultureInfoInstance(Culture.languageMap[language]);
			}
			if (Array.BinarySearch<string>(Culture.supportedBrowserLanguages, language, StringComparer.OrdinalIgnoreCase) >= 0)
			{
				return Culture.GetCultureInfoInstance(CultureInfo.GetCultureInfo(language).LCID);
			}
			return null;
		}

		internal static string[] GetSupportedBrowserLanguageArray()
		{
			string[] array = new string[Culture.supportedCultureInfos.Count];
			int num = 0;
			foreach (CultureInfo cultureInfo in Culture.supportedCultureInfos)
			{
				array[num++] = cultureInfo.Name;
			}
			Array.Sort<string>(array, StringComparer.OrdinalIgnoreCase);
			return array;
		}

		internal static string LookUpHelpDirectoryForCulture(CultureInfo culture)
		{
			string text = null;
			string str = HttpRuntime.AppDomainAppPath + "help\\";
			while (!culture.Equals(CultureInfo.InvariantCulture))
			{
				text = culture.Name;
				if (Directory.Exists(str + text))
				{
					break;
				}
				culture = culture.Parent;
			}
			return text;
		}

		internal static CultureInfo GetUserCulture()
		{
			return Thread.CurrentThread.CurrentCulture;
		}

		internal static CultureInfo GetCultureInfoInstance(int lcid)
		{
			CultureInfo cultureInfo = new CultureInfo(lcid);
			Calendar calendar = new GregorianCalendar();
			cultureInfo.DateTimeFormat.Calendar = calendar;
			return cultureInfo;
		}

		private static Dictionary<int, string> LoadFontFileNameDictionary()
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			dictionary[31748] = (dictionary[3076] = (dictionary[5124] = (dictionary[1028] = "owafont_zh_cht.css")));
			dictionary[4] = (dictionary[4100] = (dictionary[2052] = "owafont_zh_chs.css"));
			dictionary[17] = (dictionary[1041] = "owafont_ja.css");
			dictionary[18] = (dictionary[1042] = "owafont_ko.css");
			dictionary[1066] = "owafont_vi.css";
			return dictionary;
		}

		private static Dictionary<string, int> LoadLanguageMap()
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
			dictionary["ar"] = 1025;
			dictionary["bg"] = 1026;
			dictionary["ca"] = 1027;
			dictionary["cs"] = 1029;
			dictionary["da"] = 1030;
			dictionary["de"] = 1031;
			dictionary["el"] = 1032;
			dictionary["en"] = 1033;
			dictionary["fi"] = 1035;
			dictionary["fr"] = 1036;
			dictionary["he"] = 1037;
			dictionary["hu"] = 1038;
			dictionary["is"] = 1039;
			dictionary["it"] = 1040;
			dictionary["ja"] = 1041;
			dictionary["ko"] = 1042;
			dictionary["ko-kp"] = 1042;
			dictionary["nl"] = 1043;
			dictionary["no"] = 1044;
			dictionary["nb"] = 1044;
			dictionary["nn"] = 1044;
			dictionary["nn-no"] = 1044;
			dictionary["pl"] = 1045;
			dictionary["ro"] = 1048;
			dictionary["ro-md"] = 1048;
			dictionary["ru"] = 1049;
			dictionary["ru-mo"] = 1049;
			dictionary["hr"] = 1050;
			dictionary["sk"] = 1051;
			dictionary["sv"] = 1053;
			dictionary["th"] = 1054;
			dictionary["tr"] = 1055;
			dictionary["ur"] = 1056;
			dictionary["id"] = 1057;
			dictionary["uk"] = 1058;
			dictionary["sl"] = 1060;
			dictionary["et"] = 1061;
			dictionary["lv"] = 1062;
			dictionary["lt"] = 1063;
			dictionary["fa"] = 1065;
			dictionary["vi"] = 1066;
			dictionary["eu"] = 1069;
			dictionary["hi"] = 1081;
			dictionary["ms"] = 1086;
			dictionary["kk"] = 1087;
			dictionary["sw"] = 1089;
			dictionary["bn"] = 1093;
			dictionary["gu"] = 1095;
			dictionary["or"] = 1096;
			dictionary["ta"] = 1097;
			dictionary["te"] = 1098;
			dictionary["kn"] = 1099;
			dictionary["ml"] = 1100;
			dictionary["mr"] = 1102;
			dictionary["cy"] = 1106;
			dictionary["gl"] = 1110;
			dictionary["am"] = 1118;
			dictionary["fil-Latn"] = 1124;
			dictionary["fil"] = 1124;
			dictionary["zh"] = 2052;
			dictionary["zh-Hans-cn"] = 2052;
			dictionary["zh-Hans"] = 2052;
			dictionary["pt"] = 2070;
			dictionary["sr"] = 2074;
			dictionary["zh-Hant-tw"] = 3076;
			dictionary["zh-Hant"] = 3076;
			dictionary["es"] = 3082;
			dictionary["es-us"] = 3082;
			return dictionary;
		}

		private static Dictionary<int, Culture.SingularPluralRegularExpression> LoadRemindersDueInRegularExpressionsMap()
		{
			Dictionary<int, Culture.SingularPluralRegularExpression> dictionary = new Dictionary<int, Culture.SingularPluralRegularExpression>();
			Culture.SingularPluralRegularExpression value = new Culture.SingularPluralRegularExpression("^1$|[^1]1$", "^[234]$|[^1][234]$");
			Culture.SingularPluralRegularExpression value2 = new Culture.SingularPluralRegularExpression(".", "^[234]$");
			dictionary[1029] = value2;
			dictionary[1051] = value2;
			dictionary[1060] = value2;
			dictionary[1058] = value2;
			dictionary[1045] = new Culture.SingularPluralRegularExpression(".", "^[234]$|[^1][234]$");
			dictionary[1049] = value;
			dictionary[2074] = value;
			dictionary[3098] = value;
			dictionary[1063] = value;
			dictionary[1062] = value;
			return dictionary;
		}

		private static Dictionary<string, string[]> LoadOneLetterDayNamesMap()
		{
			Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
			string[] array = new string[]
			{
				Encoding.Unicode.GetString(new byte[]
				{
					229,
					101
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					0,
					78
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					140,
					78
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					9,
					78
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					219,
					86
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					148,
					78
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					109,
					81
				})
			};
			dictionary["zh-MO"] = array;
			dictionary["zh-TW"] = array;
			dictionary["zh-CN"] = array;
			dictionary["zh-SG"] = array;
			array = new string[]
			{
				Encoding.Unicode.GetString(new byte[]
				{
					45,
					6
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					70,
					6
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					43,
					6
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					49,
					6
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					46,
					6
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					44,
					6
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					51,
					6
				})
			};
			dictionary["ar"] = array;
			dictionary["ar-SA"] = array;
			dictionary["ar-IQ"] = array;
			dictionary["ar-EG"] = array;
			dictionary["ar-LY"] = array;
			dictionary["ar-DZ"] = array;
			dictionary["ar-MA"] = array;
			dictionary["ar-TN"] = array;
			dictionary["ar-OM"] = array;
			dictionary["ar-YE"] = array;
			dictionary["ar-SY"] = array;
			dictionary["ar-JO"] = array;
			dictionary["ar-LB"] = array;
			dictionary["ar-KW"] = array;
			dictionary["ar-AE"] = array;
			dictionary["ar-BH"] = array;
			dictionary["ar-QA"] = array;
			array = new string[]
			{
				Encoding.Unicode.GetString(new byte[]
				{
					204,
					6
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					47,
					6
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					51,
					6
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					134,
					6
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					126,
					6
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					44,
					6
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					52,
					6
				})
			};
			dictionary["fa"] = array;
			dictionary["fa-IR"] = array;
			array = new string[]
			{
				Encoding.Unicode.GetString(new byte[]
				{
					208,
					5
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					209,
					5
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					210,
					5
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					211,
					5
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					212,
					5
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					213,
					5
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					233,
					5
				})
			};
			dictionary["he"] = array;
			dictionary["he-IL"] = array;
			dictionary["hi"] = new string[]
			{
				Encoding.Unicode.GetString(new byte[]
				{
					48,
					9
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					56,
					9,
					75,
					9
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					46,
					9,
					2,
					9
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					44,
					9,
					65,
					9
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					23,
					9,
					65,
					9
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					54,
					9,
					65,
					9
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					54,
					9
				})
			};
			dictionary["th"] = new string[]
			{
				Encoding.Unicode.GetString(new byte[]
				{
					45,
					14
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					8,
					14
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					45,
					14
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					30,
					14
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					30,
					14
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					40,
					14
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					42,
					14
				})
			};
			array = new string[]
			{
				"I",
				"A",
				"R",
				"Z",
				"G",
				"O",
				"L"
			};
			dictionary["eu"] = array;
			dictionary["eu-ES"] = array;
			array = new string[]
			{
				"D",
				"L",
				"M",
				"X",
				"J",
				"V",
				"S"
			};
			dictionary["ca"] = array;
			dictionary["ca-ES"] = array;
			array = new string[]
			{
				"s",
				"m",
				"t",
				"w",
				"t",
				"f",
				"s"
			};
			dictionary["vi"] = array;
			dictionary["vi-VN"] = array;
			array = new string[]
			{
				Encoding.Unicode.GetString(new byte[]
				{
					39,
					6
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					126,
					6
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					69,
					6
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					40,
					6
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					44,
					6
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					44,
					6
				}),
				Encoding.Unicode.GetString(new byte[]
				{
					71,
					6
				})
			};
			dictionary["ur"] = array;
			dictionary["ur-PK"] = array;
			CultureInfo[] supportedCultures = Culture.GetSupportedCultures();
			for (int i = 0; i < supportedCultures.Length; i++)
			{
				if (!dictionary.ContainsKey(supportedCultures[i].Name))
				{
					string[] abbreviatedDayNames = supportedCultures[i].DateTimeFormat.AbbreviatedDayNames;
					array = new string[7];
					for (int j = 0; j < abbreviatedDayNames.Length; j++)
					{
						array[j] = abbreviatedDayNames[j][0].ToString();
					}
					dictionary[supportedCultures[i].Name] = array;
				}
			}
			return dictionary;
		}

		private static int CompareCultureNames(CultureInfo x, CultureInfo y)
		{
			return string.Compare(x.NativeName, y.NativeName, StringComparison.CurrentCulture);
		}

		private static int CompareCultureLCIDs(CultureInfo x, CultureInfo y)
		{
			return x.LCID.CompareTo(y.LCID);
		}

		private static string ValidateLanguageTag(string tag)
		{
			if (tag.Length < 1 || tag.Length > 44)
			{
				return null;
			}
			int num = 0;
			while (num < tag.Length && char.IsWhiteSpace(tag[num]))
			{
				num++;
			}
			if (num == tag.Length)
			{
				return null;
			}
			int num2 = num;
			for (int i = 0; i < 3; i++)
			{
				int num3 = 0;
				while (num3 < 8 && num2 < tag.Length && ((tag[num2] >= 'a' && tag[num2] <= 'z') || (tag[num2] >= 'A' && tag[num2] <= 'Z')))
				{
					num3++;
					num2++;
				}
				if (num2 == tag.Length || tag[num2] != '-')
				{
					break;
				}
				num2++;
			}
			if (num2 != tag.Length && tag[num2] != ';' && !char.IsWhiteSpace(tag[num2]))
			{
				return null;
			}
			return tag.Substring(num, num2 - num);
		}

		private static List<CultureInfo> CreateCultureInfosFromNames(string[] cultureNames)
		{
			List<CultureInfo> list = new List<CultureInfo>(cultureNames.Length);
			foreach (string name in cultureNames)
			{
				list.Add(CultureInfo.GetCultureInfo(name));
			}
			return list;
		}

		private static CultureInfo[] CreateSortedSupportedCultures(bool sortByName)
		{
			CultureInfo[] array = new CultureInfo[Culture.supportedCultureInfos.Count];
			int num = 0;
			foreach (CultureInfo cultureInfo in Culture.supportedCultureInfos)
			{
				array[num++] = CultureInfo.GetCultureInfo(cultureInfo.LCID);
			}
			if (sortByName)
			{
				Array.Sort<CultureInfo>(array, new Comparison<CultureInfo>(Culture.CompareCultureNames));
			}
			else
			{
				Array.Sort<CultureInfo>(array, new Comparison<CultureInfo>(Culture.CompareCultureLCIDs));
			}
			return array;
		}

		public const string LtrDirectionMark = "&#x200E;";

		public const string RtlDirectionMark = "&#x200F;";

		private const int LanguageThreshold = 5;

		private const string DefaultSingularExpression = "^1$";

		private const string DefaultPluralExpression = ".";

		private const string CzechPluralExpression = "^[234]$";

		private const string RussianOrPolishPluralExpression = "^[234]$|[^1][234]$";

		private const string RussianSingularExpression = "^1$|[^1]1$";

		private const string DefaultCssFontFileName = "owafont.css";

		private static readonly List<CultureInfo> supportedCultureInfos = new List<CultureInfo>(LanguagePackInfo.GetInstalledLanguagePackSpecificCultures(LanguagePackType.Client));

		private static readonly CultureInfo[] supportedCultureInfosSortedByLcid = Culture.CreateSortedSupportedCultures(false);

		private static readonly string[] supportedBrowserLanguages = Culture.GetSupportedBrowserLanguageArray();

		private static readonly Dictionary<string, int> languageMap = Culture.LoadLanguageMap();

		private static readonly Dictionary<int, Culture.SingularPluralRegularExpression> regularExpressionMap = Culture.LoadRemindersDueInRegularExpressionsMap();

		private static readonly Dictionary<string, string[]> oneLetterDayNamesMap = Culture.LoadOneLetterDayNamesMap();

		private static Culture.SingularPluralRegularExpression defaultRegularExpression = new Culture.SingularPluralRegularExpression("^1$", ".");

		private static Dictionary<int, string> fontFileNameTable = Culture.LoadFontFileNameDictionary();

		public struct SingularPluralRegularExpression
		{
			internal SingularPluralRegularExpression(string singularExpression, string pluralExpression)
			{
				this.singularExpression = singularExpression;
				this.pluralExpression = pluralExpression;
			}

			internal string SingularExpression
			{
				get
				{
					return this.singularExpression;
				}
			}

			internal string PluralExpression
			{
				get
				{
					return this.pluralExpression;
				}
			}

			private string singularExpression;

			private string pluralExpression;
		}
	}
}
