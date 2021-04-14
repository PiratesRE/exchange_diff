using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MapiHierarchyCollectorBase : MapiUnk
	{
		internal MapiHierarchyCollectorBase(IExImportHierarchyChanges iExchangeImportHierarchyChanges, MapiStore mapiStore) : base(iExchangeImportHierarchyChanges, null, mapiStore)
		{
			this.iExchangeImportHierarchyChanges = iExchangeImportHierarchyChanges;
		}

		protected override void MapiInternalDispose()
		{
			this.iExchangeImportHierarchyChanges = null;
			base.MapiInternalDispose();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiHierarchyCollectorBase>(this);
		}

		internal IntPtr ExchangeCollector
		{
			get
			{
				base.CheckDisposed();
				return ((SafeExImportHierarchyChangesHandle)this.iExchangeImportHierarchyChanges).DangerousGetHandle();
			}
		}

		public unsafe void ImportFolderChange(PropValue[] propValues)
		{
			base.CheckDisposed();
			if (propValues == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("propValues");
			}
			if (propValues.Length <= 0)
			{
				throw MapiExceptionHelper.ArgumentOutOfRangeException("propValues", "values must contain at least 1 element");
			}
			base.LockStore();
			try
			{
				int num = 0;
				for (int i = 0; i < propValues.Length; i++)
				{
					num += propValues[i].GetBytesToMarshal();
				}
				try
				{
					fixed (byte* ptr = new byte[num])
					{
						PropValue.MarshalToNative(propValues, ptr);
						int num2 = this.iExchangeImportHierarchyChanges.ImportFolderChange(propValues.Length, (SPropValue*)ptr);
						if (num2 != 0)
						{
							base.ThrowIfErrorOrWarning("Unable to import folder change.", num2);
						}
					}
				}
				finally
				{
					byte* ptr = null;
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public unsafe void ImportFolderDeletion(ImportDeletionFlags importDeletionFlags, params PropValue[] sourceKeys)
		{
			base.CheckDisposed();
			if (sourceKeys == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("sourceKeys");
			}
			if (sourceKeys.Length <= 0)
			{
				throw MapiExceptionHelper.ArgumentOutOfRangeException("sourceKeys", "sourceKeys must contain at least 1 element");
			}
			base.LockStore();
			try
			{
				SBinary[] array = new SBinary[sourceKeys.Length];
				for (int i = 0; i < sourceKeys.Length; i++)
				{
					array[i] = new SBinary(sourceKeys[i].GetBytes());
				}
				int bytesToMarshal = SBinaryArray.GetBytesToMarshal(array);
				try
				{
					fixed (byte* ptr = new byte[bytesToMarshal])
					{
						SBinaryArray.MarshalToNative(ptr, array);
						int num = this.iExchangeImportHierarchyChanges.ImportFolderDeletion((int)importDeletionFlags, (_SBinaryArray*)ptr);
						if (num != 0)
						{
							base.ThrowIfErrorOrWarning("Unable to import folder deletion.", num);
						}
					}
				}
				finally
				{
					byte* ptr = null;
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public void ImportFolderDeletion(params PropValue[] sourceKeys)
		{
			this.ImportFolderDeletion(ImportDeletionFlags.None, sourceKeys);
		}

		private IExImportHierarchyChanges iExchangeImportHierarchyChanges;
	}
}
