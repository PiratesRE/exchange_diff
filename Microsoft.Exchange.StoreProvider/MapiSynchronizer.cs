using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MapiSynchronizer : MapiUnk
	{
		internal MapiSynchronizer(IExExportChanges iExchangeExportChanges, MapiStore mapiStore) : base(iExchangeExportChanges, null, mapiStore)
		{
			this.iExchangeExportChanges = iExchangeExportChanges;
		}

		protected override void MapiInternalDispose()
		{
			this.iExchangeExportChanges = null;
			base.MapiInternalDispose();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiSynchronizer>(this);
		}

		public unsafe void Config(Stream stream, SyncConfigFlags flags, object mapiCollector, Restriction restriction, PropTag[] propsInclude, PropTag[] propsExclude)
		{
			base.CheckDisposed();
			if (mapiCollector == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("mapiCollector");
			}
			IntPtr iCollector = IntPtr.Zero;
			if (mapiCollector is MapiCollectorBase)
			{
				iCollector = ((MapiCollectorBase)mapiCollector).ExchangeCollector;
			}
			else if (mapiCollector is MapiHierarchyCollectorBase)
			{
				iCollector = ((MapiHierarchyCollectorBase)mapiCollector).ExchangeCollector;
			}
			else
			{
				if (!(mapiCollector is MapiManifestCollector))
				{
					throw MapiExceptionHelper.ArgumentException("mapiCollector", "argument is not a MapiCollector");
				}
				if ((flags & SyncConfigFlags.OnlySpecifiedProps) == SyncConfigFlags.None)
				{
					throw MapiExceptionHelper.ArgumentException("mapiCollector", "When MapiManifestCollector is used, SyncConfigFlags.OnlySpecifiedProps must be specified.");
				}
				iCollector = ((MapiManifestCollector)mapiCollector).ExchangeCollector;
			}
			base.LockStore();
			try
			{
				int num = 0;
				if (stream == null)
				{
					throw MapiExceptionHelper.ArgumentNullException("stream");
				}
				stream.Seek(0L, SeekOrigin.Begin);
				MapiIStream iStream = new MapiIStream(stream);
				if (restriction != null)
				{
					int bytesToMarshal = restriction.GetBytesToMarshal();
					try
					{
						fixed (byte* ptr = new byte[bytesToMarshal])
						{
							byte* ptr2 = ptr;
							SRestriction* ptr3 = (SRestriction*)ptr;
							ptr2 += (SRestriction.SizeOf + 7 & -8);
							restriction.MarshalToNative(ptr3, ref ptr2);
							num = this.iExchangeExportChanges.Config(iStream, (int)flags, iCollector, ptr3, PropTagHelper.SPropTagArray(propsInclude), PropTagHelper.SPropTagArray(propsExclude), 0);
							goto IL_148;
						}
					}
					finally
					{
						byte* ptr = null;
					}
				}
				num = this.iExchangeExportChanges.Config(iStream, (int)flags, iCollector, null, PropTagHelper.SPropTagArray(propsInclude), PropTagHelper.SPropTagArray(propsExclude), 0);
				IL_148:
				if (num != 0)
				{
					base.ThrowIfError("Unable to configure ICS synchronizer.", num);
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public int Synchronize()
		{
			base.CheckDisposed();
			base.LockStore();
			int result;
			try
			{
				int num = 0;
				int num2 = 0;
				int num3 = this.iExchangeExportChanges.Synchronize(out num, out num2);
				if (num3 != 0)
				{
					base.ThrowIfError("Synchronization failued.", num3);
				}
				result = num3;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
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
					stream.Seek(0L, SeekOrigin.Begin);
					iStream = new MapiIStream(stream);
				}
				int num = this.iExchangeExportChanges.UpdateState(iStream);
				if (num != 0)
				{
					base.ThrowIfError("Unable to update ICS synchronizer.", num);
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		private IExExportChanges iExchangeExportChanges;
	}
}
