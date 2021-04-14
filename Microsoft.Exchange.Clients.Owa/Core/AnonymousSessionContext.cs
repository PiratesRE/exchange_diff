using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class AnonymousSessionContext : ISessionContext
	{
		public static string GetEscapedPathFromUri(Uri uri)
		{
			string pathAndQuery = uri.PathAndQuery;
			int length = pathAndQuery.LastIndexOf('/');
			return pathAndQuery.Substring(0, length);
		}

		public AnonymousSessionContext(OwaContext owaContext)
		{
			this.owaContext = owaContext;
			this.PublishingUrl = (PublishingUrl)owaContext.HttpContext.Items["AnonymousUserContextPublishedUrl"];
			HttpCookie httpCookie = owaContext.HttpContext.Request.Cookies["timezone"];
			ExTimeZone exTimeZone;
			if (httpCookie != null && !string.IsNullOrEmpty(httpCookie.Value) && ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(httpCookie.Value, out exTimeZone))
			{
				this.timeZone = exTimeZone;
				this.IsTimeZoneFromCookie = true;
				return;
			}
			this.timeZone = ExTimeZone.CurrentTimeZone;
		}

		public PublishingUrl PublishingUrl { get; private set; }

		public bool IsRtl
		{
			get
			{
				return Culture.IsRtl;
			}
		}

		public Theme Theme
		{
			get
			{
				return ThemeManager.GetDefaultTheme(OwaConfigurationManager.Configuration.DefaultTheme);
			}
		}

		public bool IsBasicExperience
		{
			get
			{
				return string.Compare("Basic", this.experiences[0].Name, StringComparison.OrdinalIgnoreCase) == 0;
			}
		}

		public Experience[] Experiences
		{
			get
			{
				return this.experiences;
			}
			set
			{
				if (this.experiences == null)
				{
					this.experiences = value;
					return;
				}
				throw new InvalidOperationException("Experiences can only be initialized once");
			}
		}

		public CultureInfo UserCulture
		{
			get
			{
				return this.owaContext.Culture;
			}
			set
			{
				this.owaContext.Culture = value;
			}
		}

		public ulong SegmentationFlags
		{
			get
			{
				return OwaConfigurationManager.Configuration.SegmentationFlags;
			}
		}

		public BrowserType BrowserType
		{
			get
			{
				BrowserType browserType = Utilities.GetBrowserType(this.owaContext.HttpContext.Request.UserAgent);
				if (browserType == BrowserType.Other)
				{
					browserType = BrowserType.Safari;
				}
				return browserType;
			}
		}

		public bool IsExplicitLogon
		{
			get
			{
				return false;
			}
		}

		public bool IsWebPartRequest
		{
			get
			{
				return false;
			}
		}

		public bool IsProxy
		{
			get
			{
				return this.owaContext.RequestExecution != RequestExecution.Local;
			}
		}

		public string Canary
		{
			get
			{
				return string.Empty;
			}
		}

		public int LogonAndErrorLanguage
		{
			get
			{
				return OwaConfigurationManager.Configuration.LogonAndErrorLanguage;
			}
		}

		public DayOfWeek WeekStartDay
		{
			get
			{
				return DayOfWeek.Sunday;
			}
		}

		public WorkingHours WorkingHours
		{
			get
			{
				return WorkingHours.CreateFromAvailabilityWorkingHours(this, null);
			}
		}

		public ExTimeZone TimeZone
		{
			get
			{
				return this.timeZone;
			}
			set
			{
				this.timeZone = value;
			}
		}

		public bool IsTimeZoneFromCookie { get; private set; }

		public string DateFormat
		{
			get
			{
				return DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
			}
		}

		public string GetWeekdayDateFormat(bool useFullWeekdayFormat)
		{
			string str = useFullWeekdayFormat ? "dddd" : "ddd";
			return str + " " + this.DateFormat;
		}

		public string TimeFormat
		{
			get
			{
				return DateTimeFormatInfo.CurrentInfo.ShortTimePattern;
			}
		}

		public bool HideMailTipsByDefault
		{
			get
			{
				return true;
			}
		}

		public bool ShowWeekNumbers
		{
			get
			{
				return false;
			}
		}

		public CalendarWeekRule FirstWeekOfYear
		{
			get
			{
				return CalendarWeekRule.FirstDay;
			}
		}

		public bool CanActAsOwner
		{
			get
			{
				return true;
			}
		}

		public int HourIncrement
		{
			get
			{
				return 30;
			}
		}

		public bool IsSmsEnabled
		{
			get
			{
				return false;
			}
		}

		public bool IsReplyByPhoneEnabled
		{
			get
			{
				return false;
			}
		}

		public string CalendarFolderOwaIdString
		{
			get
			{
				return "PublishedCalendar";
			}
		}

		public bool IsInstantMessageEnabled()
		{
			return false;
		}

		public bool IsPublicRequest(HttpRequest request)
		{
			return true;
		}

		public void RenderCssLink(TextWriter writer, HttpRequest request)
		{
			SessionContextUtilities.RenderCssLink(writer, request, this);
		}

		public void RenderThemeFileUrl(TextWriter writer, ThemeFileId themeFileId)
		{
			SessionContextUtilities.RenderThemeFileUrl(writer, themeFileId, this);
		}

		public void RenderThemeFileUrl(TextWriter writer, int themeFileIndex)
		{
			SessionContextUtilities.RenderThemeFileUrl(writer, themeFileIndex, this);
		}

		public void RenderThemeImage(StringBuilder builder, ThemeFileId themeFileId, string styleClass, params object[] extraAttributes)
		{
			SessionContextUtilities.RenderThemeImage(builder, themeFileId, styleClass, this, extraAttributes);
		}

		public void RenderThemeImage(TextWriter writer, ThemeFileId themeFileId)
		{
			SessionContextUtilities.RenderThemeImage(writer, themeFileId, this);
		}

		public void RenderThemeImage(TextWriter writer, ThemeFileId themeFileId, string styleClass, params object[] extraAttributes)
		{
			SessionContextUtilities.RenderThemeImage(writer, themeFileId, styleClass, this, extraAttributes);
		}

		public void RenderBaseThemeImage(TextWriter writer, ThemeFileId themeFileId)
		{
			SessionContextUtilities.RenderBaseThemeImage(writer, themeFileId, this);
		}

		public void RenderBaseThemeImage(TextWriter writer, ThemeFileId themeFileId, string styleClass, params object[] extraAttributes)
		{
			SessionContextUtilities.RenderBaseThemeImage(writer, themeFileId, styleClass, this, extraAttributes);
		}

		public void RenderThemeImageWithToolTip(TextWriter writer, ThemeFileId themeFileId, string styleClass, params string[] extraAttributes)
		{
			SessionContextUtilities.RenderThemeImageWithToolTip(writer, themeFileId, styleClass, this, extraAttributes);
		}

		public void RenderThemeImageWithToolTip(TextWriter writer, ThemeFileId themeFileId, string styleClass, Strings.IDs tooltipStringId, params string[] extraAttributes)
		{
			SessionContextUtilities.RenderThemeImageWithToolTip(writer, themeFileId, styleClass, tooltipStringId, this, extraAttributes);
		}

		public void RenderThemeImageStart(TextWriter writer, ThemeFileId themeFileId, string styleClass)
		{
			SessionContextUtilities.RenderThemeImageStart(writer, themeFileId, styleClass, this);
		}

		public void RenderThemeImageStart(TextWriter writer, ThemeFileId themeFileId, string styleClass, bool renderBaseTheme)
		{
			SessionContextUtilities.RenderThemeImageStart(writer, themeFileId, styleClass, renderBaseTheme, this);
		}

		public void RenderThemeImageEnd(TextWriter writer, ThemeFileId themeFileId)
		{
			SessionContextUtilities.RenderThemeImageEnd(writer, themeFileId);
		}

		public string GetThemeFileUrl(ThemeFileId themeFileId)
		{
			return SessionContextUtilities.GetThemeFileUrl(themeFileId, this);
		}

		public void RenderCssFontThemeFileUrl(TextWriter writer)
		{
			SessionContextUtilities.RenderCssFontThemeFileUrl(writer, this);
		}

		public bool IsFeatureEnabled(Feature feature)
		{
			return this.AreFeaturesEnabled((ulong)((uint)feature));
		}

		public bool AreFeaturesEnabled(ulong features)
		{
			return (features & this.SegmentationFlags) == features;
		}

		public string EscapedPath
		{
			get
			{
				if (this.escapedPath == null)
				{
					this.escapedPath = AnonymousSessionContext.GetEscapedPathFromUri(this.PublishingUrl.Uri);
				}
				return this.escapedPath;
			}
		}

		public const string PublishedCalendarIdentity = "PublishedCalendar";

		public const string TimeZoneCookieName = "timezone";

		private OwaContext owaContext;

		private Experience[] experiences;

		private ExTimeZone timeZone;

		private string escapedPath;
	}
}
