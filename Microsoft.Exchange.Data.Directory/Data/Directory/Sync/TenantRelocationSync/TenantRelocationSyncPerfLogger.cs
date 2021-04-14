using System;

namespace Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync
{
	internal class TenantRelocationSyncPerfLogger
	{
		internal TenantRelocationSyncPerfLogger(TenantRelocationSyncData syncData)
		{
			this.syncData = syncData;
			this.timestampOfLastCheckpoint = DateTime.UtcNow;
		}

		internal void IncrementPlaceHolderCount()
		{
			this.placeholderCount++;
			this.IncrementProcessedObjectCount();
		}

		internal void IncrementUpdateCount()
		{
			this.updateCount++;
			this.IncrementProcessedObjectCount();
		}

		internal void IncrementDeleteCount()
		{
			this.deleteCount++;
			this.IncrementProcessedObjectCount();
		}

		internal void IncrementRenameCount()
		{
			this.renameCount++;
			this.IncrementProcessedObjectCount();
		}

		internal void AddLinkCount(int delta)
		{
			this.linkCount += delta;
		}

		internal void IncrementPageCount()
		{
			this.pageCount++;
		}

		internal void Flush()
		{
			DateTime utcNow = DateTime.UtcNow;
			TimeSpan timeSpan = utcNow - this.timestampOfLastCheckpoint;
			int num = this.processedObjectCount % TenantRelocationSyncPerfLogger.CheckpointObjectNumber;
			num = ((num == 0) ? TenantRelocationSyncPerfLogger.CheckpointObjectNumber : num);
			double num2 = (timeSpan.TotalSeconds == 0.0) ? -1.0 : ((double)num / timeSpan.TotalSeconds);
			TenantRelocationSyncLogger.Instance.Log(this.syncData, "Info", null, string.Format("Page No. {0}: {1} objects processed, {2} placeholders, {3} updates, {4} deletions, {5} renames, {6} links, rate: {7} objects/s", new object[]
			{
				this.pageCount,
				this.processedObjectCount,
				this.placeholderCount,
				this.updateCount,
				this.deleteCount,
				this.renameCount,
				this.linkCount,
				num2
			}), null);
		}

		private void IncrementProcessedObjectCount()
		{
			this.processedObjectCount++;
			if (this.processedObjectCount % TenantRelocationSyncPerfLogger.CheckpointObjectNumber == 0)
			{
				this.Flush();
				this.timestampOfLastCheckpoint = DateTime.UtcNow;
			}
		}

		private static readonly int CheckpointObjectNumber = 100;

		private TenantRelocationSyncData syncData;

		private int pageCount;

		private int processedObjectCount;

		private int placeholderCount;

		private int deleteCount;

		private int renameCount;

		private int linkCount;

		private int updateCount;

		private DateTime timestampOfLastCheckpoint;
	}
}
