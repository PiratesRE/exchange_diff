using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using Microsoft.Win32;

namespace System.Globalization
{
	[FriendAccessAllowed]
	internal class CultureData
	{
		private static Dictionary<string, string> RegionNames
		{
			get
			{
				if (CultureData.s_RegionNames == null)
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string>
					{
						{
							"029",
							"en-029"
						},
						{
							"AE",
							"ar-AE"
						},
						{
							"AF",
							"prs-AF"
						},
						{
							"AL",
							"sq-AL"
						},
						{
							"AM",
							"hy-AM"
						},
						{
							"AR",
							"es-AR"
						},
						{
							"AT",
							"de-AT"
						},
						{
							"AU",
							"en-AU"
						},
						{
							"AZ",
							"az-Cyrl-AZ"
						},
						{
							"BA",
							"bs-Latn-BA"
						},
						{
							"BD",
							"bn-BD"
						},
						{
							"BE",
							"nl-BE"
						},
						{
							"BG",
							"bg-BG"
						},
						{
							"BH",
							"ar-BH"
						},
						{
							"BN",
							"ms-BN"
						},
						{
							"BO",
							"es-BO"
						},
						{
							"BR",
							"pt-BR"
						},
						{
							"BY",
							"be-BY"
						},
						{
							"BZ",
							"en-BZ"
						},
						{
							"CA",
							"en-CA"
						},
						{
							"CH",
							"it-CH"
						},
						{
							"CL",
							"es-CL"
						},
						{
							"CN",
							"zh-CN"
						},
						{
							"CO",
							"es-CO"
						},
						{
							"CR",
							"es-CR"
						},
						{
							"CS",
							"sr-Cyrl-CS"
						},
						{
							"CZ",
							"cs-CZ"
						},
						{
							"DE",
							"de-DE"
						},
						{
							"DK",
							"da-DK"
						},
						{
							"DO",
							"es-DO"
						},
						{
							"DZ",
							"ar-DZ"
						},
						{
							"EC",
							"es-EC"
						},
						{
							"EE",
							"et-EE"
						},
						{
							"EG",
							"ar-EG"
						},
						{
							"ES",
							"es-ES"
						},
						{
							"ET",
							"am-ET"
						},
						{
							"FI",
							"fi-FI"
						},
						{
							"FO",
							"fo-FO"
						},
						{
							"FR",
							"fr-FR"
						},
						{
							"GB",
							"en-GB"
						},
						{
							"GE",
							"ka-GE"
						},
						{
							"GL",
							"kl-GL"
						},
						{
							"GR",
							"el-GR"
						},
						{
							"GT",
							"es-GT"
						},
						{
							"HK",
							"zh-HK"
						},
						{
							"HN",
							"es-HN"
						},
						{
							"HR",
							"hr-HR"
						},
						{
							"HU",
							"hu-HU"
						},
						{
							"ID",
							"id-ID"
						},
						{
							"IE",
							"en-IE"
						},
						{
							"IL",
							"he-IL"
						},
						{
							"IN",
							"hi-IN"
						},
						{
							"IQ",
							"ar-IQ"
						},
						{
							"IR",
							"fa-IR"
						},
						{
							"IS",
							"is-IS"
						},
						{
							"IT",
							"it-IT"
						},
						{
							"IV",
							""
						},
						{
							"JM",
							"en-JM"
						},
						{
							"JO",
							"ar-JO"
						},
						{
							"JP",
							"ja-JP"
						},
						{
							"KE",
							"sw-KE"
						},
						{
							"KG",
							"ky-KG"
						},
						{
							"KH",
							"km-KH"
						},
						{
							"KR",
							"ko-KR"
						},
						{
							"KW",
							"ar-KW"
						},
						{
							"KZ",
							"kk-KZ"
						},
						{
							"LA",
							"lo-LA"
						},
						{
							"LB",
							"ar-LB"
						},
						{
							"LI",
							"de-LI"
						},
						{
							"LK",
							"si-LK"
						},
						{
							"LT",
							"lt-LT"
						},
						{
							"LU",
							"lb-LU"
						},
						{
							"LV",
							"lv-LV"
						},
						{
							"LY",
							"ar-LY"
						},
						{
							"MA",
							"ar-MA"
						},
						{
							"MC",
							"fr-MC"
						},
						{
							"ME",
							"sr-Latn-ME"
						},
						{
							"MK",
							"mk-MK"
						},
						{
							"MN",
							"mn-MN"
						},
						{
							"MO",
							"zh-MO"
						},
						{
							"MT",
							"mt-MT"
						},
						{
							"MV",
							"dv-MV"
						},
						{
							"MX",
							"es-MX"
						},
						{
							"MY",
							"ms-MY"
						},
						{
							"NG",
							"ig-NG"
						},
						{
							"NI",
							"es-NI"
						},
						{
							"NL",
							"nl-NL"
						},
						{
							"NO",
							"nn-NO"
						},
						{
							"NP",
							"ne-NP"
						},
						{
							"NZ",
							"en-NZ"
						},
						{
							"OM",
							"ar-OM"
						},
						{
							"PA",
							"es-PA"
						},
						{
							"PE",
							"es-PE"
						},
						{
							"PH",
							"en-PH"
						},
						{
							"PK",
							"ur-PK"
						},
						{
							"PL",
							"pl-PL"
						},
						{
							"PR",
							"es-PR"
						},
						{
							"PT",
							"pt-PT"
						},
						{
							"PY",
							"es-PY"
						},
						{
							"QA",
							"ar-QA"
						},
						{
							"RO",
							"ro-RO"
						},
						{
							"RS",
							"sr-Latn-RS"
						},
						{
							"RU",
							"ru-RU"
						},
						{
							"RW",
							"rw-RW"
						},
						{
							"SA",
							"ar-SA"
						},
						{
							"SE",
							"sv-SE"
						},
						{
							"SG",
							"zh-SG"
						},
						{
							"SI",
							"sl-SI"
						},
						{
							"SK",
							"sk-SK"
						},
						{
							"SN",
							"wo-SN"
						},
						{
							"SV",
							"es-SV"
						},
						{
							"SY",
							"ar-SY"
						},
						{
							"TH",
							"th-TH"
						},
						{
							"TJ",
							"tg-Cyrl-TJ"
						},
						{
							"TM",
							"tk-TM"
						},
						{
							"TN",
							"ar-TN"
						},
						{
							"TR",
							"tr-TR"
						},
						{
							"TT",
							"en-TT"
						},
						{
							"TW",
							"zh-TW"
						},
						{
							"UA",
							"uk-UA"
						},
						{
							"US",
							"en-US"
						},
						{
							"UY",
							"es-UY"
						},
						{
							"UZ",
							"uz-Cyrl-UZ"
						},
						{
							"VE",
							"es-VE"
						},
						{
							"VN",
							"vi-VN"
						},
						{
							"YE",
							"ar-YE"
						},
						{
							"ZA",
							"af-ZA"
						},
						{
							"ZW",
							"en-ZW"
						}
					};
					CultureData.s_RegionNames = dictionary;
				}
				return CultureData.s_RegionNames;
			}
		}

		internal static CultureData Invariant
		{
			get
			{
				if (CultureData.s_Invariant == null)
				{
					CultureData cultureData = new CultureData();
					cultureData.bUseOverrides = false;
					cultureData.sRealName = "";
					CultureData.nativeInitCultureData(cultureData);
					cultureData.bUseOverrides = false;
					cultureData.sRealName = "";
					cultureData.sWindowsName = "";
					cultureData.sName = "";
					cultureData.sParent = "";
					cultureData.bNeutral = false;
					cultureData.bFramework = true;
					cultureData.sEnglishDisplayName = "Invariant Language (Invariant Country)";
					cultureData.sNativeDisplayName = "Invariant Language (Invariant Country)";
					cultureData.sSpecificCulture = "";
					cultureData.sISO639Language = "iv";
					cultureData.sLocalizedLanguage = "Invariant Language";
					cultureData.sEnglishLanguage = "Invariant Language";
					cultureData.sNativeLanguage = "Invariant Language";
					cultureData.sRegionName = "IV";
					cultureData.iGeoId = 244;
					cultureData.sEnglishCountry = "Invariant Country";
					cultureData.sNativeCountry = "Invariant Country";
					cultureData.sISO3166CountryName = "IV";
					cultureData.sPositiveSign = "+";
					cultureData.sNegativeSign = "-";
					cultureData.saNativeDigits = new string[]
					{
						"0",
						"1",
						"2",
						"3",
						"4",
						"5",
						"6",
						"7",
						"8",
						"9"
					};
					cultureData.iDigitSubstitution = 1;
					cultureData.iLeadingZeros = 1;
					cultureData.iDigits = 2;
					cultureData.iNegativeNumber = 1;
					cultureData.waGrouping = new int[]
					{
						3
					};
					cultureData.sDecimalSeparator = ".";
					cultureData.sThousandSeparator = ",";
					cultureData.sNaN = "NaN";
					cultureData.sPositiveInfinity = "Infinity";
					cultureData.sNegativeInfinity = "-Infinity";
					cultureData.iNegativePercent = 0;
					cultureData.iPositivePercent = 0;
					cultureData.sPercent = "%";
					cultureData.sPerMille = "‰";
					cultureData.sCurrency = "¤";
					cultureData.sIntlMonetarySymbol = "XDR";
					cultureData.sEnglishCurrency = "International Monetary Fund";
					cultureData.sNativeCurrency = "International Monetary Fund";
					cultureData.iCurrencyDigits = 2;
					cultureData.iCurrency = 0;
					cultureData.iNegativeCurrency = 0;
					cultureData.waMonetaryGrouping = new int[]
					{
						3
					};
					cultureData.sMonetaryDecimal = ".";
					cultureData.sMonetaryThousand = ",";
					cultureData.iMeasure = 0;
					cultureData.sListSeparator = ",";
					cultureData.sAM1159 = "AM";
					cultureData.sPM2359 = "PM";
					cultureData.saLongTimes = new string[]
					{
						"HH:mm:ss"
					};
					cultureData.saShortTimes = new string[]
					{
						"HH:mm",
						"hh:mm tt",
						"H:mm",
						"h:mm tt"
					};
					cultureData.saDurationFormats = new string[]
					{
						"HH:mm:ss"
					};
					cultureData.iFirstDayOfWeek = 0;
					cultureData.iFirstWeekOfYear = 0;
					cultureData.waCalendars = new int[]
					{
						1
					};
					cultureData.calendars = new CalendarData[23];
					cultureData.calendars[0] = CalendarData.Invariant;
					cultureData.iReadingLayout = 0;
					cultureData.sTextInfo = "";
					cultureData.sCompareInfo = "";
					cultureData.sScripts = "Latn;";
					cultureData.iLanguage = 127;
					cultureData.iDefaultAnsiCodePage = 1252;
					cultureData.iDefaultOemCodePage = 437;
					cultureData.iDefaultMacCodePage = 10000;
					cultureData.iDefaultEbcdicCodePage = 37;
					cultureData.sAbbrevLang = "IVL";
					cultureData.sAbbrevCountry = "IVC";
					cultureData.sISO639Language2 = "ivl";
					cultureData.sISO3166CountryName2 = "ivc";
					cultureData.iInputLanguageHandle = 127;
					cultureData.sConsoleFallbackName = "";
					cultureData.sKeyboardsToInstall = "0409:00000409";
					CultureData.s_Invariant = cultureData;
				}
				return CultureData.s_Invariant;
			}
		}

