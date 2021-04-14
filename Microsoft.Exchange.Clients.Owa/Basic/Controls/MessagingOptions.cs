using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	public class MessagingOptions : OptionsBase
	{
		public MessagingOptions(OwaContext owaContext, TextWriter writer) : base(owaContext, writer)
		{
			this.CommitAndLoad();
		}

		private void Load()
		{
			this.displayItemsPerPage = this.userContext.UserOptions.BasicViewRowCount;
			this.nextSelection = this.userContext.UserOptions.NextSelection;
			if (this.userContext.IsFeatureEnabled(Feature.Signature))
			{
				this.autoAddSignature = this.userContext.UserOptions.AutoAddSignature;
				this.signatureText = this.userContext.UserOptions.SignatureText;
				if (!Utilities.WhiteSpaceOnlyOrNullEmpty(this.signatureText))
				{
					this.signatureHtml = this.userContext.UserOptions.SignatureHtml;
				}
			}
			this.readReceiptResponse = this.userContext.UserOptions.ReadReceipt;
			this.checkNameInContactsFirst = this.userContext.UserOptions.CheckNameInContactsFirst;
			this.addRecentRecipientsToMrr = this.userContext.UserOptions.AddRecipientsToAutoCompleteCache;
			this.emptyDeletedItemsOnLogoff = this.userContext.UserOptions.EmptyDeletedItemsOnLogoff;
		}

		public override void Render()
		{
			this.RenderMessagingOptions();
			if (this.userContext.IsFeatureEnabled(Feature.Signature))
			{
				this.RenderEmailSignature();
			}
			this.RenderMessageTracking();
			this.RenderEmailNameResolution();
			this.RenderDeletedItems();
		}

		public override void RenderScript()
		{
			base.RenderJSVariable("a_iRwCnt", this.userContext.UserOptions.BasicViewRowCount.ToString());
			base.RenderJSVariable("a_iNxt", ((int)this.userContext.UserOptions.NextSelection).ToString());
			if (this.userContext.IsFeatureEnabled(Feature.Signature))
			{
				base.RenderJSVariable("a_fAddSg", this.userContext.UserOptions.AutoAddSignature);
				base.RenderJSVariable("a_fSgPr", !string.IsNullOrEmpty(this.signatureHtml));
			}
			base.RenderJSVariable("a_fEDI", this.userContext.UserOptions.EmptyDeletedItemsOnLogoff);
			base.RenderJSVariable("a_iRdRcpt", ((int)this.userContext.UserOptions.ReadReceipt).ToString());
			base.RenderJSVariable("a_fAddMRR", this.userContext.UserOptions.AddRecipientsToAutoCompleteCache);
			if (this.userContext.IsFeatureEnabled(Feature.Contacts))
			{
				base.RenderJSVariable("a_iAnrFst", this.userContext.UserOptions.CheckNameInContactsFirst ? "1" : "0");
			}
			base.RenderJSVariableWithQuotes("a_sPrClrMRR", LocalizedStrings.GetNonEncoded(136482375));
		}

		private void CommitAndLoad()
		{
			this.Load();
			if (Utilities.IsPostRequest(this.request) && !string.IsNullOrEmpty(base.Command))
			{
				if (base.Command.Equals("ClrMRR", StringComparison.Ordinal))
				{
					AutoCompleteCache autoCompleteCache = AutoCompleteCache.TryGetCache(OwaContext.Current.UserContext);
					if (autoCompleteCache != null)
					{
						autoCompleteCache.ClearCache();
						autoCompleteCache.Commit(false);
					}
					this.isClearMrrRequest = true;
				}
				string formParameter = Utilities.GetFormParameter(this.request, "selRwCnt");
				if (string.IsNullOrEmpty(formParameter) || !int.TryParse(formParameter, out this.displayItemsPerPage))
				{
					throw new OwaInvalidRequestException("Row count must be a valid number");
				}
				string formParameter2 = Utilities.GetFormParameter(this.request, "selNxt");
				int num;
				if (string.IsNullOrEmpty(formParameter2) || !int.TryParse(formParameter2, out num) || num < 0 || num > 2)
				{
					throw new OwaInvalidRequestException("Next selection must be a valid number");
				}
				this.nextSelection = (NextSelectionDirection)num;
				if (this.userContext.IsFeatureEnabled(Feature.Signature))
				{
					this.autoAddSignature = (Utilities.GetFormParameter(this.request, "chkAddSg", false) != null);
					this.signatureText = Utilities.GetFormParameter(this.request, "txtSg", false);
				}
				this.emptyDeletedItemsOnLogoff = (Utilities.GetFormParameter(this.request, "chkEmDel", false) != null);
				string formParameter3 = Utilities.GetFormParameter(this.request, "rdRcpt", false);
				if (!string.IsNullOrEmpty(formParameter3))
				{
					this.readReceiptResponse = (ReadReceiptResponse)int.Parse(formParameter3);
				}
				this.addRecentRecipientsToMrr = (Utilities.GetFormParameter(this.request, "chkAddMRR", false) != null);
				if (this.userContext.IsFeatureEnabled(Feature.Contacts))
				{
					string formParameter4 = Utilities.GetFormParameter(this.request, "anrFst");
					this.checkNameInContactsFirst = (!string.IsNullOrEmpty(formParameter4) && formParameter4 == "1");
				}
				if (!this.isClearMrrRequest)
				{
					bool flag = false;
					if (this.displayItemsPerPage != this.userContext.UserOptions.BasicViewRowCount)
					{
						this.userContext.UserOptions.BasicViewRowCount = this.displayItemsPerPage;
						flag = true;
					}
					if (this.nextSelection != this.userContext.UserOptions.NextSelection)
					{
						this.userContext.UserOptions.NextSelection = this.nextSelection;
						flag = true;
					}
					if (this.userContext.IsFeatureEnabled(Feature.Signature))
					{
						if (this.autoAddSignature != this.userContext.UserOptions.AutoAddSignature)
						{
							this.userContext.UserOptions.AutoAddSignature = this.autoAddSignature;
							flag = true;
						}
						if (!Utilities.WhiteSpaceOnlyOrNullEmpty(this.signatureText) && (Utilities.WhiteSpaceOnlyOrNullEmpty(this.userContext.UserOptions.SignatureText) || !string.IsNullOrEmpty(Utilities.GetFormParameter(this.request, "chkRplSg", false))))
						{
							this.userContext.UserOptions.SignatureText = this.signatureText;
							this.signatureHtml = BodyConversionUtilities.ConvertTextToHtml(this.signatureText);
							this.userContext.UserOptions.SignatureHtml = this.signatureHtml;
							flag = true;
						}
					}
					if (this.userContext.UserOptions.EmptyDeletedItemsOnLogoff != this.emptyDeletedItemsOnLogoff)
					{
						this.userContext.UserOptions.EmptyDeletedItemsOnLogoff = this.emptyDeletedItemsOnLogoff;
						flag = true;
					}
					if (this.userContext.UserOptions.ReadReceipt != this.readReceiptResponse)
					{
						this.userContext.UserOptions.ReadReceipt = this.readReceiptResponse;
						flag = true;
					}
					if (this.userContext.UserOptions.AddRecipientsToAutoCompleteCache != this.addRecentRecipientsToMrr)
					{
						this.userContext.UserOptions.AddRecipientsToAutoCompleteCache = this.addRecentRecipientsToMrr;
						flag = true;
					}
					if (this.userContext.IsFeatureEnabled(Feature.Contacts) && this.userContext.UserOptions.CheckNameInContactsFirst != this.checkNameInContactsFirst)
					{
						this.userContext.UserOptions.CheckNameInContactsFirst = this.checkNameInContactsFirst;
						flag = true;
					}
					if (flag)
					{
						try
						{
							this.userContext.UserOptions.CommitChanges();
							base.SetSavedSuccessfully(true);
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
			}
		}

		private void RenderMessagingOptions()
		{
			base.RenderHeaderRow(ThemeFileId.EMailLarge, 1847724788);
			this.writer.Write("<tr><td class=\"bd\">");
			this.writer.Write("<table class=\"fmt\">");
			this.writer.Write("<tr><td nowrap>");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(1884547222));
			this.writer.Write("</td><td class=\"w100\">");
			this.RenderViewerRowCount();
			this.writer.Write("</td></tr>");
			this.writer.Write("<tr><td nowrap>");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(888173523));
			this.writer.Write("</td><td class=\"w100\">");
			this.RenderNextSelection();
			this.writer.Write("</td></tr>");
			this.writer.Write("</table>");
			this.writer.Write("</td></tr>");
		}

		private void RenderViewerRowCount()
		{
			int num = 100;
			this.writer.Write("<select name=\"");
			this.writer.Write("selRwCnt");
			this.writer.Write("\" onchange=\"onChgSel(this);\">");
			int i;
			for (i = 5; i < 50; i += 5)
			{
				if (i >= num)
				{
					break;
				}
				this.writer.Write("<option value=\"{1}\"{0}>{2}</option>", (i == this.displayItemsPerPage) ? " selected" : string.Empty, i, i);
			}
			while (i <= num)
			{
				this.writer.Write("<option value=\"{1}\"{0}>{2}</option>", (i == this.displayItemsPerPage) ? " selected" : string.Empty, i, i);
				i += 25;
			}
			this.writer.Write("</select>");
		}

		private void RenderNextSelection()
		{
			this.writer.Write("<select name=\"");
			this.writer.Write("selNxt");
			this.writer.Write("\" onchange=\"onChgSel(this);\">");
			this.writer.Write("<option value=\"{1}\"{0}>{2}</option>", (this.nextSelection == NextSelectionDirection.Previous) ? " selected" : string.Empty, 0, LocalizedStrings.GetHtmlEncoded(1192767596));
			this.writer.Write("<option value=\"{1}\"{0}>{2}</option>", (this.nextSelection == NextSelectionDirection.Next) ? " selected" : string.Empty, 1, LocalizedStrings.GetHtmlEncoded(-1347357868));
			this.writer.Write("<option value=\"{1}\"{0}>{2}</option>", (this.nextSelection == NextSelectionDirection.ReturnToView) ? " selected" : string.Empty, 2, LocalizedStrings.GetHtmlEncoded(21771688));
			this.writer.Write("</select>");
		}

		private void RenderEmailSignature()
		{
			base.RenderHeaderRow(ThemeFileId.SignatureLarge, -735243648);
			this.writer.Write("<tr><td class=\"bd\">");
			this.writer.Write("<input type=\"checkbox\" name=\"{0}\"{1} id=\"{0}\" onclick=\"return onClkChkBx(this);\" value=\"1\"><label for=\"{0}\">{2}</label>", "chkAddSg", this.autoAddSignature ? " checked" : string.Empty, LocalizedStrings.GetHtmlEncoded(780915179));
			this.writer.Write("</td></tr>");
			if (!string.IsNullOrEmpty(this.signatureHtml))
			{
				this.writer.Write("<tr><td>");
				this.writer.Write("<table class=\"csg\" cellpadding=0 cellspacing=0>");
				this.writer.Write("<tr><td class=\"dsl\" rowspan=3><img src=\"");
				this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.Clear);
				this.writer.Write("\" alt=\"\" class=\"wh1\"></td><td class=\"w100\"></td><td class=\"dsr\" rowspan=3><img src=\"");
				this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.Clear);
				this.writer.Write("\" alt=\"\" class=\"wh1\"></td></tr>");
				this.writer.Write("<tr><td class=\"ds\">");
				this.writer.Write(Utilities.SanitizeHtml(this.signatureHtml));
				this.writer.Write("</td></tr>");
				this.writer.Write("<tr><td></td></tr></table>");
				this.writer.Write("<table class=\"csgnb\"><tr><td class=\"df\"><input type=\"checkbox\" name=\"");
				this.writer.Write("chkRplSg");
				this.writer.Write("\" id=\"");
				this.writer.Write("chkRplSg");
				this.writer.Write("\" onclick=\"return onClkChkBx(this);\" value=\"1\"");
				this.writer.Write((this.isClearMrrRequest && Utilities.GetFormParameter(this.request, "chkRplSg", false) != null) ? " checked" : string.Empty);
				this.writer.Write("><label for=\"");
				this.writer.Write("chkRplSg");
				this.writer.Write("\">");
				this.writer.Write(LocalizedStrings.GetHtmlEncoded(-1257264716));
				this.writer.Write("</label></td></tr>");
				this.writer.Write("<tr><td class=\"w100\">");
				this.writer.Write("<textarea name=\"");
				this.writer.Write("txtSg");
				this.writer.Write("\" class=\"w100\" title=\"");
				this.writer.Write(LocalizedStrings.GetHtmlEncoded(-977498918));
				this.writer.Write("\" rows=6 cols=61 onfocus=\"onFcsTxt('");
				this.writer.Write("chkRplSg");
				this.writer.Write("');\">");
				if (this.isClearMrrRequest && !string.IsNullOrEmpty(this.signatureText))
				{
					Utilities.HtmlEncode(this.signatureText, this.writer);
				}
				this.writer.Write("</textarea>");
				this.writer.Write("</td></tr>");
				this.writer.Write("</table></td></tr>");
				return;
			}
			this.writer.Write("<tr><td>");
			this.writer.Write("<table class=\"csg\" cellpadding=0 cellspacing=0>");
			this.writer.Write("<tr><td class=\"w100\">");
			this.writer.Write("<textarea name=\"");
			this.writer.Write("txtSg");
			this.writer.Write("\" class=\"w100\" title=\"");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(-977498918));
			this.writer.Write("\" rows=6 cols=61>");
			if (this.isClearMrrRequest && !string.IsNullOrEmpty(this.signatureText))
			{
				Utilities.HtmlEncode(this.signatureText, this.writer);
			}
			this.writer.Write("</textarea>");
			this.writer.Write("</table></td></tr>");
		}

		private void RenderDeletedItems()
		{
			base.RenderHeaderRow(ThemeFileId.EmptyDeletedItems, -681344097);
			this.writer.Write("<tr><td class=\"bd\">");
			this.writer.Write("<input type=\"checkbox\" name=\"{0}\"{1} id=\"{0}\" onclick=\"return onClkChkBx(this);\" value=\"1\"><label for=\"{0}\">{2}</label>", "chkEmDel", this.emptyDeletedItemsOnLogoff ? " checked" : string.Empty, LocalizedStrings.GetHtmlEncoded(1812531684));
			this.writer.Write("</td></tr>");
		}

		private void RenderMessageTracking()
		{
			base.RenderHeaderRow(ThemeFileId.EMailExtraLarge, 1399211590);
			this.writer.Write("<tr><td class=\"bd\"><table class=\"fmt\">");
			this.writer.Write("<tr><td><input type=\"radio\" id=\"rdoAskRsp\" name=\"rdRcpt\" value=\"0\"");
			if (this.readReceiptResponse == ReadReceiptResponse.DoNotAutomaticallySend)
			{
				this.writer.Write(" checked");
			}
			this.writer.Write("><label for=\"rdoAskRsp\">");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(-416380764));
			this.writer.Write("</label></td></tr>");
			this.writer.Write("<tr><td><input type=\"radio\" id=\"rdoYesRsp\" name=\"rdRcpt\" value=\"1\"");
			if (this.readReceiptResponse == ReadReceiptResponse.AlwaysSend)
			{
				this.writer.Write(" checked");
			}
			this.writer.Write("><label for=\"rdoYesRsp\">");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(-1477686773));
			this.writer.Write("</label></td></tr>");
			this.writer.Write("<tr><td><input type=\"radio\" id=\"rdoNoRsp\" name=\"rdRcpt\" value=\"2\"");
			if (this.readReceiptResponse == ReadReceiptResponse.NeverSend)
			{
				this.writer.Write(" checked");
			}
			this.writer.Write("><label for=\"rdoNoRsp\">");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(-1975510762));
			this.writer.Write("</label></td></tr>");
			this.writer.Write("</table></td></tr>");
		}

		private void RenderEmailNameResolution()
		{
			base.RenderHeaderRow(ThemeFileId.AnrOptions, 780065525);
			this.writer.Write("<tr><td class=\"bd\"><table class=\"fmt\">");
			this.writer.Write("<tr><td><input type=\"checkbox\" id=\"chkAddMRR\" name=\"chkAddMRR\"");
			if (this.addRecentRecipientsToMrr)
			{
				this.writer.Write(" checked");
			}
			this.writer.Write("><label for=\"chkAddMRR\">");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(-1675794646));
			this.writer.Write("</label></td></tr>");
			this.writer.Write("<tr><td id=\"cmrr\"><a href=\"#\" onclick=\"return onClkClrMRR();\">");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(480488025));
			this.writer.Write("</a></td></tr></table></td></tr>");
			if (this.userContext.IsFeatureEnabled(Feature.Contacts))
			{
				this.writer.Write("<tr><td class=\"bd\"><table class=\"fmt\"><tr><td>");
				this.writer.Write(LocalizedStrings.GetHtmlEncoded(-1914279991));
				this.writer.Write("</td></tr>");
				this.writer.Write("<tr><td><input type=\"radio\" id=\"rdoGAL\" name=\"anrFst\" value=\"0\"");
				if (!this.checkNameInContactsFirst)
				{
					this.writer.Write(" checked");
				}
				this.writer.Write("><label for=\"rdoGAL\">");
				this.writer.Write(LocalizedStrings.GetHtmlEncoded(1164140307));
				this.writer.Write("</label></td></tr>");
				this.writer.Write("<tr><td><input type=\"radio\" id=\"rdoCtcts\" name=\"anrFst\" value=\"1\"");
				if (this.checkNameInContactsFirst)
				{
					this.writer.Write(" checked");
				}
				this.writer.Write("><label for=\"rdoCtcts\">");
				this.writer.Write(LocalizedStrings.GetHtmlEncoded(1716044995));
				this.writer.Write("</label></td></tr></table></td></tr>");
			}
		}

		private const string FormRowCount = "selRwCnt";

		private const string FormJavaScriptRowCount = "a_iRwCnt";

		private const string FormNextSelection = "selNxt";

		private const string FormJavaScriptNextSelection = "a_iNxt";

		private const string FormAddSignature = "chkAddSg";

		private const string FormJavaScriptAddSignature = "a_fAddSg";

		private const string FormReplaceSignature = "chkRplSg";

		private const string FormReplaceSignatureText = "txtSg";

		private const string FormEmptyDeletedItemsOnLogOff = "chkEmDel";

		private const string FormJavaScriptEmptyDeletedItemsOnLogOff = "a_fEDI";

		private const string FormJavaScriptSignaturePresent = "a_fSgPr";

		private const string FormReadReceipt = "rdRcpt";

		private const string FormJavaScriptReadReceipt = "a_iRdRcpt";

		private const string FormAddToMrr = "chkAddMRR";

		private const string FormJavaScriptAddToMrr = "a_fAddMRR";

		private const string FormAnrFirst = "anrFst";

		private const string FormJavaScriptAnrFirst = "a_iAnrFst";

		private const string FormJavaScriptClearMrrPrompt = "a_sPrClrMRR";

		private const string AnrInContactFirstValue = "1";

		private const string ClearMostRecentReceiptCommand = "ClrMRR";

		private const string Option = "<option value=\"{1}\"{0}>{2}</option>";

		private const string FormCheckBox = "<input type=\"checkbox\" name=\"{0}\"{1} id=\"{0}\" onclick=\"return onClkChkBx(this);\" value=\"1\"><label for=\"{0}\">{2}</label>";

		private int displayItemsPerPage;

		private NextSelectionDirection nextSelection = NextSelectionDirection.Next;

		private bool autoAddSignature;

		private string signatureText;

		private string signatureHtml;

		private ReadReceiptResponse readReceiptResponse;

		private bool checkNameInContactsFirst;

		private bool addRecentRecipientsToMrr;

		private bool emptyDeletedItemsOnLogoff;

		private bool isClearMrrRequest;
	}
}
