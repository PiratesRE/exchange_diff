using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Core.Directory;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	public class AddressBook : OwaForm
	{
		internal MessageItem Message
		{
			get
			{
				return base.Item as MessageItem;
			}
		}

		internal CalendarItemBase CalendarItemBase
		{
			get
			{
				return base.Item as CalendarItemBase;
			}
		}

		protected string MessageId
		{
			get
			{
				if (base.Item != null && base.Item.Id != null)
				{
					return base.Item.Id.ObjectId.ToBase64String();
				}
				return string.Empty;
			}
		}

		protected int PageNumber
		{
			get
			{
				return this.pageNumber;
			}
		}

		protected string ChangeKey
		{
			get
			{
				if (base.Item != null && base.Item.Id != null)
				{
					return base.Item.Id.ChangeKeyAsBase64String();
				}
				return string.Empty;
			}
		}

		protected AddressBook.Mode AddressBookMode
		{
			get
			{
				return this.viewMode;
			}
		}

		protected string AddressBookToSearch
		{
			get
			{
				return this.addressBookToSearch;
			}
		}

		protected string SearchString
		{
			get
			{
				return this.searchString;
			}
		}

		protected ColumnId SortColumnId
		{
			get
			{
				return this.sortColumn;
			}
		}

		protected SortOrder SortOrder
		{
			get
			{
				return this.sortOrder;
			}
		}

		protected int RecipientWell
		{
			get
			{
				return (int)this.recipientWell;
			}
		}

		protected string BackUrl
		{
			get
			{
				return ((AddressBookViewState)base.UserContext.LastClientViewState).PreviousViewState.ToQueryString();
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "ctx");
			int num;
			if (!int.TryParse(queryStringParameter, out num))
			{
				throw new OwaInvalidRequestException("Context parameter is having invalid format");
			}
			if (num < 0 || num > 4)
			{
				throw new OwaInvalidRequestException("Invalid context value in the querystring parameter");
			}
			this.viewMode = (AddressBook.Mode)num;
			if (this.viewMode == AddressBook.Mode.None)
			{
				this.viewMode = AddressBook.Mode.Lookup;
			}
			if (base.IsPostFromMyself())
			{
				this.action = Utilities.GetFormParameter(base.Request, "hidcmdpst", false);
				this.searchString = Utilities.GetFormParameter(base.Request, "hidss", false);
				this.pageNumber = RequestParser.TryGetIntValueFromForm(base.Request, "hidpg", 1);
				this.sortColumn = (ColumnId)RequestParser.TryGetIntValueFromForm(base.Request, "hidcid", 11);
				this.sortOrder = (SortOrder)RequestParser.TryGetIntValueFromForm(base.Request, "hidso", 1);
				this.addressBookToSearch = Utilities.GetFormParameter(base.Request, "hidAB", false);
				if (string.IsNullOrEmpty(this.addressBookToSearch))
				{
					throw new OwaInvalidRequestException("addressbookGuid can't be null");
				}
				this.recipientWell = (RecipientItemType)RequestParser.TryGetIntValueFromForm(base.Request, "hidrw", 1);
			}
			else
			{
				this.searchString = Utilities.GetQueryStringParameter(base.Request, "sch", false);
				if (!string.IsNullOrEmpty(this.searchString))
				{
					Utilities.VerifySearchCanaryInGetRequest(base.Request);
				}
				this.pageNumber = RequestParser.TryGetIntValueFromQueryString(base.Request, "pg", 1);
				this.sortColumn = (ColumnId)RequestParser.TryGetIntValueFromQueryString(base.Request, "cid", 11);
				this.sortOrder = (SortOrder)RequestParser.TryGetIntValueFromQueryString(base.Request, "so", 1);
				this.addressBookToSearch = Utilities.GetQueryStringParameter(base.Request, "ab", false);
				this.recipientWell = (RecipientItemType)RequestParser.TryGetIntValueFromQueryString(base.Request, "rw", 1);
			}
			this.GetSearchLocation();
			if (AddressBook.IsEditingMode(this.viewMode))
			{
				if (!base.IsPostFromMyself())
				{
					bool required = this.viewMode != AddressBook.Mode.EditCalendar;
					StoreObjectId itemId = QueryStringUtilities.CreateItemStoreObjectId(base.UserContext.MailboxSession, base.Request, required);
					base.Item = AddressBookHelper.GetItem(base.UserContext, this.viewMode, itemId, null);
				}
				else
				{
					StoreObjectId itemId2 = null;
					string formParameter = Utilities.GetFormParameter(base.Request, "hidid", true);
					if (!string.IsNullOrEmpty(formParameter))
					{
						itemId2 = Utilities.CreateStoreObjectId(base.UserContext.MailboxSession, formParameter);
					}
					string formParameter2 = Utilities.GetFormParameter(base.Request, "hidchk", true);
					base.Item = AddressBookHelper.GetItem(base.UserContext, this.viewMode, itemId2, formParameter2);
					string a;
					if ((a = this.action) != null)
					{
						if (!(a == "addrcp"))
						{
							if (a == "rmrcp")
							{
								int intValueFromForm = RequestParser.GetIntValueFromForm(base.Request, "hidri");
								if (this.viewMode == AddressBook.Mode.EditMessage || this.viewMode == AddressBook.Mode.EditMeetingResponse)
								{
									if (intValueFromForm >= 0 && intValueFromForm < this.Message.Recipients.Count)
									{
										this.Message.Recipients.RemoveAt(intValueFromForm);
										AddressBookHelper.SaveItem(base.Item);
									}
								}
								else if (this.viewMode == AddressBook.Mode.EditCalendar)
								{
									CalendarUtilities.RemoveAttendeeAt(this.CalendarItemBase, intValueFromForm);
									EditCalendarItemHelper.CreateUserContextData(base.UserContext, this.CalendarItemBase);
								}
							}
						}
						else
						{
							int num2 = RequestParser.TryGetIntValueFromQueryString(base.Request, "rt", 1);
							if (num2 == 1)
							{
								this.type = RecipientItemType.To;
							}
							else if (num2 == 2)
							{
								this.type = RecipientItemType.Cc;
							}
							else if (num2 == 3)
							{
								this.type = RecipientItemType.Bcc;
							}
							string text = base.Request.Form["chkRcpt"];
							if (!string.IsNullOrEmpty(text))
							{
								this.ids = text.Split(new char[]
								{
									','
								});
								if (this.searchLocation == AddressBook.SearchLocation.AddressBook)
								{
									AddressBookHelper.AddRecipientsToDraft(this.ids, base.Item, this.type, base.UserContext);
								}
								else
								{
									AddressBookHelper.AddContactsToDraft(base.Item, this.type, base.UserContext, this.ids);
								}
							}
						}
					}
				}
			}
			if (!string.IsNullOrEmpty(this.searchString))
			{
				this.searchString = this.searchString.Trim();
				if (this.searchString.Length > Globals.MaxSearchStringLength)
				{
					throw new OwaInvalidRequestException("Search string length is more than 256 characters");
				}
			}
			if (this.pageNumber == 0)
			{
				this.pageNumber = 1;
			}
			this.firstItemOnPage = (this.pageNumber - 1) * base.UserContext.UserOptions.BasicViewRowCount + 1;
			this.lastItemOnPage = this.firstItemOnPage + base.UserContext.UserOptions.BasicViewRowCount - 1;
			this.CreateListView();
			if (AddressBook.IsEditingMode(this.viewMode) || !string.IsNullOrEmpty(this.searchString))
			{
				base.UserContext.LastClientViewState = new AddressBookSearchViewState(base.UserContext.LastClientViewState, this.viewMode, this.addressBookToSearch, this.searchString, this.pageNumber, (base.Item == null || base.Item.Id == null) ? null : base.Item.Id.ObjectId, (base.Item == null || base.Item.Id == null) ? null : base.Item.Id.ChangeKeyAsBase64String(), this.recipientWell, this.sortColumn, this.sortOrder);
				return;
			}
			base.UserContext.LastClientViewState = new AddressBookViewState(base.UserContext.LastClientViewState, this.viewMode, this.pageNumber, (base.Item == null || base.Item.Id == null) ? null : base.Item.Id.ObjectId, (base.Item == null || base.Item.Id == null) ? null : base.Item.Id.ChangeKeyAsBase64String(), this.recipientWell, this.sortColumn, this.sortOrder);
		}

		protected override void OnUnload(EventArgs e)
		{
			if (this.folder != null)
			{
				this.folder.Dispose();
				this.folder = null;
			}
			if (this.searchFolder != null)
			{
				this.searchFolder.Dispose();
				this.searchFolder = null;
			}
			base.OnUnload(e);
		}

		private void GetSearchLocation()
		{
			if (string.IsNullOrEmpty(this.searchString) || string.IsNullOrEmpty(this.addressBookToSearch))
			{
				this.addressBookBase = base.UserContext.GlobalAddressListInfo.ToAddressBookBase();
				this.addressBookToSearch = "Ad" + ';' + this.addressBookBase.Base64Guid;
				this.addressBookInfo = new string[]
				{
					"Ad",
					this.addressBookBase.Base64Guid
				};
				return;
			}
			bool flag = false;
			this.addressBookInfo = this.addressBookToSearch.Split(new char[]
			{
				';'
			});
			if (this.addressBookInfo.Length == 2)
			{
				if (string.CompareOrdinal(this.addressBookInfo[0], "Ad") == 0)
				{
					if (!string.IsNullOrEmpty(this.addressBookInfo[1]))
					{
						flag = true;
						this.addressBookBase = DirectoryAssistance.FindAddressBook(this.addressBookInfo[1], base.UserContext);
					}
				}
				else if (string.CompareOrdinal(this.addressBookInfo[0], "Con") == 0)
				{
					flag = true;
					if (string.CompareOrdinal(this.action, "s") == 0)
					{
						this.isNewSearch = false;
					}
					this.searchLocation = AddressBook.SearchLocation.Contacts;
					this.folder = Folder.Bind(base.UserContext.MailboxSession, base.UserContext.ContactsFolderId);
				}
			}
			if (!flag)
			{
				throw new OwaInvalidRequestException("Invalid search location for addressbook");
			}
		}

		public void CreateListView()
		{
			if (this.searchLocation == AddressBook.SearchLocation.AddressBook)
			{
				this.listView = new AddressBookListView(this.searchString, base.UserContext, ColumnId.DisplayNameAD, SortOrder.Ascending, this.addressBookBase, AddressBookBase.RecipientCategory.All);
			}
			else
			{
				FolderSearch folderSearch = new FolderSearch();
				SearchScope searchScope = base.UserContext.MailboxSession.Mailbox.IsContentIndexingEnabled ? SearchScope.SelectedAndSubfolders : SearchScope.SelectedFolder;
				this.searchFolder = folderSearch.Execute(base.UserContext, this.folder, searchScope, this.searchString, this.isNewSearch, false);
				object obj = this.searchFolder.TryGetProperty(FolderSchema.ItemCount);
				if (!(obj is PropertyError))
				{
					this.itemCount = (int)obj;
				}
				this.listView = new ContactsListView(base.UserContext, this.sortColumn, this.sortOrder, this.searchFolder, searchScope);
			}
			this.listView.Initialize(this.firstItemOnPage, this.lastItemOnPage);
			if (!string.IsNullOrEmpty(this.searchString))
			{
				SanitizedHtmlString sanitizedHtmlString = new SanitizedHtmlString("<a href=\"#\" onclick=\"return onClkClrLnk();\">" + LocalizedStrings.GetHtmlEncoded(1155007962) + "</a>");
				sanitizedHtmlString.DecreeToBeTrusted();
				SanitizedHtmlString messageHtml;
				if (this.listView.TotalCount == 0)
				{
					messageHtml = SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(-761327948), new object[]
					{
						sanitizedHtmlString
					});
				}
				else if (this.searchLocation == AddressBook.SearchLocation.AddressBook)
				{
					if (this.addressBookBase.Base64Guid == base.UserContext.GlobalAddressListInfo.ToAddressBookBase().Base64Guid)
					{
						messageHtml = SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(-121508646), new object[]
						{
							this.listView.TotalCount,
							this.searchString,
							sanitizedHtmlString
						});
					}
					else
					{
						messageHtml = SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(-1725472335), new object[]
						{
							this.listView.TotalCount,
							this.searchString,
							this.addressBookBase.DisplayName,
							sanitizedHtmlString
						});
					}
				}
				else
				{
					messageHtml = SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(-1403744948), new object[]
					{
						this.listView.TotalCount,
						this.searchString,
						sanitizedHtmlString
					});
				}
				base.Infobar.AddMessageHtml(messageHtml, InfobarMessageType.Informational);
			}
		}

		public void RenderAddressBookView(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			string empty = string.Empty;
			this.listView.Render(writer, empty);
		}

		public void RenderRecipientWell(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (base.Item == null)
			{
				throw new OwaInvalidRequestException("item cannot be null in the people picker mode");
			}
			if (AddressBook.IsEditingMode(this.viewMode))
			{
				writer.Write("<table class=\"rw\" cellpadding=0 cellspacing=0><tr><td class=\"hPd\">");
				Strings.IDs label = (this.viewMode == AddressBook.Mode.EditMessage || this.viewMode == AddressBook.Mode.EditMeetingResponse) ? 932616230 : 1982771038;
				Strings.IDs label2 = (this.viewMode == AddressBook.Mode.EditMessage || this.viewMode == AddressBook.Mode.EditMeetingResponse) ? -876870293 : 1605591873;
				Strings.IDs label3 = (this.viewMode == AddressBook.Mode.EditMessage || this.viewMode == AddressBook.Mode.EditMeetingResponse) ? 125372521 : 1671797350;
				AddressBookHelper.RenderAddressBookButton(writer, "1", label);
				writer.Write("</td><td class=\"pd\"><div class=\"rWll\">");
				if (this.viewMode == AddressBook.Mode.EditMessage || this.viewMode == AddressBook.Mode.EditMeetingResponse)
				{
					AddressBookHelper.RenderRecipients(writer, RecipientItemType.To, this.Message);
				}
				else
				{
					AddressBookHelper.RenderAttendees(writer, AttendeeType.Required, this.CalendarItemBase);
				}
				writer.Write("</div></td></tr><tr><td class=\"hPd\">");
				AddressBookHelper.RenderAddressBookButton(writer, "2", label2);
				writer.Write("</td><td class=\"pd\"><div class=\"rWll\">");
				if (this.viewMode == AddressBook.Mode.EditMessage || this.viewMode == AddressBook.Mode.EditMeetingResponse)
				{
					AddressBookHelper.RenderRecipients(writer, RecipientItemType.Cc, this.Message);
				}
				else
				{
					AddressBookHelper.RenderAttendees(writer, AttendeeType.Optional, this.CalendarItemBase);
				}
				writer.Write("</div></td></tr><tr><td class=\"hPd\">");
				AddressBookHelper.RenderAddressBookButton(writer, "3", label3);
				writer.Write("</td><td class=\"pd\"><div class=\"rWll\">");
				if (this.viewMode == AddressBook.Mode.EditMessage || this.viewMode == AddressBook.Mode.EditMeetingResponse)
				{
					AddressBookHelper.RenderRecipients(writer, RecipientItemType.Bcc, this.Message);
				}
				else
				{
					AddressBookHelper.RenderAttendees(writer, AttendeeType.Resource, this.CalendarItemBase);
				}
				writer.Write("</div></td></tr><tr><td colspan=2 class=\"spc\"></td></tr></table>");
			}
		}

		public void RenderNavigation()
		{
			Navigation navigation = null;
			if (this.viewMode == AddressBook.Mode.Lookup)
			{
				navigation = new Navigation(NavigationModule.AddressBook, base.OwaContext, base.Response.Output);
			}
			else if (this.viewMode == AddressBook.Mode.EditMessage || this.viewMode == AddressBook.Mode.EditMeetingResponse)
			{
				navigation = new Navigation(NavigationModule.Mail, base.OwaContext, base.Response.Output);
			}
			else if (this.viewMode == AddressBook.Mode.EditCalendar)
			{
				navigation = new Navigation(NavigationModule.Calendar, base.OwaContext, base.Response.Output);
			}
			if (navigation != null)
			{
				navigation.Render();
			}
		}

		protected override void RenderOptions(string helpFile)
		{
			OptionsBar.SearchModule searchModule = OptionsBar.SearchModule.None;
			OptionsBar.RenderingFlags renderingFlags = OptionsBar.RenderingFlags.AddressBookSelected | OptionsBar.RenderingFlags.ShowSearchContext;
			string searchUrlSuffix = null;
			if (AddressBook.IsEditingMode(this.viewMode))
			{
				searchModule = OptionsBar.SearchModule.PeoplePicker;
				searchUrlSuffix = OptionsBar.BuildPeoplePickerSearchUrlSuffix(this.viewMode, this.MessageId, this.recipientWell);
			}
			OptionsBar optionsBar = new OptionsBar(base.UserContext, base.Response.Output, searchModule, renderingFlags, searchUrlSuffix);
			optionsBar.Render(helpFile);
		}

		public void RenderSecondaryNavigation(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<table class=\"snt\"><tr><td class=\"absn\">");
			writer.Write(LocalizedStrings.GetHtmlEncoded(-454140714));
			writer.Write("</td></tr>");
			if (AddressBook.IsEditingMode(this.viewMode))
			{
				writer.Write("<tr><td class=\"absn\">");
				if (this.viewMode == AddressBook.Mode.EditCalendar)
				{
					writer.Write(LocalizedStrings.GetHtmlEncoded(-611516900));
				}
				else
				{
					writer.Write(LocalizedStrings.GetHtmlEncoded(72613029));
				}
				writer.Write("</td></tr>");
			}
			writer.Write("</table>");
		}

		public void RenderHeaderToolbar()
		{
			Toolbar toolbar = new Toolbar(base.Response.Output, true);
			toolbar.RenderStart();
			switch (this.viewMode)
			{
			case AddressBook.Mode.Lookup:
				toolbar.RenderButton(ToolbarButtons.SendEmail);
				if (base.UserContext.IsFeatureEnabled(Feature.Calendar))
				{
					toolbar.RenderButton(ToolbarButtons.SendMeetingRequest);
					toolbar.RenderDivider();
				}
				if (this.searchLocation == AddressBook.SearchLocation.AddressBook && base.UserContext.IsFeatureEnabled(Feature.Contacts))
				{
					toolbar.RenderButton(ToolbarButtons.AddToContacts);
					toolbar.RenderDivider();
				}
				toolbar.RenderButton(ToolbarButtons.CloseText);
				break;
			case AddressBook.Mode.EditMessage:
			case AddressBook.Mode.EditCalendar:
			case AddressBook.Mode.EditMeetingResponse:
				toolbar.RenderButton(ToolbarButtons.MessageRecipients);
				toolbar.RenderButton(ToolbarButtons.Done);
				toolbar.RenderDivider();
				toolbar.RenderButton(ToolbarButtons.CloseText);
				break;
			}
			toolbar.RenderFill();
			this.RenderPaging(false);
			toolbar.RenderSpace();
			toolbar.RenderButton(ToolbarButtons.CloseImage);
			toolbar.RenderEnd();
		}

		public void RenderFooterToolbar()
		{
			Toolbar toolbar = new Toolbar(base.Response.Output, false);
			toolbar.RenderStart();
			switch (this.viewMode)
			{
			case AddressBook.Mode.Lookup:
				toolbar.RenderFill();
				break;
			case AddressBook.Mode.EditMessage:
			case AddressBook.Mode.EditCalendar:
			case AddressBook.Mode.EditMeetingResponse:
				toolbar.RenderButton(ToolbarButtons.Done);
				toolbar.RenderDivider();
				toolbar.RenderButton(ToolbarButtons.CloseText);
				toolbar.RenderFill();
				break;
			}
			this.RenderPaging(true);
			toolbar.RenderEnd();
		}

		protected void RenderPaging(bool renderPageNumbers)
		{
			if (this.searchLocation == AddressBook.SearchLocation.AddressBook)
			{
				if (renderPageNumbers)
				{
					this.listView.RenderADContentsPaging(base.Response.Output, this.pageNumber);
					return;
				}
				this.listView.RenderADContentsHeaderPaging(base.Response.Output, this.pageNumber);
				return;
			}
			else
			{
				if (this.pageNumber == 0)
				{
					this.pageNumber = 1;
				}
				int totalNumberOfPages;
				if (this.itemCount % base.UserContext.UserOptions.BasicViewRowCount == 0)
				{
					totalNumberOfPages = this.itemCount / base.UserContext.UserOptions.BasicViewRowCount;
				}
				else
				{
					totalNumberOfPages = this.itemCount / base.UserContext.UserOptions.BasicViewRowCount + 1;
				}
				if (renderPageNumbers)
				{
					ListView.RenderPageNumbers(base.Response.Output, this.pageNumber, totalNumberOfPages);
					base.Response.Write("<td>&nbsp;</td>");
					this.listView.RenderPagingControls(base.Response.Output, this.pageNumber, totalNumberOfPages);
					return;
				}
				this.listView.RenderHeaderPagingControls(base.Response.Output, this.pageNumber, totalNumberOfPages);
				return;
			}
		}

		protected void RenderInfobar()
		{
			if (base.Infobar.MessageCount > 0)
			{
				TextWriter output = base.Response.Output;
				output.Write("<tr id=trPwdIB><td class=vwinfbr>");
				base.Infobar.Render(output);
				output.Write("</td></tr>");
			}
		}

		internal static bool IsEditingMode(AddressBook.Mode addressBookMode)
		{
			return addressBookMode == AddressBook.Mode.EditMessage || addressBookMode == AddressBook.Mode.EditCalendar || addressBookMode == AddressBook.Mode.EditMeetingResponse;
		}

		internal const string ViewContextQueryStringParameter = "ctx";

		internal const string SearchStringQueryStringParameter = "sch";

		internal const string SearchLocationQueryStringParameter = "ab";

		internal const string ColumnSortOrderQueryStringParameter = "so";

		internal const string SortColumnIdQueryStringParameter = "cid";

		internal const string PageNumberQueryStringParameter = "pg";

		internal const string RecipientWellQueryStringParameter = "rw";

		internal const string ContactSearchLocation = "Con";

		internal const string ADSearchLocation = "Ad";

		internal const char SearchLocationDelimiter = ';';

		private const string CommandFormParameter = "hidcmdpst";

		private const string SortCommand = "s";

		private const string RecipientTypeQueryStringParameter = "rt";

		private const string RecipientCheckBox = "chkRcpt";

		private const string SerachLocationFormParameter = "hidAB";

		private const string MessageIdFormParameter = "hidid";

		private const string ColumnSortOrderFormParameter = "hidso";

		private const string SortColumnIdFormParameter = "hidcid";

		private const string RecipientIndexValue = "hidri";

		private const string SearchStringInFormParameter = "hidss";

		private const string PageNumberFromParameter = "hidpg";

		private const string ChangeKeyString = "hidchk";

		private const string RecipientWellString = "hidrw";

		private Folder folder;

		private Folder searchFolder;

		private ListView listView;

		private AddressBookBase addressBookBase;

		private ColumnId sortColumn = ColumnId.FileAs;

		private SortOrder sortOrder = SortOrder.Descending;

		private string addressBookToSearch;

		private string action;

		private string[] ids;

		private string[] addressBookInfo;

		private string searchString = LocalizedStrings.GetNonEncoded(-903656651);

		private bool isNewSearch = true;

		private AddressBook.Mode viewMode = AddressBook.Mode.Lookup;

		private RecipientItemType type = RecipientItemType.To;

		private RecipientItemType recipientWell = RecipientItemType.To;

		private int itemCount;

		private int pageNumber;

		private int firstItemOnPage;

		private int lastItemOnPage;

		private AddressBook.SearchLocation searchLocation;

		public enum Mode
		{
			None,
			Lookup,
			EditMessage,
			EditCalendar,
			EditMeetingResponse,
			LastMode = 4
		}

		private enum SearchLocation
		{
			AddressBook,
			Contacts
		}
	}
}