		[SecurityCritical]
		private static bool IsResourcePresent(string resourceKey)
		{
			if (CultureData.MscorlibResourceSet == null)
			{
				CultureData.MscorlibResourceSet = new ResourceSet(typeof(Environment).Assembly.GetManifestResourceStream("mscorlib.resources"));
			}
			return CultureData.MscorlibResourceSet.GetString(resourceKey) != null;
		}

		[FriendAccessAllowed]
		internal static CultureData GetCultureData(string cultureName, bool useUserOverride)
		{
			if (string.IsNullOrEmpty(cultureName))
			{
				return CultureData.Invariant;
			}
			if (CompatibilitySwitches.IsAppEarlierThanWindowsPhone8)
			{
				if (cultureName.Equals("iw", StringComparison.OrdinalIgnoreCase))
				{
					cultureName = "he";
				}
				else if (cultureName.Equals("tl", StringComparison.OrdinalIgnoreCase))
				{
					cultureName = "fil";
				}
				else if (cultureName.Equals("english", StringComparison.OrdinalIgnoreCase))
				{
					cultureName = "en";
				}
			}
			string key = CultureData.AnsiToLower(useUserOverride ? cultureName : (cultureName + "*"));
			Dictionary<string, CultureData> dictionary = CultureData.s_cachedCultures;
			if (dictionary == null)
			{
				dictionary = new Dictionary<string, CultureData>();
			}
			else
			{
				object syncRoot = ((ICollection)dictionary).SyncRoot;
				CultureData cultureData;
				lock (syncRoot)
				{
					dictionary.TryGetValue(key, out cultureData);
				}
				if (cultureData != null)
				{
					return cultureData;
				}
			}
			CultureData cultureData2 = CultureData.CreateCultureData(cultureName, useUserOverride);
			if (cultureData2 == null)
			{
				return null;
			}
			object syncRoot2 = ((ICollection)dictionary).SyncRoot;
			lock (syncRoot2)
			{
				dictionary[key] = cultureData2;
			}
			CultureData.s_cachedCultures = dictionary;
			return cultureData2;
		}

		private static CultureData CreateCultureData(string cultureName, bool useUserOverride)
		{
			CultureData cultureData = new CultureData();
			cultureData.bUseOverrides = useUserOverride;
			cultureData.sRealName = cultureName;
			if (!cultureData.InitCultureData() && !cultureData.InitCompatibilityCultureData() && !cultureData.InitLegacyAlternateSortData())
			{
				return null;
			}
			return cultureData;
		}

		private bool InitCultureData()
		{
			if (!CultureData.nativeInitCultureData(this))
			{
				return false;
			}
			if (CultureInfo.IsTaiwanSku)
			{
				this.TreatTaiwanParentChainAsHavingTaiwanAsSpecific();
			}
			return true;
		}

		[SecuritySafeCritical]
		private void TreatTaiwanParentChainAsHavingTaiwanAsSpecific()
		{
			if (this.IsNeutralInParentChainOfTaiwan() && CultureData.IsOsPriorToWin7() && !this.IsReplacementCulture)
			{
				string text = this.SNATIVELANGUAGE;
				text = this.SENGLISHLANGUAGE;
				text = this.SLOCALIZEDLANGUAGE;
				text = this.STEXTINFO;
				text = this.SCOMPAREINFO;
				text = this.FONTSIGNATURE;
				int num = this.IDEFAULTANSICODEPAGE;
				num = this.IDEFAULTOEMCODEPAGE;
				num = this.IDEFAULTMACCODEPAGE;
				this.sSpecificCulture = "zh-TW";
				this.sWindowsName = "zh-TW";
			}
		}

		private bool IsNeutralInParentChainOfTaiwan()
		{
			return this.sRealName == "zh" || this.sRealName == "zh-Hant";
		}

		private static bool IsOsPriorToWin7()
		{
			return Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version < CultureData.s_win7Version;
		}

		private static bool IsOsWin7OrPrior()
		{
			return Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version < new Version(6, 2);
		}

		private bool InitCompatibilityCultureData()
		{
			string testString = this.sRealName;
			string a = CultureData.AnsiToLower(testString);
			string text;
			string text2;
			if (!(a == "zh-chs"))
			{
				if (!(a == "zh-cht"))
				{
					return false;
				}
				text = "zh-Hant";
				text2 = "zh-CHT";
			}
			else
			{
				text = "zh-Hans";
				text2 = "zh-CHS";
			}
			this.sRealName = text;
			if (!this.InitCultureData())
			{
				return false;
			}
			this.sName = text2;
			this.sParent = text;
			this.bFramework = true;
			return true;
		}

		private bool InitLegacyAlternateSortData()
		{
			if (!CompareInfo.IsLegacy20SortingBehaviorRequested)
			{
				return false;
			}
			string testString = this.sRealName;
			string a = CultureData.AnsiToLower(testString);
			if (!(a == "ko-kr_unicod"))
			{
				if (!(a == "ja-jp_unicod"))
				{
					if (!(a == "zh-hk_stroke"))
					{
						return false;
					}
					testString = "zh-HK_stroke";
					this.sRealName = "zh-HK";
					this.iLanguage = 134148;
				}
				else
				{
					testString = "ja-JP_unicod";
					this.sRealName = "ja-JP";
					this.iLanguage = 66577;
				}
			}
			else
			{
				testString = "ko-KR_unicod";
				this.sRealName = "ko-KR";
				this.iLanguage = 66578;
			}
			if (!CultureData.nativeInitCultureData(this))
			{
				return false;
			}
			this.sRealName = testString;
			this.sCompareInfo = testString;
			this.bFramework = true;
			return true;
		}

		[SecurityCritical]
		internal static CultureData GetCultureDataForRegion(string cultureName, bool useUserOverride)
		{
			if (string.IsNullOrEmpty(cultureName))
			{
				return CultureData.Invariant;
			}
			CultureData cultureData = CultureData.GetCultureData(cultureName, useUserOverride);
			if (cultureData != null && !cultureData.IsNeutralCulture)
			{
				return cultureData;
			}
			CultureData cultureData2 = cultureData;
			string key = CultureData.AnsiToLower(useUserOverride ? cultureName : (cultureName + "*"));
			Dictionary<string, CultureData> dictionary = CultureData.s_cachedRegions;
			if (dictionary == null)
			{
				dictionary = new Dictionary<string, CultureData>();
			}
			else
			{
				object syncRoot = ((ICollection)dictionary).SyncRoot;
				lock (syncRoot)
				{
					dictionary.TryGetValue(key, out cultureData);
				}
				if (cultureData != null)
				{
					return cultureData;
				}
			}
			try
			{
				RegistryKey registryKey = Registry.LocalMachine.InternalOpenSubKey(CultureData.s_RegionKey, false);
				if (registryKey != null)
				{
					try
					{
						object obj = registryKey.InternalGetValue(cultureName, null, false, false);
						if (obj != null)
						{
							string cultureName2 = obj.ToString();
							cultureData = CultureData.GetCultureData(cultureName2, useUserOverride);
						}
					}
					finally
					{
						registryKey.Close();
					}
				}
			}
			catch (ObjectDisposedException)
			{
			}
			catch (ArgumentException)
			{
			}
			if ((cultureData == null || cultureData.IsNeutralCulture) && CultureData.RegionNames.ContainsKey(cultureName))
			{
				cultureData = CultureData.GetCultureData(CultureData.RegionNames[cultureName], useUserOverride);
			}
			if (cultureData == null || cultureData.IsNeutralCulture)
			{
				CultureInfo[] array = CultureData.SpecificCultures;
				for (int i = 0; i < array.Length; i++)
				{
					if (string.Compare(array[i].m_cultureData.SREGIONNAME, cultureName, StringComparison.OrdinalIgnoreCase) == 0)
					{
						cultureData = array[i].m_cultureData;
						break;
					}
				}
			}
			if (cultureData != null && !cultureData.IsNeutralCulture)
			{
				object syncRoot2 = ((ICollection)dictionary).SyncRoot;
				lock (syncRoot2)
				{
					dictionary[key] = cultureData;
				}
				CultureData.s_cachedRegions = dictionary;
			}
			else
			{
				cultureData = cultureData2;
			}
			return cultureData;
		}

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string LCIDToLocaleName(int lcid);

