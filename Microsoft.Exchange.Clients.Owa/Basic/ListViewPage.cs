using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.InfoWorker.Common.OOF;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	public abstract class ListViewPage : OwaPage
	{
		protected override bool UseStrictMode
		{
			get
			{
				return false;
			}
		}

		protected ListViewPage(Trace callTracer, Trace algorithmTracer)
		{
			this.callTracer = callTracer;
			this.algorithmTracer = algorithmTracer;
		}

		internal abstract StoreObjectId DefaultFolderId { get; }

		protected abstract SortOrder DefaultSortOrder { get; }

		protected abstract ColumnId DefaultSortedColumn { get; }

		protected abstract string CheckBoxId { get; }

		internal Folder Folder
		{
			get
			{
				return this.folder;
			}
			set
			{
				this.folder = value;
			}
		}

		internal StoreObjectId FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		public string FolderName
		{
			get
			{
				return this.folder.DisplayName;
			}
		}

		protected bool IsInDeletedItems
		{
			get
			{
				return Utilities.IsDefaultFolderId(base.UserContext.MailboxSession, this.FolderId, DefaultFolderType.DeletedItems);
			}
		}

		protected bool IsInJunkEmail
		{
			get
			{
				return Utilities.IsDefaultFolderId(base.UserContext.MailboxSession, this.FolderId, DefaultFolderType.JunkEmail);
			}
		}

		protected int FirstItemOnPage
		{
			get
			{
				return this.firstItemOnPage;
			}
			set
			{
				this.firstItemOnPage = value;
			}
		}

		protected int LastItemOnPage
		{
			get
			{
				return this.lastItemOnPage;
			}
			set
			{
				this.lastItemOnPage = value;
			}
		}

		protected ListView ListView
		{
			get
			{
				return this.listView;
			}
			set
			{
				this.listView = value;
			}
		}

		protected int ItemCount
		{
			get
			{
				return this.itemCount;
			}
			set
			{
				this.itemCount = value;
			}
		}

		protected int PageNumber
		{
			get
			{
				return this.pageNumber;
			}
			set
			{
				this.pageNumber = value;
			}
		}

		public bool FilteredView
		{
			get
			{
				return this.filteredView;
			}
			set
			{
				this.filteredView = value;
			}
		}

		protected Infobar Infobar
		{
			get
			{
				return this.infobar;
			}
		}

		internal Folder SearchFolder
		{
			get
			{
				return this.searchFolder;
			}
		}

		protected string SearchString
		{
			get
			{
				return this.searchString;
			}
		}

		internal SearchScope SearchScope
		{
			get
			{
				return this.searchScope;
			}
		}

		protected int SearchScopeInt
		{
			get
			{
				return (int)this.searchScope;
			}
		}

		protected SortOrder SortOrder
		{
			get
			{
				return this.sortOrder;
			}
		}

		protected ColumnId SortedColumn
		{
			get
			{
				return this.sortedColumn;
			}
		}

		protected abstract void CreateListView(ColumnId sortedColumn, SortOrder sortOrder);

		protected override void OnLoad(EventArgs e)
		{
			this.callTracer.TraceDebug((long)this.GetHashCode(), "ListViewPage.OnLoad");
			this.errorMessage = string.Empty;
			this.folderId = QueryStringUtilities.CreateFolderStoreObjectId(base.UserContext.MailboxSession, base.Request, false);
			if (this.folderId == null)
			{
				this.algorithmTracer.TraceDebug((long)this.GetHashCode(), "folderId is null, using default folder");
				this.folderId = this.DefaultFolderId;
			}
			else if (!Folder.IsFolderId(this.folderId))
			{
				throw new OwaInvalidRequestException("The given Id is not a valid folder Id. Folder Id:" + this.folderId);
			}
			bool newSearch = base.UserContext.ForceNewSearch;
			this.GetSearchStringAndScope();
			bool flag = false;
			ColumnId value = this.DefaultSortedColumn;
			SortOrder sortOrder = this.DefaultSortOrder;
			if (base.IsPostFromMyself())
			{
				string formParameter = Utilities.GetFormParameter(base.Request, "hidcmdpst", false);
				string key;
				if ((key = formParameter) != null)
				{
					if (<PrivateImplementationDetails>{912F2AED-BF68-4DDC-9379-4CB89AA1AA01}.$$method0x6000270-1 == null)
					{
						<PrivateImplementationDetails>{912F2AED-BF68-4DDC-9379-4CB89AA1AA01}.$$method0x6000270-1 = new Dictionary<string, int>(10)
						{
							{
								"s",
								0
							},
							{
								"delete",
								1
							},
							{
								"markread",
								2
							},
							{
								"markunread",
								3
							},
							{
								"emptyfolder",
								4
							},
							{
								"junk",
								5
							},
							{
								"notjunk",
								6
							},
							{
								"addjnkeml",
								7
							},
							{
								"hideoof",
								8
							},
							{
								"turnoffoof",
								9
							}
						};
					}
					int num;
					if (<PrivateImplementationDetails>{912F2AED-BF68-4DDC-9379-4CB89AA1AA01}.$$method0x6000270-1.TryGetValue(key, out num))
					{
						switch (num)
						{
						case 0:
							flag = true;
							value = (ColumnId)RequestParser.GetIntValueFromForm(base.Request, "hidcid");
							sortOrder = (SortOrder)RequestParser.GetIntValueFromForm(base.Request, "hidso");
							if (base.UserContext.IsWebPartRequest)
							{
								goto IL_4F2;
							}
							using (UserConfiguration folderConfiguration = UserConfigurationUtilities.GetFolderConfiguration("Owa.BasicFolderOption", base.UserContext, this.folderId))
							{
								if (folderConfiguration != null)
								{
									IDictionary dictionary = folderConfiguration.GetDictionary();
									dictionary["SortColumn"] = ColumnIdParser.GetString(value);
									dictionary["SortOrder"] = (int)sortOrder;
									try
									{
										folderConfiguration.Save();
									}
									catch (StoragePermanentException ex)
									{
										this.algorithmTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Failed to save configuration data. Error: {0}. Stack: {1}.", ex.Message, ex.StackTrace);
									}
									catch (StorageTransientException ex2)
									{
										this.algorithmTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Failed to save configuration data. Error: {0}. Stack: {1}.", ex2.Message, ex2.StackTrace);
									}
								}
								goto IL_4F2;
							}
							break;
						case 1:
							break;
						case 2:
							this.sourceIds = this.GetSelectedItems();
							Utilities.BasicMarkUserMailboxItemsAsRead(base.UserContext, this.sourceIds, this.GetJunkEmailStatus(), false);
							goto IL_4F2;
						case 3:
							this.sourceIds = this.GetSelectedItems();
							Utilities.BasicMarkUserMailboxItemsAsRead(base.UserContext, this.sourceIds, this.GetJunkEmailStatus(), true);
							goto IL_4F2;
						case 4:
							if (this.IsInDeletedItems || this.IsInJunkEmail)
							{
								base.UserContext.MailboxSession.DeleteAllObjects(DeleteItemFlags.SoftDelete, this.folderId);
							}
							if (this.filteredView)
							{
								newSearch = true;
								goto IL_4F2;
							}
							goto IL_4F2;
						case 5:
						{
							if (!base.UserContext.IsJunkEmailEnabled)
							{
								throw new OwaInvalidRequestException(LocalizedStrings.GetNonEncoded(552277155));
							}
							this.sourceIds = this.GetSelectedItems();
							InfobarMessage infobarMessage = JunkEmailHelper.MarkAsJunk(base.UserContext, this.sourceIds);
							if (infobarMessage != null)
							{
								this.infobar.AddMessage(infobarMessage);
							}
							if (this.filteredView)
							{
								newSearch = true;
								goto IL_4F2;
							}
							goto IL_4F2;
						}
						case 6:
						{
							if (!base.UserContext.IsJunkEmailEnabled)
							{
								throw new OwaInvalidRequestException(LocalizedStrings.GetNonEncoded(552277155));
							}
							this.sourceIds = this.GetSelectedItems();
							InfobarMessage infobarMessage2 = JunkEmailHelper.MarkAsNotJunk(base.UserContext, this.sourceIds);
							if (infobarMessage2 != null)
							{
								this.infobar.AddMessage(infobarMessage2);
							}
							if (this.filteredView)
							{
								newSearch = true;
								goto IL_4F2;
							}
							goto IL_4F2;
						}
						case 7:
						{
							if (!base.UserContext.IsJunkEmailEnabled)
							{
								throw new OwaInvalidRequestException(LocalizedStrings.GetNonEncoded(552277155));
							}
							InfobarMessage infobarMessage3 = JunkEmailHelper.AddEmailToSendersList(base.UserContext, base.Request);
							if (infobarMessage3 != null)
							{
								this.infobar.AddMessage(infobarMessage3);
								goto IL_4F2;
							}
							goto IL_4F2;
						}
						case 8:
							if (base.UserContext.IsWebPartRequest)
							{
								throw new OwaInvalidRequestException("Should not show out of office infobar in web part request");
							}
							RenderingFlags.HideOutOfOfficeInfoBar(base.UserContext, true);
							goto IL_4F2;
						case 9:
						{
							if (base.UserContext.IsWebPartRequest)
							{
								throw new OwaInvalidRequestException("Should not show out of office dialog in web part request");
							}
							UserOofSettings userOofSettings = UserOofSettings.GetUserOofSettings(base.UserContext.MailboxSession);
							userOofSettings.OofState = OofState.Disabled;
							userOofSettings.Save(base.UserContext.MailboxSession);
							goto IL_4F2;
						}
						default:
							goto IL_4F2;
						}
						this.sourceIds = this.GetSelectedItems();
						this.DeleteCalendarItems(this.sourceIds);
						if (this.IsInDeletedItems)
						{
							Utilities.DeleteItems(base.UserContext, DeleteItemFlags.SoftDelete, this.sourceIds);
						}
						else
						{
							Utilities.DeleteItems(base.UserContext, DeleteItemFlags.MoveToDeletedItems, this.sourceIds);
						}
						if (this.filteredView)
						{
							newSearch = true;
						}
					}
				}
			}
			IL_4F2:
			this.folder = Folder.Bind(base.UserContext.MailboxSession, this.folderId);
			this.sortOrder = this.DefaultSortOrder;
			this.sortedColumn = this.DefaultSortedColumn;
			if (base.UserContext.IsWebPartRequest)
			{
				if (!flag)
				{
					string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "view", false);
					WebPartModuleViewState webPartModuleViewState = base.UserContext.LastClientViewState as WebPartModuleViewState;
					if (string.IsNullOrEmpty(queryStringParameter) && webPartModuleViewState != null)
					{
						this.sortedColumn = webPartModuleViewState.SortedColumn;
						this.sortOrder = webPartModuleViewState.SortOrder;
					}
					else
					{
						WebPartListView webPartListView = WebPartUtilities.LookUpWebPartView(this.folder.Id.ObjectId.ObjectType, this.folder.ClassName, queryStringParameter);
						if (webPartListView != null)
						{
							if (webPartListView.ColumnId != null)
							{
								this.sortedColumn = (ColumnId)webPartListView.ColumnId.Value;
							}
							if (webPartListView.SortOrder != null)
							{
								this.sortOrder = (SortOrder)webPartListView.SortOrder.Value;
							}
						}
					}
				}
				else
				{
					this.sortedColumn = value;
					this.sortOrder = sortOrder;
				}
			}
			else
			{
				using (UserConfiguration folderConfiguration2 = UserConfigurationUtilities.GetFolderConfiguration("Owa.BasicFolderOption", base.UserContext, this.folder.Id))
				{
					if (folderConfiguration2 != null)
					{
						IDictionary dictionary2 = folderConfiguration2.GetDictionary();
						object obj = dictionary2["SortColumn"];
						if (obj != null)
						{
							this.sortedColumn = ColumnIdParser.Parse((string)obj);
						}
						if (this.sortedColumn == ColumnId.Count)
						{
							this.sortedColumn = this.DefaultSortedColumn;
						}
						obj = dictionary2["SortOrder"];
						if (obj != null)
						{
							this.sortOrder = (SortOrder)obj;
						}
					}
				}
			}
			if (this.algorithmTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				this.algorithmTracer.TraceDebug((long)this.GetHashCode(), "Creating ListView with sortedColumn={0}, sortOrder={1}, and folder: name={2} id={3}", new object[]
				{
					(int)this.sortedColumn,
					(int)this.sortOrder,
					this.folder.DisplayName,
					this.folder.Id.ToBase64String()
				});
			}
			if (!this.filteredView)
			{
				this.itemCount = this.Folder.ItemCount;
			}
			else
			{
				string queryStringParameter2 = Utilities.GetQueryStringParameter(base.Request, "newSch", false);
				if (!string.IsNullOrEmpty(queryStringParameter2) && string.CompareOrdinal(queryStringParameter2, "1") == 0)
				{
					newSearch = true;
				}
				FolderSearch folderSearch = new FolderSearch();
				this.searchFolder = folderSearch.Execute(base.UserContext, this.Folder, this.searchScope, this.searchString, newSearch, false);
				base.UserContext.ForceNewSearch = false;
				this.itemCount = this.searchFolder.ItemCount;
			}
			this.pageNumber = RequestParser.TryGetIntValueFromQueryString(base.Request, "pg", 1);
			if (this.pageNumber < 1)
			{
				this.pageNumber = 1;
			}
			if (this.itemCount <= 0)
			{
				this.firstItemOnPage = 1;
				this.lastItemOnPage = 1;
				this.pageNumber = 1;
				this.numberOfPages = 1;
			}
			else if (this.itemCount > 0)
			{
				this.numberOfPages = (this.itemCount - 1) / base.UserContext.UserOptions.BasicViewRowCount + 1;
				this.pageNumber = Math.Min(this.pageNumber, this.numberOfPages);
				this.firstItemOnPage = (this.pageNumber - 1) * base.UserContext.UserOptions.BasicViewRowCount + 1;
				this.lastItemOnPage = this.firstItemOnPage + base.UserContext.UserOptions.BasicViewRowCount - 1;
				this.firstItemOnPage = Math.Min(this.firstItemOnPage, this.itemCount);
				this.lastItemOnPage = Math.Min(this.lastItemOnPage, this.itemCount);
				this.firstItemOnPage = Math.Max(this.firstItemOnPage, 1);
				this.lastItemOnPage = Math.Max(this.lastItemOnPage, 1);
			}
			this.CreateListView(this.sortedColumn, this.sortOrder);
			if (this.FilteredView)
			{
				this.BuildSearchInfobarMessage();
			}
			base.OnLoad(e);
		}

		private JunkEmailStatus GetJunkEmailStatus()
		{
			if (!this.IsInJunkEmail)
			{
				return JunkEmailStatus.NotJunk;
			}
			if (this.filteredView)
			{
				return JunkEmailStatus.Unknown;
			}
			return JunkEmailStatus.Junk;
		}

		protected override void OnUnload(EventArgs e)
		{
			if (this.folder != null)
			{
				this.folder.Dispose();
			}
			if (this.searchFolder != null)
			{
				this.searchFolder.Dispose();
			}
		}

		protected void InitializeListView()
		{
			this.ListView.Initialize(this.FirstItemOnPage, this.LastItemOnPage);
		}

		protected void RenderListView()
		{
			this.listView.Render(base.Response.Output, this.errorMessage);
		}

		private void GetSearchStringAndScope()
		{
			this.searchString = Utilities.GetQueryStringParameter(base.Request, "sch", false);
			if (!string.IsNullOrEmpty(this.searchString))
			{
				this.searchString = this.searchString.Trim();
				if (this.searchString.Length > Globals.MaxSearchStringLength)
				{
					throw new OwaInvalidRequestException("Search string length is more than 256 characters");
				}
				Utilities.VerifySearchCanaryInGetRequest(base.Request);
				this.filteredView = !string.IsNullOrEmpty(this.searchString);
				this.searchScope = (SearchScope)RequestParser.TryGetIntValueFromQueryString(base.Request, "scp", (int)this.searchScope);
				if (this.searchScope != SearchScope.SelectedFolder && this.searchScope != SearchScope.SelectedAndSubfolders && this.searchScope != SearchScope.AllFoldersAndItems)
				{
					throw new OwaInvalidRequestException("Search scope is not supported");
				}
			}
		}

		public void RenderNavigation(NavigationModule navigationModule)
		{
			Navigation navigation = new Navigation(navigationModule, base.OwaContext, base.Response.Output);
			navigation.Render();
		}

		protected void RenderPaging(bool renderPageNumbers)
		{
			if (this.pageNumber < 1)
			{
				this.pageNumber = 1;
			}
			if (renderPageNumbers)
			{
				ListView.RenderPageNumbers(base.Response.Output, this.pageNumber, this.numberOfPages);
				base.Response.Write("<td>&nbsp;</td>");
				this.listView.RenderPagingControls(base.Response.Output, this.pageNumber, this.numberOfPages);
				return;
			}
			this.listView.RenderHeaderPagingControls(base.Response.Output, this.pageNumber, this.numberOfPages);
		}

		private StoreObjectId[] GetSelectedItems()
		{
			string formParameter = Utilities.GetFormParameter(base.Request, this.CheckBoxId);
			string[] array = formParameter.Split(new char[]
			{
				','
			});
			if (array.Length == 0)
			{
				throw new OwaInvalidRequestException("No item is selected");
			}
			StoreObjectId[] array2 = new StoreObjectId[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = Utilities.CreateStoreObjectId(base.UserContext.MailboxSession, array[i]);
			}
			if (base.UserContext.UserOptions.BasicViewRowCount < array2.Length)
			{
				throw new OwaInvalidOperationException(string.Format("This action is not supported for {0} item(s) in a single request", array2.Length));
			}
			return array2;
		}

		private void DeleteCalendarItems(StoreObjectId[] selectedItems)
		{
			if (selectedItems == null)
			{
				throw new ArgumentNullException("selectedItems");
			}
			string formParameter = Utilities.GetFormParameter(base.Request, "hidmtgmsg", false);
			if (formParameter != null)
			{
				for (int i = 0; i < selectedItems.Length; i++)
				{
					if (formParameter.IndexOf(selectedItems[i].ToBase64String(), StringComparison.Ordinal) != -1)
					{
						MeetingUtilities.DeleteMeetingMessageCalendarItem(selectedItems[i]);
					}
				}
			}
		}

		protected abstract SanitizedHtmlString BuildConcretSearchInfobarMessage(int resultsCount, SanitizedHtmlString clearSearchLink);

		protected void BuildSearchInfobarMessage()
		{
			SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
			sanitizingStringBuilder.Append("<a href=\"#\" onclick=\"return onClkClrLnk();\">");
			sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(1155007962));
			sanitizingStringBuilder.Append("</a>");
			if (this.ListView.TotalCount == 0)
			{
				this.Infobar.AddMessageHtml(SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(-761327948), new object[]
				{
					sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>()
				}), InfobarMessageType.Informational);
			}
			else
			{
				int num = this.ListView.TotalCount;
				object obj = this.SearchFolder.TryGetProperty(FolderSchema.SearchFolderItemCount);
				if (obj is int)
				{
					num = (int)obj;
				}
				this.Infobar.AddMessageHtml(this.BuildConcretSearchInfobarMessage(num, sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>()), InfobarMessageType.Informational);
				if (num > this.ListView.TotalCount)
				{
					this.Infobar.AddMessageLocalized(825345585, InfobarMessageType.Informational);
				}
			}
			if (!base.UserContext.MailboxSession.Mailbox.IsContentIndexingEnabled)
			{
				this.Infobar.AddMessageLocalized(-332074645, InfobarMessageType.Informational);
			}
		}

		internal const string PageNumberQueryStringParameter = "pg";

		internal const string FindAction = "Find";

		internal const string SearchStringQueryStringParameter = "sch";

		internal const string SearchScopeQueryStringParameter = "scp";

		private const string IsNewSearchQueryParameter = "newSch";

		private const string IsNewSearchQueryValue = "1";

		private const string CommandPostValue = "hidcmdpst";

		private const string SortColumnIdValue = "hidcid";

		private const string SortOrderValue = "hidso";

		private const string MeetingMessageIdFormValue = "hidmtgmsg";

		internal const string BasicFolderOptionConfigurationName = "Owa.BasicFolderOption";

		internal const string SortColumnKey = "SortColumn";

		internal const string SortOrderKey = "SortOrder";

		private Folder folder;

		private StoreObjectId folderId;

		private ListView listView;

		private int itemCount = -1;

		private int pageNumber = -1;

		private int firstItemOnPage;

		private int lastItemOnPage;

		private StoreObjectId[] sourceIds;

		private bool filteredView;

		private string searchString = string.Empty;

		private SearchScope searchScope;

		private Infobar infobar = new Infobar();

		private Folder searchFolder;

		private string errorMessage = string.Empty;

		private Trace callTracer;

		private Trace algorithmTracer;

		private int numberOfPages = -1;

		private ColumnId sortedColumn;

		private SortOrder sortOrder;
	}
}
