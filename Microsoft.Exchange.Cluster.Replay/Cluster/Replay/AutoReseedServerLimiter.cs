using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AutoReseedServerLimiter
	{
		public AutoReseedServerLimiter(IEnumerable<CopyStatusClientCachedEntry> serverCopyStatuses)
		{
			if (serverCopyStatuses != null)
			{
				foreach (CopyStatusClientCachedEntry copyStatusClientCachedEntry in serverCopyStatuses)
				{
					if (copyStatusClientCachedEntry.Result == CopyStatusRpcResult.Success && !copyStatusClientCachedEntry.IsActive)
					{
						if (copyStatusClientCachedEntry.CopyStatus.CopyStatus == CopyStatusEnum.Seeding)
						{
							this.m_seedsInProgress++;
						}
						if (copyStatusClientCachedEntry.CopyStatus.ContentIndexStatus == ContentIndexStatusType.Seeding)
						{
							this.m_ciSeedsInProgress++;
						}
					}
				}
			}
		}

		public bool TryStartSeed(out int maximumLimit)
		{
			maximumLimit = this.m_maxSeedsInParallel;
			if (this.m_seedsInProgress >= this.m_maxSeedsInParallel)
			{
				return false;
			}
			this.m_seedsInProgress++;
			return true;
		}

		public bool TryStartCiSeed(out int maximumLimit)
		{
			maximumLimit = this.m_maxCiSeedsInParallel;
			if (this.m_ciSeedsInProgress >= this.m_maxCiSeedsInParallel)
			{
				return false;
			}
			this.m_ciSeedsInProgress++;
			return true;
		}

		private readonly int m_maxSeedsInParallel = RegistryParameters.AutoReseedDbMaxConcurrentSeeds;

		private readonly int m_maxCiSeedsInParallel = RegistryParameters.AutoReseedCiMaxConcurrentSeeds;

		private int m_seedsInProgress;

		private int m_ciSeedsInProgress;
	}
}