		internal static CultureData GetCultureData(int culture, bool bUseUserOverride)
		{
			string text = null;
			CultureData cultureData = null;
			if (CompareInfo.IsLegacy20SortingBehaviorRequested)
			{
				if (culture != 66577)
				{
					if (culture != 66578)
					{
						if (culture == 134148)
						{
							text = "zh-HK_stroke";
						}
					}
					else
					{
						text = "ko-KR_unicod";
					}
				}
				else
				{
					text = "ja-JP_unicod";
				}
			}
			if (text == null)
			{
				text = CultureData.LCIDToLocaleName(culture);
			}
			if (string.IsNullOrEmpty(text))
			{
				if (culture == 127)
				{
					return CultureData.Invariant;
				}
			}
			else
			{
				if (!(text == "zh-Hans"))
				{
					if (text == "zh-Hant")
					{
						text = "zh-CHT";
					}
				}
				else
				{
					text = "zh-CHS";
				}
				cultureData = CultureData.GetCultureData(text, bUseUserOverride);
			}
			if (cultureData == null)
			{
				throw new CultureNotFoundException("culture", culture, Environment.GetResourceString("Argument_CultureNotSupported"));
			}
			return cultureData;
		}

		internal static void ClearCachedData()
		{
			CultureData.s_cachedCultures = null;
			CultureData.s_cachedRegions = null;
			CultureData.s_replacementCultureNames = null;
		}

		[SecuritySafeCritical]
		internal static CultureInfo[] GetCultures(CultureTypes types)
		{
			if (types <= (CultureTypes)0 || (types & ~(CultureTypes.NeutralCultures | CultureTypes.SpecificCultures | CultureTypes.InstalledWin32Cultures | CultureTypes.UserCustomCulture | CultureTypes.ReplacementCultures | CultureTypes.WindowsOnlyCultures | CultureTypes.FrameworkCultures)) != (CultureTypes)0)
			{
				throw new ArgumentOutOfRangeException("types", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentOutOfRange_Range"), CultureTypes.NeutralCultures, CultureTypes.FrameworkCultures));
			}
			if ((types & CultureTypes.WindowsOnlyCultures) != (CultureTypes)0)
			{
				types &= ~CultureTypes.WindowsOnlyCultures;
			}
			string[] array = null;
			if (CultureData.nativeEnumCultureNames((int)types, JitHelpers.GetObjectHandleOnStack<string[]>(ref array)) == 0)
			{
				return new CultureInfo[0];
			}
			int num = array.Length;
			if ((types & (CultureTypes.NeutralCultures | CultureTypes.FrameworkCultures)) != (CultureTypes)0)
			{
				num += 2;
			}
			CultureInfo[] array2 = new CultureInfo[num];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = new CultureInfo(array[i]);
			}
			if ((types & (CultureTypes.NeutralCultures | CultureTypes.FrameworkCultures)) != (CultureTypes)0)
			{
				array2[array.Length] = new CultureInfo("zh-CHS");
				array2[array.Length + 1] = new CultureInfo("zh-CHT");
			}
			return array2;
		}

		private static CultureInfo[] SpecificCultures
		{
			get
			{
				if (CultureData.specificCultures == null)
				{
					CultureData.specificCultures = CultureData.GetCultures(CultureTypes.SpecificCultures);
				}
				return CultureData.specificCultures;
			}
		}

		internal bool IsReplacementCulture
		{
			get
			{
				return CultureData.IsReplacementCultureName(this.SNAME);
			}
		}

		[SecuritySafeCritical]
		private static bool IsReplacementCultureName(string name)
		{
			string[] array = CultureData.s_replacementCultureNames;
			if (array == null)
			{
				if (CultureData.nativeEnumCultureNames(16, JitHelpers.GetObjectHandleOnStack<string[]>(ref array)) == 0)
				{
					return false;
				}
				Array.Sort<string>(array);
				CultureData.s_replacementCultureNames = array;
			}
			return Array.BinarySearch<string>(array, name) >= 0;
		}

		internal string CultureName
		{
			get
			{
				string a = this.sName;
				if (a == "zh-CHS" || a == "zh-CHT")
				{
					return this.sName;
				}
				return this.sRealName;
			}
		}

		internal bool UseUserOverride
		{
			get
			{
				return this.bUseOverrides;
			}
		}

		internal string SNAME
		{
			get
			{
				if (this.sName == null)
				{
					this.sName = string.Empty;
				}
				return this.sName;
			}
		}

		internal string SPARENT
		{
			[SecurityCritical]
			get
			{
				if (this.sParent == null)
				{
					this.sParent = this.DoGetLocaleInfo(this.sRealName, 109U);
					string a = this.sParent;
					if (!(a == "zh-Hans"))
					{
						if (a == "zh-Hant")
						{
							this.sParent = "zh-CHT";
						}
					}
					else
					{
						this.sParent = "zh-CHS";
					}
				}
				return this.sParent;
			}
		}

		internal string SLOCALIZEDDISPLAYNAME
		{
			[SecurityCritical]
			get
			{
				if (this.sLocalizedDisplayName == null)
				{
					string text = "Globalization.ci_" + this.sName;
					if (CultureData.IsResourcePresent(text))
					{
						this.sLocalizedDisplayName = Environment.GetResourceString(text);
					}
					if (string.IsNullOrEmpty(this.sLocalizedDisplayName))
					{
						if (this.IsNeutralCulture)
						{
							this.sLocalizedDisplayName = this.SLOCALIZEDLANGUAGE;
						}
						else
						{
							if (CultureInfo.UserDefaultUICulture.Name.Equals(Thread.CurrentThread.CurrentUICulture.Name))
							{
								this.sLocalizedDisplayName = this.DoGetLocaleInfo(2U);
							}
							if (string.IsNullOrEmpty(this.sLocalizedDisplayName))
							{
								this.sLocalizedDisplayName = this.SNATIVEDISPLAYNAME;
							}
						}
					}
				}
				return this.sLocalizedDisplayName;
			}
		}

		internal string SENGDISPLAYNAME
		{
			[SecurityCritical]
			get
			{
				if (this.sEnglishDisplayName == null)
				{
					if (this.IsNeutralCulture)
					{
						this.sEnglishDisplayName = this.SENGLISHLANGUAGE;
						string a = this.sName;
						if (a == "zh-CHS" || a == "zh-CHT")
						{
							this.sEnglishDisplayName += " Legacy";
						}
					}
					else
					{
						this.sEnglishDisplayName = this.DoGetLocaleInfo(114U);
						if (string.IsNullOrEmpty(this.sEnglishDisplayName))
						{
							if (this.SENGLISHLANGUAGE.EndsWith(')'))
							{
								this.sEnglishDisplayName = this.SENGLISHLANGUAGE.Substring(0, this.sEnglishLanguage.Length - 1) + ", " + this.SENGCOUNTRY + ")";
							}
							else
							{
								this.sEnglishDisplayName = this.SENGLISHLANGUAGE + " (" + this.SENGCOUNTRY + ")";
							}
						}
					}
				}
				return this.sEnglishDisplayName;
			}
		}

		internal string SNATIVEDISPLAYNAME
		{
			[SecurityCritical]
			get
			{
				if (this.sNativeDisplayName == null)
				{
					if (this.IsNeutralCulture)
					{
						this.sNativeDisplayName = this.SNATIVELANGUAGE;
						string a = this.sName;
						if (!(a == "zh-CHS"))
						{
							if (a == "zh-CHT")
							{
								this.sNativeDisplayName += " 舊版";
							}
						}
						else
						{
							this.sNativeDisplayName += " 旧版";
						}
					}
					else
					{
						if (this.IsIncorrectNativeLanguageForSinhala())
						{
							this.sNativeDisplayName = "සිංහල (ශ්‍රී ලංකා)";
						}
						else
						{
							this.sNativeDisplayName = this.DoGetLocaleInfo(115U);
						}
						if (string.IsNullOrEmpty(this.sNativeDisplayName))
						{
							this.sNativeDisplayName = this.SNATIVELANGUAGE + " (" + this.SNATIVECOUNTRY + ")";
						}
					}
				}
				return this.sNativeDisplayName;
			}
		}

		internal string SSPECIFICCULTURE
		{
			get
			{
				return this.sSpecificCulture;
			}
		}

