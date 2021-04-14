using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MapiModifyTable : MapiUnk
	{
		internal MapiModifyTable(IExModifyTable iExchangeModifyTable, MapiStore mapiStore) : base(iExchangeModifyTable, null, mapiStore)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(18))
			{
				ComponentTrace<MapiNetTags>.Trace<string>(11986, 18, (long)this.GetHashCode(), "MapiModifyTable.MapiModifyTable: this={0}", TraceUtils.MakeHash(this));
			}
			this.iExchangeModifyTable = iExchangeModifyTable;
		}

		protected MapiModifyTable()
		{
		}

		protected override void MapiInternalDispose()
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(18))
			{
				ComponentTrace<MapiNetTags>.Trace<string>(16082, 18, (long)this.GetHashCode(), "MapiModifyTable.InternalDispose: this={0}", TraceUtils.MakeHash(this));
			}
			this.iExchangeModifyTable = null;
			base.MapiInternalDispose();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiModifyTable>(this);
		}

		public virtual MapiTable GetTable(GetTableFlags flags)
		{
			base.CheckDisposed();
			base.LockStore();
			MapiTable result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(18))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(8402, 18, (long)this.GetHashCode(), "MapiModifyTable.GetTable params: flags=0x{0}", flags.ToString("x"));
				}
				IExMapiTable exMapiTable = null;
				MapiTable mapiTable;
				try
				{
					int table = this.iExchangeModifyTable.GetTable((int)(flags | (GetTableFlags)(-2147483648)), out exMapiTable);
					if (table != 0)
					{
						base.ThrowIfError("Unable to get modify table.", table);
					}
					mapiTable = new MapiTable(exMapiTable, base.MapiStore);
					exMapiTable = null;
				}
				finally
				{
					exMapiTable.DisposeIfValid();
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(18))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(12498, 18, (long)this.GetHashCode(), "MapiModifyTable.GetTable results: mapiTable={0}", TraceUtils.MakeHash(mapiTable));
				}
				result = mapiTable;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public virtual void ModifyTable(ModifyTableFlags flags, ICollection<RowEntry> rowList)
		{
			base.CheckDisposed();
			base.LockStore();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(18))
				{
					ComponentTrace<MapiNetTags>.Trace<string, string>(10450, 18, (long)this.GetHashCode(), "MapiModifyTable.ModifyTable params: flags=0x{0}, rowList={1}", flags.ToString("x"), TraceUtils.DumpCollection<RowEntry>(rowList));
				}
				if (rowList != null && (rowList.Count != 0 || flags != ModifyTableFlags.None))
				{
					int num = this.iExchangeModifyTable.ModifyTable((int)flags, rowList);
					if (num != 0)
					{
						base.ThrowIfError("Unable to modify table.", num);
					}
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		private IExModifyTable iExchangeModifyTable;
	}
}
