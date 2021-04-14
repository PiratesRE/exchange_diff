using System;

namespace Microsoft.Exchange.Server.Storage.HA
{
	internal unsafe delegate int DoneWithInstanceForBackupDelegate(_ESEBACK_CONTEXT* pBackupContext, ulong ulInstanceId, uint fComplete, void* pvReserved);
}
