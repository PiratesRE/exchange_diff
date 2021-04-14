using System;
using Microsoft.Exchange.Data.Mapi.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MapiTableWrapper : IDisposeTrackable, IDisposable
	{
		internal MapiMessageStoreSession MapiSession
		{
			get
			{
				return this.mapiSession;
			}
		}

		internal MapiTableWrapper(MapiTable mapiTable, MapiMessageStoreSession mapiSession, MapiObjectId mapiObjectId)
		{
			this.mapiTable = mapiTable;
			this.mapiSession = mapiSession;
			this.mapiObjectId = mapiObjectId;
			this.disposeTracker = this.GetDisposeTracker();
		}

		public int SeekRow(BookMark bookmark, int crowsSeek)
		{
			int result = 0;
			this.MapiSession.InvokeWithWrappedException(delegate()
			{
				result = this.mapiTable.SeekRow(bookmark, crowsSeek);
			}, Strings.ErrorMapiTableSeekRow(this.mapiObjectId.ToString(), this.MapiSession.ServerName), this.mapiObjectId);
			return result;
		}

		public void SetColumns(params PropTag[] propTags)
		{
			this.MapiSession.InvokeWithWrappedException(delegate()
			{
				this.mapiTable.SetColumns(propTags);
			}, Strings.ErrorMapiTableSetColumn(this.mapiObjectId.ToString(), this.MapiSession.ServerName), this.mapiObjectId);
		}

		public PropValue[][] QueryRows(int crows)
		{
			PropValue[][] results = null;
			this.MapiSession.InvokeWithWrappedException(delegate()
			{
				results = this.mapiTable.QueryRows(crows);
			}, Strings.ErrorMapiTableQueryRows(this.mapiObjectId.ToString(), this.MapiSession.ServerName), this.mapiObjectId);
			return results;
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MapiTableWrapper>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			if (this.mapiTable != null)
			{
				this.mapiTable.Dispose();
				this.mapiTable = null;
			}
			if (this.mapiSession != null)
			{
				this.mapiSession = null;
			}
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
		}

		private MapiTable mapiTable;

		private MapiMessageStoreSession mapiSession;

		private MapiObjectId mapiObjectId;

		private DisposeTracker disposeTracker;
	}
}
