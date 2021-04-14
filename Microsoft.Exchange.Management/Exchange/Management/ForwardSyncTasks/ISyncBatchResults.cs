using System;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	public interface ISyncBatchResults
	{
		SyncBatchStatisticsBase Stats { get; set; }

		string RawResponse { get; set; }

		void CalculateStats();
	}
}
