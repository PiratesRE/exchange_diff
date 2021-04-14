using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IQueuedCallback
	{
		DateTime CreateTimeUtc { get; }

		DateTime StartTimeUtc { set; }

		DateTime EndTimeUtc { set; }

		bool IsEquivalentOrSuperset(IQueuedCallback otherCallback);

		void Cancel();

		void Execute();

		void ReportStatus(QueuedItemStatus status);

		Exception LastException { get; }
	}
}
