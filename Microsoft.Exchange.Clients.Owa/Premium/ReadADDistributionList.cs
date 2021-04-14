using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class ReadADDistributionList : ReadADRecipientPage, IRegistryOnlyForm
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.distributionList = (base.ADRecipient as IADDistributionList);
			if (this.distributionList == null)
			{
				throw new OwaInvalidRequestException();
			}
			this.sMimeEnabled = Utilities.IsClientSMimeControlUsable(Utilities.CheckClientSMimeControlStatus(Utilities.GetQueryStringParameter(base.Request, "smime", false), base.OwaContext));
		}

		protected bool SMimeEnabled
		{
			get
			{
				return this.sMimeEnabled;
			}
		}

		protected void RenderListLink()
		{
			if (base.ADRecipient.RecipientType == RecipientType.Group)
			{
				Utilities.HtmlEncode(base.ADRecipient.Alias, base.Response.Output);
				return;
			}
			this.RenderRecipientLink();
		}

		protected void RenderOwnerLink()
		{
			if (this.distributionList.ManagedBy != null)
			{
				ADObjectId managedBy = this.distributionList.ManagedBy;
				ADRecipient adrecipient = base.ADRecipientSession.Read(managedBy);
				if (adrecipient != null)
				{
					base.Response.Write("<div class=\"row\"><div class=\"lbl\">" + LocalizedStrings.GetHtmlEncoded(-1563830359) + "</div><div class=\"fld\">");
					this.RenderMemberLink(adrecipient, false);
					base.Response.Write("</div></div>");
				}
			}
		}

		protected void RenderMemberList()
		{
			List<OWARecipient> list = Utilities.LoadAndSortDistributionListMembers(this.distributionList, this.SMimeEnabled);
			foreach (OWARecipient owarecipient in list)
			{
				RecipientType userRecipientType = owarecipient.UserRecipientType;
				base.Response.Write("<tr>");
				base.Response.Write("<td class='adDlMmbrsLft'>");
				if (owarecipient.IsDistributionList)
				{
					base.UserContext.RenderThemeImage(base.Response.Output, ThemeFileId.DistributionListOther);
				}
				else
				{
					base.UserContext.RenderThemeImage(base.Response.Output, ThemeFileId.DistributionListUser);
				}
				base.Response.Write("</td>");
				base.Response.Write("<td class=adDlMmbrsRt");
				if (this.SMimeEnabled)
				{
					if (owarecipient.IsDistributionList)
					{
						base.Response.Write(" _em=\"");
						Utilities.HtmlEncode(owarecipient.LegacyDN, base.Response.Output);
						base.Response.Write('"');
					}
					else if (!owarecipient.HasValidDigitalId)
					{
						base.Response.Write(" _nc=1");
					}
				}
				base.Response.Write(">");
				this.RenderMemberLink(owarecipient, false);
				base.Response.Write("</td>");
				if (base.UserContext.IsPhoneticNamesEnabled)
				{
					base.Response.Write("<td class=adDlMmbrsRt>");
					this.RenderMemberLink(owarecipient, true);
					base.Response.Write("</td>");
				}
				base.Response.Write("</tr>");
			}
		}

		private void RenderMemberLink(ADRecipient recipient, bool usePhoneticName)
		{
			this.RenderMemberLink(recipient.Id, usePhoneticName ? recipient.PhoneticDisplayName : recipient.DisplayName, recipient.RecipientType);
		}

		private void RenderMemberLink(OWARecipient recipient, bool usePhoneticName)
		{
			this.RenderMemberLink(recipient.Id, usePhoneticName ? recipient.PhoneticDisplayName : recipient.DisplayName, recipient.UserRecipientType);
		}

		private void RenderMemberLink(ADObjectId id, string displayName, RecipientType recipientType)
		{
			string s;
			if (Utilities.IsADDistributionList(recipientType))
			{
				s = "ADDistList";
			}
			else
			{
				s = "AD.RecipientType.User";
			}
			if (string.IsNullOrEmpty(displayName))
			{
				displayName = LocalizedStrings.GetNonEncoded(-808148510);
			}
			string base64StringFromADObjectId = Utilities.GetBase64StringFromADObjectId(id);
			string handlerCode = string.Format("openItmRdFm(\"{0}\",\"{1}\");", Utilities.JavascriptEncode(s), Utilities.JavascriptEncode(base64StringFromADObjectId));
			base.Response.Write("<a class=lnk ");
			base.Response.Write(Utilities.GetScriptHandler("onclick", handlerCode));
			base.Response.Write(">");
			Utilities.HtmlEncode(displayName, base.Response.Output);
			base.Response.Write("</a>");
		}

		private void RenderRecipientLink()
		{
			string handlerCode = string.Format("opnNwMsg(\"{0}\",\"{1}\",\"\",\"{2}\");", Utilities.JavascriptEncode(base.ADRecipient.LegacyExchangeDN), Utilities.JavascriptEncode(base.ADRecipient.DisplayName), Utilities.JavascriptEncode(2.ToString()));
			base.Response.Write("<a class=lnk ");
			Utilities.RenderScriptHandler(base.Response.Output, "onclick", handlerCode);
			base.Response.Write(">");
			Utilities.HtmlEncode(base.ADRecipient.Alias, base.Response.Output);
			base.Response.Write("</a>");
		}

		protected void RenderNotes()
		{
			RecipientType recipientType = base.ADRecipient.RecipientType;
			if (recipientType == RecipientType.Group || recipientType == RecipientType.MailUniversalDistributionGroup || recipientType == RecipientType.MailUniversalSecurityGroup || recipientType == RecipientType.MailNonUniversalGroup)
			{
				ADGroup adgroup = (ADGroup)this.distributionList;
				if (!string.IsNullOrEmpty(adgroup.Notes))
				{
					base.Response.Write("<div class=\"row2sp\"><div class=\"secCol\"><span class=spS>" + LocalizedStrings.GetHtmlEncoded(1601836855) + "</span></div><div class=\"fltBefore\"><textarea class=\"adNts\" readonly>");
					Utilities.HtmlEncode(adgroup.Notes, base.Response.Output);
					base.Response.Write("</textarea></div>");
				}
			}
		}

		protected void RenderSecureMessaging()
		{
			if (!this.SMimeEnabled || base.ADRecipient.RecipientType == RecipientType.DynamicDistributionGroup)
			{
				return;
			}
			bool hiddenGroupMembershipEnabled = ((ADGroup)base.ADRecipient).HiddenGroupMembershipEnabled;
			base.Response.Write("<div class=\"row2sp\"><div class=\"secCol\"><span class=spS>");
			base.Response.Write(LocalizedStrings.GetHtmlEncoded(-2096722623));
			base.Response.Write("</span></div><div class=\"lbl noindent\"");
			if (!hiddenGroupMembershipEnabled)
			{
				base.Response.Write(" id=tdSM");
			}
			base.Response.Write('>');
			if (hiddenGroupMembershipEnabled)
			{
				base.Response.Write(LocalizedStrings.GetHtmlEncoded(2141668304));
			}
			else
			{
				base.Response.Write("<span id=spGCPrg style=\"display:none\"><span class=t><img src=\"");
				base.UserContext.RenderThemeFileUrl(base.Response.Output, ThemeFileId.ProgressSmall);
				base.Response.Write("\"></span> <span id=\"spSPS\">");
				base.Response.Write(LocalizedStrings.GetHtmlEncoded(-695375226));
				base.Response.Write("</span></span>");
			}
			base.Response.Write("</div></div>");
		}

		private IADDistributionList distributionList;

		private bool sMimeEnabled;
	}
}
