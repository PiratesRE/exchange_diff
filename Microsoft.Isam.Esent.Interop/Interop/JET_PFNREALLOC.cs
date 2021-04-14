using System;

namespace Microsoft.Isam.Esent.Interop
{
	[CLSCompliant(false)]
	public delegate IntPtr JET_PFNREALLOC(IntPtr context, IntPtr memory, uint requestedSize);
}
