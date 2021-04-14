using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal delegate JET_err NATIVE_PfnErrESECBGetDatabasesInfo(IntPtr pBackupContext, out uint pcInfo, out IntPtr prgInfo, uint fReserved);
}
