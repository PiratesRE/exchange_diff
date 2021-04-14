using System;

namespace Microsoft.Exchange.EdgeSync
{
	internal delegate bool TargetCacheRemoveTargetOnlyEntries(TargetConnection targetConnection, TestShutdownAndLeaseDelegate testShutdownAndLease, object state);
}
