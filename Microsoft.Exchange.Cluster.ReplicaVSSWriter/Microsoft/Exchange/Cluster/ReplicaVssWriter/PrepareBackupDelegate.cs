using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Cluster.ReplicaVssWriter
{
	[return: MarshalAs(UnmanagedType.U1)]
	internal unsafe delegate bool PrepareBackupDelegate(IVssWriterComponents* pComponents);
}
