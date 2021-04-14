using System;
using Microsoft.Exchange.Data.Mapi.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MapiModifyTableWrapper : IDisposeTrackable, IDisposable
	{
		internal MapiMessageStoreSession MapiSession
		{
			get
			{
				return this.mapiSession;
			}
		}

		internal MapiModifyTableWrapper(MapiModifyTable mapiModifyTable, MapiMessageStoreSession mapiSession, MapiStore mapiStore, MapiObjectId mapiObjectId)
		{
			this.mapiModifyTable = mapiModifyTable;
			this.mapiSession = mapiSession;
			this.mapiStore = mapiStore;
			this.mapiObjectId = mapiObjectId;
			this.disposeTracker = this.GetDisposeTracker();
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MapiModifyTableWrapper>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public MapiTableWrapper GetTable(GetTableFlags flags)
		{
			MapiTable mapiTable = null;
			this.MapiSession.InvokeWithWrappedException(delegate()
			{
				mapiTable = this.mapiModifyTable.GetTable(GetTableFlags.None);
			}, Strings.ErrorGetMapiTableWithIdentityAndServer(this.mapiObjectId.ToString(), this.MapiSession.ServerName), this.mapiObjectId);
			return new MapiTableWrapper(mapiTable, this.MapiSession, this.mapiObjectId);
		}

		public void ModifyTable(ModifyTableFlags flags, RowEntry[] rowList)
		{
			this.MapiSession.InvokeWithWrappedException(delegate()
			{
				this.mapiModifyTable.ModifyTable(flags, rowList);
			}, Strings.ErrorModifyMapiTableWithIdentityAndServer(this.mapiObjectId.ToString(), this.MapiSession.ServerName), this.mapiObjectId);
		}

		public void Dispose()
		{
			if (this.mapiModifyTable != null)
			{
				this.mapiModifyTable.Dispose();
				this.mapiModifyTable = null;
			}
			if (this.mapiStore != null)
			{
				this.mapiStore.Dispose();
				this.mapiStore = null;
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

		private MapiModifyTable mapiModifyTable;

		private MapiMessageStoreSession mapiSession;

		private MapiStore mapiStore;

		private MapiObjectId mapiObjectId;

		private DisposeTracker disposeTracker;

		public static readonly Guid IExchangeModifyTable = new Guid("2d734cb0-53fd-101b-b19d-08002b3056e3");
	}
}
