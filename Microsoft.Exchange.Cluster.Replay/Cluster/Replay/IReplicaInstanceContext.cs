using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IReplicaInstanceContext
	{
		bool Initializing { get; }

		bool Resynchronizing { get; }

		bool Running { get; }

		bool Seeding { get; }
	}
}
