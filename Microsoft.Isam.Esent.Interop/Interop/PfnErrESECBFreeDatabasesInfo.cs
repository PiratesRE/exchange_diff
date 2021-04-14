using System;

namespace Microsoft.Isam.Esent.Interop
{
	public delegate JET_err PfnErrESECBFreeDatabasesInfo(ESEBACK_CONTEXT pBackupContext, int cInfo, INSTANCE_BACKUP_INFO[] rgInfo);
}
