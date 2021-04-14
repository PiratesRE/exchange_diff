using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class SafeBuffer : DisposableBase.Finalizable
	{
		internal SafeBuffer(int cb)
		{
			this.buf = new byte[cb];
			if (cb >= 4)
			{
				this.gc = GCHandle.Alloc(this.buf, GCHandleType.Pinned);
			}
		}

		internal SafeBuffer(byte[] b)
		{
			this.buf = b;
			if (b.Length >= 4)
			{
				this.gc = GCHandle.Alloc(this.buf, GCHandleType.Pinned);
			}
		}

		internal byte[] Buffer
		{
			get
			{
				return this.buf;
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				Array.Clear(this.buf, 0, this.buf.Length);
			}
			if (this.gc.IsAllocated)
			{
				this.gc.Free();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SafeBuffer>(this);
		}

		private byte[] buf;

		private GCHandle gc;
	}
}
