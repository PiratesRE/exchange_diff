using System;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	internal abstract class VirtualListViewEventHandler2 : OwaEventHandlerBase
	{
		protected abstract VirtualListViewState ListViewState { get; }

		public abstract void LoadFresh();

		public abstract void LoadNext();

		public abstract void LoadPrevious();

		public abstract void SeekNext();

		public abstract void SeekPrevious();

		public abstract void Sort();

		public abstract void SetMultiLineState();

		public abstract void TypeDownSearch();

		protected void InternalLoadFresh()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "VirtualListViewEventHandler.InternalLoadFresh");
			this.InternalLoadFresh(false);
		}

		protected void InternalLoadNext()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "VirtualListViewEventHandler.InternalLoadNext");
			this.LoadNextOrPrevious(SeekDirection.Next);
		}

		protected void InternalLoadPrevious()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "VirtualListViewEventHandler.InternalLoadPrevious");
			this.LoadNextOrPrevious(SeekDirection.Previous);
		}

		protected void InternalSeekNext()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "VirtualListViewEventHandler.InternalSeekNext");
			this.SeekNextOrPrevious(SeekDirection.Next);
		}

		protected void InternalSeekPrevious()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "VirtualListViewEventHandler.InternalSeekPrevious");
			this.SeekNextOrPrevious(SeekDirection.Previous);
		}

		protected void InternalSort()
		{
			try
			{
				ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "VirtualListViewEventHandler.InternalSort");
				int rowCount = this.GetRowCount();
				this.PersistSort();
				VirtualListView2 listView = this.GetListView();
				listView.UnSubscribeForFolderContentChanges();
				if (base.IsParameterSet("SId"))
				{
					OwaStoreObjectId seekId = this.GetSeekId();
					listView.LoadData(seekId, SeekDirection.Next, rowCount);
				}
				else
				{
					listView.LoadData(0, rowCount);
				}
				listView.RenderData(this.Writer);
				listView.RenderChunk(this.Writer);
				this.RenderExtraData(listView);
				listView.RenderHeaders(this.Writer);
				this.RenderNewSelection(listView);
			}
			finally
			{
				this.EndProcessEvent();
			}
		}

		protected void InternalSetMultiLineState()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "VirtualListViewEventHandler.InternalSetMultiLineState");
			this.PersistMultiLineState();
			this.InternalLoadFresh(true);
		}

		protected void InternalTypeDownSearch()
		{
			try
			{
				int rowCount = this.GetRowCount();
				string text = (string)base.GetParameter("td");
				if (text.Equals(string.Empty, StringComparison.Ordinal))
				{
					throw new OwaInvalidRequestException("Type down search string cannot be empty.");
				}
				Column column = ListViewColumns.GetColumn(this.ListViewState.SortedColumn);
				if (!column.IsTypeDownCapable)
				{
					throw new OwaInvalidRequestException("Type down search is not supported.");
				}
				VirtualListView2 listView = this.GetListView();
				listView.LoadData(text, rowCount);
				listView.RenderData(this.Writer);
				listView.RenderChunk(this.Writer);
				this.RenderExtraData(listView);
			}
			finally
			{
				this.EndProcessEvent();
			}
		}

		protected abstract VirtualListView2 GetListView();

		protected virtual void RenderExtraData(VirtualListView2 listView)
		{
		}

		protected virtual void PersistFilter()
		{
		}

		protected virtual OwaStoreObjectId GetSeekId()
		{
			return (OwaStoreObjectId)base.GetParameter("SId");
		}

		protected virtual OwaStoreObjectId GetNewSelection()
		{
			return null;
		}

		protected virtual void PersistSort()
		{
		}

		protected virtual void PersistMultiLineState()
		{
		}

		protected virtual void EndProcessEvent()
		{
		}

		private void InternalLoadFresh(bool renderHeaders)
		{
			try
			{
				int num = (int)base.GetParameter("SR");
				int rowCount = this.GetRowCount();
				if (num < 0)
				{
					throw new OwaInvalidRequestException("StartRange cannot be less than 0");
				}
				VirtualListView2 listView = this.GetListView();
				listView.LoadData(num, rowCount);
				listView.RenderData(this.Writer);
				listView.RenderChunk(this.Writer);
				this.RenderExtraData(listView);
				if (renderHeaders)
				{
					listView.RenderHeaders(this.Writer);
				}
				if (base.IsParameterSet("fltr"))
				{
					this.PersistFilter();
				}
			}
			finally
			{
				this.EndProcessEvent();
			}
			if (Globals.ArePerfCountersEnabled)
			{
				OwaSingleCounters.MailViewRefreshes.Increment();
			}
		}

		private void LoadNextOrPrevious(SeekDirection seekDirection)
		{
			try
			{
				ObjectId adjacentId = (ObjectId)base.GetParameter("AId");
				int rowCount = this.GetRowCount();
				VirtualListView2 listView = this.GetListView();
				listView.LoadAdjacent(adjacentId, seekDirection, rowCount);
				listView.RenderData(this.Writer);
				listView.RenderChunk(this.Writer);
				this.RenderExtraData(listView);
			}
			finally
			{
				this.EndProcessEvent();
			}
		}

		private void SeekNextOrPrevious(SeekDirection seekDirection)
		{
			try
			{
				ObjectId seekRowId = (ObjectId)base.GetParameter("SId");
				int rowCount = this.GetRowCount();
				VirtualListView2 listView = this.GetListView();
				listView.LoadData(seekRowId, seekDirection, rowCount);
				listView.RenderData(this.Writer);
				listView.RenderChunk(this.Writer);
				this.RenderExtraData(listView);
				if (base.IsParameterSet("nwSel") && (bool)base.GetParameter("nwSel"))
				{
					this.RenderNewSelection(listView);
				}
			}
			finally
			{
				this.EndProcessEvent();
			}
		}

		private void RenderNewSelection(VirtualListView2 listView)
		{
			MessageVirtualListView2 messageVirtualListView = listView as MessageVirtualListView2;
			OwaStoreObjectId owaStoreObjectId;
			if (messageVirtualListView != null && messageVirtualListView.GetNewSelectionId() != null)
			{
				owaStoreObjectId = messageVirtualListView.GetNewSelectionId();
			}
			else
			{
				owaStoreObjectId = this.GetNewSelection();
			}
			if (owaStoreObjectId != null)
			{
				this.Writer.Write("<div id=\"nwSel\" sId=\"");
				Utilities.HtmlEncode(owaStoreObjectId.ToString(), this.Writer);
				this.Writer.Write("\"></div>");
			}
		}

		private int GetRowCount()
		{
			int num = (int)base.GetParameter("RC");
			if (num < 1)
			{
				throw new OwaInvalidRequestException("RowCount must be at least 1");
			}
			if (num > 100)
			{
				throw new OwaInvalidRequestException("RowCount cannot be more than " + 100.ToString());
			}
			return num;
		}

		public const int MaxSelectionSize = 500;

		public const int MaxChunkSize = 100;

		public const string MethodLoadFresh = "LoadFresh";

		public const string MethodLoadNext = "LoadNext";

		public const string MethodLoadPrevious = "LoadPrevious";

		public const string MethodSeekNext = "SeekNext";

		public const string MethodSeekPrevious = "SeekPrevious";

		public const string MethodSort = "Sort";

		public const string MethodSetMultiLineState = "SetML";

		public const string MethodTypeDown = "TypeDown";

		public const string State = "St";

		public const string StartRange = "SR";

		public const string RowCount = "RC";

		public const string AdjacentId = "AId";

		public const string SeekId = "SId";

		public const string ReturnNewSelection = "nwSel";

		public const string Id = "id";

		public const string TypeDownSearchString = "td";

		public const string ReadingPanePosition = "s";

		public const string FilterString = "fltr";

		public const string PaaPicker = "paa";

		public const string MobilePicker = "mbl";

		public const string IsNewestOnTop = "isNewestOnTop";

		public const string ShowTreeInListView = "showTreeInListView";
	}
}
