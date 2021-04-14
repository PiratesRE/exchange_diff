using System;

namespace Microsoft.Exchange.Transport.Sync.Worker.Throttling
{
	internal enum SyncResourceMonitorType
	{
		DatabaseRPCLatency,
		DatabaseReplicationLog,
		MailboxCPU,
		ServerTransportQueue,
		UserTransportQueue,
		Unknown
	}
}
