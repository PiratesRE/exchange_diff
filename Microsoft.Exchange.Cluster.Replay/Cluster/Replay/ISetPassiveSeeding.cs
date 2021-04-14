using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ISetPassiveSeeding : IGetStatus
	{
		void BeginPassiveSeeding(PassiveSeedingSourceContextEnum PassiveSeedingSourceContext, bool invokedForRestart);

		void EndPassiveSeeding();

		PassiveSeedingSourceContextEnum PassiveSeedingSourceContext { get; }
	}
}
