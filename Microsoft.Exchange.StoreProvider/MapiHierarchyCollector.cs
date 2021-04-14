using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MapiHierarchyCollector : MapiHierarchyCollectorBase
	{
		internal MapiHierarchyCollector(IExImportHierarchyChanges iExchangeImportHierarchyChanges, MapiStore mapiStore) : base(iExchangeImportHierarchyChanges, mapiStore)
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
			return DisposeTracker.Get<MapiHierarchyCollector>(this);
		}

		public void Config(Stream stream, CollectorConfigFlags flags)
		{
			base.CheckDisposed();
			base.LockStore();
			try
			{
				MapiIStream iStream = null;
				if (stream != null)
				{
					iStream = new MapiIStream(stream);
				}
				int num = this.iExchangeImportHierarchyChanges.Config(iStream, (int)flags);
				if (num != 0)
				{
					base.ThrowIfErrorOrWarning("Unable to configure ICS collector.", num);
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public void UpdateState(Stream stream)
		{
			base.CheckDisposed();
			base.LockStore();
			try
			{
				MapiIStream iStream = null;
				if (stream != null)
				{
					iStream = new MapiIStream(stream);
				}
				int num = this.iExchangeImportHierarchyChanges.UpdateState(iStream);
				if (num != 0)
				{
					base.ThrowIfErrorOrWarning("Unable to update collector state.", num);
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		private IExImportHierarchyChanges iExchangeImportHierarchyChanges;
	}
}
