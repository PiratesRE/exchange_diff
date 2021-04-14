using System;

namespace Microsoft.Exchange.Server.Storage.HA
{
	internal unsafe delegate int GetDatabasesInfoDelegate(_ESEBACK_CONTEXT* pBackupContext, uint* pcInfo, _INSTANCE_BACKUP_INFO** prgInfo, uint fReserved);
}
