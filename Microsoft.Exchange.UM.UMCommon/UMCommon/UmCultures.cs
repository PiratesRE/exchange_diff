using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Microsoft.Exchange.Data.Globalization;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class UmCultures
	{
		private UmCultures()
		{
			this.promptCultures = UmCultures.BuildSupportedPromptCultures();
			this.clientCultures = UmCultures.BuildSupportedClientCultures();
			this.disambiguousLanguageFamilies = this.BuildDisambiguousLanguageFamilies();
		}

		internal static int IndexOfIETFLanguage(List<CultureInfo> list, CultureInfo key)
		{
			if (list == null || key == null)
			{
				return -1;
			}
			string ietfLanguageTag = key.IetfLanguageTag;
			for (int i = 0; i < list.Count; i++)
			{
				string ietfLanguageTag2 = list[i].IetfLanguageTag;
				if (ietfLanguageTag.Equals(ietfLanguageTag2, StringComparison.OrdinalIgnoreCase))
				{
					return i;
				}
			}
			return -1;
		}

		internal static int IndexOfParentIETFLanguage(List<CultureInfo> list, CultureInfo key)
		{
			if (list == null || key == null || key.Parent == null)
			{
				return -1;
			}
			string ietfLanguageTag = key.Parent.IetfLanguageTag;
			for (int i = 0; i < list.Count; i++)
			{
				string ietfLanguageTag2 = list[i].IetfLanguageTag;
				if (ietfLanguageTag.Equals(ietfLanguageTag2, StringComparison.OrdinalIgnoreCase))
				{
					return i;
				}
			}
			return -1;
		}

		internal static int IndexOfThreeLetterISOLanguage(List<CultureInfo> list, CultureInfo key)
		{
			if (list == null || key == null)
			{
				return -1;
			}
			string threeLetterISOLanguageName = key.ThreeLetterISOLanguageName;
			for (int i = 0; i < list.Count; i++)
			{
				string threeLetterISOLanguageName2 = list[i].ThreeLetterISOLanguageName;
				if (threeLetterISOLanguageName.Equals(threeLetterISOLanguageName2, StringComparison.OrdinalIgnoreCase))
				{
					return i;
				}
			}
			return -1;
		}

		internal static int IndexOfThreeLetterWindowsLanguageName(List<CultureInfo> list, CultureInfo key)
		{
			if (list == null || key == null)
			{
				return -1;
			}
			string threeLetterWindowsLanguageName = key.ThreeLetterWindowsLanguageName;
			for (int i = 0; i < list.Count; i++)
			{
				string threeLetterWindowsLanguageName2 = list[i].ThreeLetterWindowsLanguageName;
				if (threeLetterWindowsLanguageName.Equals(threeLetterWindowsLanguageName2, StringComparison.OrdinalIgnoreCase))
				{
					return i;
				}
			}
			return -1;
		}

		internal static int IndexOfTwoLetterISOLanguage(List<CultureInfo> list, CultureInfo key)
		{
			if (list == null || key == null)
			{
				return -1;
			}
			string twoLetterISOLanguageName = key.TwoLetterISOLanguageName;
			for (int i = 0; i < list.Count; i++)
			{
				string twoLetterISOLanguageName2 = list[i].TwoLetterISOLanguageName;
				if (twoLetterISOLanguageName.Equals(twoLetterISOLanguageName2, StringComparison.OrdinalIgnoreCase))
				{
					return i;
				}
			}
			return -1;
		}

		internal static int IndexOfFallbackLanguage(List<CultureInfo> list, CultureInfo key)
		{
			int result = -1;
			if (list != null && key != null)
			{
				string twoLetterISOLanguageName = key.TwoLetterISOLanguageName;
				CultureInfo item = null;
				if (UmCultures.TryCreateFallbackCulture(twoLetterISOLanguageName, out item))
				{
					result = list.IndexOf(item);
				}
			}
			return result;
		}

		internal static bool TryCreateFallbackCulture(string keyIso, out CultureInfo fallback)
		{
			fallback = null;
			try
			{
				fallback = CultureInfo.CreateSpecificCulture(keyIso);
			}
			catch (ArgumentException)
			{
			}
			return null != fallback;
		}

		internal static void InvalidatePromptCulture(CultureInfo culture)
		{
			int num = UmCultures.GetSingleton().promptCultures.IndexOf(culture);
			if (num >= 0)
			{
				UmCultures.GetSingleton().promptCultures.RemoveAt(num);
			}
			if (UmCultures.SortedSupportedPromptCulturesLoader.SortedPromptCultures.ContainsKey(culture))
			{
				UmCultures.SortedSupportedPromptCulturesLoader.SortedPromptCultures.Remove(culture);
			}
		}

		internal static bool IsPromptCultureAvailable(CultureInfo culture)
		{
			return UmCultures.GetSingleton().promptCultures.IndexOf(culture) >= 0;
		}

		internal static List<CultureInfo> GetSupportedPromptCultures()
		{
			return UmCultures.GetSingleton().promptCultures;
		}

		internal static List<CultureInfo> GetSortedSupportedPromptCultures(CultureInfo key)
		{
			return UmCultures.SortedSupportedPromptCulturesLoader.SortedPromptCultures[key];
		}

		internal static List<CultureInfo> GetSupportedClientCultures()
		{
			return UmCultures.GetSingleton().clientCultures;
		}

		internal static CultureInfo GetBestSupportedPromptCulture(CultureInfo language)
		{
			if (language == null)
			{
				throw new ArgumentException("The proposed language is null");
			}
			return UmCultures.GetBestSupportedCulture(UmCultures.GetSupportedPromptCultures(), language);
		}

		internal static CultureInfo GetPreferredClientCulture(CultureInfo[] preferredCultures)
		{
			if (preferredCultures.Length > 0)
			{
				List<CultureInfo> list = UmCultures.GetSingleton().clientCultures;
				foreach (CultureInfo cultureInfo in preferredCultures)
				{
					if (list.Contains(cultureInfo))
					{
						return cultureInfo;
					}
				}
				if (Utils.RunningInTestMode)
				{
					return preferredCultures[0];
				}
			}
			return null;
		}

		internal static CultureInfo GetBestSupportedCulture(List<CultureInfo> cultures, CultureInfo language)
		{
			if (cultures == null || language == null)
			{
				return null;
			}
			if (0 <= cultures.IndexOf(language))
			{
				return language;
			}
			int index;
			if (-1 != (index = UmCultures.IndexOfIETFLanguage(cultures, language)))
			{
				return cultures[index];
			}
			if (-1 != (index = UmCultures.IndexOfFallbackLanguage(cultures, language)))
			{
				return cultures[index];
			}
			if (-1 != (index = UmCultures.IndexOfThreeLetterWindowsLanguageName(cultures, language)))
			{
				return cultures[index];
			}
			if (-1 != (index = UmCultures.IndexOfThreeLetterISOLanguage(cultures, language)))
			{
				return cultures[index];
			}
			if (-1 != (index = UmCultures.IndexOfTwoLetterISOLanguage(cultures, language)))
			{
				return cultures[index];
			}
			if (-1 != (index = UmCultures.IndexOfParentIETFLanguage(cultures, language)))
			{
				return cultures[index];
			}
			return null;
		}

		internal static CultureInfo GetDisambiguousLanguageFamily(CultureInfo language)
		{
			return UmCultures.GetSingleton().disambiguousLanguageFamilies[language];
		}

		internal static int GetLanguagePromptLCID(CultureInfo lang)
		{
			if (lang.Name == "es-MX")
			{
				return 58378;
			}
			return lang.LCID;
		}

		internal static CultureInfo GetGrxmlCulture(CultureInfo lang)
		{
			return lang;
		}

		private static UmCultures GetSingleton()
		{
			if (UmCultures.singleton == null)
			{
				UmCultures.singleton = new UmCultures();
			}
			return UmCultures.singleton;
		}

		private static Dictionary<CultureInfo, List<CultureInfo>> BuildSortedSupportedPromptCultures()
		{
			ResourceManager resourceManager = new ResourceManager("Microsoft.Exchange.UM.Prompts.Prompts.Strings", Assembly.Load("Microsoft.Exchange.UM.Prompts"));
			Dictionary<CultureInfo, List<CultureInfo>> dictionary = new Dictionary<CultureInfo, List<CultureInfo>>();
			foreach (CultureInfo cultureInfo in UmCultures.GetSupportedPromptCultures())
			{
				SortedDictionary<string, CultureInfo> sortedDictionary = new SortedDictionary<string, CultureInfo>(StringComparer.Create(cultureInfo, true));
				foreach (CultureInfo cultureInfo2 in UmCultures.GetSupportedPromptCultures())
				{
					string @string = resourceManager.GetString(string.Format(CultureInfo.InvariantCulture, "Language-{0}", new object[]
					{
						UmCultures.GetLanguagePromptLCID(cultureInfo2)
					}), cultureInfo);
					sortedDictionary.Add(@string, cultureInfo2);
				}
				dictionary.Add(cultureInfo, new List<CultureInfo>());
				foreach (CultureInfo item in sortedDictionary.Values)
				{
					dictionary[cultureInfo].Add(item);
				}
			}
			return dictionary;
		}

		private static List<CultureInfo> BuildSupportedPromptCultures()
		{
			return new List<CultureInfo>(LanguagePackInfo.GetInstalledLanguagePackCultures(LanguagePackType.UnifiedMessaging));
		}

		private static List<CultureInfo> BuildSupportedClientCultures()
		{
			return new List<CultureInfo>(LanguagePackInfo.GetInstalledLanguagePackSpecificCultures(LanguagePackType.Client));
		}

		private Dictionary<CultureInfo, CultureInfo> BuildDisambiguousLanguageFamilies()
		{
			Dictionary<CultureInfo, CultureInfo> dictionary = new Dictionary<CultureInfo, CultureInfo>();
			Dictionary<string, CultureInfo> dictionary2 = new Dictionary<string, CultureInfo>();
			foreach (CultureInfo cultureInfo in this.promptCultures)
			{
				if (!dictionary2.ContainsKey(cultureInfo.TwoLetterISOLanguageName))
				{
					dictionary.Add(cultureInfo, cultureInfo.Parent);
					dictionary2.Add(cultureInfo.TwoLetterISOLanguageName, cultureInfo);
				}
				else
				{
					dictionary.Add(cultureInfo, cultureInfo);
					CultureInfo cultureInfo2 = dictionary2[cultureInfo.TwoLetterISOLanguageName];
					dictionary[cultureInfo2] = cultureInfo2;
				}
			}
			return dictionary;
		}

		private static UmCultures singleton;

		private List<CultureInfo> promptCultures;

		private List<CultureInfo> clientCultures;

		private Dictionary<CultureInfo, CultureInfo> disambiguousLanguageFamilies;

		private static class SortedSupportedPromptCulturesLoader
		{
			internal static Dictionary<CultureInfo, List<CultureInfo>> SortedPromptCultures
			{
				get
				{
					return UmCultures.SortedSupportedPromptCulturesLoader.sortedPromptCultures;
				}
			}

			private static Dictionary<CultureInfo, List<CultureInfo>> sortedPromptCultures = UmCultures.BuildSortedSupportedPromptCultures();
		}
	}
}
