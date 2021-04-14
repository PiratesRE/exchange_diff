using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.OAB
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class NativeBuffer : DisposeTrackableBase
	{
		public NativeBuffer(int size)
		{
			if (size < 0)
			{
				throw new ArgumentException("size");
			}
			this.size = size;
			this.buffer = Marshal.AllocHGlobal(size);
		}

		public int Size
		{
			get
			{
				return this.size;
			}
		}

		public IntPtr Buffer
		{
			get
			{
				return this.buffer;
			}
		}

		public void CopyIn(byte[] data)
		{
			if (data.Length > this.size)
			{
				throw new ArgumentException("inputData > bufferSize");
			}
			Marshal.Copy(data, 0, this.buffer, data.Length);
		}

		public byte[] CopyOut(int dataSize)
		{
			if (dataSize > this.size)
			{
				throw new ArgumentException("dataSize > bufferSize");
			}
			byte[] array = new byte[dataSize];
			Marshal.Copy(this.buffer, array, 0, dataSize);
			return array;
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing && this.buffer != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.buffer);
				this.buffer = IntPtr.Zero;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<NativeBuffer>(this);
		}

		private readonly int size;

		private IntPtr buffer;
	}
}
