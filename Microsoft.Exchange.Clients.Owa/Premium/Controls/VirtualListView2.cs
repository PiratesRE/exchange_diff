using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal abstract class VirtualListView2 : IListView
	{
		public VirtualListView2(UserContext userContext, string id, bool isMultiLine, ColumnId sortedColumn, SortOrder sortOrder) : this(userContext, id, isMultiLine, sortedColumn, sortOrder, false)
		{
			this.userContext = userContext;
			this.id = id;
			this.sortedColumn = sortedColumn;
			this.sortOrder = sortOrder;
		}

		public VirtualListView2(UserContext userContext, string id, bool isMultiLine, ColumnId sortedColumn, SortOrder sortOrder, bool isFiltered)
		{
			this.userContext = userContext;
			this.id = id;
			this.sortedColumn = sortedColumn;
			this.sortOrder = sortOrder;
			this.isFiltered = isFiltered;
		}

		protected virtual bool IsInSearch
		{
			get
			{
				return this.IsFiltered;
			}
		}

		protected abstract Folder DataFolder { get; }

		public abstract ViewType ViewType { get; }

		public abstract string OehNamespace { get; }

		public int TotalCount
		{
			get
			{
				return this.Contents.DataSource.TotalCount;
			}
		}

		public IListViewDataSource DataSource
		{
			get
			{
				return this.contents.DataSource;
			}
		}

		protected virtual bool IsMultiLine
		{
			get
			{
				return true;
			}
		}

		protected ColumnId SortedColumn
		{
			get
			{
				return this.sortedColumn;
			}
		}

		protected SortOrder SortOrder
		{
			get
			{
				return this.sortOrder;
			}
		}

		protected bool IsFiltered
		{
			get
			{
				return this.isFiltered;
			}
		}

		protected UserContext UserContext
		{
			get
			{
				return this.userContext;
			}
		}

		protected ListViewContents2 Contents
		{
			get
			{
				return this.contents;
			}
		}

		protected Strings.IDs EmptyViewStringId
		{
			get
			{
				if (this.IsSearchInProgress())
				{
					return -1057914178;
				}
				if (this.IsFiltered)
				{
					return 417836457;
				}
				return -474826895;
			}
		}

		protected virtual bool ShouldSubscribeForFolderContentChanges
		{
			get
			{
				return false;
			}
		}

		private bool IsSearchInProgress()
		{
			bool result = false;
			FolderListViewDataSource folderListViewDataSource = this.contents.DataSource as FolderListViewDataSource;
			if (folderListViewDataSource != null && folderListViewDataSource.IsSearchInProgress())
			{
				result = true;
			}
			return result;
		}

		public virtual bool GetDidLastSearchFail()
		{
			if (!this.IsInSearch)
			{
				return false;
			}
			if (this.didLastSearchFail != null)
			{
				return this.didLastSearchFail.Value;
			}
			SearchFolder searchFolder = this.DataFolder as SearchFolder;
			if (searchFolder != null && this.UserContext.IsPushNotificationsEnabled)
			{
				bool flag2;
				bool flag = this.UserContext.MapiNotificationManager.HasCurrentSearchCompleted((MailboxSession)searchFolder.Session, searchFolder.StoreObjectId, out flag2);
				if (flag && flag2)
				{
					this.didLastSearchFail = new bool?((searchFolder.GetSearchCriteria().SearchState & SearchState.Failed) != SearchState.None);
				}
				if (this.didLastSearchFail != null && this.didLastSearchFail == true)
				{
					SearchPerformanceData searchPerformanceData = this.UserContext.MapiNotificationManager.GetSearchPerformanceData((MailboxSession)searchFolder.Session);
					searchPerformanceData.SearchFailed();
				}
			}
			return this.didLastSearchFail != null && this.didLastSearchFail.Value;
		}

		public void Render(TextWriter writer)
		{
			this.OnBeforeRender();
			writer.Write("<div");
			VirtualListView2.RenderAttribute(writer, "id", this.id);
			VirtualListView2.RenderAttribute(writer, "class", "absFill");
			VirtualListView2.RenderAttribute(writer, "iT", (int)this.ViewType);
			VirtualListView2.RenderAttribute(writer, "sEvtNS", this.OehNamespace);
			VirtualListView2.RenderAttribute(writer, "sSid", this.contents.DataSource.ContainerId);
			VirtualListView2.RenderAttribute(writer, "fML", this.IsMultiLine ? 1 : 0);
			VirtualListView2.RenderAttribute(writer, "iSC", (int)this.sortedColumn);
			VirtualListView2.RenderAttribute(writer, "iSO", (int)this.sortOrder);
			VirtualListView2.RenderAttribute(writer, "iTC", this.contents.DataSource.TotalCount);
			VirtualListView2.RenderAttribute(writer, "iTIC", this.contents.DataSource.TotalItemCount);
			VirtualListView2.RenderAttribute(writer, "iNsDir", (int)this.userContext.UserOptions.NextSelection);
			VirtualListView2.RenderAttribute(writer, "sPbPrps", this.publicProperties.ToString());
			VirtualListView2.RenderAttribute(writer, "fTD", ListViewColumns.GetColumn(this.sortedColumn).IsTypeDownCapable ? 1 : 0);
			VirtualListView2.RenderAttribute(writer, "L_BigSel", LocalizedStrings.GetNonEncoded(719114324));
			VirtualListView2.RenderAttribute(writer, "L_Ldng", LocalizedStrings.GetNonEncoded(-695375226));
			VirtualListView2.RenderAttribute(writer, "L_Srchng", LocalizedStrings.GetNonEncoded(-1057914178));
			VirtualListView2.RenderAttribute(writer, "L_Fltrng", LocalizedStrings.GetNonEncoded(320310349));
			foreach (string text in this.extraAttributes.Keys)
			{
				VirtualListView2.RenderAttribute(writer, text, this.extraAttributes[text]);
			}
			writer.Write(">");
			this.RenderHeaders(writer);
			writer.Write("<a href=\"#\" id=\"linkVLV\" class=\"offscreen\">&nbsp;</a>");
			writer.Write("<div id=\"divHeaderSpacer\">&nbsp;</div>");
			if (this.HasInlineControl)
			{
				this.RenderInlineControl(writer);
			}
			writer.Write("<div id=\"divList\"");
			this.RenderListViewClasses(writer);
			writer.Write(">");
			writer.Write("<div id=\"divViewport\" draggable=\"true\">");
			this.RenderChunk(writer);
			writer.Write("</div>");
			writer.Write("<div id=\"divScrollbar\"><div id=\"divScrollRegion\"></div></div>");
			writer.Write("</div>");
			writer.Write("<div id=\"divPrgBg\" style=\"display:none\"></div><div id=\"divPrgrs\" style=\"display:none\"><img src=\"");
			this.userContext.RenderThemeFileUrl(writer, ThemeFileId.ProgressSmall);
			writer.Write("\"><span id=\"spnTxt\"></span></div>");
			writer.Write("</div>");
		}

		public void RenderForCompactWebPart(TextWriter writer)
		{
			this.AddAttribute("iWP", "1");
			this.Render(writer);
		}

		public virtual void LoadData(int startRange, int rowCount)
		{
			this.contents = this.CreateListViewContents();
			if (this.GetDidLastSearchFail())
			{
				this.contents.DataSource = new FolderListViewEmptyDataSource(this.DataFolder, this.contents.Properties);
			}
			else
			{
				this.contents.DataSource = this.CreateDataSource(this.contents.Properties);
			}
			this.contents.DataSource.Load(startRange, rowCount);
			this.SubscribeForFolderContentChanges();
		}

		public virtual void LoadData(ObjectId seekRowId, SeekDirection seekDirection, int rowCount)
		{
			this.contents = this.CreateListViewContents();
			this.contents.DataSource = this.CreateDataSource(this.contents.Properties);
			this.contents.DataSource.Load(seekRowId, seekDirection, rowCount);
			this.SubscribeForFolderContentChanges();
		}

		public virtual void LoadData(string seekValue, int rowCount)
		{
			this.contents = this.CreateListViewContents();
			this.contents.DataSource = this.CreateDataSource(this.contents.Properties);
			this.contents.DataSource.Load(seekValue, rowCount);
			this.SubscribeForFolderContentChanges();
		}

		public virtual void LoadAdjacent(ObjectId adjacentId, SeekDirection seekDirection, int rowCount)
		{
			this.contents = this.CreateListViewContents();
			this.contents.DataSource = this.CreateDataSource(this.contents.Properties);
			if (!this.contents.DataSource.LoadAdjacent(adjacentId, seekDirection, rowCount))
			{
				this.forceRefresh = true;
			}
			this.SubscribeForFolderContentChanges();
		}

		public void RenderChunk(TextWriter writer)
		{
			if (this.contents == null)
			{
				throw new OwaInvalidOperationException("LoadData must be called before calling RenderChunk");
			}
			if (this.GetDidLastSearchFail())
			{
				this.InternalRenderChunk(writer, true, "fNoRws", new Strings.IDs?(1073923836));
			}
			if (!this.contents.DataSource.UserHasRightToLoad)
			{
				this.InternalRenderChunk(writer, true, "fNoRws", new Strings.IDs?(-593658721));
				return;
			}
			if (this.contents.DataSource.TotalCount == 0)
			{
				this.InternalRenderChunk(writer, true, "fNoRws", new Strings.IDs?(this.EmptyViewStringId));
				return;
			}
			if (this.contents.DataSource.RangeCount == 0)
			{
				this.InternalRenderChunk(writer, true, "fNull", null);
				return;
			}
			this.InternalRenderChunk(writer, false, null, null);
		}

		public void RenderData(TextWriter writer)
		{
			writer.Write("<div id=\"data\"");
			this.InternalRenderData(writer);
			writer.Write("></div>");
		}

		public void RenderHeaders(TextWriter writer)
		{
			ListViewHeaders2 listViewHeaders = new ArrangeByHeaders2(this.SortedColumn, this.SortOrder, this.userContext);
			writer.Write("<div id=\"divHeaders\">");
			listViewHeaders.Render(writer);
			writer.Write("</div>");
		}

		public virtual void RenderListViewClasses(TextWriter writer)
		{
		}

		public void AddAttribute(string name, string value)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("name is null or empty.");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.extraAttributes.Add(name, value);
		}

		protected static void RenderAttribute(TextWriter writer, string name, string value)
		{
			writer.Write(" ");
			writer.Write(name);
			writer.Write("=\"");
			Utilities.HtmlEncode(value, writer);
			writer.Write("\"");
		}

		protected static void RenderAttribute(TextWriter writer, string name, int value)
		{
			writer.Write(" ");
			writer.Write(name);
			writer.Write("=");
			writer.Write(value);
		}

		protected abstract ListViewContents2 CreateListViewContents();

		protected ListViewContents2 CreateGroupByList(ListViewContents2 originalContents)
		{
			Column column = ListViewColumns.GetColumn(this.SortedColumn);
			if (column.GroupType == GroupType.Expanded)
			{
				return new GroupByList2(this.sortedColumn, this.sortOrder, (ItemList2)originalContents, this.userContext);
			}
			return originalContents;
		}

		protected abstract IListViewDataSource CreateDataSource(Hashtable properties);

		protected virtual void OnBeforeRender()
		{
		}

		protected void MakePropertyPublic(string attribute)
		{
			if (this.publicProperties.Length > 0)
			{
				this.publicProperties.Append(";");
			}
			this.publicProperties.Append(attribute);
		}

		protected virtual void InternalRenderData(TextWriter writer)
		{
			VirtualListView2.RenderAttribute(writer, "iTC", this.contents.DataSource.TotalCount);
			VirtualListView2.RenderAttribute(writer, "iTIC", this.contents.DataSource.TotalItemCount);
			VirtualListView2.RenderAttribute(writer, "sDataFId", this.contents.DataSource.ContainerId);
			if (this.IsSearchInProgress())
			{
				VirtualListView2.RenderAttribute(writer, "fSrchInProg", 1);
			}
			if (this.forceRefresh)
			{
				VirtualListView2.RenderAttribute(writer, "fR", 1);
			}
		}

		protected virtual void RenderInlineControl(TextWriter writer)
		{
		}

		protected virtual bool HasInlineControl
		{
			get
			{
				return false;
			}
		}

		private void InternalRenderChunk(TextWriter writer, bool error, string errorFlag, Strings.IDs? errorString)
		{
			writer.Write("<div id=\"divChnk\" class=\"vlvChnk\"");
			if (!error)
			{
				VirtualListView2.RenderAttribute(writer, "iSR", this.contents.DataSource.StartRange);
				VirtualListView2.RenderAttribute(writer, "iER", this.contents.DataSource.EndRange);
			}
			else if (!string.IsNullOrEmpty(errorFlag))
			{
				VirtualListView2.RenderAttribute(writer, errorFlag, 1);
			}
			writer.Write(">");
			if (!error)
			{
				this.contents.RenderForVirtualListView(writer);
			}
			else if (errorString != null)
			{
				writer.Write("<div id=\"divNI\">");
				writer.Write(LocalizedStrings.GetHtmlEncoded(errorString.Value));
				writer.Write("</div>");
			}
			writer.Write("</div>");
		}

		protected virtual void SubscribeForFolderContentChanges()
		{
		}

		internal virtual void UnSubscribeForFolderContentChanges()
		{
		}

		public const int DefaultRowsLoaded = 50;

		public const string ContainerId = "divVLV";

		private UserContext userContext;

		private string id;

		private ColumnId sortedColumn;

		private SortOrder sortOrder;

		private ListViewContents2 contents;

		private Dictionary<string, string> extraAttributes = new Dictionary<string, string>();

		private StringBuilder publicProperties = new StringBuilder();

		private bool forceRefresh;

		private bool isFiltered;

		private bool? didLastSearchFail = null;
	}
}
