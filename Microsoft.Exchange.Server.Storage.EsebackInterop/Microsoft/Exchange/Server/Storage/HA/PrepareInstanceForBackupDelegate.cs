using System;

namespace Microsoft.Exchange.Server.Storage.HA
{
	internal unsafe delegate int PrepareInstanceForBackupDelegate(_ESEBACK_CONTEXT* pBackupContext, ulong ulInstanceId, void* pvReserved);
}
