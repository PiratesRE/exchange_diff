using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Globalization;

namespace Microsoft.Exchange.Net
{
	internal static class ClientCultures
	{
		static ClientCultures()
		{
			CultureInfo[] installedLanguagePackCultures = LanguagePackInfo.GetInstalledLanguagePackCultures(LanguagePackType.Client);
			ClientCultures.dsnlocalizedLanguages = new Dictionary<int, bool>(installedLanguagePackCultures.Length);
			foreach (CultureInfo cultureInfo in installedLanguagePackCultures)
			{
				ClientCultures.dsnlocalizedLanguages[cultureInfo.LCID] = true;
			}
		}

		public static List<CultureInfo> SupportedCultureInfos
		{
			get
			{
				if (ClientCultures.supportedCultureInfos == null)
				{
					ClientCultures.supportedCultureInfos = new List<CultureInfo>(LanguagePackInfo.GetInstalledLanguagePackSpecificCultures(LanguagePackType.Client));
				}
				return ClientCultures.supportedCultureInfos;
			}
		}

		internal static CultureInfo GetPreferredCultureInfo(IEnumerable<CultureInfo> cultures)
		{
			foreach (CultureInfo cultureInfo in cultures)
			{
				if (ClientCultures.SupportedCultureInfos.Contains(cultureInfo))
				{
					return cultureInfo;
				}
			}
			return null;
		}

		public static bool IsSupportedCulture(CultureInfo culture)
		{
			return ClientCultures.SupportedCultureInfos.Contains(culture);
		}

		public static bool IsSupportedCulture(int lcid)
		{
			return lcid > 0 && ClientCultures.IsSupportedCulture(CultureInfo.GetCultureInfo(lcid));
		}

		public static CultureInfo[] GetAllSupportedDsnLanguages()
		{
			CultureInfo[] array = new CultureInfo[ClientCultures.dsnlocalizedLanguages.Count];
			int num = 0;
			foreach (int culture in ClientCultures.dsnlocalizedLanguages.Keys)
			{
				array[num++] = CultureInfo.GetCultureInfo(culture);
			}
			return array;
		}

		public static bool IsCultureSupportedForDsn(CultureInfo culture)
		{
			CultureInfo parent = culture.Parent;
			return ClientCultures.dsnlocalizedLanguages.ContainsKey(culture.LCID) || (parent.IsNeutralCulture && ClientCultures.dsnlocalizedLanguages.ContainsKey(parent.LCID));
		}

		public static bool IsCultureSupportedForDsnCustomization(CultureInfo culture)
		{
			return LanguagePackInfo.expectedCultureLcids.Contains(culture.LCID);
		}

		public const string DefaultClientLanguage = "en-US";

		private static Dictionary<int, bool> dsnlocalizedLanguages;

		private static List<CultureInfo> supportedCultureInfos = null;
	}
}
