using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReplicaInstances : SafeInstanceTable<ReplicaInstance>
	{
		internal bool TryGetBackupReplicaInstance(Guid guid, out ReplicaInstance instance)
		{
			string identityFromGuid = ReplayConfiguration.GetIdentityFromGuid(guid);
			return base.TryGetInstance(identityFromGuid, out instance);
		}
	}
}
