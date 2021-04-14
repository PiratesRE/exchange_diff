using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal delegate JET_err NATIVE_CALLBACK(IntPtr sesid, uint dbid, IntPtr tableid, uint cbtyp, IntPtr arg1, IntPtr arg2, IntPtr context, IntPtr unused);
}
