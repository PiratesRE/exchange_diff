using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IThreadPoolThreadCountManager
	{
		bool SetMinThreads(int workerThreads, int? completionPortThreads, bool force);

		bool IncrementMinThreadsBy(int incWorkerThreadsBy, int? incCompletionPortThreadsBy);

		bool DecrementMinThreadsBy(int decWorkerThreadsBy, int? decCompletionPortThreadsBy);

		bool Reset(bool force);
	}
}
