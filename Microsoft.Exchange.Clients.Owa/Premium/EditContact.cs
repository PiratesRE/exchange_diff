using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class EditContact : EditItemForm, IRegistryOnlyForm
	{
		private static PropertyDefinition[] CreatePrefetchProperties()
		{
			PropertyDefinition[] array = new PropertyDefinition[ContactUtilities.PrefetchProperties.Length + EditContact.additionalPrefetchProperties.Length];
			ContactUtilities.PrefetchProperties.CopyTo(array, 0);
			EditContact.additionalPrefetchProperties.CopyTo(array, ContactUtilities.PrefetchProperties.Length);
			return array;
		}

		protected override void OnLoad(EventArgs e)
		{
			ExTraceGlobals.ContactsCallTracer.TraceDebug((long)this.GetHashCode(), "EditContact.OnLoad");
			base.OnLoad(e);
			this.contact = base.Initialize<Contact>(false, EditContact.prefetchProperties);
			this.firstPhoneProperty = null;
			this.firstEmailProperty = null;
			if (this.contact != null)
			{
				for (int i = 0; i < ContactUtilities.PhoneNumberProperties.Length; i++)
				{
					ContactPropertyInfo contactPropertyInfo = ContactUtilities.PhoneNumberProperties[i];
					string propertyValue = this.GetPropertyValue(contactPropertyInfo.PropertyDefinition);
					if (this.firstPhoneProperty == null && !string.IsNullOrEmpty(propertyValue))
					{
						this.firstPhoneProperty = contactPropertyInfo;
						break;
					}
				}
				for (int j = 0; j < ContactUtilities.EmailAddressProperties.Length; j++)
				{
					ContactPropertyInfo contactPropertyInfo2 = ContactUtilities.EmailAddressProperties[j];
					string value = null;
					string value2 = null;
					this.GetEmailAddressValue(contactPropertyInfo2, out value, out value2);
					this.GetPropertyValue(contactPropertyInfo2.PropertyDefinition);
					if (this.firstEmailProperty == null && (!string.IsNullOrEmpty(value) || !string.IsNullOrEmpty(value2)))
					{
						this.firstEmailProperty = contactPropertyInfo2;
						break;
					}
				}
				InfobarMessageBuilder.AddFlag(this.infobar, this.contact, base.UserContext);
			}
			if (this.firstPhoneProperty == null)
			{
				this.firstPhoneProperty = ContactUtilities.AssistantPhoneNumber;
			}
			if (this.firstEmailProperty == null)
			{
				this.firstEmailProperty = ContactUtilities.Email1EmailAddress;
			}
			this.isPhoneticNamesEnabled = Utilities.IsJapanese;
			this.toolbar = new EditContactToolbar(this.contact);
			this.toolbar.ToolbarType = ToolbarType.View;
		}

		private void GetEmailAddressValue(ContactPropertyInfo propertyInfo, out string displayName, out string email)
		{
			displayName = string.Empty;
			email = string.Empty;
			if (base.Item == null)
			{
				return;
			}
			EmailAddressIndex emailPropertyIndex = ContactUtilities.GetEmailPropertyIndex(propertyInfo);
			ContactUtilities.GetContactEmailAddress(this.contact, emailPropertyIndex, out email, out displayName);
		}

		protected void RenderTextProperty(ContactPropertyInfo propertyInfo)
		{
			if (propertyInfo == null)
			{
				throw new ArgumentNullException("propertyInfo");
			}
			base.Response.Write("<div class=\"cntLabel\">");
			base.Response.Write(LocalizedStrings.GetHtmlEncoded(propertyInfo.Label));
			base.Response.Write("</div><div class=\"cntField\"><input type=\"text\" id=\"");
			base.Response.Write(propertyInfo.Id);
			base.Response.Write("\" class=\"cntWell\" maxlength=\"255\" value=\"");
			if (base.Item != null)
			{
				string propertyValue = this.GetPropertyValue(propertyInfo.PropertyDefinition);
				Utilities.HtmlEncode(propertyValue, base.Response.Output);
			}
			base.Response.Write("\"></div><div class=\"clear\" />");
		}

		protected void RenderMultiLineTextProperty(ContactPropertyInfo propertyInfo)
		{
			if (propertyInfo == null)
			{
				throw new ArgumentNullException("propertyInfo");
			}
			base.Response.Write("<div class=\"cntLabelMulti\">");
			base.Response.Write(LocalizedStrings.GetHtmlEncoded(propertyInfo.Label));
			base.Response.Write("</div><div class=\"cntMulti\"><textarea id=\"");
			base.Response.Write(propertyInfo.Id);
			base.Response.Write("\" class=\"cntWellMulti\">");
			if (base.Item != null)
			{
				string propertyValue = this.GetPropertyValue(propertyInfo.PropertyDefinition);
				Utilities.HtmlEncode(propertyValue, base.Response.Output);
			}
			base.Response.Write("</textarea></div>");
		}

		protected void RenderSeparator()
		{
			base.Response.Write("<div class=\"cntSep\"></div>");
		}

		protected void RenderSeparator(string title)
		{
			base.Response.Write("<div class=\"cntInnerSectTxt\">");
			base.Response.Write(title);
			base.Response.Write("</div>");
		}

		protected void RenderFileAs()
		{
			FileAsDropDownList dropDown = new FileAsDropDownList("divFA", ContactUtilities.GetFileAs(base.Item));
			this.RenderLabelAndDropDown(LocalizedStrings.GetHtmlEncoded(ContactUtilities.FileAsId.Label), dropDown);
		}

		private void RenderLabelAndDropDown(string label, DropDownList dropDown)
		{
			base.Response.Write("<div class=\"cntLabel\">");
			base.Response.Write(label);
			base.Response.Write("</div><div class=\"cntCmbField\">");
			dropDown.Render(base.Response.Output);
			base.Response.Write("</div>");
		}

		private string GetPropertyValue(PropertyDefinition property)
		{
			string result = string.Empty;
			if (base.Item == null)
			{
				return result;
			}
			string text = base.Item.TryGetProperty(property) as string;
			if (text != null)
			{
				result = text;
			}
			return result;
		}

		protected void RenderPhoneProperties()
		{
			int num = ContactUtilities.PhoneNumberProperties.Length;
			DropDownListItem[] array = new DropDownListItem[num];
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < num; i++)
			{
				ContactPropertyInfo contactPropertyInfo = ContactUtilities.PhoneNumberProperties[i];
				string propertyValue = this.GetPropertyValue(contactPropertyInfo.PropertyDefinition);
				stringBuilder.Append("<input id=\"");
				stringBuilder.Append(contactPropertyInfo.Id);
				stringBuilder.Append("\"");
				array[i] = new DropDownListItem(contactPropertyInfo.Id, contactPropertyInfo.Label, "drp_" + contactPropertyInfo.Id, !string.IsNullOrEmpty(propertyValue.Trim()));
				if (contactPropertyInfo != this.firstPhoneProperty)
				{
					stringBuilder.Append(" style=\"display:none\"");
				}
				stringBuilder.Append(" maxlength=\"256\" class=\"cntWell\" type=\"text\" value=\"");
				stringBuilder.Append(Utilities.HtmlEncode(propertyValue));
				stringBuilder.Append("\">");
			}
			base.Response.Write("<div class=\"cntLabelCombo\">");
			DropDownList dropDownList = new DropDownList("divPH", this.firstPhoneProperty.Id, array);
			dropDownList.Render(base.Response.Output);
			base.Response.Write("</div><div class=\"cntField\">");
			base.Response.Write(stringBuilder);
			base.Response.Write("</div>");
		}

		protected void RenderEmailProperties()
		{
			EmailDropDownList emailDropDownList = new EmailDropDownList("divEM", this.firstEmailProperty);
			base.Response.Write("<div class=\"cntLabelCombo\">");
			emailDropDownList.Render(base.Response.Output);
			base.Response.Write("</div><div class=\"cntField\">");
			string[] array = new string[ContactUtilities.EmailAddressProperties.Length];
			for (int i = 0; i < ContactUtilities.EmailAddressProperties.Length; i++)
			{
				string text = null;
				string s = null;
				ContactPropertyInfo contactPropertyInfo = ContactUtilities.EmailAddressProperties[i];
				this.GetEmailAddressValue(contactPropertyInfo, out text, out s);
				array[i] = text;
				base.Response.Write("<input id=\"");
				base.Response.Write(contactPropertyInfo.Id);
				base.Response.Write("\"");
				if (contactPropertyInfo != this.firstEmailProperty)
				{
					base.Response.Write(" style=\"display:none\"");
				}
				ContactPropertyInfo emailDisplayAsProperty = ContactUtilities.GetEmailDisplayAsProperty(contactPropertyInfo);
				base.Response.Write(" maxlength=\"256\" class=\"cntWell\" type=\"text\" value=\"");
				Utilities.HtmlEncode(s, base.Response.Output);
				base.Response.Write("\" _da=\"");
				base.Response.Write(emailDisplayAsProperty.Id);
				base.Response.Write("\">");
			}
			base.Response.Write("</div>");
			base.Response.Write("<div class=\"cntLabel\">");
			base.Response.Write(LocalizedStrings.GetHtmlEncoded(1019177604));
			base.Response.Write("</div><div class=\"cntField\">");
			for (int j = 0; j < ContactUtilities.EmailAddressProperties.Length; j++)
			{
				ContactPropertyInfo contactPropertyInfo2 = ContactUtilities.EmailAddressProperties[j];
				string s2 = array[j];
				base.Response.Write("<input id=\"");
				base.Response.Write(ContactUtilities.GetEmailDisplayAsProperty(contactPropertyInfo2).Id);
				base.Response.Write("\"");
				if (contactPropertyInfo2 != this.firstEmailProperty)
				{
					base.Response.Write(" style=\"display:none\"");
				}
				base.Response.Write(" maxlength=\"256\" class=\"cntWell\" type=\"text\" value=\"");
				Utilities.HtmlEncode(s2, base.Response.Output);
				base.Response.Write("\">");
			}
			base.Response.Write("</div>");
		}

		protected void RenderMailingAddress()
		{
			MailingAddressDropDownList dropDown = new MailingAddressDropDownList("divMA", (PhysicalAddressType)this.GetMailingAddress());
			this.RenderLabelAndDropDown(LocalizedStrings.GetHtmlEncoded(ContactUtilities.PostalAddressId.Label), dropDown);
		}

		protected int GetMailingAddress()
		{
			PhysicalAddressType result = PhysicalAddressType.None;
			if (base.Item != null)
			{
				object obj = base.Item.TryGetProperty(ContactSchema.PostalAddressId);
				if (obj is int)
				{
					result = (PhysicalAddressType)obj;
				}
			}
			return (int)result;
		}

		private void RenderAddress(List<ContactPropertyInfo> addressInfo, PropertyDefinition street)
		{
			foreach (ContactPropertyInfo contactPropertyInfo in addressInfo)
			{
				if (contactPropertyInfo.PropertyDefinition == street)
				{
					this.RenderMultiLineTextProperty(contactPropertyInfo);
				}
				else
				{
					this.RenderTextProperty(contactPropertyInfo);
				}
			}
		}

		protected void RenderAddresses()
		{
			List<ContactPropertyInfo> addressInfo = ContactUtilities.GetAddressInfo(PhysicalAddressType.Business);
			this.RenderSeparator(LocalizedStrings.GetHtmlEncoded(-765825260));
			this.RenderAddress(addressInfo, ContactSchema.WorkAddressStreet);
			this.RenderSeparator(LocalizedStrings.GetHtmlEncoded(1414246315));
			addressInfo = ContactUtilities.GetAddressInfo(PhysicalAddressType.Home);
			this.RenderAddress(addressInfo, ContactSchema.HomeStreet);
			this.RenderSeparator(LocalizedStrings.GetHtmlEncoded(-582599340));
			addressInfo = ContactUtilities.GetAddressInfo(PhysicalAddressType.Other);
			this.RenderAddress(addressInfo, ContactSchema.OtherStreet);
		}

		protected void LoadMessageBodyIntoStream()
		{
			BodyConversionUtilities.RenderMeetingPlainTextBody(base.Response.Output, base.Item, base.UserContext, false);
		}

		protected void RenderAttachments()
		{
			base.Response.Write("<div id=\"cntAttchLabel\" class=\"cntInnerSectTxt\">");
			base.Response.Write(LocalizedStrings.GetHtmlEncoded(796893232));
			base.Response.Write("</div>");
			base.Response.Write("<div id=\"divCntAttchWell\">");
			AttachmentWell.RenderAttachmentWell(base.Response.Output, AttachmentWellType.ReadWrite, this.AttachmentWellRenderObjects, base.UserContext);
			base.Response.Write("</div>");
		}

		protected void CreateAttachmentHelpers()
		{
			if (base.Item != null)
			{
				this.attachmentWellRenderObjects = AttachmentWell.GetAttachmentInformation(base.Item, base.AttachmentLinks, base.UserContext.IsPublicLogon);
				foreach (object obj in this.attachmentWellRenderObjects)
				{
					AttachmentWellInfo attachmentWellInfo = (AttachmentWellInfo)obj;
					if (string.CompareOrdinal("ContactPicture.jpg", attachmentWellInfo.FileName) == 0)
					{
						this.attachmentWellRenderObjects.Remove(attachmentWellInfo);
						break;
					}
				}
				InfobarRenderingHelper infobarRenderingHelper = new InfobarRenderingHelper(this.attachmentWellRenderObjects);
				if (infobarRenderingHelper.HasLevelOne)
				{
					this.infobar.AddMessage(SanitizedHtmlString.FromStringId(-2118248931), InfobarMessageType.Informational, AttachmentWell.AttachmentInfobarHtmlTag);
				}
			}
		}

		protected void RenderCategoriesJavascriptArray()
		{
			CategorySwatch.RenderCategoriesJavascriptArray(base.SanitizingResponse, base.Item);
		}

		protected void RenderCategories()
		{
			base.SanitizingResponse.Write("<div id=\"divWellCategories\"");
			if (!base.HasCategories)
			{
				base.SanitizingResponse.Write(" style=\"display:none\"");
			}
			base.SanitizingResponse.Write("><div id=\"divFieldCategories\">");
			if (base.Item != null)
			{
				CategorySwatch.RenderCategories(base.OwaContext, base.SanitizingResponse, base.Item);
			}
			base.SanitizingResponse.Write("</div></div>");
		}

		protected ArrayList AttachmentWellRenderObjects
		{
			get
			{
				return this.attachmentWellRenderObjects;
			}
		}

		protected Infobar Infobar
		{
			get
			{
				return this.infobar;
			}
		}

		protected EditContactToolbar Toolbar
		{
			get
			{
				return this.toolbar;
			}
		}

		protected ContactPropertyInfo FirstPhoneProperty
		{
			get
			{
				return this.firstPhoneProperty;
			}
		}

		protected ContactPropertyInfo FirstEmailProperty
		{
			get
			{
				return this.firstEmailProperty;
			}
		}

		protected int FileAsValue
		{
			get
			{
				return (int)ContactUtilities.GetFileAs(base.Item);
			}
		}

		protected static int StoreObjectTypeContact
		{
			get
			{
				return 17;
			}
		}

		protected void RenderTitle()
		{
			if (base.Item == null)
			{
				base.Response.Write(LocalizedStrings.GetHtmlEncoded(-1873027801));
				return;
			}
			string propertyValue = this.GetPropertyValue(ContactBaseSchema.FileAs);
			if (string.IsNullOrEmpty(propertyValue))
			{
				base.Response.Write(LocalizedStrings.GetHtmlEncoded(-1873027801));
				return;
			}
			Utilities.HtmlEncode(propertyValue, base.Response.Output);
		}

		protected void RenderProfileArea()
		{
			if (ContactUtilities.GetDefaultFileAs() != FileAsMapping.LastCommaFirst)
			{
				if (this.isPhoneticNamesEnabled)
				{
					this.RenderTextProperty(ContactUtilities.YomiLastName);
				}
				this.RenderTextProperty(ContactUtilities.SurName);
				if (this.isPhoneticNamesEnabled)
				{
					this.RenderTextProperty(ContactUtilities.YomiFirstName);
				}
				this.RenderTextProperty(ContactUtilities.GivenName);
				if (!this.isPhoneticNamesEnabled)
				{
					this.RenderTextProperty(ContactUtilities.MiddleName);
				}
			}
			else
			{
				if (this.isPhoneticNamesEnabled)
				{
					this.RenderTextProperty(ContactUtilities.YomiFirstName);
				}
				this.RenderTextProperty(ContactUtilities.GivenName);
				if (!this.isPhoneticNamesEnabled)
				{
					this.RenderTextProperty(ContactUtilities.MiddleName);
				}
				if (this.isPhoneticNamesEnabled)
				{
					this.RenderTextProperty(ContactUtilities.YomiLastName);
				}
				this.RenderTextProperty(ContactUtilities.SurName);
			}
			this.RenderFileAs();
			this.RenderSeparator();
			this.RenderTextProperty(ContactUtilities.Title);
			this.RenderTextProperty(ContactUtilities.OfficeLocation);
			this.RenderTextProperty(ContactUtilities.Department);
			if (this.isPhoneticNamesEnabled)
			{
				this.RenderTextProperty(ContactUtilities.CompanyYomi);
			}
			this.RenderTextProperty(ContactUtilities.CompanyName);
			this.RenderTextProperty(ContactUtilities.Manager);
			this.RenderTextProperty(ContactUtilities.AssistantName);
		}

		protected void RenderContactArea()
		{
			this.RenderTextProperty(ContactUtilities.BusinessPhoneNumber);
			this.RenderTextProperty(ContactUtilities.HomePhone);
			this.RenderTextProperty(ContactUtilities.MobilePhone);
			this.RenderPhoneProperties();
			this.RenderSeparator();
			this.RenderEmailProperties();
			this.RenderSeparator();
			this.RenderTextProperty(ContactUtilities.IMAddress);
			this.RenderTextProperty(ContactUtilities.WebPage);
		}

		private static readonly PropertyDefinition[] additionalPrefetchProperties = new PropertyDefinition[]
		{
			ItemSchema.FlagStatus,
			ItemSchema.FlagCompleteTime,
			MessageItemSchema.ReplyTime,
			ItemSchema.UtcDueDate,
			ItemSchema.UtcStartDate,
			ItemSchema.ReminderDueBy,
			ItemSchema.ReminderIsSet,
			StoreObjectSchema.EffectiveRights
		};

		private static readonly PropertyDefinition[] prefetchProperties = EditContact.CreatePrefetchProperties();

		private bool isPhoneticNamesEnabled;

		private Infobar infobar = new Infobar();

		private EditContactToolbar toolbar;

		private ArrayList attachmentWellRenderObjects;

		private ContactPropertyInfo firstPhoneProperty;

		private ContactPropertyInfo firstEmailProperty;

		private Contact contact;
	}
}
