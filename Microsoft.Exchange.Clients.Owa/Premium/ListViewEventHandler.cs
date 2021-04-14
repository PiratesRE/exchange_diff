using System;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	internal abstract class ListViewEventHandler : OwaEventHandlerBase
	{
		protected abstract ListViewState ListViewState { get; }

		protected virtual void PreRefresh()
		{
		}

		protected virtual void EndProcessEvent()
		{
		}

		protected abstract ListView GetListView();

		protected abstract IListViewDataSource GetDataSource(ListView listView);

		protected virtual void PersistFilter()
		{
		}

		protected abstract IListViewDataSource GetDataSource(ListView listView, bool newSearch);

		public static void Register()
		{
			OwaEventRegistry.RegisterEnum(typeof(ColumnId));
			OwaEventRegistry.RegisterEnum(typeof(SortOrder));
			OwaEventRegistry.RegisterEnum(typeof(ReadingPanePosition));
			OwaEventRegistry.RegisterEnum(typeof(SearchScope));
		}

		public virtual void Refresh()
		{
			try
			{
				ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "ListViewEventHandler.InternalRefresh");
				this.Refresh(ListView.RenderFlags.Contents, this.ListViewState.StartRange - 1);
				if (base.IsParameterSet("fltr"))
				{
					this.PersistFilter();
				}
				if (Globals.ArePerfCountersEnabled)
				{
					OwaSingleCounters.MailViewRefreshes.Increment();
				}
			}
			finally
			{
				this.EndProcessEvent();
			}
		}

		public virtual void TypeDown()
		{
			try
			{
				ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "ListViewEventHandler.TypeDown");
				this.PreRefresh();
				ListView listView = this.GetListView();
				Column column = ListViewColumns.GetColumn(this.ListViewState.SortedColumnId);
				if (!column.IsTypeDownCapable)
				{
					throw new OwaInvalidOperationException("Type down search is not supported");
				}
				IListViewDataSource dataSource = this.GetDataSource(listView);
				dataSource.Load((string)base.GetParameter("td"), base.UserContext.UserOptions.ViewRowCount);
				listView.DataSource = dataSource;
				this.WriteResponse(ListView.RenderFlags.Contents, listView);
			}
			finally
			{
				this.EndProcessEvent();
			}
		}

		public virtual void ToggleMultiVsSingleLine()
		{
			try
			{
				ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "ListViewEventHandler.ToggleMultiVsSingleLine");
				this.PersistMultiLineState(this.ListViewState.IsMultiLine);
				this.Refresh(ListView.RenderFlags.Contents | ListView.RenderFlags.Headers, this.ListViewState.StartRange - 1);
			}
			finally
			{
				this.EndProcessEvent();
			}
		}

		protected virtual void PersistMultiLineState(bool isMultiLine)
		{
		}

		public virtual void FirstPage()
		{
			try
			{
				ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "ListViewEventHandler.FirstPage");
				this.Refresh(ListView.RenderFlags.Contents, 0);
			}
			finally
			{
				this.EndProcessEvent();
			}
		}

		public virtual void PreviousPage()
		{
			try
			{
				ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "ListViewEventHandler.PreviousPage");
				int startRange = this.ListViewState.StartRange - base.UserContext.UserOptions.ViewRowCount - 1;
				this.Refresh(ListView.RenderFlags.Contents, startRange);
			}
			finally
			{
				this.EndProcessEvent();
			}
		}

		public virtual void NextPage()
		{
			try
			{
				ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "ListViewEventHandler.NextPage");
				this.Refresh(ListView.RenderFlags.Contents, this.ListViewState.EndRange);
			}
			finally
			{
				this.EndProcessEvent();
			}
		}

		public virtual void LastPage()
		{
			try
			{
				ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "ListViewEventHandler.LastPage");
				int startRange = this.ListViewState.TotalCount - base.UserContext.UserOptions.ViewRowCount;
				this.Refresh(ListView.RenderFlags.Contents, startRange);
			}
			finally
			{
				this.EndProcessEvent();
			}
		}

		public virtual void Sort()
		{
			try
			{
				ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "ListViewEventHandler.Sort");
				bool flag = this.ShouldSeekToItemOnSort(this.ListViewState.SortedColumnId);
				this.PersistSort(this.ListViewState.SortedColumnId, this.ListViewState.SortDirection);
				ObjectId objectId = base.GetParameter("mId") as ObjectId;
				if (objectId != null && flag)
				{
					this.PreRefresh();
					ListView listView = this.GetListView();
					IListViewDataSource dataSource = this.GetDataSource(listView);
					dataSource.Load(objectId, SeekDirection.Next, base.UserContext.UserOptions.ViewRowCount);
					listView.DataSource = dataSource;
					this.WriteResponse(ListView.RenderFlags.Contents | ListView.RenderFlags.Headers, listView);
				}
				else
				{
					this.Refresh(ListView.RenderFlags.Contents | ListView.RenderFlags.Headers, 0);
				}
			}
			finally
			{
				this.EndProcessEvent();
			}
		}

		public virtual void Find()
		{
			try
			{
				string text = (string)base.GetParameter("srch");
				if (255 < text.Length)
				{
					throw new OwaInvalidOperationException("Search string excedes the maximum supported length");
				}
				ListView listView = this.GetListView();
				IListViewDataSource dataSource = this.GetDataSource(listView, true);
				dataSource.Load(0, base.UserContext.UserOptions.ViewRowCount);
				listView.DataSource = dataSource;
				this.WriteResponse(ListView.RenderFlags.Contents, listView);
			}
			finally
			{
				this.EndProcessEvent();
			}
		}

		protected virtual void PersistSort(ColumnId sortedColumn, SortOrder sortOrder)
		{
		}

		protected virtual bool ShouldSeekToItemOnSort(ColumnId newSortColumn)
		{
			return true;
		}

		private void Refresh(ListView.RenderFlags renderFlags, int startRange)
		{
			this.Refresh(renderFlags, startRange, SeekDirection.Next);
		}

		private void Refresh(ListView.RenderFlags renderFlags, int startRange, SeekDirection direction)
		{
			this.PreRefresh();
			if (startRange < 0)
			{
				startRange = 0;
			}
			ListView listView = this.GetListView();
			IListViewDataSource dataSource = this.GetDataSource(listView);
			this.LoadDataSource(dataSource, startRange, base.UserContext.UserOptions.ViewRowCount, direction);
			listView.DataSource = dataSource;
			this.WriteResponse(renderFlags, listView);
		}

		protected virtual void LoadDataSource(IListViewDataSource listViewDataSource, int startRange, int itemCount, SeekDirection direction)
		{
			listViewDataSource.Load(startRange, itemCount);
		}

		protected void WriteResponse(ListView.RenderFlags renderFlags, ListView listView)
		{
			if (listView.IsFilteredView)
			{
				this.Writer.Write("<span id=spnSR>");
				this.RenderSearchResultsHeader(listView.DataSource);
				this.Writer.Write("</span>");
			}
			this.Writer.Write("<div id=\"data\" sR=\"");
			this.Writer.Write((0 < listView.DataSource.TotalCount) ? (listView.StartRange + 1) : 0);
			this.Writer.Write("\" eR=\"");
			this.Writer.Write((0 < listView.DataSource.TotalCount) ? (listView.EndRange + 1) : 0);
			this.Writer.Write("\" tC=\"");
			this.Writer.Write(listView.DataSource.TotalCount);
			this.Writer.Write("\" sCki=\"");
			this.Writer.Write(listView.Cookie);
			this.Writer.Write("\" iLcid=\"");
			this.Writer.Write(listView.CookieLcid);
			this.Writer.Write("\" sPfdDC=\"");
			this.Writer.Write(Utilities.HtmlEncode(listView.PreferredDC));
			this.Writer.Write("\" uC=\"");
			this.Writer.Write(listView.DataSource.UnreadCount);
			this.Writer.Write("\"></div>");
			listView.Render(this.Writer, renderFlags);
		}

		protected virtual void RenderSearchResultsHeader(IListViewDataSource dataSource)
		{
			this.Writer.Write(LocalizedStrings.GetHtmlEncoded(-774873536), dataSource.TotalCount);
		}

		public const string MethodRefresh = "Refresh";

		public const string MethodTypeDown = "TypeDown";

		public const string MethodToggleMultiVsSingleLine = "ToggleMultiVsSingleLine";

		public const string MethodFirstPage = "FirstPage";

		public const string MethodPreviousPage = "PreviousPage";

		public const string MethodNextPage = "NextPage";

		public const string MethodLastPage = "LastPage";

		public const string MethodSort = "Sort";

		public const string MethodNewMessageToPeople = "MsgToPeople";

		public const string MethodMove = "Move";

		public const string MethodCopy = "Copy";

		public const string MethodPersistSearchScope = "PSS";

		public const string MethodSeekItem = "SeekItem";

		public const string MItemId = "mId";

		public const string Id = "id";

		public const string Conversations = "cnvs";

		public const string Width = "w";

		public const string Height = "h";

		public const string ReadingPanePosition = "s";

		public const string TypeDownSearch = "td";

		public const string SearchString = "srch";

		public const string DestinationFolderId = "destId";

		public const string FilterString = "fltr";

		public const string ExpandedConversations = "cnvs";

		public const string VisibleReadItems = "vR";

		public const string State = "st";

		public const string PaaPicker = "paa";

		public const string MobilePicker = "mbl";

		private static readonly PropertyDefinition[] addressProperties = new PropertyDefinition[]
		{
			ContactSchema.Email1EmailAddress,
			ContactSchema.Email1AddrType,
			ContactSchema.Email1DisplayName,
			ContactSchema.Email2EmailAddress,
			ContactSchema.Email2AddrType,
			ContactSchema.Email2DisplayName,
			ContactSchema.Email3EmailAddress,
			ContactSchema.Email3AddrType,
			ContactSchema.Email3DisplayName
		};

		private enum EmailAddressProperty
		{
			EmailAddress,
			AddressType,
			DisplayName,
			Count
		}
	}
}
