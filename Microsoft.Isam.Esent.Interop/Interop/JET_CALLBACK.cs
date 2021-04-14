using System;

namespace Microsoft.Isam.Esent.Interop
{
	public delegate JET_err JET_CALLBACK(JET_SESID sesid, JET_DBID dbid, JET_TABLEID tableid, JET_cbtyp cbtyp, object arg1, object arg2, IntPtr context, IntPtr unused);
}
