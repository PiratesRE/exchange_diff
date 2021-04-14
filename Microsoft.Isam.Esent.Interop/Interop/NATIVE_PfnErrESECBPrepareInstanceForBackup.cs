using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal delegate JET_err NATIVE_PfnErrESECBPrepareInstanceForBackup(IntPtr pBackupContext, IntPtr ulInstanceId, IntPtr pvReserved);
}
