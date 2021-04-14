using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal interface IStoreMountDismount : IDisposable
	{
		void MountDatabase(Guid guidStorageGroup, Guid guidMdb, int flags);

		void UnmountDatabase(Guid guidStorageGroup, Guid guidMdb, int flags);
	}
}
