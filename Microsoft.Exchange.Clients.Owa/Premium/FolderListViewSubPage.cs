using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public abstract class FolderListViewSubPage : ListViewSubPage
	{
		protected static int StoreObjectTypeMessage
		{
			get
			{
				return 9;
			}
		}

		protected FolderListViewSubPage(Trace callTracer, Trace algorithmTracer) : base(callTracer, algorithmTracer)
		{
		}

		protected bool IsFilteredViewInFavorites
		{
			get
			{
				return this.favoritesFilterParameter != null;
			}
		}

		internal Folder Folder
		{
			get
			{
				return this.folder;
			}
		}

		internal DefaultFolderType FolderType
		{
			get
			{
				if (this.folderType == null)
				{
					if (this.favoritesFilterParameter != null)
					{
						this.folderType = new DefaultFolderType?(Utilities.GetDefaultFolderType(this.Folder.Session, this.favoritesFilterParameter.SourceFolderId.StoreObjectId));
					}
					else
					{
						this.folderType = new DefaultFolderType?(Utilities.GetDefaultFolderType(this.folder));
					}
				}
				return this.folderType.Value;
			}
		}

		protected bool IsInDeleteItems
		{
			get
			{
				return Utilities.IsDefaultFolder(this.Folder, DefaultFolderType.DeletedItems);
			}
		}

		protected bool IsDeletedItemsSubFolder
		{
			get
			{
				return Utilities.IsItemInDefaultFolder(this.Folder, DefaultFolderType.DeletedItems);
			}
		}

		internal abstract StoreObjectId DefaultFolderId { get; }

		protected override int ViewWidth
		{
			get
			{
				return Math.Max(this.viewWidth, 325);
			}
		}

		protected override int ListViewTop
		{
			get
			{
				int num = 31;
				if (this.ShouldRenderSearch && !this.IsPublicFolder)
				{
					num = 60;
				}
				if (!this.ShouldRenderToolbar)
				{
					num -= this.ToolbarHeight;
				}
				return num;
			}
		}

		protected override int ViewHeight
		{
			get
			{
				return this.viewHeight;
			}
		}

		protected override SortOrder SortOrder
		{
			get
			{
				return this.sortOrder;
			}
		}

		protected override ColumnId SortedColumn
		{
			get
			{
				return this.sortedColumn;
			}
		}

		protected override ReadingPanePosition ReadingPanePosition
		{
			get
			{
				return this.readingPanePosition;
			}
		}

		protected override bool IsMultiLine
		{
			get
			{
				return this.isMultiLine;
			}
		}

		protected override bool ShouldRenderSearch
		{
			get
			{
				return !base.UserContext.IsWebPartRequest;
			}
		}

		protected virtual bool ShouldRenderELCCommentAndQuotaLink
		{
			get
			{
				return true;
			}
		}

		protected override bool AllowAdvancedSearch
		{
			get
			{
				return this.IsPublicFolder || this.IsOtherMailboxFolder || ((MailboxSession)this.Folder.Session).Mailbox.IsContentIndexingEnabled;
			}
		}

		protected string ELCFolderIdValue
		{
			get
			{
				if (this.elcFolderIdValue == null)
				{
					return string.Empty;
				}
				return Utilities.JavascriptEncode(this.elcFolderIdValue);
			}
		}

		protected long ELCFolderQuota
		{
			get
			{
				return this.elcFolderQuota;
			}
		}

		protected bool IsELCFolderWithQuota
		{
			get
			{
				return this.isELCFolderWithQuota;
			}
		}

		protected bool HasELCComment
		{
			get
			{
				return this.elcFolderComment != null && !string.IsNullOrEmpty(this.elcFolderComment.Trim());
			}
		}

		protected bool ShouldRenderELCInfobar
		{
			get
			{
				return this.HasELCComment || this.IsELCFolderWithQuota;
			}
		}

		protected bool IsELCInfobarVisible
		{
			get
			{
				return this.elcShowComment;
			}
		}

		protected override string ContainerName
		{
			get
			{
				if (!this.IsPublicFolder && !this.IsOtherMailboxFolder && this.folder.Id.ObjectId.Equals(base.UserContext.GetRootFolderId((MailboxSession)this.Folder.Session)))
				{
					return Utilities.GetMailboxOwnerDisplayName((MailboxSession)this.Folder.Session);
				}
				if (this.IsPublicFolder && base.UserContext.IsPublicFolderRootId(this.folder.Id.ObjectId))
				{
					return LocalizedStrings.GetNonEncoded(-1116491328);
				}
				if (this.IsOtherMailboxFolder)
				{
					return Utilities.GetFolderNameWithSessionName(this.folder);
				}
				return this.folder.DisplayName;
			}
		}

		protected bool IsPublicFolder
		{
			get
			{
				if (this.isPublicFolder == null)
				{
					this.isPublicFolder = new bool?(Utilities.IsPublic(this.Folder));
				}
				return this.isPublicFolder.Value;
			}
		}

		protected bool IsArchiveMailboxFolder
		{
			get
			{
				if (this.isArchiveMailboxFolder == null)
				{
					this.isArchiveMailboxFolder = new bool?(Utilities.IsInArchiveMailbox(this.Folder));
				}
				return this.isArchiveMailboxFolder.Value;
			}
		}

		protected bool IsOtherMailboxFolder
		{
			get
			{
				if (this.isOtherMailboxFolder == null)
				{
					this.isOtherMailboxFolder = new bool?(base.UserContext.IsInOtherMailbox(this.Folder) || Utilities.IsWebPartDelegateAccessRequest(OwaContext.Current));
				}
				return this.isOtherMailboxFolder.Value;
			}
		}

		protected int FolderEffectiveRights
		{
			get
			{
				return (int)Utilities.GetFolderProperty<EffectiveRights>(this.folder, StoreObjectSchema.EffectiveRights, EffectiveRights.None);
			}
		}

		protected override void LoadViewState()
		{
			OwaStoreObjectId owaStoreObjectId = null;
			if (base.SerializedContainerId != null)
			{
				if (OwaStoreObjectId.IsDummyArchiveFolder(base.SerializedContainerId))
				{
					owaStoreObjectId = base.UserContext.GetArchiveRootFolderId();
					this.archiveRootFolderId = owaStoreObjectId.ToString();
				}
				else
				{
					owaStoreObjectId = OwaStoreObjectId.CreateFromString(base.SerializedContainerId);
				}
			}
			if (owaStoreObjectId == null)
			{
				base.AlgorithmTracer.TraceDebug((long)this.GetHashCode(), "folder Id is null, using default folder");
				owaStoreObjectId = OwaStoreObjectId.CreateFromMailboxFolderId(this.DefaultFolderId);
			}
			PropertyDefinition[] array = new PropertyDefinition[]
			{
				FolderSchema.DisplayName,
				FolderSchema.ItemCount,
				FolderSchema.UnreadCount,
				ViewStateProperties.ReadingPanePosition,
				ViewStateProperties.ViewWidth,
				ViewStateProperties.ViewHeight,
				ViewStateProperties.MultiLine,
				ViewStateProperties.SortColumn,
				ViewStateProperties.SortOrder,
				ViewStateProperties.ViewFilter,
				ViewStateProperties.FilteredViewLabel,
				FolderSchema.SearchFolderAllowAgeout,
				FolderSchema.IsOutlookSearchFolder,
				FolderSchema.AdminFolderFlags,
				FolderSchema.FolderQuota,
				FolderSchema.FolderSize,
				FolderSchema.ELCFolderComment,
				FolderSchema.ELCPolicyIds,
				FolderSchema.ExtendedFolderFlags,
				StoreObjectSchema.EffectiveRights,
				FolderSchema.OutlookSearchFolderClsId
			};
			this.folder = Utilities.GetFolderForContent<Folder>(base.UserContext, owaStoreObjectId, array);
			this.favoritesFilterParameter = Utilities.GetFavoritesFilterViewParameter(base.UserContext, this.Folder);
			if (this.folder is SearchFolder && this.favoritesFilterParameter != null && !this.favoritesFilterParameter.IsCurrentVersion)
			{
				this.favoritesFilterParameter.UpgradeFilter(this.folder as SearchFolder, array);
			}
			this.sortOrder = this.DefaultSortOrder;
			this.sortedColumn = this.DefaultSortedColumn;
			this.isMultiLine = this.DefaultMultiLineSetting;
			this.readingPanePosition = this.DefaultReadingPanePosition;
			FolderViewStates folderViewStates = base.UserContext.GetFolderViewStates(this.folder);
			if (base.UserContext.IsWebPartRequest)
			{
				string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "view", false);
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
					if (webPartListView.IsMultiLine != null)
					{
						this.isMultiLine = webPartListView.IsMultiLine.Value;
					}
				}
			}
			else
			{
				this.viewWidth = folderViewStates.ViewWidth;
				this.viewHeight = folderViewStates.ViewHeight;
				this.sortOrder = folderViewStates.GetSortOrder(this.DefaultSortOrder);
				this.isMultiLine = folderViewStates.GetMultiLine(this.DefaultMultiLineSetting);
				string sortColumn = folderViewStates.GetSortColumn(null);
				if (sortColumn != null)
				{
					ColumnId columnId = ColumnIdParser.Parse(sortColumn);
					if (columnId < ColumnId.Count && (!this.isMultiLine || ListViewColumns.GetColumn(columnId).SortBoundaries != null))
					{
						this.sortedColumn = columnId;
					}
				}
			}
			if (ConversationUtilities.IsConversationSortColumn(this.sortedColumn) && !ConversationUtilities.ShouldAllowConversationView(base.UserContext, this.Folder))
			{
				this.sortedColumn = ColumnId.DeliveryTime;
			}
			this.readingPanePosition = folderViewStates.GetReadingPanePosition(this.DefaultReadingPanePosition);
			this.LoadELCData();
		}

		protected override IListViewDataSource CreateDataSource(ListView listView)
		{
			SortBy[] sortByProperties = listView.GetSortByProperties();
			return new FolderListViewDataSource(base.UserContext, listView.Properties, this.folder, sortByProperties);
		}

		protected override void OnUnload(EventArgs e)
		{
			if (this.folder != null)
			{
				this.folder.Dispose();
				this.folder = null;
			}
		}

		protected override OwaQueryStringParameters GetDefaultItemParameters()
		{
			IListViewDataSource dataSource = ((VirtualListView2)this.listView).DataSource;
			if (dataSource.RangeCount < 1 || Utilities.IsPublic(this.Folder))
			{
				return null;
			}
			OwaQueryStringParameters owaQueryStringParameters = new OwaQueryStringParameters();
			dataSource.MoveToItem(0);
			bool flag = Utilities.IsDefaultFolder(this.Folder, DefaultFolderType.JunkEmail);
			int itemProperty = dataSource.GetItemProperty<int>(ItemSchema.EdgePcl, 1);
			bool itemProperty2 = dataSource.GetItemProperty<bool>(ItemSchema.LinkEnabled, false);
			bool flag2 = JunkEmailUtilities.IsSuspectedPhishingItem(itemProperty) && !itemProperty2;
			string itemClass = dataSource.GetItemClass();
			if (ObjectClass.IsOfClass(itemClass, "IPM.Sharing") && !flag && !flag2)
			{
				owaQueryStringParameters.SetApplicationElement("PreFormAction");
				owaQueryStringParameters.Action = "Preview";
			}
			else
			{
				owaQueryStringParameters.SetApplicationElement("Item");
				owaQueryStringParameters.Action = "Preview";
			}
			if ((!flag && !flag2) || ObjectClass.IsOfClass(itemClass, "IPM.Conversation"))
			{
				bool itemProperty3 = dataSource.GetItemProperty<bool>(MessageItemSchema.IsDraft, false);
				bool itemProperty4 = dataSource.GetItemProperty<bool>(MessageItemSchema.HasBeenSubmitted, false);
				TaskType itemProperty5 = (TaskType)dataSource.GetItemProperty<int>(TaskSchema.TaskType, 0);
				bool flag3 = TaskUtilities.IsAssignedTaskType(itemProperty5);
				owaQueryStringParameters.ItemClass = itemClass;
				if ((itemProperty3 && !itemProperty4) || ObjectClass.IsContact(itemClass) || ObjectClass.IsDistributionList(itemClass))
				{
					owaQueryStringParameters.State = "Draft";
				}
				else if (ObjectClass.IsTask(itemClass) && flag3)
				{
					owaQueryStringParameters.State = "Assigned";
				}
			}
			owaQueryStringParameters.Id = dataSource.GetItemId();
			return owaQueryStringParameters;
		}

		protected override void RenderSearch()
		{
			base.RenderSearch(this.folder);
		}

		protected void SetSortOrder(SortOrder sortOrder)
		{
			this.sortOrder = sortOrder;
		}

		private void LoadELCData()
		{
			if (this.folder is SearchFolder)
			{
				return;
			}
			this.elcFolderIdValue = Utilities.GetQueryStringParameter(base.Request, "elcId", false);
			if (string.IsNullOrEmpty(this.elcFolderIdValue))
			{
				this.GetELCFolderData(this.folder);
			}
			else
			{
				StoreObjectId folderId = Utilities.CreateStoreObjectId(base.UserContext.MailboxSession, this.elcFolderIdValue);
				using (Folder folder = Folder.Bind(base.UserContext.MailboxSession, folderId, new PropertyDefinition[]
				{
					FolderSchema.AdminFolderFlags,
					FolderSchema.FolderQuota,
					FolderSchema.FolderSize,
					FolderSchema.ELCFolderComment,
					FolderSchema.ELCPolicyIds
				}))
				{
					this.GetELCFolderData(folder);
				}
			}
			if ((this.elcAdminFolderFlags & 8) > 0 && this.elcFolderQuota > 0L)
			{
				this.isELCFolderWithQuota = true;
			}
			this.elcDisplayName = this.GetFolderDisplayName();
			this.SetElcShowComment();
		}

		private void GetELCFolderData(Folder folder)
		{
			this.elcAdminFolderFlags = 0;
			this.elcFolderComment = string.Empty;
			this.elcMustDisplayComment = false;
			this.elcFolderQuota = 0L;
			this.elcFolderSize = 0L;
			this.elcPolicyIds = string.Empty;
			object obj = folder.TryGetProperty(FolderSchema.AdminFolderFlags);
			if (!(obj is PropertyError))
			{
				this.elcAdminFolderFlags = (int)obj;
			}
			obj = folder.TryGetProperty(FolderSchema.ELCPolicyIds);
			if (!(obj is PropertyError))
			{
				this.elcPolicyIds = (string)obj;
			}
			if (Utilities.IsELCFolder(this.elcAdminFolderFlags) || (Utilities.IsSpecialFolder(folder.Id.ObjectId, base.UserContext) && !string.IsNullOrEmpty(this.elcPolicyIds)))
			{
				this.elcMustDisplayComment = ((this.elcAdminFolderFlags & 4) > 0);
				obj = folder.TryGetProperty(FolderSchema.ELCFolderComment);
				if (!(obj is PropertyError))
				{
					this.elcFolderComment = (string)obj;
				}
			}
			else if (!Utilities.IsPublic(folder))
			{
				this.elcMustDisplayComment = base.UserContext.AllFoldersPolicyMustDisplayComment;
				this.elcFolderComment = base.UserContext.AllFoldersPolicyComment;
			}
			if ((this.elcAdminFolderFlags & 8) > 0)
			{
				obj = folder.TryGetProperty(FolderSchema.FolderQuota);
				if (!(obj is PropertyError))
				{
					this.elcFolderQuota = (long)((int)obj) * 1024L;
				}
				obj = folder.TryGetProperty(FolderSchema.FolderSize);
				if (!(obj is PropertyError))
				{
					this.elcFolderSize = (long)((int)obj);
				}
			}
		}

		private string GetFolderDisplayName()
		{
			return this.ContainerName;
		}

		private void SetElcShowComment()
		{
			object obj = this.folder.TryGetProperty(FolderSchema.ExtendedFolderFlags);
			this.elcShowComment = true;
			if (!(obj is PropertyError) && Utilities.IsFlagSet((int)obj, 32))
			{
				this.elcShowComment = false;
			}
		}

		private bool IsOutlookSearchFolder()
		{
			bool result = false;
			object obj = this.folder.TryGetProperty(FolderSchema.IsOutlookSearchFolder);
			if (!(obj is PropertyError))
			{
				result = (bool)obj;
			}
			return result;
		}

		protected void RenderJavascriptEncodedLegacyDN()
		{
			Utilities.JavascriptEncode(base.UserContext.ExchangePrincipal.LegacyDn, base.Response.Output);
		}

		protected string RenderELCComment()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<div class=\"divElcComponent\" id=\"divElcComment\">");
			stringBuilder.Append("<div class=\"divIBTxt\">");
			stringBuilder.Append(Utilities.HtmlEncode(this.elcDisplayName));
			stringBuilder.Append(": ");
			if (this.HasELCComment)
			{
				stringBuilder.Append(Utilities.HtmlEncode(this.elcFolderComment));
			}
			if (!this.elcMustDisplayComment)
			{
				stringBuilder.Append("</div><div class=\"divIBTxt\"><a href=# id=\"lnkHdELC\">");
				stringBuilder.Append(LocalizedStrings.GetHtmlEncoded(1303059585) + "</a>");
			}
			stringBuilder.Append("</div></div>");
			return stringBuilder.ToString();
		}

		protected string RenderELCQuota()
		{
			if (!this.isELCFolderWithQuota)
			{
				return string.Empty;
			}
			QuotaLevel quotaLevel = QuotaLevel.Normal;
			int num = (int)Math.Round((double)this.elcFolderSize / (double)this.elcFolderQuota * 100.0);
			if (num >= 100)
			{
				quotaLevel = QuotaLevel.Exceeded;
			}
			else if (num >= 75)
			{
				quotaLevel = QuotaLevel.AboveWarning;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<div class=\"divElcComponent\" id=\"divElcQuota\">");
			stringBuilder.Append("<div class=\"divIBTxt\" id=\"divQuotaBar\">");
			StringBuilder stringBuilder2 = new StringBuilder();
			using (StringWriter stringWriter = new StringWriter(stringBuilder2))
			{
				RenderingUtilities.RenderQuotaBar(stringWriter, base.UserContext, num, quotaLevel);
			}
			stringBuilder.Append(stringBuilder2.ToString());
			stringBuilder.Append("</div> ");
			stringBuilder.Append("<div class=\"divIBTxt\" id=\"divFldUsg\" ");
			stringBuilder.Append((num < 100) ? string.Empty : "style=\"display:none;\" ");
			stringBuilder.Append(">");
			stringBuilder2 = new StringBuilder();
			using (StringWriter stringWriter2 = new StringWriter(stringBuilder2))
			{
				Utilities.RenderSizeWithUnits(stringWriter2, this.elcFolderQuota, false);
			}
			stringBuilder.AppendFormat(LocalizedStrings.GetHtmlEncoded(-659755432), "<span id=spnFldPrcntUsd>" + num + "</span>", stringBuilder2.ToString() + "</span>");
			stringBuilder.Append("</div> <div class=\"divIBTxt\" id=\"divFldExcd\" ");
			stringBuilder.Append((num < 100) ? "style=\"display:none;\" " : string.Empty);
			stringBuilder.Append(">");
			stringBuilder.Append(LocalizedStrings.GetHtmlEncoded(231890609));
			stringBuilder.Append("</div></div>");
			return stringBuilder.ToString();
		}

		protected void RenderJavascriptEncodedFolderId()
		{
			Utilities.JavascriptEncode(Utilities.GetIdAsString(this.Folder), base.Response.Output);
		}

		protected void RenderJavascriptEncodedContainerName()
		{
			Utilities.JavascriptEncode(this.ContainerName, base.Response.Output);
		}

		protected void RenderJavascriptEncodedContainerMetadataString()
		{
			base.Response.Output.Write(LocalizedStrings.GetJavascriptEncoded(-648401288));
		}

		protected void RenderJavascriptEncodedFlaggedItemsAndTasksFolderId()
		{
			Utilities.JavascriptEncode(base.UserContext.FlaggedItemsAndTasksFolderId.ToBase64String(), base.Response.Output);
		}

		protected void RenderReplaceFolderId()
		{
			if (!string.IsNullOrEmpty(this.archiveRootFolderId))
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("<div id=\"divReplaceFolderId\" style=\"display:none\" oldfolderid =\"f");
				stringBuilder.Append(Utilities.HtmlEncode(base.SerializedContainerId));
				stringBuilder.Append("\" newfolderid =\"f");
				stringBuilder.Append(Utilities.HtmlEncode(this.archiveRootFolderId));
				stringBuilder.Append("\"></div>");
				base.Response.Write(stringBuilder.ToString());
				NavigationHost.RenderFavoritesAndNavigationTrees(base.Writer, base.UserContext, null, new NavigationNodeGroupSection[]
				{
					NavigationNodeGroupSection.First
				});
			}
		}

		private const string ElcFolderIdQueryParameter = "elcId";

		private const int ElcFolderQuotaWarningPercentage = 75;

		private const int ElcFolderQuotaMaximumPercentage = 100;

		private Folder folder;

		private bool isMultiLine = true;

		private bool? isPublicFolder;

		private bool? isArchiveMailboxFolder;

		private bool? isOtherMailboxFolder;

		private DefaultFolderType? folderType;

		private ReadingPanePosition readingPanePosition;

		protected int viewWidth = 450;

		private int viewHeight = 250;

		private ColumnId sortedColumn;

		private SortOrder sortOrder;

		private FolderVirtualListViewFilter favoritesFilterParameter;

		private string elcFolderIdValue;

		private string elcDisplayName;

		private int elcAdminFolderFlags;

		private string elcFolderComment;

		private bool elcMustDisplayComment;

		private long elcFolderSize;

		private long elcFolderQuota;

		private string elcPolicyIds;

		private bool elcShowComment;

		private bool isELCFolderWithQuota;

		private string archiveRootFolderId;
	}
}
