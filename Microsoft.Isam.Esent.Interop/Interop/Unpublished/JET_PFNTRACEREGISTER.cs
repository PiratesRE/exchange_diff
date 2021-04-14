using System;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	public delegate void JET_PFNTRACEREGISTER(JET_tracetag tag, string description, out IntPtr context);
}
