using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class ReadContact : OwaForm, IRegistryOnlyForm
	{
		protected override void OnLoad(EventArgs e)
		{
			ExTraceGlobals.ContactsCallTracer.TraceDebug((long)this.GetHashCode(), "ReadContact.OnLoad");
			base.OnLoad(e);
			this.contact = base.Initialize<Contact>(ReadContact.prefetchProperties);
			InfobarMessageBuilder.AddFlag(this.infobar, this.contact, base.UserContext);
			InfobarMessageBuilder.AddNoEditPermissionWarning(this.infobar, this.contact, base.IsPreviewForm);
			this.isPhoneticNamesEnabled = Utilities.IsJapanese;
		}

		protected override void OnUnload(EventArgs e)
		{
			try
			{
				if (this.contact != null)
				{
					this.contact.Dispose();
					this.contact = null;
				}
			}
			finally
			{
				base.OnUnload(e);
			}
		}

		protected void LoadMessageBodyIntoStream()
		{
			BodyConversionUtilities.RenderMeetingPlainTextBody(base.Response.Output, base.Item, base.UserContext, false);
		}

		protected void RenderContactPicture()
		{
			if (base.Item.AttachmentCollection == null)
			{
				return;
			}
			string text = string.Empty;
			string contactPictureAttachmentId = RenderingUtilities.GetContactPictureAttachmentId(base.Item);
			if (!string.IsNullOrEmpty(contactPictureAttachmentId))
			{
				if (base.IsEmbeddedItemInNonSMimeItem)
				{
					text = base.RenderEmbeddedUrl() + Utilities.UrlEncode(contactPictureAttachmentId);
				}
				else
				{
					text = RenderingUtilities.GetContactPictureUrl(Utilities.GetIdAsString(base.Item), contactPictureAttachmentId, string.Empty);
				}
			}
			if (text.Length > 0)
			{
				this.hasPicture = true;
				base.Response.Write("<div id=divPic class=\"spP fltBefore\"><div class=\"fltBefore\" id=frm><a id=lnkPic target=_blank href=\"");
				Utilities.HtmlEncode(text, base.Response.Output);
				base.Response.Write("\"><IMG id=imgPic src=\"");
				Utilities.HtmlEncode(text, base.Response.Output);
				base.Response.Write("\"></a></div></div>");
			}
		}

		protected void RenderFileAs()
		{
			string htmlEncodedValue = this.GetHtmlEncodedValue(ContactBaseSchema.FileAs);
			if (htmlEncodedValue.Length > 0)
			{
				base.Response.Write(htmlEncodedValue);
				return;
			}
			base.Response.Write(LocalizedStrings.GetHtmlEncoded(-1873027801));
		}

		protected void RenderTitleBar()
		{
			if (this.HasPicture)
			{
				base.Response.Write("<div class=\"fltBefore\" id=ttl>");
			}
			else
			{
				base.Response.Write("<div class=\"fltBefore ctHd\" id=ttl>");
			}
			if (this.isPhoneticNamesEnabled)
			{
				string htmlEncodedValue = this.GetHtmlEncodedValue(ContactSchema.YomiLastName);
				string htmlEncodedValue2 = this.GetHtmlEncodedValue(ContactSchema.YomiFirstName);
				if (htmlEncodedValue.Length > 0 || htmlEncodedValue2.Length > 0)
				{
					base.Response.Write("<span id=spnTY class=gryTxt>");
					if (htmlEncodedValue.Length > 0)
					{
						base.Response.Write(htmlEncodedValue);
						base.Response.Write(string.Empty);
					}
					if (htmlEncodedValue2.Length > 0)
					{
						base.Response.Write(htmlEncodedValue2);
					}
					base.Response.Write("</span><br>");
				}
			}
			base.Response.Write("<span id=spnTN class=nm>");
			this.RenderFileAs();
			base.Response.Write("</span>");
			string htmlEncodedValue3 = this.GetHtmlEncodedValue(ContactSchema.Title);
			string htmlEncodedValue4 = this.GetHtmlEncodedValue(ContactSchema.Department);
			if (htmlEncodedValue3.Length > 0 && htmlEncodedValue4.Length > 0)
			{
				base.Response.Write("<br><span id=spnTJ class=gryTxt>");
				base.Response.Write(string.Format("<b>{0}</b>, {1}", htmlEncodedValue3, htmlEncodedValue4));
				base.Response.Write("</span>");
			}
			else if (htmlEncodedValue3.Length > 0)
			{
				base.Response.Write("<br><span id=spnTJ class=gryTxt>");
				base.Response.Write("<b>");
				base.Response.Write(htmlEncodedValue3);
				base.Response.Write("</b></span>");
			}
			else if (htmlEncodedValue4.Length > 0)
			{
				base.Response.Write("<br><span id=spnTJ class=gryTxt>");
				base.Response.Write(htmlEncodedValue4);
				base.Response.Write("</span>");
			}
			string htmlEncodedValue5 = this.GetHtmlEncodedValue(ContactSchema.CompanyName);
			if (htmlEncodedValue5.Length > 0)
			{
				base.Response.Write("<br><span id=spnTCo class=gryTxt>");
				base.Response.Write(htmlEncodedValue5);
				base.Response.Write("</span>");
			}
			base.Response.Write("</div>");
		}

		protected void RenderData()
		{
			if (base.HasCategories)
			{
				this.isSectionStarted = false;
				this.sectionHeader = LocalizedStrings.GetHtmlEncoded(-2125925114);
				this.sectionId = "spnSCat";
				this.RenderSectionStart(false);
				base.Response.Write("<div class=\"\">");
				CategorySwatch.RenderCategories(base.OwaContext, base.SanitizingResponse, base.Item);
				base.Response.Write("</div></div>");
			}
			this.isSectionStarted = false;
			this.sectionHeader = LocalizedStrings.GetHtmlEncoded(447307630);
			this.sectionId = "spnSCt";
			this.isFirstLine = true;
			this.RenderEmailSubsectionData();
			this.RenderPhoneSubsectionData();
			this.RenderPersonalSubsectionData();
			this.isFirstLine = true;
			this.RenderProfileSectionData();
			this.isFirstLine = true;
			this.RenderAddressSectionData();
			this.isFirstLine = true;
			this.RenderSecureMessaging();
			this.isFirstLine = true;
			this.RenderDetailsSectionData();
		}

		private void RenderEmailSubsectionData()
		{
			foreach (EmailAddressIndex emailAddressIndex in ContactUtilities.EmailAddressIndexesToRead)
			{
				string label = null;
				string id = null;
				switch (emailAddressIndex)
				{
				case EmailAddressIndex.Email1:
					label = LocalizedStrings.GetHtmlEncoded(1111077458);
					id = "E1";
					break;
				case EmailAddressIndex.Email2:
					label = LocalizedStrings.GetHtmlEncoded(1405549740);
					id = "E2";
					break;
				case EmailAddressIndex.Email3:
					label = LocalizedStrings.GetHtmlEncoded(-160534201);
					id = "E3";
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
						if (participant.RoutingType != null)
						{
							StringBuilder stringBuilder = new StringBuilder();
							stringBuilder.Append("?ae=Item&t=IPM.Note&a=New&to=");
							stringBuilder.Append(Utilities.UrlEncode(participant.EmailAddress));
							if (!string.IsNullOrEmpty(text2))
							{
								stringBuilder.Append("&nm=");
								stringBuilder.Append(Utilities.UrlEncode(text2));
							}
							if (!string.IsNullOrEmpty(participant.RoutingType))
							{
								stringBuilder.Append("&rt=");
								stringBuilder.Append(Utilities.UrlEncode(participant.RoutingType));
							}
							if (!base.IsEmbeddedItem)
							{
								stringBuilder.Append("&ao=");
								stringBuilder.Append(Utilities.UrlEncode(1.ToString()));
								stringBuilder.Append("&stId=");
								stringBuilder.Append(Utilities.UrlEncode(base.Item.Id.ObjectId.ToBase64String()));
								stringBuilder.Append("&ei=");
								StringBuilder stringBuilder2 = stringBuilder;
								int num = (int)emailAddressIndex;
								stringBuilder2.Append(num.ToString());
							}
							this.RenderSectionLabelValueAndUrl(label, id, string.IsNullOrEmpty(text2) ? text : text2, stringBuilder.ToString(), false);
						}
						else
						{
							this.RenderSectionLabelValueAndUrl(label, id, string.IsNullOrEmpty(text2) ? text : text2, null, false);
						}
					}
				}
			}
		}

		private void RenderPhoneSubsectionData()
		{
			this.isSubsectionStarted = false;
			this.RenderLabelValue(LocalizedStrings.GetHtmlEncoded(-765825260), "BP", ContactSchema.BusinessPhoneNumber);
			this.RenderLabelValue(LocalizedStrings.GetHtmlEncoded(1414246315), "HP", ContactSchema.HomePhone);
			this.RenderLabelValue(LocalizedStrings.GetHtmlEncoded(1158653436), "MP", ContactSchema.MobilePhone);
			this.RenderLabelValue(LocalizedStrings.GetHtmlEncoded(-11305699), "FP", ContactSchema.WorkFax);
			this.RenderLabelValue(LocalizedStrings.GetHtmlEncoded(-1816252206), "AP", ContactSchema.AssistantPhoneNumber);
			this.RenderLabelValue(LocalizedStrings.GetHtmlEncoded(-534169914), "BP2", ContactSchema.BusinessPhoneNumber2);
			this.RenderLabelValue(LocalizedStrings.GetHtmlEncoded(-646524091), "CbP", ContactSchema.CallbackPhone);
			this.RenderLabelValue(LocalizedStrings.GetHtmlEncoded(159631176), "CP", ContactSchema.CarPhone);
			this.RenderLabelValue(LocalizedStrings.GetHtmlEncoded(-1918812500), "OMP", ContactSchema.OrganizationMainPhone);
			this.RenderLabelValue(LocalizedStrings.GetHtmlEncoded(1930214269), "HP2", ContactSchema.HomePhone2);
			this.RenderLabelValue(LocalizedStrings.GetHtmlEncoded(1180016964), "HFP", ContactSchema.HomeFax);
			this.RenderLabelValue(LocalizedStrings.GetHtmlEncoded(57098496), "ISDN", ContactSchema.InternationalIsdnNumber);
			this.RenderLabelValue(LocalizedStrings.GetHtmlEncoded(-582599340), "OP", ContactSchema.OtherTelephone);
			this.RenderLabelValue(LocalizedStrings.GetHtmlEncoded(-679895069), "OFP", ContactSchema.OtherFax);
			this.RenderLabelValue(LocalizedStrings.GetHtmlEncoded(410048973), "Pg", ContactSchema.Pager);
			this.RenderLabelValue(LocalizedStrings.GetHtmlEncoded(1442239260), "PP", ContactSchema.PrimaryTelephoneNumber);
			this.RenderLabelValue(LocalizedStrings.GetHtmlEncoded(-166006211), "RP", ContactSchema.RadioPhone);
			this.RenderLabelValue(LocalizedStrings.GetHtmlEncoded(1096044911), "TP", ContactSchema.TelexNumber);
			this.RenderLabelValue(LocalizedStrings.GetHtmlEncoded(-1028516975), "Tty", ContactSchema.TtyTddPhoneNumber);
		}

		private void RenderPersonalSubsectionData()
		{
			this.isSubsectionStarted = false;
			string propertyValue = this.GetPropertyValue(ContactSchema.IMAddress);
			if (propertyValue.Length > 0)
			{
				this.RenderSectionLabelValueAndUrl(LocalizedStrings.GetHtmlEncoded(-859851584), "IM", propertyValue, "?ae=Item&t=IPM.Note&a=New&to=" + Utilities.UrlEncode(propertyValue), true);
			}
			propertyValue = this.GetPropertyValue(ContactSchema.BusinessHomePage);
			if (propertyValue.Length > 0)
			{
				bool flag = false;
				Uri uri;
				if (null == (uri = Utilities.TryParseUri(propertyValue)))
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
				if (flag)
				{
					this.RenderSectionLabelValueAndUrl(LocalizedStrings.GetHtmlEncoded(521829799), "Wb", propertyValue, Redir.BuildExplicitRedirUrl(base.OwaContext, propertyValue), true, false);
					return;
				}
				this.RenderSectionLabelValueAndUrl(LocalizedStrings.GetHtmlEncoded(521829799), "Wb", propertyValue, null, true, false, LocalizedStrings.GetNonEncoded(-1891509726));
			}
		}

		private void RenderProfileSectionData()
		{
			this.isSectionStarted = false;
			this.sectionHeader = LocalizedStrings.GetHtmlEncoded(1346342343);
			this.sectionId = "spnSPf";
			this.RenderLabelValue(LocalizedStrings.GetHtmlEncoded(587115635), "PT", ContactSchema.Title);
			this.RenderLabelValue(LocalizedStrings.GetHtmlEncoded(1855823700), "PD", ContactSchema.Department);
			if (this.isPhoneticNamesEnabled)
			{
				this.RenderLabelValue(LocalizedStrings.GetHtmlEncoded(1805298069), "PYc", ContactSchema.YomiCompany);
			}
			this.RenderLabelValue(LocalizedStrings.GetHtmlEncoded(642177943), "PC", ContactSchema.CompanyName);
			this.RenderADPerson(LocalizedStrings.GetHtmlEncoded(-128712621), "PM", ContactSchema.Manager);
			this.RenderADPerson(LocalizedStrings.GetHtmlEncoded(425094986), "PA", ContactSchema.AssistantName);
			this.RenderLabelValue(LocalizedStrings.GetHtmlEncoded(275231482), "PO", ContactSchema.OfficeLocation);
		}

		private void RenderADPerson(string label, string id, PropertyDefinition property)
		{
			string propertyValue = this.GetPropertyValue(property);
			if (propertyValue.Length == 0)
			{
				return;
			}
			string url = null;
			string text = null;
			IRecipientSession recipientSession = Utilities.CreateADRecipientSession(ConsistencyMode.IgnoreInvalid, base.UserContext);
			ADRecipient[] array = recipientSession.FindByANR(propertyValue, 2, null);
			if (array != null && array.Length == 1 && array[0].RecipientType == RecipientType.UserMailbox)
			{
				text = Convert.ToBase64String(array[0].Id.ObjectGuid.ToByteArray());
			}
			if (text != null)
			{
				url = "?ae=Item&t=AD.RecipientType.User&a=Open&id=" + Utilities.UrlEncode(text);
			}
			this.RenderSectionLabelValueAndUrl(label, id, propertyValue, url, true, false);
		}

		private void RenderAddressSectionData()
		{
			this.isSectionStarted = false;
			this.sectionHeader = LocalizedStrings.GetHtmlEncoded(-1159205642);
			this.sectionId = "spnSAd";
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
				this.isFirstLine = true;
				this.RenderAddress(physicalAddressType, true);
			}
			for (int i = 0; i < array2.Length; i++)
			{
				if (array2[i] != PhysicalAddressType.None && array2[i] != physicalAddressType)
				{
					this.RenderAddress(array2[i], false);
				}
			}
		}

		private void RenderAddress(PhysicalAddressType type, bool isSelected)
		{
			IDictionary<AddressFormatTable.AddressPart, AddressComponent> addressInfo = ContactUtilities.GetAddressInfo(base.Item, type);
			if (addressInfo.Count == 0)
			{
				return;
			}
			this.isSubsectionStarted = false;
			this.RenderSectionStart(true);
			string s = string.Empty;
			string text = string.Empty;
			base.Response.Write("<div ");
			switch (type)
			{
			case PhysicalAddressType.Home:
				s = LocalizedStrings.GetHtmlEncoded(1414246315);
				text = "AH";
				break;
			case PhysicalAddressType.Business:
				s = LocalizedStrings.GetHtmlEncoded(-765825260);
				text = "AB";
				break;
			case PhysicalAddressType.Other:
				s = LocalizedStrings.GetHtmlEncoded(-582599340);
				text = "AO";
				break;
			}
			base.Response.Write(" id=td");
			base.Response.Write(text);
			base.Response.Write(" class=\"bLn");
			if (!this.isFirstLine)
			{
				base.Response.Write(" indent");
			}
			base.Response.Write("\">");
			base.Response.Write(s);
			base.Response.Write("</div><div");
			base.Response.Write(" id=td");
			base.Response.Write(text);
			base.Response.Write("S class=\"fld bLn\">");
			if (isSelected)
			{
				base.Response.Write("<span id=spnSel class=gryTxt>");
				base.Response.Write(base.UserContext.IsRtl ? string.Empty : " ");
				base.Response.Write(base.UserContext.DirectionMark);
				base.Response.Write("(");
				base.Response.Write(LocalizedStrings.GetHtmlEncoded(1912536019));
				base.Response.Write(")");
				base.Response.Write(base.UserContext.DirectionMark);
				base.Response.Write("</span>");
			}
			base.Response.Write("</div></div>");
			this.isFirstLine = true;
			foreach (KeyValuePair<AddressFormatTable.AddressPart, AddressComponent> keyValuePair in addressInfo)
			{
				string id = text + ((int)keyValuePair.Key).ToString();
				if (this.isFirstLine)
				{
					this.isFirstLine = false;
					base.Response.Write("<div class=\"clear row noindent\">");
				}
				else
				{
					base.Response.Write("<div class=\"clear row\">");
				}
				this.RenderLabelValueAndUrl(keyValuePair.Value.Label, id, keyValuePair.Value.Value, null, false, null);
			}
		}

		private void RenderDetailsSectionData()
		{
			this.isSectionStarted = false;
			this.sectionHeader = LocalizedStrings.GetHtmlEncoded(-728684336);
			this.sectionId = "tdSDtl";
			if (this.ShouldRenderAttachmentWell)
			{
				this.RenderSectionStart(false);
				base.Response.Write("<div class=\"lbl noindent\">");
				base.Response.Write(LocalizedStrings.GetHtmlEncoded(796893232));
				base.Response.Write("</div><div class=\"clear indent\">");
				AttachmentWell.RenderAttachmentWell(base.Response.Output, AttachmentWellType.ReadOnly, this.AttachmentWellRenderObjects, base.UserContext);
				base.Response.Write("</div>");
			}
			if (this.ShouldRenderNotes)
			{
				if (!this.ShouldRenderAttachmentWell)
				{
					this.RenderSectionStart(false);
					base.Response.Write("<div class=\"lbl noindent\">");
				}
				else
				{
					base.Response.Write("<div class=\"clear row\">");
					base.Response.Write("<div class=\"lbl\">");
				}
				base.Response.Write(LocalizedStrings.GetHtmlEncoded(1601836855));
				base.Response.Write("</div><textarea id=\"notes\" class=\"cntBdy indent\" readOnly=\"true\">");
				this.LoadMessageBodyIntoStream();
				base.Response.Write("</textarea></div></div>");
			}
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

		private void RenderLabelValue(string label, string id, PropertyDefinition property)
		{
			this.RenderLabelValue(label, id, property, false);
		}

		private void RenderLabelValue(string label, string id, PropertyDefinition property, bool alwaysLTR)
		{
			string propertyValue = this.GetPropertyValue(property);
			if (string.IsNullOrEmpty(propertyValue))
			{
				return;
			}
			this.RenderSectionLabelValueAndUrl(label, id, propertyValue, null, true, alwaysLTR);
		}

		private void RenderSectionLabelValueAndUrl(string label, string id, string value, string url, bool hasSubsection)
		{
			this.RenderSectionStart(hasSubsection);
			this.RenderLabelValueAndUrl(label, id, value, url, true);
		}

		private void RenderSectionLabelValueAndUrl(string label, string id, string value, string url, bool hasSubsection, bool isFormLink)
		{
			this.RenderSectionStart(hasSubsection);
			this.RenderLabelValueAndUrl(label, id, value, url, isFormLink);
		}

		private void RenderSectionLabelValueAndUrl(string label, string id, string value, string url, bool hasSubsection, bool isFormLink, string error)
		{
			this.RenderSectionStart(hasSubsection);
			this.RenderLabelValueAndUrl(label, id, value, url, isFormLink, error);
		}

		private void RenderSectionStart(bool hasSubsection)
		{
			if (!this.isSectionStarted)
			{
				if (this.sectionCount == 0)
				{
					base.Response.Write("<div class=\"row\">");
				}
				else
				{
					base.Response.Write("<div class=\"row2sp\">");
				}
				base.Response.Write("<div class=\"fltBefore secCol\"><span id=");
				base.Response.Write(this.sectionId);
				base.Response.Write(" class=spS>");
				base.Response.Write(this.sectionHeader);
				base.Response.Write("</span></div>");
				this.isSectionStarted = true;
				this.isSubsectionStarted = true;
				this.sectionCount++;
				return;
			}
			if (!this.isSubsectionStarted && hasSubsection)
			{
				base.Response.Write("<div class=\"row1sp\">");
				this.isSubsectionStarted = true;
				return;
			}
			base.Response.Write("<div class=\"row\">");
		}

		private void RenderLabelValueAndUrl(string label, string id, string value, string url, bool isFormLink)
		{
			this.RenderLabelValueAndUrl(label, id, value, url, isFormLink, null);
		}

		private void RenderLabelValueAndUrl(string label, string id, string value, string url, bool isFormLink, string error)
		{
			if (this.isFirstLine)
			{
				base.Response.Write("<div class=\"lbl noindent\">");
				this.isFirstLine = false;
			}
			else
			{
				base.Response.Write("<div class=\"lbl\">");
			}
			base.Response.Write(label);
			base.Response.Write("</div><div class=\"fld\" id=td");
			base.Response.Write(id);
			base.Response.Write(">");
			if (url != null)
			{
				base.Response.Write("<a id=lnk");
				base.Response.Write(id);
				base.Response.Write(" class=lnk");
				if (isFormLink)
				{
					base.Response.Write(" ");
					Utilities.RenderScriptHandler(base.Response.Output, "onclick", "openItm(\"" + Utilities.JavascriptEncode(url) + "\");");
					base.Response.Write(">");
				}
				else
				{
					base.Response.Write(" target=_blank href=\"");
					base.Response.Write(Utilities.HtmlEncode(url));
					base.Response.Write("\">");
				}
			}
			Utilities.HtmlEncode(value, base.Response.Output);
			if (url != null)
			{
				base.Response.Write("</a>");
			}
			if (error != null)
			{
				base.Response.Write("<div class=\"clear cntErr\">");
				Utilities.HtmlEncode(error, base.Response.Output);
				base.Response.Write("</div>");
			}
			base.Response.Write("</div></div>");
		}

		protected void CreateAttachmentHelpers()
		{
			this.attachmentWellRenderObjects = AttachmentWell.GetAttachmentInformation(base.Item, base.AttachmentLinks, base.UserContext.IsPublicLogon, base.IsEmbeddedItem);
			foreach (object obj in this.attachmentWellRenderObjects)
			{
				AttachmentWellInfo attachmentWellInfo = (AttachmentWellInfo)obj;
				if (string.CompareOrdinal("ContactPicture.jpg", attachmentWellInfo.FileName) == 0)
				{
					this.attachmentWellRenderObjects.Remove(attachmentWellInfo);
					break;
				}
			}
			this.shouldRenderAttachmentWell = RenderingUtilities.AddAttachmentInfobarMessages(base.Item, base.IsEmbeddedItem, false, this.infobar, this.attachmentWellRenderObjects);
		}

		protected void RenderOwaPlainTextStyle()
		{
			OwaPlainTextStyle.WriteLocalizedStyleIntoHeadForPlainTextBody(base.Item, base.Response.Output, "textarea#notes ");
		}

		protected void RenderToolbar()
		{
			if (!base.IsPreviewForm)
			{
				ReadContactToolbar readContactToolbar = new ReadContactToolbar(this.contact);
				readContactToolbar.Render(base.Response.Output);
			}
		}

		protected ArrayList AttachmentWellRenderObjects
		{
			get
			{
				return this.attachmentWellRenderObjects;
			}
		}

		protected bool ShouldRenderAttachmentWell
		{
			get
			{
				return this.shouldRenderAttachmentWell;
			}
		}

		protected bool ShouldRenderNotes
		{
			get
			{
				return base.Item != null && base.Item.Body != null && base.Item.Body.Size > 0L;
			}
		}

		protected Infobar Infobar
		{
			get
			{
				return this.infobar;
			}
		}

		protected bool HasPicture
		{
			get
			{
				return this.hasPicture;
			}
		}

		protected static int StoreObjectTypeContact
		{
			get
			{
				return 17;
			}
		}

		private void RenderSecureMessaging()
		{
			if (!Utilities.IsClientSMimeControlUsable(base.ClientSMimeControlStatus))
			{
				return;
			}
			base.Response.Write("<div class=\"row2sp\"><div class=\"secCol\"><span class=spS>");
			base.Response.Write(LocalizedStrings.GetHtmlEncoded(-2096722623));
			base.Response.Write("</span></div>");
			base.Response.Write("<div class=\"lbl noindent\" id=\"tdSM\">");
			bool flag = Utilities.FindBestCertificate(ItemUtility.GetProperty<byte[][]>(base.Item, ContactSchema.UserX509Certificates, null), null, true, false) != null;
			base.Response.Write(LocalizedStrings.GetHtmlEncoded(flag ? -1588000202 : -629647425));
			base.Response.Write("</div></div>");
		}

		private const string NewMessageUrl = "?ae=Item&t=IPM.Note&a=New&to=";

		private static readonly PropertyDefinition[] prefetchProperties = new PropertyDefinition[]
		{
			BodySchema.Codepage,
			BodySchema.InternetCpid,
			ContactSchema.PostalAddressId,
			ContactSchema.HomeCity,
			ContactSchema.HomeCountry,
			ContactSchema.HomePostalCode,
			ContactSchema.HomeState,
			ContactSchema.HomeStreet,
			ContactSchema.OtherCity,
			ContactSchema.OtherCountry,
			ContactSchema.OtherPostalCode,
			ContactSchema.OtherState,
			ContactSchema.OtherStreet,
			ContactSchema.WorkAddressCity,
			ContactSchema.WorkAddressCountry,
			ContactSchema.WorkAddressPostalCode,
			ContactSchema.WorkAddressState,
			ContactSchema.WorkAddressStreet,
			ContactSchema.FileAsId,
			ContactSchema.YomiCompany,
			ContactSchema.BusinessHomePage,
			ContactSchema.BusinessPhoneNumber,
			ContactSchema.HomePhone,
			ContactSchema.MobilePhone,
			ContactSchema.WorkFax,
			ContactSchema.AssistantPhoneNumber,
			ContactSchema.BusinessPhoneNumber2,
			ContactSchema.CallbackPhone,
			ContactSchema.CarPhone,
			ContactSchema.OrganizationMainPhone,
			ContactSchema.HomePhone2,
			ContactSchema.HomeFax,
			ContactSchema.InternationalIsdnNumber,
			ContactSchema.OtherTelephone,
			ContactSchema.OtherFax,
			ContactSchema.Pager,
			ContactSchema.PrimaryTelephoneNumber,
			ContactSchema.RadioPhone,
			ContactSchema.TtyTddPhoneNumber,
			ContactSchema.TelexNumber,
			ContactSchema.BusinessHomePage,
			ContactSchema.Manager,
			ContactSchema.OfficeLocation,
			ContactSchema.Title,
			ItemSchema.FlagStatus,
			ItemSchema.FlagCompleteTime,
			MessageItemSchema.ReplyTime,
			ItemSchema.UtcDueDate,
			ItemSchema.UtcStartDate,
			ItemSchema.ReminderDueBy,
			ItemSchema.ReminderIsSet,
			StoreObjectSchema.EffectiveRights,
			AttachmentSchema.IsContactPhoto,
			ContactSchema.UserX509Certificates
		};

		private Contact contact;

		private int sectionCount;

		private bool isSectionStarted;

		private bool isSubsectionStarted;

		private bool isFirstLine;

		private string sectionHeader = LocalizedStrings.GetHtmlEncoded(447307630);

		private string sectionId = "spnSCt";

		private Infobar infobar = new Infobar();

		private ArrayList attachmentWellRenderObjects;

		private bool shouldRenderAttachmentWell;

		private bool hasPicture;

		private bool isPhoneticNamesEnabled;
	}
}
