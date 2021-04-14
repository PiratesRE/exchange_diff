using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	public class EditContact : OwaForm
	{
		protected NavigationModule Module
		{
			get
			{
				return this.navigationModule;
			}
		}

		protected bool IsPhoneticNamesEnabled
		{
			get
			{
				return this.isPhoneticNamesEnabled;
			}
		}

		protected bool IsPropertiesChanged
		{
			get
			{
				return this.helper.IsPropertiesChanged;
			}
		}

		protected string ContactId
		{
			get
			{
				return base.Item.Id.ObjectId.ToBase64String();
			}
		}

		protected string ContactIdString
		{
			get
			{
				if (base.Item != null)
				{
					return HttpUtility.UrlEncode(this.ContactId);
				}
				return string.Empty;
			}
		}

		internal StoreObjectId FolderId
		{
			get
			{
				return this.helper.FolderId;
			}
		}

		protected string FolderIdString
		{
			get
			{
				return this.FolderId.ToBase64String();
			}
		}

		protected string ChangeKey
		{
			get
			{
				if (base.Item != null)
				{
					return base.Item.Id.ChangeKeyAsBase64String();
				}
				return string.Empty;
			}
		}

		protected static void RenderVariable(TextWriter output, string name, string value)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("name may not be null or empty string");
			}
			output.Write("var ");
			output.Write(name);
			output.Write(" = \"");
			Utilities.JavascriptEncode(value, output);
			output.Write("\";\n");
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.helper = new EditContactHelper(base.UserContext, base.Request, false);
			base.Item = (this.helper.Contact ?? null);
			this.isPhoneticNamesEnabled = Utilities.IsJapanese;
			if (base.Item != null)
			{
				base.CreateAttachmentHelpers(AttachmentWellType.ReadWrite);
			}
			this.navigationModule = Navigation.GetNavigationModuleFromFolder(base.UserContext, this.FolderId);
		}

		protected override void OnUnload(EventArgs e)
		{
			if (this.helper != null)
			{
				this.helper.Dispose();
				this.helper = null;
			}
			base.OnUnload(e);
		}

		protected void RenderNavigation()
		{
			Navigation navigation = new Navigation(this.navigationModule, base.OwaContext, base.Response.Output);
			navigation.Render();
		}

		protected void RenderSecondaryNavigation()
		{
			NavigationModule navigationModule = this.navigationModule;
			if (navigationModule == NavigationModule.Contacts)
			{
				ContactSecondaryNavigation contactSecondaryNavigation = new ContactSecondaryNavigation(base.OwaContext, this.FolderId, null);
				contactSecondaryNavigation.RenderContacts(base.Response.Output);
				return;
			}
			base.Response.Write("<div class=\"nsn\"></div>");
		}

		protected override void RenderOptions(string helpFile)
		{
			OptionsBar optionsBar = new OptionsBar(base.UserContext, base.Response.Output, OptionsBar.SearchModule.Contacts);
			optionsBar.Render(helpFile);
		}

		protected void RenderHeaderToolbar()
		{
			Toolbar toolbar = new Toolbar(base.Response.Output, true);
			toolbar.RenderStart();
			toolbar.RenderButton(ToolbarButtons.SaveAndClose);
			toolbar.RenderDivider();
			toolbar.RenderButton(ToolbarButtons.Cancel);
			toolbar.RenderDivider();
			toolbar.RenderButton(ToolbarButtons.AttachFile);
			toolbar.RenderFill();
			toolbar.RenderButton(ToolbarButtons.CloseImage);
			toolbar.RenderEnd();
		}

		protected void RenderFooterToolbar()
		{
			Toolbar toolbar = new Toolbar(base.Response.Output, false);
			toolbar.RenderStart();
			toolbar.RenderFill();
			toolbar.RenderEnd();
		}

		protected void RenderInfobar()
		{
			base.Infobar.Render(base.Response.Output);
		}

		protected void RenderProfile()
		{
			if (ContactUtilities.GetDefaultFileAs() != FileAsMapping.LastCommaFirst)
			{
				if (this.IsPhoneticNamesEnabled)
				{
					this.RenderTextProperty(new ContactPropertyInfo[]
					{
						ContactUtilities.YomiLastName
					});
				}
				this.RenderTextProperty(new ContactPropertyInfo[]
				{
					ContactUtilities.SurName
				});
				if (this.IsPhoneticNamesEnabled)
				{
					this.RenderTextProperty(new ContactPropertyInfo[]
					{
						ContactUtilities.YomiFirstName
					});
				}
				this.RenderTextProperty(new ContactPropertyInfo[]
				{
					ContactUtilities.GivenName
				});
				if (!this.IsPhoneticNamesEnabled)
				{
					this.RenderTextProperty(new ContactPropertyInfo[]
					{
						ContactUtilities.MiddleName
					});
				}
			}
			else
			{
				if (this.IsPhoneticNamesEnabled)
				{
					this.RenderTextProperty(new ContactPropertyInfo[]
					{
						ContactUtilities.YomiFirstName
					});
				}
				this.RenderTextProperty(new ContactPropertyInfo[]
				{
					ContactUtilities.GivenName
				});
				if (!this.IsPhoneticNamesEnabled)
				{
					this.RenderTextProperty(new ContactPropertyInfo[]
					{
						ContactUtilities.MiddleName
					});
				}
				if (this.IsPhoneticNamesEnabled)
				{
					this.RenderTextProperty(new ContactPropertyInfo[]
					{
						ContactUtilities.YomiLastName
					});
				}
				this.RenderTextProperty(new ContactPropertyInfo[]
				{
					ContactUtilities.SurName
				});
			}
			this.RenderFileAs();
			this.RenderSeparator();
			this.RenderTextProperty(new ContactPropertyInfo[]
			{
				ContactUtilities.Title
			});
			this.RenderTextProperty(new ContactPropertyInfo[]
			{
				ContactUtilities.OfficeLocation
			});
			this.RenderTextProperty(new ContactPropertyInfo[]
			{
				ContactUtilities.Department
			});
			if (this.IsPhoneticNamesEnabled)
			{
				this.RenderTextProperty(new ContactPropertyInfo[]
				{
					ContactUtilities.CompanyYomi
				});
			}
			this.RenderTextProperty(new ContactPropertyInfo[]
			{
				ContactUtilities.CompanyName
			});
			this.RenderTextProperty(new ContactPropertyInfo[]
			{
				ContactUtilities.Manager
			});
			this.RenderTextProperty(new ContactPropertyInfo[]
			{
				ContactUtilities.AssistantName
			});
		}

		protected void RenderFileAs()
		{
			base.Response.Write("<tr><td class=\"cntPT\">");
			base.Response.Write(LocalizedStrings.GetHtmlEncoded(ContactUtilities.FileAsId.Label));
			base.Response.Write("</td><td class=\"cntPV\"><select name=\"");
			base.Response.Write(ContactUtilities.FileAsId.Id);
			base.Response.Write("\" class=\"cntDD cntCB\">");
			FileAsMapping fileAs = ContactUtilities.GetFileAs(base.Item);
			foreach (FileAsMapping fileAsMapping in ContactUtilities.GetSupportedFileAsMappings())
			{
				int num = (int)fileAsMapping;
				this.RenderListItem(num.ToString(), ContactUtilities.GetFileAsString(fileAsMapping), fileAsMapping == fileAs);
			}
			base.Response.Write("</select></td></tr>");
		}

		protected void RenderContact()
		{
			this.RenderTextProperty(ThemeFileId.WorkPhone, new ContactPropertyInfo[]
			{
				ContactUtilities.BusinessPhoneNumber
			});
			this.RenderTextProperty(ThemeFileId.HomePhone, new ContactPropertyInfo[]
			{
				ContactUtilities.HomePhone
			});
			this.RenderTextProperty(ThemeFileId.MobilePhone, new ContactPropertyInfo[]
			{
				ContactUtilities.MobilePhone
			});
			this.RenderPhoneProperties();
			this.RenderSeparator();
			this.RenderEmailProperties();
			this.RenderSeparator();
			this.RenderTextProperty(new ContactPropertyInfo[]
			{
				ContactUtilities.IMAddress
			});
			this.RenderTextProperty(new ContactPropertyInfo[]
			{
				ContactUtilities.WebPage
			});
		}

		protected void RenderPhoneProperties()
		{
			base.Response.Write("<tr><td class=\"cntPT\"><select id=phl class=\"cntDD\">");
			this.RenderListBody(ContactUtilities.AssistantPhoneNumber.Id, ContactUtilities.PhoneNumberProperties);
			base.Response.Write("</select></td><td class=\"cntPV\">");
			this.RenderTextControl(true, ContactUtilities.PhoneNumberProperties);
			base.Response.Write("</td></tr>");
			base.Response.Write("<tr><td colspan=2 class=cntPV2><table cellspacing=0 cellpadding=0 class=w100><tr><td id=cntLt>");
			base.Response.Write(LocalizedStrings.GetHtmlEncoded(-746050732));
			base.Response.Write("</td><td id=cntRt><select name=\"");
			base.Response.Write(ContactUtilities.DefaultPhoneNumber.Id);
			base.Response.Write("\" class=\"cntDD cntCB\">");
			int intValue = this.helper.GetIntValue(ContactUtilities.DefaultPhoneNumber, 1);
			for (int i = 0; i <= 18; i++)
			{
				this.RenderListItem(i.ToString(), ContactUtilities.GetPropertyStringFromPhoneNumberType((PhoneNumberType)i), intValue == i);
			}
			base.Response.Write("</select></td></tr></table></td></tr>");
		}

		protected void RenderEmailProperties()
		{
			base.Response.Write("<tr><td class=\"cntPT\"><select id=eml class=\"cntDD\">");
			this.RenderListBody(ContactUtilities.Email1EmailAddress.Id, ContactUtilities.EmailAddressProperties);
			base.Response.Write("</select></td><td class=\"cntPV\">");
			this.RenderTextControl(true, ContactUtilities.EmailAddressProperties);
			base.Response.Write("</td></tr>");
			base.Response.Write("<tr><td class=\"cntPT\">");
			base.Response.Write(LocalizedStrings.GetHtmlEncoded(1019177604));
			base.Response.Write("</td><td class=\"cntPV\">");
			this.RenderTextControl(new ContactPropertyInfo[]
			{
				ContactUtilities.Email1DisplayName,
				ContactUtilities.Email2DisplayName,
				ContactUtilities.Email3DisplayName
			});
			base.Response.Write("</td></tr>");
		}

		protected void RenderAddresses()
		{
			List<ContactPropertyInfo> addressInfo = ContactUtilities.GetAddressInfo(PhysicalAddressType.Business);
			List<ContactPropertyInfo> addressInfo2 = ContactUtilities.GetAddressInfo(PhysicalAddressType.Home);
			List<ContactPropertyInfo> addressInfo3 = ContactUtilities.GetAddressInfo(PhysicalAddressType.Other);
			for (int i = 0; i < addressInfo.Count; i++)
			{
				if (addressInfo[i].PropertyDefinition == ContactSchema.WorkAddressStreet)
				{
					this.RenderMultilineTextProperty(new ContactPropertyInfo[]
					{
						addressInfo[i],
						addressInfo2[i],
						addressInfo3[i]
					});
				}
				else
				{
					this.RenderTextProperty(new ContactPropertyInfo[]
					{
						addressInfo[i],
						addressInfo2[i],
						addressInfo3[i]
					});
				}
			}
			this.RenderMailingAddressCheckBox();
		}

		protected void RenderMailingAddressCheckBox()
		{
			base.Response.Write("<tr><td class=\"cntPT\">&nbsp;</td><td class=\"cntPV\"><input name=\"");
			base.Response.Write(ContactUtilities.PostalAddressId.Id);
			base.Response.Write("\" id=\"");
			base.Response.Write(ContactUtilities.PostalAddressId.Id);
			base.Response.Write("\" type=hidden value=");
			int intValue = this.helper.GetIntValue(ContactUtilities.PostalAddressId, 0);
			base.Response.Write(intValue);
			base.Response.Write("><input type=checkbox id=chkPa");
			if (intValue == 2)
			{
				base.Response.Write(" checked");
			}
			base.Response.Write("><label for=chkPa>");
			base.Response.Write(LocalizedStrings.GetHtmlEncoded(163570127));
			base.Response.Write("</label></td></tr>");
		}

		protected void RenderNotes()
		{
			base.Response.Write("<tr><td colspan=2 class=\"cntPV2\"><textarea name=\"");
			base.Response.Write("notes");
			base.Response.Write("\" id=\"notes\">");
			string notes = this.helper.GetNotes();
			if (!string.IsNullOrEmpty(notes))
			{
				Utilities.HtmlEncode(notes, base.Response.Output);
			}
			base.Response.Write("</textarea></td></tr>");
		}

		protected void RenderAttachments()
		{
			if (base.ShouldRenderAttachmentWell)
			{
				base.Response.Write("<tr><td colspan=2 class=\"cntPV2\">");
				AttachmentWell.RenderAttachmentWell(base.Response.Output, AttachmentWellType.ReadWrite, base.AttachmentWellRenderObjects, this.ContactId, base.UserContext, base.AttachmentWellFlags & ~AttachmentWell.AttachmentWellFlags.RenderEmbeddedItem);
				base.Response.Write("</td></tr>");
			}
		}

		protected void RenderTextControl(params ContactPropertyInfo[] propertyInfoList)
		{
			this.RenderTextControl(false, propertyInfoList);
		}

		protected void RenderTextControl(bool renderHiddenLabel, params ContactPropertyInfo[] propertyInfoList)
		{
			if (propertyInfoList == null)
			{
				throw new ArgumentNullException("propertyInfoList");
			}
			bool flag = true;
			foreach (ContactPropertyInfo contactPropertyInfo in propertyInfoList)
			{
				base.Response.Write("<input name=\"");
				base.Response.Write(contactPropertyInfo.Id);
				base.Response.Write("\" id=\"");
				base.Response.Write(contactPropertyInfo.Id);
				base.Response.Write("\" maxlength=256 class=\"cntTxt\" type=text value=\"");
				this.RenderPropertyStringValue(contactPropertyInfo);
				if (flag)
				{
					base.Response.Write("\">");
					flag = false;
				}
				else
				{
					base.Response.Write("\" style=\"display:none\">");
				}
				if (renderHiddenLabel)
				{
					base.Response.Write("<label class=hid for=");
					base.Response.Write(contactPropertyInfo.Id);
					base.Response.Write(">");
					base.Response.Write(LocalizedStrings.GetHtmlEncoded(contactPropertyInfo.Label));
					base.Response.Write("</label>");
				}
			}
		}

		protected void RenderTextProperty(params ContactPropertyInfo[] propertyInfoList)
		{
			if (propertyInfoList == null)
			{
				throw new ArgumentNullException("propertyInfoList");
			}
			base.Response.Write("<tr><td class=\"cntPT\">");
			base.Response.Write(LocalizedStrings.GetHtmlEncoded(propertyInfoList[0].Label));
			base.Response.Write("</td><td class=\"cntPV\">");
			this.RenderTextControl(propertyInfoList);
			base.Response.Write("</td></tr>");
		}

		protected void RenderTextProperty(ThemeFileId themeFileId, params ContactPropertyInfo[] propertyInfoList)
		{
			if (propertyInfoList == null)
			{
				throw new ArgumentNullException("propertyInfoList");
			}
			base.Response.Write("<tr><td class=\"cntImPT\"><img src=\"");
			base.UserContext.RenderThemeFileUrl(base.Response.Output, themeFileId);
			base.Response.Write("\" ");
			Utilities.RenderImageAltAttribute(base.Response.Output, base.UserContext, themeFileId);
			base.Response.Write(" class=cntIm>");
			base.Response.Write(LocalizedStrings.GetHtmlEncoded(propertyInfoList[0].Label));
			base.Response.Write("</td><td class=\"cntPV\">");
			this.RenderTextControl(propertyInfoList);
			base.Response.Write("</td></tr>");
		}

		protected void RenderMultilineTextProperty(params ContactPropertyInfo[] propertyInfoList)
		{
			if (propertyInfoList == null)
			{
				throw new ArgumentNullException("propertyInfoList");
			}
			base.Response.Write("<tr><td class=\"cntPT\"><label for=");
			base.Response.Write(propertyInfoList[0].Id);
			base.Response.Write(">");
			base.Response.Write(LocalizedStrings.GetHtmlEncoded(propertyInfoList[0].Label));
			base.Response.Write("</label></td><td class=\"cntPV\">");
			bool flag = true;
			foreach (ContactPropertyInfo contactPropertyInfo in propertyInfoList)
			{
				if (!flag)
				{
					base.Response.Write("<label class=hid for=");
					base.Response.Write(contactPropertyInfo.Id);
					base.Response.Write(">");
					base.Response.Write(LocalizedStrings.GetHtmlEncoded(contactPropertyInfo.Label));
					base.Response.Write("</label>");
				}
				base.Response.Write("<textarea name=\"");
				base.Response.Write(contactPropertyInfo.Id);
				base.Response.Write("\" id=\"");
				base.Response.Write(contactPropertyInfo.Id);
				base.Response.Write("\" class=\"cntTxt\" wrap=\"virtual\"");
				if (flag)
				{
					base.Response.Write(">");
					flag = false;
				}
				else
				{
					base.Response.Write(" style=\"display:none\">");
				}
				this.RenderPropertyStringValue(contactPropertyInfo);
				base.Response.Write("</textarea>");
			}
			base.Response.Write("</td></tr>");
		}

		private void RenderHiddenInput(string name, string value, string id)
		{
			base.Response.Write("<input type=hidden name=\"");
			base.Response.Write(name);
			base.Response.Write("\"");
			if (!string.IsNullOrEmpty(id))
			{
				base.Response.Write(" id=\"");
				base.Response.Write(id);
				base.Response.Write("\"");
			}
			base.Response.Write(" value=\"");
			Utilities.HtmlEncode(value, base.Response.Output);
			base.Response.Write("\">");
		}

		protected void RenderHiddenInputs()
		{
			if (base.Item != null)
			{
				this.RenderHiddenInput("hidid", this.ContactId, "hidid");
				this.RenderHiddenInput("hidchk", this.ChangeKey, null);
			}
			else
			{
				string formParameterStringValue = this.helper.GetFormParameterStringValue("hidid");
				string formParameterStringValue2 = this.helper.GetFormParameterStringValue("hidchk");
				if (!string.IsNullOrEmpty(formParameterStringValue) && !string.IsNullOrEmpty(formParameterStringValue2))
				{
					this.RenderHiddenInput("hidid", formParameterStringValue, "hidid");
					this.RenderHiddenInput("hidchk", formParameterStringValue2, null);
				}
			}
			this.RenderHiddenInput("hidfldid", this.FolderIdString, "hidfldid");
		}

		private void RenderListItem(string value, string display, bool isSelected)
		{
			base.Response.Write("<option");
			if (isSelected)
			{
				base.Response.Write(" selected");
			}
			base.Response.Write(" value=\"");
			Utilities.HtmlEncode(value, base.Response.Output);
			base.Response.Write("\">");
			Utilities.HtmlEncode(display, base.Response.Output);
			base.Response.Write("</option>");
		}

		protected void RenderListBody(string selectedValue, params ContactPropertyInfo[] propertyInfoList)
		{
			if (propertyInfoList == null)
			{
				throw new ArgumentNullException("propertyInfoList");
			}
			foreach (ContactPropertyInfo contactPropertyInfo in propertyInfoList)
			{
				this.RenderListItem(contactPropertyInfo.Id, LocalizedStrings.GetNonEncoded(contactPropertyInfo.Label), contactPropertyInfo.Id == selectedValue);
			}
		}

		protected void RenderSeparator()
		{
			base.Response.Write("<tr><td colspan=2 class=\"cntSep\">&nbsp;</td></tr>");
		}

		protected void RenderDashBorderSeparator()
		{
			base.Response.Write("<tr><td colspan=2 class=\"cntSep\"><table><tr><td></td></tr></table></td></tr>");
		}

		protected void RenderPropertyStringValue(ContactPropertyInfo propertyInfo)
		{
			this.RenderPropertyStringValue(propertyInfo, string.Empty);
		}

		protected void RenderPropertyStringValue(ContactPropertyInfo propertyInfo, string defaultValue)
		{
			if (propertyInfo == null)
			{
				throw new ArgumentNullException("propertyInfo");
			}
			string stringValue = this.helper.GetStringValue(propertyInfo, defaultValue);
			if (!string.IsNullOrEmpty(stringValue))
			{
				Utilities.HtmlEncode(stringValue, base.Response.Output);
			}
		}

		private bool isPhoneticNamesEnabled;

		private EditContactHelper helper;

		private NavigationModule navigationModule;
	}
}
