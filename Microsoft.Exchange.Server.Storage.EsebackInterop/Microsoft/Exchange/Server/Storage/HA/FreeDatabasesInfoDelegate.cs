using System;

namespace Microsoft.Exchange.Server.Storage.HA
{
	internal unsafe delegate int FreeDatabasesInfoDelegate(_ESEBACK_CONTEXT* pBackupContext, uint cInfo, _INSTANCE_BACKUP_INFO* rgInfo);
}
