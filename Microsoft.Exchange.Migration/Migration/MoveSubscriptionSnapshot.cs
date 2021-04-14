using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MoveSubscriptionSnapshot : SubscriptionSnapshot
	{
		public MoveSubscriptionSnapshot(ISubscriptionId id, SnapshotStatus status, bool isInitialSyncComplete, ExDateTime createTime, ExDateTime? lastUpdateTime, ExDateTime? lastSyncTime, LocalizedString? errorMessage, string batchName, MigrationBatchDirection direction, bool primaryOnly, bool archiveOnly, string targetDatabase, string targetArchiveDatabase) : base(id, status, isInitialSyncComplete, createTime, lastUpdateTime, lastSyncTime, errorMessage, batchName)
		{
			this.Direction = direction;
			this.PrimaryOnly = primaryOnly;
			this.ArchiveOnly = archiveOnly;
			this.TargetDatabase = targetDatabase;
			this.TargetArchiveDatabase = targetArchiveDatabase;
		}

		public MigrationBatchDirection Direction { get; private set; }

		public bool PrimaryOnly { get; private set; }

		public bool ArchiveOnly { get; private set; }

		public string TargetDatabase { get; private set; }

		public string TargetArchiveDatabase { get; private set; }
	}
}
