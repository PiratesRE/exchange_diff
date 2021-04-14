using System;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	public class BacklogEstimateResults : BacklogEstimateBatch
	{
		public TimeSpan ResponseTime { get; set; }

		public string RawResponse { get; set; }

		public BacklogEstimateResults(BacklogEstimateBatch batch)
		{
			base.ContextBacklogs = batch.ContextBacklogs;
			base.NextPageToken = batch.NextPageToken;
			base.StatusCode = batch.StatusCode;
		}
	}
}
