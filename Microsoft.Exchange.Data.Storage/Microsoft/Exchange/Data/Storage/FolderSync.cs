using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FolderSync
	{
		public FolderSync(ISyncProvider syncProvider, IFolderSyncState syncState, ConflictResolutionPolicy policy, bool deferStateModifications)
		{
			EnumValidator.ThrowIfInvalid<ConflictResolutionPolicy>(policy, "policy");
			this.SyncLogger = syncProvider.SyncLogger;
			if (syncProvider == null)
			{
				throw new ArgumentNullException("syncProvider");
			}
			if (syncState == null)
			{
				throw new ArgumentNullException("syncState");
			}
			this.filters = new ISyncFilter[]
			{
				new NoSyncFilter()
			};
			this.syncProvider = syncProvider;
			this.conflictPolicy = policy;
			this.deferStateModifications = deferStateModifications;
			syncProvider.BindToFolderSync(this);
			this.syncState = syncState;
			this.syncState.OnCommitStateModifications(new FolderSyncStateUtil.CommitStateModificationsDelegate(this.CommitAllDeferredModifiers));
			if (this.CurMaxWatermark == null)
			{
				this.CurMaxWatermark = syncProvider.CreateNewWatermark();
			}
			this.PrepareModifyState(this.prevMaxWatermarkModifiers, delegate
			{
				if (this.PrevMaxWatermark == null)
				{
					this.PrevMaxWatermark = this.syncProvider.CreateNewWatermark();
				}
			});
			this.PrepareModifyState(this.clientStateModifiers, delegate
			{
				if (this.ClientState == null)
				{
					this.ClientState = new Dictionary<ISyncItemId, FolderSync.ClientStateInformation>();
				}
			});
			if (this.CurDelayedServerOperationQueue == null)
			{
				this.CurDelayedServerOperationQueue = new Dictionary<ISyncItemId, ServerManifestEntry>();
			}
			if (this.CumulativeClientManifest == null)
			{
				this.CumulativeClientManifest = new Dictionary<ISyncItemId, ClientManifestEntry>();
			}
			if (this.RecoveryClientManifest == null)
			{
				this.RecoveryClientManifest = new Dictionary<ISyncItemId, ClientManifestEntry>();
			}
			if (this.CurServerManifest == null)
			{
				this.CurServerManifest = new Dictionary<ISyncItemId, ServerManifestEntry>();
			}
			this.PrepareModifyState(this.prevServerManifestModifiers, delegate
			{
				if (this.PrevServerManifest == null)
				{
					this.PrevServerManifest = new Dictionary<ISyncItemId, ServerManifestEntry>();
				}
			});
			if (this.CurFilterId == null)
			{
				this.CurFilterId = this.filters[0].Id;
			}
			this.PrepareModifyState(this.prevFilterIdModifiers, delegate
			{
				if (this.PrevFilterId == null)
				{
					this.PrevFilterId = this.filters[0].Id;
				}
			});
		}

		public ISyncProvider SyncProvider
		{
			get
			{
				return this.syncProvider;
			}
		}

		public ISyncLogger SyncLogger { get; private set; }

		public int EnumerateServerOperationsIterations { get; private set; }

		public TimeSpan EnumerateServerOperationsElapsed { get; private set; }

		public bool FastReadFlagFilterCheck
		{
			get
			{
				return this.fastReadFlagFilterCheck;
			}
			set
			{
				this.fastReadFlagFilterCheck = value;
			}
		}

		public bool MidnightRollover { get; set; }

		public Dictionary<ISyncItemId, FolderSync.ClientStateInformation> ClientState
		{
			get
			{
				this.CommitModifyState(this.clientStateModifiers);
				GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, FolderSync.ClientStateInformation> genericDictionaryData = (GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, FolderSync.ClientStateInformation>)this.syncState[SyncStateProp.ClientState];
				if (genericDictionaryData == null)
				{
					return null;
				}
				return genericDictionaryData.Data;
			}
			set
			{
				this.CommitModifyState(this.clientStateModifiers);
				this.syncState[SyncStateProp.ClientState] = new GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, FolderSync.ClientStateInformation>(value);
			}
		}

		private Dictionary<ISyncItemId, ClientManifestEntry> CumulativeClientManifest
		{
			get
			{
				GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, ClientManifestEntry> genericDictionaryData = (GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, ClientManifestEntry>)this.syncState[SyncStateProp.CumulativeClientManifest];
				if (genericDictionaryData == null)
				{
					return null;
				}
				return genericDictionaryData.Data;
			}
			set
			{
				this.syncState[SyncStateProp.CumulativeClientManifest] = new GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, ClientManifestEntry>(value);
			}
		}

		protected Dictionary<ISyncItemId, ServerManifestEntry> CurDelayedServerOperationQueue
		{
			get
			{
				GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, ServerManifestEntry> genericDictionaryData = (GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, ServerManifestEntry>)this.syncState[SyncStateProp.CurDelayedServerOperationQueue];
				Dictionary<ISyncItemId, ServerManifestEntry> dictionary = (genericDictionaryData != null) ? genericDictionaryData.Data : null;
				if (!this.backupCurDelayedServerOperationQueueHasBeenSet && this.shouldBackUpState)
				{
					if (dictionary != null)
					{
						this.backupCurDelayedServerOperationQueue = FolderSync.CloneDictionary(dictionary);
					}
					this.backupCurDelayedServerOperationQueueHasBeenSet = true;
				}
				return dictionary;
			}
			set
			{
				GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, ServerManifestEntry> genericDictionaryData = (GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, ServerManifestEntry>)this.syncState[SyncStateProp.CurDelayedServerOperationQueue];
				Dictionary<ISyncItemId, ServerManifestEntry> dictionary = (genericDictionaryData != null) ? genericDictionaryData.Data : null;
				if (!this.backupCurDelayedServerOperationQueueHasBeenSet && this.shouldBackUpState)
				{
					this.backupCurDelayedServerOperationQueue = dictionary;
					this.backupCurDelayedServerOperationQueueHasBeenSet = true;
				}
				this.syncState[SyncStateProp.CurDelayedServerOperationQueue] = new GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, ServerManifestEntry>(value);
			}
		}

		protected bool CurLastSyncConversationMode
		{
			get
			{
				BooleanData booleanData = (BooleanData)this.syncState[SyncStateProp.CurLastSyncConversationMode];
				bool result = booleanData != null && booleanData.Data;
				if (!this.backupCurLastSyncConversationModeHasBeenSet && this.shouldBackUpState)
				{
					this.backupCurLastSyncConversationMode = result;
					this.backupCurLastSyncConversationModeHasBeenSet = true;
				}
				return result;
			}
			set
			{
				BooleanData booleanData = (BooleanData)this.syncState[SyncStateProp.CurLastSyncConversationMode];
				bool flag = booleanData != null && booleanData.Data;
				if (!this.backupCurLastSyncConversationModeHasBeenSet && this.shouldBackUpState)
				{
					this.backupCurLastSyncConversationMode = flag;
					this.backupCurLastSyncConversationModeHasBeenSet = true;
				}
				BooleanData value2 = new BooleanData(value);
				this.syncState[SyncStateProp.CurLastSyncConversationMode] = value2;
			}
		}

		protected string CurFilterId
		{
			get
			{
				StringData stringData = (StringData)this.syncState[SyncStateProp.CurFilterId];
				if (stringData == null)
				{
					return null;
				}
				return stringData.Data;
			}
			set
			{
				if (!this.backupCurFilterIdHasBeenSet && this.shouldBackUpState)
				{
					if (this.syncState.Contains(SyncStateProp.CurFilterId))
					{
						this.backupCurFilterId = ((StringData)this.syncState[SyncStateProp.CurFilterId]).Data;
					}
					this.backupCurFilterIdHasBeenSet = true;
				}
				this.syncState[SyncStateProp.CurFilterId] = new StringData(value);
			}
		}

		protected ISyncWatermark CurMaxWatermark
		{
			get
			{
				ISyncWatermark syncWatermark = (ISyncWatermark)this.syncState[SyncStateProp.CurMaxWatermark];
				if (!this.backupCurMaxWatermarkHasBeenSet && this.shouldBackUpState)
				{
					if (syncWatermark != null)
					{
						this.backupCurMaxWatermark = (ISyncWatermark)syncWatermark.Clone();
					}
					this.backupCurMaxWatermarkHasBeenSet = true;
				}
				return syncWatermark;
			}
			set
			{
				if (!this.backupCurMaxWatermarkHasBeenSet && this.shouldBackUpState)
				{
					if (this.syncState.Contains(SyncStateProp.CurMaxWatermark))
					{
						this.backupCurMaxWatermark = (ISyncWatermark)this.syncState[SyncStateProp.CurMaxWatermark];
					}
					this.backupCurMaxWatermarkHasBeenSet = true;
				}
				this.syncState[SyncStateProp.CurMaxWatermark] = value;
			}
		}

		protected Dictionary<ISyncItemId, ServerManifestEntry> CurServerManifest
		{
			get
			{
				GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, ServerManifestEntry> genericDictionaryData = (GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, ServerManifestEntry>)this.syncState[SyncStateProp.CurServerManifest];
				Dictionary<ISyncItemId, ServerManifestEntry> dictionary = (genericDictionaryData != null) ? genericDictionaryData.Data : null;
				if (!this.backupCurServerManifestHasBeenSet && this.shouldBackUpState)
				{
					if (dictionary != null)
					{
						this.backupCurServerManifest = FolderSync.CloneDictionary(dictionary);
					}
					this.backupCurServerManifestHasBeenSet = true;
				}
				return dictionary;
			}
			set
			{
				if (!this.backupCurServerManifestHasBeenSet && this.shouldBackUpState)
				{
					GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, ServerManifestEntry> genericDictionaryData = (GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, ServerManifestEntry>)this.syncState[SyncStateProp.CurServerManifest];
					this.backupCurServerManifest = ((genericDictionaryData != null) ? genericDictionaryData.Data : null);
					this.backupCurServerManifestHasBeenSet = true;
				}
				this.syncState[SyncStateProp.CurServerManifest] = new GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, ServerManifestEntry>(value);
			}
		}

		protected ISyncWatermark CurSnapShotWatermark
		{
			get
			{
				ISyncWatermark syncWatermark = (ISyncWatermark)this.syncState[SyncStateProp.CurSnapShotWatermark];
				if (!this.backupCurSnapShotWatermarkHasBeenSet && this.shouldBackUpState)
				{
					if (syncWatermark != null)
					{
						this.backupCurSnapShotWatermark = (ISyncWatermark)syncWatermark.Clone();
					}
					this.backupCurSnapShotWatermarkHasBeenSet = true;
				}
				return syncWatermark;
			}
			set
			{
				if (!this.backupCurSnapShotWatermarkHasBeenSet && this.shouldBackUpState)
				{
					if (this.syncState.Contains(SyncStateProp.CurSnapShotWatermark))
					{
						this.backupCurSnapShotWatermark = (ISyncWatermark)this.syncState[SyncStateProp.CurSnapShotWatermark];
					}
					this.backupCurSnapShotWatermarkHasBeenSet = true;
				}
				this.syncState[SyncStateProp.CurSnapShotWatermark] = value;
			}
		}

		protected Dictionary<ISyncItemId, ServerManifestEntry> PrevDelayedServerOperationQueue
		{
			get
			{
				this.CommitModifyState(this.prevDelayedServerOperationQueueModifiers);
				GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, ServerManifestEntry> genericDictionaryData = (GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, ServerManifestEntry>)this.syncState[SyncStateProp.PrevDelayedServerOperationQueue];
				if (genericDictionaryData == null)
				{
					return null;
				}
				return genericDictionaryData.Data;
			}
			set
			{
				this.CommitModifyState(this.prevDelayedServerOperationQueueModifiers);
				this.syncState[SyncStateProp.PrevDelayedServerOperationQueue] = new GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, ServerManifestEntry>(value);
			}
		}

		protected bool PrevLastSyncConversationMode
		{
			get
			{
				this.CommitModifyState(this.prevLastSyncConversationModeModifiers);
				BooleanData booleanData = (BooleanData)this.syncState[SyncStateProp.PrevLastSyncConversationMode];
				return booleanData != null && booleanData.Data;
			}
			set
			{
				this.CommitModifyState(this.prevLastSyncConversationModeModifiers);
				BooleanData value2 = new BooleanData(value);
				this.syncState[SyncStateProp.PrevLastSyncConversationMode] = value2;
			}
		}

		protected string PrevFilterId
		{
			get
			{
				this.CommitModifyState(this.prevFilterIdModifiers);
				StringData stringData = (StringData)this.syncState[SyncStateProp.PrevFilterId];
				if (stringData == null)
				{
					return null;
				}
				return stringData.Data;
			}
			set
			{
				this.CommitModifyState(this.prevFilterIdModifiers);
				this.syncState[SyncStateProp.PrevFilterId] = new StringData(value);
			}
		}

		protected ISyncWatermark PrevMaxWatermark
		{
			get
			{
				this.CommitModifyState(this.prevMaxWatermarkModifiers);
				return (ISyncWatermark)this.syncState[SyncStateProp.PrevMaxWatermark];
			}
			set
			{
				this.CommitModifyState(this.prevMaxWatermarkModifiers);
				this.syncState[SyncStateProp.PrevMaxWatermark] = value;
			}
		}

		protected Dictionary<ISyncItemId, ServerManifestEntry> PrevServerManifest
		{
			get
			{
				this.CommitModifyState(this.prevServerManifestModifiers);
				GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, ServerManifestEntry> genericDictionaryData = (GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, ServerManifestEntry>)this.syncState[SyncStateProp.PrevServerManifest];
				if (genericDictionaryData == null)
				{
					return null;
				}
				return genericDictionaryData.Data;
			}
			set
			{
				this.CommitModifyState(this.prevServerManifestModifiers);
				this.syncState[SyncStateProp.PrevServerManifest] = new GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, ServerManifestEntry>(value);
			}
		}

		protected ISyncWatermark PrevSnapShotWatermark
		{
			get
			{
				this.CommitModifyState(this.prevSnapShotWatermarkModifiers);
				return (ISyncWatermark)this.syncState[SyncStateProp.PrevSnapShotWatermark];
			}
			set
			{
				this.CommitModifyState(this.prevSnapShotWatermarkModifiers);
				this.syncState[SyncStateProp.PrevSnapShotWatermark] = value;
			}
		}

		protected Dictionary<ISyncItemId, ClientManifestEntry> RecoveryClientManifest
		{
			get
			{
				GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, ClientManifestEntry> genericDictionaryData = (GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, ClientManifestEntry>)this.syncState[SyncStateProp.RecoveryClientManifest];
				if (genericDictionaryData == null)
				{
					return null;
				}
				return genericDictionaryData.Data;
			}
			set
			{
				this.syncState[SyncStateProp.RecoveryClientManifest] = new GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, ClientManifestEntry>(value);
			}
		}

		private bool UsingQueryBasedFilter
		{
			get
			{
				BooleanData booleanData = (BooleanData)this.syncState[SyncStateProp.UsingQueryBasedFilter];
				return booleanData != null && booleanData.Data;
			}
			set
			{
				BooleanData value2 = new BooleanData(value);
				this.syncState[SyncStateProp.UsingQueryBasedFilter] = value2;
			}
		}

		private HashSet<ConversationId> CurrentConversationIdList
		{
			get
			{
				if (this.currentConversationIdList == null)
				{
					this.currentConversationIdList = new HashSet<ConversationId>();
				}
				return this.currentConversationIdList;
			}
		}

		private HashSet<ConversationId> AssociatedAddConversationIdList
		{
			get
			{
				if (this.associatedAddConversationIdList == null)
				{
					this.associatedAddConversationIdList = new HashSet<ConversationId>();
				}
				return this.associatedAddConversationIdList;
			}
		}

		private HashSet<ConversationId> FirstMessageInFilterConversationIdList
		{
			get
			{
				if (this.firstMessageInFilterConversationIdList == null)
				{
					this.firstMessageInFilterConversationIdList = new HashSet<ConversationId>();
				}
				return this.firstMessageInFilterConversationIdList;
			}
		}

		public bool IsValidClientOperation(ISyncItemId id, ChangeType changeType)
		{
			EnumValidator.ThrowIfInvalid<ChangeType>(changeType, "changeType");
			if (this.ClientState == null || id == null)
			{
				return false;
			}
			if (changeType == ChangeType.Add)
			{
				return !this.ClientHasItem(id);
			}
			return this.ClientHasItem(id);
		}

		public string GetMessageClassFromItemId(ISyncItemId itemId)
		{
			FolderSync.ClientStateInformation clientStateInformation;
			if (itemId == null || this.ClientState == null || !this.ClientState.TryGetValue(itemId, out clientStateInformation))
			{
				return null;
			}
			return clientStateInformation.MessageClass;
		}

		protected virtual FolderSync.AcknowledgeModifications CreateAcknowledgeModifications()
		{
			return new FolderSync.AcknowledgeModifications();
		}

		public Exception AcknowledgeServerOperations()
		{
			this.acknowledgeModifications = this.CreateAcknowledgeModifications();
			int count = this.clientStateModifiers.Count;
			this.SyncLogger.Information(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "Storage.FolderSync.AcknowledgeServerOperations");
			Exception result = null;
			foreach (ServerManifestEntry serverManifestEntry in this.CurServerManifest.Values)
			{
				if (!serverManifestEntry.IsAcknowledgedByClient)
				{
					switch (serverManifestEntry.ChangeType)
					{
					case ChangeType.Add:
					case ChangeType.Change:
					case ChangeType.ReadFlagChange:
					case ChangeType.AssociatedAdd:
						if (serverManifestEntry.IsRejected)
						{
							this.acknowledgeModifications.RecordClientReject(serverManifestEntry);
						}
						else
						{
							this.acknowledgeModifications.RecordClientHasItem(serverManifestEntry, true, this.UsingQueryBasedFilter);
							this.acknowledgeModifications.UpdateClientStateChangeTrackingInformation(serverManifestEntry);
						}
						break;
					case ChangeType.Delete:
						this.acknowledgeModifications.RemoveFromClientState(serverManifestEntry);
						break;
					case ChangeType.SoftDelete:
						if (!serverManifestEntry.IsRejected)
						{
							goto IL_F7;
						}
						this.acknowledgeModifications.RecordClientReject(serverManifestEntry);
						break;
					case ChangeType.OutOfFilter:
						goto IL_F7;
					}
					IL_143:
					serverManifestEntry.IsAcknowledgedByClient = true;
					continue;
					IL_F7:
					this.acknowledgeModifications.RecordClientHasItem(serverManifestEntry, false, this.UsingQueryBasedFilter);
					ClientManifestEntry clientManifestEntry;
					this.CumulativeClientManifest.TryGetValue(serverManifestEntry.Id, out clientManifestEntry);
					if (clientManifestEntry != null && clientManifestEntry.SoftDeletePending && clientManifestEntry.Watermark.Equals(serverManifestEntry.Watermark))
					{
						clientManifestEntry.SoftDeletePending = false;
						goto IL_143;
					}
					goto IL_143;
				}
			}
			List<ISyncItemId> list = new List<ISyncItemId>(this.CumulativeClientManifest.Count);
			foreach (KeyValuePair<ISyncItemId, ClientManifestEntry> keyValuePair in this.CumulativeClientManifest)
			{
				ISyncItemId key = keyValuePair.Key;
				ClientManifestEntry value = keyValuePair.Value;
				if (value.Watermark != null && value.Watermark.CompareTo(this.CurMaxWatermark) <= 0)
				{
					list.Add(key);
					if (value.SoftDeletePending)
					{
						result = new InvalidOperationException("There are still soft deletes pending in the client manifest entry when we are trying to remove it!");
					}
				}
			}
			foreach (ISyncItemId key2 in list)
			{
				this.CumulativeClientManifest.Remove(key2);
			}
			this.RecoveryClientManifest.Clear();
			this.SavePreviousState();
			this.preparedAcknowledgeServerOperations = true;
			this.PrepareModifyState(this.clientStateModifiers, new FolderSync.StateModifier(this.CommitAcknowledgeServerOperations));
			return result;
		}

		protected virtual void SavePreviousState()
		{
			this.acknowledgeModifications.SavePreviousState((ISyncWatermark)this.CurMaxWatermark.Clone(), FolderSync.CloneDictionary(this.CurServerManifest), this.CurFilterId, FolderSync.CloneDictionary(this.CurDelayedServerOperationQueue), (this.CurSnapShotWatermark != null) ? ((ISyncWatermark)this.CurSnapShotWatermark.Clone()) : null, this.CurLastSyncConversationMode);
		}

		public ConflictResult DetectConflict(ISyncItemId clientId, string clientAddId, ChangeType clientChangeType)
		{
			EnumValidator.ThrowIfInvalid<ChangeType>(clientChangeType, "clientChangeType");
			this.SyncLogger.Information<ISyncItemId, ChangeType>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "Storage.FolderSync.DetectConflict. ItemId = {0}, ChangeType = {1}", clientId, clientChangeType);
			if (clientChangeType != ChangeType.Add && clientChangeType != ChangeType.Change && clientChangeType != ChangeType.Delete)
			{
				throw new ArgumentOutOfRangeException(ServerStrings.ExInvalidChangeType(clientChangeType.ToString()));
			}
			if ((clientChangeType == ChangeType.Change || clientChangeType == ChangeType.Delete) && clientId == null)
			{
				throw new ArgumentNullException(ServerStrings.ExInvalidNullParameterForChangeTypes("clientId", "ChangeType.Change, ChangeType.Delete"));
			}
			if (clientChangeType == ChangeType.Add && clientAddId == null)
			{
				throw new ArgumentNullException(ServerStrings.ExInvalidNullParameterForChangeTypes("clientAddId", "ChangeType.Add"));
			}
			ConflictResult result = ConflictResult.AcceptClientChange;
			if ((clientChangeType == ChangeType.Delete || clientChangeType == ChangeType.Change) && !this.ClientHasItem(clientId))
			{
				this.SyncLogger.TraceDebug<ChangeType, ISyncItemId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "Client refered to an ID that is not in the client state. This is most likely due to change/delete for items immediately after MoveItems. ChangeType = {0}, clientId = {1}", clientChangeType, clientId);
			}
			ISyncItem syncItem = null;
			try
			{
				switch (clientChangeType)
				{
				case ChangeType.Add:
					if (this.currentlyInRecovery && this.conflictPolicy == ConflictResolutionPolicy.ServerWins && this.HasRecoveryAddItemChanged(clientAddId) == FolderSync.HasRecoveryAddItemChangedResult.ItemChanged)
					{
						result = ConflictResult.RejectClientChange;
					}
					break;
				case ChangeType.Change:
				{
					FolderSync.TryGetItemResult tryGetItemResult = this.TryGetItem(clientId, out syncItem);
					if (tryGetItemResult == FolderSync.TryGetItemResult.NotFound)
					{
						result = ConflictResult.ObjectNotFound;
					}
					else if (FolderSync.TryGetItemResult.Success == tryGetItemResult && (syncItem.Watermark.CompareTo(this.CurMaxWatermark) > 0 || (this.CurDelayedServerOperationQueue.ContainsKey(clientId) && syncItem.Watermark.CompareTo(this.CurDelayedServerOperationQueue[clientId].Watermark) > 0)) && !this.IsItemVersionInServerManifest(clientId, syncItem.Watermark) && !this.IsItemVersionInCumulativeClientManifest(clientId, syncItem.Watermark) && (!this.currentlyInRecovery || !this.IsItemVersionInPrevClientManifest(clientId, syncItem.Watermark)) && this.conflictPolicy == ConflictResolutionPolicy.ServerWins)
					{
						result = ConflictResult.RejectClientChange;
					}
					break;
				}
				case ChangeType.Delete:
					result = ConflictResult.AcceptClientChange;
					break;
				}
			}
			finally
			{
				if (syncItem != null)
				{
					syncItem.Dispose();
				}
			}
			return result;
		}

		protected virtual bool GetNewOperations(int windowSize, Dictionary<ISyncItemId, ServerManifestEntry> tempServerManifest)
		{
			this.SyncLogger.Information<int, int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync.GetNewOperations [Pre], windowSize: {0}, tempServerManifest count {1}", windowSize, tempServerManifest.Count);
			bool newOperations = this.syncProvider.GetNewOperations(this.CurMaxWatermark, this.CurSnapShotWatermark, true, windowSize, (this.CurSnapShotWatermark != null) ? FolderSync.ComputeQueryHint(this.filters) : null, tempServerManifest);
			this.SyncLogger.Information<bool, int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync.GetNewOperations [Post], More Available? {0}, tempServerManifest count {1}", newOperations, tempServerManifest.Count);
			return newOperations;
		}

		private void ProcessClientPendingSoftDeletes(Dictionary<ISyncItemId, ServerManifestEntry> newServerManifest, int windowSize, ref bool moreAvailable)
		{
			this.SyncLogger.Information<int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync.ProcessClientPendingSoftDeletes, this.CumulativeClientManifest count {0}", this.CumulativeClientManifest.Count);
			foreach (KeyValuePair<ISyncItemId, ClientManifestEntry> keyValuePair in this.CumulativeClientManifest)
			{
				ISyncItemId key = keyValuePair.Key;
				ClientManifestEntry value = keyValuePair.Value;
				if (value.SoftDeletePending)
				{
					if (newServerManifest.Count >= windowSize && -1 != windowSize)
					{
						this.SyncLogger.Information(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ProcessClientPendingSoftDeleted, fill window size, skipping remaining items in client manifest.");
						moreAvailable = true;
						break;
					}
					ServerManifestEntry serverManifestEntry = new ServerManifestEntry(key);
					serverManifestEntry.ChangeType = ChangeType.SoftDelete;
					serverManifestEntry.Watermark = value.Watermark;
					newServerManifest[serverManifestEntry.Id] = serverManifestEntry;
					this.SyncLogger.Information<ISyncItemId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ProcessClientPendingSoftDeletes, Add to server manifest {0}", serverManifestEntry.Id);
				}
			}
		}

		private bool HaveFilterSettingsChanged
		{
			get
			{
				bool flag = this.filters[0].Id != this.CurFilterId || (this.CurLastSyncConversationMode && !this.currentConversationMode);
				if (flag)
				{
					this.SyncLogger.Information(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync.HaveFilterSettingsChanged.  OldFilter: '{0}', NewFilter: '{1}', OldConvMode:'{2}', NewConvMode:'{3}'", new object[]
					{
						this.CurFilterId,
						this.filters[0].Id,
						this.CurLastSyncConversationMode,
						this.currentConversationMode
					});
				}
				return flag;
			}
		}

		private void ProcessConversationModeChange()
		{
			if (this.CurLastSyncConversationMode && !this.currentConversationMode)
			{
				this.SyncLogger.Information<int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ProcessConversationModeChange, CurDelayedServerOperationQueue.Count {0}", this.CurDelayedServerOperationQueue.Count);
				List<ISyncItemId> list = new List<ISyncItemId>(this.CurDelayedServerOperationQueue.Count);
				foreach (KeyValuePair<ISyncItemId, ServerManifestEntry> keyValuePair in this.CurDelayedServerOperationQueue)
				{
					ISyncItemId key = keyValuePair.Key;
					ServerManifestEntry value = keyValuePair.Value;
					if (value.ChangeType == ChangeType.AssociatedAdd)
					{
						list.Add(key);
					}
				}
				foreach (ISyncItemId syncItemId in list)
				{
					this.SyncLogger.Information<ISyncItemId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ProcessConversationModeChange, Removing AssociatedAdd in delay operation queue, id = {0}", syncItemId);
					this.CurDelayedServerOperationQueue.Remove(syncItemId);
				}
			}
		}

		protected virtual void ProcessFilterSettingsChanged(Dictionary<ISyncItemId, ServerManifestEntry> newServerManifest, ref bool computeSoftOperationsBelowWatermark, ref bool itemsInFilterInitialized, out int totalOperationsInFilter)
		{
			if (this.HaveFilterSettingsChanged)
			{
				this.SyncLogger.Information(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ProcessFilterSettingsChanged, Filter has changed");
				this.ProcessConversationModeChange();
				if (this.IsSafeToUseSnapShotQueryHint(this.filters))
				{
					this.CurSnapShotWatermark = (this.MidnightRollover ? null : this.syncProvider.GetMaxItemWatermark(this.CurMaxWatermark));
				}
				else
				{
					this.CurSnapShotWatermark = null;
				}
				this.SyncLogger.Information<bool, bool>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ProcessFilterSettingsChanged, this.CurSnapShotWatermark != null ({0}), this.MidnightRollover {1}", this.CurSnapShotWatermark != null, this.MidnightRollover);
				if (this.IsFirstSyncScenario)
				{
					this.SyncLogger.Information(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ProcessFilterSettingsChanged, IsFirstSyncScenario");
					this.CurFilterId = this.filters[0].Id;
					this.CurLastSyncConversationMode = this.currentConversationMode;
				}
				else
				{
					QueryBasedSyncFilter queryBasedSyncFilter = this.filters[0] as QueryBasedSyncFilter;
					if (queryBasedSyncFilter != null)
					{
						this.SyncLogger.Information(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ProcessFilterSettingsChanged, queryBasedSyncFilter != null");
						this.InitializeAllItemsInFilter(queryBasedSyncFilter);
						itemsInFilterInitialized = true;
					}
					if (queryBasedSyncFilter != null && 1 == this.filters.Length)
					{
						this.ComputeSoftAddsUsingQueryBasedFilter(queryBasedSyncFilter, newServerManifest);
					}
					computeSoftOperationsBelowWatermark = true;
					this.CurFilterId = this.filters[0].Id;
					this.CurLastSyncConversationMode = this.currentConversationMode;
				}
			}
			if (this.currentConversationMode)
			{
				this.SyncLogger.Information(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ProcessFilterSettingsChanged, GenerateCurrentConversationIdList");
				this.GenerateCurrentConversationIdList(newServerManifest, itemsInFilterInitialized);
			}
			totalOperationsInFilter = 0;
			foreach (ServerManifestEntry serverManifestEntry in newServerManifest.Values)
			{
				if (serverManifestEntry.ChangeType != ChangeType.OutOfFilter)
				{
					totalOperationsInFilter++;
				}
			}
			this.SyncLogger.Information<int, int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ProcessFilterSettingsChanged, totalOperationsInFilter {0}, newServerManifest.Values.Count {1}", totalOperationsInFilter, newServerManifest.Values.Count);
		}

		protected virtual void InitializeAllItemsInFilter(QueryBasedSyncFilter queryBasedSyncFilter)
		{
			queryBasedSyncFilter.InitializeAllItemsInFilter(this.syncProvider);
		}

		private bool ProcessDelayedOperations(Dictionary<ISyncItemId, ServerManifestEntry> delayedOperations, Dictionary<ISyncItemId, ServerManifestEntry> tempServerManifest, int windowSize, int totalOperationsInFilter, ref bool moreAvailable)
		{
			if (delayedOperations.Count > 0)
			{
				this.SyncLogger.Information<int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ProcessDelayedOperations, delayedOperations.Count {0}", delayedOperations.Count);
				bool flag = null != this.CurSnapShotWatermark;
				List<ISyncItemId> list = new List<ISyncItemId>();
				foreach (KeyValuePair<ISyncItemId, ServerManifestEntry> keyValuePair in delayedOperations)
				{
					ISyncItemId key = keyValuePair.Key;
					ServerManifestEntry value = keyValuePair.Value;
					if (flag && value.ChangeType != ChangeType.AssociatedAdd)
					{
						this.SyncLogger.Information(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ProcessDelayedOperations isCurSnapShotWatermarkNotNull");
					}
					else
					{
						if (windowSize != -1 && tempServerManifest.Count + totalOperationsInFilter == windowSize)
						{
							this.SyncLogger.Information(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ProcessDelayedOperations set moreAvailable");
							moreAvailable = true;
							break;
						}
						list.Add(key);
						if (value.ChangeType == ChangeType.Change)
						{
							this.SyncLogger.Information(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ProcessDelayedOperations change to add");
							value.ChangeType = ChangeType.Add;
						}
						tempServerManifest[key] = value;
						this.SyncLogger.Information<ISyncItemId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ProcessDelayedOperations, Added delayed server manifest from last loop, id = {0}", key);
					}
				}
				this.SyncLogger.Information<int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ProcessDelayedOperations, removeList.Count {0}", list.Count);
				foreach (ISyncItemId key2 in list)
				{
					this.CurDelayedServerOperationQueue.Remove(key2);
					delayedOperations.Remove(key2);
				}
				return true;
			}
			return false;
		}

		private void ProcessTempServerManifest(Dictionary<ISyncItemId, ServerManifestEntry> tempServerManifest, Dictionary<ISyncItemId, ServerManifestEntry> newServerManifest, HashSet<ISyncItemId> itemsDeleted, int totalOperationsInFilter)
		{
			foreach (ServerManifestEntry serverManifestEntry in tempServerManifest.Values)
			{
				if (serverManifestEntry.ChangeType == ChangeType.Add && !this.ClientHasItem(serverManifestEntry.Id))
				{
					FolderSync.ProcessServerOperationResult processServerOperationResult = this.ProcessServerOperationAboveWatermark(serverManifestEntry, newServerManifest);
					this.SyncLogger.Information(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ProcessTempServerManifest. returned Result = {0}, serverManifestEntry = {1}, newServerManifest.Count = {2}, totalOperationsInFilter = {3}", new object[]
					{
						processServerOperationResult,
						serverManifestEntry,
						newServerManifest.Count,
						totalOperationsInFilter
					});
				}
			}
			this.SyncLogger.Information<int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ProcessTempServerManifest, tempServerManifest.Values.Count {0}", tempServerManifest.Values.Count);
			foreach (ServerManifestEntry serverManifestEntry2 in tempServerManifest.Values)
			{
				if (serverManifestEntry2.ChangeType != ChangeType.OutOfFilter)
				{
					if (serverManifestEntry2.ChangeType != ChangeType.Add || this.ClientHasItem(serverManifestEntry2.Id))
					{
						FolderSync.ProcessServerOperationResult processServerOperationResult2 = this.ProcessServerOperationAboveWatermark(serverManifestEntry2, newServerManifest);
						this.SyncLogger.Information(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ProcessTempServerManifest. returned Result = {0}, serverManifestEntry = {1}, newServerManifest.Count = {2}, totalOperationsInFilter = {3}, ChangeType = {4}", new object[]
						{
							processServerOperationResult2,
							serverManifestEntry2,
							newServerManifest.Count,
							totalOperationsInFilter,
							serverManifestEntry2.ChangeType
						});
					}
					if (serverManifestEntry2.ChangeType == ChangeType.Delete)
					{
						itemsDeleted.Add(serverManifestEntry2.Id);
					}
				}
			}
			this.SyncLogger.Information<int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ProcessTempServerManifest, newServerManifest.Values.Count {0}", newServerManifest.Values.Count);
		}

		private int CountTotalOperationsInFilter(Dictionary<ISyncItemId, ServerManifestEntry> newServerManifest)
		{
			int num = 0;
			foreach (ServerManifestEntry serverManifestEntry in newServerManifest.Values)
			{
				if (serverManifestEntry.ChangeType != ChangeType.OutOfFilter)
				{
					this.SyncLogger.Information<ServerManifestEntry>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::CountTotalOperationsInFilter, serverManifestEntry = {0}", serverManifestEntry);
					FolderSync.ClientStateInformation clientStateInformation;
					if (this.ClientState.TryGetValue(serverManifestEntry.Id, out clientStateInformation) && clientStateInformation.ClientHasItem)
					{
						serverManifestEntry.ChangeTrackingInformation = clientStateInformation.ChangeTrackingInformation;
						if (serverManifestEntry.MessageClass == null)
						{
							serverManifestEntry.MessageClass = clientStateInformation.MessageClass;
						}
						if (serverManifestEntry.FilterDate == null)
						{
							serverManifestEntry.FilterDate = clientStateInformation.FilterDate;
						}
					}
					num++;
				}
			}
			return num;
		}

		private bool PushFilterChangeExtrasIntoDelayQueue(Dictionary<ISyncItemId, ServerManifestEntry> newServerManifest, ref int totalOperationsInFilter, int windowSize)
		{
			bool result = false;
			if (totalOperationsInFilter > windowSize && windowSize != -1)
			{
				this.SyncLogger.TraceDebug<int, int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "[FolderSync.PushFilterChangeExtrasIntoDelayQueue] Filter change generated more changes than we can emit in window.  Saving in delayed operations.  Window Size: {0}, Total Generated: {1}", windowSize, totalOperationsInFilter);
				result = true;
				int arg = 0;
				List<ISyncItemId> list = new List<ISyncItemId>();
				foreach (KeyValuePair<ISyncItemId, ServerManifestEntry> keyValuePair in newServerManifest)
				{
					if (arg++ >= windowSize)
					{
						list.Add(keyValuePair.Key);
						totalOperationsInFilter--;
						this.SyncLogger.TraceDebug<int, ChangeType, ISyncItemId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "[FolderSync.PushFilterChangeExtrasIntoDelayQueue] Moving item {0} [{1}] to delay queue: {2}", arg, keyValuePair.Value.ChangeType, keyValuePair.Key);
						this.QueueDelayedServerOperation(keyValuePair.Value);
					}
				}
				foreach (ISyncItemId key in list)
				{
					newServerManifest.Remove(key);
				}
			}
			return result;
		}

		public SyncOperations EnumerateServerOperations(int windowSize)
		{
			this.EnumerateServerOperationsIterations = 0;
			this.EnumerateServerOperationsElapsed = TimeSpan.Zero;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			this.shouldBackUpState = true;
			Dictionary<ISyncItemId, ServerManifestEntry> dictionary = new Dictionary<ISyncItemId, ServerManifestEntry>();
			bool flag = false;
			bool flag2 = false;
			if (windowSize <= 0 && -1 != windowSize)
			{
				throw new ArgumentOutOfRangeException("windowSize");
			}
			this.SyncLogger.Information<bool, bool>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::EnumerateServerOperations, CurLastSyncConversationMode = {0}, currentConversationMode = {1}", this.CurLastSyncConversationMode, this.currentConversationMode);
			this.ProcessClientPendingSoftDeletes(dictionary, windowSize, ref flag);
			int num = dictionary.Count;
			this.SyncLogger.Information<int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::EnumerateServerOperations, totalOperationsInFilter after ProcessClientPendingSoftDeletes: {0}", num);
			HashSet<ISyncItemId> itemsDeleted = new HashSet<ISyncItemId>();
			bool flag3 = false;
			try
			{
				if (!flag)
				{
					this.ProcessFilterSettingsChanged(dictionary, ref flag3, ref flag2, out num);
					bool flag4 = flag2;
					this.SyncLogger.TraceDebug<bool, int, int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "[FolderSync.EnumerateServerOperations] FilterChanged: {0}, TotalOperationsInFilter: {1}, WindowSize: {2}", flag2, num, windowSize);
					Dictionary<ISyncItemId, ServerManifestEntry> dictionary2 = FolderSync.CloneDictionary(this.CurDelayedServerOperationQueue);
					flag = this.PushFilterChangeExtrasIntoDelayQueue(dictionary, ref num, windowSize);
					if (!flag)
					{
						bool flag5;
						do
						{
							this.EnumerateServerOperationsIterations++;
							flag = false;
							Dictionary<ISyncItemId, ServerManifestEntry> dictionary3 = new Dictionary<ISyncItemId, ServerManifestEntry>();
							flag5 = this.ProcessDelayedOperations(dictionary2, dictionary3, windowSize, num, ref flag);
							if (dictionary2.Count <= 0 || this.CurSnapShotWatermark != null)
							{
								this.SyncLogger.Information<int, bool>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::EnumerateServerOperations, delayedOperations.Count {0}, this.CurSnapShotWatermark != null ({1})", dictionary2.Count, this.CurSnapShotWatermark != null);
								int num2 = (windowSize == -1) ? -1 : (windowSize - num);
								if (num2 > 0 || num2 == -1)
								{
									if (!flag4 || this.CurSnapShotWatermark == null)
									{
										flag |= this.GetNewOperations(num2, dictionary3);
									}
									if (flag4)
									{
										flag5 = true;
										flag4 = false;
									}
								}
								flag5 = (flag5 || flag);
							}
							this.SyncLogger.Information<int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::EnumerateServerOperations, tempServerManifest.Values.Count {0}", dictionary3.Values.Count);
							this.ProcessTempServerManifest(dictionary3, dictionary, itemsDeleted, num);
							this.SyncLogger.Information<int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::EnumerateServerOperations, totalOperationsInFilter before recount: {0}", num);
							num = this.CountTotalOperationsInFilter(dictionary);
							this.SyncLogger.Information<int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::EnumerateServerOperations, totalOperationsInFilter after recount: {0}", num);
							if (!flag && this.CurSnapShotWatermark != null)
							{
								this.SyncLogger.Information(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::EnumerateServerOperations, switch to using no enumerateQuery hint.  Done with FTS");
								this.MarkFirstTimeSyncAsComplete();
								flag5 = true;
							}
							else
							{
								flag5 |= (flag && num < windowSize);
							}
							this.SyncLogger.Information<bool, int, int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::EnumerateServerOperations, Loop check, loop = {0}, totalOperationsInFilter = {1}, windowSize = {2}", flag5, num, windowSize);
						}
						while (stopwatch.Elapsed < FolderSync.maxElapsedTime.Value && flag5 && (num < windowSize || -1 == windowSize));
					}
					if (num > windowSize && -1 != windowSize)
					{
						throw new InvalidOperationException(string.Format("number server manifest operations [{0}] > windowSize [{1}].  Max Windows Size: {2}", num, windowSize, -1));
					}
				}
			}
			finally
			{
				this.syncProvider.DisposeNewOperationsCachedData();
			}
			if (!flag && flag3)
			{
				this.SyncLogger.Information(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::EnumerateServerOperations, computeSoftOperationsBelowWatermark");
				flag = this.ComputeSoftOperationsBelowWatermark(this.filters[0], windowSize, ref num, dictionary, itemsDeleted);
			}
			if (this.currentConversationMode)
			{
				this.SyncLogger.Information(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::EnumerateServerOperations, ComputeAssociatedAddForConversationMode");
				flag |= this.ComputeAssociatedAddForConversationMode(windowSize, ref num, dictionary);
			}
			if (!this.CurLastSyncConversationMode && this.currentConversationMode)
			{
				this.SyncLogger.Information(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::EnumerateServerOperations, ConversationMode off->on");
				this.CurLastSyncConversationMode = this.currentConversationMode;
			}
			this.CurServerManifest = dictionary;
			stopwatch.Stop();
			this.EnumerateServerOperationsElapsed += stopwatch.Elapsed;
			this.SyncLogger.Information(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::EnumerateServerOperations, Returning, CurServerManifest.Count = {0}, moreAvailable = {1}, Elapsed = {2}, Iterations = {3}", new object[]
			{
				this.CurServerManifest.Count,
				flag,
				this.EnumerateServerOperationsElapsed,
				this.EnumerateServerOperationsIterations
			});
			return new SyncOperations(this, this.CurServerManifest, flag);
		}

		protected virtual void MarkFirstTimeSyncAsComplete()
		{
			this.CurMaxWatermark = this.CurSnapShotWatermark;
			this.CurSnapShotWatermark = null;
		}

		public ISyncItem GetItem(ISyncItemId id, params PropertyDefinition[] prefetchProperties)
		{
			return this.syncProvider.GetItem(id, prefetchProperties);
		}

		public void RecordClientOperation(ISyncClientOperation clientOperation)
		{
			this.SyncLogger.Information<ISyncClientOperation>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "Storage.FolderSync.RecordClientOperation. clientOperation = {0}", clientOperation);
			if (clientOperation == null)
			{
				throw new ArgumentNullException("clientOperation");
			}
			if (clientOperation.ChangeType == ChangeType.Add && string.IsNullOrEmpty(clientOperation.ClientAddId))
			{
				throw new InvalidOperationException(ServerStrings.ExInvalidNullParameterForChangeTypes("clientAddId", "ChangeType.Add"));
			}
			ChangeType changeType;
			if (clientOperation.SendEnabled)
			{
				changeType = ChangeType.Send;
			}
			else if (clientOperation.ChangeType == ChangeType.ReadFlagChange)
			{
				changeType = ChangeType.Change;
			}
			else
			{
				changeType = clientOperation.ChangeType;
			}
			if ((changeType == ChangeType.Add || changeType == ChangeType.Change) && (clientOperation.Id == null || clientOperation.Item == null))
			{
				throw new ArgumentException(ServerStrings.ExInvalidNullParameterForChangeTypes("item", "ChangeType.Add, ChangeType.Change"));
			}
			if ((changeType == ChangeType.Delete || changeType == ChangeType.Change) && !this.ClientHasItem(clientOperation.Id))
			{
				throw new InvalidOperationException(ServerStrings.ExItemNotFoundInClientManifest);
			}
			ClientManifestEntry clientManifestEntry = new ClientManifestEntry(clientOperation.Id);
			clientManifestEntry.ChangeType = changeType;
			clientManifestEntry.ClientAddId = clientOperation.ClientAddId;
			if (changeType == ChangeType.Change)
			{
				clientManifestEntry.ChangeTrackingInformation = this.ClientState[clientOperation.Id].ChangeTrackingInformation;
			}
			if (changeType == ChangeType.Add || changeType == ChangeType.Change)
			{
				clientManifestEntry.Watermark = clientOperation.Item.Watermark;
				clientManifestEntry.UpdateManifestFromItem(clientOperation.Item);
			}
			ServerManifestEntry serverManifestEntry = new ServerManifestEntry(clientManifestEntry.Id);
			serverManifestEntry.ChangeType = clientManifestEntry.ChangeType;
			serverManifestEntry.Watermark = clientManifestEntry.Watermark;
			serverManifestEntry.ChangeTrackingInformation = clientManifestEntry.ChangeTrackingInformation;
			serverManifestEntry.MessageClass = clientManifestEntry.MessageClass;
			serverManifestEntry.FilterDate = clientManifestEntry.FilterDate;
			serverManifestEntry.ConversationId = clientManifestEntry.ConversationId;
			serverManifestEntry.FirstMessageInConversation = clientManifestEntry.FirstMessageInConversation;
			if (serverManifestEntry.ChangeType == ChangeType.Add || serverManifestEntry.ChangeType == ChangeType.Change)
			{
				this.tempSyncOperation.Bind(clientOperation.Item, serverManifestEntry);
			}
			else
			{
				this.tempSyncOperation.Bind(this, serverManifestEntry, false);
			}
			for (int i = 0; i < this.filters.Length; i++)
			{
				this.filters[i].UpdateFilterState(this.tempSyncOperation);
			}
			if (changeType == ChangeType.Add || changeType == ChangeType.Change)
			{
				clientManifestEntry.SoftDeletePending = !this.filters[0].IsItemInFilter(clientManifestEntry.Id);
			}
			this.CumulativeClientManifest[clientOperation.Id] = clientManifestEntry;
			this.RecoveryClientManifest[clientOperation.Id] = clientManifestEntry;
			if (changeType == ChangeType.Add || changeType == ChangeType.Change)
			{
				FolderSync.RecordClientHasItem(this.ClientState, clientOperation.Id, true, this.UsingQueryBasedFilter, clientManifestEntry.MessageClass, clientManifestEntry.FilterDate, clientManifestEntry.ConversationId);
				this.ClientState[clientOperation.Id].ChangeTrackingInformation = clientOperation.ChangeTrackingInformation;
			}
			else if (changeType == ChangeType.Delete || (changeType == ChangeType.Send && this.ClientHasItem(clientOperation.Id)))
			{
				this.ClientState.Remove(clientOperation.Id);
			}
			if (this.CurDelayedServerOperationQueue.ContainsKey(clientOperation.Id))
			{
				this.CurDelayedServerOperationQueue.Remove(clientOperation.Id);
			}
		}

		public virtual void Recover(ISyncClientOperation[] clientOperations)
		{
			this.SyncLogger.Information<ISyncClientOperation[], int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync.RecoverClientOperations. clientOperations = {0}, count = {1}", clientOperations, (clientOperations != null) ? clientOperations.Length : 0);
			this.currentlyInRecovery = true;
			this.prevClientManifest = this.RecoveryClientManifest;
			this.RecoveryClientManifest = new Dictionary<ISyncItemId, ClientManifestEntry>();
			this.prevClientManifestAdds = new Dictionary<string, ClientManifestEntry>();
			if (this.prevClientManifest != null)
			{
				foreach (ClientManifestEntry clientManifestEntry in this.prevClientManifest.Values)
				{
					if (clientManifestEntry.ChangeType == ChangeType.Add)
					{
						this.prevClientManifestAdds.Add(clientManifestEntry.ClientAddId, clientManifestEntry);
					}
				}
			}
			Hashtable hashtable = new Hashtable();
			if (clientOperations != null)
			{
				for (int i = 0; i < clientOperations.Length; i++)
				{
					if (clientOperations[i].ChangeType == ChangeType.Add)
					{
						hashtable.Add(clientOperations[i].ClientAddId, clientOperations[i]);
					}
				}
			}
			foreach (ClientManifestEntry clientManifestEntry2 in this.prevClientManifest.Values)
			{
				switch (clientManifestEntry2.ChangeType)
				{
				case ChangeType.Add:
					this.CumulativeClientManifest.Remove(clientManifestEntry2.Id);
					this.ClientState.Remove(clientManifestEntry2.Id);
					if (!hashtable.Contains(clientManifestEntry2.ClientAddId) || this.DetectConflict(null, clientManifestEntry2.ClientAddId, ChangeType.Add) == ConflictResult.AcceptClientChange)
					{
						this.syncProvider.DeleteItems(new ISyncItemId[]
						{
							clientManifestEntry2.Id
						});
						continue;
					}
					continue;
				case ChangeType.Change:
					this.CumulativeClientManifest.Remove(clientManifestEntry2.Id);
					this.ClientState[clientManifestEntry2.Id].ChangeTrackingInformation = clientManifestEntry2.ChangeTrackingInformation;
					continue;
				case ChangeType.Delete:
					this.CumulativeClientManifest.Remove(clientManifestEntry2.Id);
					FolderSync.RecordClientHasItem(this.ClientState, clientManifestEntry2.Id, true, this.UsingQueryBasedFilter, clientManifestEntry2.MessageClass, clientManifestEntry2.FilterDate, clientManifestEntry2.ConversationId);
					continue;
				}
				throw new ArgumentOutOfRangeException(ServerStrings.ExInvalidChangeType(clientManifestEntry2.ChangeType.ToString()));
			}
			this.CurMaxWatermark = (ISyncWatermark)this.PrevMaxWatermark.Clone();
			this.CurServerManifest = FolderSync.CloneDictionary(this.PrevServerManifest);
			this.CurFilterId = this.PrevFilterId;
			this.CurSnapShotWatermark = this.PrevSnapShotWatermark;
			this.CurDelayedServerOperationQueue = FolderSync.CloneDictionary(this.PrevDelayedServerOperationQueue);
			this.CurLastSyncConversationMode = this.PrevLastSyncConversationMode;
		}

		public void SetConversationMode(bool enableConversationMode)
		{
			this.currentConversationMode = enableConversationMode;
		}

		public void SetSyncFilters(QueryFilter activeFilter, string stringId, params ISyncFilter[] statefulFilters)
		{
			ISyncFilter arg = null;
			if (this.filters != null && this.filters.Length > 0)
			{
				arg = this.filters[0];
			}
			this.SyncLogger.Information<ISyncFilter, QueryFilter>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::SetSyncFilters was = {0}, now = {1}", arg, activeFilter);
			this.filters = new ISyncFilter[statefulFilters.Length + 1];
			if (statefulFilters.Length == 0)
			{
				this.UsingQueryBasedFilter = true;
			}
			else
			{
				this.UsingQueryBasedFilter = false;
			}
			if (activeFilter == null)
			{
				if (statefulFilters.Length == 0)
				{
					throw new ArgumentException(ServerStrings.ExStatefulFilterMustBeSetWhenSetSyncFiltersIsInvokedWithNullFilter);
				}
				this.filters[0] = new NoSyncFilter();
			}
			else
			{
				this.filters[0] = new QueryBasedSyncFilter(activeFilter, stringId);
			}
			for (int i = 0; i < statefulFilters.Length; i++)
			{
				this.filters[i + 1] = statefulFilters[i];
			}
		}

		public void SetSyncFilters(ISyncFilter customActiveFilter, params ISyncFilter[] statefulFilters)
		{
			ISyncFilter arg = null;
			if (this.filters != null && this.filters.Length > 0)
			{
				arg = this.filters[0];
			}
			this.SyncLogger.Information<ISyncFilter, ISyncFilter>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::SetSyncFilters was = {0}, now = {1}", arg, customActiveFilter);
			this.UsingQueryBasedFilter = false;
			this.filters = new ISyncFilter[statefulFilters.Length + 1];
			if (customActiveFilter == null)
			{
				if (statefulFilters.Length == 0)
				{
					throw new ArgumentException(ServerStrings.ExStatefulFilterMustBeSetWhenSetSyncFiltersIsInvokedWithNullFilter);
				}
				this.filters[0] = new NoSyncFilter();
			}
			else
			{
				this.filters[0] = customActiveFilter;
			}
			for (int i = 0; i < statefulFilters.Length; i++)
			{
				this.filters[i + 1] = statefulFilters[i];
			}
		}

		public UpdateClientOperationResult UpdateClientOperation(ISyncClientOperation clientOperation)
		{
			if (clientOperation == null)
			{
				throw new ArgumentNullException("clientOperation");
			}
			if (clientOperation.ChangeType == ChangeType.Change && clientOperation.Id != null)
			{
				if (!this.ClientHasItem(clientOperation.Id))
				{
					return UpdateClientOperationResult.ClientStateMissingObject;
				}
				clientOperation.ChangeTrackingInformation = this.ClientState[clientOperation.Id].ChangeTrackingInformation;
			}
			else if (clientOperation.ChangeType == ChangeType.Delete && clientOperation.Id != null && !this.ClientHasItem(clientOperation.Id))
			{
				return UpdateClientOperationResult.ClientStateMissingObject;
			}
			return UpdateClientOperationResult.Success;
		}

		public virtual void UndoServerOperations()
		{
			if (this.backupCurMaxWatermarkHasBeenSet)
			{
				if (this.backupCurMaxWatermark != null)
				{
					this.CurMaxWatermark = this.backupCurMaxWatermark;
				}
				else
				{
					this.syncState.Remove(SyncStateProp.CurMaxWatermark);
				}
			}
			if (this.backupCurServerManifestHasBeenSet)
			{
				if (this.backupCurServerManifest != null)
				{
					this.CurServerManifest = this.backupCurServerManifest;
				}
				else
				{
					this.syncState.Remove(SyncStateProp.CurServerManifest);
				}
			}
			if (this.backupCurFilterIdHasBeenSet)
			{
				if (this.backupCurFilterId != null)
				{
					this.CurFilterId = this.backupCurFilterId;
				}
				else
				{
					this.syncState.Remove(SyncStateProp.CurFilterId);
				}
			}
			if (this.backupCurSnapShotWatermarkHasBeenSet)
			{
				if (this.backupCurSnapShotWatermark != null)
				{
					this.CurSnapShotWatermark = this.backupCurSnapShotWatermark;
				}
				else
				{
					this.syncState.Remove(SyncStateProp.CurSnapShotWatermark);
				}
			}
			if (this.backupCurDelayedServerOperationQueueHasBeenSet)
			{
				if (this.backupCurDelayedServerOperationQueue != null)
				{
					this.CurDelayedServerOperationQueue = this.backupCurDelayedServerOperationQueue;
				}
				else
				{
					this.syncState.Remove(SyncStateProp.CurDelayedServerOperationQueue);
				}
			}
			if (this.backupCurLastSyncConversationModeHasBeenSet)
			{
				this.CurLastSyncConversationMode = this.backupCurLastSyncConversationMode;
			}
		}

		internal bool ClientHasItem(ISyncItemId id)
		{
			FolderSync.ClientStateInformation clientStateInformation;
			return this.ClientState.TryGetValue(id, out clientStateInformation) && clientStateInformation.ClientHasItem;
		}

		internal void CommitAllDeferredModifiers()
		{
			this.CommitModifyState(this.clientStateModifiers);
		}

		internal ISyncItem GetItem(ServerManifestEntry serverManifestEntry, params PropertyDefinition[] prefetchProperties)
		{
			if (ChangeType.Delete != serverManifestEntry.ChangeType)
			{
				ISyncItem item = this.GetItem(serverManifestEntry.Id, prefetchProperties);
				serverManifestEntry.Watermark = item.Watermark;
				return item;
			}
			throw new InvalidOperationException(ServerStrings.ExCannotGetDeletedItem);
		}

		internal void QueueDelayedServerOperation(ServerManifestEntry serverManifestEntry)
		{
			this.SyncLogger.Information(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "Storage.FolderSync.QueueDelayedServerOperation.");
			if (ChangeType.Delete != serverManifestEntry.ChangeType)
			{
				this.RejectServerOperation(serverManifestEntry);
			}
			ServerManifestEntry serverManifestEntry2 = new ServerManifestEntry(serverManifestEntry.Id);
			serverManifestEntry2.ChangeType = serverManifestEntry.ChangeType;
			serverManifestEntry2.ConversationId = serverManifestEntry.ConversationId;
			serverManifestEntry2.FilterDate = serverManifestEntry.FilterDate;
			serverManifestEntry2.MessageClass = serverManifestEntry.MessageClass;
			if (serverManifestEntry.ChangeType == ChangeType.Change)
			{
				serverManifestEntry2.ChangeType = ChangeType.Add;
			}
			serverManifestEntry2.IsDelayedServerOperation = true;
			if (serverManifestEntry2.ChangeType != ChangeType.AssociatedAdd)
			{
				serverManifestEntry2.Watermark = this.CurMaxWatermark;
			}
			this.CurDelayedServerOperationQueue[serverManifestEntry2.Id] = serverManifestEntry2;
		}

		internal void RejectServerOperation(ServerManifestEntry serverManifestEntry)
		{
			this.SyncLogger.Information(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "Storage.FolderSync.RejectServerOperation.");
			if (ChangeType.Delete == serverManifestEntry.ChangeType)
			{
				throw new InvalidOperationException(ServerStrings.ExCannotRejectDeletes);
			}
			ServerManifestEntry serverManifestEntry2;
			if (this.CurServerManifest.TryGetValue(serverManifestEntry.Id, out serverManifestEntry2))
			{
				serverManifestEntry2.IsRejected = true;
			}
		}

		public static Dictionary<ISyncItemId, ServerManifestEntry> CloneDictionary(Dictionary<ISyncItemId, ServerManifestEntry> dictionary)
		{
			if (dictionary == null)
			{
				return new Dictionary<ISyncItemId, ServerManifestEntry>(0);
			}
			return new Dictionary<ISyncItemId, ServerManifestEntry>(dictionary);
		}

		protected static QueryFilter ComputeQueryHint(ISyncFilter[] filters)
		{
			QueryBasedSyncFilter queryBasedSyncFilter = filters[0] as QueryBasedSyncFilter;
			if (queryBasedSyncFilter == null)
			{
				return null;
			}
			return queryBasedSyncFilter.FilterQuery;
		}

		private static void RecordClientHasItem(Dictionary<ISyncItemId, FolderSync.ClientStateInformation> clientState, ISyncItemId id, bool clientHasItem, bool usingQueryBasedFilter, string messageClass, ExDateTime? filterDate, ConversationId conversationId)
		{
			FolderSync.ClientStateInformation clientStateInformation;
			bool flag = clientState.TryGetValue(id, out clientStateInformation);
			if (usingQueryBasedFilter && !clientHasItem)
			{
				if (flag && !clientStateInformation.IsRejected)
				{
					clientState.Remove(id);
				}
				return;
			}
			if (!flag)
			{
				clientStateInformation = new FolderSync.ClientStateInformation();
				clientState[id] = clientStateInformation;
			}
			clientStateInformation.ClientHasItem = clientHasItem;
			clientStateInformation.MessageClass = messageClass;
			clientStateInformation.FilterDate = filterDate;
			clientStateInformation.ConversationId = conversationId;
		}

		private static void RecordClientReject(Dictionary<ISyncItemId, FolderSync.ClientStateInformation> clientState, ISyncItemId id, bool isRejected)
		{
			FolderSync.ClientStateInformation clientStateInformation;
			if (!clientState.TryGetValue(id, out clientStateInformation))
			{
				clientStateInformation = new FolderSync.ClientStateInformation();
				clientStateInformation.ClientHasItem = false;
				clientState[id] = clientStateInformation;
			}
			clientStateInformation.IsRejected = isRejected;
		}

		private void CommitAcknowledgeServerOperations()
		{
			if (!this.preparedAcknowledgeServerOperations)
			{
				return;
			}
			this.preparedAcknowledgeServerOperations = false;
			this.acknowledgeModifications.Commit(this.ClientState);
			this.CommitPreviousState();
		}

		protected virtual void CommitPreviousState()
		{
			ISyncWatermark prevMaxWatermark;
			Dictionary<ISyncItemId, ServerManifestEntry> prevServerManifest;
			string prevFilterId;
			Dictionary<ISyncItemId, ServerManifestEntry> prevDelayedServerOperationQueue;
			ISyncWatermark prevSnapShotWatermark;
			bool prevLastSyncConversationMode;
			this.acknowledgeModifications.CommitPreviousState(out prevMaxWatermark, out prevServerManifest, out prevFilterId, out prevDelayedServerOperationQueue, out prevSnapShotWatermark, out prevLastSyncConversationMode);
			this.PrevMaxWatermark = prevMaxWatermark;
			this.PrevServerManifest = prevServerManifest;
			this.PrevFilterId = prevFilterId;
			this.PrevDelayedServerOperationQueue = prevDelayedServerOperationQueue;
			this.PrevSnapShotWatermark = prevSnapShotWatermark;
			this.PrevLastSyncConversationMode = prevLastSyncConversationMode;
		}

		protected void CommitModifyState(List<FolderSync.StateModifier> modifiers)
		{
			if (modifiers.Count == 0)
			{
				return;
			}
			List<FolderSync.StateModifier> list = new List<FolderSync.StateModifier>(modifiers);
			modifiers.Clear();
			foreach (FolderSync.StateModifier stateModifier in list)
			{
				stateModifier();
			}
		}

		private void ComputeSoftAddsUsingQueryBasedFilter(QueryBasedSyncFilter queryBasedSyncFilter, Dictionary<ISyncItemId, ServerManifestEntry> newServerManifest)
		{
			ExDateTime exDateTime = ExDateTime.MaxValue;
			foreach (ServerManifestEntry serverManifestEntry in queryBasedSyncFilter.EntriesInFilter.Values)
			{
				FolderSync.ClientStateInformation clientStateInformation;
				if (!this.ClientState.TryGetValue(serverManifestEntry.Id, out clientStateInformation) || !clientStateInformation.ClientHasItem)
				{
					if (serverManifestEntry.FilterDate != null && serverManifestEntry.FilterDate < exDateTime)
					{
						exDateTime = serverManifestEntry.FilterDate.Value;
					}
					if (clientStateInformation != null && clientStateInformation.IsRejected)
					{
						this.SyncLogger.TraceDebug<ISyncItemId>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[FolderSync.ComputeSoftAddsUsingQueryBasedFilter] Discarding item '{0}' because client rejected it.", serverManifestEntry.Id);
					}
					else
					{
						serverManifestEntry.ChangeType = ChangeType.Add;
						serverManifestEntry.Watermark = null;
						newServerManifest[serverManifestEntry.Id] = serverManifestEntry;
					}
				}
			}
			this.UpdateWatermarkFromMinReceivedDate(exDateTime);
		}

		protected virtual void UpdateWatermarkFromMinReceivedDate(ExDateTime minReceivedDate)
		{
		}

		private bool ComputeSoftOperationsBelowWatermark(ISyncFilter syncFilter, int windowSize, ref int totalOperationsInFilter, Dictionary<ISyncItemId, ServerManifestEntry> newServerManifest, HashSet<ISyncItemId> itemsDeleted)
		{
			QueryBasedSyncFilter queryBasedSyncFilter = syncFilter as QueryBasedSyncFilter;
			List<ISyncItemId> list = new List<ISyncItemId>();
			this.SyncLogger.Information<int, int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ComputeSoftOperationsBelowWatermark newServerManifest.Count {0}, ClientState.Count {1}", newServerManifest.Count, this.ClientState.Count);
			foreach (KeyValuePair<ISyncItemId, FolderSync.ClientStateInformation> keyValuePair in this.ClientState)
			{
				ISyncItemId key = keyValuePair.Key;
				FolderSync.ClientStateInformation value = keyValuePair.Value;
				if (itemsDeleted.Contains(key))
				{
					this.SyncLogger.Information<ISyncItemId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ComputeSoftOperationsBelowWatermark Contains id {0}", key);
				}
				else if (!this.ClientHasItem(key))
				{
					this.SyncLogger.Information<ISyncItemId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ComputeSoftOperationsBelowWatermark client does not have id {0}", key);
					if (syncFilter == null || syncFilter.IsItemInFilter(key))
					{
						this.SyncLogger.Information<ISyncItemId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ComputeSoftOperationsBelowWatermark item in filter id {0}", key);
						if (!value.IsRejected)
						{
							ServerManifestEntry serverManifestEntry = new ServerManifestEntry(key);
							serverManifestEntry.ChangeType = ChangeType.Add;
							if (queryBasedSyncFilter != null && !queryBasedSyncFilter.EntriesInFilter.ContainsKey(serverManifestEntry.Id))
							{
								this.SyncLogger.Information<ISyncItemId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ComputeSoftOperationsBelowWatermark item deleted before folder change, id {0}", key);
								list.Add(key);
							}
							else
							{
								serverManifestEntry.ConversationId = value.ConversationId;
								serverManifestEntry.FilterDate = value.FilterDate;
								serverManifestEntry.MessageClass = value.MessageClass;
								serverManifestEntry.Watermark = null;
								ServerManifestEntry serverManifestEntry2;
								if (!newServerManifest.TryGetValue(serverManifestEntry.Id, out serverManifestEntry2) || serverManifestEntry2.ChangeType == ChangeType.OutOfFilter)
								{
									totalOperationsInFilter++;
								}
								this.SyncLogger.Information<ISyncItemId, bool>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ComputeSoftOperationsBelowWatermark id {0}, newServerManifest contains the id {1}", key, newServerManifest.ContainsKey(serverManifestEntry.Id));
								newServerManifest[serverManifestEntry.Id] = serverManifestEntry;
							}
						}
					}
				}
				else if (syncFilter != null && !syncFilter.IsItemInFilter(key))
				{
					this.SyncLogger.Information<ISyncItemId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ComputeSoftOperationsBelowWatermark item is not in sync filter, id {0}", key);
					if (!this.currentConversationMode || !this.CurrentConversationIdList.Contains(value.ConversationId))
					{
						ClientManifestEntry clientManifestEntry;
						this.CumulativeClientManifest.TryGetValue(key, out clientManifestEntry);
						ServerManifestEntry serverManifestEntry3;
						newServerManifest.TryGetValue(key, out serverManifestEntry3);
						if (serverManifestEntry3 != null && clientManifestEntry != null && clientManifestEntry.SoftDeletePending)
						{
							this.SyncLogger.Information<ISyncItemId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ComputeSoftOperationsBelowWatermark SoftDeletePending,  id {0}", key);
						}
						else if (serverManifestEntry3 != null && serverManifestEntry3.ChangeType == ChangeType.Delete)
						{
							this.SyncLogger.Information<ISyncItemId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ComputeSoftOperationsBelowWatermark no need to override, id {0}", key);
						}
						else
						{
							if (serverManifestEntry3 != null)
							{
								this.SyncLogger.Information<ServerManifestEntry>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ComputeSoftOperationsBelowWatermark Old {0}", serverManifestEntry3);
							}
							serverManifestEntry3 = new ServerManifestEntry(key);
							serverManifestEntry3.ChangeType = ChangeType.SoftDelete;
							serverManifestEntry3.ConversationId = value.ConversationId;
							serverManifestEntry3.FilterDate = value.FilterDate;
							serverManifestEntry3.MessageClass = value.MessageClass;
							ServerManifestEntry serverManifestEntry4;
							if (!newServerManifest.TryGetValue(serverManifestEntry3.Id, out serverManifestEntry4) || serverManifestEntry4.ChangeType == ChangeType.OutOfFilter)
							{
								totalOperationsInFilter++;
							}
							this.SyncLogger.Information<ServerManifestEntry>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ComputeSoftOperationsBelowWatermark New {0}", serverManifestEntry3);
							this.SyncLogger.Information<ISyncItemId, bool>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ComputeSoftOperationsBelowWatermark (SoftDelete) id {0}, newServerManifest contains the id {1}", key, newServerManifest.ContainsKey(serverManifestEntry3.Id));
							newServerManifest[serverManifestEntry3.Id] = serverManifestEntry3;
						}
					}
				}
			}
			foreach (ISyncItemId key2 in list)
			{
				this.ClientState.Remove(key2);
			}
			return this.PushFilterChangeExtrasIntoDelayQueue(newServerManifest, ref totalOperationsInFilter, windowSize);
		}

		private FolderSync.HasRecoveryAddItemChangedResult HasRecoveryAddItemChanged(string clientAddId)
		{
			ClientManifestEntry clientManifestEntry;
			if (this.prevClientManifestAdds.TryGetValue(clientAddId, out clientManifestEntry))
			{
				ISyncItem syncItem = null;
				try
				{
					FolderSync.TryGetItemResult tryGetItemResult = this.TryGetItem(clientManifestEntry.Id, out syncItem);
					if (FolderSync.TryGetItemResult.Success == tryGetItemResult && !clientManifestEntry.Watermark.Equals(syncItem.Watermark))
					{
						return FolderSync.HasRecoveryAddItemChangedResult.ItemChanged;
					}
				}
				finally
				{
					if (syncItem != null)
					{
						syncItem.Dispose();
					}
				}
				return FolderSync.HasRecoveryAddItemChangedResult.ItemDidNotChange;
			}
			return FolderSync.HasRecoveryAddItemChangedResult.ItemDidNotChange;
		}

		protected bool IsFirstSyncScenario
		{
			get
			{
				return this.CurMaxWatermark.IsNew;
			}
		}

		private bool IsItemVersionInCumulativeClientManifest(ISyncItemId id, ISyncWatermark watermark)
		{
			ClientManifestEntry clientManifestEntry;
			return this.CumulativeClientManifest.TryGetValue(id, out clientManifestEntry) && (clientManifestEntry.ChangeType == ChangeType.Send || (clientManifestEntry.ChangeType != ChangeType.Delete && clientManifestEntry.Watermark.Equals(watermark)));
		}

		private bool IsItemVersionInCumulativeClientManifest(ServerManifestEntry serverManifest, bool readFlagOptimizationApplied)
		{
			ISyncItemId id = serverManifest.Id;
			ClientManifestEntry clientManifestEntry;
			if (this.CumulativeClientManifest.TryGetValue(id, out clientManifestEntry))
			{
				if (clientManifestEntry.ChangeType == ChangeType.Send)
				{
					return true;
				}
				if (readFlagOptimizationApplied)
				{
					return serverManifest.ChangeType == ChangeType.ReadFlagChange && serverManifest.IsReadFlagInitialized && clientManifestEntry.IsRead == serverManifest.IsRead;
				}
				this.tempSyncOperation.Bind(this, serverManifest, true);
				this.tempSyncOperation.EnsureServerManifestWatermark();
				if (clientManifestEntry.ChangeType != ChangeType.Delete && clientManifestEntry.Watermark.Equals(serverManifest.Watermark))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsItemVersionInPrevClientManifest(ISyncItemId id, ISyncWatermark watermark)
		{
			ClientManifestEntry clientManifestEntry;
			return this.prevClientManifest.TryGetValue(id, out clientManifestEntry) && clientManifestEntry.ChangeType != ChangeType.Delete && clientManifestEntry.Watermark.Equals(watermark);
		}

		private bool IsItemVersionInServerManifest(ISyncItemId id, ISyncWatermark watermark)
		{
			ServerManifestEntry serverManifestEntry;
			return this.CurServerManifest.TryGetValue(id, out serverManifestEntry) && serverManifestEntry != null && serverManifestEntry.ChangeType != ChangeType.Delete && serverManifestEntry.Watermark != null && serverManifestEntry.Watermark.Equals(watermark);
		}

		private bool IsSafeToUseSnapShotQueryHint(ISyncFilter[] filters)
		{
			return this.filters.Length <= 1 && FolderSync.ComputeQueryHint(filters) != null;
		}

		protected void PrepareModifyState(List<FolderSync.StateModifier> modifiers, FolderSync.StateModifier modifier)
		{
			if (this.deferStateModifications)
			{
				modifiers.Add(modifier);
				return;
			}
			modifier();
		}

		private FolderSync.ProcessServerOperationResult ProcessServerOperationAboveWatermark(ServerManifestEntry serverManifestEntry, Dictionary<ISyncItemId, ServerManifestEntry> newServerManifest)
		{
			if (ChangeType.Delete == serverManifestEntry.ChangeType && !this.ClientHasItem(serverManifestEntry.Id))
			{
				if (this.ClientState.ContainsKey(serverManifestEntry.Id))
				{
					this.ClientState.Remove(serverManifestEntry.Id);
				}
				return FolderSync.ProcessServerOperationResult.RejectedAsRedundant;
			}
			if (ChangeType.Add == serverManifestEntry.ChangeType && this.ClientHasItem(serverManifestEntry.Id))
			{
				serverManifestEntry.ChangeType = ChangeType.Change;
			}
			serverManifestEntry.IsRejected = false;
			bool readFlagOptimizationApplied = false;
			if (serverManifestEntry.ChangeType != ChangeType.Add && serverManifestEntry.ChangeType != ChangeType.Change)
			{
				if (serverManifestEntry.ChangeType != ChangeType.ReadFlagChange)
				{
					goto IL_3D1;
				}
			}
			bool flag;
			try
			{
				if (serverManifestEntry.ChangeType == ChangeType.ReadFlagChange && this.fastReadFlagFilterCheck)
				{
					flag = this.ClientHasItem(serverManifestEntry.Id);
					readFlagOptimizationApplied = true;
				}
				else
				{
					try
					{
						this.tempSyncOperation.Bind(this, serverManifestEntry, true);
						if (this.CurSnapShotWatermark == null)
						{
							for (int i = 0; i < this.filters.Length; i++)
							{
								this.filters[i].UpdateFilterState(this.tempSyncOperation);
							}
							flag = this.filters[0].IsItemInFilter(serverManifestEntry.Id);
							MailboxSyncProvider mailboxSyncProvider = this.SyncProvider as MailboxSyncProvider;
							if (flag && mailboxSyncProvider != null && !(mailboxSyncProvider.ItemQueryOptimizationFilter is FalseFilter) && mailboxSyncProvider.ItemQueryOptimizationFilter != null)
							{
								flag = EvaluatableFilter.Evaluate(mailboxSyncProvider.ItemQueryOptimizationFilter, this.tempSyncOperation);
							}
						}
						else
						{
							flag = true;
						}
						if (serverManifestEntry.IsDelayedServerOperation)
						{
							this.SyncLogger.Information<ISyncItemId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ProcessServerOperationAboveWatermark. Checking delayedOperation with ID {0}", serverManifestEntry.Id);
							ISyncItem item = this.tempSyncOperation.GetItem(new PropertyDefinition[0]);
							if (item.Watermark.CompareTo(serverManifestEntry.Watermark) < 0)
							{
								return FolderSync.ProcessServerOperationResult.RejectedAsRedundant;
							}
						}
					}
					catch (WrongObjectTypeException ex)
					{
						this.SyncLogger.TraceError<string, ISyncItemId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ProcessServerOperationAboveWatermark. Exception {0} while fetching item. ManifestEntryID:{1}", ex.ToString(), serverManifestEntry.Id);
						return FolderSync.ProcessServerOperationResult.RejectedAsRedundant;
					}
					catch (ConversionFailedException ex2)
					{
						this.SyncLogger.TraceError<string, ISyncItemId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ProcessServerOperationAboveWatermark. Exception {0} while fetching item. ManifestEntryID:{1}", ex2.ToString(), serverManifestEntry.Id);
						ExWatson.SendReport(ex2, ReportOptions.DoNotCollectDumps, null);
						return FolderSync.ProcessServerOperationResult.RejectedAsRedundant;
					}
					finally
					{
						this.tempSyncOperation.DisposeCachedItem();
					}
				}
				FolderSync.ClientStateInformation clientStateInformation;
				if (this.ClientState.TryGetValue(serverManifestEntry.Id, out clientStateInformation))
				{
					if (serverManifestEntry.FilterDate == null)
					{
						serverManifestEntry.FilterDate = clientStateInformation.FilterDate;
					}
					if (serverManifestEntry.MessageClass == null)
					{
						serverManifestEntry.MessageClass = clientStateInformation.MessageClass;
					}
					if (serverManifestEntry.ConversationId == null)
					{
						serverManifestEntry.ConversationId = clientStateInformation.ConversationId;
					}
				}
				if (this.currentConversationMode && serverManifestEntry.ConversationId != null)
				{
					this.SyncLogger.Information<ConversationId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ProcessServerOperationAboveWatermark. Checking ConversationId {0}", serverManifestEntry.ConversationId);
					if (flag)
					{
						this.SyncLogger.Information<ConversationId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ProcessServerOperationAboveWatermark. Adding ConversationId {0} to ConversationIdList", serverManifestEntry.ConversationId);
						bool flag2 = this.CurrentConversationIdList.Add(serverManifestEntry.ConversationId);
						if (flag2 && serverManifestEntry.ChangeType == ChangeType.Add)
						{
							if (serverManifestEntry.FirstMessageInConversation)
							{
								this.SyncLogger.Information<ConversationId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ProcessServerOperationAboveWatermark. Adding ConversationId {0} to FirstMessageInFilterConversationIdList", serverManifestEntry.ConversationId);
								this.FirstMessageInFilterConversationIdList.Add(serverManifestEntry.ConversationId);
							}
							else
							{
								this.SyncLogger.Information<ConversationId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ProcessServerOperationAboveWatermark. Adding ConversationId {0} to AssociatedAddConversationIdList", serverManifestEntry.ConversationId);
								this.AssociatedAddConversationIdList.Add(serverManifestEntry.ConversationId);
							}
						}
					}
					else if (this.CurrentConversationIdList.Contains(serverManifestEntry.ConversationId))
					{
						this.SyncLogger.Information<ISyncItemId, ConversationId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ProcessServerOperationAboveWatermark. Keeping item {0} back to filter because the ConversationId {1} is in filter", serverManifestEntry.Id, serverManifestEntry.ConversationId);
						flag = true;
						if (serverManifestEntry.ChangeType == ChangeType.Add)
						{
							serverManifestEntry.ChangeType = ChangeType.AssociatedAdd;
						}
					}
				}
				goto IL_56F;
			}
			catch (ObjectNotFoundException)
			{
				serverManifestEntry.ChangeType = ChangeType.Delete;
				return this.ProcessServerOperationAboveWatermark(serverManifestEntry, newServerManifest);
			}
			goto IL_3D1;
			IL_56F:
			if (!flag)
			{
				switch (serverManifestEntry.ChangeType)
				{
				case ChangeType.Add:
				case ChangeType.SoftDelete:
					goto IL_681;
				case ChangeType.Change:
				case ChangeType.ReadFlagChange:
					if (this.ClientHasItem(serverManifestEntry.Id))
					{
						serverManifestEntry.ChangeType = ChangeType.SoftDelete;
						FolderSync.ProcessServerOperationResult result = FolderSync.ProcessServerOperationResult.AddedToManifest;
						ServerManifestEntry serverManifestEntry2;
						if (newServerManifest.TryGetValue(serverManifestEntry.Id, out serverManifestEntry2))
						{
							result = FolderSync.ProcessServerOperationResult.UpdatedInManifest;
							ClientManifestEntry clientManifestEntry;
							if (serverManifestEntry2.ChangeType == ChangeType.SoftDelete && this.CumulativeClientManifest.TryGetValue(serverManifestEntry.Id, out clientManifestEntry))
							{
								clientManifestEntry.SoftDeletePending = false;
							}
						}
						newServerManifest[serverManifestEntry.Id] = serverManifestEntry;
						return result;
					}
					goto IL_681;
				}
				throw new InvalidOperationException(ServerStrings.ExInvalidChangeType(serverManifestEntry.ChangeType.ToString()));
				IL_681:
				serverManifestEntry.ChangeType = ChangeType.OutOfFilter;
				newServerManifest[serverManifestEntry.Id] = serverManifestEntry;
				return FolderSync.ProcessServerOperationResult.RejectedAsOutOfFilter;
			}
			if (this.IsItemVersionInCumulativeClientManifest(serverManifestEntry, readFlagOptimizationApplied))
			{
				return FolderSync.ProcessServerOperationResult.RejectedAsReflection;
			}
			FolderSync.ProcessServerOperationResult result2 = FolderSync.ProcessServerOperationResult.AddedToManifest;
			ServerManifestEntry serverManifestEntry3;
			if (newServerManifest.TryGetValue(serverManifestEntry.Id, out serverManifestEntry3))
			{
				result2 = FolderSync.ProcessServerOperationResult.UpdatedInManifest;
				ClientManifestEntry clientManifestEntry2;
				if (serverManifestEntry3.ChangeType == ChangeType.SoftDelete && serverManifestEntry.ChangeType != ChangeType.SoftDelete && this.CumulativeClientManifest.TryGetValue(serverManifestEntry.Id, out clientManifestEntry2))
				{
					clientManifestEntry2.SoftDeletePending = false;
				}
			}
			newServerManifest[serverManifestEntry.Id] = serverManifestEntry;
			return result2;
			IL_3D1:
			if (serverManifestEntry.ChangeType == ChangeType.Delete || serverManifestEntry.ChangeType == ChangeType.SoftDelete)
			{
				this.tempSyncOperation.Bind(this, serverManifestEntry, false);
				for (int j = 0; j < this.filters.Length; j++)
				{
					this.filters[j].UpdateFilterState(this.tempSyncOperation);
				}
				FolderSync.ProcessServerOperationResult result3 = FolderSync.ProcessServerOperationResult.AddedToManifest;
				ServerManifestEntry serverManifestEntry4;
				if (newServerManifest.TryGetValue(serverManifestEntry.Id, out serverManifestEntry4))
				{
					result3 = FolderSync.ProcessServerOperationResult.UpdatedInManifest;
					ClientManifestEntry clientManifestEntry3;
					if (serverManifestEntry4.ChangeType == ChangeType.SoftDelete && serverManifestEntry.ChangeType == ChangeType.Delete && this.CumulativeClientManifest.TryGetValue(serverManifestEntry.Id, out clientManifestEntry3))
					{
						clientManifestEntry3.SoftDeletePending = false;
					}
				}
				newServerManifest[serverManifestEntry.Id] = serverManifestEntry;
				return result3;
			}
			if (serverManifestEntry.ChangeType != ChangeType.AssociatedAdd)
			{
				string changeType = string.Format("{0}, IsAcknowledgedByClient:{1}, IsDelayedServerOperation:{2}, IsRejected:{3}, IsNew:{4}, MessageClass:{5}", new object[]
				{
					serverManifestEntry.ChangeType.ToString(),
					serverManifestEntry.IsAcknowledgedByClient,
					serverManifestEntry.IsDelayedServerOperation,
					serverManifestEntry.IsRejected,
					serverManifestEntry.IsNew,
					serverManifestEntry.MessageClass
				});
				throw new InvalidOperationException(ServerStrings.ExInvalidChangeType(changeType));
			}
			if (!this.currentConversationMode)
			{
				throw new InvalidOperationException("There shouldn't be any AssociatedAdd item still in delayed operation queue when conversation mode is off!");
			}
			this.SyncLogger.Information<ISyncItemId, ConversationId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ProcessServerOperationAboveWatermark. Associated-Add {0}, ConversationId {1} from DelayOperationQueue", serverManifestEntry.Id, serverManifestEntry.ConversationId);
			FolderSync.ProcessServerOperationResult result4 = FolderSync.ProcessServerOperationResult.AddedToManifest;
			if (newServerManifest.ContainsKey(serverManifestEntry.Id) || this.ClientHasItem(serverManifestEntry.Id))
			{
				result4 = FolderSync.ProcessServerOperationResult.RejectedAsRedundant;
			}
			else
			{
				newServerManifest[serverManifestEntry.Id] = serverManifestEntry;
			}
			return result4;
		}

		private FolderSync.TryGetItemResult TryGetItem(ISyncItemId id, out ISyncItem item)
		{
			item = null;
			FolderSync.TryGetItemResult result;
			try
			{
				item = this.GetItem(id, new PropertyDefinition[0]);
				result = FolderSync.TryGetItemResult.Success;
			}
			catch (Exception ex)
			{
				if (!(ex is ObjectNotFoundException) && !(ex is ArgumentException))
				{
					throw;
				}
				result = FolderSync.TryGetItemResult.NotFound;
			}
			return result;
		}

		private bool ComputeAssociatedAddForConversationMode(int windowSize, ref int totalOperationsInFilter, Dictionary<ISyncItemId, ServerManifestEntry> newServerManifest)
		{
			bool result = false;
			foreach (ConversationId conversationId in this.AssociatedAddConversationIdList)
			{
				if (this.FirstMessageInFilterConversationIdList.Contains(conversationId))
				{
					this.SyncLogger.Information<ConversationId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ComputeAssociatedAddForConversationMode. Skipping ConversationId {1} because its first message is in filter", conversationId);
				}
				else
				{
					List<IConversationTreeNode> inFolderItemsForConversation = this.syncProvider.GetInFolderItemsForConversation(conversationId);
					if (inFolderItemsForConversation != null)
					{
						foreach (IConversationTreeNode conversationTreeNode in inFolderItemsForConversation)
						{
							VersionedId versionedId = conversationTreeNode.StorePropertyBags[0].TryGetProperty(ItemSchema.Id) as VersionedId;
							if (versionedId != null)
							{
								ISyncItemId syncItemId = this.syncProvider.CreateISyncItemIdForNewItem(versionedId.ObjectId);
								ServerManifestEntry serverManifestEntry;
								if (newServerManifest.TryGetValue(syncItemId, out serverManifestEntry))
								{
									if (serverManifestEntry.ChangeType != ChangeType.OutOfFilter)
									{
										this.SyncLogger.Information<ISyncItemId, ConversationId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ComputeAssociatedAddForConversationMode. Skipping item {0}, ConversationId {1} because its already in current server manifests", syncItemId, conversationId);
										continue;
									}
									serverManifestEntry.ChangeType = ChangeType.AssociatedAdd;
									totalOperationsInFilter++;
									this.SyncLogger.Information<ISyncItemId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ComputeAssociatedAddForConversationMode. bring OutOfFilter item {0} back as associated-add", syncItemId);
								}
								FolderSync.ClientStateInformation clientStateInformation;
								if (this.ClientState.TryGetValue(syncItemId, out clientStateInformation) && clientStateInformation.IsRejected)
								{
									this.SyncLogger.Information<ISyncItemId, ConversationId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ComputeAssociatedAddForConversationMode. Skipping item {0}, ConversationId {1} because its mark as rejected by client", syncItemId, conversationId);
								}
								else if (!this.CurDelayedServerOperationQueue.ContainsKey(syncItemId) && !this.ClientHasItem(syncItemId) && !this.filters[0].IsItemInFilter(syncItemId))
								{
									ServerManifestEntry serverManifestEntry2 = new ServerManifestEntry(syncItemId);
									serverManifestEntry2.ChangeType = ChangeType.AssociatedAdd;
									serverManifestEntry2.ConversationId = conversationId;
									serverManifestEntry2.MessageClass = (conversationTreeNode.StorePropertyBags[0].TryGetProperty(StoreObjectSchema.ItemClass) as string);
									serverManifestEntry2.Watermark = null;
									if (-1 != windowSize && totalOperationsInFilter >= windowSize)
									{
										this.SyncLogger.Information<ISyncItemId, ConversationId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ComputeAssociatedAddForConversationMode. Queue Associated-Add for item {0}, ConversationId {1} in DelayedOperationQueue", syncItemId, conversationId);
										this.QueueDelayedServerOperation(serverManifestEntry2);
										result = true;
									}
									else
									{
										this.SyncLogger.Information<ISyncItemId, ConversationId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::ComputeAssociatedAddForConversationMode. Add Associated-Add for item {0}, ConversationId {1} to current manifests", syncItemId, conversationId);
										if (serverManifestEntry == null || serverManifestEntry.ChangeType == ChangeType.OutOfFilter)
										{
											totalOperationsInFilter++;
										}
										newServerManifest[serverManifestEntry2.Id] = serverManifestEntry2;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		private void GenerateCurrentConversationIdList(Dictionary<ISyncItemId, ServerManifestEntry> newServerManifest, bool itemsInFilterInitialized)
		{
			if (newServerManifest == null)
			{
				throw new ArgumentNullException("newServerManifest");
			}
			foreach (KeyValuePair<ISyncItemId, FolderSync.ClientStateInformation> keyValuePair in this.ClientState)
			{
				ServerManifestEntry serverManifestEntry;
				if (keyValuePair.Value.ConversationId != null && this.ClientHasItem(keyValuePair.Key) && (!newServerManifest.TryGetValue(keyValuePair.Key, out serverManifestEntry) || serverManifestEntry.ChangeType != ChangeType.SoftDelete) && (!itemsInFilterInitialized || this.filters[0].IsItemInFilter(keyValuePair.Key)))
				{
					this.SyncLogger.Information<ConversationId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::GenerateCurrentConversationIdList. Adding ConversationId {0} to ConversationIdList", keyValuePair.Value.ConversationId);
					this.CurrentConversationIdList.Add(keyValuePair.Value.ConversationId);
					if (!this.CurLastSyncConversationMode && this.currentConversationMode)
					{
						this.SyncLogger.Information<ConversationId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::GenerateCurrentConversationIdList. Adding ConversationId {0} to AssociatedAddConversationIdList", keyValuePair.Value.ConversationId);
						this.AssociatedAddConversationIdList.Add(keyValuePair.Value.ConversationId);
					}
				}
			}
			foreach (KeyValuePair<ISyncItemId, ServerManifestEntry> keyValuePair2 in newServerManifest)
			{
				if (keyValuePair2.Value.ConversationId != null && keyValuePair2.Value.ChangeType == ChangeType.Add)
				{
					this.SyncLogger.Information<ConversationId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::GenerateCurrentConversationIdList. Adding ConversationId {0} to ConversationIdList", keyValuePair2.Value.ConversationId);
					this.CurrentConversationIdList.Add(keyValuePair2.Value.ConversationId);
					if (keyValuePair2.Value.FirstMessageInConversation)
					{
						this.SyncLogger.Information<ConversationId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::GenerateCurrentConversationIdList. Adding ConversationId {0} to FirstMessageInFilterConversationIdList", keyValuePair2.Value.ConversationId);
						this.FirstMessageInFilterConversationIdList.Add(keyValuePair2.Value.ConversationId);
					}
					else
					{
						this.SyncLogger.Information<ConversationId>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::GenerateCurrentConversationIdList. Adding ConversationId {0} to AssociatedAddConversationIdList", keyValuePair2.Value.ConversationId);
						this.AssociatedAddConversationIdList.Add(keyValuePair2.Value.ConversationId);
					}
				}
			}
			this.SyncLogger.Information<int, int, int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderSync::GenerateCurrentConversationIdList. ConversationIdList.Count {0}, AssociatedAddConversationIdList.Count = {1}, FirstMessageInFilterConversationIdList.Count = {2}", this.CurrentConversationIdList.Count, this.AssociatedAddConversationIdList.Count, this.FirstMessageInFilterConversationIdList.Count);
		}

		public const int MaximumWindowSize = -1;

		protected const int ExpectedNumberOfDeferredModifications = 5;

		private const int IdxActiveFilter = 0;

		private static readonly TimeSpanAppSettingsEntry maxElapsedTime = new TimeSpanAppSettingsEntry("FolderSync.MaxElapsedTimeInSec", TimeSpanUnit.Seconds, TimeSpan.FromSeconds(20.0), ExTraceGlobals.SyncTracer);

		protected FolderSync.AcknowledgeModifications acknowledgeModifications;

		private List<FolderSync.StateModifier> clientStateModifiers = new List<FolderSync.StateModifier>(5);

		private ConflictResolutionPolicy conflictPolicy;

		private bool deferStateModifications = true;

		protected ISyncFilter[] filters;

		private bool currentlyInRecovery;

		protected bool shouldBackUpState;

		private bool preparedAcknowledgeServerOperations;

		private Dictionary<ISyncItemId, ClientManifestEntry> prevClientManifest;

		private Dictionary<string, ClientManifestEntry> prevClientManifestAdds;

		private List<FolderSync.StateModifier> prevDelayedServerOperationQueueModifiers = new List<FolderSync.StateModifier>(5);

		private List<FolderSync.StateModifier> prevFilterIdModifiers = new List<FolderSync.StateModifier>(5);

		private List<FolderSync.StateModifier> prevMaxWatermarkModifiers = new List<FolderSync.StateModifier>(5);

		private List<FolderSync.StateModifier> prevServerManifestModifiers = new List<FolderSync.StateModifier>(5);

		private List<FolderSync.StateModifier> prevSnapShotWatermarkModifiers = new List<FolderSync.StateModifier>(5);

		private List<FolderSync.StateModifier> prevLastSyncConversationModeModifiers = new List<FolderSync.StateModifier>(5);

		protected ISyncProvider syncProvider;

		protected IFolderSyncState syncState;

		private SyncOperation tempSyncOperation = new SyncOperation();

		private ISyncWatermark backupCurMaxWatermark;

		private bool backupCurMaxWatermarkHasBeenSet;

		private Dictionary<ISyncItemId, ServerManifestEntry> backupCurServerManifest;

		private bool backupCurServerManifestHasBeenSet;

		private string backupCurFilterId;

		private bool backupCurFilterIdHasBeenSet;

		private ISyncWatermark backupCurSnapShotWatermark;

		private bool backupCurSnapShotWatermarkHasBeenSet;

		private Dictionary<ISyncItemId, ServerManifestEntry> backupCurDelayedServerOperationQueue;

		private bool backupCurDelayedServerOperationQueueHasBeenSet;

		private bool backupCurLastSyncConversationMode;

		private bool backupCurLastSyncConversationModeHasBeenSet;

		private bool currentConversationMode;

		private HashSet<ConversationId> currentConversationIdList;

		private HashSet<ConversationId> associatedAddConversationIdList;

		private HashSet<ConversationId> firstMessageInFilterConversationIdList;

		private bool fastReadFlagFilterCheck;

		internal delegate void StateModifier();

		internal enum ProcessServerOperationResult
		{
			AddedToManifest,
			UpdatedInManifest,
			RejectedAsReflection,
			RejectedAsOutOfFilter,
			RejectedAsRedundant
		}

		private enum HasRecoveryAddItemChangedResult
		{
			ItemChanged,
			ItemDidNotChange
		}

		private enum TryGetItemResult
		{
			NotFound,
			Success
		}

		public sealed class ClientStateInformation : ICustomSerializable
		{
			public static string InternMessageClass(string messageClassIn)
			{
				if (messageClassIn == "IPM.Note")
				{
					return "IPM.Note";
				}
				if (messageClassIn == "IPM.Appointment")
				{
					return "IPM.Appointment";
				}
				if (messageClassIn == "IPM.Schedule.Meeting.Request")
				{
					return "IPM.Schedule.Meeting.Request";
				}
				if (messageClassIn == "IPM.Schedule.Meeting.Canceled")
				{
					return "IPM.Schedule.Meeting.Canceled";
				}
				if (messageClassIn == "IPM.Note.SMIME.MultipartSigned")
				{
					return "IPM.Note.SMIME.MultipartSigned";
				}
				if (messageClassIn == "IPM.Schedule.Meeting.Resp.Pos")
				{
					return "IPM.Schedule.Meeting.Resp.Pos";
				}
				if (messageClassIn == "IPM.Note.Mobile.SMS")
				{
					return "IPM.Note.Mobile.SMS";
				}
				if (messageClassIn == "IPM.Contact")
				{
					return "IPM.Contact";
				}
				if (messageClassIn == "IPM.Task")
				{
					return "IPM.Task";
				}
				return messageClassIn;
			}

			public int?[] ChangeTrackingInformation
			{
				get
				{
					return this.changeTrackingInformation;
				}
				set
				{
					this.changeTrackingInformation = value;
				}
			}

			public bool ClientHasItem
			{
				get
				{
					return this.clientHasItem;
				}
				set
				{
					this.clientHasItem = value;
				}
			}

			public bool IsRejected
			{
				get
				{
					return this.rejected;
				}
				set
				{
					this.rejected = value;
				}
			}

			public ConversationId ConversationId
			{
				get
				{
					return this.conversationId;
				}
				set
				{
					this.conversationId = value;
				}
			}

			public ExDateTime? FilterDate
			{
				get
				{
					return this.filterDate;
				}
				set
				{
					this.filterDate = value;
				}
			}

			public string MessageClass
			{
				get
				{
					return this.messageClass;
				}
				set
				{
					this.messageClass = ((value == null) ? null : FolderSync.ClientStateInformation.InternMessageClass(value));
				}
			}

			public void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
			{
				this.clientHasItem = reader.ReadBoolean();
				ArrayData<NullableData<Int32Data, int>, int?> nullableInt32ArrayInstance = componentDataPool.GetNullableInt32ArrayInstance();
				nullableInt32ArrayInstance.DeserializeData(reader, componentDataPool);
				this.changeTrackingInformation = nullableInt32ArrayInstance.Data;
				this.IsRejected = reader.ReadBoolean();
				if (componentDataPool.InternalVersion > 0)
				{
					NullableDateTimeData nullableDateTimeDataInstance = componentDataPool.GetNullableDateTimeDataInstance();
					nullableDateTimeDataInstance.DeserializeData(reader, componentDataPool);
					this.filterDate = nullableDateTimeDataInstance.Data;
					StringData stringDataInstance = componentDataPool.GetStringDataInstance();
					stringDataInstance.DeserializeData(reader, componentDataPool);
					this.MessageClass = stringDataInstance.Data;
					ConversationIdData conversationIdDataInstance = componentDataPool.GetConversationIdDataInstance();
					conversationIdDataInstance.DeserializeData(reader, componentDataPool);
					this.ConversationId = conversationIdDataInstance.Data;
				}
			}

			public void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
			{
				writer.Write(this.clientHasItem);
				componentDataPool.GetNullableInt32ArrayInstance().Bind(this.changeTrackingInformation).SerializeData(writer, componentDataPool);
				writer.Write(this.IsRejected);
				componentDataPool.GetNullableDateTimeDataInstance().Bind(this.filterDate).SerializeData(writer, componentDataPool);
				componentDataPool.GetStringDataInstance().Bind(this.messageClass).SerializeData(writer, componentDataPool);
				componentDataPool.GetConversationIdDataInstance().Bind(this.conversationId).SerializeData(writer, componentDataPool);
			}

			private int?[] changeTrackingInformation;

			private bool clientHasItem;

			private bool rejected;

			private ConversationId conversationId;

			private ExDateTime? filterDate;

			private string messageClass;
		}

		protected class AcknowledgeModifications
		{
			public void Commit(Dictionary<ISyncItemId, FolderSync.ClientStateInformation> clientState)
			{
				foreach (FolderSync.AcknowledgeModifications.ClientStateModification clientStateModification in this.modifications)
				{
					switch (clientStateModification.Type)
					{
					case FolderSync.AcknowledgeModifications.ClientStateModificationType.RecordClientHasItem:
						FolderSync.AcknowledgeModifications.ApplyClientHasItem(clientState, clientStateModification);
						break;
					case FolderSync.AcknowledgeModifications.ClientStateModificationType.Remove:
						FolderSync.AcknowledgeModifications.ApplyRemove(clientState, clientStateModification);
						break;
					case FolderSync.AcknowledgeModifications.ClientStateModificationType.UpdateChangeTrackingInformation:
						FolderSync.AcknowledgeModifications.ApplyChangeTrackingInformation(clientState, clientStateModification);
						break;
					case FolderSync.AcknowledgeModifications.ClientStateModificationType.RecordRejectedItem:
						FolderSync.AcknowledgeModifications.ApplyClientReject(clientState, clientStateModification);
						break;
					default:
						throw new InvalidOperationException();
					}
				}
				this.modifications.Clear();
			}

			public void CommitPreviousState(out ISyncWatermark prevMaxWatermark, out Dictionary<ISyncItemId, ServerManifestEntry> prevServerManifest, out string prevFilterId, out Dictionary<ISyncItemId, ServerManifestEntry> prevDelayedServerOperationQueue, out ISyncWatermark prevSnapShotWatermark, out bool prevLastSyncConversationMode)
			{
				prevMaxWatermark = this.prevMaxWatermark;
				prevServerManifest = this.prevServerManifest;
				prevFilterId = this.prevFilterId;
				prevDelayedServerOperationQueue = this.prevDelayedServerOperationQueue;
				prevSnapShotWatermark = this.prevSnapShotWatermark;
				prevLastSyncConversationMode = this.prevLastSyncConversationMode;
			}

			public void RecordClientHasItem(ServerManifestEntry serverManifestEntry, bool clientHasItem, bool usingQueryBasedFilter)
			{
				FolderSync.AcknowledgeModifications.ClientStateModification clientStateModification = new FolderSync.AcknowledgeModifications.ClientStateModification(serverManifestEntry);
				clientStateModification.Type = FolderSync.AcknowledgeModifications.ClientStateModificationType.RecordClientHasItem;
				clientStateModification.ClientHasItem = clientHasItem;
				clientStateModification.UsingQueryBasedFilter = usingQueryBasedFilter;
				this.modifications.Add(clientStateModification);
			}

			public void RecordClientReject(ServerManifestEntry serverManifestEntry)
			{
				FolderSync.AcknowledgeModifications.ClientStateModification clientStateModification = new FolderSync.AcknowledgeModifications.ClientStateModification(serverManifestEntry);
				clientStateModification.Type = FolderSync.AcknowledgeModifications.ClientStateModificationType.RecordRejectedItem;
				clientStateModification.IsRejected = true;
				this.modifications.Add(clientStateModification);
			}

			public void RemoveFromClientState(ServerManifestEntry serverManifestEntry)
			{
				FolderSync.AcknowledgeModifications.ClientStateModification clientStateModification = new FolderSync.AcknowledgeModifications.ClientStateModification(serverManifestEntry);
				clientStateModification.Type = FolderSync.AcknowledgeModifications.ClientStateModificationType.Remove;
				this.modifications.Add(clientStateModification);
			}

			public void SavePreviousState(ISyncWatermark prevMaxWatermark, Dictionary<ISyncItemId, ServerManifestEntry> prevServerManifest, string prevFilterId, Dictionary<ISyncItemId, ServerManifestEntry> prevDelayedServerOperationQueue, ISyncWatermark prevSnapShotWatermark, bool prevLastSyncConversationMode)
			{
				this.prevMaxWatermark = prevMaxWatermark;
				this.prevServerManifest = prevServerManifest;
				this.prevFilterId = prevFilterId;
				this.prevDelayedServerOperationQueue = prevDelayedServerOperationQueue;
				this.prevSnapShotWatermark = prevSnapShotWatermark;
				this.prevLastSyncConversationMode = prevLastSyncConversationMode;
			}

			public void UpdateClientStateChangeTrackingInformation(ServerManifestEntry serverManifestEntry)
			{
				FolderSync.AcknowledgeModifications.ClientStateModification clientStateModification = new FolderSync.AcknowledgeModifications.ClientStateModification(serverManifestEntry);
				clientStateModification.Type = FolderSync.AcknowledgeModifications.ClientStateModificationType.UpdateChangeTrackingInformation;
				clientStateModification.ChangeTrackingInformation = serverManifestEntry.ChangeTrackingInformation;
				this.modifications.Add(clientStateModification);
			}

			private static void ApplyClientHasItem(Dictionary<ISyncItemId, FolderSync.ClientStateInformation> clientState, FolderSync.AcknowledgeModifications.ClientStateModification modification)
			{
				FolderSync.RecordClientHasItem(clientState, modification.Id, modification.ClientHasItem, modification.UsingQueryBasedFilter, modification.MessageClass, modification.FilterDate, modification.ConversationId);
			}

			private static void ApplyClientReject(Dictionary<ISyncItemId, FolderSync.ClientStateInformation> clientState, FolderSync.AcknowledgeModifications.ClientStateModification modification)
			{
				FolderSync.RecordClientReject(clientState, modification.Id, modification.IsRejected);
			}

			private static void ApplyRemove(Dictionary<ISyncItemId, FolderSync.ClientStateInformation> clientState, FolderSync.AcknowledgeModifications.ClientStateModification modification)
			{
				clientState.Remove(modification.Id);
			}

			private static void ApplyChangeTrackingInformation(Dictionary<ISyncItemId, FolderSync.ClientStateInformation> clientState, FolderSync.AcknowledgeModifications.ClientStateModification modification)
			{
				FolderSync.ClientStateInformation clientStateInformation = clientState[modification.Id];
				clientStateInformation.ChangeTrackingInformation = modification.ChangeTrackingInformation;
				clientStateInformation.MessageClass = modification.MessageClass;
				clientStateInformation.FilterDate = modification.FilterDate;
				clientStateInformation.ConversationId = modification.ConversationId;
			}

			private const int ExpectedModificationCount = 10;

			private List<FolderSync.AcknowledgeModifications.ClientStateModification> modifications = new List<FolderSync.AcknowledgeModifications.ClientStateModification>(10);

			private Dictionary<ISyncItemId, ServerManifestEntry> prevDelayedServerOperationQueue;

			private string prevFilterId;

			private ISyncWatermark prevMaxWatermark;

			private Dictionary<ISyncItemId, ServerManifestEntry> prevServerManifest;

			private ISyncWatermark prevSnapShotWatermark;

			private bool prevLastSyncConversationMode;

			private enum ClientStateModificationType
			{
				RecordClientHasItem,
				Remove,
				UpdateChangeTrackingInformation,
				RecordRejectedItem
			}

			private class ClientStateModification
			{
				public ClientStateModification(ServerManifestEntry entry)
				{
					this.id = entry.Id;
					this.filterDate = entry.FilterDate;
					this.messageClass = entry.MessageClass;
					this.conversationId = entry.ConversationId;
				}

				public int?[] ChangeTrackingInformation
				{
					get
					{
						return this.changeTrackingInformation;
					}
					set
					{
						this.changeTrackingInformation = value;
					}
				}

				public bool ClientHasItem
				{
					get
					{
						return this.clientHasItem;
					}
					set
					{
						this.clientHasItem = value;
					}
				}

				public ISyncItemId Id
				{
					get
					{
						return this.id;
					}
					set
					{
						this.id = value;
					}
				}

				public bool IsRejected
				{
					get
					{
						return this.isRejected;
					}
					set
					{
						this.isRejected = value;
					}
				}

				public FolderSync.AcknowledgeModifications.ClientStateModificationType Type
				{
					get
					{
						return this.type;
					}
					set
					{
						this.type = value;
					}
				}

				public bool UsingQueryBasedFilter
				{
					get
					{
						return this.usingQueryBasedFilter;
					}
					set
					{
						this.usingQueryBasedFilter = value;
					}
				}

				public ConversationId ConversationId
				{
					get
					{
						return this.conversationId;
					}
					set
					{
						this.conversationId = value;
					}
				}

				public ExDateTime? FilterDate
				{
					get
					{
						return this.filterDate;
					}
					set
					{
						this.filterDate = value;
					}
				}

				public string MessageClass
				{
					get
					{
						return this.messageClass;
					}
					set
					{
						this.messageClass = value;
					}
				}

				private int?[] changeTrackingInformation;

				private bool clientHasItem;

				private ISyncItemId id;

				private bool isRejected;

				private FolderSync.AcknowledgeModifications.ClientStateModificationType type;

				private bool usingQueryBasedFilter;

				private ConversationId conversationId;

				private ExDateTime? filterDate;

				private string messageClass;
			}
		}
	}
}
