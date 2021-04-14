using System;

namespace Microsoft.Isam.Esent.Interop
{
	public delegate JET_err PfnErrESECBPrepareInstanceForBackup(ESEBACK_CONTEXT pBackupContext, JET_INSTANCE ulInstanceId);
}
