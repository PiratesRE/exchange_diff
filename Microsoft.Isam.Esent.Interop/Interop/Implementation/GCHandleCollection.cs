using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop.Implementation
{
	[StructLayout(LayoutKind.Auto)]
	internal struct GCHandleCollection : IDisposable
	{
		public void Dispose()
		{
			if (this.handles != null)
			{
				foreach (GCHandle gchandle in this.handles)
				{
					gchandle.Free();
				}
				this.handles = null;
			}
		}

		public IntPtr Add(object value)
		{
			if (value == null)
			{
				return IntPtr.Zero;
			}
			if (this.handles == null)
			{
				this.handles = new List<GCHandle>();
			}
			GCHandle item = GCHandle.Alloc(value, GCHandleType.Pinned);
			this.handles.Add(item);
			return item.AddrOfPinnedObject();
		}

		private List<GCHandle> handles;
	}
}
