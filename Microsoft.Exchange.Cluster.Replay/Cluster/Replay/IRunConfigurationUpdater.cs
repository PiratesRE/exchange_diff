using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IRunConfigurationUpdater
	{
		void Start();

		void PrepareToStop();

		void Stop();

		void RunConfigurationUpdater(bool waitForCompletion, ReplayConfigChangeHints changeHint);

		ReplayQueuedItemBase NotifyChangedReplayConfiguration(Guid dbGuid, bool waitForCompletion, ReplayConfigChangeHints changeHint, int waitTimeoutMs = -1);

		ReplayQueuedItemBase NotifyChangedReplayConfiguration(Guid dbGuid, bool waitForCompletion, bool exitAfterEnqueueing, bool isHighPriority, ReplayConfigChangeHints changeHint, int waitTimeoutMs = -1);

		ReplayQueuedItemBase NotifyChangedReplayConfiguration(Guid dbGuid, bool waitForCompletion, bool exitAfterEnqueueing, bool isHighPriority, bool forceRestart, ReplayConfigChangeHints changeHint, int waitTimeoutMs = -1);
	}
}
