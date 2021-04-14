using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RestartInstanceWrapper
	{
		internal RestartInstanceWrapper(ReplicaInstanceContainer oldReplicaInstance)
		{
			this.OldReplicaInstance = oldReplicaInstance;
			this.IdentityGuid = oldReplicaInstance.ReplicaInstance.Configuration.IdentityGuid;
			this.Identity = oldReplicaInstance.ReplicaInstance.Configuration.Identity;
			this.DisplayName = oldReplicaInstance.ReplicaInstance.Configuration.DisplayName;
		}

		internal ReplicaInstanceContainer OldReplicaInstance { get; private set; }

		internal Guid IdentityGuid { get; private set; }

		internal string Identity { get; private set; }

		internal string DisplayName { get; private set; }
	}
}
