using System;
using System.Globalization;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	public class RegionalOptions : OptionsBase
	{
		private DateTimeFormatInfo DateTimeFormat
		{
			get
			{
				return this.cultureInfo.DateTimeFormat;
			}
		}

		public RegionalOptions(OwaContext owaContext, TextWriter writer) : base(owaContext, writer)
		{
			this.CommitAndLoad();
		}

		private void Load()
		{
			this.cultureInfo = Thread.CurrentThread.CurrentUICulture;
			this.currentCultureLCID = this.cultureInfo.LCID.ToString();
			if (Utilities.IsPostRequest(this.request) && base.Command != null && base.Command.Equals("chglng", StringComparison.Ordinal))
			{
				string formParameter = Utilities.GetFormParameter(this.request, "selLng", false);
				if (!string.IsNullOrEmpty(formParameter))
				{
					if (Culture.IsSupportedCulture(int.Parse(formParameter)))
					{
						this.cultureInfo = Culture.GetCultureInfoInstance(int.Parse(formParameter));
						return;
					}
					ExTraceGlobals.UserOptionsDataTracer.TraceDebug<string>((long)this.GetHashCode(), "Attempted to update date/time styles with unsupported culture (LCID: {0})", formParameter);
				}
			}
		}

		private void CommitAndLoad()
		{
			this.Load();
			bool flag = false;
			if (Utilities.IsPostRequest(this.request) && base.Command != null && base.Command.Equals("save", StringComparison.Ordinal))
			{
				flag = true;
				string formParameter = Utilities.GetFormParameter(this.request, "selLng", false);
				if (!string.IsNullOrEmpty(formParameter))
				{
					if (!Culture.IsSupportedCulture(int.Parse(formParameter)))
					{
						ExTraceGlobals.CoreTracer.TraceDebug<string>((long)this.GetHashCode(), "Attempted to set user culture to unsupported culture (LCID: {0})", formParameter);
						throw new OwaEventHandlerException("Regional options could not be saved due to an invalid language parameter.", LocalizedStrings.GetNonEncoded(1073923836));
					}
					this.cultureInfo = Culture.GetCultureInfoInstance(int.Parse(formParameter));
					this.currentCultureLCID = this.cultureInfo.LCID.ToString();
					Culture.UpdateUserCulture(this.userContext, this.cultureInfo);
				}
				string formParameter2 = Utilities.GetFormParameter(this.request, "selDtStl", false);
				if (!string.IsNullOrEmpty(formParameter2))
				{
					this.userContext.UserOptions.DateFormat = formParameter2;
					this.userContext.MailboxSession.PreferedCulture.DateTimeFormat.ShortDatePattern = formParameter2;
				}
				string formParameter3 = Utilities.GetFormParameter(this.request, "selTmStl", false);
				if (!string.IsNullOrEmpty(formParameter3))
				{
					this.userContext.UserOptions.TimeFormat = formParameter3;
					this.userContext.MailboxSession.PreferedCulture.DateTimeFormat.ShortTimePattern = formParameter3;
				}
				string formParameter4 = Utilities.GetFormParameter(this.request, "selTmZn", false);
				if (!string.IsNullOrEmpty(formParameter4))
				{
					this.userContext.UserOptions.TimeZone = formParameter4;
				}
				DateTimeUtilities.SetSessionTimeZone(this.userContext);
			}
			if (flag)
			{
				try
				{
					this.userContext.UserOptions.CommitChanges();
					base.SetSavedSuccessfully(true);
					this.Load();
				}
				catch (StoragePermanentException)
				{
					base.SetSavedSuccessfully(false);
				}
				catch (StorageTransientException)
				{
					base.SetSavedSuccessfully(false);
				}
			}
		}

		public override void Render()
		{
			this.RenderLanguageOptions();
			this.RenderDateTimeFormats();
		}

		public override void RenderScript()
		{
			base.RenderJSVariable("a_iLng", this.currentCultureLCID);
			base.RenderJSVariableWithQuotes("a_sDtStl", this.userContext.UserOptions.DateFormat);
			base.RenderJSVariableWithQuotes("a_sTmStl", this.userContext.UserOptions.TimeFormat);
			base.RenderJSVariableWithQuotes("a_iTmZn", this.userContext.UserOptions.TimeZone);
		}

		private void RenderLanguageOptions()
		{
			base.RenderHeaderRow(ThemeFileId.Globe, 468777496);
			this.writer.Write("<tr><td class=\"bd lrgLn\">");
			this.writer.Write("<div>");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(1882021095));
			this.writer.Write("</div>");
			this.writer.Write("<div>");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(-833638460));
			this.RenderLanguageSelection();
			this.writer.Write("</div>");
			this.writer.Write("<div>");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(647522285));
			this.writer.Write("</div>");
			this.writer.Write("</td></tr>");
		}

		protected void RenderLanguageSelection()
		{
			this.writer.Write("<select name=\"{0}\"{1}{2}>", "selLng", " class=\"rs padLR multiLanguangeChar\"", " onchange=\"return onChgLng(this);\"");
			CultureInfo[] supportedCultures = Culture.GetSupportedCultures(true);
			for (int i = 0; i < supportedCultures.Length; i++)
			{
				this.writer.Write("<option value=\"");
				this.writer.Write(supportedCultures[i].LCID);
				this.writer.Write('"');
				if (supportedCultures[i].LCID == this.cultureInfo.LCID)
				{
					this.writer.Write(" selected");
				}
				this.writer.Write('>');
				Utilities.HtmlEncode(supportedCultures[i].NativeName, this.writer);
				this.writer.Write("</option>");
			}
			this.writer.Write("</select>");
		}

		private void RenderDateTimeFormats()
		{
			base.RenderHeaderRow(ThemeFileId.DateBook, -1860736294);
			this.writer.Write("<tr><td class=\"bd\">");
			this.writer.Write("<table class=\"fmt\">");
			this.writer.Write("<tr>");
			this.writer.Write("<td nowrap>");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(-1176481198));
			this.writer.Write("</td>");
			this.writer.Write("<td>");
			this.RenderDateStyleSelection();
			this.writer.Write("</td>");
			this.writer.Write("<td class=\"w100\">");
			this.RenderCalendarOptions();
			this.writer.Write("</td>");
			this.writer.Write("</tr>");
			this.writer.Write("<tr>");
			this.writer.Write("<td nowrap>");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(1486945283));
			this.writer.Write("</td>");
			this.writer.Write("<td colspan=2 class=\"w100\">");
			this.RenderTimeStyleSelection();
			this.writer.Write("</td>");
			this.writer.Write("</tr>");
			this.writer.Write("<tr>");
			this.writer.Write("<td nowrap>");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(-1147623709));
			this.writer.Write("</td>");
			this.writer.Write("<td colspan=2 class=\"w100\">");
			this.RenderTimeZoneSelection();
			this.writer.Write("</td>");
			this.writer.Write("</tr>");
			this.writer.Write("</table>");
			this.writer.Write("</td></tr>");
			if (this.userContext.WorkingHours.IsTimeZoneDifferent && this.userContext.IsFeatureEnabled(Feature.Calendar))
			{
				this.writer.Write("<tr><td id=\"tdTZNt\">");
				this.writer.Write(LocalizedStrings.GetHtmlEncoded(566587615));
				this.writer.Write("</td></tr>");
			}
		}

		protected void RenderCalendarOptions()
		{
		}

		protected void RenderDateStyleSelection()
		{
			string[] allDateTimePatterns = this.DateTimeFormat.GetAllDateTimePatterns('d');
			this.writer.Write("<select name=\"{0}\"{1}{2}>", "selDtStl", " class=\"rs padLR\"", " onchange=\"return onChgSel(this);\"");
			for (int i = 0; i < allDateTimePatterns.Length; i++)
			{
				this.writer.Write("<option value=\"");
				Utilities.HtmlEncode(allDateTimePatterns[i], this.writer);
				this.writer.Write('"');
				if (this.userContext.UserOptions.DateFormat == allDateTimePatterns[i])
				{
					this.writer.Write(" selected");
				}
				this.writer.Write('>');
				Utilities.HtmlEncode(DateTimeUtilities.ExampleDate.ToString(allDateTimePatterns[i], this.DateTimeFormat), this.writer);
				this.writer.Write("</option>");
			}
			this.writer.Write("</select>");
		}

		protected void RenderTimeStyleSelection()
		{
			string[] allDateTimePatterns = this.DateTimeFormat.GetAllDateTimePatterns('t');
			ExDateTime exDateTime = new ExDateTime(this.userContext.TimeZone, 2008, 1, 21, 1, 1, 0);
			ExDateTime exDateTime2 = new ExDateTime(this.userContext.TimeZone, 2008, 1, 21, 23, 59, 0);
			this.writer.Write("<select name=\"{0}\"{1}{2}>", "selTmStl", " class=\"rs padLR\"", " onchange=\"return onChgSel(this);\"");
			for (int i = 0; i < allDateTimePatterns.Length; i++)
			{
				this.writer.Write("<option value=\"");
				Utilities.HtmlEncode(allDateTimePatterns[i], this.writer);
				this.writer.Write('"');
				if (allDateTimePatterns[i] == this.userContext.UserOptions.TimeFormat)
				{
					this.writer.Write(" selected");
				}
				this.writer.Write('>');
				this.writer.Write(LocalizedStrings.GetHtmlEncoded(466973109), Utilities.HtmlEncode(exDateTime.ToString(allDateTimePatterns[i], this.DateTimeFormat)), Utilities.HtmlEncode(exDateTime2.ToString(allDateTimePatterns[i], this.DateTimeFormat)));
				this.writer.Write("</option>");
			}
			this.writer.Write("</select>");
		}

		protected void RenderTimeZoneSelection()
		{
			this.writer.Write("<select name=\"{0}\"{1}{2}>", "selTmZn", " class=\"padLR\"", " onchange=\"return onChgSel(this);\"");
			foreach (ExTimeZone exTimeZone in ExTimeZoneEnumerator.Instance)
			{
				this.writer.Write("<option value=\"");
				Utilities.HtmlEncode(exTimeZone.Id, this.writer);
				this.writer.Write('"');
				if (string.Equals(exTimeZone.Id, this.userContext.UserOptions.TimeZone, StringComparison.OrdinalIgnoreCase))
				{
					this.writer.Write(" selected");
				}
				this.writer.Write('>');
				Utilities.HtmlEncode(exTimeZone.LocalizableDisplayName.ToString(Thread.CurrentThread.CurrentUICulture), this.writer);
				this.writer.Write("</option>");
			}
			this.writer.Write("</select>");
		}

		private const string SelectText = "<select name=\"{0}\"{1}{2}>";

		private const string FormLanguageSelection = "selLng";

		private const string FormDateSelection = "selDtStl";

		private const string FormTimeSelection = "selTmStl";

		private const string FormTimeZoneSelection = "selTmZn";

		private const string FormJavaScriptLanguageSelection = "a_iLng";

		private const string FormJavaScriptDateSelection = "a_sDtStl";

		private const string FormJavaScriptTimeSelection = "a_sTmStl";

		private const string FormJavaScriptTimeZoneSelection = "a_iTmZn";

		private const string ChangeLanguageCommand = "chglng";

		private const string SaveCommand = "save";

		private CultureInfo cultureInfo;

		private string currentCultureLCID = string.Empty;
	}
}
