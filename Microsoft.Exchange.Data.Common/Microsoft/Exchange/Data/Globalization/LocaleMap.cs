using System;
using System.Globalization;

namespace Microsoft.Exchange.Data.Globalization
{
	internal static class LocaleMap
	{
		public static CultureInfo GetCultureFromLcid(int lcid)
		{
			return CultureInfo.GetCultureInfo(lcid);
		}

		public static int GetLcidFromCulture(CultureInfo culture)
		{
			return culture.LCID;
		}

		public static int GetCompareLcidFromCulture(CultureInfo culture)
		{
			return culture.CompareInfo.LCID;
		}

		public static int GetANSICodePage(CultureInfo culture)
		{
			return culture.TextInfo.ANSICodePage;
		}

		public static CultureInfo GetSpecificCulture(string cultureName)
		{
			return CultureInfo.CreateSpecificCulture(cultureName);
		}
	}
}