		internal string SISO639LANGNAME
		{
			[SecurityCritical]
			get
			{
				if (this.sISO639Language == null)
				{
					this.sISO639Language = this.DoGetLocaleInfo(89U);
				}
				return this.sISO639Language;
			}
		}

		internal string SISO639LANGNAME2
		{
			[SecurityCritical]
			get
			{
				if (this.sISO639Language2 == null)
				{
					this.sISO639Language2 = this.DoGetLocaleInfo(103U);
				}
				return this.sISO639Language2;
			}
		}

		internal string SABBREVLANGNAME
		{
			[SecurityCritical]
			get
			{
				if (this.sAbbrevLang == null)
				{
					this.sAbbrevLang = this.DoGetLocaleInfo(3U);
				}
				return this.sAbbrevLang;
			}
		}

		internal string SLOCALIZEDLANGUAGE
		{
			[SecurityCritical]
			get
			{
				if (this.sLocalizedLanguage == null)
				{
					if (CultureInfo.UserDefaultUICulture.Name.Equals(Thread.CurrentThread.CurrentUICulture.Name))
					{
						this.sLocalizedLanguage = this.DoGetLocaleInfo(111U);
					}
					if (string.IsNullOrEmpty(this.sLocalizedLanguage))
					{
						this.sLocalizedLanguage = this.SNATIVELANGUAGE;
					}
				}
				return this.sLocalizedLanguage;
			}
		}

		internal string SENGLISHLANGUAGE
		{
			[SecurityCritical]
			get
			{
				if (this.sEnglishLanguage == null)
				{
					this.sEnglishLanguage = this.DoGetLocaleInfo(4097U);
				}
				return this.sEnglishLanguage;
			}
		}

		internal string SNATIVELANGUAGE
		{
			[SecurityCritical]
			get
			{
				if (this.sNativeLanguage == null)
				{
					if (this.IsIncorrectNativeLanguageForSinhala())
					{
						this.sNativeLanguage = "සිංහල";
					}
					else
					{
						this.sNativeLanguage = this.DoGetLocaleInfo(4U);
					}
				}
				return this.sNativeLanguage;
			}
		}

		private bool IsIncorrectNativeLanguageForSinhala()
		{
			return CultureData.IsOsWin7OrPrior() && (this.sName == "si-LK" || this.sName == "si") && !this.IsReplacementCulture;
		}

		internal string SREGIONNAME
		{
			[SecurityCritical]
			get
			{
				if (this.sRegionName == null)
				{
					this.sRegionName = this.DoGetLocaleInfo(90U);
				}
				return this.sRegionName;
			}
		}

		internal int ICOUNTRY
		{
			get
			{
				return this.DoGetLocaleInfoInt(5U);
			}
		}

		internal int IGEOID
		{
			get
			{
				if (this.iGeoId == -1)
				{
					this.iGeoId = this.DoGetLocaleInfoInt(91U);
				}
				return this.iGeoId;
			}
		}

		internal string SLOCALIZEDCOUNTRY
		{
			[SecurityCritical]
			get
			{
				if (this.sLocalizedCountry == null)
				{
					string text = "Globalization.ri_" + this.SREGIONNAME;
					if (CultureData.IsResourcePresent(text))
					{
						this.sLocalizedCountry = Environment.GetResourceString(text);
					}
					if (string.IsNullOrEmpty(this.sLocalizedCountry))
					{
						if (CultureInfo.UserDefaultUICulture.Name.Equals(Thread.CurrentThread.CurrentUICulture.Name))
						{
							this.sLocalizedCountry = this.DoGetLocaleInfo(6U);
						}
						if (string.IsNullOrEmpty(this.sLocalizedDisplayName))
						{
							this.sLocalizedCountry = this.SNATIVECOUNTRY;
						}
					}
				}
				return this.sLocalizedCountry;
			}
		}

		internal string SENGCOUNTRY
		{
			[SecurityCritical]
			get
			{
				if (this.sEnglishCountry == null)
				{
					this.sEnglishCountry = this.DoGetLocaleInfo(4098U);
				}
				return this.sEnglishCountry;
			}
		}

		internal string SNATIVECOUNTRY
		{
			[SecurityCritical]
			get
			{
				if (this.sNativeCountry == null)
				{
					this.sNativeCountry = this.DoGetLocaleInfo(8U);
				}
				return this.sNativeCountry;
			}
		}

		internal string SISO3166CTRYNAME
		{
			[SecurityCritical]
			get
			{
				if (this.sISO3166CountryName == null)
				{
					this.sISO3166CountryName = this.DoGetLocaleInfo(90U);
				}
				return this.sISO3166CountryName;
			}
		}

		internal string SISO3166CTRYNAME2
		{
			[SecurityCritical]
			get
			{
				if (this.sISO3166CountryName2 == null)
				{
					this.sISO3166CountryName2 = this.DoGetLocaleInfo(104U);
				}
				return this.sISO3166CountryName2;
			}
		}

		internal string SABBREVCTRYNAME
		{
			[SecurityCritical]
			get
			{
				if (this.sAbbrevCountry == null)
				{
					this.sAbbrevCountry = this.DoGetLocaleInfo(7U);
				}
				return this.sAbbrevCountry;
			}
		}

		private int IDEFAULTCOUNTRY
		{
			get
			{
				return this.DoGetLocaleInfoInt(10U);
			}
		}

		internal int IINPUTLANGUAGEHANDLE
		{
			get
			{
				if (this.iInputLanguageHandle == -1)
				{
					if (this.IsSupplementalCustomCulture)
					{
						this.iInputLanguageHandle = 1033;
					}
					else
					{
						this.iInputLanguageHandle = this.ILANGUAGE;
					}
				}
				return this.iInputLanguageHandle;
			}
		}

		internal string SCONSOLEFALLBACKNAME
		{
			[SecurityCritical]
			get
			{
				if (this.sConsoleFallbackName == null)
				{
					string a = this.DoGetLocaleInfo(110U);
					if (a == "es-ES_tradnl")
					{
						a = "es-ES";
					}
					this.sConsoleFallbackName = a;
				}
				return this.sConsoleFallbackName;
			}
		}

		private bool ILEADINGZEROS
		{
			get
			{
				return this.DoGetLocaleInfoInt(18U) == 1;
			}
		}

		internal int[] WAGROUPING
		{
			[SecurityCritical]
			get
			{
				if (this.waGrouping == null || this.UseUserOverride)
				{
					this.waGrouping = CultureData.ConvertWin32GroupString(this.DoGetLocaleInfo(16U));
				}
				return this.waGrouping;
			}
		}

		internal string SNAN
		{
			[SecurityCritical]
			get
			{
				if (this.sNaN == null)
				{
					this.sNaN = this.DoGetLocaleInfo(105U);
				}
				return this.sNaN;
			}
		}

		internal string SPOSINFINITY
		{
			[SecurityCritical]
			get
			{
				if (this.sPositiveInfinity == null)
				{
					this.sPositiveInfinity = this.DoGetLocaleInfo(106U);
				}
				return this.sPositiveInfinity;
			}
		}

		internal string SNEGINFINITY
		{
			[SecurityCritical]
			get
			{
				if (this.sNegativeInfinity == null)
				{
					this.sNegativeInfinity = this.DoGetLocaleInfo(107U);
				}
				return this.sNegativeInfinity;
			}
		}

		internal int INEGATIVEPERCENT
		{
			get
			{
				if (this.iNegativePercent == -1)
				{
					this.iNegativePercent = this.DoGetLocaleInfoInt(116U);
				}
				return this.iNegativePercent;
			}
		}

		internal int IPOSITIVEPERCENT
		{
			get
			{
				if (this.iPositivePercent == -1)
				{
					this.iPositivePercent = this.DoGetLocaleInfoInt(117U);
				}
				return this.iPositivePercent;
			}
		}

		internal string SPERCENT
		{
			[SecurityCritical]
			get
			{
				if (this.sPercent == null)
				{
					this.sPercent = this.DoGetLocaleInfo(118U);
				}
				return this.sPercent;
			}
		}

		internal string SPERMILLE
		{
			[SecurityCritical]
			get
			{
				if (this.sPerMille == null)
				{
					this.sPerMille = this.DoGetLocaleInfo(119U);
				}
				return this.sPerMille;
			}
		}

		internal string SCURRENCY
		{
			[SecurityCritical]
			get
			{
				if (this.sCurrency == null || this.UseUserOverride)
				{
					this.sCurrency = this.DoGetLocaleInfo(20U);
				}
				return this.sCurrency;
			}
		}

		internal string SINTLSYMBOL
		{
			[SecurityCritical]
			get
			{
				if (this.sIntlMonetarySymbol == null)
				{
					this.sIntlMonetarySymbol = this.DoGetLocaleInfo(21U);
				}
				return this.sIntlMonetarySymbol;
			}
		}

		internal string SENGLISHCURRENCY
		{
			[SecurityCritical]
			get
			{
				if (this.sEnglishCurrency == null)
				{
					this.sEnglishCurrency = this.DoGetLocaleInfo(4103U);
				}
				return this.sEnglishCurrency;
			}
		}

		internal string SNATIVECURRENCY
		{
			[SecurityCritical]
			get
			{
				if (this.sNativeCurrency == null)
				{
					this.sNativeCurrency = this.DoGetLocaleInfo(4104U);
				}
				return this.sNativeCurrency;
			}
		}

