using System;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal enum AmDbLockReason
	{
		Mount = 1,
		Dismount,
		Move,
		Remount,
		UpdatePerfCounter
	}
}
