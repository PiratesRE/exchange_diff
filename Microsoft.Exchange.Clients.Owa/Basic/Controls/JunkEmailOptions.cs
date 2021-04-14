using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	public class JunkEmailOptions : OptionsBase
	{
		public JunkEmailOptions(OwaContext owaContext, TextWriter writer) : base(owaContext, writer)
		{
			this.junkEmailRule = this.userContext.MailboxSession.JunkEmailRule;
			this.isEnabled = this.userContext.IsJunkEmailEnabled;
			this.isContactsTrusted = this.junkEmailRule.IsContactsFolderTrusted;
			this.safeListsOnly = this.junkEmailRule.TrustedListsOnly;
			if (Utilities.IsPostRequest(this.request) && !string.IsNullOrEmpty(base.Command))
			{
				this.SaveChanges();
			}
		}

		public override void Render()
		{
			base.RenderHeaderRow(ThemeFileId.JunkEmailBig, -2053927452);
			this.writer.Write("<tr><td class=\"bd\"><table class=\"fmt\">");
			this.writer.Write("<tr><td>");
			this.RenderLabeledInput("radio", "rdoDsbl", "rdoJnk", "enblJnk();", "0", !this.isEnabled, LocalizedStrings.GetHtmlEncoded(-847566036));
			this.writer.Write("</td></tr>");
			this.writer.Write("<tr><td>");
			this.RenderLabeledInput("radio", "rdoEnbl", "rdoJnk", "enblJnk();", "1", this.isEnabled, LocalizedStrings.GetHtmlEncoded(897595079));
			this.writer.Write("</td></tr>");
			this.writer.Write("</table></td></tr>");
			this.writer.Write("<tr><td class=\"jmt\">");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(-1711390873));
			this.writer.Write("</td></tr>");
			this.writer.Write("<tr><td class=\"jms\">");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(-1007937404), "<br>");
			this.writer.Write("</td></tr>");
			this.writer.Write("<tr><td class=\"jms\">");
			this.RenderEmailListControlGroup("Ssl", this.junkEmailRule.TrustedSenderEmailCollection, this.junkEmailRule.TrustedSenderDomainCollection);
			this.writer.Write("</td></tr>");
			if (this.userContext.IsFeatureEnabled(Feature.Contacts))
			{
				this.writer.Write("<tr><td class=\"jms\">");
				this.RenderCheckbox("chkTrstCnt", this.isContactsTrusted, LocalizedStrings.GetHtmlEncoded(-611691961));
				this.writer.Write("</td></tr>");
			}
			this.writer.Write("<tr><td class=\"dsh\"><img src=\"");
			this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.Clear);
			this.writer.Write("\" alt=\"\" class=\"wh1\"></td></tr>");
			this.writer.Write("<tr><td class=\"jmt\">");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(271542056));
			this.writer.Write("</td></tr>");
			this.writer.Write("<tr><td class=\"jms\">");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(-503651893));
			this.writer.Write("</td></tr>");
			this.writer.Write("<tr><td class=\"jms\">");
			this.RenderEmailListControlGroup("Bsl", this.junkEmailRule.BlockedSenderEmailCollection, this.junkEmailRule.BlockedSenderDomainCollection);
			this.writer.Write("</td></tr>");
			this.writer.Write("<tr><td class=\"dsh\"><img src=\"");
			this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.Clear);
			this.writer.Write("\" alt=\"\" class=\"wh1\"></td></tr>");
			this.writer.Write("<tr><td class=\"jmt\">");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(1813922675));
			this.writer.Write("</td></tr>");
			this.writer.Write("<tr><td class=\"jms\">");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(2043062686));
			this.writer.Write("<br>");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(-1859829305));
			this.writer.Write("</td></tr>");
			this.writer.Write("<tr><td class=\"jms\">");
			this.RenderEmailListControlGroup("Srl", this.junkEmailRule.TrustedRecipientEmailCollection, this.junkEmailRule.TrustedRecipientDomainCollection);
			this.writer.Write("</td></tr>");
			this.writer.Write("<tr><td class=\"dsh\"><img src=\"");
			this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.Clear);
			this.writer.Write("\" alt=\"\" class=\"wh1\"></td></tr>");
			this.writer.Write("<tr><td class=\"bd\">");
			this.RenderCheckbox("chkSfOnly", this.safeListsOnly, LocalizedStrings.GetHtmlEncoded(-1352873434));
			this.writer.Write("</td></tr>");
			this.writer.Write("<input type=\"hidden\" name=\"");
			this.writer.Write("hidlst");
			this.writer.Write("\" value=\"\"><input type=\"hidden\" name=\"");
			this.writer.Write("hidoldeml");
			this.writer.Write("\" value=\"\">");
		}

		public override void RenderScript()
		{
			base.RenderJSVariable("a_iRdJnk", this.userContext.IsJunkEmailEnabled ? "1" : "0");
			if (this.userContext.IsFeatureEnabled(Feature.Contacts))
			{
				base.RenderJSVariable("a_fTrstCnt", this.junkEmailRule.IsContactsFolderTrusted);
			}
			base.RenderJSVariable("a_fSfOly", this.junkEmailRule.TrustedListsOnly);
			base.RenderJSVariable("g_sbOptPg", "enblJnk");
		}

		private void SaveChanges()
		{
			string formParameter = Utilities.GetFormParameter(this.request, "rdoJnk", false);
			this.isEnabled = (formParameter != null && formParameter == "1");
			if (this.isEnabled)
			{
				this.isContactsTrusted = (Utilities.GetFormParameter(this.request, "chkTrstCnt", false) != null);
				this.safeListsOnly = (Utilities.GetFormParameter(this.request, "chkSfOnly", false) != null);
			}
			string command;
			if ((command = base.Command) != null)
			{
				if (command == "a")
				{
					this.Add();
					return;
				}
				if (command == "e")
				{
					this.Edit();
					return;
				}
				if (command == "r")
				{
					this.Remove();
					return;
				}
				if (command == "save")
				{
					JunkEmailUtilities.SaveOptions(this.isEnabled, this.isContactsTrusted, this.safeListsOnly, this.userContext);
					base.SetSavedSuccessfully(true);
					this.junkEmailRule = this.userContext.MailboxSession.JunkEmailRule;
					return;
				}
			}
			throw new OwaInvalidRequestException("Unknown command");
		}

		private void Add()
		{
			string formParameter = Utilities.GetFormParameter(this.request, "hidlst");
			string text = Utilities.GetFormParameter(this.request, "txt" + formParameter).Trim();
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			string message;
			if (JunkEmailUtilities.Add(text, JunkEmailHelper.GetListType(formParameter), this.userContext, true, out message))
			{
				base.SetInfobarMessage(message, InfobarMessageType.Informational);
				this.junkEmailRule = this.userContext.MailboxSession.JunkEmailRule;
				return;
			}
			base.SetInfobarMessage(message, InfobarMessageType.Error);
			this.initialInputs[formParameter] = text;
		}

		private void Edit()
		{
			string formParameter = Utilities.GetFormParameter(this.request, "hidlst");
			string text = Utilities.GetFormParameter(this.request, "txt" + formParameter).Trim();
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			string formParameter2 = Utilities.GetFormParameter(this.request, "hidoldeml");
			string message;
			if (JunkEmailUtilities.Edit(formParameter2, text, JunkEmailHelper.GetListType(formParameter), this.userContext, true, out message))
			{
				base.SetInfobarMessage(message, InfobarMessageType.Informational);
				this.junkEmailRule = this.userContext.MailboxSession.JunkEmailRule;
				return;
			}
			base.SetInfobarMessage(message, InfobarMessageType.Error);
			this.initialInputs[formParameter] = text;
		}

		private void Remove()
		{
			string formParameter = Utilities.GetFormParameter(this.request, "hidlst");
			string formParameter2 = Utilities.GetFormParameter(this.request, "sel" + formParameter);
			JunkEmailUtilities.Remove(formParameter2.Split(new char[]
			{
				','
			}), JunkEmailHelper.GetListType(formParameter), this.userContext);
			this.junkEmailRule = this.userContext.MailboxSession.JunkEmailRule;
		}

		private void RenderEmailListControlGroup(string groupId, JunkEmailCollection emails, JunkEmailCollection domains)
		{
			this.writer.Write("<table class=\"jnkTbl\" cellpadding=0 cellspacing=0><tr>");
			this.writer.Write("<td><input class=\"w100 txt\" type=\"text\" id=\"txt");
			this.writer.Write(groupId);
			this.writer.Write("\" name=\"txt");
			this.writer.Write(groupId);
			if (this.initialInputs.ContainsKey(groupId))
			{
				this.writer.Write("\" value=\"");
				Utilities.HtmlEncode(this.initialInputs[groupId], this.writer);
			}
			this.writer.Write("\" onkeypress=\"onKPIpt('");
			this.writer.Write(groupId);
			this.writer.Write("', event)\" onfocus=\"onFcsIpt('");
			this.writer.Write(groupId);
			this.writer.Write("')\" onblur=\"onBlrIpt('");
			this.writer.Write(groupId);
			this.writer.Write("')\"></td>");
			this.RenderButton(groupId, 292745765, "Add");
			this.writer.Write("</tr><tr>");
			this.writer.Write("<td class=\"sel\">");
			this.writer.Write("<select size=6 class=\"w100 sel\" multiple id=\"sel");
			this.writer.Write(groupId);
			this.writer.Write("\" name=\"sel");
			this.writer.Write(groupId);
			this.writer.Write("\" onchange=\"onChgEmSel('");
			this.writer.Write(groupId);
			this.writer.Write("')\" onfocus=\"onFcsEmSel('");
			this.writer.Write(groupId);
			this.writer.Write("')\" onkeydown=\"onKDEmSel('");
			this.writer.Write(groupId);
			this.writer.Write("', event)\" onkeypress=\"onKPEmSel('");
			this.writer.Write(groupId);
			this.writer.Write("', event)\">");
			emails.Sort();
			domains.Sort();
			for (int i = 0; i < emails.Count; i++)
			{
				this.writer.Write("<option value=\"");
				this.writer.Write(emails[i]);
				this.writer.Write("\">");
				this.writer.Write(emails[i]);
				this.writer.Write("</option>");
			}
			for (int j = 0; j < domains.Count; j++)
			{
				this.writer.Write("<option value=\"");
				this.writer.Write(domains[j]);
				this.writer.Write("\">");
				this.writer.Write(domains[j]);
				this.writer.Write("</option>");
			}
			this.writer.Write("</select></td>");
			this.writer.Write("<td valign=\"bottom\"><table class=\"btn\" cellpadding=0 cellspacing=0>");
			this.writer.Write("<tr>");
			this.RenderButton(groupId, 2119799890, "Edt");
			this.writer.Write("</tr>");
			this.writer.Write("<tr><td class=\"w2\"></td></tr>");
			this.writer.Write("<tr>");
			this.RenderButton(groupId, 1388922078, "Rmv");
			this.writer.Write("</tr>");
			this.writer.Write("</table></td>");
			this.writer.Write("</tr></table>");
		}

		private void RenderButton(string groupId, Strings.IDs label, string type)
		{
			this.writer.Write("<td class=\"btn\" nowrap><table cellpadding=0 cellspacing=0><tr><td><a href=\"#\" id=\"btn");
			this.writer.Write(groupId);
			this.writer.Write(type);
			this.writer.Write("\" class=\"disabled\" onClick=\"return onClk");
			this.writer.Write(type);
			this.writer.Write("Btn('");
			this.writer.Write(groupId);
			this.writer.Write("');\" title=\"");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(label));
			this.writer.Write("\">");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(label));
			this.writer.Write("</a></td></tr></table></td>");
		}

		private void RenderLabeledInput(string type, string id, string name, string onclick, string value, bool isChecked, string label)
		{
			this.writer.Write("<input type=\"");
			this.writer.Write(type);
			if (id != null)
			{
				this.writer.Write("\" id=\"");
				this.writer.Write(id);
			}
			if (name != null)
			{
				this.writer.Write("\" name=\"");
				this.writer.Write(name);
			}
			this.writer.Write("\"");
			if (onclick != null)
			{
				this.writer.Write(" onclick=\"");
				this.writer.Write(onclick);
				this.writer.Write("\"");
			}
			if (value != null)
			{
				this.writer.Write(" value=\"");
				this.writer.Write(value);
				this.writer.Write("\"");
			}
			if (isChecked)
			{
				this.writer.Write(" checked");
			}
			this.writer.Write("><label for=\"");
			this.writer.Write(id);
			this.writer.Write("\">");
			this.writer.Write(label);
			this.writer.Write("</label>");
		}

		private void RenderCheckbox(string name, bool isChecked, string label)
		{
			this.RenderLabeledInput("checkbox", name, name, null, null, isChecked, label);
		}

		private const string JunkEmailEnabledParameter = "rdoJnk";

		private const string JunkEmailEnabledValue = "1";

		private const string JunkEmailEnabledJavaScriptVar = "a_iRdJnk";

		private const string IsContactsTrustedParameter = "chkTrstCnt";

		private const string IsContactsTrustedJavaScriptVar = "a_fTrstCnt";

		private const string SafeListsOnlyParameter = "chkSfOnly";

		private const string SafeListsOnlyJavaScriptVar = "a_fSfOly";

		private const string SafeSendersListParameter = "selSsl";

		private const string BlockedSendersListParameter = "selBsl";

		private const string SafeRecipientsListParameter = "selSrl";

		private const string ListNameParameter = "hidlst";

		private const string OldEmailParameter = "hidoldeml";

		private const string SaveCommand = "save";

		private const string AddCommand = "a";

		private const string EditComand = "e";

		private const string RemoveCommand = "r";

		private JunkEmailRule junkEmailRule;

		private bool isEnabled;

		private bool isContactsTrusted;

		private bool safeListsOnly;

		private Dictionary<string, string> initialInputs = new Dictionary<string, string>();
	}
}
