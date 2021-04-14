using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal enum NetError : uint
	{
		NERR_Success,
		NERR_BASE = 2100U,
		NERR_UnknownDevDir = 2116U,
		NERR_DuplicateShare = 2118U,
		NERR_BufTooSmall = 2123U,
		NERR_NetNameNotFound = 2310U
	}
}
