using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ISetActiveSeeding : IGetStatus
	{
		void BeginActiveSeeding(SeedType seedType);

		void EndActiveSeeding();

		bool ActiveSeedingSource { get; }

		SeedType SeedType { get; }
	}
}
