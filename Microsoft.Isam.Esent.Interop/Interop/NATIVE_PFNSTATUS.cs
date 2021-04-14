using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal delegate JET_err NATIVE_PFNSTATUS(IntPtr nativeSesid, uint snp, uint snt, IntPtr snprog);
}
