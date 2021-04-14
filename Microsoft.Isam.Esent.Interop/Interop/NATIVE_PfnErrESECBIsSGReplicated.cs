using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal delegate JET_err NATIVE_PfnErrESECBIsSGReplicated(IntPtr pContext, IntPtr ulInstanceId, out int pfReplicated, uint cbSGGuid, IntPtr wszSGGuid, out uint pcInfo, out IntPtr prgInfo);
}