		internal int[] WAMONGROUPING
		{
			[SecurityCritical]
			get
			{
				if (this.waMonetaryGrouping == null || this.UseUserOverride)
				{
					this.waMonetaryGrouping = CultureData.ConvertWin32GroupString(this.DoGetLocaleInfo(24U));
				}
				return this.waMonetaryGrouping;
			}
		}

		internal int IMEASURE
		{
			get
			{
				if (this.iMeasure == -1 || this.UseUserOverride)
				{
					this.iMeasure = this.DoGetLocaleInfoInt(13U);
				}
				return this.iMeasure;
			}
		}

		internal string SLIST
		{
			[SecurityCritical]
			get
			{
				if (this.sListSeparator == null || this.UseUserOverride)
				{
					this.sListSeparator = this.DoGetLocaleInfo(12U);
				}
				return this.sListSeparator;
			}
		}

		private int IPAPERSIZE
		{
			get
			{
				return this.DoGetLocaleInfoInt(4106U);
			}
		}

		internal string SAM1159
		{
			[SecurityCritical]
			get
			{
				if (this.sAM1159 == null || this.UseUserOverride)
				{
					this.sAM1159 = this.DoGetLocaleInfo(40U);
				}
				return this.sAM1159;
			}
		}

		internal string SPM2359
		{
			[SecurityCritical]
			get
			{
				if (this.sPM2359 == null || this.UseUserOverride)
				{
					this.sPM2359 = this.DoGetLocaleInfo(41U);
				}
				return this.sPM2359;
			}
		}

		internal string[] LongTimes
		{
			get
			{
				if (this.saLongTimes == null || this.UseUserOverride)
				{
					string[] array = this.DoEnumTimeFormats();
					if (array == null || array.Length == 0)
					{
						this.saLongTimes = CultureData.Invariant.saLongTimes;
					}
					else
					{
						this.saLongTimes = array;
					}
				}
				return this.saLongTimes;
			}
		}

		internal string[] ShortTimes
		{
			get
			{
				if (this.saShortTimes == null || this.UseUserOverride)
				{
					string[] array = this.DoEnumShortTimeFormats();
					if (array == null || array.Length == 0)
					{
						array = this.DeriveShortTimesFromLong();
					}
					this.saShortTimes = array;
				}
				return this.saShortTimes;
			}
		}

		private string[] DeriveShortTimesFromLong()
		{
			string[] array = new string[this.LongTimes.Length];
			for (int i = 0; i < this.LongTimes.Length; i++)
			{
				array[i] = CultureData.StripSecondsFromPattern(this.LongTimes[i]);
			}
			return array;
		}

		private static string StripSecondsFromPattern(string time)
		{
			bool flag = false;
			int num = -1;
			for (int i = 0; i < time.Length; i++)
			{
				if (time[i] == '\'')
				{
					flag = !flag;
				}
				else if (time[i] == '\\')
				{
					i++;
				}
				else if (!flag)
				{
					char c = time[i];
					if (c <= 'h')
					{
						if (c != 'H' && c != 'h')
						{
							goto IL_D3;
						}
					}
					else if (c != 'm')
					{
						if (c == 's')
						{
							if (i - num <= 4 && i - num > 1 && time[num + 1] != '\'' && time[i - 1] != '\'' && num >= 0)
							{
								i = num + 1;
							}
							bool flag2;
							int indexOfNextTokenAfterSeconds = CultureData.GetIndexOfNextTokenAfterSeconds(time, i, out flag2);
							StringBuilder stringBuilder = new StringBuilder(time.Substring(0, i));
							if (flag2)
							{
								stringBuilder.Append(' ');
							}
							stringBuilder.Append(time.Substring(indexOfNextTokenAfterSeconds));
							time = stringBuilder.ToString();
							goto IL_D3;
						}
						goto IL_D3;
					}
					num = i;
				}
				IL_D3:;
			}
			return time;
		}

		private static int GetIndexOfNextTokenAfterSeconds(string time, int index, out bool containsSpace)
		{
			bool flag = false;
			containsSpace = false;
			while (index < time.Length)
			{
				char c = time[index];
				if (c <= 'H')
				{
					if (c != ' ')
					{
						if (c != '\'')
						{
							if (c == 'H')
							{
								goto IL_63;
							}
						}
						else
						{
							flag = !flag;
						}
					}
					else
					{
						containsSpace = true;
					}
				}
				else if (c <= 'h')
				{
					if (c != '\\')
					{
						if (c == 'h')
						{
							goto IL_63;
						}
					}
					else
					{
						index++;
						if (time[index] == ' ')
						{
							containsSpace = true;
						}
					}
				}
				else if (c == 'm' || c == 't')
				{
					goto IL_63;
				}
				IL_68:
				index++;
				continue;
				IL_63:
				if (!flag)
				{
					return index;
				}
				goto IL_68;
			}
			containsSpace = false;
			return index;
		}

		internal string[] SADURATION
		{
			[SecurityCritical]
			get
			{
				if (this.saDurationFormats == null)
				{
					string str = this.DoGetLocaleInfo(93U);
					this.saDurationFormats = new string[]
					{
						CultureData.ReescapeWin32String(str)
					};
				}
				return this.saDurationFormats;
			}
		}

		internal int IFIRSTDAYOFWEEK
		{
			get
			{
				if (this.iFirstDayOfWeek == -1 || this.UseUserOverride)
				{
					this.iFirstDayOfWeek = CultureData.ConvertFirstDayOfWeekMonToSun(this.DoGetLocaleInfoInt(4108U));
				}
				return this.iFirstDayOfWeek;
			}
		}

		internal int IFIRSTWEEKOFYEAR
		{
			get
			{
				if (this.iFirstWeekOfYear == -1 || this.UseUserOverride)
				{
					this.iFirstWeekOfYear = this.DoGetLocaleInfoInt(4109U);
				}
				return this.iFirstWeekOfYear;
			}
		}

		internal string[] ShortDates(int calendarId)
		{
			return this.GetCalendar(calendarId).saShortDates;
		}

		internal string[] LongDates(int calendarId)
		{
			return this.GetCalendar(calendarId).saLongDates;
		}

		internal string[] YearMonths(int calendarId)
		{
			return this.GetCalendar(calendarId).saYearMonths;
		}

		internal string[] DayNames(int calendarId)
		{
			return this.GetCalendar(calendarId).saDayNames;
		}

		internal string[] AbbreviatedDayNames(int calendarId)
		{
			return this.GetCalendar(calendarId).saAbbrevDayNames;
		}

		internal string[] SuperShortDayNames(int calendarId)
		{
			return this.GetCalendar(calendarId).saSuperShortDayNames;
		}

		internal string[] MonthNames(int calendarId)
		{
			return this.GetCalendar(calendarId).saMonthNames;
		}

		internal string[] GenitiveMonthNames(int calendarId)
		{
			return this.GetCalendar(calendarId).saMonthGenitiveNames;
		}

		internal string[] AbbreviatedMonthNames(int calendarId)
		{
			return this.GetCalendar(calendarId).saAbbrevMonthNames;
		}

		internal string[] AbbreviatedGenitiveMonthNames(int calendarId)
		{
			return this.GetCalendar(calendarId).saAbbrevMonthGenitiveNames;
		}

		internal string[] LeapYearMonthNames(int calendarId)
		{
			return this.GetCalendar(calendarId).saLeapYearMonthNames;
		}

		internal string MonthDay(int calendarId)
		{
			return this.GetCalendar(calendarId).sMonthDay;
		}

