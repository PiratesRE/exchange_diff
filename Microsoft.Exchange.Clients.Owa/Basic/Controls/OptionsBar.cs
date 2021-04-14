using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal sealed class OptionsBar
	{
		public static string BuildPeoplePickerSearchUrlSuffix(AddressBook.Mode mode, string itemId, RecipientItemType recipientWell)
		{
			StringBuilder stringBuilder = new StringBuilder('&' + "ctx" + '=');
			stringBuilder.Append((int)mode);
			stringBuilder.Append('&' + "id" + '=');
			stringBuilder.Append(Utilities.UrlEncode(itemId));
			stringBuilder.Append('&' + "rw" + '=');
			stringBuilder.Append((int)recipientWell);
			return stringBuilder.ToString();
		}

		public static string BuildFolderSearchUrlSuffix(UserContext userContext, StoreObjectId folderId)
		{
			StringBuilder stringBuilder = new StringBuilder('&' + "id" + '=');
			stringBuilder.Append(Utilities.UrlEncode(folderId.ToBase64String()));
			MessageModuleViewState messageModuleViewState = userContext.LastClientViewState as MessageModuleViewState;
			if (messageModuleViewState != null && messageModuleViewState.FolderId.Equals(folderId))
			{
				stringBuilder.Append('&' + "slUsng" + '=');
				stringBuilder.Append((int)messageModuleViewState.SelectedUsing);
			}
			return stringBuilder.ToString();
		}

		public OptionsBar(UserContext userContext, TextWriter writer, OptionsBar.SearchModule searchModule, OptionsBar.RenderingFlags renderingFlags, string searchUrlSuffix) : this(userContext, writer, searchModule)
		{
			if (this.IsFlagSet(OptionsBar.RenderingFlags.OptionsSelected) && this.IsFlagSet(OptionsBar.RenderingFlags.AddressBookSelected))
			{
				throw new ArgumentException("options and address book cannot both selected");
			}
			this.renderingFlags = renderingFlags;
			this.searchUrlSuffix = searchUrlSuffix;
			this.addressBookSearchViewState = (userContext.LastClientViewState as AddressBookSearchViewState);
			this.isInSearch = false;
			if (this.IsFlagSet(OptionsBar.RenderingFlags.ShowSearchContext))
			{
				if (OptionsBar.SearchModule.Mail == searchModule || OptionsBar.SearchModule.Calendar == searchModule)
				{
					MessageModuleSearchViewState messageModuleSearchViewState = userContext.LastClientViewState as MessageModuleSearchViewState;
					if (messageModuleSearchViewState != null && !string.IsNullOrEmpty(messageModuleSearchViewState.SearchString))
					{
						this.isInSearch = true;
						this.searchString = messageModuleSearchViewState.SearchString;
						return;
					}
				}
				else if (OptionsBar.SearchModule.Contacts == searchModule)
				{
					ContactModuleSearchViewState contactModuleSearchViewState = userContext.LastClientViewState as ContactModuleSearchViewState;
					if (contactModuleSearchViewState != null && !string.IsNullOrEmpty(contactModuleSearchViewState.SearchString))
					{
						this.isInSearch = true;
						this.searchString = contactModuleSearchViewState.SearchString;
						return;
					}
				}
				else if (this.addressBookSearchViewState != null && !string.IsNullOrEmpty(this.addressBookSearchViewState.SearchString))
				{
					this.isInSearch = true;
					this.searchString = this.addressBookSearchViewState.SearchString;
				}
			}
		}

		public OptionsBar(UserContext userContext, TextWriter writer, OptionsBar.SearchModule searchModule)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			this.userContext = userContext;
			this.writer = writer;
			this.searchModule = searchModule;
		}

		public void Render(string helpFile)
		{
			this.writer.Write("<table cellpadding=0 cellspacing=0><tr><td class=\"w100\">");
			this.RenderSearchModule();
			this.writer.Write("</td><td>");
			this.RenderButtonsStart();
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaDeployment.RenderPrivacyStatement.Enabled)
			{
				string text = Utilities.BuildPrivacyStatmentHref(this.userContext);
				if (!string.IsNullOrEmpty(text))
				{
					this.RenderPrivacyStatementLink(text);
					this.RenderDivider();
				}
			}
			this.RenderAddressBookButton();
			this.RenderDivider();
			this.RenderOptionsButton();
			this.RenderDivider();
			this.RenderHelpButton(helpFile, string.Empty);
			this.RenderDivider();
			this.RenderLogOffButton();
			this.RenderButtonsEnd();
			this.writer.Write("</td></tr></table>");
		}

		private void RenderButtonsStart()
		{
			this.writer.Write("<table class=\"ob\" cellpadding=0 cellspacing=0><caption>");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(1713194478));
			this.writer.Write("</caption><tr>");
		}

		private void RenderButtonsEnd()
		{
			this.writer.Write("</tr></table>");
		}

		private void RenderDivider()
		{
			this.writer.Write("<td class=\"dv\"><img src=\"");
			this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.OptionsBarDivider);
			this.writer.Write("\" alt=\"\"></td>");
		}

		private void RenderLogOffButton()
		{
			this.writer.Write("<td nowrap><div class=\"sch\"><a id=\"lo\" class=\"btn\" href=\"#\" onClick=\"return onClkLgf('");
			Utilities.HtmlEncode(Utilities.GetCurrentCanary(this.userContext), this.writer);
			this.writer.Write("');\" title=\"");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(-1065776905));
			this.writer.Write("\"><img class=\"noSrc\" src=\"");
			this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.Clear);
			this.writer.Write("\" alt=\"\">");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(-1065776905));
			this.writer.Write("</a></div></td>");
		}

		private void RenderPrivacyStatementLink(string privacyStatementHref)
		{
			this.writer.Write("<td nowrap><div class=\"sch\"><a class=\"btn\" href=\"#\" onClick=\"opnWin('");
			this.writer.Write(privacyStatementHref);
			this.writer.Write("')\" title=\"");
			this.writer.Write("{0}\"><img class=\"noSrc\" src=\"", LocalizedStrings.GetHtmlEncoded(-43540613));
			this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.Clear);
			this.writer.Write("\" alt=\"\">");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(-43540613));
			this.writer.Write("</a></div></td>");
		}

		private void RenderOptionsButton()
		{
			this.writer.Write("<td nowrap><td nowrap><div class=\"sch");
			if (this.IsFlagSet(OptionsBar.RenderingFlags.OptionsSelected))
			{
				this.writer.Write(" sMod\"><a class=\"btnb\"");
			}
			else
			{
				this.writer.Write("\"><a class=\"btn\" onClick=\"return onClkOp();\"");
			}
			this.writer.Write(" href=\"");
			if (this.IsFlagSet(OptionsBar.RenderingFlags.OptionsSelected))
			{
				this.writer.Write('#');
			}
			else if (this.IsFlagSet(OptionsBar.RenderingFlags.RenderCalendarOptionsLink))
			{
				this.writer.Write("?ae=Options&t=Calendar");
			}
			else
			{
				this.writer.Write("?ae=Options&t=Messaging");
			}
			this.writer.Write("\" title=\"");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(1511584348));
			this.writer.Write("\"><img src=\"");
			this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.Options);
			this.writer.Write("\" alt=\"\">");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(1511584348));
			this.writer.Write("</a></div></td>");
		}

		private void RenderSearchModule()
		{
			this.writer.Write("<table id=\"tblSch\" cellspacing=0 cellpadding=0><caption>");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(656259478));
			this.writer.Write("</caption><tr>");
			this.RenderSearchInputBox();
			this.writer.Write("<td><select id=\"selSch\"");
			if (this.isInSearch)
			{
				this.writer.Write(" onChange=\"onChgSch();\"");
			}
			this.writer.Write(" onKeyPress=\"return onEOSch(event);\">");
			if (OptionsBar.SearchModule.Mail == this.searchModule || OptionsBar.SearchModule.Calendar == this.searchModule)
			{
				this.RenderMailSearchOptions();
				this.RenderAddressBookSearchOption(string.Empty, false, LocalizedStrings.GetNonEncoded(1139489555), true);
				this.RenderContactSearchOption(true);
			}
			else
			{
				this.RenderContactSearchOption(false);
				this.RenderAddressBookSearchOptions();
			}
			this.writer.Write("</select></td>");
			this.RenderSearchAndClearButtons();
			this.writer.Write("</tr></table>");
		}

		private void RenderSearchInputBox()
		{
			this.writer.Write("<td><input type=\"text\" id=\"txtSch\" value=\"");
			if (this.IsFlagSet(OptionsBar.RenderingFlags.ShowSearchContext) && this.isInSearch)
			{
				Utilities.HtmlEncode(this.searchString, this.writer);
			}
			else
			{
				if (OptionsBar.SearchModule.Mail == this.searchModule || OptionsBar.SearchModule.Calendar == this.searchModule)
				{
					this.writer.Write(LocalizedStrings.GetHtmlEncoded(-1550155221));
				}
				else
				{
					this.writer.Write(LocalizedStrings.GetHtmlEncoded(-903656651));
				}
				this.writer.Write("\" class=\"noSch");
			}
			this.writer.Write("\" title=\"");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(656259478));
			this.writer.Write("\" onKeyPress=\"return onEOSch(event);\"");
			if (!this.isInSearch)
			{
				this.writer.Write(" onFocus=\"onFOSch(this);\"");
			}
			else
			{
				this.writer.Write(" onKeyUp=\"onKUSch();\" onChange=\"onChgSch();\"");
			}
			this.writer.Write(" maxlength=256></td>");
		}

		private void RenderSearchAndClearButtons()
		{
			this.writer.Write("<td><a id=\"schBtn\"");
			if (this.isInSearch)
			{
				this.writer.Write(" style=\"display:none\"");
			}
			this.writer.Write(" href=\"#\" onClick=\"return onClkSch();\" title=\"");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(656259478));
			this.writer.Write("\"><img src=\"");
			this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.BasicSearch);
			this.writer.Write("\" alt=\"");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(656259478));
			this.writer.Write("\"></a>");
			if (this.isInSearch)
			{
				this.writer.Write("<a id=\"clrBtn\" href=\"");
				this.writer.Write(((ISearchViewState)this.userContext.LastClientViewState).ClearSearchQueryString());
				this.writer.Write("\" title=\"");
				this.writer.Write(LocalizedStrings.GetHtmlEncoded(613695225));
				this.writer.Write("\"><img src=\"");
				this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.BasicCancelSearch);
				this.writer.Write("\" alt=\"");
				this.writer.Write(LocalizedStrings.GetHtmlEncoded(613695225));
				this.writer.Write("\"></a></td>");
			}
		}

		private void RenderMailSearchOptions()
		{
			SearchScope searchScope = this.userContext.UserOptions.GetSearchScope(OutlookModule.Mail);
			if (this.IsFlagSet(OptionsBar.RenderingFlags.ShowSearchContext) & this.isInSearch)
			{
				MessageModuleSearchViewState messageModuleSearchViewState = this.userContext.LastClientViewState as MessageModuleSearchViewState;
				if (messageModuleSearchViewState != null)
				{
					searchScope = messageModuleSearchViewState.SearchScope;
				}
			}
			if (OptionsBar.SearchModule.Calendar != this.searchModule)
			{
				this.RenderMailSearchOption(SearchScope.SelectedFolder, SearchScope.SelectedFolder == searchScope, 1749416719);
			}
			if (this.userContext.MailboxSession.Mailbox.IsContentIndexingEnabled)
			{
				this.RenderMailSearchOption(SearchScope.AllFoldersAndItems, SearchScope.AllFoldersAndItems == searchScope, 591328129);
			}
		}

		private void RenderMailSearchOption(SearchScope searchScope, bool isSelected, Strings.IDs nameId)
		{
			this.writer.Write("<option value=\"?ae=Folder&t=IPF.Note&newSch=1&scp=");
			this.writer.Write((int)searchScope);
			if (!string.IsNullOrEmpty(this.searchUrlSuffix) && (OptionsBar.SearchModule.Mail == this.searchModule || OptionsBar.SearchModule.Calendar == this.searchModule))
			{
				this.writer.Write(this.searchUrlSuffix);
			}
			if (isSelected)
			{
				this.writer.Write("\" selected>");
			}
			else
			{
				this.writer.Write("\">");
			}
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(nameId));
			this.writer.Write("</option>");
		}

		private void RenderAddressBookSearchOptions()
		{
			string base64Guid = this.userContext.GlobalAddressListInfo.ToAddressBookBase().Base64Guid;
			string text = "Ad" + ';' + base64Guid;
			string b = null;
			if (OptionsBar.SearchModule.Contacts != this.searchModule)
			{
				if (this.IsFlagSet(OptionsBar.RenderingFlags.ShowSearchContext) && this.addressBookSearchViewState != null)
				{
					b = this.addressBookSearchViewState.SearchLocation;
				}
				else
				{
					b = text;
				}
			}
			this.RenderAddressBookSearchOption(text, text == b, LocalizedStrings.GetNonEncoded(1139489555), false);
			if (this.userContext.IsFeatureEnabled(Feature.AddressLists))
			{
				foreach (AddressBookBase addressBookBase in DirectoryAssistance.GetAllAddressBooks(this.userContext))
				{
					if (addressBookBase.Base64Guid != base64Guid)
					{
						string text2 = "Ad" + ';' + addressBookBase.Base64Guid;
						this.RenderAddressBookSearchOption(text2, text2 == b, ". " + addressBookBase.DisplayName, false);
					}
				}
			}
		}

		private void RenderAddressBookSearchOption(string searchLocation, bool isSelected, string name, bool hasColor)
		{
			this.writer.Write("<option value=\"");
			if (this.IsFlagSet(OptionsBar.RenderingFlags.RenderSearchLocationOnly))
			{
				Utilities.HtmlEncode(searchLocation, this.writer);
			}
			else
			{
				this.writer.Write("?ae=Dialog&t=AddressBook&ab=");
				Utilities.HtmlEncode(Utilities.UrlEncode(searchLocation), this.writer);
				if (OptionsBar.SearchModule.PeoplePicker == this.searchModule && !string.IsNullOrEmpty(this.searchUrlSuffix))
				{
					this.writer.Write(this.searchUrlSuffix);
				}
				else
				{
					this.writer.Write("&ctx=1");
				}
			}
			if (hasColor)
			{
				this.writer.Write("\" class=\"cb");
			}
			if (isSelected)
			{
				this.writer.Write("\" selected>");
			}
			else
			{
				this.writer.Write("\">");
			}
			Utilities.HtmlEncode(name, this.writer);
			this.writer.Write("</option>");
		}

		private void RenderContactSearchOption(bool hasColor)
		{
			if (this.userContext.IsFeatureEnabled(Feature.Contacts))
			{
				if (OptionsBar.SearchModule.PeoplePicker == this.searchModule)
				{
					string text = "Con" + ';';
					bool isSelected = this.IsFlagSet(OptionsBar.RenderingFlags.ShowSearchContext) && this.addressBookSearchViewState != null && this.addressBookSearchViewState.SearchLocation == text;
					this.RenderAddressBookSearchOption(text, isSelected, LocalizedStrings.GetNonEncoded(1716044995), false);
				}
				else
				{
					this.writer.Write("<option value=\"?ae=Folder&t=IPF.Contact&newSch=1&scp=");
					if (this.userContext.MailboxSession.Mailbox.IsContentIndexingEnabled)
					{
						this.writer.Write(1);
					}
					else
					{
						this.writer.Write(0);
						if (OptionsBar.SearchModule.Contacts == this.searchModule && !string.IsNullOrEmpty(this.searchUrlSuffix))
						{
							this.writer.Write(this.searchUrlSuffix);
						}
					}
				}
				if (hasColor)
				{
					this.writer.Write("\" class=\"cb");
				}
				if (OptionsBar.SearchModule.Contacts == this.searchModule)
				{
					this.writer.Write("\" selected>");
				}
				else
				{
					this.writer.Write("\">");
				}
				this.writer.Write(LocalizedStrings.GetHtmlEncoded(1716044995));
				this.writer.Write("</option>");
			}
		}

		private void RenderHelpButton(string helpFile, string helpAnchor)
		{
			this.writer.Write("<td nowrap><div class=\"sch\"><a id=\"hlp\" class=\"btn\" href=\"#\" onClick=\"opnHlp('");
			this.writer.Write(Utilities.JavascriptEncode(Utilities.BuildEhcHref(helpFile)));
			this.writer.Write("')\" title=\"");
			this.writer.Write("{0}\"><img src=\"", LocalizedStrings.GetHtmlEncoded(1454393937));
			this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.Help);
			this.writer.Write("\" alt=\"");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(1454393937));
			this.writer.Write("\"></a></div></td>");
		}

		private void RenderAddressBookButton()
		{
			this.writer.Write("<td nowrap><div class=\"sch");
			if (this.IsFlagSet(OptionsBar.RenderingFlags.AddressBookSelected))
			{
				this.writer.Write(" sMod\"><a class=\"btnb\"");
			}
			else
			{
				this.writer.Write("\"><a class=\"btn\" onClick=\"return onClkAB();\"");
			}
			this.writer.Write(" href=\"");
			if (this.IsFlagSet(OptionsBar.RenderingFlags.AddressBookSelected))
			{
				this.writer.Write('#');
			}
			else if (OptionsBar.SearchModule.PeoplePicker == this.searchModule && !string.IsNullOrEmpty(this.searchUrlSuffix))
			{
				this.writer.Write("?ae=Dialog&t=AddressBook");
				this.writer.Write(this.searchUrlSuffix);
			}
			else
			{
				this.writer.Write("?ae=Dialog&t=AddressBook&ctx=1");
			}
			this.writer.Write("\" title=\"");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(1139489555));
			this.writer.Write("\"><img src=\"");
			this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.AddressBook);
			this.writer.Write("\" alt=\"");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(1139489555));
			this.writer.Write("\"></a></div></td>");
		}

		private bool IsFlagSet(OptionsBar.RenderingFlags flag)
		{
			return flag == (flag & this.renderingFlags);
		}

		private readonly TextWriter writer;

		private readonly UserContext userContext;

		private readonly OptionsBar.SearchModule searchModule;

		private readonly OptionsBar.RenderingFlags renderingFlags;

		private readonly AddressBookSearchViewState addressBookSearchViewState;

		private string searchString;

		private string searchUrlSuffix;

		private bool isInSearch;

		public enum SearchModule
		{
			None,
			Mail,
			Calendar,
			Contacts,
			PeoplePicker
		}

		[Flags]
		public enum RenderingFlags
		{
			None = 0,
			OptionsSelected = 1,
			AddressBookSelected = 2,
			ShowSearchContext = 4,
			RenderCalendarOptionsLink = 8,
			RenderSearchLocationOnly = 16
		}
	}
}
