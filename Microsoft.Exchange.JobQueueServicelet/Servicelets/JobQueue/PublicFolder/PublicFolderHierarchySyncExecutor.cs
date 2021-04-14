using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Common.IL;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.PublicFolder;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ServiceHost.PublicFolder;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Servicelets.JobQueue.PublicFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class PublicFolderHierarchySyncExecutor : IHierarchySyncExecutor
	{
		protected PublicFolderHierarchySyncExecutor(PublicFolderSynchronizerContext syncContext, IBudget budget, string callerInfo)
		{
			ArgumentValidator.ThrowIfNull("syncContext", syncContext);
			this.syncContext = syncContext;
			this.performanceLogger = new PublicFolderPerformanceLogger(this.syncContext);
			this.transientExceptionHandler = new TransientExceptionHandler(PublicFolderHierarchySyncExecutor.Tracer, 3, PublicFolderHierarchySyncExecutor.RetryDelayOnTransientException, new FilterDelegate(null, (UIntPtr)ldftn(ShouldRetryException)), this.syncContext.CorrelationId, null, budget, callerInfo);
			this.batchSize = PublicFolderHierarchySyncExecutor.GetConfigValue("FoldersPerHierarchySyncBatch", "SyncBatchSize", 3);
		}

		protected PublicFolderSynchronizerContext SyncContext
		{
			get
			{
				return this.syncContext;
			}
		}

		protected PublicFolderPerformanceLogger PerformanceLogger
		{
			get
			{
				return this.performanceLogger;
			}
		}

		protected bool IsFirstBatch
		{
			get
			{
				return this.batchNumber == 1;
			}
		}

		private protected MailboxChangesManifest HierarchyChangeManifest { protected get; private set; }

		protected abstract bool IsLastSync { get; }

		protected abstract EnumerateHierarchyChangesFlags EnumerateHierarchyChangesFlags { get; }

		protected abstract int MaxHierarchyChanges { get; }

		public static PublicFolderHierarchySyncExecutor CreateForSingleFolderSync(PublicFolderSynchronizerContext syncContext)
		{
			ArgumentValidator.ThrowIfNull("syncContext", syncContext);
			return new PublicFolderHierarchySyncExecutor.SyncAllAndProcessEach(syncContext, null, null);
		}

		public static PublicFolderHierarchySyncExecutor Create(PublicFolderSynchronizerContext syncContext, IBudget budget, string callerInfo)
		{
			ArgumentValidator.ThrowIfNull("syncContext", syncContext);
			bool config = ConfigBase<MRSConfigSchema>.GetConfig<bool>("CanExportFoldersInBatch");
			bool flag = config && syncContext.SourceMailbox.IsCapabilitySupported(MRSProxyCapabilities.SetItemProperties) && syncContext.SourceMailbox.IsCapabilitySupported(MRSProxyCapabilities.ExportFolders) && syncContext.SourceMailbox.IsCapabilitySupported(MRSProxyCapabilities.PagedEnumeration);
			if (syncContext.Logger != null)
			{
				syncContext.Logger.LogEvent(LogEventType.Verbose, string.Format("Creating PublicFolderHierarchySyncExecutor. IsBulkProcess={0}, CanExportFoldersInBatch={1}, Server Version Information='{2}'", flag, config, ((RemoteMailbox)syncContext.SourceMailbox).ServerVersion));
			}
			if (flag)
			{
				return new PublicFolderHierarchySyncExecutor.PagedSyncAndBulkProcess(syncContext, budget, callerInfo);
			}
			return new PublicFolderHierarchySyncExecutor.SyncAllAndProcessEach(syncContext, budget, callerInfo);
		}

		public void SyncSingleFolder(byte[] folderId)
		{
			CommonUtils.CatchKnownExceptions(delegate
			{
				PublicFolderHierarchySyncExecutor.PublicFolderRec publicFolderRec;
				this.CoreSyncFolderUpdate(folderId, out publicFolderRec);
				if (publicFolderRec != null && publicFolderRec.DumpsterFolderId != null)
				{
					PublicFolderHierarchySyncExecutor.PublicFolderRec publicFolderRec2;
					this.CoreSyncFolderUpdate(publicFolderRec.DumpsterFolderId, out publicFolderRec2);
				}
			}, delegate(Exception syncException)
			{
				if (CommonUtils.IsTransientException(syncException))
				{
					throw new PublicFolderSyncTransientException(ServerStrings.PublicFolderSyncFolderFailed(CommonUtils.FullExceptionMessage(syncException, true)));
				}
				throw new PublicFolderSyncPermanentException(ServerStrings.PublicFolderSyncFolderFailed(CommonUtils.FullExceptionMessage(syncException, true)));
			});
		}

		public bool ProcessNextBatch()
		{
			this.performanceLogger.InitializeCounters(this.batchNumber);
			bool result;
			try
			{
				using (this.performanceLogger.GetTaskFrame(SyncActivity.ProcessNextBatch))
				{
					if (this.batchNumber > 10 && ExEnvironment.IsTest && ExEnvironment.GetTestRegistryValue("\\PublicFolder", "CheckPointBreak", 0) == 1)
					{
						throw new PublicFolderSyncPermanentException(ServerStrings.PublicFolderSyncFolderHierarchyFailed("This happens only in Test Topology. This is to test checkpoint logic. Please rerun the synchronizer to sync from the point where the synchronizer failed"));
					}
					result = this.InternalProcessNextBatch();
				}
			}
			finally
			{
				this.performanceLogger.WriteActivitiesCountersToLog();
			}
			return result;
		}

		public void HandleException(Exception syncException)
		{
			if (this.syncContext.IsLoggerInitialized && !this.isTrySaveExecuted)
			{
				try
				{
					if (this.currentFolderId != null)
					{
						string text = HexConverter.ByteArrayToHexString(this.currentFolderId);
						this.syncContext.Logger.ReportError(string.Format(CultureInfo.InvariantCulture, (this.isCurrentOperationUpdate ? "UPDATING:" : "DELETING:") + "{0}", new object[]
						{
							text
						}), PublicFolderHierarchySyncExecutor.GetExceptionToLog(syncException));
					}
					else
					{
						this.syncContext.Logger.ReportError("ERROR", PublicFolderHierarchySyncExecutor.GetExceptionToLog(syncException));
					}
				}
				catch (StorageTransientException)
				{
				}
				catch (StoragePermanentException)
				{
				}
				this.syncContext.Logger.SetSyncMetadataValue("NumberOfBatchesExecuted", this.batchNumber - 1);
				this.syncContext.Logger.SetSyncMetadataValue("NumberOfFoldersSynced", this.numberOfFoldersSynced);
				this.syncContext.Logger.TrySave();
				return;
			}
			PublicFolderSynchronizerLogger.LogOnServer(PublicFolderHierarchySyncExecutor.GetExceptionToLog(syncException));
		}

		public void EnsureDestinationFolderHasParentChain(FolderRec sourceFolderRec)
		{
			ArgumentValidator.ThrowIfNull("sourceFolderRec", sourceFolderRec);
			ISourceFolder sourceMailboxFolder = this.GetSourceMailboxFolder(sourceFolderRec.EntryId);
			PublicFolderHierarchySyncExecutor.PublicFolderRec sourceFolderRec2 = new PublicFolderHierarchySyncExecutor.PublicFolderRec(sourceMailboxFolder, sourceFolderRec);
			this.EnsureDestinationFolderHasParentChain(sourceFolderRec2);
		}

		protected static int GetConfigValue(string configSettingName, string testOverrideRegistryKey, int testOverrideDefaultValue)
		{
			int result;
			if (!ExEnvironment.IsTest)
			{
				result = ConfigBase<MRSConfigSchema>.GetConfig<int>(configSettingName);
			}
			else if (!ConfigBase<MRSConfigSchema>.TryGetConfig<int>(configSettingName, out result))
			{
				result = ExEnvironment.GetTestRegistryValue("\\PublicFolder", testOverrideRegistryKey, testOverrideDefaultValue);
			}
			return result;
		}

		protected abstract int ProcessChanges();

		protected int ProcessChangeOneByOne()
		{
			int i = 0;
			while (i < this.batchSize)
			{
				if (this.updatedFolderIndex != this.HierarchyChangeManifest.ChangedFolders.Count)
				{
					PublicFolderHierarchySyncExecutor.<>c__DisplayClass6 CS$<>8__locals1 = new PublicFolderHierarchySyncExecutor.<>c__DisplayClass6();
					CS$<>8__locals1.entryId = this.HierarchyChangeManifest.ChangedFolders[this.updatedFolderIndex];
					if (!this.folderIdSet.Contains(IdConverter.GuidGlobCountFromEntryId(CS$<>8__locals1.entryId)))
					{
						PublicFolderHierarchySyncExecutor.<>c__DisplayClass8 CS$<>8__locals2 = new PublicFolderHierarchySyncExecutor.<>c__DisplayClass8();
						CS$<>8__locals2.CS$<>8__locals7 = CS$<>8__locals1;
						CS$<>8__locals2.<>4__this = this;
						this.transientExceptionHandler.ExecuteWithRetry(new TryDelegate(CS$<>8__locals2, (UIntPtr)ldftn(<ProcessChangeOneByOne>b__5)));
						this.folderIdSet.Insert(IdConverter.GuidGlobCountFromEntryId(CS$<>8__locals1.entryId));
						i++;
					}
					this.updatedFolderIndex++;
				}
				else
				{
					if (this.deletedFolderIndex == this.HierarchyChangeManifest.DeletedFolders.Count)
					{
						this.isCurrentManifestBatchFullyProcessed = true;
						break;
					}
					byte[] array = this.HierarchyChangeManifest.DeletedFolders[this.deletedFolderIndex];
					if (!this.folderIdSet.Contains(IdConverter.GuidGlobCountFromEntryId(array)))
					{
						this.CoreSyncFolderDelete(array);
						this.folderIdSet.Insert(IdConverter.GuidGlobCountFromEntryId(array));
						i++;
					}
					this.deletedFolderIndex++;
				}
			}
			return i;
		}

		protected int ProcessChangeInBulk()
		{
			int i = 0;
			int num = this.updatedFolderIndex;
			List<byte[]> list = new List<byte[]>(this.batchSize);
			IdSet idSet = new IdSet();
			while (num < this.HierarchyChangeManifest.ChangedFolders.Count && i + list.Count < this.batchSize)
			{
				PublicFolderHierarchySyncExecutor.<>c__DisplayClassb CS$<>8__locals1 = new PublicFolderHierarchySyncExecutor.<>c__DisplayClassb();
				CS$<>8__locals1.entryId = this.HierarchyChangeManifest.ChangedFolders[num];
				GuidGlobCount guidGlobCount = IdConverter.GuidGlobCountFromEntryId(CS$<>8__locals1.entryId);
				if (!this.folderIdSet.Contains(guidGlobCount) && !idSet.Contains(guidGlobCount))
				{
					WellKnownPublicFolders.FolderType? folderType;
					if (this.syncContext.SourceWellKnownFolders.GetFolderType(CS$<>8__locals1.entryId, out folderType))
					{
						PublicFolderHierarchySyncExecutor.<>c__DisplayClassd CS$<>8__locals2 = new PublicFolderHierarchySyncExecutor.<>c__DisplayClassd();
						CS$<>8__locals2.CS$<>8__localsc = CS$<>8__locals1;
						CS$<>8__locals2.<>4__this = this;
						this.transientExceptionHandler.ExecuteWithRetry(new TryDelegate(CS$<>8__locals2, (UIntPtr)ldftn(<ProcessChangeInBulk>b__a)));
						this.folderIdSet.Insert(guidGlobCount);
						i++;
					}
					else
					{
						list.Add(CS$<>8__locals1.entryId);
						idSet.Insert(guidGlobCount);
					}
				}
				num++;
			}
			this.UpdateFoldersInBatch(list);
			foreach (byte[] entryId in list)
			{
				this.folderIdSet.Insert(IdConverter.GuidGlobCountFromEntryId(entryId));
			}
			i += list.Count;
			this.updatedFolderIndex = num;
			while (i < this.batchSize)
			{
				if (this.deletedFolderIndex == this.HierarchyChangeManifest.DeletedFolders.Count)
				{
					this.isCurrentManifestBatchFullyProcessed = true;
					break;
				}
				byte[] array = this.HierarchyChangeManifest.DeletedFolders[this.deletedFolderIndex];
				if (!this.folderIdSet.Contains(IdConverter.GuidGlobCountFromEntryId(array)))
				{
					this.CoreSyncFolderDelete(array);
					this.folderIdSet.Insert(IdConverter.GuidGlobCountFromEntryId(array));
					i++;
				}
				this.deletedFolderIndex++;
			}
			if (i == this.batchSize && this.updatedFolderIndex == this.HierarchyChangeManifest.ChangedFolders.Count && this.deletedFolderIndex == this.HierarchyChangeManifest.DeletedFolders.Count)
			{
				this.isCurrentManifestBatchFullyProcessed = true;
			}
			return i;
		}

		private static IdSet GetIdSetFromByteArray(byte[] bytes)
		{
			IdSet result;
			using (Reader reader = Reader.CreateBufferReader(bytes))
			{
				try
				{
					result = IdSet.ParseWithReplGuids(reader);
				}
				catch (BufferParseException ex)
				{
					throw new PublicFolderSyncPermanentException(ServerStrings.PublicFolderSyncFolderFailed(CommonUtils.FullExceptionMessage(ex, true)));
				}
			}
			return result;
		}

		private static IdSet GetIdSetFromSyncState(string syncState)
		{
			MapiSyncState mapiSyncState = MapiSyncState.Deserialize(syncState);
			byte[] idsetGiven = mapiSyncState.HierarchyData.IdsetGiven;
			return PublicFolderHierarchySyncExecutor.GetIdSetFromByteArray(idsetGiven);
		}

		private static Exception GetExceptionToLog(Exception syncException)
		{
			Exception result = syncException;
			IMRSRemoteException ex = syncException as IMRSRemoteException;
			if (ex != null)
			{
				string message = string.Format("This is a remote MRS exception. [RemoteStackTrace:{0}]", string.IsNullOrEmpty(ex.RemoteStackTrace) ? string.Empty : ex.RemoteStackTrace.Replace("\r\n", string.Empty));
				result = new Exception(message, syncException);
			}
			return result;
		}

		private static void AppendEntryIdList(StringBuilder output, string description, IList<byte[]> entryIdList)
		{
			output.AppendLine();
			output.AppendLine(description);
			if (entryIdList != null)
			{
				foreach (byte[] array in entryIdList)
				{
					if (array != null && array.Length > 0)
					{
						for (int i = 0; i < array.Length; i++)
						{
							output.Append(array[i].ToString("X2"));
						}
					}
					output.AppendLine();
				}
			}
		}

		private static void ExtractReconcileFolderProperties(PropValueData[] propValueData, out MapiFolderPath folderPath, out int contentCount, out bool hasSubfolders)
		{
			folderPath = null;
			contentCount = -1;
			hasSubfolders = false;
			foreach (PropValueData propValueData2 in propValueData)
			{
				PublicFolderHierarchySyncExecutor.Tracer.TraceDebug<int, object>(0L, "ExtractReconcileFolderProperties. PropTag:{0}, Value:{1}", propValueData2.PropTag, propValueData2.Value);
				PropTag propTag = (PropTag)propValueData2.PropTag;
				PropTag propTag2 = propTag;
				if (propTag2 != PropTag.ContentCount)
				{
					if (propTag2 != PropTag.SubFolders)
					{
						if (propTag.Id() == PropTag.FolderPathName.Id())
						{
							string input = propValueData2.Value as string;
							folderPath = MapiFolderPath.Parse(input);
						}
					}
					else
					{
						hasSubfolders = (bool)propValueData2.Value;
					}
				}
				else
				{
					contentCount = (int)propValueData2.Value;
				}
			}
		}

		private static bool ShouldRetryException(object e)
		{
			Exception exception = e as Exception;
			return TransientExceptionHandler.IsTransientException(exception) && !TransientExceptionHandler.IsConnectionFailure(exception);
		}

		private IdSet GetIdSetFromContentMailbox()
		{
			PublicFolderSession destinationMailboxSession = this.syncContext.DestinationMailboxSession;
			StorageIcsState state = default(StorageIcsState);
			using (Folder folder = Folder.Bind(destinationMailboxSession, destinationMailboxSession.GetPublicFolderRootId()))
			{
				CoreFolder coreFolder = folder.CoreObject as CoreFolder;
				using (HierarchyManifestProvider hierarchyManifest = coreFolder.GetHierarchyManifest(ManifestConfigFlags.Catchup, state, new PropertyDefinition[0], new PropertyDefinition[0]))
				{
					ManifestFolderChange manifestFolderChange;
					while (hierarchyManifest.TryGetNextChange(out manifestFolderChange))
					{
					}
					hierarchyManifest.GetFinalState(ref state);
				}
			}
			return PublicFolderHierarchySyncExecutor.GetIdSetFromByteArray(state.StateIdsetGiven);
		}

		private bool InternalProcessNextBatch()
		{
			PublicFolderHierarchySyncExecutor.Tracer.TraceDebug<int, Guid>((long)this.GetHashCode(), "Starting processing batch {0} on mailbox {1}", this.batchNumber, this.syncContext.ContentMailboxGuid);
			if (this.IsFirstBatch)
			{
				this.PrepareInitialStates();
			}
			if (this.isCurrentManifestBatchFullyProcessed)
			{
				this.GetHierarchyChangeManifest();
				this.updatedFolderIndex = 0;
				this.deletedFolderIndex = 0;
				this.isCurrentManifestBatchFullyProcessed = false;
				this.nextSyncState = this.PrepareNextSyncState();
			}
			this.syncContext.Logger.LogEvent(LogEventType.Verbose, "Iteration=" + this.batchNumber);
			int folderProcessed = this.ProcessChanges();
			this.CommitCurrentJobBatch(folderProcessed);
			if (this.isCurrentManifestBatchFullyProcessed)
			{
				if (this.IsLastSync)
				{
					if (this.syncContext.ExecuteReconcileFolders)
					{
						this.Reconcile();
					}
					else
					{
						this.syncContext.Logger.LogEvent(LogEventType.Verbose, "SkippingReconcilation");
					}
				}
				this.CommitCurrentSync();
				if (this.IsLastSync)
				{
					this.isTrySaveExecuted = true;
					this.syncContext.Logger.TrySave();
					this.SetHierarchyReadyFlag();
					return false;
				}
			}
			PublicFolderHierarchySyncExecutor.Tracer.TraceDebug<int, Guid>((long)this.GetHashCode(), "Finished processing batch {0} on mailbox {1}", this.batchNumber, this.syncContext.ContentMailboxGuid);
			this.batchNumber++;
			return true;
		}

		private void UpdateFoldersInBatch(List<byte[]> entryIds)
		{
			if (entryIds.Count > 0)
			{
				using (this.performanceLogger.GetTaskFrame(SyncActivity.UpdateFoldersInBatch))
				{
					using (IFxProxyPool fxProxyPool = this.syncContext.DestinationMailbox.GetFxProxyPool(new List<byte[]>()))
					{
						using (PublicFolderHierarchyProxyPool publicFolderHierarchyProxyPool = new PublicFolderHierarchyProxyPool(this.syncContext, this, fxProxyPool, this.transientExceptionHandler))
						{
							this.syncContext.SourceMailbox.ExportFolders(entryIds, publicFolderHierarchyProxyPool, ExportFoldersDataToCopyFlags.OutputCreateMessages | ExportFoldersDataToCopyFlags.IncludeCopyToStream, GetFolderRecFlags.None, PublicFolderHierarchySyncExecutor.AdditionalPtagsToLoadForGetFolderRec, CopyPropertiesFlags.None, PublicFolderHierarchySyncExecutor.BatchFolderUpdatePtagsToExclude, AclFlags.FolderAcl);
						}
					}
				}
			}
		}

		private void PrepareInitialStates()
		{
			using (this.performanceLogger.GetTaskFrame(SyncActivity.GetChangeManifestInitializeSyncContext))
			{
				PublicFolderHierarchySyncExecutor.<>c__DisplayClass10 CS$<>8__locals1 = new PublicFolderHierarchySyncExecutor.<>c__DisplayClass10();
				CS$<>8__locals1.<>4__this = this;
				this.syncContext.Logger.TryGetSyncMetadataValue<string>("SyncState", out CS$<>8__locals1.syncState);
				this.transientExceptionHandler.ExecuteWithRetry(new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<PrepareInitialStates>b__f)));
				this.syncContext.SyncStateCounter.BytesSent += (long)((CS$<>8__locals1.syncState == null) ? 0 : CS$<>8__locals1.syncState.Length);
				byte[] array;
				if (this.syncContext.Logger.TryGetSyncMetadataValue<byte[]>("PartiallyCommittedFolderIds", out array) && array != null)
				{
					using (Reader reader = Reader.CreateBufferReader(array))
					{
						this.folderIdSet = IdSet.ParseWithReplGuids(reader);
						goto IL_C1;
					}
				}
				this.folderIdSet = new IdSet();
				IL_C1:;
			}
		}

		private string PrepareNextSyncState()
		{
			string nextJobSyncState;
			using (this.performanceLogger.GetTaskFrame(SyncActivity.GetChangeManifestPersistSyncContext))
			{
				PublicFolderHierarchySyncExecutor.<>c__DisplayClass14 CS$<>8__locals1 = new PublicFolderHierarchySyncExecutor.<>c__DisplayClass14();
				CS$<>8__locals1.<>4__this = this;
				this.syncContext.Logger.SetSyncMetadataValue("NumberOfFoldersToBeSynced", this.HierarchyChangeManifest.ChangedFolders.Count + this.HierarchyChangeManifest.DeletedFolders.Count);
				this.syncContext.Logger.SetSyncMetadataValue("BatchSize", this.batchSize);
				this.syncContext.Logger.TryGetSyncMetadataValue<string>("FinalJobSyncState", out CS$<>8__locals1.nextJobSyncState);
				if (string.IsNullOrEmpty(CS$<>8__locals1.nextJobSyncState))
				{
					this.transientExceptionHandler.ExecuteWithRetry(new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<PrepareNextSyncState>b__12)));
					this.syncContext.SyncStateCounter.BytesReceived += (long)((CS$<>8__locals1.nextJobSyncState == null) ? 0 : CS$<>8__locals1.nextJobSyncState.Length);
					this.syncContext.Logger.SetSyncMetadataValue("FinalJobSyncState", CS$<>8__locals1.nextJobSyncState);
				}
				nextJobSyncState = CS$<>8__locals1.nextJobSyncState;
			}
			return nextJobSyncState;
		}

		private void CommitCurrentJobBatch(int folderProcessed)
		{
			using (this.performanceLogger.GetTaskFrame(SyncActivity.CommitBatch))
			{
				this.numberOfFoldersSynced += folderProcessed;
				this.syncContext.Logger.SetSyncMetadataValue("NumberOfBatchesExecuted", this.batchNumber);
				this.syncContext.Logger.SetSyncMetadataValue("NumberOfFoldersSynced", this.numberOfFoldersSynced);
				byte[] array = this.folderIdSet.SerializeWithReplGuids();
				if (array != null && array.Length > 0)
				{
					this.syncContext.Logger.SetSyncMetadataValue("PartiallyCommittedFolderIds", array);
				}
				this.syncContext.Logger.SaveCheckPoint();
			}
		}

		private void CommitCurrentSync()
		{
			using (this.performanceLogger.GetTaskFrame(SyncActivity.SetIcsState))
			{
				this.syncContext.Logger.SetSyncMetadataValue("PartiallyCommittedFolderIds", null);
				this.syncContext.Logger.SetSyncMetadataValue("SyncState", this.nextSyncState);
				this.syncContext.Logger.SetSyncMetadataValue("FinalJobSyncState", null);
				this.syncContext.Logger.SaveCheckPoint();
				this.folderIdSet = new IdSet();
			}
		}

		private void GetHierarchyChangeManifest()
		{
			using (this.PerformanceLogger.GetTaskFrame(SyncActivity.EnumerateHierarchyChanges))
			{
				this.HierarchyChangeManifest = this.SyncContext.SourceMailbox.EnumerateHierarchyChanges(this.EnumerateHierarchyChangesFlags, this.MaxHierarchyChanges);
			}
			if (PublicFolderHierarchySyncExecutor.Tracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				this.TraceManifest();
			}
		}

		private void TraceManifest()
		{
			if (this.HierarchyChangeManifest == null)
			{
				PublicFolderHierarchySyncExecutor.Tracer.TraceDebug((long)this.GetHashCode(), "HierarchyChangeManifest:<null>");
				return;
			}
			int capacity = 200 + ((this.HierarchyChangeManifest.ChangedFolders != null) ? (this.HierarchyChangeManifest.ChangedFolders.Count * 50) : 0) + ((this.HierarchyChangeManifest.DeletedFolders != null) ? (this.HierarchyChangeManifest.DeletedFolders.Count * 50) : 0);
			StringBuilder stringBuilder = new StringBuilder(capacity);
			PublicFolderHierarchySyncExecutor.AppendEntryIdList(stringBuilder, "ChangedFolders:", this.HierarchyChangeManifest.ChangedFolders);
			PublicFolderHierarchySyncExecutor.AppendEntryIdList(stringBuilder, "DeletedFolders:", this.HierarchyChangeManifest.DeletedFolders);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("HasMoreHierarchyChanges: " + this.HierarchyChangeManifest.HasMoreHierarchyChanges);
			PublicFolderHierarchySyncExecutor.Tracer.TraceDebug<StringBuilder>((long)this.GetHashCode(), "HierarchyChangeManifest:{0}", stringBuilder);
		}

		private void Reconcile()
		{
			this.syncContext.Logger.LogEvent(LogEventType.Verbose, "BeginReconcilation");
			this.transientExceptionHandler.ExecuteWithRetry(new TryDelegate(this, (UIntPtr)ldftn(ReconcileFolders)));
			this.syncContext.Logger.LogEvent(LogEventType.Verbose, "EndReconcilation");
		}

		private void ReconcileFolders()
		{
			IdSet idSetFromContentMailbox;
			using (this.performanceLogger.GetTaskFrame(SyncActivity.GetDestinationFolderIdSet))
			{
				idSetFromContentMailbox = this.GetIdSetFromContentMailbox();
			}
			IdSet idSetFromSyncState;
			using (this.performanceLogger.GetTaskFrame(SyncActivity.GetSourceFolderIdSet))
			{
				idSetFromSyncState = PublicFolderHierarchySyncExecutor.GetIdSetFromSyncState(this.nextSyncState);
			}
			if (idSetFromSyncState.CountIds > 0UL && idSetFromContentMailbox.CountIds > 0UL)
			{
				IdConverter.ExpandIdSet(IdSet.Subtract(idSetFromSyncState, idSetFromContentMailbox), delegate(byte[] sourceFolderId)
				{
					byte[] sourceSessionSpecificEntryId = this.GetSourceSessionSpecificEntryId(sourceFolderId);
					WellKnownPublicFolders.FolderType? folderType;
					if (!this.syncContext.SourceWellKnownFolders.GetFolderType(sourceSessionSpecificEntryId, out folderType))
					{
						this.syncContext.Logger.LogEvent(LogEventType.Warning, string.Format(CultureInfo.InvariantCulture, "ReconcileFolders found missing folder in the content mailbox and will attempt to sync it. Id:{0}.", new object[]
						{
							HexConverter.ByteArrayToHexString(sourceSessionSpecificEntryId)
						}));
						PublicFolderHierarchySyncExecutor.PublicFolderRec publicFolderRec;
						this.CoreSyncFolderUpdate(sourceFolderId, out publicFolderRec);
					}
				});
				IdConverter.ExpandIdSet(IdSet.Subtract(idSetFromContentMailbox, idSetFromSyncState), delegate(byte[] destinationFolderId)
				{
					using (this.performanceLogger.GetTaskFrame(SyncActivity.FixOrphanFolders))
					{
						byte[] destinationSessionSpecificEntryId = this.GetDestinationSessionSpecificEntryId(destinationFolderId);
						WellKnownPublicFolders.FolderType? folderType;
						if (!this.syncContext.DestinationWellKnownFolders.GetFolderType(destinationSessionSpecificEntryId, out folderType))
						{
							string text = HexConverter.ByteArrayToHexString(destinationSessionSpecificEntryId);
							this.syncContext.FolderOperationCount.OrphanDetected++;
							using (IDestinationFolder folder = this.syncContext.DestinationMailbox.GetFolder(destinationSessionSpecificEntryId))
							{
								if (folder != null)
								{
									FolderRec folderRec = folder.GetFolderRec(PublicFolderHierarchySyncExecutor.PropertiesToLoadForOrphanFolder, GetFolderRecFlags.None);
									if (folderRec.AdditionalProps.Length > 0)
									{
										MapiFolderPath mapiFolderPath;
										int num;
										bool flag;
										PublicFolderHierarchySyncExecutor.ExtractReconcileFolderProperties(folderRec.AdditionalProps, out mapiFolderPath, out num, out flag);
										if (mapiFolderPath.IsIpmPath)
										{
											PublicFolderHierarchySyncExecutor.Tracer.TraceDebug<string>((long)this.GetHashCode(), "ReconcileFolders: Processing orphan folder. Id:{0}.", text);
											this.syncContext.Logger.LogEvent(LogEventType.Verbose, string.Format(CultureInfo.InvariantCulture, "ReconcileFolders: Fixing Orphan folder. FolderId={0};ItemCount={1};HasSubfolders={2};", new object[]
											{
												text,
												num,
												flag
											}));
											this.FixOrphanFolder(folder, folderRec);
										}
										else
										{
											PublicFolderHierarchySyncExecutor.Tracer.TraceDebug<string>((long)this.GetHashCode(), "ReconcileFolders: Skipping folder recovery for Non IPM folder. Orphan Id:{0}.", text);
										}
									}
									else
									{
										PublicFolderHierarchySyncExecutor.Tracer.TraceWarning<string>((long)this.GetHashCode(), "ReconcileFolders: Skipping folder recovery as the folder path can't be retrieved. Orphan Id:{0}.", text);
									}
								}
								else
								{
									PublicFolderHierarchySyncExecutor.Tracer.TraceWarning<string>((long)this.GetHashCode(), "ReconcileFolders: Skipping folder recovery due to not found object during ReconcileFolder operation. Orphan Id:{0}.", text);
								}
							}
						}
					}
				});
			}
		}

		private void FixOrphanFolder(IDestinationFolder orphanFolder, FolderRec orphanFolderRec)
		{
			using (IDestinationFolder folder = this.syncContext.DestinationMailbox.GetFolder(orphanFolderRec.ParentId))
			{
				if (folder != null)
				{
					FolderRec folderRec = folder.GetFolderRec(PublicFolderHierarchySyncExecutor.AdditionalPtagsToLoadForGetFolderRec, GetFolderRecFlags.None);
					byte[] array = PublicFolderHierarchyProxyPool.GetDumpsterEntryIdFromFolderRec(folderRec);
					if (array != null)
					{
						PublicFolderHierarchySyncExecutor.Tracer.TraceDebug<int>((long)this.GetHashCode(), "ReconcileFolders: Setting OverallAgeLimit on the orphan folder to {0}.", 5184000);
						array = this.GetDestinationSessionSpecificEntryId(array);
						orphanFolder.SetProps(new PropValueData[]
						{
							new PropValueData(PropTag.OverallAgeLimit, 5184000)
						});
						if (PublicFolderHierarchySyncExecutor.Tracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							PublicFolderHierarchySyncExecutor.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "ReconcileFolders: Moving orphan folder to parent dumpster. Parent Id:{1}, Dumpster Id:{2}.", HexConverter.ByteArrayToHexString(orphanFolderRec.ParentId), HexConverter.ByteArrayToHexString(array));
						}
						this.syncContext.DestinationMailbox.MoveFolder(orphanFolderRec.EntryId, orphanFolderRec.ParentId, array);
						this.syncContext.FolderOperationCount.OrphanFixed++;
						PublicFolderHierarchySyncExecutor.Tracer.TraceDebug((long)this.GetHashCode(), "ReconcileFolders: Succeeded processing orphan folder.");
					}
					else
					{
						PublicFolderHierarchySyncExecutor.Tracer.TraceWarning((long)this.GetHashCode(), "ReconcileFolders: Skipping folder recovery as the parent dumpster is unknown.");
					}
				}
				else
				{
					PublicFolderHierarchySyncExecutor.Tracer.TraceWarning((long)this.GetHashCode(), "ReconcileFolders: Skipping folder recovery as the parent folder is unknown.");
				}
			}
		}

		private void CoreSyncFolderUpdate(byte[] folderId, out PublicFolderHierarchySyncExecutor.PublicFolderRec sourceFolderRec)
		{
			this.currentFolderId = folderId;
			this.isCurrentOperationUpdate = true;
			sourceFolderRec = null;
			using (ISourceFolder sourceMailboxFolder = this.GetSourceMailboxFolder(this.GetSourceSessionSpecificEntryId(folderId)))
			{
				if (sourceMailboxFolder != null)
				{
					sourceFolderRec = new PublicFolderHierarchySyncExecutor.PublicFolderRec(sourceMailboxFolder, this.GetFolderRec(sourceMailboxFolder));
					this.EnsureDestinationFolderHasParentChain(sourceFolderRec);
					this.CoreSyncFolderUpdate(sourceFolderRec);
				}
			}
		}

		private void CoreSyncFolderUpdate(PublicFolderHierarchySyncExecutor.PublicFolderRec sourceFolderRec)
		{
			if (sourceFolderRec.FolderRec.FolderType == FolderType.Search)
			{
				return;
			}
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				bool flag;
				byte[] array = this.MapSourceToDestinationFolderId(sourceFolderRec.FolderRec.EntryId, out flag);
				byte[] array2 = null;
				if (sourceFolderRec.FolderRec.ParentId != null)
				{
					bool flag2;
					array2 = this.MapSourceToDestinationFolderId(sourceFolderRec.FolderRec.ParentId, out flag2);
				}
				IDestinationFolder destinationMailboxFolder = this.GetDestinationMailboxFolder(array);
				if (destinationMailboxFolder == null)
				{
					byte[] entryId = sourceFolderRec.FolderRec.EntryId;
					byte[] parentId = sourceFolderRec.FolderRec.ParentId;
					sourceFolderRec.FolderRec.EntryId = array;
					sourceFolderRec.FolderRec.ParentId = array2;
					byte[] folderId;
					using (this.performanceLogger.GetTaskFrame(SyncActivity.CreateFolder))
					{
						this.syncContext.DestinationMailbox.CreateFolder(sourceFolderRec.FolderRec, CreateFolderFlags.None, out folderId);
					}
					sourceFolderRec.FolderRec.EntryId = entryId;
					sourceFolderRec.FolderRec.ParentId = parentId;
					destinationMailboxFolder = this.GetDestinationMailboxFolder(folderId);
					disposeGuard.Add<IDestinationFolder>(destinationMailboxFolder);
					PublicFolderHierarchySyncExecutor.Tracer.TraceDebug<FolderRec>((long)this.GetHashCode(), "Folder created: {0}", sourceFolderRec.FolderRec);
					if (this.syncContext.Logger != null)
					{
						this.syncContext.Logger.LogFolderCreated(sourceFolderRec.FolderRec.EntryId);
					}
				}
				else
				{
					disposeGuard.Add<IDestinationFolder>(destinationMailboxFolder);
					FolderRec folderRec = destinationMailboxFolder.GetFolderRec(null, GetFolderRecFlags.None);
					if (!CommonUtils.IsSameEntryId(folderRec.ParentId, array2))
					{
						using (this.performanceLogger.GetTaskFrame(SyncActivity.MoveFolder))
						{
							this.syncContext.DestinationMailbox.MoveFolder(folderRec.EntryId, folderRec.ParentId, array2);
						}
					}
					PublicFolderHierarchySyncExecutor.Tracer.TraceDebug<FolderRec>((long)this.GetHashCode(), "Folder updated: {0}", sourceFolderRec.FolderRec);
					if (this.syncContext.Logger != null)
					{
						this.syncContext.Logger.LogFolderUpdated(sourceFolderRec.FolderRec.EntryId);
					}
				}
				List<PropTag> list = new List<PropTag>(PublicFolderHierarchySyncExecutor.AlwaysExcludedFolderPtags.Length);
				list.AddRange(PublicFolderHierarchySyncExecutor.AlwaysExcludedFolderPtags);
				if (sourceFolderRec.FolderRec.FolderType == FolderType.Root)
				{
					list.AddRange(PublicFolderHierarchySyncExecutor.ExcludedRootFolderPtags);
				}
				if (flag)
				{
					list.Add(PropTag.ReplicaList);
				}
				using (this.performanceLogger.GetTaskFrame(SyncActivity.ClearFolderProperties))
				{
					destinationMailboxFolder.SetProps(CommonUtils.PropertiesToDelete);
				}
				using (this.performanceLogger.GetTaskFrame(SyncActivity.SetSecurityDescriptor))
				{
					if (this.ShouldUseExtendedAclInformation())
					{
						destinationMailboxFolder.SetExtendedAcl(AclFlags.FolderAcl, sourceFolderRec.Folder.GetExtendedAcl(AclFlags.FolderAcl));
					}
					else
					{
						destinationMailboxFolder.SetSecurityDescriptor(SecurityProp.NTSD, sourceFolderRec.Folder.GetSecurityDescriptor(SecurityProp.NTSD));
					}
				}
				using (this.performanceLogger.GetTaskFrame(SyncActivity.FxCopyProperties))
				{
					using (IFxProxy fxProxy = destinationMailboxFolder.GetFxProxy(FastTransferFlags.None))
					{
						((ISourceFolder)sourceFolderRec.Folder).CopyTo(fxProxy, CopyPropertiesFlags.None, list.ToArray());
					}
				}
				if (sourceFolderRec.DumpsterFolderId != null)
				{
					byte[] sourceSessionSpecificEntryId = this.GetSourceSessionSpecificEntryId(sourceFolderRec.DumpsterFolderId);
					bool flag3;
					byte[] value = this.MapSourceToDestinationFolderId(sourceSessionSpecificEntryId, out flag3);
					if (flag3)
					{
						using (this.performanceLogger.GetTaskFrame(SyncActivity.UpdateDumpsterId))
						{
							destinationMailboxFolder.SetProps(new PropValueData[]
							{
								new PropValueData(PropTag.IpmWasteBasketEntryId, value)
							});
						}
					}
				}
			}
		}

		private void CoreSyncFolderDelete(byte[] folderId)
		{
			this.currentFolderId = folderId;
			this.isCurrentOperationUpdate = false;
			using (IDestinationFolder destinationMailboxFolder = this.GetDestinationMailboxFolder(this.GetDestinationSessionSpecificEntryId(folderId)))
			{
				if (destinationMailboxFolder != null)
				{
					PublicFolderHierarchySyncExecutor.PublicFolderRec publicFolderRec = new PublicFolderHierarchySyncExecutor.PublicFolderRec(destinationMailboxFolder, this.GetFolderRec(destinationMailboxFolder));
					if (publicFolderRec.FolderRec.FolderType != FolderType.Search)
					{
						using (this.performanceLogger.GetTaskFrame(SyncActivity.DeleteFolder))
						{
							this.syncContext.DestinationMailbox.DeleteFolder(publicFolderRec.FolderRec);
						}
						PublicFolderHierarchySyncExecutor.Tracer.TraceDebug<FolderRec>((long)this.GetHashCode(), "Folder deleted: {0}", publicFolderRec.FolderRec);
						if (this.syncContext.Logger != null)
						{
							this.syncContext.Logger.LogFolderDeleted(folderId);
						}
					}
				}
			}
		}

		private void EnsureDestinationFolderHasParentChain(PublicFolderHierarchySyncExecutor.PublicFolderRec sourceFolderRec)
		{
			List<PublicFolderHierarchySyncExecutor.PublicFolderRec> list = new List<PublicFolderHierarchySyncExecutor.PublicFolderRec>();
			if (sourceFolderRec.FolderRec.ParentId == null)
			{
				return;
			}
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				for (;;)
				{
					bool flag;
					byte[] folderId = this.MapSourceToDestinationFolderId(sourceFolderRec.FolderRec.ParentId, out flag);
					using (IDestinationFolder destinationMailboxFolder = this.GetDestinationMailboxFolder(folderId))
					{
						if (destinationMailboxFolder == null)
						{
							ISourceFolder sourceMailboxFolder = this.GetSourceMailboxFolder(sourceFolderRec.FolderRec.ParentId);
							if (sourceMailboxFolder != null)
							{
								disposeGuard.Add<ISourceFolder>(sourceMailboxFolder);
								PublicFolderHierarchySyncExecutor.PublicFolderRec publicFolderRec = new PublicFolderHierarchySyncExecutor.PublicFolderRec(sourceMailboxFolder, this.GetFolderRec(sourceMailboxFolder));
								list.Insert(0, publicFolderRec);
								sourceFolderRec = publicFolderRec;
							}
							continue;
						}
					}
					break;
				}
				this.SyncParentChainFolders(list);
			}
		}

		private void SyncParentChainFolders(List<PublicFolderHierarchySyncExecutor.PublicFolderRec> sourceParentFolderChain)
		{
			if (sourceParentFolderChain.Count > 0)
			{
				this.syncContext.FolderOperationCount.ParentChainMissing++;
			}
			foreach (PublicFolderHierarchySyncExecutor.PublicFolderRec publicFolderRec in sourceParentFolderChain)
			{
				this.CoreSyncFolderUpdate(publicFolderRec);
				if (publicFolderRec.DumpsterFolderId != null)
				{
					PublicFolderHierarchySyncExecutor.PublicFolderRec publicFolderRec2;
					this.CoreSyncFolderUpdate(publicFolderRec.DumpsterFolderId, out publicFolderRec2);
				}
			}
		}

		private IDestinationFolder GetDestinationMailboxFolder(byte[] folderId)
		{
			IDestinationFolder folder;
			using (this.performanceLogger.GetTaskFrame(SyncActivity.GetDestinationMailboxFolder))
			{
				folder = this.syncContext.DestinationMailbox.GetFolder(folderId);
			}
			return folder;
		}

		private byte[] GetDestinationSessionSpecificEntryId(byte[] sourceEntryId)
		{
			byte[] sessionSpecificEntryId;
			using (this.performanceLogger.GetTaskFrame(SyncActivity.GetDestinationSessionSpecificEntryId))
			{
				sessionSpecificEntryId = this.syncContext.DestinationMailbox.GetSessionSpecificEntryId(sourceEntryId);
			}
			return sessionSpecificEntryId;
		}

		private ISourceFolder GetSourceMailboxFolder(byte[] folderId)
		{
			ISourceFolder folder;
			using (this.performanceLogger.GetTaskFrame(SyncActivity.GetSourceMailboxFolder))
			{
				folder = this.syncContext.SourceMailbox.GetFolder(folderId);
			}
			return folder;
		}

		private byte[] GetSourceSessionSpecificEntryId(byte[] folderEntryId)
		{
			byte[] sessionSpecificEntryId;
			using (this.performanceLogger.GetTaskFrame(SyncActivity.GetSourceSessionSpecificEntryId))
			{
				sessionSpecificEntryId = this.syncContext.SourceMailbox.GetSessionSpecificEntryId(folderEntryId);
			}
			return sessionSpecificEntryId;
		}

		private byte[] MapSourceToDestinationFolderId(byte[] folderId, out bool isWellKnownFolder)
		{
			byte[] result;
			using (this.performanceLogger.GetTaskFrame(SyncActivity.MapSourceToDestinationFolderId))
			{
				result = this.syncContext.MapSourceToDestinationFolderId(folderId, out isWellKnownFolder);
			}
			return result;
		}

		private bool ShouldUseExtendedAclInformation()
		{
			return CommonUtils.ShouldUseExtendedAclInformation(this.syncContext.SourceMailbox, this.syncContext.DestinationMailbox);
		}

		private FolderRec GetFolderRec(IFolder folder)
		{
			FolderRec folderRec;
			using (this.performanceLogger.GetTaskFrame(SyncActivity.GetFolderRec))
			{
				folderRec = folder.GetFolderRec(PublicFolderHierarchySyncExecutor.AdditionalPtagsToLoadForGetFolderRec, GetFolderRecFlags.None);
			}
			return folderRec;
		}

		private void SetHierarchyReadyFlag()
		{
			IRecipientSession adsession = this.syncContext.GetADSession();
			ADRecipient contentMailboxADRecipient = this.syncContext.GetContentMailboxADRecipient(adsession);
			if (contentMailboxADRecipient == null)
			{
				this.syncContext.Logger.LogEvent(LogEventType.Warning, "IsHierarchyReady not set as user was not found on AD");
				return;
			}
			if (!(bool)contentMailboxADRecipient[ADRecipientSchema.IsHierarchyReady])
			{
				this.syncContext.Logger.LogEvent(LogEventType.Verbose, "Setting IsHierarchyReady=true");
				contentMailboxADRecipient[ADRecipientSchema.IsHierarchyReady] = true;
				adsession.Save(contentMailboxADRecipient);
				return;
			}
			this.syncContext.Logger.LogEvent(LogEventType.Verbose, "IsHierarchyReady was already set");
		}

		private const string SyncState = "SyncState";

		private const string FinalJobSyncState = "FinalJobSyncState";

		private const string PartiallyCommittedFolderIds = "PartiallyCommittedFolderIds";

		private const int MaxRetriesOnTransientException = 3;

		private const int RetentionPolicyInSecondsForOrphanFolder = 5184000;

		private static readonly Trace Tracer = ExTraceGlobals.PublicFolderSynchronizerTracer;

		private static readonly TimeSpan RetryDelayOnTransientException = TimeSpan.FromSeconds(10.0);

		private static readonly PropTag[] AlwaysExcludedFolderPtags = new PropTag[]
		{
			PropTag.ContainerContents,
			PropTag.FolderAssociatedContents,
			PropTag.ContainerHierarchy,
			PropTag.MessageSize,
			PropTag.InternetArticleNumber,
			PropTag.Access,
			PropTag.SubFolders,
			PropTag.AssocContentCount,
			PropTag.SourceKey,
			PropTag.ParentSourceKey,
			PropTag.ChangeKey,
			PropTag.NTSD,
			PropTag.FreeBusyNTSD
		};

		private static readonly PropTag[] ExcludedRootFolderPtags = new PropTag[]
		{
			PropTag.DisplayName,
			PropTag.Comment
		};

		private static readonly PropTag[] AdditionalPtagsToLoadForGetFolderRec = new PropTag[]
		{
			PropTag.IpmWasteBasketEntryId
		};

		private static readonly PropTag[] PropertiesToLoadForOrphanFolder = new PropTag[]
		{
			PropTag.FolderPathName,
			PropTag.ContentCount,
			PropTag.SubFolders
		};

		private static readonly PropTag[] BatchFolderUpdatePtagsToExclude = new List<PropTag>(PublicFolderHierarchySyncExecutor.AlwaysExcludedFolderPtags.Union(PublicFolderHierarchySyncExecutor.AdditionalPtagsToLoadForGetFolderRec)).ToArray();

		private readonly int batchSize;

		private readonly PublicFolderSynchronizerContext syncContext;

		private readonly PublicFolderPerformanceLogger performanceLogger;

		private readonly TransientExceptionHandler transientExceptionHandler;

		private int batchNumber = 1;

		private int updatedFolderIndex;

		private int deletedFolderIndex;

		private IdSet folderIdSet;

		private string nextSyncState;

		private int numberOfFoldersSynced;

		private bool isCurrentManifestBatchFullyProcessed = true;

		private bool isTrySaveExecuted;

		private byte[] currentFolderId;

		private bool isCurrentOperationUpdate;

		private sealed class PublicFolderRec
		{
			public PublicFolderRec(IFolder folder, FolderRec folderRec)
			{
				this.Folder = folder;
				this.FolderRec = folderRec;
				this.DumpsterFolderId = PublicFolderHierarchyProxyPool.GetDumpsterEntryIdFromFolderRec(folderRec);
			}

			public IFolder Folder { get; private set; }

			public FolderRec FolderRec { get; private set; }

			public byte[] DumpsterFolderId { get; private set; }
		}

		private sealed class SyncAllAndProcessEach : PublicFolderHierarchySyncExecutor
		{
			public SyncAllAndProcessEach(PublicFolderSynchronizerContext syncContext, IBudget budget, string callerInfo) : base(syncContext, budget, callerInfo)
			{
			}

			protected override bool IsLastSync
			{
				get
				{
					return true;
				}
			}

			protected override EnumerateHierarchyChangesFlags EnumerateHierarchyChangesFlags
			{
				get
				{
					return EnumerateHierarchyChangesFlags.None;
				}
			}

			protected override int MaxHierarchyChanges
			{
				get
				{
					return 0;
				}
			}

			protected override int ProcessChanges()
			{
				return base.ProcessChangeOneByOne();
			}
		}

		private sealed class PagedSyncAndBulkProcess : PublicFolderHierarchySyncExecutor
		{
			public PagedSyncAndBulkProcess(PublicFolderSynchronizerContext syncContext, IBudget budget, string callerInfo) : base(syncContext, budget, callerInfo)
			{
				this.maxHierarchyChanges = PublicFolderHierarchySyncExecutor.GetConfigValue("ChangesPerIcsManifestPage", "SyncMaxChanges", 1000);
				this.syncContext.SourceMailbox.SetOtherSideVersion(this.syncContext.DestinationMailbox.GetVersion());
				this.syncContext.DestinationMailbox.SetOtherSideVersion(this.syncContext.SourceMailbox.GetVersion());
			}

			protected override bool IsLastSync
			{
				get
				{
					return base.HierarchyChangeManifest != null && !base.HierarchyChangeManifest.HasMoreHierarchyChanges;
				}
			}

			protected override EnumerateHierarchyChangesFlags EnumerateHierarchyChangesFlags
			{
				get
				{
					if (!base.IsFirstBatch)
					{
						return EnumerateHierarchyChangesFlags.None;
					}
					return EnumerateHierarchyChangesFlags.FirstPage;
				}
			}

			protected override int MaxHierarchyChanges
			{
				get
				{
					return this.maxHierarchyChanges;
				}
			}

			protected override int ProcessChanges()
			{
				return base.ProcessChangeInBulk();
			}

			private readonly int maxHierarchyChanges = 500;
		}
	}
}