		internal int[] CalendarIds
		{
			get
			{
				if (this.waCalendars == null)
				{
					int[] array = new int[23];
					int num = CalendarData.nativeGetCalendars(this.sWindowsName, this.bUseOverrides, array);
					if (num == 0)
					{
						this.waCalendars = CultureData.Invariant.waCalendars;
					}
					else
					{
						if (this.sWindowsName == "zh-TW")
						{
							bool flag = false;
							for (int i = 0; i < num; i++)
							{
								if (array[i] == 4)
								{
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								num++;
								Array.Copy(array, 1, array, 2, 21);
								array[1] = 4;
							}
						}
						int[] destinationArray = new int[num];
						Array.Copy(array, destinationArray, num);
						this.waCalendars = destinationArray;
					}
				}
				return this.waCalendars;
			}
		}

		internal string CalendarName(int calendarId)
		{
			return this.GetCalendar(calendarId).sNativeName;
		}

		internal CalendarData GetCalendar(int calendarId)
		{
			int num = calendarId - 1;
			if (this.calendars == null)
			{
				this.calendars = new CalendarData[23];
			}
			CalendarData calendarData = this.calendars[num];
			if (calendarData == null || this.UseUserOverride)
			{
				calendarData = new CalendarData(this.sWindowsName, calendarId, this.UseUserOverride);
				if (CultureData.IsOsWin7OrPrior() && !this.IsSupplementalCustomCulture && !this.IsReplacementCulture)
				{
					calendarData.FixupWin7MonthDaySemicolonBug();
				}
				this.calendars[num] = calendarData;
			}
			return calendarData;
		}

		internal int CurrentEra(int calendarId)
		{
			return this.GetCalendar(calendarId).iCurrentEra;
		}

		internal bool IsRightToLeft
		{
			get
			{
				return this.IREADINGLAYOUT == 1;
			}
		}

		private int IREADINGLAYOUT
		{
			get
			{
				if (this.iReadingLayout == -1)
				{
					this.iReadingLayout = this.DoGetLocaleInfoInt(112U);
				}
				return this.iReadingLayout;
			}
		}

		internal string STEXTINFO
		{
			[SecuritySafeCritical]
			get
			{
				if (this.sTextInfo == null)
				{
					if (this.IsNeutralCulture || this.IsSupplementalCustomCulture)
					{
						string cultureName = this.DoGetLocaleInfo(123U);
						this.sTextInfo = CultureData.GetCultureData(cultureName, this.bUseOverrides).SNAME;
					}
					if (this.sTextInfo == null)
					{
						this.sTextInfo = this.SNAME;
					}
				}
				return this.sTextInfo;
			}
		}

		internal string SCOMPAREINFO
		{
			[SecuritySafeCritical]
			get
			{
				if (this.sCompareInfo == null)
				{
					if (this.IsSupplementalCustomCulture)
					{
						this.sCompareInfo = this.DoGetLocaleInfo(123U);
					}
					if (this.sCompareInfo == null)
					{
						this.sCompareInfo = this.sWindowsName;
					}
				}
				return this.sCompareInfo;
			}
		}

		internal bool IsSupplementalCustomCulture
		{
			get
			{
				return CultureData.IsCustomCultureId(this.ILANGUAGE);
			}
		}

		private string SSCRIPTS
		{
			[SecuritySafeCritical]
			get
			{
				if (this.sScripts == null)
				{
					this.sScripts = this.DoGetLocaleInfo(108U);
				}
				return this.sScripts;
			}
		}

		private string SOPENTYPELANGUAGETAG
		{
			[SecuritySafeCritical]
			get
			{
				return this.DoGetLocaleInfo(122U);
			}
		}

		private string FONTSIGNATURE
		{
			[SecuritySafeCritical]
			get
			{
				if (this.fontSignature == null)
				{
					this.fontSignature = this.DoGetLocaleInfo(88U);
				}
				return this.fontSignature;
			}
		}

		private string SKEYBOARDSTOINSTALL
		{
			[SecuritySafeCritical]
			get
			{
				return this.DoGetLocaleInfo(94U);
			}
		}

		internal int IDEFAULTANSICODEPAGE
		{
			get
			{
				if (this.iDefaultAnsiCodePage == -1)
				{
					this.iDefaultAnsiCodePage = this.DoGetLocaleInfoInt(4100U);
				}
				return this.iDefaultAnsiCodePage;
			}
		}

		internal int IDEFAULTOEMCODEPAGE
		{
			get
			{
				if (this.iDefaultOemCodePage == -1)
				{
					this.iDefaultOemCodePage = this.DoGetLocaleInfoInt(11U);
				}
				return this.iDefaultOemCodePage;
			}
		}

		internal int IDEFAULTMACCODEPAGE
		{
			get
			{
				if (this.iDefaultMacCodePage == -1)
				{
					this.iDefaultMacCodePage = this.DoGetLocaleInfoInt(4113U);
				}
				return this.iDefaultMacCodePage;
			}
		}

		internal int IDEFAULTEBCDICCODEPAGE
		{
			get
			{
				if (this.iDefaultEbcdicCodePage == -1)
				{
					this.iDefaultEbcdicCodePage = this.DoGetLocaleInfoInt(4114U);
				}
				return this.iDefaultEbcdicCodePage;
			}
		}

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int LocaleNameToLCID(string localeName);

		internal int ILANGUAGE
		{
			get
			{
				if (this.iLanguage == 0)
				{
					this.iLanguage = CultureData.LocaleNameToLCID(this.sRealName);
				}
				return this.iLanguage;
			}
		}

		internal bool IsWin32Installed
		{
			get
			{
				return this.bWin32Installed;
			}
		}

		internal bool IsFramework
		{
			get
			{
				return this.bFramework;
			}
		}

		internal bool IsNeutralCulture
		{
			get
			{
				return this.bNeutral;
			}
		}

		internal bool IsInvariantCulture
		{
			get
			{
				return string.IsNullOrEmpty(this.SNAME);
			}
		}

		internal Calendar DefaultCalendar
		{
			get
			{
				int num = this.DoGetLocaleInfoInt(4105U);
				if (num == 0)
				{
					num = this.CalendarIds[0];
				}
				return CultureInfo.GetCalendarInstance(num);
			}
		}

		internal string[] EraNames(int calendarId)
		{
			return this.GetCalendar(calendarId).saEraNames;
		}

		internal string[] AbbrevEraNames(int calendarId)
		{
			return this.GetCalendar(calendarId).saAbbrevEraNames;
		}

		internal string[] AbbreviatedEnglishEraNames(int calendarId)
		{
			return this.GetCalendar(calendarId).saAbbrevEnglishEraNames;
		}

		internal string TimeSeparator
		{
			[SecuritySafeCritical]
			get
			{
				if (this.sTimeSeparator == null || this.UseUserOverride)
				{
					string text = CultureData.ReescapeWin32String(this.DoGetLocaleInfo(4099U));
					if (string.IsNullOrEmpty(text))
					{
						text = this.LongTimes[0];
					}
					this.sTimeSeparator = CultureData.GetTimeSeparator(text);
				}
				return this.sTimeSeparator;
			}
		}

		internal string DateSeparator(int calendarId)
		{
			if (calendarId == 3 && !AppContextSwitches.EnforceLegacyJapaneseDateParsing)
			{
				return "/";
			}
			return CultureData.GetDateSeparator(this.ShortDates(calendarId)[0]);
		}

		private static string UnescapeNlsString(string str, int start, int end)
		{
			StringBuilder stringBuilder = null;
			int num = start;
			while (num < str.Length && num <= end)
			{
				char c = str[num];
				if (c != '\'')
				{
					if (c != '\\')
					{
						if (stringBuilder != null)
						{
							stringBuilder.Append(str[num]);
						}
					}
					else
					{
						if (stringBuilder == null)
						{
							stringBuilder = new StringBuilder(str, start, num - start, str.Length);
						}
						num++;
						if (num < str.Length)
						{
							stringBuilder.Append(str[num]);
						}
					}
				}
				else if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder(str, start, num - start, str.Length);
				}
				num++;
			}
			if (stringBuilder == null)
			{
				return str.Substring(start, end - start + 1);
			}
			return stringBuilder.ToString();
		}

