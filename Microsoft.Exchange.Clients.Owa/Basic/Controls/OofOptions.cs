using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InfoWorker.Common.OOF;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	public class OofOptions : OptionsBase
	{
		public OofOptions(OwaContext owaContext, TextWriter writer) : base(owaContext, writer)
		{
			this.CommitAndLoad();
		}

		private void Load()
		{
			this.oofState = this.oofSettings.OofState;
			this.externalAudience = this.oofSettings.ExternalAudience;
			this.oofToInternal = this.oofSettings.InternalReply.Message;
			this.oofToExternal = this.oofSettings.ExternalReply.Message;
			this.oofEnabled = (this.oofState != OofState.Disabled);
			this.isScheduled = (this.oofState == OofState.Scheduled);
			this.isScheduledForExternal = (this.externalAudience != ExternalAudience.None);
			this.externalAllEnabled = (this.externalAudience != ExternalAudience.Known);
			if (this.oofSettings.Duration != null)
			{
				this.startTime = new ExDateTime(this.userContext.TimeZone, this.oofSettings.Duration.StartTime);
				this.endTime = new ExDateTime(this.userContext.TimeZone, this.oofSettings.Duration.EndTime);
				return;
			}
			ExDateTime localTime = DateTimeUtilities.GetLocalTime();
			this.startTime = new ExDateTime(this.userContext.TimeZone, localTime.Year, localTime.Month, localTime.Day, localTime.Hour, 0, 0).AddHours(1.0);
			this.endTime = this.startTime.AddHours(1.0);
		}

		public override void Render()
		{
			this.RenderOofOptions();
		}

		public override void RenderScript()
		{
			base.RenderJSVariable("a_fOof", this.oofEnabled.ToString().ToLowerInvariant());
			base.RenderJSVariable("a_fAll", this.externalAllEnabled.ToString().ToLowerInvariant());
			base.RenderJSVariable("a_fExtSnd", this.isScheduledForExternal.ToString().ToLowerInvariant());
			base.RenderJSVariable("a_fTmd", this.isScheduled.ToString().ToLowerInvariant());
			if (this.isInvalidDuration)
			{
				ExDateTime minValue = ExDateTime.MinValue;
				ExDateTime minValue2 = ExDateTime.MinValue;
				base.RenderJSVariable("a_iSM", minValue.Month.ToString());
				base.RenderJSVariable("a_iSD", minValue.Day.ToString());
				base.RenderJSVariable("a_iSY", minValue.Year.ToString());
				base.RenderJSVariable("a_iST", minValue.Hour.ToString());
				base.RenderJSVariable("a_iEM", minValue2.Month.ToString());
				base.RenderJSVariable("a_iED", minValue2.Day.ToString());
				base.RenderJSVariable("a_iEY", minValue2.Year.ToString());
				base.RenderJSVariable("a_iET", minValue2.Hour.ToString());
			}
			else
			{
				base.RenderJSVariable("a_iSM", this.startTime.Month.ToString());
				base.RenderJSVariable("a_iSD", this.startTime.Day.ToString());
				base.RenderJSVariable("a_iSY", this.startTime.Year.ToString());
				base.RenderJSVariable("a_iST", this.startTime.Hour.ToString());
				base.RenderJSVariable("a_iEM", this.endTime.Month.ToString());
				base.RenderJSVariable("a_iED", this.endTime.Day.ToString());
				base.RenderJSVariable("a_iEY", this.endTime.Year.ToString());
				base.RenderJSVariable("a_iET", this.endTime.Hour.ToString());
			}
			base.RenderJSVariable("a_fOofInt", !string.IsNullOrEmpty(this.oofToInternal));
			base.RenderJSVariable("a_fOofExt", !string.IsNullOrEmpty(this.oofToExternal));
		}

		private void CommitAndLoad()
		{
			this.oofSettings = UserOofSettings.GetUserOofSettings(this.userContext.MailboxSession);
			this.Load();
			bool flag = false;
			if (Utilities.IsPostRequest(this.request) && !string.IsNullOrEmpty(base.Command))
			{
				string formParameter = Utilities.GetFormParameter(this.request, "rdoOof", false);
				if (!string.IsNullOrEmpty(formParameter))
				{
					bool flag2 = formParameter == 1.ToString();
					bool flag3 = Utilities.GetFormParameter(this.request, "chkTmd", false) != null;
					if (!flag2)
					{
						if (this.oofSettings.OofState != OofState.Disabled)
						{
							this.oofSettings.OofState = OofState.Disabled;
							flag = true;
						}
					}
					else if (flag3)
					{
						if (this.oofSettings.OofState != OofState.Scheduled)
						{
							this.oofSettings.OofState = OofState.Scheduled;
							flag = true;
						}
						string formParameter2 = Utilities.GetFormParameter(this.request, "selSM", false);
						string formParameter3 = Utilities.GetFormParameter(this.request, "selSD", false);
						string formParameter4 = Utilities.GetFormParameter(this.request, "selSY", false);
						string formParameter5 = Utilities.GetFormParameter(this.request, "selST", false);
						string formParameter6 = Utilities.GetFormParameter(this.request, "selEM", false);
						string formParameter7 = Utilities.GetFormParameter(this.request, "selED", false);
						string formParameter8 = Utilities.GetFormParameter(this.request, "selEY", false);
						string formParameter9 = Utilities.GetFormParameter(this.request, "selET", false);
						if (string.IsNullOrEmpty(formParameter2) || string.IsNullOrEmpty(formParameter3) || string.IsNullOrEmpty(formParameter4) || string.IsNullOrEmpty(formParameter5) || string.IsNullOrEmpty(formParameter6) || string.IsNullOrEmpty(formParameter7) || string.IsNullOrEmpty(formParameter8) || string.IsNullOrEmpty(formParameter9))
						{
							base.SetInfobarMessage(string.Format(LocalizedStrings.GetNonEncoded(1140546334), this.userContext.UserOptions.DateFormat), InfobarMessageType.Error);
							return;
						}
						int num = int.Parse(formParameter3);
						if (num > ExDateTime.DaysInMonth(int.Parse(formParameter4), int.Parse(formParameter2)))
						{
							num = ExDateTime.DaysInMonth(int.Parse(formParameter4), int.Parse(formParameter2));
						}
						ExDateTime t = new ExDateTime(this.userContext.TimeZone, int.Parse(formParameter4), int.Parse(formParameter2), num);
						num = int.Parse(formParameter7);
						if (num > ExDateTime.DaysInMonth(int.Parse(formParameter8), int.Parse(formParameter6)))
						{
							num = ExDateTime.DaysInMonth(int.Parse(formParameter8), int.Parse(formParameter6));
						}
						ExDateTime t2 = new ExDateTime(this.userContext.TimeZone, int.Parse(formParameter8), int.Parse(formParameter6), num);
						t = t.AddHours((double)int.Parse(formParameter5));
						t2 = t2.AddHours((double)int.Parse(formParameter9));
						if (t > t2)
						{
							base.SetInfobarMessage(LocalizedStrings.GetNonEncoded(107113300), InfobarMessageType.Error);
							this.isInvalidDuration = true;
						}
						if (this.oofSettings.Duration == null)
						{
							this.oofSettings.Duration = new Duration();
						}
						if (this.oofSettings.Duration.StartTime != (DateTime)t.ToUtc())
						{
							this.oofSettings.Duration.StartTime = (DateTime)t.ToUtc();
							flag = true;
						}
						if (this.oofSettings.Duration.EndTime != (DateTime)t2.ToUtc())
						{
							this.oofSettings.Duration.EndTime = (DateTime)t2.ToUtc();
							flag = true;
						}
					}
					else if (this.oofSettings.OofState != OofState.Enabled)
					{
						this.oofSettings.OofState = OofState.Enabled;
						flag = true;
					}
					string formParameter10 = Utilities.GetFormParameter(this.request, "txtInt", false);
					string formParameter11 = Utilities.GetFormParameter(this.request, "chkInt", false);
					if (((formParameter11 == null && string.IsNullOrEmpty(this.oofToInternal)) || !string.IsNullOrEmpty(formParameter11)) && !Utilities.WhiteSpaceOnlyOrNullEmpty(formParameter10))
					{
						this.oofSettings.InternalReply.Message = BodyConversionUtilities.ConvertTextToHtml(formParameter10);
						flag = true;
					}
					string formParameter12 = Utilities.GetFormParameter(this.request, "txtExt", false);
					string formParameter13 = Utilities.GetFormParameter(this.request, "chkExt", false);
					if (((formParameter13 == null && string.IsNullOrEmpty(this.oofToExternal)) || !string.IsNullOrEmpty(formParameter13)) && !Utilities.WhiteSpaceOnlyOrNullEmpty(formParameter12))
					{
						this.oofSettings.ExternalReply.Message = BodyConversionUtilities.ConvertTextToHtml(formParameter12);
						flag = true;
					}
					if (Utilities.GetFormParameter(this.request, "chkExtSnd", false) != null)
					{
						string formParameter14 = Utilities.GetFormParameter(this.request, "rdoAll", false);
						if (!string.IsNullOrEmpty(formParameter14))
						{
							if (formParameter14 == 3.ToString())
							{
								if (this.oofSettings.ExternalAudience != ExternalAudience.All)
								{
									this.oofSettings.ExternalAudience = ExternalAudience.All;
									flag = true;
								}
							}
							else if (this.oofSettings.ExternalAudience != ExternalAudience.Known)
							{
								this.oofSettings.ExternalAudience = ExternalAudience.Known;
								flag = true;
							}
						}
					}
					else if (this.oofSettings.ExternalAudience != ExternalAudience.None)
					{
						this.oofSettings.ExternalAudience = ExternalAudience.None;
						flag = true;
					}
					if (flag)
					{
						try
						{
							this.oofSettings.Save(this.userContext.MailboxSession);
							base.SetSavedSuccessfully(true);
						}
						catch (InvalidScheduledOofDuration)
						{
							base.SetInfobarMessage(LocalizedStrings.GetNonEncoded(-561991348), InfobarMessageType.Error);
							this.isInvalidDuration = true;
						}
						this.Load();
					}
				}
			}
		}

		private void RenderOofOptions()
		{
			base.RenderHeaderRow(ThemeFileId.Oof, 917218743);
			this.RenderOofInternalOptions();
			this.writer.Write("<tr><td class=\"dsh\"><img src=\"");
			this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.Clear);
			this.writer.Write("\" alt=\"\" class=\"wh1\"></td></tr>");
			this.RenderOofExternalOptions();
		}

		private void RenderOofInternalOptions()
		{
			this.writer.Write("<tr><td class=\"bd\">");
			this.writer.Write("<table class=\"fmt\">");
			this.writer.Write("<tr><td>");
			this.writer.Write("<input type=\"radio\" name=\"{0}\"{2} id=\"{5}\" onclick=\"return onClkOofRdo({1});\" value=\"{1}\"{3}><label for=\"{5}\">{4}</label>", new object[]
			{
				"rdoOof",
				0.ToString(),
				(!this.oofEnabled) ? " checked" : string.Empty,
				string.Empty,
				LocalizedStrings.GetHtmlEncoded(-640476466),
				"rdoOof1"
			});
			this.writer.Write("</td></tr>");
			this.writer.Write("<tr><td>");
			this.writer.Write("<input type=\"radio\" name=\"{0}\"{2} id=\"{5}\" onclick=\"return onClkOofRdo({1});\" value=\"{1}\"{3}><label for=\"{5}\">{4}</label>", new object[]
			{
				"rdoOof",
				1.ToString(),
				this.oofEnabled ? " checked" : string.Empty,
				string.Empty,
				LocalizedStrings.GetHtmlEncoded(1328100008),
				"rdoOof2"
			});
			this.writer.Write("</td></tr>");
			this.writer.Write("<tr><td>");
			this.RenderTimeDuration();
			this.writer.Write("</td></tr>");
			this.writer.Write("<tr><td>");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(1004950132));
			this.writer.Write("</td></tr>");
			this.writer.Write("<tr><td>");
			this.RenderOofInternalMessage();
			this.writer.Write("</td></tr>");
			this.writer.Write("</table>");
			this.writer.Write("</td></tr>");
		}

		private void RenderOofExternalOptions()
		{
			this.writer.Write("<tr><td class=\"bd\">");
			this.writer.Write("<table class=\"fmt\">");
			this.writer.Write("<tr><td>");
			string text = string.Format(LocalizedStrings.GetHtmlEncoded(980899065), "<b>" + LocalizedStrings.GetHtmlEncoded(119471705) + "</b>");
			this.writer.Write("<input type=\"checkbox\" name=\"{0}\"{2} id=\"{0}\" onclick=\"return onClkOofChkBx({1});\" value=\"\"{3}><label for=\"{0}\">{4}</label>", new object[]
			{
				"chkExtSnd",
				1.ToString(),
				this.isScheduledForExternal ? " checked" : string.Empty,
				this.oofEnabled ? string.Empty : " disabled",
				text
			});
			this.writer.Write("</td></tr>");
			this.writer.Write("<tr><td>");
			this.RenderOofExternalSenderSelection();
			this.writer.Write("</td></tr>");
			this.writer.Write("<tr><td>");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(-1973556468));
			this.writer.Write("</td></tr>");
			this.writer.Write("<tr><td>");
			this.RenderOofExternalMessage();
			this.writer.Write("</td></tr>");
			this.writer.Write("</table>");
			this.writer.Write("</td></tr>");
		}

		private void RenderOofExternalSenderSelection()
		{
			this.writer.Write("<table class=\"tm\">");
			this.writer.Write("<tr><td>");
			this.writer.Write("<input type=\"radio\" name=\"{0}\"{2} id=\"{5}\" onclick=\"return onClkOofRdo({1});\" value=\"{1}\"{3}><label for=\"{5}\">{4}</label>", new object[]
			{
				"rdoAll",
				2.ToString(),
				(!this.externalAllEnabled) ? " checked" : string.Empty,
				(this.oofEnabled && this.isScheduledForExternal) ? string.Empty : " disabled",
				LocalizedStrings.GetHtmlEncoded(-1839461610),
				"rdoAll1"
			});
			this.writer.Write("</td></tr>");
			this.writer.Write("<tr><td>");
			this.writer.Write("<input type=\"radio\" name=\"{0}\"{2} id=\"{5}\" onclick=\"return onClkOofRdo({1});\" value=\"{1}\"{3}><label for=\"{5}\">{4}</label>", new object[]
			{
				"rdoAll",
				3.ToString(),
				this.externalAllEnabled ? " checked" : string.Empty,
				(this.oofEnabled && this.isScheduledForExternal) ? string.Empty : " disabled",
				LocalizedStrings.GetHtmlEncoded(-689266169),
				"rdoAll2"
			});
			this.writer.Write("</td></tr>");
			this.writer.Write("</table>");
		}

		private void RenderOofExternalMessage()
		{
			if (!string.IsNullOrEmpty(this.oofToExternal))
			{
				this.writer.Write("<table class=\"oof\" cellpadding=0 cellspacing=0>");
				this.writer.Write("<tr><td class=\"dsl\" rowspan=3><img src=\"");
				this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.Clear);
				this.writer.Write("\" alt=\"\" class=\"wh1\"></td><td class=\"w100\"></td><td class=\"dsr\" rowspan=3><img src=\"");
				this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.Clear);
				this.writer.Write("\" alt=\"\" class=\"wh1\"></td></tr>");
				this.writer.Write("<tr><td class=\"ds\">");
				this.writer.Write(Utilities.SanitizeHtml(this.oofToExternal));
				this.writer.Write("</td></tr>");
				this.writer.Write("<tr><td></td></tr></table>");
				this.writer.Write("<table class=\"oofnb\"><tr><td class=\"df\"><input type=\"checkbox\" name=\"");
				this.writer.Write("chkExt");
				this.writer.Write("\" id=\"");
				this.writer.Write("chkExt");
				this.writer.Write("\" onclick=\"return onClkChkBx(this);\" value=\"1\"");
				this.writer.Write((this.oofEnabled && this.isScheduledForExternal) ? ">" : " disabled>");
				this.writer.Write("<label for=\"");
				this.writer.Write("chkExt");
				this.writer.Write("\">");
				this.writer.Write(LocalizedStrings.GetHtmlEncoded(-1886172398));
				this.writer.Write("</label></td></tr>");
				this.writer.Write("<tr><td class=\"w100\">");
				this.writer.Write("<textarea name=\"");
				this.writer.Write("txtExt");
				this.writer.Write("\" class=\"w100\" title=\"");
				this.writer.Write(LocalizedStrings.GetHtmlEncoded(1640929204));
				this.writer.Write("\" rows=12 cols=61 onfocus=\"onFcsTxt('");
				this.writer.Write("chkExt");
				this.writer.Write("');\"");
				this.writer.Write((this.oofEnabled && this.isScheduledForExternal) ? ">" : " disabled>");
				this.writer.Write("</textarea>");
				this.writer.Write("</td></tr>");
				this.writer.Write("</table>");
				return;
			}
			this.writer.Write("<table class=\"oofnb\"><tr><td class=\"w100\"><textarea name=\"");
			this.writer.Write("txtExt");
			this.writer.Write("\" class=\"w100\" title=\"");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(1640929204));
			this.writer.Write("\" rows=12 cols=61");
			this.writer.Write((this.oofEnabled && this.isScheduledForExternal) ? ">" : " disabled>");
			this.writer.Write("</textarea>");
			this.writer.Write("</td></tr>");
			this.writer.Write("</table>");
		}

		private void RenderOofInternalMessage()
		{
			if (!string.IsNullOrEmpty(this.oofToInternal))
			{
				this.writer.Write("<table class=\"oof\" cellpadding=0 cellspacing=0>");
				this.writer.Write("<tr><td class=\"dsl\" rowspan=3><img src=\"");
				this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.Clear);
				this.writer.Write("\" alt=\"\" class=\"wh1\"></td><td class=\"w100\"></td><td class=\"dsr\" rowspan=3><img src=\"");
				this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.Clear);
				this.writer.Write("\" alt=\"\" class=\"wh1\"></td></tr>");
				this.writer.Write("<tr><td class=\"ds\">");
				this.writer.Write(Utilities.SanitizeHtml(this.oofToInternal));
				this.writer.Write("</td></tr>");
				this.writer.Write("<tr><td></td></tr></table>");
				this.writer.Write("<table class=\"oofnb\"><tr><td class=\"df\"><input type=\"checkbox\" name=\"");
				this.writer.Write("chkInt");
				this.writer.Write("\" id=\"");
				this.writer.Write("chkInt");
				this.writer.Write("\" onclick=\"return onClkChkBx(this);\" value=\"1\"");
				this.writer.Write(this.oofEnabled ? ">" : " disabled>");
				this.writer.Write("<label for=\"");
				this.writer.Write("chkInt");
				this.writer.Write("\">");
				this.writer.Write(LocalizedStrings.GetHtmlEncoded(626160585));
				this.writer.Write("<label></td></tr>");
				this.writer.Write("<tr><td class=\"w100\">");
				this.writer.Write("<textarea name=\"");
				this.writer.Write("txtInt");
				this.writer.Write("\" class=\"w100\" title=\"");
				this.writer.Write(LocalizedStrings.GetHtmlEncoded(-1268432338));
				this.writer.Write("\" rows=12 cols=61 onfocus=\"onFcsTxt('");
				this.writer.Write("chkInt");
				this.writer.Write("');\"");
				this.writer.Write(this.oofEnabled ? ">" : " disabled>");
				this.writer.Write("</textarea>");
				this.writer.Write("</td></tr>");
				this.writer.Write("</table>");
				return;
			}
			this.writer.Write("<table class=\"oofnb\"><tr><td class=\"w100\"><textarea name=\"");
			this.writer.Write("txtInt");
			this.writer.Write("\" class=\"w100\" title=\"");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(-1268432338));
			this.writer.Write("\" rows=12 cols=61");
			this.writer.Write(this.oofEnabled ? ">" : " disabled>");
			this.writer.Write("</textarea>");
			this.writer.Write("</td></tr>");
			this.writer.Write("</table>");
		}

		private void RenderTimeDuration()
		{
			this.writer.Write("<table class=\"tm\">");
			this.writer.Write("<tr><td colspan=5>");
			this.writer.Write("<input type=\"checkbox\" name=\"{0}\"{2} id=\"{0}\" onclick=\"return onClkOofChkBx({1});\" value=\"\"{3}><label for=\"{0}\">{4}</label>", new object[]
			{
				"chkTmd",
				0.ToString(),
				this.isScheduled ? " checked" : string.Empty,
				this.oofEnabled ? string.Empty : " disabled",
				LocalizedStrings.GetHtmlEncoded(192681489)
			});
			this.writer.Write("</td></tr>");
			this.writer.Write("<tr><td>");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(-694533162));
			this.writer.Write("</td>");
			this.RenderTime(true);
			this.writer.Write("</tr><tr><td>");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(-521769177));
			this.writer.Write("</td>");
			this.RenderTime(false);
			this.writer.Write("</tr></table>");
		}

		private void RenderTime(bool isStartTime)
		{
			ExDateTime localTime = DateTimeUtilities.GetLocalTime();
			string[] monthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
			this.writer.Write("<td>");
			this.writer.Write("<select name=\"{0}\" onchange=\"{1}\"{2}>", isStartTime ? "selSM" : "selEM", "onChgSel(this);", (this.oofEnabled && this.isScheduled) ? string.Empty : " disabled");
			int num = isStartTime ? this.startTime.Month : this.endTime.Month;
			for (int i = 1; i < monthNames.Length; i++)
			{
				this.writer.Write("<option value=\"{1}\"{0}>{2}</option>", (i == num) ? " selected" : string.Empty, i, Utilities.HtmlEncode(monthNames[i - 1]));
			}
			this.writer.Write("</select></td>");
			this.writer.Write("<td>");
			this.writer.Write("<select name=\"{0}\" onchange=\"{1}\"{2}>", isStartTime ? "selSD" : "selED", "onChgSel(this);", (this.oofEnabled && this.isScheduled) ? string.Empty : " disabled");
			num = (isStartTime ? this.startTime.Day : this.endTime.Day);
			for (int j = 1; j <= 31; j++)
			{
				this.writer.Write("<option value=\"{1}\"{0}>{2}</option>", (j == num) ? " selected" : string.Empty, j, j);
			}
			this.writer.Write("</select></td>");
			this.writer.Write("<td>");
			this.writer.Write("<select name=\"{0}\" onchange=\"{1}\"{2}>", isStartTime ? "selSY" : "selEY", "onChgSel(this);", (this.oofEnabled && this.isScheduled) ? string.Empty : " disabled");
			num = (isStartTime ? this.startTime.Year : this.endTime.Year);
			int num2 = localTime.Year - 2;
			int num3 = localTime.Year + 4;
			for (int k = num2; k <= num3; k++)
			{
				this.writer.Write("<option value=\"{1}\"{0}>{2}</option>", (k == num) ? " selected" : string.Empty, k, k);
			}
			this.writer.Write("</select></td>");
			this.writer.Write("<td>");
			num = (isStartTime ? this.startTime.Hour : this.endTime.Hour);
			this.writer.Write("<select name=\"");
			this.writer.Write(isStartTime ? "selST" : "selET");
			this.writer.Write("\" onchange=\"onChgSel(this);\"");
			this.writer.Write((this.oofEnabled && this.isScheduled) ? ">" : " disabled>");
			for (int l = 0; l < 24; l++)
			{
				DateTime dateTime = new DateTime(1, 1, 1, l, 0, 0);
				this.writer.Write("<option value=\"{1}\"{0}>{2}</option>", (l == num) ? " selected" : string.Empty, l, dateTime.ToString(this.userContext.UserOptions.TimeFormat, CultureInfo.CurrentUICulture.DateTimeFormat));
			}
			this.writer.Write("</select></td>");
		}

		private const string Option = "<option value=\"{1}\"{0}>{2}</option>";

		private const string FormCheckBox = "<input type=\"checkbox\" name=\"{0}\"{2} id=\"{0}\" onclick=\"return onClkOofChkBx({1});\" value=\"\"{3}><label for=\"{0}\">{4}</label>";

		private const string FormRadioButton = "<input type=\"radio\" name=\"{0}\"{2} id=\"{5}\" onclick=\"return onClkOofRdo({1});\" value=\"{1}\"{3}><label for=\"{5}\">{4}</label>";

		private const string FormSelect = "<select name=\"{0}\" onchange=\"{1}\"{2}>";

		private const string FormOofRadioButton = "rdoOof";

		private const string FormOofExternalAllRadioButton = "rdoAll";

		private const string FormTimeDurationCheckbox = "chkTmd";

		private const string FormStartMonthSelect = "selSM";

		private const string FormStartDaySelect = "selSD";

		private const string FormStartYearSelect = "selSY";

		private const string FormStartTimeSelect = "selST";

		private const string FormEndMonthSelect = "selEM";

		private const string FormEndDaySelect = "selED";

		private const string FormEndYearSelect = "selEY";

		private const string FormEndTimeSelect = "selET";

		private const string FormReplaceInternal = "chkInt";

		private const string FormReplaceExternal = "chkExt";

		private const string FormSendExternal = "chkExtSnd";

		private const string FormReplaceInternalText = "txtInt";

		private const string FormReplaceExternalText = "txtExt";

		private const string FormJavaScriptOofRadioButton = "a_fOof";

		private const string FormJavaScriptOofExternalAllRadioButton = "a_fAll";

		private const string FormJavaScriptTimeDurationCheckbox = "a_fTmd";

		private const string FormJavaScriptStartMonthSelect = "a_iSM";

		private const string FormJavaScriptStartDaySelect = "a_iSD";

		private const string FormJavaScriptStartYearSelect = "a_iSY";

		private const string FormJavaScriptStartTimeSelect = "a_iST";

		private const string FormJavaScriptEndMonthSelect = "a_iEM";

		private const string FormJavaScriptEndDaySelect = "a_iED";

		private const string FormJavaScriptEndYearSelect = "a_iEY";

		private const string FormJavaScriptEndTimeSelect = "a_iET";

		private const string FormJavaScriptSendExternal = "a_fExtSnd";

		private const string FormJavaScriptOofInternalPresent = "a_fOofInt";

		private const string FormJavaScriptOofExternalPresent = "a_fOofExt";

		private UserOofSettings oofSettings;

		private OofState oofState;

		private ExternalAudience externalAudience;

		private ExDateTime startTime = ExDateTime.MinValue;

		private ExDateTime endTime = ExDateTime.MinValue;

		private string oofToInternal = string.Empty;

		private string oofToExternal = string.Empty;

		private bool oofEnabled;

		private bool isScheduled;

		private bool isScheduledForExternal;

		private bool externalAllEnabled = true;

		private bool isInvalidDuration;

		private enum OofStatus
		{
			DoNotSend,
			Send,
			SendToExternalContactsInListOnly,
			SendToExternalAll,
			Scheduled = 0,
			ScheduledForExternal
		}
	}
}
