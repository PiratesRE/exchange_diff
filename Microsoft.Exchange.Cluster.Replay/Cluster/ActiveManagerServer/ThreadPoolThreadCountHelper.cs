using System;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal static class ThreadPoolThreadCountHelper
	{
		internal static void IncreaseForDatabaseOperations(int numDatabaseOps)
		{
			int numThreadsPerPamDbOperation = RegistryParameters.NumThreadsPerPamDbOperation;
			ThreadPoolThreadCountHelper.IncrementBy(numThreadsPerPamDbOperation * numDatabaseOps);
		}

		internal static void IncreaseForServerOperations(AmConfig cfg)
		{
			int threadCount = cfg.IsPamOrSam ? cfg.DagConfig.MemberServers.Length : 1;
			ThreadPoolThreadCountHelper.IncrementBy(threadCount);
		}

		internal static void IncrementBy(int threadCount)
		{
			Dependencies.ThreadPoolThreadCountManager.IncrementMinThreadsBy(threadCount, null);
		}

		internal static void Reset()
		{
			Dependencies.ThreadPoolThreadCountManager.Reset(true);
		}
	}
}