		internal static string ReescapeWin32String(string str)
		{
			if (str == null)
			{
				return null;
			}
			StringBuilder stringBuilder = null;
			bool flag = false;
			int i = 0;
			while (i < str.Length)
			{
				if (str[i] == '\'')
				{
					if (!flag)
					{
						flag = true;
						goto IL_91;
					}
					if (i + 1 >= str.Length || str[i + 1] != '\'')
					{
						flag = false;
						goto IL_91;
					}
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder(str, 0, i, str.Length * 2);
					}
					stringBuilder.Append("\\'");
					i++;
				}
				else
				{
					if (str[i] != '\\')
					{
						goto IL_91;
					}
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder(str, 0, i, str.Length * 2);
					}
					stringBuilder.Append("\\\\");
				}
				IL_A2:
				i++;
				continue;
				IL_91:
				if (stringBuilder != null)
				{
					stringBuilder.Append(str[i]);
					goto IL_A2;
				}
				goto IL_A2;
			}
			if (stringBuilder == null)
			{
				return str;
			}
			return stringBuilder.ToString();
		}

		internal static string[] ReescapeWin32Strings(string[] array)
		{
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = CultureData.ReescapeWin32String(array[i]);
				}
			}
			return array;
		}

		private static string GetTimeSeparator(string format)
		{
			return CultureData.GetSeparator(format, "Hhms");
		}

		private static string GetDateSeparator(string format)
		{
			return CultureData.GetSeparator(format, "dyM");
		}

		private static string GetSeparator(string format, string timeParts)
		{
			int num = CultureData.IndexOfTimePart(format, 0, timeParts);
			if (num != -1)
			{
				char c = format[num];
				do
				{
					num++;
				}
				while (num < format.Length && format[num] == c);
				int num2 = num;
				if (num2 < format.Length)
				{
					int num3 = CultureData.IndexOfTimePart(format, num2, timeParts);
					if (num3 != -1)
					{
						return CultureData.UnescapeNlsString(format, num2, num3 - 1);
					}
				}
			}
			return string.Empty;
		}

		private static int IndexOfTimePart(string format, int startIndex, string timeParts)
		{
			bool flag = false;
			for (int i = startIndex; i < format.Length; i++)
			{
				if (!flag && timeParts.IndexOf(format[i]) != -1)
				{
					return i;
				}
				char c = format[i];
				if (c != '\'')
				{
					if (c == '\\' && i + 1 < format.Length)
					{
						i++;
						c = format[i];
						if (c != '\'' && c != '\\')
						{
							i--;
						}
					}
				}
				else
				{
					flag = !flag;
				}
			}
			return -1;
		}

		[SecurityCritical]
		private string DoGetLocaleInfo(uint lctype)
		{
			return this.DoGetLocaleInfo(this.sWindowsName, lctype);
		}

		[SecurityCritical]
		private string DoGetLocaleInfo(string localeName, uint lctype)
		{
			if (!this.UseUserOverride)
			{
				lctype |= 2147483648U;
			}
			string text = CultureInfo.nativeGetLocaleInfoEx(localeName, lctype);
			if (text == null)
			{
				text = string.Empty;
			}
			return text;
		}

		private int DoGetLocaleInfoInt(uint lctype)
		{
			if (!this.UseUserOverride)
			{
				lctype |= 2147483648U;
			}
			return CultureInfo.nativeGetLocaleInfoExInt(this.sWindowsName, lctype);
		}

		private string[] DoEnumTimeFormats()
		{
			return CultureData.ReescapeWin32Strings(CultureData.nativeEnumTimeFormats(this.sWindowsName, 0U, this.UseUserOverride));
		}

		private string[] DoEnumShortTimeFormats()
		{
			return CultureData.ReescapeWin32Strings(CultureData.nativeEnumTimeFormats(this.sWindowsName, 2U, this.UseUserOverride));
		}

		internal static bool IsCustomCultureId(int cultureId)
		{
			return cultureId == 3072 || cultureId == 4096;
		}

		[SecurityCritical]
		internal void GetNFIValues(NumberFormatInfo nfi)
		{
			if (this.IsInvariantCulture)
			{
				nfi.positiveSign = this.sPositiveSign;
				nfi.negativeSign = this.sNegativeSign;
				nfi.nativeDigits = this.saNativeDigits;
				nfi.digitSubstitution = this.iDigitSubstitution;
				nfi.numberGroupSeparator = this.sThousandSeparator;
				nfi.numberDecimalSeparator = this.sDecimalSeparator;
				nfi.numberDecimalDigits = this.iDigits;
				nfi.numberNegativePattern = this.iNegativeNumber;
				nfi.currencySymbol = this.sCurrency;
				nfi.currencyGroupSeparator = this.sMonetaryThousand;
				nfi.currencyDecimalSeparator = this.sMonetaryDecimal;
				nfi.currencyDecimalDigits = this.iCurrencyDigits;
				nfi.currencyNegativePattern = this.iNegativeCurrency;
				nfi.currencyPositivePattern = this.iCurrency;
			}
			else
			{
				CultureData.nativeGetNumberFormatInfoValues(this.sWindowsName, nfi, this.UseUserOverride);
			}
			nfi.numberGroupSizes = this.WAGROUPING;
			nfi.currencyGroupSizes = this.WAMONGROUPING;
			nfi.percentNegativePattern = this.INEGATIVEPERCENT;
			nfi.percentPositivePattern = this.IPOSITIVEPERCENT;
			nfi.percentSymbol = this.SPERCENT;
			nfi.perMilleSymbol = this.SPERMILLE;
			nfi.negativeInfinitySymbol = this.SNEGINFINITY;
			nfi.positiveInfinitySymbol = this.SPOSINFINITY;
			nfi.nanSymbol = this.SNAN;
			nfi.percentDecimalDigits = nfi.numberDecimalDigits;
			nfi.percentDecimalSeparator = nfi.numberDecimalSeparator;
			nfi.percentGroupSizes = nfi.numberGroupSizes;
			nfi.percentGroupSeparator = nfi.numberGroupSeparator;
			if (nfi.positiveSign == null || nfi.positiveSign.Length == 0)
			{
				nfi.positiveSign = "+";
			}
			if (nfi.currencyDecimalSeparator == null || nfi.currencyDecimalSeparator.Length == 0)
			{
				nfi.currencyDecimalSeparator = nfi.numberDecimalSeparator;
			}
			if (932 == this.IDEFAULTANSICODEPAGE || 949 == this.IDEFAULTANSICODEPAGE)
			{
				nfi.ansiCurrencySymbol = "\\";
			}
		}

		private static int ConvertFirstDayOfWeekMonToSun(int iTemp)
		{
			iTemp++;
			if (iTemp > 6)
			{
				iTemp = 0;
			}
			return iTemp;
		}

		internal static string AnsiToLower(string testString)
		{
			StringBuilder stringBuilder = new StringBuilder(testString.Length);
			foreach (char c in testString)
			{
				stringBuilder.Append((c <= 'Z' && c >= 'A') ? (c - 'A' + 'a') : c);
			}
			return stringBuilder.ToString();
		}

		private static int[] ConvertWin32GroupString(string win32Str)
		{
			if (win32Str == null || win32Str.Length == 0)
			{
				return new int[]
				{
					3
				};
			}
			if (win32Str[0] == '0')
			{
				return new int[1];
			}
			int[] array;
			if (win32Str[win32Str.Length - 1] == '0')
			{
				array = new int[win32Str.Length / 2];
			}
			else
			{
				array = new int[win32Str.Length / 2 + 2];
				array[array.Length - 1] = 0;
			}
			int num = 0;
			int num2 = 0;
			while (num < win32Str.Length && num2 < array.Length)
			{
				if (win32Str[num] < '1' || win32Str[num] > '9')
				{
					return new int[]
					{
						3
					};
				}
				array[num2] = (int)(win32Str[num] - '0');
				num += 2;
				num2++;
			}
			return array;
		}

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool nativeInitCultureData(CultureData cultureData);

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool nativeGetNumberFormatInfoValues(string localeName, NumberFormatInfo nfi, bool useUserOverride);

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] nativeEnumTimeFormats(string localeName, uint dwFlags, bool useUserOverride);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		internal static extern int nativeEnumCultureNames(int cultureTypes, ObjectHandleOnStack retStringArray);

		private const int undef = -1;

		private string sRealName;

		private string sWindowsName;

		private string sName;

		private string sParent;

		private string sLocalizedDisplayName;

		private string sEnglishDisplayName;

		private string sNativeDisplayName;

		private string sSpecificCulture;

		private string sISO639Language;

		private string sLocalizedLanguage;

		private string sEnglishLanguage;

		private string sNativeLanguage;

		private string sRegionName;

		private int iGeoId = -1;

		private string sLocalizedCountry;

		private string sEnglishCountry;

		private string sNativeCountry;

		private string sISO3166CountryName;

		private string sPositiveSign;

		private string sNegativeSign;

		private string[] saNativeDigits;

		private int iDigitSubstitution;

		private int iLeadingZeros;

		private int iDigits;

		private int iNegativeNumber;

		private int[] waGrouping;

		private string sDecimalSeparator;

		private string sThousandSeparator;

		private string sNaN;

		private string sPositiveInfinity;

		private string sNegativeInfinity;

		private int iNegativePercent = -1;

		private int iPositivePercent = -1;

		private string sPercent;

		private string sPerMille;

		private string sCurrency;

		private string sIntlMonetarySymbol;

		private string sEnglishCurrency;

		private string sNativeCurrency;

		private int iCurrencyDigits;

		private int iCurrency;

		private int iNegativeCurrency;

		private int[] waMonetaryGrouping;

		private string sMonetaryDecimal;

		private string sMonetaryThousand;

		private int iMeasure = -1;

		private string sListSeparator;

		private string sAM1159;

		private string sPM2359;

		private string sTimeSeparator;

		private volatile string[] saLongTimes;

		private volatile string[] saShortTimes;

		private volatile string[] saDurationFormats;

		private int iFirstDayOfWeek = -1;

		private int iFirstWeekOfYear = -1;

		private volatile int[] waCalendars;

		private CalendarData[] calendars;

		private int iReadingLayout = -1;

		private string sTextInfo;

		private string sCompareInfo;

		private string sScripts;

		private int iDefaultAnsiCodePage = -1;

		private int iDefaultOemCodePage = -1;

		private int iDefaultMacCodePage = -1;

		private int iDefaultEbcdicCodePage = -1;

		private int iLanguage;

		private string sAbbrevLang;

		private string sAbbrevCountry;

		private string sISO639Language2;

		private string sISO3166CountryName2;

		private int iInputLanguageHandle = -1;

		private string sConsoleFallbackName;

		private string sKeyboardsToInstall;

		private string fontSignature;

		private bool bUseOverrides;

		private bool bNeutral;

		private bool bWin32Installed;

		private bool bFramework;

		private static volatile Dictionary<string, string> s_RegionNames;

		private static volatile CultureData s_Invariant;

		internal static volatile ResourceSet MscorlibResourceSet;

		private static volatile Dictionary<string, CultureData> s_cachedCultures;

		private static readonly Version s_win7Version = new Version(6, 1);

		private static string s_RegionKey = "System\\CurrentControlSet\\Control\\Nls\\RegionMapping";

		private static volatile Dictionary<string, CultureData> s_cachedRegions;

		internal static volatile CultureInfo[] specificCultures;

		internal static volatile string[] s_replacementCultureNames;

		private const uint LOCALE_NOUSEROVERRIDE = 2147483648U;

		private const uint LOCALE_RETURN_NUMBER = 536870912U;

		private const uint LOCALE_RETURN_GENITIVE_NAMES = 268435456U;

		private const uint LOCALE_SLOCALIZEDDISPLAYNAME = 2U;

		private const uint LOCALE_SENGLISHDISPLAYNAME = 114U;

		private const uint LOCALE_SNATIVEDISPLAYNAME = 115U;

		private const uint LOCALE_SLOCALIZEDLANGUAGENAME = 111U;

		private const uint LOCALE_SENGLISHLANGUAGENAME = 4097U;

		private const uint LOCALE_SNATIVELANGUAGENAME = 4U;

		private const uint LOCALE_SLOCALIZEDCOUNTRYNAME = 6U;

		private const uint LOCALE_SENGLISHCOUNTRYNAME = 4098U;

		private const uint LOCALE_SNATIVECOUNTRYNAME = 8U;

		private const uint LOCALE_SABBREVLANGNAME = 3U;

		private const uint LOCALE_ICOUNTRY = 5U;

		private const uint LOCALE_SABBREVCTRYNAME = 7U;

		private const uint LOCALE_IGEOID = 91U;

		private const uint LOCALE_IDEFAULTLANGUAGE = 9U;

		private const uint LOCALE_IDEFAULTCOUNTRY = 10U;

		private const uint LOCALE_IDEFAULTCODEPAGE = 11U;

		private const uint LOCALE_IDEFAULTANSICODEPAGE = 4100U;

		private const uint LOCALE_IDEFAULTMACCODEPAGE = 4113U;

		private const uint LOCALE_SLIST = 12U;

		private const uint LOCALE_IMEASURE = 13U;

		private const uint LOCALE_SDECIMAL = 14U;

		private const uint LOCALE_STHOUSAND = 15U;

		private const uint LOCALE_SGROUPING = 16U;

		private const uint LOCALE_IDIGITS = 17U;

		private const uint LOCALE_ILZERO = 18U;

		private const uint LOCALE_INEGNUMBER = 4112U;

		private const uint LOCALE_SNATIVEDIGITS = 19U;

		private const uint LOCALE_SCURRENCY = 20U;

		private const uint LOCALE_SINTLSYMBOL = 21U;

		private const uint LOCALE_SMONDECIMALSEP = 22U;

		private const uint LOCALE_SMONTHOUSANDSEP = 23U;

		private const uint LOCALE_SMONGROUPING = 24U;

		private const uint LOCALE_ICURRDIGITS = 25U;

		private const uint LOCALE_IINTLCURRDIGITS = 26U;

		private const uint LOCALE_ICURRENCY = 27U;

		private const uint LOCALE_INEGCURR = 28U;

		private const uint LOCALE_SDATE = 29U;

		private const uint LOCALE_STIME = 30U;

		private const uint LOCALE_SSHORTDATE = 31U;

		private const uint LOCALE_SLONGDATE = 32U;

		private const uint LOCALE_STIMEFORMAT = 4099U;

		private const uint LOCALE_IDATE = 33U;

		private const uint LOCALE_ILDATE = 34U;

		private const uint LOCALE_ITIME = 35U;

		private const uint LOCALE_ITIMEMARKPOSN = 4101U;

		private const uint LOCALE_ICENTURY = 36U;

		private const uint LOCALE_ITLZERO = 37U;

		private const uint LOCALE_IDAYLZERO = 38U;

		private const uint LOCALE_IMONLZERO = 39U;

		private const uint LOCALE_S1159 = 40U;

		private const uint LOCALE_S2359 = 41U;

		private const uint LOCALE_ICALENDARTYPE = 4105U;

		private const uint LOCALE_IOPTIONALCALENDAR = 4107U;

		private const uint LOCALE_IFIRSTDAYOFWEEK = 4108U;

		private const uint LOCALE_IFIRSTWEEKOFYEAR = 4109U;

		private const uint LOCALE_SDAYNAME1 = 42U;

		private const uint LOCALE_SDAYNAME2 = 43U;

		private const uint LOCALE_SDAYNAME3 = 44U;

		private const uint LOCALE_SDAYNAME4 = 45U;

		private const uint LOCALE_SDAYNAME5 = 46U;

		private const uint LOCALE_SDAYNAME6 = 47U;

		private const uint LOCALE_SDAYNAME7 = 48U;

		private const uint LOCALE_SABBREVDAYNAME1 = 49U;

		private const uint LOCALE_SABBREVDAYNAME2 = 50U;

		private const uint LOCALE_SABBREVDAYNAME3 = 51U;

		private const uint LOCALE_SABBREVDAYNAME4 = 52U;

		private const uint LOCALE_SABBREVDAYNAME5 = 53U;

		private const uint LOCALE_SABBREVDAYNAME6 = 54U;

		private const uint LOCALE_SABBREVDAYNAME7 = 55U;

		private const uint LOCALE_SMONTHNAME1 = 56U;

		private const uint LOCALE_SMONTHNAME2 = 57U;

		private const uint LOCALE_SMONTHNAME3 = 58U;

		private const uint LOCALE_SMONTHNAME4 = 59U;

		private const uint LOCALE_SMONTHNAME5 = 60U;

		private const uint LOCALE_SMONTHNAME6 = 61U;

		private const uint LOCALE_SMONTHNAME7 = 62U;

		private const uint LOCALE_SMONTHNAME8 = 63U;

		private const uint LOCALE_SMONTHNAME9 = 64U;

		private const uint LOCALE_SMONTHNAME10 = 65U;

		private const uint LOCALE_SMONTHNAME11 = 66U;

		private const uint LOCALE_SMONTHNAME12 = 67U;

		private const uint LOCALE_SMONTHNAME13 = 4110U;

		private const uint LOCALE_SABBREVMONTHNAME1 = 68U;

		private const uint LOCALE_SABBREVMONTHNAME2 = 69U;

		private const uint LOCALE_SABBREVMONTHNAME3 = 70U;

		private const uint LOCALE_SABBREVMONTHNAME4 = 71U;

		private const uint LOCALE_SABBREVMONTHNAME5 = 72U;

		private const uint LOCALE_SABBREVMONTHNAME6 = 73U;

		private const uint LOCALE_SABBREVMONTHNAME7 = 74U;

		private const uint LOCALE_SABBREVMONTHNAME8 = 75U;

		private const uint LOCALE_SABBREVMONTHNAME9 = 76U;

		private const uint LOCALE_SABBREVMONTHNAME10 = 77U;

		private const uint LOCALE_SABBREVMONTHNAME11 = 78U;

		private const uint LOCALE_SABBREVMONTHNAME12 = 79U;

		private const uint LOCALE_SABBREVMONTHNAME13 = 4111U;

		private const uint LOCALE_SPOSITIVESIGN = 80U;

		private const uint LOCALE_SNEGATIVESIGN = 81U;

		private const uint LOCALE_IPOSSIGNPOSN = 82U;

		private const uint LOCALE_INEGSIGNPOSN = 83U;

		private const uint LOCALE_IPOSSYMPRECEDES = 84U;

		private const uint LOCALE_IPOSSEPBYSPACE = 85U;

		private const uint LOCALE_INEGSYMPRECEDES = 86U;

		private const uint LOCALE_INEGSEPBYSPACE = 87U;

		private const uint LOCALE_FONTSIGNATURE = 88U;

		private const uint LOCALE_SISO639LANGNAME = 89U;

		private const uint LOCALE_SISO3166CTRYNAME = 90U;

		private const uint LOCALE_IDEFAULTEBCDICCODEPAGE = 4114U;

		private const uint LOCALE_IPAPERSIZE = 4106U;

		private const uint LOCALE_SENGCURRNAME = 4103U;

		private const uint LOCALE_SNATIVECURRNAME = 4104U;

		private const uint LOCALE_SYEARMONTH = 4102U;

		private const uint LOCALE_SSORTNAME = 4115U;

		private const uint LOCALE_IDIGITSUBSTITUTION = 4116U;

		private const uint LOCALE_SNAME = 92U;

		private const uint LOCALE_SDURATION = 93U;

		private const uint LOCALE_SKEYBOARDSTOINSTALL = 94U;

		private const uint LOCALE_SSHORTESTDAYNAME1 = 96U;

		private const uint LOCALE_SSHORTESTDAYNAME2 = 97U;

		private const uint LOCALE_SSHORTESTDAYNAME3 = 98U;

		private const uint LOCALE_SSHORTESTDAYNAME4 = 99U;

		private const uint LOCALE_SSHORTESTDAYNAME5 = 100U;

		private const uint LOCALE_SSHORTESTDAYNAME6 = 101U;

		private const uint LOCALE_SSHORTESTDAYNAME7 = 102U;

		private const uint LOCALE_SISO639LANGNAME2 = 103U;

		private const uint LOCALE_SISO3166CTRYNAME2 = 104U;

		private const uint LOCALE_SNAN = 105U;

		private const uint LOCALE_SPOSINFINITY = 106U;

		private const uint LOCALE_SNEGINFINITY = 107U;

		private const uint LOCALE_SSCRIPTS = 108U;

		private const uint LOCALE_SPARENT = 109U;

		private const uint LOCALE_SCONSOLEFALLBACKNAME = 110U;

		private const uint LOCALE_IREADINGLAYOUT = 112U;

		private const uint LOCALE_INEUTRAL = 113U;

		private const uint LOCALE_INEGATIVEPERCENT = 116U;

		private const uint LOCALE_IPOSITIVEPERCENT = 117U;

		private const uint LOCALE_SPERCENT = 118U;

		private const uint LOCALE_SPERMILLE = 119U;

		private const uint LOCALE_SMONTHDAY = 120U;

		private const uint LOCALE_SSHORTTIME = 121U;

		private const uint LOCALE_SOPENTYPELANGUAGETAG = 122U;

		private const uint LOCALE_SSORTLOCALE = 123U;

		internal const uint TIME_NOSECONDS = 2U;
	}
}
