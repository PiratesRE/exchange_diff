using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	public class ReadADDistributionList : ReadADRecipient
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.adGroup = (IADDistributionList)base.ADRecipient;
		}

		protected void RenderHeader(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<table cellpadding=0 cellspacing=0 class=\"pHd\">");
			writer.Write("<tr><td class=\"dn pLT\">");
			Utilities.HtmlEncode(base.ADRecipient.DisplayName, writer);
			writer.Write("</td></tr>");
			writer.Write("<tr><td class=\"spcOP\"></td></tr>");
			writer.Write("</table>");
		}

		protected void RenderDetailsBucket(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<table cellpadding=0 cellspacing=0 class=\"pDtls\">");
			ReadADRecipient.RenderDetailHeader(writer, -2101430728);
			writer.Write("<tr><td class=\"lbl lp\">");
			writer.Write(LocalizedStrings.GetHtmlEncoded(613689222));
			writer.Write("</td>");
			if (!base.UserContext.IsWebPartRequest)
			{
				writer.Write("<td><a href=\"#\" class=\"peer\" onclick=\"return onSendMail();\">");
			}
			else
			{
				writer.Write("<td class=\"txvl\">");
			}
			Utilities.HtmlEncode(base.ADRecipient.Alias, writer);
			if (!base.UserContext.IsWebPartRequest)
			{
				writer.Write("</a>");
			}
			writer.Write("</td></tr>");
			ADObjectId managedBy = this.adGroup.ManagedBy;
			ADRecipient adrecipient = null;
			if (managedBy != null)
			{
				adrecipient = base.ADRecipientSession.Read(managedBy);
			}
			if (adrecipient != null)
			{
				string base64StringFromADObjectId = Utilities.GetBase64StringFromADObjectId(managedBy);
				writer.Write("<tr><td class=\"lbl lp\" nowrap>");
				writer.Write(LocalizedStrings.GetHtmlEncoded(-1563830359));
				writer.Write("</td><td><a href=\"#\"");
				writer.Write(" id=\"");
				Utilities.HtmlEncode(base64StringFromADObjectId, writer);
				writer.Write("\" onClick=\"return loadOrgP(this,");
				writer.Write(1);
				writer.Write(")\" class=\"peer\">");
				Utilities.HtmlEncode(adrecipient.Name, writer);
				writer.Write("</a></td></tr>");
			}
			writer.Write("<tr><td class=\"spcOP\" colspan=2></td></tr>");
			ReadADRecipient.RenderDetailHeader(writer, -905993889);
			this.RenderMembers(writer);
			writer.Write("</table>");
		}

		private void RenderMembers(TextWriter writer)
		{
			List<OWARecipient> list = Utilities.LoadAndSortDistributionListMembers(this.adGroup);
			if (list.Count == 0)
			{
				writer.Write("<tr><td colspan=2 class=\"nodtls msgpd\">");
				writer.Write(LocalizedStrings.GetHtmlEncoded(908949145));
				writer.Write("</td></tr>");
				return;
			}
			writer.Write("<tr><td class=\"lbl lp\">");
			writer.Write(LocalizedStrings.GetHtmlEncoded(1099536643));
			writer.Write("</td><td>");
			writer.Write("<table cellpadding=0 cellspacing=0 class=\"drpts\">");
			foreach (OWARecipient owarecipient in list)
			{
				string s = Convert.ToBase64String(owarecipient.Id.ObjectGuid.ToByteArray());
				writer.Write("<tr><td class=\"dltdpd\"><a href=\"#\" id=\"");
				Utilities.HtmlEncode(s, writer);
				writer.Write("\" onClick=\"return loadOrgP(this,");
				if (Utilities.IsADDistributionList(owarecipient.UserRecipientType))
				{
					writer.Write(2);
					writer.Write(")\" class=\"peer dl\"><img src=\"");
					base.UserContext.RenderThemeFileUrl(writer, ThemeFileId.ContactDL);
					writer.Write("\" ");
					Utilities.RenderImageAltAttribute(writer, base.UserContext, ThemeFileId.ContactDL);
					writer.Write(">");
					if (string.IsNullOrEmpty(owarecipient.DisplayName))
					{
						writer.Write(LocalizedStrings.GetHtmlEncoded(-808148510));
					}
					else
					{
						Utilities.HtmlEncode(owarecipient.DisplayName, writer);
					}
					writer.Write("</a></td></tr>");
				}
				else
				{
					writer.Write(1);
					writer.Write(")\" class=\"peer\">");
					Utilities.HtmlEncode(owarecipient.DisplayName, writer);
					writer.Write("</a></td></tr>");
				}
			}
			writer.Write("</table></td></tr>");
		}

		protected void RenderAddressBucket(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<table cellpadding=0 cellspacing=0 class=\"pAddr\">");
			ReadADRecipient.RenderAddressHeader(writer, 1601836855);
			writer.Write("<tr><td class=\"rp\"><textarea name=\"notes\" rows=10 cols=32 readonly>");
			Utilities.HtmlEncode(base.ADRecipient.Notes, writer);
			writer.Write("</textarea></td></tr></table>");
		}

		private IADDistributionList adGroup;
	}
}
