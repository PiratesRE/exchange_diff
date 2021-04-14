using System;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Story.V1.GraphicsInterop
{
	internal class PinHelper : IDisposable
	{
		public PinHelper(object o)
		{
			this._handle = GCHandle.Alloc(o, GCHandleType.Pinned);
		}

		public IntPtr Addr
		{
			get
			{
				return this._handle.AddrOfPinnedObject();
			}
		}

		~PinHelper()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this._handle.IsAllocated)
			{
				this._handle.Free();
			}
		}

		private GCHandle _handle;
	}
}
