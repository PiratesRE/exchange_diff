using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class NullConfigurationUpdater : IRunConfigurationUpdater
	{
		public ReplayQueuedItemBase NotifyChangedReplayConfiguration(Guid dbGuid, bool waitForCompletion, bool exitAfterEnqueueing, bool isHighPriority, bool forceRestart, ReplayConfigChangeHints changeHint, int waitTimeoutMs = -1)
		{
			return null;
		}

		public ReplayQueuedItemBase NotifyChangedReplayConfiguration(Guid dbGuid, bool waitForCompletion, bool exitAfterEnqueueing, bool isHighPriority, ReplayConfigChangeHints changeHint, int waitTimeoutMs = -1)
		{
			return null;
		}

		public ReplayQueuedItemBase NotifyChangedReplayConfiguration(Guid dbGuid, bool waitForCompletion, ReplayConfigChangeHints changeHint, int waitTimeoutMs = -1)
		{
			return null;
		}

		public void RunConfigurationUpdater(bool waitForCompletion, ReplayConfigChangeHints changeHint)
		{
		}

		public void Start()
		{
		}

		public void PrepareToStop()
		{
		}

		public void Stop()
		{
		}
	}
}
