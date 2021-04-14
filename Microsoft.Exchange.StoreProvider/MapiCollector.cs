using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MapiCollector : MapiCollectorBase
	{
		internal MapiCollector(IExImportContentsChanges iExchangeImportContentsChanges, MapiStore mapiStore) : base(iExchangeImportContentsChanges, mapiStore)
		{
			this.iExchangeImportContentsChanges = iExchangeImportContentsChanges;
		}

		protected override void MapiInternalDispose()
		{
			this.iExchangeImportContentsChanges = null;
			base.MapiInternalDispose();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiCollector>(this);
		}

		public void Config(Stream stream, CollectorConfigFlags flags)
		{
			base.CheckDisposed();
			base.LockStore();
			try
			{
				if (stream == null)
				{
					throw MapiExceptionHelper.ArgumentNullException("stream");
				}
				MapiIStream iStream = new MapiIStream(stream);
				int num = this.iExchangeImportContentsChanges.Config(iStream, (int)flags);
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
				int num = this.iExchangeImportContentsChanges.UpdateState(iStream);
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

		public void ImportMessageMove(PropValue sourceFolderKey, PropValue sourceMessageKey, PropValue changeListKey, PropValue destMessageKey, PropValue changeNumberKey)
		{
			base.CheckDisposed();
			base.LockStore();
			try
			{
				int num = base.InternalImportMessageMove(sourceFolderKey, sourceMessageKey, changeListKey, destMessageKey, changeNumberKey);
				if (num != 0)
				{
					base.ThrowIfErrorOrWarning("Unable to import message move.", num);
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public MapiMessage ImportMessageChange(PropValue[] propValues, ImportMessageChangeFlags importMessageChangeFlags)
		{
			base.CheckDisposed();
			base.LockStore();
			MapiMessage result;
			try
			{
				MapiMessage mapiMessage = null;
				bool flag = false;
				try
				{
					int num = base.InternalImportMessageChange(propValues, importMessageChangeFlags, out mapiMessage);
					if (num != 0)
					{
						base.ThrowIfErrorOrWarning("Unable to import message change.", num);
					}
					flag = true;
					result = mapiMessage;
				}
				finally
				{
					if (!flag && mapiMessage != null)
					{
						mapiMessage.Dispose();
						mapiMessage = null;
					}
				}
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		private IExImportContentsChanges iExchangeImportContentsChanges;
	}
}
