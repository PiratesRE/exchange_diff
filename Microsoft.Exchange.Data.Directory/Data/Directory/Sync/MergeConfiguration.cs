using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.BackSync;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class MergeConfiguration : TenantFullSyncConfiguration
	{
		private MergePageToken PageToken
		{
			get
			{
				return (MergePageToken)base.FullSyncPageToken;
			}
		}

		public MergeConfiguration(MergePageToken pageToken, Guid invocationId, OutputResultDelegate writeResult, ISyncEventLogger eventLogger, IExcludedObjectReporter excludedObjectReporter, PartitionId partitionId) : base(pageToken, invocationId, writeResult, eventLogger, excludedObjectReporter, MergePageToken.Parse(pageToken.ToByteArray()))
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "New MergeConfiguration");
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "pageToken.MergeState = {0}", pageToken.MergeState.ToString());
			if (pageToken.MergeState == MergeState.Start)
			{
				ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "Update merge state");
				this.PageToken.UpdateMergeState(partitionId);
			}
		}

		public override bool MoreData
		{
			get
			{
				return !this.PageToken.IsMergeComplete;
			}
		}

		public override IEnumerable<ADRawEntry> GetDataPage()
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "MergeConfiguration.GetDataPage entering");
			ExTraceGlobals.BackSyncTracer.TraceDebug<bool>((long)SyncConfiguration.TraceId, "MergeConfiguration.GetDataPage this.MoreData = {0}", this.MoreData);
			if (this.MoreData)
			{
				bool stateWasComplete = this.PageToken.State == TenantFullSyncState.Complete;
				ExTraceGlobals.BackSyncTracer.TraceDebug<bool>((long)SyncConfiguration.TraceId, "MergeConfiguration.GetDataPage stateWasComplete = {0}", stateWasComplete);
				foreach (ADRawEntry entry in this.GetDataPageWrapper())
				{
					ExTraceGlobals.BackSyncTracer.TraceDebug<ADObjectId>((long)SyncConfiguration.TraceId, "MergeConfiguration.GetDataPage entry {0}", entry.Id);
					yield return entry;
				}
				if (this.MoreData && this.PageToken.State == TenantFullSyncState.Complete && stateWasComplete)
				{
					ExTraceGlobals.MergeTracer.TraceError<Guid>((long)this.PageToken.TenantExternalDirectoryId.GetHashCode(), "Previous TFS token had MoreData=false but watermarks were not satisfied, and after the second call we are still in the same position. Apparently {0} is not receiving timely updates. Fail the cmdlet and suggest resuming incremental sync for now", this.PageToken.InvocationId);
					throw new BackSyncDataSourceReplicationException();
				}
			}
			yield break;
		}

		protected override void FinishFullSync()
		{
			ExTraceGlobals.MergeTracer.TraceDebug((long)this.PageToken.TenantExternalDirectoryId.GetHashCode(), "MergeConfiguration.FinishFullSync entering");
			base.FinishFullSync();
			this.PageToken.UpdateMergeState(base.TenantConfigurationSession.SessionSettings.PartitionId);
		}

		private IEnumerable<ADRawEntry> GetDataPageWrapper()
		{
			return base.GetDataPage();
		}
	}
}
