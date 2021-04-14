using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	public class ReadContact : ReadContactBase
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.contact = base.Initialize<Contact>(ContactUtilities.PrefetchProperties);
			base.CreateAttachmentHelpers(AttachmentWellType.ReadOnly);
			this.displayName = this.GetPropertyValue(ContactBaseSchema.FileAs);
			if (Utilities.WhiteSpaceOnlyOrNullEmpty(this.displayName))
			{
				this.displayName = LocalizedStrings.GetNonEncoded(-1873027801);
			}
			this.htmlEncodedTitle = this.GetHtmlEncodedValue(ContactSchema.Title);
			this.htmlEncodedDepartment = this.GetHtmlEncodedValue(ContactSchema.Department);
			this.htmlEncodedCompany = this.GetHtmlEncodedValue(ContactSchema.CompanyName);
			base.Module = Navigation.GetNavigationModuleFromFolder(base.UserContext, base.FolderStoreObjectId);
			this.isPhoneticNamesEnabled = Utilities.IsJapanese;
		}

		protected string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		protected string ItemIdString
		{
			get
			{
				return base.ItemId.ToBase64String();
			}
		}

		protected void RenderHeader(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<table cellpadding=0 cellspacing=0 class=\"pHd\"><tr>");
			string text = this.RenderContactPicture(writer);
			if (!string.IsNullOrEmpty(text))
			{
				writer.Write("<td style=\"width:4%;vertical-align:middle;\" class=\"cntpic\">");
				writer.Write("<table cellpadding=0 cellspacing=0><tr><td width=\"100%\">");
				writer.Write("<img src=\"");
				Utilities.HtmlEncode(text, writer);
				writer.Write("\" alt=\"\"></td></tr></table></td>");
			}
			writer.Write("<td><table cellpadding=0 cellspacing=0 width=\"100%\">");
			writer.Write("<tr><td class=\"dn pLT\">");
			if (this.isPhoneticNamesEnabled)
			{
				string htmlEncodedValue = this.GetHtmlEncodedValue(ContactSchema.YomiLastName);
				string htmlEncodedValue2 = this.GetHtmlEncodedValue(ContactSchema.YomiFirstName);
				if (htmlEncodedValue.Length > 0 || htmlEncodedValue2.Length > 0)
				{
					writer.Write("<span class=\"txnr\">");
					if (htmlEncodedValue.Length > 0)
					{
						writer.Write(htmlEncodedValue);
						writer.Write("&nbsp;");
					}
					if (htmlEncodedValue2.Length > 0)
					{
						writer.Write(htmlEncodedValue2);
					}
					writer.Write("</span><br>");
				}
			}
			Utilities.HtmlEncode(this.displayName, writer);
			writer.Write("</td></tr>");
			writer.Write("<tr><td class=\"pLT\">");
			if (!string.IsNullOrEmpty(this.htmlEncodedTitle))
			{
				writer.Write("<span class=\"txb\">");
				writer.Write(this.htmlEncodedTitle);
				writer.Write(", </span>");
			}
			writer.Write("<span class=\"txnr\">");
			writer.Write(this.htmlEncodedDepartment);
			writer.Write("</span></td></tr>");
			writer.Write("<tr><td class=\"pLT pB\"><span class=\"txnr\">");
			writer.Write(this.htmlEncodedCompany);
			writer.Write("</span></td></tr></table></td>");
			writer.Write("</tr></table>");
		}

		protected string RenderContactPicture(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			foreach (AttachmentHandle handle in base.Item.AttachmentCollection)
			{
				using (Attachment attachment = base.Item.AttachmentCollection.Open(handle))
				{
					attachment.Load(new PropertyDefinition[]
					{
						AttachmentSchema.IsContactPhoto
					});
					if (attachment.IsContactPhoto)
					{
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.Append("attachment.ashx?");
						if (base.IsEmbeddedItem)
						{
							stringBuilder.Append(AttachmentWell.RenderEmbeddedQueryString(base.ParentItem.Id.ObjectId.ToBase64String()));
							stringBuilder.Append(Utilities.UrlEncode(attachment.Id.ToBase64String()));
						}
						else
						{
							stringBuilder.Append("attach=1&id=");
							stringBuilder.Append(Utilities.UrlEncode(base.Item.Id.ObjectId.ToBase64String()));
							stringBuilder.Append("&attid0=");
							stringBuilder.Append(Utilities.UrlEncode(attachment.Id.ToBase64String()));
							stringBuilder.Append("&attcnt=1");
						}
						return stringBuilder.ToString();
					}
				}
			}
			return null;
		}

		protected void RenderDetailsBucket(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<table cellpadding=0 cellspacing=0 class=\"pDtls\">");
			writer.Write("<tr><td colspan=2 class=\"hd lp\">");
			writer.Write(LocalizedStrings.GetHtmlEncoded(447307630));
			writer.Write("</td></tr>");
			this.RenderPhoneNumbers(writer);
			this.RenderEmailAddresses(writer);
			this.RenderWebPage(writer);
			if (!this.isPhoneNumberRendered && !this.isEmailRendered && !this.isWebAddressRendered)
			{
				writer.Write("<tr><td colspan=2 class=\"nodtls msgpd\">");
				writer.Write(LocalizedStrings.GetHtmlEncoded(1475823729));
				writer.Write("</td></tr>");
			}
			writer.Write("<tr><td class=\"spcHd\" colspan=2></td></tr>");
			this.RenderContactAddress(writer);
			if (!this.isAddressRendered)
			{
				writer.Write("<tr><td colspan=2 class=\"hd lp\">");
				writer.Write(LocalizedStrings.GetHtmlEncoded(-338392418));
				writer.Write("</td></tr>");
				writer.Write("<tr><td colspan=2 class=\"nodtls msgpd\">");
				writer.Write(LocalizedStrings.GetHtmlEncoded(-1150804419));
				writer.Write("</td></tr>");
			}
			writer.Write("<tr><td class=\"spcOP\" colspan=2></td></tr>");
			writer.Write("</table>");
		}

		private static void RenderADPerson(UserContext userContext, string label, string name, TextWriter writer)
		{
			if (name.Length == 0)
			{
				return;
			}
			string text = null;
			IRecipientSession recipientSession = Utilities.CreateADRecipientSession(ConsistencyMode.IgnoreInvalid, userContext);
			ADRecipient[] array = recipientSession.FindByANR(name, 2, null);
			if (array != null && array.Length == 1 && array[0].RecipientType == RecipientType.UserMailbox)
			{
				text = Utilities.GetBase64StringFromADObjectId(array[0].Id);
			}
			writer.Write("<tr><td class=\"lbl\">");
			writer.Write(label);
			if (text != null)
			{
				writer.Write("</td><td class=\"txvl\"><a href=\"#\" id=\"");
				Utilities.HtmlEncode(text, writer);
				writer.Write("\" onclick=\"return onClkRcpt(this,1)\"");
				writer.Write(" class=\"map\">");
				Utilities.HtmlEncode(name, writer);
				writer.Write("</a></td></tr>");
				return;
			}
			writer.Write("</td><td class=\"txvl\">");
			Utilities.HtmlEncode(name, writer);
			writer.Write("</td></tr>");
		}

		private void RenderPhoneNumbers(TextWriter writer)
		{
			this.RenderPhoneWithIcon(writer, LocalizedStrings.GetHtmlEncoded(-765825260), this.GetPropertyValue(ContactSchema.BusinessPhoneNumber), ThemeFileId.WorkPhone);
			this.RenderPhoneWithIcon(writer, LocalizedStrings.GetHtmlEncoded(-534169914), this.GetPropertyValue(ContactSchema.BusinessPhoneNumber2), ThemeFileId.WorkPhone);
			this.RenderPhoneWithIcon(writer, LocalizedStrings.GetHtmlEncoded(1158653436), this.GetPropertyValue(ContactSchema.MobilePhone), ThemeFileId.MobilePhone);
			this.RenderPhoneWithIcon(writer, LocalizedStrings.GetHtmlEncoded(1414246315), this.GetPropertyValue(ContactSchema.HomePhone), ThemeFileId.HomePhone);
			this.RenderPhoneWithIcon(writer, LocalizedStrings.GetHtmlEncoded(1930214269), this.GetPropertyValue(ContactSchema.HomePhone2), ThemeFileId.HomePhone);
			this.RenderPhoneWithIcon(writer, LocalizedStrings.GetHtmlEncoded(1442239260), this.GetPropertyValue(ContactSchema.PrimaryTelephoneNumber), ThemeFileId.PrimaryPhone);
			this.RenderPhoneWithIcon(writer, LocalizedStrings.GetHtmlEncoded(-11305699), this.GetPropertyValue(ContactSchema.WorkFax), ThemeFileId.Fax);
			this.RenderPhoneWithoutIcon(writer, LocalizedStrings.GetHtmlEncoded(-1816252206), this.GetPropertyValue(ContactSchema.AssistantPhoneNumber));
			this.RenderPhoneWithoutIcon(writer, LocalizedStrings.GetHtmlEncoded(-646524091), this.GetPropertyValue(ContactSchema.CallbackPhone));
			this.RenderPhoneWithoutIcon(writer, LocalizedStrings.GetHtmlEncoded(159631176), this.GetPropertyValue(ContactSchema.CarPhone));
			this.RenderPhoneWithoutIcon(writer, LocalizedStrings.GetHtmlEncoded(-1918812500), this.GetPropertyValue(ContactSchema.OrganizationMainPhone));
			this.RenderPhoneWithIcon(writer, LocalizedStrings.GetHtmlEncoded(1180016964), this.GetPropertyValue(ContactSchema.HomeFax), ThemeFileId.Fax);
			this.RenderPhoneWithoutIcon(writer, LocalizedStrings.GetHtmlEncoded(57098496), this.GetPropertyValue(ContactSchema.InternationalIsdnNumber));
			this.RenderPhoneWithIcon(writer, LocalizedStrings.GetHtmlEncoded(-679895069), this.GetPropertyValue(ContactSchema.OtherFax), ThemeFileId.Fax);
			this.RenderPhoneWithoutIcon(writer, LocalizedStrings.GetHtmlEncoded(-582599340), this.GetPropertyValue(ContactSchema.OtherTelephone));
			this.RenderPhoneWithoutIcon(writer, LocalizedStrings.GetHtmlEncoded(-1779142331), this.GetPropertyValue(ContactSchema.Pager));
			this.RenderPhoneWithoutIcon(writer, LocalizedStrings.GetHtmlEncoded(-166006211), this.GetPropertyValue(ContactSchema.RadioPhone));
			this.RenderPhoneWithoutIcon(writer, LocalizedStrings.GetHtmlEncoded(1096044911), this.GetPropertyValue(ContactSchema.TelexNumber));
			this.RenderPhoneWithoutIcon(writer, LocalizedStrings.GetHtmlEncoded(-1028516975), this.GetPropertyValue(ContactSchema.TtyTddPhoneNumber));
			if (this.isPhoneNumberRendered)
			{
				writer.Write("<tr><td class=\"spcHd\" colspan=2></td></tr>");
			}
		}

		private void RenderPhoneWithoutIcon(TextWriter writer, string label, string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				writer.Write("<tr><td class=\"lbl lp\" nowrap>");
				writer.Write(label);
				writer.Write("</td><td class=\"txvl rptdpd\">");
				Utilities.HtmlEncode(value, writer);
				writer.Write("</td></tr>");
				this.isPhoneNumberRendered = true;
			}
		}

		private void RenderPhoneWithIcon(TextWriter writer, string label, string value, ThemeFileId themeFileId)
		{
			if (!string.IsNullOrEmpty(value))
			{
				writer.Write("<tr><td class=\"lbl lp\" nowrap>");
				writer.Write(label);
				writer.Write("</td><td class=\"txvl phtdpd\"><img src=\"");
				base.UserContext.RenderThemeFileUrl(writer, themeFileId);
				writer.Write("\" ");
				Utilities.RenderImageAltAttribute(writer, base.UserContext, themeFileId);
				writer.Write(">");
				Utilities.HtmlEncode(value, writer);
				writer.Write("</td></tr>");
				this.isPhoneNumberRendered = true;
			}
		}

		private void RenderEmailAddresses(TextWriter writer)
		{
			foreach (EmailAddressIndex emailAddressIndex in ContactUtilities.EmailAddressIndexesToRead)
			{
				string value = null;
				switch (emailAddressIndex)
				{
				case EmailAddressIndex.Email1:
					value = LocalizedStrings.GetHtmlEncoded(1111077458);
					break;
				case EmailAddressIndex.Email2:
					value = LocalizedStrings.GetHtmlEncoded(1405549740);
					break;
				case EmailAddressIndex.Email3:
					value = LocalizedStrings.GetHtmlEncoded(-160534201);
					break;
				}
				Participant participant = this.contact.EmailAddresses[emailAddressIndex];
				if (!(participant == null))
				{
					string text = null;
					string text2 = null;
					ContactUtilities.GetParticipantEmailAddress(participant, out text, out text2, false);
					if (!string.IsNullOrEmpty(text))
					{
						writer.Write("<tr><td class=\"lbl lp\">");
						writer.Write(value);
						writer.Write("</td><td class=\"txvl\">");
						base.RenderEmail(writer, text2, text, participant.RoutingType, base.IsEmbeddedItem ? null : base.Item.Id.ObjectId, emailAddressIndex);
						writer.Write("</td></tr>");
						this.isEmailRendered = true;
					}
				}
			}
			if (this.isEmailRendered)
			{
				writer.Write("<tr><td class=\"spcHd\" colspan=2></td></tr>");
			}
		}

		private void RenderWebPage(TextWriter writer)
		{
			string propertyValue = this.GetPropertyValue(ContactSchema.IMAddress);
			if (!string.IsNullOrEmpty(propertyValue))
			{
				writer.Write("<tr><td class=\"lbl lp\" nowrap>");
				writer.Write(LocalizedStrings.GetHtmlEncoded(-859851584));
				writer.Write("</td>");
				writer.Write("<td class=\"txvl\">");
				Utilities.HtmlEncode(propertyValue, writer);
				writer.Write("</td></tr>");
				this.isEmailRendered = true;
			}
			propertyValue = this.GetPropertyValue(ContactSchema.BusinessHomePage);
			if (!string.IsNullOrEmpty(propertyValue))
			{
				bool flag = false;
				Uri uri = Utilities.TryParseUri(propertyValue);
				if (null == uri)
				{
					ExTraceGlobals.ContactsTracer.TraceDebug<string>((long)this.GetHashCode(), "Contact web page URL has an invalid format: {0}", propertyValue);
				}
				else
				{
					string scheme = uri.Scheme;
					for (int i = 0; i < OwaSafeHtmlOutboundCallbacks.RedirProtocols.Length; i++)
					{
						if (CultureInfo.InvariantCulture.CompareInfo.Compare(scheme, OwaSafeHtmlOutboundCallbacks.RedirProtocols[i], CompareOptions.IgnoreCase) == 0)
						{
							flag = true;
							break;
						}
					}
				}
				writer.Write("<tr><td class=\"lbl lp\" nowrap>");
				writer.Write(LocalizedStrings.GetHtmlEncoded(-1347605932));
				writer.Write("</td>");
				if (flag)
				{
					writer.Write("<td><a href=\"");
					Utilities.HtmlEncode(Redir.BuildExplicitRedirUrl(base.OwaContext, propertyValue), writer);
					writer.Write("\" target=\"_blank\" class=\"peer\">");
					Utilities.HtmlEncode(propertyValue, writer);
					writer.Write("</a></td></tr>");
				}
				else
				{
					writer.Write("<td class=\"txvl\">");
					Utilities.HtmlEncode(propertyValue, writer);
					writer.Write("</td></tr>");
					writer.Write("<tr><td colspan=2 class=\"nolnk msgpd\">");
					writer.Write(LocalizedStrings.GetHtmlEncoded(-1891509726));
					writer.Write("</td></tr>");
				}
				this.isWebAddressRendered = true;
			}
		}

		private void RenderContactAddress(TextWriter writer)
		{
			PhysicalAddressType physicalAddressType = PhysicalAddressType.Business;
			PhysicalAddressType[] array = new PhysicalAddressType[4];
			array[0] = PhysicalAddressType.Business;
			array[1] = PhysicalAddressType.Home;
			array[2] = PhysicalAddressType.Other;
			PhysicalAddressType[] array2 = array;
			object obj = base.Item.TryGetProperty(ContactSchema.PostalAddressId);
			if (obj is int)
			{
				physicalAddressType = (PhysicalAddressType)obj;
			}
			if (physicalAddressType == PhysicalAddressType.Business || physicalAddressType == PhysicalAddressType.Home || physicalAddressType == PhysicalAddressType.Other)
			{
				this.RenderAddress(writer, physicalAddressType, true);
			}
			for (int i = 0; i < array2.Length; i++)
			{
				if (array2[i] != PhysicalAddressType.None && array2[i] != physicalAddressType)
				{
					this.RenderAddress(writer, array2[i], false);
				}
			}
		}

		private void RenderAddress(TextWriter writer, PhysicalAddressType type, bool isMailingAddress)
		{
			IDictionary<AddressFormatTable.AddressPart, AddressComponent> addressInfo = ContactUtilities.GetAddressInfo(base.Item, type);
			if (addressInfo.Count == 0)
			{
				return;
			}
			this.isAddressRendered = true;
			string value = string.Empty;
			switch (type)
			{
			case PhysicalAddressType.Home:
				value = LocalizedStrings.GetHtmlEncoded(1414246315);
				break;
			case PhysicalAddressType.Business:
				value = LocalizedStrings.GetHtmlEncoded(-765825260);
				break;
			case PhysicalAddressType.Other:
				value = LocalizedStrings.GetHtmlEncoded(-582599340);
				break;
			}
			if (isMailingAddress)
			{
				writer.Write("<tr><td colspan=2 class=\"hd lp\">");
				writer.Write(value);
				writer.Write(" <span class=\"brkt\">(");
				writer.Write(LocalizedStrings.GetHtmlEncoded(1912536019));
				writer.Write(")</span></td></tr>");
			}
			else
			{
				writer.Write("<tr><td colspan=2 class=\"hd lp\">");
				writer.Write(value);
				writer.Write("</td></tr>");
			}
			foreach (KeyValuePair<AddressFormatTable.AddressPart, AddressComponent> keyValuePair in addressInfo)
			{
				writer.Write("<tr><td class=\"lbl lp\">");
				writer.Write(keyValuePair.Value.Label);
				writer.Write("</td><td class=\"txvl\">");
				Utilities.HtmlEncode(keyValuePair.Value.Value, writer);
				writer.Write("</td></tr>");
			}
			writer.Write("<tr><td colspan=2 class=\"lbl lp\"><a href=\"");
			writer.Write("<tr><td class=\"spcOP\" colspan=2></td></tr>");
		}

		protected void RenderProfileBucket(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			string propertyValue = this.GetPropertyValue(ContactSchema.OfficeLocation);
			string propertyValue2 = this.GetPropertyValue(ContactSchema.Manager);
			string propertyValue3 = this.GetPropertyValue(ContactSchema.AssistantName);
			writer.Write("<table cellpadding=0 cellspacing=0 class=\"pAddr\">");
			if (!string.IsNullOrEmpty(this.htmlEncodedDepartment) || !string.IsNullOrEmpty(this.htmlEncodedTitle) || !string.IsNullOrEmpty(this.htmlEncodedCompany) || !string.IsNullOrEmpty(propertyValue) || !string.IsNullOrEmpty(propertyValue2) || !string.IsNullOrEmpty(propertyValue3))
			{
				writer.Write("<tr><td colspan=2 class=\"hd\">{0}</td></tr>", LocalizedStrings.GetHtmlEncoded(1346342343));
				if (!string.IsNullOrEmpty(this.htmlEncodedTitle))
				{
					writer.Write("<tr><td class=\"lbl\">");
					writer.Write(LocalizedStrings.GetHtmlEncoded(587115635));
					writer.Write("</td><td class=\"txvl\">");
					writer.Write(this.htmlEncodedTitle);
					writer.Write("</td></tr>");
				}
				if (!string.IsNullOrEmpty(this.htmlEncodedDepartment))
				{
					writer.Write("<tr><td class=\"lbl\">");
					writer.Write(LocalizedStrings.GetHtmlEncoded(1855823700));
					writer.Write("</td><td class=\"txvl\">");
					writer.Write(this.htmlEncodedDepartment);
					writer.Write("</td></tr>");
				}
				if (!string.IsNullOrEmpty(this.htmlEncodedCompany))
				{
					if (this.isPhoneticNamesEnabled)
					{
						string htmlEncodedValue = this.GetHtmlEncodedValue(ContactSchema.YomiCompany);
						if (!string.IsNullOrEmpty(htmlEncodedValue))
						{
							writer.Write("<tr><td class=\"lbl\">");
							writer.Write(LocalizedStrings.GetHtmlEncoded(1805298069));
							writer.Write("</td><td class=\"txvl\">");
							writer.Write(htmlEncodedValue);
							writer.Write("</td></tr>");
						}
					}
					writer.Write("<tr><td class=\"lbl\">");
					writer.Write(LocalizedStrings.GetHtmlEncoded(642177943));
					writer.Write("</td><td class=\"txvl\">");
					writer.Write(this.htmlEncodedCompany);
					writer.Write("</td></tr>");
				}
				if (!string.IsNullOrEmpty(propertyValue))
				{
					writer.Write("<tr><td class=\"lbl\">");
					writer.Write(LocalizedStrings.GetHtmlEncoded(275231482));
					writer.Write("</td><td class=\"txvl\">");
					Utilities.HtmlEncode(propertyValue, writer);
					writer.Write("</td></tr>");
				}
				ReadContact.RenderADPerson(base.UserContext, LocalizedStrings.GetHtmlEncoded(-128712621), propertyValue2, writer);
				ReadContact.RenderADPerson(base.UserContext, LocalizedStrings.GetHtmlEncoded(425094986), propertyValue3, writer);
				writer.Write("<tr><td class=\"spcOP\" colspan=2></td></tr>");
			}
			base.RenderNotes(writer);
			if (base.ShouldRenderAttachmentWell)
			{
				writer.Write("<tr><td class=\"spcOP\" colspan=2></td></tr>");
				base.Response.Write("<tr><td colspan=2 class=\"hd\">");
				writer.Write(LocalizedStrings.GetHtmlEncoded(796893232));
				writer.Write("</td></tr>");
				writer.Write("<tr><td colspan=2 class=\"txvl\">");
				AttachmentWell.RenderAttachmentWell(base.Response.Output, AttachmentWellType.ReadOnly, base.AttachmentWellRenderObjects, base.ItemId.ToBase64String(), base.UserContext, base.AttachmentWellFlags);
				base.Response.Write("</td></tr>");
			}
			writer.Write("</table>");
		}

		private string GetHtmlEncodedValue(PropertyDefinition property)
		{
			string text = this.GetPropertyValue(property);
			if (text.Length > 0)
			{
				text = Utilities.HtmlEncode(text);
			}
			return text;
		}

		private string GetPropertyValue(PropertyDefinition property)
		{
			string text = string.Empty;
			object obj = base.Item.TryGetProperty(property) as string;
			if (obj != null)
			{
				text = (string)obj;
				text = text.Trim();
			}
			return text;
		}

		private Contact contact;

		private string displayName;

		private string htmlEncodedDepartment;

		private string htmlEncodedTitle;

		private string htmlEncodedCompany;

		private bool isPhoneNumberRendered;

		private bool isEmailRendered;

		private bool isAddressRendered;

		private bool isWebAddressRendered;

		private bool isPhoneticNamesEnabled;
	}
}
