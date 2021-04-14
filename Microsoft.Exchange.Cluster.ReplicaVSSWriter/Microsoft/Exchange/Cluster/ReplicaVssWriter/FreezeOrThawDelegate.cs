using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Cluster.ReplicaVssWriter
{
	internal delegate int FreezeOrThawDelegate([MarshalAs(UnmanagedType.U1)] bool fFreeze, [MarshalAs(UnmanagedType.U1)] bool fLock);
}
