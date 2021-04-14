using System;

namespace Microsoft.Exchange.EdgeSync
{
	internal delegate void LoadTargetCache(TargetConnection targetConnection, TestShutdownAndLeaseDelegate testShutdownAndLease, object state);
}
