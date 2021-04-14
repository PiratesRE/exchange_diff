using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class NoncachingReplayAdObjectLookupPartiallyConsistent : NoncachingReplayAdObjectLookup
	{
		public NoncachingReplayAdObjectLookupPartiallyConsistent()
		{
			base.Initialize(ConsistencyMode.PartiallyConsistent);
		}
	}
}
