using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal delegate JET_err NATIVE_PfnErrESECBDoneWithInstanceForBackup(IntPtr pBackupContext, IntPtr ulInstanceId, uint fComplete, IntPtr pvReserved);
}
