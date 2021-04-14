using System;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	public class DeltaSyncBatchResults : DirectoryChanges, ISyncBatchResults
	{
		public byte[] LastCookie { get; set; }

		public string RawResponse { get; set; }

		public SyncBatchStatisticsBase Stats { get; set; }

		public DeltaSyncBatchResults()
		{
			this.Stats = new DeltaSyncBatchStatistics();
		}

		public DeltaSyncBatchResults(DirectoryChanges Changes) : this()
		{
			base.NextCookie = Changes.NextCookie;
			base.Objects = Changes.Objects;
			base.Links = Changes.Links;
			base.Contexts = Changes.Contexts;
			base.More = Changes.More;
			base.OperationResultCode = Changes.OperationResultCode;
		}

		public void CalculateStats()
		{
			(this.Stats as DeltaSyncBatchStatistics).Calculate(this);
		}
	}
}
