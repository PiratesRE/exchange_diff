using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MapiFastTransferStream : MapiUnk
	{
		internal MapiFastTransferStream(SafeExFastTransferStreamHandle iMAPIFxStream, MapiFastTransferStreamMode mode, MapiStore mapiStore) : base(iMAPIFxStream, null, mapiStore)
		{
			this.mode = mode;
			this.iMAPIFxStream = iMAPIFxStream;
		}

		protected override void MapiInternalDispose()
		{
			this.iMAPIFxStream = null;
			base.MapiInternalDispose();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiFastTransferStream>(this);
		}

		public unsafe void Configure(ICollection<PropValue> properties)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (properties == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("properties");
			}
			if (properties.Count <= 0)
			{
				throw MapiExceptionHelper.ArgumentOutOfRangeException("properties", "pva must contain at least 1 element");
			}
			base.LockStore();
			try
			{
				int num = 0;
				foreach (PropValue propValue in properties)
				{
					num += propValue.GetBytesToMarshal();
				}
				try
				{
					fixed (byte* ptr = new byte[num])
					{
						PropValue.MarshalToNative(properties, ptr);
						int num2 = this.iMAPIFxStream.Configure(properties.Count, (SPropValue*)ptr);
						if (num2 != 0)
						{
							base.ThrowIfError("Unable to configure the object.", num2);
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

		public byte[] Download()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (this.mode != MapiFastTransferStreamMode.Download)
			{
				throw MapiExceptionHelper.NotSupportedException("Download is not supported because object was created for upload.");
			}
			base.LockStore();
			byte[] result;
			try
			{
				SafeExMemoryHandle safeExMemoryHandle = null;
				byte[] array = null;
				try
				{
					int num = 0;
					int num2 = this.iMAPIFxStream.Download(out num, out safeExMemoryHandle);
					if (num2 != 0)
					{
						base.ThrowIfError("Unable to download data.", num2);
					}
					if (num > 0 && safeExMemoryHandle != null)
					{
						array = new byte[num];
						safeExMemoryHandle.CopyTo(array, 0, num);
					}
				}
				finally
				{
					if (safeExMemoryHandle != null)
					{
						safeExMemoryHandle.Dispose();
					}
				}
				result = array;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public void Upload(ArraySegment<byte> buffer)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (this.mode != MapiFastTransferStreamMode.Upload)
			{
				throw MapiExceptionHelper.NotSupportedException("Upload is not supported because object was created for download.");
			}
			if (buffer.Count == 0)
			{
				throw MapiExceptionHelper.ArgumentException("buffer", "Buffer is empty.");
			}
			base.LockStore();
			try
			{
				int num = this.iMAPIFxStream.Upload(buffer);
				if (num != 0)
				{
					base.ThrowIfError("Unable to upload data.", num);
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public void Flush()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (this.mode != MapiFastTransferStreamMode.Upload)
			{
				throw MapiExceptionHelper.NotSupportedException("Flush is not supported because object was created for download.");
			}
			base.LockStore();
			try
			{
				int num = this.iMAPIFxStream.Flush();
				if (num != 0)
				{
					base.ThrowIfError("Unable to flush data.", num);
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		private SafeExFastTransferStreamHandle iMAPIFxStream;

		private MapiFastTransferStreamMode mode;

		public static class WellKnownIds
		{
			public static readonly Guid InferenceLog = new Guid("8ebdc484-475b-4d27-aaad-647e1cac4144");
		}
	}
}
