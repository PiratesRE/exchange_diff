using System;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	public class TenantSyncBatchResults : DirectoryObjectsAndLinks, ISyncBatchResults
	{
		public byte[] LastPageToken { get; set; }

		public string RawResponse { get; set; }

		public SyncBatchStatisticsBase Stats { get; set; }

		public TenantSyncBatchResults()
		{
			this.Stats = new TenantSyncBatchStatistics();
		}

		public TenantSyncBatchResults(DirectoryObjectsAndLinks ObjectsAndLinks) : this()
		{
			base.Objects = ObjectsAndLinks.Objects;
			base.Links = ObjectsAndLinks.Links;
			base.More = ObjectsAndLinks.More;
			base.Errors = ObjectsAndLinks.Errors;
		}

		public void CalculateStats()
		{
			(this.Stats as TenantSyncBatchStatistics).Calculate(this);
		}
	}
}
