using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	public class ReadContactBase : OwaForm
	{
		protected NavigationModule Module
		{
			get
			{
				return this.module;
			}
			set
			{
				this.module = value;
			}
		}

		protected string FolderId
		{
			get
			{
				return this.FolderStoreObjectId.ToBase64String();
			}
		}

		internal StoreObjectId FolderStoreObjectId
		{
			get
			{
				if (!base.IsEmbeddedItem)
				{
					return base.Item.ParentId;
				}
				return base.ParentItem.ParentId;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!base.IsEmbeddedItem)
			{
				this.hasOwner = !string.IsNullOrEmpty(Utilities.GetQueryStringParameter(base.Request, "oT", false));
				if (this.hasOwner)
				{
					this.addressBookMode = AddressBookHelper.TryReadAddressBookMode(base.Request, AddressBook.Mode.None);
					if (AddressBook.IsEditingMode(this.addressBookMode))
					{
						this.editingItemId = Utilities.GetQueryStringParameter(base.Request, "oId", false);
						this.editingItemChangeKey = Utilities.GetQueryStringParameter(base.Request, "oCk", false);
						return;
					}
				}
				else
				{
					AddressBookViewState addressBookViewState = base.UserContext.LastClientViewState as AddressBookViewState;
					if (addressBookViewState != null)
					{
						this.addressBookMode = addressBookViewState.Mode;
						this.recipientWell = addressBookViewState.RecipientWell;
						if (addressBookViewState.ItemId != null)
						{
							this.editingItemId = addressBookViewState.ItemId.ToBase64String();
							this.editingItemChangeKey = addressBookViewState.ItemChangeKey;
						}
					}
				}
			}
		}

		protected void RenderNotes(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<tr><td colspan=2 class=\"hd\">");
			writer.Write(LocalizedStrings.GetHtmlEncoded(1601836855));
			writer.Write("</td></tr>");
			writer.Write("<tr><td colspan=2 class=\"rp\">");
			writer.Write("<textarea name=\"notes\" rows=10 cols=32 readonly>");
			BodyConversionUtilities.ConvertAndOutputBody(base.Response.Output, base.Item.Body, Markup.PlainText, null, true);
			writer.Write("</textarea>");
			writer.Write("</td></tr>");
		}

		protected override void RenderOptions(string helpFile)
		{
			OptionsBar optionsBar;
			if (base.UserContext.LastClientViewState is ContactModuleViewState)
			{
				optionsBar = new OptionsBar(base.UserContext, base.Response.Output, OptionsBar.SearchModule.Contacts, OptionsBar.RenderingFlags.None, OptionsBar.BuildFolderSearchUrlSuffix(base.UserContext, this.FolderStoreObjectId));
			}
			else if (AddressBook.IsEditingMode(this.addressBookMode))
			{
				string searchUrlSuffix = OptionsBar.BuildPeoplePickerSearchUrlSuffix(this.addressBookMode, this.editingItemId, this.recipientWell);
				optionsBar = new OptionsBar(base.UserContext, base.Response.Output, OptionsBar.SearchModule.PeoplePicker, OptionsBar.RenderingFlags.None, searchUrlSuffix);
			}
			else
			{
				optionsBar = new OptionsBar(base.UserContext, base.Response.Output, OptionsBar.SearchModule.None);
			}
			optionsBar.Render(helpFile);
		}

		protected void RenderNavigation()
		{
			Navigation navigation = new Navigation(this.Module, base.OwaContext, base.Response.Output);
			navigation.Render();
		}

		public void RenderSecondaryNavigation()
		{
			switch (this.Module)
			{
			case NavigationModule.Mail:
				this.RenderMailSecondaryNavigation();
				return;
			case NavigationModule.Contacts:
				this.RenderContactsSecondaryNavigation();
				return;
			}
			base.Response.Write("<div class=\"nsn\"></div>");
		}

		private void RenderContactsSecondaryNavigation()
		{
			ContactSecondaryNavigation contactSecondaryNavigation = new ContactSecondaryNavigation(base.OwaContext, this.FolderStoreObjectId, null);
			contactSecondaryNavigation.RenderContacts(base.Response.Output);
		}

		private void RenderMailSecondaryNavigation()
		{
			MailSecondaryNavigation mailSecondaryNavigation = new MailSecondaryNavigation(base.OwaContext, this.FolderStoreObjectId, null, null, null);
			mailSecondaryNavigation.Render(base.Response.Output);
		}

		protected void RenderHeaderToolbar()
		{
			Toolbar toolbar = new Toolbar(base.Response.Output, true);
			toolbar.RenderStart();
			if (!base.IsEmbeddedItem && !AddressBook.IsEditingMode(this.addressBookMode))
			{
				if (base.Item is Contact)
				{
					toolbar.RenderButton(ToolbarButtons.EditContact);
					toolbar.RenderDivider();
				}
				toolbar.RenderButton(ToolbarButtons.Move);
				toolbar.RenderButton(ToolbarButtons.Delete);
				toolbar.RenderDivider();
				toolbar.RenderButton(ToolbarButtons.SendEmailToContact);
				if (base.UserContext.IsFeatureEnabled(Feature.Calendar))
				{
					toolbar.RenderSpace();
					toolbar.RenderButton(ToolbarButtons.SendMeetingRequestToContact);
				}
				toolbar.RenderDivider();
			}
			toolbar.RenderButton(ToolbarButtons.CloseText);
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

		protected void RenderCustomJavascriptVariables()
		{
			TextWriter output = base.Response.Output;
			string name = "a_iCtx";
			int num = (int)this.addressBookMode;
			RenderingUtilities.RenderJavascriptVariable(output, name, num.ToString());
			if (AddressBook.IsEditingMode(this.addressBookMode))
			{
				if (!this.hasOwner)
				{
					RenderingUtilities.RenderJavascriptVariable(base.Response.Output, "a_sEdtId", this.editingItemId ?? string.Empty);
					RenderingUtilities.RenderJavascriptVariable(base.Response.Output, "a_sEdtCk", this.editingItemChangeKey ?? string.Empty);
				}
				TextWriter output2 = base.Response.Output;
				string name2 = "a_iEdtRw";
				int num2 = (int)this.recipientWell;
				RenderingUtilities.RenderJavascriptVariable(output2, name2, num2.ToString());
			}
			if (base.IsEmbeddedItem)
			{
				RenderingUtilities.RenderJavascriptVariable(base.Response.Output, "a_sPrTp", base.ParentItem.ClassName);
				return;
			}
			RenderingUtilities.RenderJavascriptVariable(base.Response.Output, "a_fInDelItms", base.IsInDeleteItems);
			Utilities.GetQueryStringParameter(base.Request, "oT", false);
			if (this.hasOwner)
			{
				RenderingUtilities.RenderJavascriptVariable(base.Response.Output, "a_fOw", true);
				RenderingUtilities.RenderOwnerItemInformationFromQueryString(base.Request, base.Response.Output);
				return;
			}
			RenderingUtilities.RenderJavascriptVariable(base.Response.Output, "a_fOw", false);
			RenderingUtilities.RenderJavascriptVariable(base.Response.Output, "a_sBkUrl", base.UserContext.LastClientViewState.ToQueryString());
		}

		internal void RenderEmail(TextWriter writer, string displayName, string emailAddress, string routingType, StoreObjectId storeId, EmailAddressIndex emailAddressIndex)
		{
			if (!string.IsNullOrEmpty(displayName))
			{
				string text = string.Format("({0})", emailAddress);
				if (displayName.EndsWith(text, StringComparison.Ordinal))
				{
					Utilities.HtmlEncode(displayName.Substring(0, displayName.Length - text.Length), writer);
				}
				else
				{
					Utilities.HtmlEncode(displayName, writer);
				}
				writer.Write(' ');
			}
			if (!string.IsNullOrEmpty(emailAddress))
			{
				if (!string.IsNullOrEmpty(routingType))
				{
					writer.Write("<span class=\"embra\">(");
					if (!base.UserContext.IsWebPartRequest)
					{
						writer.Write("<a href=\"#\" onclick=\"onClkAddRcpt('");
						Utilities.HtmlEncode(Utilities.JavascriptEncode(emailAddress), writer);
						writer.Write("','");
						if (!string.IsNullOrEmpty(displayName))
						{
							Utilities.HtmlEncode(Utilities.JavascriptEncode(displayName), writer);
						}
						writer.Write("','");
						Utilities.HtmlEncode(Utilities.JavascriptEncode(routingType), writer);
						writer.Write("','");
						if (storeId != null)
						{
							Utilities.HtmlEncode(Utilities.JavascriptEncode(storeId.ToBase64String()), writer);
							if (!Utilities.IsMapiPDL(routingType))
							{
								writer.Write("','");
								int num = (int)emailAddressIndex;
								Utilities.HtmlEncode(Utilities.JavascriptEncode(num.ToString()), writer);
							}
						}
						writer.Write("');\" class=\"emadr\">");
					}
					if (Utilities.IsCustomRoutingType(emailAddress, routingType))
					{
						Utilities.HtmlEncode(string.Concat(new string[]
						{
							"[",
							routingType,
							": ",
							emailAddress,
							"]"
						}), writer);
					}
					else
					{
						Utilities.HtmlEncode(emailAddress, writer);
					}
					if (!base.UserContext.IsWebPartRequest)
					{
						writer.Write("</a>");
					}
					writer.Write(")</span>");
					return;
				}
				writer.Write("(");
				Utilities.HtmlEncode(emailAddress, writer);
				writer.Write(")");
			}
		}

		private NavigationModule module = NavigationModule.Contacts;

		private AddressBook.Mode addressBookMode;

		private string editingItemId;

		private string editingItemChangeKey;

		private RecipientItemType recipientWell = RecipientItemType.To;

		private bool hasOwner;
	}
}
