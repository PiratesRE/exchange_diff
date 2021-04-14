using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal delegate JET_err NATIVE_PfnErrESECBFreeDatabasesInfo(IntPtr pBackupContext, uint cInfo, IntPtr rgInfo);
}
