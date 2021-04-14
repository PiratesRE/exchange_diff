using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MapiContainer : MapiProp
	{
		internal MapiContainer(IExMapiContainer iMAPIContainer, object externalIMAPIContainer, MapiStore mapiStore, Guid[] interfaceIds) : base(iMAPIContainer, externalIMAPIContainer, mapiStore, interfaceIds)
		{
			this.iMAPIContainer = iMAPIContainer;
		}

		protected override void MapiInternalDispose()
		{
			this.iMAPIContainer = null;
			base.MapiInternalDispose();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiContainer>(this);
		}

		internal static object Wrap(IExInterface iObj, int objType, MapiStore mapiStore)
		{
			switch (objType)
			{
			case 3:
				return new MapiFolder(iObj.ToInterface<IExMapiFolder>(), null, mapiStore);
			case 5:
				return new MapiMessage(iObj.ToInterface<IExMapiMessage>(), null, mapiStore);
			}
			throw MapiExceptionHelper.ArgumentException("objType", string.Format("Unable to wrap unknown object type {0}; object must be MapiFolder or MapiMessage", objType));
		}

		public MapiTable GetContentsTable(ContentsTableFlags flags)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			MapiTable result;
			try
			{
				MapiTable mapiTable = null;
				IExMapiTable exMapiTable = null;
				try
				{
					int contentsTable = this.iMAPIContainer.GetContentsTable((int)flags, out exMapiTable);
					if (contentsTable != 0)
					{
						base.ThrowIfError("Unable to get contents table.", contentsTable);
					}
					mapiTable = new MapiTable(exMapiTable, base.MapiStore);
					exMapiTable = null;
				}
				finally
				{
					exMapiTable.DisposeIfValid();
				}
				result = mapiTable;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public MapiTable GetContentsTable()
		{
			return this.GetContentsTable(ContentsTableFlags.DeferredErrors | ContentsTableFlags.Unicode);
		}

		public MapiTable GetAssociatedContentsTable()
		{
			return this.GetContentsTable(ContentsTableFlags.Associated | ContentsTableFlags.DeferredErrors | ContentsTableFlags.Unicode);
		}

		public MapiTable GetHierarchyTable(HierarchyTableFlags flags)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			MapiTable result;
			try
			{
				MapiTable mapiTable = null;
				IExMapiTable exMapiTable = null;
				try
				{
					int hierarchyTable = this.iMAPIContainer.GetHierarchyTable((int)flags, out exMapiTable);
					if (hierarchyTable != 0)
					{
						base.ThrowIfError("Unable to get hierarchy table.", hierarchyTable);
					}
					mapiTable = new MapiTable(exMapiTable, base.MapiStore);
					exMapiTable = null;
				}
				finally
				{
					exMapiTable.DisposeIfValid();
				}
				result = mapiTable;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public MapiTable GetHierarchyTable()
		{
			return this.GetHierarchyTable(HierarchyTableFlags.DeferredErrors | HierarchyTableFlags.Unicode);
		}

		public void SetSearchCriteria(Restriction restriction, byte[][] entryIds, SearchCriteriaFlags flags)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace<SearchCriteriaFlags, string>(12466, 15, (long)this.GetHashCode(), "MapiFolder.SetSearchCriteria params: flags={0}, entryIds={1}", flags, TraceUtils.DumpArray(entryIds));
				}
				int num = this.iMAPIContainer.SetSearchCriteria(restriction, entryIds, (int)flags);
				if (num != 0)
				{
					base.ThrowIfError("Unable to SetSearchCriteria.", num);
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public void GetSearchCriteria(out Restriction restriction, out byte[][] entryIds, out SearchState state)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			try
			{
				int num = 0;
				state = SearchState.None;
				int searchCriteria = this.iMAPIContainer.GetSearchCriteria(int.MinValue, out restriction, out entryIds, out num);
				if (searchCriteria != 0)
				{
					base.ThrowIfError("Unable to GetSearchCriteria.", searchCriteria);
				}
				state = (SearchState)num;
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public object OpenEntry(byte[] entryId)
		{
			return this.OpenEntry(entryId, OpenEntryFlags.BestAccess | OpenEntryFlags.DeferredErrors);
		}

		public object OpenEntry(byte[] entryId, OpenEntryFlags flags)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			object result;
			try
			{
				object obj = null;
				int objType = 0;
				IExInterface exInterface = null;
				try
				{
					exInterface = this.InternalOpenEntry(entryId, flags, out objType);
					if (exInterface != null)
					{
						obj = MapiContainer.Wrap(exInterface, objType, base.MapiStore);
						exInterface = null;
					}
				}
				finally
				{
					exInterface.DisposeIfValid();
				}
				result = obj;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		protected IExInterface InternalOpenEntry(byte[] entryId, OpenEntryFlags flags, out int objType)
		{
			base.BlockExternalObjectCheck();
			IExInterface exInterface = null;
			bool flag = false;
			int ulFlags = (int)(flags & ~OpenEntryFlags.DontThrowIfEntryIsMissing);
			try
			{
				int num = this.iMAPIContainer.OpenEntry(entryId, Guid.Empty, ulFlags, out objType, out exInterface);
				if (num == -2147221233 && (flags & OpenEntryFlags.DontThrowIfEntryIsMissing) != OpenEntryFlags.None)
				{
					return null;
				}
				if (num != 0)
				{
					base.ThrowIfError("Unable to open entry.", num);
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					exInterface.DisposeIfValid();
					exInterface = null;
				}
			}
			return exInterface;
		}

		protected IExMapiContainer iMAPIContainer;
	}
}
