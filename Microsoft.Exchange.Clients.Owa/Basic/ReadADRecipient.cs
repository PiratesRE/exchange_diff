using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Directory;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	public class ReadADRecipient : OwaPage, IRegistryOnlyForm
	{
		protected override bool UseStrictMode
		{
			get
			{
				return false;
			}
		}

		protected ADRecipient ADRecipient
		{
			get
			{
				return this.adRecipient;
			}
		}

		internal IRecipientSession ADRecipientSession
		{
			get
			{
				return this.session;
			}
		}

		protected string MessageId
		{
			get
			{
				return this.messageIdString ?? string.Empty;
			}
		}

		protected string ChangeKey
		{
			get
			{
				return this.changeKey;
			}
		}

		protected AddressBook.Mode AddressBookMode
		{
			get
			{
				return this.viewMode;
			}
		}

		protected string RecipientId
		{
			get
			{
				return this.recipientId;
			}
		}

		protected int ReadItemType
		{
			get
			{
				if (!(this.adRecipient is IADDistributionList))
				{
					return 1;
				}
				return 2;
			}
		}

		protected int RecipientWell
		{
			get
			{
				return (int)this.recipientWell;
			}
		}

		protected string DisplayName
		{
			get
			{
				return this.adRecipient.DisplayName;
			}
		}

		protected static void RenderSecondaryNavigation(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<table class=\"snt\"><tr><td class=\"absn\">");
			writer.Write(LocalizedStrings.GetHtmlEncoded(-454140714));
			writer.Write("</td></tr></table>");
		}

		protected static void RenderDetailHeader(TextWriter writer, Strings.IDs name)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<tr><td colspan=2 class=\"hd lp\">");
			writer.Write(LocalizedStrings.GetHtmlEncoded(name));
			writer.Write("</td></tr>");
		}

		protected static void RenderAddressHeader(TextWriter writer, Strings.IDs name)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<tr><td colspan=2 class=\"hd\">");
			writer.Write(LocalizedStrings.GetHtmlEncoded(name));
			writer.Write("</td></tr>");
		}

		protected static void RenderADRecipient(TextWriter writer, int readItemType, ADObjectId adObjectId, string displayName)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<a href=\"#\" id=\"");
			writer.Write(Utilities.GetBase64StringFromADObjectId(adObjectId));
			writer.Write("\" class=\"peer\"");
			writer.Write(" onClick=\"return loadOrgP(this,");
			writer.Write(readItemType.ToString());
			writer.Write(")\">");
			Utilities.HtmlEncode(displayName, writer);
			writer.Write("</a>");
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.recipientId = Utilities.GetQueryStringParameter(base.Request, "id");
			this.hasOwner = !string.IsNullOrEmpty(Utilities.GetQueryStringParameter(base.Request, "oT", false));
			if (this.hasOwner)
			{
				this.viewMode = AddressBookHelper.TryReadAddressBookMode(base.Request, AddressBook.Mode.None);
				if (AddressBook.IsEditingMode(this.viewMode))
				{
					this.messageIdString = Utilities.GetQueryStringParameter(base.Request, "oId", false);
					this.changeKey = Utilities.GetQueryStringParameter(base.Request, "oCk", false);
				}
			}
			else
			{
				AddressBookViewState addressBookViewState = base.UserContext.LastClientViewState as AddressBookViewState;
				if (addressBookViewState != null)
				{
					this.viewMode = addressBookViewState.Mode;
					this.recipientWell = addressBookViewState.RecipientWell;
					if (addressBookViewState.ItemId != null)
					{
						this.messageIdString = addressBookViewState.ItemId.ToBase64String();
						this.changeKey = addressBookViewState.ItemChangeKey;
					}
				}
			}
			this.session = Utilities.CreateADRecipientSession(Microsoft.Exchange.Clients.Owa.Core.Culture.GetUserCulture().LCID, true, ConsistencyMode.FullyConsistent, true, base.UserContext);
			ADObjectId adobjectId = DirectoryAssistance.ParseADObjectId(this.recipientId);
			if (adobjectId == null)
			{
				throw new OwaADObjectNotFoundException();
			}
			this.adRecipient = this.session.Read(adobjectId);
			if (this.adRecipient == null)
			{
				throw new OwaADObjectNotFoundException();
			}
			this.session = Utilities.CreateADRecipientSession(Microsoft.Exchange.Clients.Owa.Core.Culture.GetUserCulture().LCID, true, ConsistencyMode.IgnoreInvalid, true, base.UserContext);
		}

		protected void RenderNavigation()
		{
			NavigationModule navigationModule = NavigationModule.AddressBook;
			switch (this.viewMode)
			{
			case AddressBook.Mode.EditMessage:
			case AddressBook.Mode.EditMeetingResponse:
				navigationModule = NavigationModule.Mail;
				break;
			case AddressBook.Mode.EditCalendar:
				navigationModule = NavigationModule.Calendar;
				break;
			}
			Navigation navigation = new Navigation(navigationModule, base.OwaContext, base.Response.Output);
			navigation.Render();
		}

		protected void RenderOptions(string helpFile)
		{
			OptionsBar optionsBar;
			if (AddressBook.IsEditingMode(this.viewMode))
			{
				string searchUrlSuffix = OptionsBar.BuildPeoplePickerSearchUrlSuffix(this.viewMode, this.MessageId, this.recipientWell);
				optionsBar = new OptionsBar(base.UserContext, base.Response.Output, OptionsBar.SearchModule.PeoplePicker, OptionsBar.RenderingFlags.None, searchUrlSuffix);
			}
			else
			{
				optionsBar = new OptionsBar(base.UserContext, base.Response.Output, OptionsBar.SearchModule.None);
			}
			optionsBar.Render(helpFile);
		}

		protected void RenderHeaderToolbar()
		{
			Toolbar toolbar = new Toolbar(base.Response.Output, true);
			toolbar.RenderStart();
			if (this.viewMode == AddressBook.Mode.Lookup || this.viewMode == AddressBook.Mode.None)
			{
				toolbar.RenderButton(ToolbarButtons.SendEmail);
				if (base.UserContext.IsFeatureEnabled(Feature.Calendar))
				{
					toolbar.RenderButton(ToolbarButtons.SendMeetingRequest);
					toolbar.RenderDivider();
				}
				if (base.UserContext.IsFeatureEnabled(Feature.Contacts))
				{
					toolbar.RenderButton(ToolbarButtons.AddToContacts);
					toolbar.RenderDivider();
				}
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

		protected void RenderDetailsLabel(TextWriter writer, Strings.IDs name, string value, ThemeFileId? themeFileId)
		{
			this.RenderDetailsLabel(writer, LocalizedStrings.GetHtmlEncoded(name), value, themeFileId);
		}

		protected void RenderDetailsLabel(TextWriter writer, string name, string value, ThemeFileId? themeFileId)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<tr><td class=\"lbl lp\" nowrap>");
			writer.Write(name);
			writer.Write("</td><td class=\"txvl");
			if (themeFileId != null)
			{
				writer.Write(" phtdpd\"><img ");
				Utilities.RenderImageAltAttribute(writer, base.UserContext, themeFileId.Value);
				writer.Write(" src=\"");
				base.UserContext.RenderThemeFileUrl(writer, themeFileId.Value);
			}
			writer.Write("\">");
			Utilities.HtmlEncode(value, writer);
			writer.Write("</td></tr>");
		}

		protected void RenderCustomJavascriptVariables()
		{
			if (this.hasOwner)
			{
				RenderingUtilities.RenderJavascriptVariable(base.Response.Output, "a_fOw", true);
				RenderingUtilities.RenderOwnerItemInformationFromQueryString(base.Request, base.Response.Output);
				return;
			}
			RenderingUtilities.RenderJavascriptVariable(base.Response.Output, "a_fOw", false);
			RenderingUtilities.RenderJavascriptVariable(base.Response.Output, "a_sBkUrl", base.UserContext.LastClientViewState.ToQueryString());
		}

		private IRecipientSession session;

		private ADRecipient adRecipient;

		private string changeKey = string.Empty;

		private string messageIdString;

		private string recipientId;

		private AddressBook.Mode viewMode;

		private RecipientItemType recipientWell = RecipientItemType.To;

		private bool hasOwner;
	}
}
