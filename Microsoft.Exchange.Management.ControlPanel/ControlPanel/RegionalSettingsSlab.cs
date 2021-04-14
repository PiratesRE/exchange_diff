using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class RegionalSettingsSlab : SlabControl
	{
		public static Dictionary<string, Dictionary<string, string>> LanguageDateSets
		{
			get
			{
				return RegionalSettingsSlab.languageDateSets;
			}
		}

		public static Dictionary<string, Dictionary<string, string>> LanguageTimeSets
		{
			get
			{
				return RegionalSettingsSlab.languageTimeSets;
			}
		}

		static RegionalSettingsSlab()
		{
			foreach (CultureInfo cultureInfo in RegionalSettingsSlab.supportedCultureInfos)
			{
				RegionalSettingsSlab.supportedLanguages.Add(cultureInfo.NativeName, cultureInfo.LCID);
				cultureInfo.DateTimeFormat.Calendar = new GregorianCalendar();
				RegionalSettingsSlab.languageDateSets.Add(cultureInfo.LCID.ToString(), RegionalSettingsSlab.GetDateFormatsInCulture(cultureInfo));
				RegionalSettingsSlab.languageTimeSets.Add(cultureInfo.LCID.ToString(), RegionalSettingsSlab.GetTimeFormatsInCulture(cultureInfo));
			}
			if (RegionalSettingsSlab.supportedLanguages.Contains(RegionalSettingsSlab.cultureInfoForSrCyrlCS.NativeName))
			{
				RegionalSettingsSlab.supportedLanguages.Remove(RegionalSettingsSlab.cultureInfoForSrCyrlCS.NativeName);
				RegionalSettingsSlab.supportedLanguages.Add("српски (ћирилица, Србија и Црна Гора (бивша))", RegionalSettingsSlab.cultureInfoForSrCyrlCS.LCID);
			}
			if (RegionalSettingsSlab.supportedLanguages.Contains(RegionalSettingsSlab.cultureInfoForSrLatnCS.NativeName))
			{
				RegionalSettingsSlab.supportedLanguages.Remove(RegionalSettingsSlab.cultureInfoForSrLatnCS.NativeName);
				RegionalSettingsSlab.supportedLanguages.Add("srpski (latinica, Srbija i Crna Gora (bivša))", RegionalSettingsSlab.cultureInfoForSrLatnCS.LCID);
			}
		}

		private static Dictionary<string, string> GetDateFormatsInCulture(CultureInfo culture)
		{
			string arg = culture.TextInfo.IsRightToLeft ? RtlUtil.DecodedRtlDirectionMark : RtlUtil.DecodedLtrDirectionMark;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (string text in culture.DateTimeFormat.GetAllDateTimePatterns('d'))
			{
				if (!dictionary.ContainsKey(text))
				{
					dictionary.Add(text, string.Format("{0}{1}{0}", arg, RegionalSettingsSlab.sampleDate.ToString(text, culture)));
				}
			}
			return dictionary;
		}

		private static Dictionary<string, string> GetTimeFormatsInCulture(CultureInfo culture)
		{
			string arg = culture.TextInfo.IsRightToLeft ? RtlUtil.DecodedRtlDirectionMark : RtlUtil.DecodedLtrDirectionMark;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (string text in culture.DateTimeFormat.GetAllDateTimePatterns('t'))
			{
				if (!dictionary.ContainsKey(text))
				{
					dictionary.Add(text, string.Format("{0}{1}{0} - {0}{2}{0}", arg, RegionalSettingsSlab.sampleStartTime.ToString(text, culture), RegionalSettingsSlab.sampleEndTime.ToString(text, culture)));
				}
			}
			return dictionary;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			bool isRtl = RtlUtil.IsRtl;
			for (int i = 0; i < RegionalSettingsSlab.supportedLanguages.Count; i++)
			{
				this.ddlLanguage.Items.Add(new ListItem(RtlUtil.ConvertToDecodedBidiString(RegionalSettingsSlab.supportedLanguages.GetKey(i).ToString(), isRtl), RegionalSettingsSlab.supportedLanguages.GetByIndex(i).ToString()));
			}
			foreach (ExTimeZone exTimeZone in ExTimeZoneEnumerator.Instance)
			{
				string text = RtlUtil.ConvertToDecodedBidiString(exTimeZone.LocalizableDisplayName.ToString(CultureInfo.CurrentCulture), isRtl);
				this.ddlTimeZone.Items.Add(new ListItem(text, exTimeZone.Id));
			}
		}

		private const string SrLatnCSNativeNameSuffixedWithFormer = "srpski (latinica, Srbija i Crna Gora (bivša))";

		private const string SrCyrlCSNativeNameSuffixedWithFormer = "српски (ћирилица, Србија и Црна Гора (бивша))";

		protected DropDownList ddlLanguage;

		protected DropDownList ddlTimeZone;

		private static readonly List<CultureInfo> supportedCultureInfos = new List<CultureInfo>(LanguagePackInfo.GetInstalledLanguagePackSpecificCultures(LanguagePackType.Client));

		private static readonly Dictionary<string, Dictionary<string, string>> languageDateSets = new Dictionary<string, Dictionary<string, string>>();

		private static readonly Dictionary<string, Dictionary<string, string>> languageTimeSets = new Dictionary<string, Dictionary<string, string>>();

		private static SortedList supportedLanguages = new SortedList();

		private static DateTime sampleDate = new DateTime(2013, 9, 1);

		private static DateTime sampleStartTime = new DateTime(2013, 9, 1, 1, 1, 0, 0);

		private static DateTime sampleEndTime = new DateTime(2013, 9, 1, 23, 59, 0, 0);

		private static CultureInfo cultureInfoForSrCyrlCS = new CultureInfo("sr-Cyrl-CS");

		private static CultureInfo cultureInfoForSrLatnCS = new CultureInfo("sr-Latn-CS");
	}
}
