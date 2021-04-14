using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UmLanguagePackConfigurationInfo : InstallableUnitConfigurationInfo
	{
		public static decimal GetUmLanguagePackSizeForCultureInfo(CultureInfo umlang)
		{
			decimal result = RequiredDiskSpaceStatistics.MaximumSizeOfOneLanguagePack;
			if (umlang != null)
			{
				string key = umlang.ToString().ToLower();
				if (UmLanguagePackConfigurationInfo.UmLanguagePackSizes.ContainsKey(key))
				{
					result = UmLanguagePackConfigurationInfo.UmLanguagePackSizes[key];
				}
			}
			return result;
		}

		static UmLanguagePackConfigurationInfo()
		{
			UmLanguagePackConfigurationInfo.UmLanguagePackSizes.Add("de-de", RequiredDiskSpaceStatistics.DeDeLanguagePack);
			UmLanguagePackConfigurationInfo.UmLanguagePackSizes.Add("en-us", RequiredDiskSpaceStatistics.EnUsLanguagePack);
			UmLanguagePackConfigurationInfo.UmLanguagePackSizes.Add("en-au", RequiredDiskSpaceStatistics.EnAuLanguagePack);
			UmLanguagePackConfigurationInfo.UmLanguagePackSizes.Add("en-gb", RequiredDiskSpaceStatistics.EnGbLanguagePack);
			UmLanguagePackConfigurationInfo.UmLanguagePackSizes.Add("es-es", RequiredDiskSpaceStatistics.EsEsLanguagePack);
			UmLanguagePackConfigurationInfo.UmLanguagePackSizes.Add("es-mx", RequiredDiskSpaceStatistics.EsMxLanguagePack);
			UmLanguagePackConfigurationInfo.UmLanguagePackSizes.Add("fr-fr", RequiredDiskSpaceStatistics.FrFrLanguagePack);
			UmLanguagePackConfigurationInfo.UmLanguagePackSizes.Add("fr-ca", RequiredDiskSpaceStatistics.FrCaLanguagePack);
			UmLanguagePackConfigurationInfo.UmLanguagePackSizes.Add("it-it", RequiredDiskSpaceStatistics.ItItLanguagePack);
			UmLanguagePackConfigurationInfo.UmLanguagePackSizes.Add("ja-jp", RequiredDiskSpaceStatistics.JaJpLanguagePack);
			UmLanguagePackConfigurationInfo.UmLanguagePackSizes.Add("ko-kr", RequiredDiskSpaceStatistics.KoKrLanguagePack);
			UmLanguagePackConfigurationInfo.UmLanguagePackSizes.Add("nl-nl", RequiredDiskSpaceStatistics.NlNlLanguagePack);
			UmLanguagePackConfigurationInfo.UmLanguagePackSizes.Add("pt-br", RequiredDiskSpaceStatistics.PtBrLanguagePack);
			UmLanguagePackConfigurationInfo.UmLanguagePackSizes.Add("sv-se", RequiredDiskSpaceStatistics.SvSeLanguagePack);
			UmLanguagePackConfigurationInfo.UmLanguagePackSizes.Add("zh-cn", RequiredDiskSpaceStatistics.ZhCnLanguagePack);
			UmLanguagePackConfigurationInfo.UmLanguagePackSizes.Add("zh-tw", RequiredDiskSpaceStatistics.ZhTwLanguagePack);
		}

		public override string Name
		{
			get
			{
				return UmLanguagePackConfigurationInfo.GetUmLanguagePackNameForCultureInfo(this.Culture);
			}
		}

		public override LocalizedString DisplayName
		{
			get
			{
				if (this.Culture == null)
				{
					return Strings.UmLanguagePackDisplayName;
				}
				return Strings.UmLanguagePackDisplayNameWithCulture(this.umlang.ToString());
			}
		}

		public override decimal Size
		{
			get
			{
				if (this.Culture == null)
				{
					return RequiredDiskSpaceStatistics.MaximumSizeOfOneLanguagePack;
				}
				return UmLanguagePackConfigurationInfo.GetUmLanguagePackSizeForCultureInfo(this.Culture);
			}
		}

		public CultureInfo Culture
		{
			get
			{
				return this.umlang;
			}
		}

		public UmLanguagePackConfigurationInfo(CultureInfo umlang)
		{
			this.umlang = umlang;
		}

		public UmLanguagePackConfigurationInfo()
		{
			this.umlang = null;
		}

		public static string GetUmLanguagePackNameForCultureInfo(CultureInfo umlang)
		{
			if (umlang == null)
			{
				return "UmLanguagePack";
			}
			return "UmLanguagePack(" + umlang.ToString() + ")";
		}

		private CultureInfo umlang;

		private static Dictionary<string, decimal> UmLanguagePackSizes = new Dictionary<string, decimal>();
	}
}
