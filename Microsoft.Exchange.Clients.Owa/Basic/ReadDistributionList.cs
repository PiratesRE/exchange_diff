using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	public class ReadDistributionList : ReadContactBase
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.distributionList = base.Initialize<DistributionList>(new PropertyDefinition[0]);
			base.Module = Navigation.GetNavigationModuleFromFolder(base.UserContext, base.FolderStoreObjectId);
		}

		protected string DisplayName
		{
			get
			{
				if (Utilities.WhiteSpaceOnlyOrNullEmpty(this.distributionList.DisplayName))
				{
					return LocalizedStrings.GetNonEncoded(-808148510);
				}
				return this.distributionList.DisplayName;
			}
		}

		protected string ItemIdString
		{
			get
			{
				return base.ItemId.ToBase64String();
			}
		}

		protected void RenderDetailsBucket(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<table cellpadding=0 cellspacing=0 class=\"pDtls\">");
			writer.Write("<tr><td colspan=2 class=\"hd lp\">");
			writer.Write(LocalizedStrings.GetHtmlEncoded(-905993889));
			writer.Write("</td></tr>");
			bool flag = false;
			foreach (DistributionListMember distributionListMember in this.distributionList)
			{
				if (!(distributionListMember.Participant == null))
				{
					Participant participant = distributionListMember.Participant;
					writer.Write("<tr><td class=\"lbl lp\" nowrap>");
					if (!flag)
					{
						writer.Write(LocalizedStrings.GetHtmlEncoded(1099536643));
						flag = true;
					}
					writer.Write("</td>");
					string routingType;
					if ((routingType = participant.RoutingType) == null)
					{
						goto IL_E7;
					}
					if (!(routingType == "MAPIPDL"))
					{
						if (!(routingType == "EX"))
						{
							if (!(routingType == "SMTP"))
							{
								goto IL_E7;
							}
							goto IL_E7;
						}
						else
						{
							this.RenderADParticipant(writer, participant);
						}
					}
					else
					{
						this.RenderPDLParticipant(writer, participant);
					}
					IL_EF:
					writer.Write("</tr>");
					continue;
					IL_E7:
					this.RenderSmtpParticipant(writer, participant);
					goto IL_EF;
				}
			}
			writer.Write("</table>");
		}

		private void RenderADParticipant(TextWriter writer, Participant participant)
		{
			if (this.adRecipientSession == null)
			{
				this.adRecipientSession = Utilities.CreateADRecipientSession(Microsoft.Exchange.Clients.Owa.Core.Culture.GetUserCulture().LCID, true, ConsistencyMode.IgnoreInvalid, true, base.UserContext);
			}
			ADRecipient recipientByLegacyExchangeDN = Utilities.GetRecipientByLegacyExchangeDN(this.adRecipientSession, participant.EmailAddress);
			bool flag = recipientByLegacyExchangeDN is IADDistributionList;
			if (recipientByLegacyExchangeDN != null && (flag || recipientByLegacyExchangeDN is IADOrgPerson))
			{
				this.RenderMemberWithIcon(writer, participant.DisplayName, Utilities.GetBase64StringFromADObjectId(recipientByLegacyExchangeDN.Id), flag ? ListViewContents.ReadItemType.AdDistributionList : ListViewContents.ReadItemType.AdOrgPerson, flag ? ThemeFileId.DistributionListOther : ThemeFileId.AddressBook, flag);
				return;
			}
			writer.Write("<td class=\"txvl pdlncn\">");
			Utilities.HtmlEncode(participant.DisplayName, writer);
			writer.Write("</td>");
		}

		private void RenderPDLParticipant(TextWriter writer, Participant participant)
		{
			StoreParticipantOrigin storeParticipantOrigin = participant.Origin as StoreParticipantOrigin;
			if (storeParticipantOrigin != null)
			{
				StoreObjectId originItemId = storeParticipantOrigin.OriginItemId;
				this.RenderMemberWithIcon(writer, participant.DisplayName, originItemId.ToBase64String(), ListViewContents.ReadItemType.ContactDistributionList, ThemeFileId.DistributionListOther, true);
				return;
			}
			writer.Write("<td class=\"txvl pdlncn\"><img src=\"");
			base.UserContext.RenderThemeFileUrl(writer, ThemeFileId.DistributionListOther);
			writer.Write("\"><b>");
			Utilities.HtmlEncode(participant.DisplayName, writer);
			writer.Write("</b></td>");
		}

		private void RenderSmtpParticipant(TextWriter writer, Participant participant)
		{
			StoreParticipantOrigin storeParticipantOrigin = participant.Origin as StoreParticipantOrigin;
			if (storeParticipantOrigin != null)
			{
				StoreObjectId originItemId = storeParticipantOrigin.OriginItemId;
				this.RenderMemberWithIcon(writer, participant.DisplayName, originItemId.ToBase64String(), ListViewContents.ReadItemType.Contact, ThemeFileId.Contact, false);
				return;
			}
			writer.Write("<td class=\"txvl pdlncn\">");
			base.RenderEmail(writer, participant.DisplayName, participant.EmailAddress, participant.RoutingType, null, EmailAddressIndex.None);
			writer.Write("</td>");
		}

		private void RenderMemberWithIcon(TextWriter writer, string name, string id, ListViewContents.ReadItemType type, ThemeFileId themeFileId, bool isBold)
		{
			writer.Write("<td class=\"txvl phtdpd\"><a href=\"#\" id=\"");
			Utilities.HtmlEncode(id, writer);
			writer.Write("\" onClick=\"return onClkRcpt(this,");
			int num = (int)type;
			writer.Write(num.ToString());
			writer.Write(");\" class=\"map\"><img src=\"");
			base.UserContext.RenderThemeFileUrl(writer, themeFileId);
			writer.Write("\">");
			if (isBold)
			{
				writer.Write("<b>");
			}
			Utilities.HtmlEncode(name, writer);
			if (isBold)
			{
				writer.Write("</b>");
			}
			writer.Write("</a></td>");
		}

		protected void RenderProfileBucket(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<table cellpadding=0 cellspacing=0 class=\"pAddr\">");
			base.RenderNotes(writer);
			writer.Write("</table>");
		}

		private DistributionList distributionList;

		private IRecipientSession adRecipientSession;
	}
}
