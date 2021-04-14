using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PSTImportSubscriptionSnapshot : SubscriptionSnapshot
	{
		public PSTImportSubscriptionSnapshot(ISubscriptionId id, SnapshotStatus status, bool isInitialSyncComplete, ExDateTime createTime, ExDateTime? lastUpdateTime, ExDateTime? lastSyncTime, LocalizedString? errorMessage, string batchName, bool primaryOnly, bool archiveOnly, string pstFilePath) : base(id, status, isInitialSyncComplete, createTime, lastUpdateTime, lastSyncTime, errorMessage, batchName)
		{
			this.PrimaryOnly = primaryOnly;
			this.ArchiveOnly = archiveOnly;
			this.PstFilePath = pstFilePath;
		}

		public bool PrimaryOnly { get; private set; }

		public bool ArchiveOnly { get; private set; }

		public string PstFilePath { get; set; }
	}
}
