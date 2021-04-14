using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal class FirstTimeFolderSync : FolderSync
	{
		public FirstTimeFolderSync(ISyncProvider syncProvider, IFolderSyncState syncState, ConflictResolutionPolicy policy, bool deferStateModifications) : base(syncProvider, syncState, policy, deferStateModifications)
		{
		}

		public string CollectionId { get; set; }

		public override void UndoServerOperations()
		{
			base.UndoServerOperations();
			if (this.backupCurFTSMaxWatermarkHasBeenSet)
			{
				if (this.backupCurFTSMaxWatermark != null)
				{
					this.CurFTSMaxWatermark = this.backupCurFTSMaxWatermark;
					return;
				}
				this.syncState.Remove(SyncStateProp.CurFTSMaxWatermark);
			}
		}

		private ISyncWatermark CurFTSMaxWatermark
		{
			get
			{
				ISyncWatermark syncWatermark = this.syncState.Contains(SyncStateProp.CurFTSMaxWatermark) ? ((ISyncWatermark)this.syncState[SyncStateProp.CurFTSMaxWatermark]) : null;
				if (!this.backupCurFTSMaxWatermarkHasBeenSet && this.shouldBackUpState)
				{
					if (syncWatermark != null)
					{
						this.backupCurFTSMaxWatermark = (ISyncWatermark)syncWatermark.Clone();
					}
					this.backupCurFTSMaxWatermarkHasBeenSet = true;
				}
				return syncWatermark;
			}
			set
			{
				if (!this.backupCurFTSMaxWatermarkHasBeenSet && this.shouldBackUpState)
				{
					if (this.syncState.Contains(SyncStateProp.CurFTSMaxWatermark))
					{
						this.backupCurFTSMaxWatermark = (ISyncWatermark)this.syncState[SyncStateProp.CurFTSMaxWatermark];
					}
					this.backupCurFTSMaxWatermarkHasBeenSet = true;
				}
				this.syncState[SyncStateProp.CurFTSMaxWatermark] = value;
			}
		}

		private ISyncWatermark PrevFTSMaxWatermark
		{
			get
			{
				base.CommitModifyState(this.prevMaxFTSWatermarkModifiers);
				if (!this.syncState.Contains(SyncStateProp.PrevFTSMaxWatermark))
				{
					return null;
				}
				return (ISyncWatermark)this.syncState[SyncStateProp.PrevFTSMaxWatermark];
			}
			set
			{
				base.CommitModifyState(this.prevMaxFTSWatermarkModifiers);
				this.syncState[SyncStateProp.PrevFTSMaxWatermark] = value;
			}
		}

		protected override void InitializeAllItemsInFilter(QueryBasedSyncFilter queryBasedSyncFilter)
		{
			FirstTimeSyncProvider firstTimeSyncProvider = this.syncProvider as FirstTimeSyncProvider;
			if (firstTimeSyncProvider != null)
			{
				ExTraceGlobals.SyncProcessTracer.Information((long)this.GetHashCode(), "FirstTimeFolderSync.InitializeAllItemsInFilter.  Using FTS Provider");
				queryBasedSyncFilter.EntriesInFilter.Clear();
				FirstTimeSyncWatermark minWatermark = firstTimeSyncProvider.GetNewFirstTimeSyncWatermark() as FirstTimeSyncWatermark;
				if (Command.CurrentCommand != null)
				{
					Command.CurrentCommand.ProtocolLogger.SetProviderSyncType(this.CollectionId, ProviderSyncType.FCS);
					Command.CurrentCommand.Context.SetDiagnosticValue(AirSyncConditionalHandlerSchema.FilterChangeSync, true);
					if (base.IsFirstSyncScenario)
					{
						Command.CurrentCommand.Context.SetDiagnosticValue(AirSyncConditionalHandlerSchema.InitialSync, true);
					}
				}
				firstTimeSyncProvider.FirstTimeSync(null, minWatermark, queryBasedSyncFilter.FilterQuery, -1, queryBasedSyncFilter.EntriesInFilter);
				return;
			}
			ExTraceGlobals.SyncProcessTracer.Information((long)this.GetHashCode(), "FirstTimeFolderSync.InitializeAllItemsInFilter.  Using Old Provider");
			base.InitializeAllItemsInFilter(queryBasedSyncFilter);
		}

		protected override void UpdateWatermarkFromMinReceivedDate(ExDateTime minReceivedDate)
		{
			FirstTimeSyncWatermark firstTimeSyncWatermark = this.CurFTSMaxWatermark as FirstTimeSyncWatermark;
			if (firstTimeSyncWatermark == null)
			{
				this.CurFTSMaxWatermark = FirstTimeSyncWatermark.Create(minReceivedDate, 0);
				return;
			}
			if (firstTimeSyncWatermark.ReceivedDateUtc == ExDateTime.MinValue || firstTimeSyncWatermark.ReceivedDateUtc > minReceivedDate)
			{
				firstTimeSyncWatermark.Update(0, false, minReceivedDate);
			}
		}

		protected override bool GetNewOperations(int windowSize, Dictionary<ISyncItemId, ServerManifestEntry> tempServerManifest)
		{
			if (base.CurSnapShotWatermark == null)
			{
				if (Command.CurrentCommand != null)
				{
					Command.CurrentCommand.ProtocolLogger.SetProviderSyncType(this.CollectionId, ProviderSyncType.ICS);
					Command.CurrentCommand.Context.SetDiagnosticValue(AirSyncConditionalHandlerSchema.IcsSync, true);
				}
				return this.syncProvider.GetNewOperations(base.CurMaxWatermark, null, true, windowSize, null, tempServerManifest);
			}
			FirstTimeSyncProvider firstTimeSyncProvider = this.syncProvider as FirstTimeSyncProvider;
			if (this.CurFTSMaxWatermark == null)
			{
				this.CurFTSMaxWatermark = firstTimeSyncProvider.GetNewFirstTimeSyncWatermark();
			}
			ExTraceGlobals.SyncProcessTracer.Information<ExDateTime>((long)this.GetHashCode(), "FirstTimeFolderSync.GetNewOperations.  Performing FirstTimeSync with MaxWatermark '{0}'", (this.CurFTSMaxWatermark as FirstTimeSyncWatermark).ReceivedDateUtc);
			if (Command.CurrentCommand != null)
			{
				Command.CurrentCommand.ProtocolLogger.SetProviderSyncType(this.CollectionId, ProviderSyncType.IQ);
				Command.CurrentCommand.Context.SetDiagnosticValue(AirSyncConditionalHandlerSchema.ItemQuerySync, true);
			}
			return firstTimeSyncProvider.FirstTimeSync(base.ClientState, this.CurFTSMaxWatermark as FirstTimeSyncWatermark, FolderSync.ComputeQueryHint(this.filters), windowSize, tempServerManifest);
		}

		protected override void MarkFirstTimeSyncAsComplete()
		{
			base.MarkFirstTimeSyncAsComplete();
			this.CurFTSMaxWatermark = null;
		}

		protected override void CommitPreviousState()
		{
			ISyncWatermark prevMaxWatermark;
			ISyncWatermark prevFTSMaxWatermark;
			Dictionary<ISyncItemId, ServerManifestEntry> prevServerManifest;
			string prevFilterId;
			Dictionary<ISyncItemId, ServerManifestEntry> prevDelayedServerOperationQueue;
			ISyncWatermark prevSnapShotWatermark;
			bool prevLastSyncConversationMode;
			(this.acknowledgeModifications as FirstTimeFolderSync.FirstTimeAcknowledgeModifications).CommitPreviousState(out prevMaxWatermark, out prevFTSMaxWatermark, out prevServerManifest, out prevFilterId, out prevDelayedServerOperationQueue, out prevSnapShotWatermark, out prevLastSyncConversationMode);
			base.PrevMaxWatermark = prevMaxWatermark;
			this.PrevFTSMaxWatermark = prevFTSMaxWatermark;
			base.PrevServerManifest = prevServerManifest;
			base.PrevFilterId = prevFilterId;
			base.PrevDelayedServerOperationQueue = prevDelayedServerOperationQueue;
			base.PrevSnapShotWatermark = prevSnapShotWatermark;
			base.PrevLastSyncConversationMode = prevLastSyncConversationMode;
		}

		protected override void SavePreviousState()
		{
			(this.acknowledgeModifications as FirstTimeFolderSync.FirstTimeAcknowledgeModifications).SavePreviousState((ISyncWatermark)base.CurMaxWatermark.Clone(), (this.CurFTSMaxWatermark != null) ? ((ISyncWatermark)this.CurFTSMaxWatermark.Clone()) : null, FolderSync.CloneDictionary(base.CurServerManifest), base.CurFilterId, FolderSync.CloneDictionary(base.CurDelayedServerOperationQueue), (base.CurSnapShotWatermark != null) ? ((ISyncWatermark)base.CurSnapShotWatermark.Clone()) : null, base.CurLastSyncConversationMode);
		}

		protected override FolderSync.AcknowledgeModifications CreateAcknowledgeModifications()
		{
			return new FirstTimeFolderSync.FirstTimeAcknowledgeModifications();
		}

		private List<FolderSync.StateModifier> prevMaxFTSWatermarkModifiers = new List<FolderSync.StateModifier>(5);

		private ISyncWatermark backupCurFTSMaxWatermark;

		private bool backupCurFTSMaxWatermarkHasBeenSet;

		private class FirstTimeAcknowledgeModifications : FolderSync.AcknowledgeModifications
		{
			public void CommitPreviousState(out ISyncWatermark prevMaxWatermark, out ISyncWatermark prevFTSMaxWatermark, out Dictionary<ISyncItemId, ServerManifestEntry> prevServerManifest, out string prevFilterId, out Dictionary<ISyncItemId, ServerManifestEntry> prevDelayedServerOperationQueue, out ISyncWatermark prevSnapShotWatermark, out bool prevLastSyncConversationMode)
			{
				base.CommitPreviousState(out prevMaxWatermark, out prevServerManifest, out prevFilterId, out prevDelayedServerOperationQueue, out prevSnapShotWatermark, out prevLastSyncConversationMode);
				prevFTSMaxWatermark = this.prevFTSMaxWatermark;
			}

			public void SavePreviousState(ISyncWatermark prevMaxWatermark, ISyncWatermark prevFTSMaxWatermark, Dictionary<ISyncItemId, ServerManifestEntry> prevServerManifest, string prevFilterId, Dictionary<ISyncItemId, ServerManifestEntry> prevDelayedServerOperationQueue, ISyncWatermark prevSnapShotWatermark, bool prevLastSyncConversationMode)
			{
				base.SavePreviousState(prevMaxWatermark, prevServerManifest, prevFilterId, prevDelayedServerOperationQueue, prevSnapShotWatermark, prevLastSyncConversationMode);
				this.prevFTSMaxWatermark = prevFTSMaxWatermark;
			}

			private ISyncWatermark prevFTSMaxWatermark;
		}
	}
}
