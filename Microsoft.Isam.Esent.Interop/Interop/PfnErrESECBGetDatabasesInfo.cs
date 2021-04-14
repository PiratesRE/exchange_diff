using System;

namespace Microsoft.Isam.Esent.Interop
{
	public delegate JET_err PfnErrESECBGetDatabasesInfo(ESEBACK_CONTEXT pBackupContext, out INSTANCE_BACKUP_INFO[] prgInfo);
}
