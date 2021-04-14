using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public interface ISessionContext
	{
		bool IsRtl { get; }

		Theme Theme { get; }

		bool IsBasicExperience { get; }

		CultureInfo UserCulture { get; set; }

		ulong SegmentationFlags { get; }

		BrowserType BrowserType { get; }

		bool IsExplicitLogon { get; }

		bool IsWebPartRequest { get; }

		bool IsProxy { get; }

		string Canary { get; }

		int LogonAndErrorLanguage { get; }

		DayOfWeek WeekStartDay { get; }

		WorkingHours WorkingHours { get; }

		ExTimeZone TimeZone { get; }

		string DateFormat { get; }

		bool HideMailTipsByDefault { get; }

		bool ShowWeekNumbers { get; }

		CalendarWeekRule FirstWeekOfYear { get; }

		Experience[] Experiences { get; }

		string TimeFormat { get; }

		bool CanActAsOwner { get; }

		int HourIncrement { get; }

		bool IsSmsEnabled { get; }

		bool IsReplyByPhoneEnabled { get; }

		string CalendarFolderOwaIdString { get; }

		string GetWeekdayDateFormat(bool useFullWeekdayFormat);

		bool IsInstantMessageEnabled();

		bool IsPublicRequest(HttpRequest request);

		void RenderCssLink(TextWriter writer, HttpRequest request);

		void RenderThemeFileUrl(TextWriter writer, ThemeFileId themeFileId);

		void RenderThemeFileUrl(TextWriter writer, int themeFileIndex);

		void RenderThemeImage(StringBuilder builder, ThemeFileId themeFileId, string styleClass, params object[] extraAttributes);

		void RenderThemeImage(TextWriter writer, ThemeFileId themeFileId);

		void RenderThemeImage(TextWriter writer, ThemeFileId themeFileId, string styleClass, params object[] extraAttributes);

		void RenderBaseThemeImage(TextWriter writer, ThemeFileId themeFileId);

		void RenderBaseThemeImage(TextWriter writer, ThemeFileId themeFileId, string styleClass, params object[] extraAttributes);

		void RenderThemeImageWithToolTip(TextWriter writer, ThemeFileId themeFileId, string styleClass, params string[] extraAttributes);

		void RenderThemeImageWithToolTip(TextWriter writer, ThemeFileId themeFileId, string styleClass, Strings.IDs tooltipStringId, params string[] extraAttributes);

		void RenderThemeImageStart(TextWriter writer, ThemeFileId themeFileId, string styleClass);

		void RenderThemeImageStart(TextWriter writer, ThemeFileId themeFileId, string styleClass, bool renderBaseTheme);

		void RenderThemeImageEnd(TextWriter writer, ThemeFileId themeFileId);

		string GetThemeFileUrl(ThemeFileId themeFileId);

		void RenderCssFontThemeFileUrl(TextWriter writer);

		bool IsFeatureEnabled(Feature feature);

		bool AreFeaturesEnabled(ulong features);
	}
}
