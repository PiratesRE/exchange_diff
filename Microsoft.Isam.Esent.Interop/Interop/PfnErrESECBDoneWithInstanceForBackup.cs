using System;

namespace Microsoft.Isam.Esent.Interop
{
	public delegate JET_err PfnErrESECBDoneWithInstanceForBackup(ESEBACK_CONTEXT pBackupContext, JET_INSTANCE ulInstanceId, BackupDone fComplete);
}
