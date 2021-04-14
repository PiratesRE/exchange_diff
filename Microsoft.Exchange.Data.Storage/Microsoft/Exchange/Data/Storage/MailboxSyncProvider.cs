using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxSyncProvider : ISyncProvider, IDisposeTrackable, IDisposable
	{
		public MailboxSyncProvider(Folder folder, bool trackReadFlagChanges, bool trackAssociatedMessageChanges, bool returnNewestFirst, bool trackConversations, bool allowTableRestrict, ISyncLogger syncLogger = null) : this(folder, trackReadFlagChanges, trackAssociatedMessageChanges, returnNewestFirst, trackConversations, allowTableRestrict, true, syncLogger)
		{
		}

		public MailboxSyncProvider(Folder folder, bool trackReadFlagChanges, bool trackAssociatedMessageChanges, bool returnNewestFirst, bool trackConversations, bool allowTableRestrict, bool disposeFolder, ISyncLogger syncLogger = null)
		{
			this.itemQueryOptimizationFilter = MailboxSyncProvider.falseFilterInstance;
			base..ctor();
			this.folder = folder;
			this.SyncLogger = (syncLogger ?? TracingLogger.Singleton);
			this.allowTableRestrict = allowTableRestrict;
			this.trackReadFlagChanges = trackReadFlagChanges;
			this.trackAssociatedMessageChanges = trackAssociatedMessageChanges;
			this.returnNewestChangesFirst = returnNewestFirst;
			this.trackConversations = trackConversations;
			this.UseSortOrder = true;
			this.disposeTracker = this.GetDisposeTracker();
			this.disposeFolder = disposeFolder;
		}

		protected MailboxSyncProvider()
		{
			this.itemQueryOptimizationFilter = MailboxSyncProvider.falseFilterInstance;
			base..ctor();
			StorageGlobals.TraceConstructIDisposable(this);
			this.disposeTracker = this.GetDisposeTracker();
		}

		public bool IsTrackingConversations
		{
			get
			{
				this.CheckDisposed("get_IsTrackingConversations");
				return this.trackConversations;
			}
		}

		public ISyncLogger SyncLogger { get; private set; }

		public QueryFilter ItemQueryOptimizationFilter
		{
			get
			{
				this.CheckDisposed("get_ItemQueryOptimizationFilter");
				return this.itemQueryOptimizationFilter;
			}
			set
			{
				this.CheckDisposed("set_ItemQueryOptimizationFilter");
				if (value != null && !(value is ComparisonFilter))
				{
					throw new InvalidOperationException();
				}
				this.itemQueryOptimizationFilter = value;
			}
		}

		public QueryFilter IcsPropertyGroupFilter
		{
			get
			{
				this.CheckDisposed("get_IcsPropertyGroupFilter");
				return this.icsPropertyGroupFilter;
			}
			set
			{
				this.CheckDisposed("set_IcsPropertyGroupFilter");
				this.icsPropertyGroupFilter = value;
			}
		}

		public bool UseSortOrder
		{
			get
			{
				this.CheckDisposed("get_UseSortOrder");
				return this.useSortOrder;
			}
			set
			{
				this.CheckDisposed("set_UseSortOrder");
				this.useSortOrder = value;
			}
		}

		internal static PropertyDefinition[] QueryColumns
		{
			get
			{
				return MailboxSyncProvider.queryColumns;
			}
		}

		internal FolderSync FolderSync
		{
			get
			{
				return this.folderSync;
			}
		}

		internal Folder Folder
		{
			get
			{
				return this.folder;
			}
		}

		public static MailboxSyncProvider Bind(Folder folder, bool trackReadFlagChanges, bool trackAssociatedMessageChanges, bool returnNewestChangesFirst, bool trackConversations, bool allowTableRestrict, ISyncLogger syncLogger = null)
		{
			return new MailboxSyncProvider(folder, trackReadFlagChanges, trackAssociatedMessageChanges, returnNewestChangesFirst, trackConversations, allowTableRestrict, syncLogger);
		}

		public static MailboxSyncProvider Bind(Folder folder, bool trackReadFlagChanges, bool trackAssociatedMessageChanges, bool returnNewestChangesFirst, bool trackConversations, bool allowTableRestrict, bool disposeFolder, ISyncLogger syncLogger = null)
		{
			return new MailboxSyncProvider(folder, trackReadFlagChanges, trackAssociatedMessageChanges, returnNewestChangesFirst, trackConversations, allowTableRestrict, disposeFolder, syncLogger);
		}

		private static bool IcsStateEquals(byte[] stateA, byte[] stateB)
		{
			if (stateA == null && stateB == null)
			{
				return true;
			}
			if (stateA == null || stateB == null)
			{
				return false;
			}
			if (object.ReferenceEquals(stateA, stateB))
			{
				return true;
			}
			if (stateA.Length != stateB.Length)
			{
				return false;
			}
			for (int i = 0; i < stateA.Length; i++)
			{
				if (stateA[i] != stateB[i])
				{
					return false;
				}
			}
			return true;
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MailboxSyncProvider>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void BindToFolderSync(FolderSync folderSync)
		{
			this.CheckDisposed("BindToFolderSync");
			this.folderSync = folderSync;
		}

		public virtual ISyncWatermark CreateNewWatermark()
		{
			this.CheckDisposed("CreateNewWatermark");
			return MailboxSyncWatermark.Create();
		}

		public virtual OperationResult DeleteItems(params ISyncItemId[] syncIds)
		{
			this.CheckDisposed("DeleteItems");
			StoreObjectId[] array = new StoreObjectId[syncIds.Length];
			for (int i = 0; i < syncIds.Length; i++)
			{
				array[i] = (StoreObjectId)syncIds[i].NativeId;
			}
			return this.folder.Session.Delete(DeleteItemFlags.SoftDelete, array).OperationResult;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void DisposeNewOperationsCachedData()
		{
			this.CheckDisposed("DisposeNewOperationsCachedData");
			if (this.syncQueryResult != null)
			{
				this.syncQueryResult.Dispose();
				this.syncQueryResult = null;
			}
		}

		public ISyncItem GetItem(ISyncItemId id, params PropertyDefinition[] specifiedPrefetchProperties)
		{
			this.CheckDisposed("GetItem");
			PropertyDefinition[] array;
			if (specifiedPrefetchProperties != null && specifiedPrefetchProperties.Length != 0)
			{
				array = new PropertyDefinition[specifiedPrefetchProperties.Length + MailboxSyncProvider.defaultPrefetchProperties.Length];
				specifiedPrefetchProperties.CopyTo(array, 0);
				MailboxSyncProvider.defaultPrefetchProperties.CopyTo(array, specifiedPrefetchProperties.Length);
			}
			else
			{
				array = MailboxSyncProvider.defaultPrefetchProperties;
			}
			return this.GetItem(this.BindToItemWithItemClass((StoreObjectId)id.NativeId, array));
		}

		public ISyncWatermark GetMaxItemWatermark(ISyncWatermark currentWatermark)
		{
			this.CheckDisposed("GetMaxItemWatermark");
			MailboxSyncWatermark mailboxSyncWatermark = MailboxSyncWatermark.Create();
			mailboxSyncWatermark.IcsState = this.CatchUpIcsState((MailboxSyncWatermark)currentWatermark);
			using (QueryResult queryResult = this.folder.ItemQuery(ItemQueryType.None, null, MailboxSyncProvider.sortByArticleIdDescending, MailboxSyncProvider.queryColumns))
			{
				object[][] rows = queryResult.GetRows(1);
				if (rows.Length != 0)
				{
					StoreObjectId objectId = ((VersionedId)rows[0][1]).ObjectId;
					mailboxSyncWatermark.ChangeNumber = (int)rows[0][0];
				}
			}
			return mailboxSyncWatermark;
		}

		public virtual bool GetNewOperations(ISyncWatermark minSyncWatermark, ISyncWatermark maxSyncWatermark, bool enumerateDeletes, int numOperations, QueryFilter filter, Dictionary<ISyncItemId, ServerManifestEntry> newServerManifest)
		{
			if (numOperations < 0 && numOperations != -1)
			{
				throw new ArgumentException("numOperations is not valid, value = " + numOperations);
			}
			this.CheckDisposed("GetNewOperations");
			MailboxSyncWatermark minWatermark = minSyncWatermark as MailboxSyncWatermark;
			MailboxSyncWatermark maxWatermark = maxSyncWatermark as MailboxSyncWatermark;
			if (filter != null)
			{
				this.SyncLogger.Information<int>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "MailboxSyncProvider.GetNewOperations. numOperations = {0} With filter", numOperations);
				return this.GetNewOperationsWithFilter(minWatermark, maxWatermark, numOperations, filter, newServerManifest);
			}
			this.SyncLogger.Information<int>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "MailboxSyncProvider.GetNewOperations. numOperations = {0} with ICS", numOperations);
			return this.IcsGetNewOperations(minWatermark, enumerateDeletes, numOperations, newServerManifest);
		}

		public List<IConversationTreeNode> GetInFolderItemsForConversation(ConversationId conversationId)
		{
			this.CheckDisposed("GetInFolderItemsForConversation");
			if (conversationId == null)
			{
				return null;
			}
			List<IConversationTreeNode> list = null;
			Conversation conversation = Conversation.Load((MailboxSession)this.folder.Session, conversationId, MailboxSyncProvider.conversationPrefetchProperties);
			if (conversation == null)
			{
				return list;
			}
			list = new List<IConversationTreeNode>(conversation.ConversationTree.Count);
			StoreObjectId objectId = this.folder.Id.ObjectId;
			foreach (IConversationTreeNode conversationTreeNode in conversation.ConversationTree)
			{
				StoreObjectId id = (StoreObjectId)conversationTreeNode.StorePropertyBags[0].TryGetProperty(StoreObjectSchema.ParentItemId);
				if (objectId.Equals(id))
				{
					list.Add(conversationTreeNode);
				}
			}
			return list;
		}

		public virtual ISyncItemId CreateISyncItemIdForNewItem(StoreObjectId itemId)
		{
			this.CheckDisposed("CreateISyncItemIdForNewItem");
			if (itemId == null)
			{
				throw new ArgumentNullException("itemId");
			}
			return MailboxSyncItemId.CreateForNewItem(itemId);
		}

		internal static ServerManifestEntry CreateItemDeleteManifestEntry(ISyncItemId syncItemId)
		{
			return new ServerManifestEntry(syncItemId)
			{
				ChangeType = ChangeType.Delete
			};
		}

		protected virtual ISyncItem GetItem(Item item)
		{
			return MailboxSyncItem.Bind(item);
		}

		protected void CheckDisposed(string methodName)
		{
			if (this.disposed)
			{
				StorageGlobals.TraceFailedCheckDisposed(this, methodName);
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		public ServerManifestEntry CreateItemChangeManifestEntry(ISyncItemId syncItemId, ISyncWatermark watermark)
		{
			return new ServerManifestEntry(syncItemId)
			{
				Watermark = watermark,
				ChangeType = ChangeType.Add
			};
		}

		internal ServerManifestEntry CreateReadFlagChangeManifestEntry(ISyncItemId syncItemId, bool read)
		{
			if (this.folderSync != null && !this.folderSync.ClientHasItem(syncItemId))
			{
				return null;
			}
			return new ServerManifestEntry(syncItemId)
			{
				ChangeType = ChangeType.ReadFlagChange,
				IsRead = read
			};
		}

		protected virtual void InternalDispose(bool disposing)
		{
			if (this.manifest != null)
			{
				this.manifest.Dispose();
				this.manifest = null;
			}
			if (disposing)
			{
				if (this.folder != null && this.disposeFolder)
				{
					this.folder.Dispose();
					this.folder = null;
				}
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
				}
			}
		}

		private Item BindToItemWithItemClass(StoreObjectId id, ICollection<PropertyDefinition> properties)
		{
			switch (id.ObjectType)
			{
			case StoreObjectType.Message:
				return MessageItem.Bind(this.folder.Session, id, properties);
			case StoreObjectType.MeetingRequest:
				return MeetingRequest.Bind(this.folder.Session, id, properties);
			case StoreObjectType.MeetingResponse:
				return MeetingResponse.Bind(this.folder.Session, id, properties);
			case StoreObjectType.MeetingCancellation:
				return MeetingCancellation.Bind(this.folder.Session, id, properties);
			case StoreObjectType.Contact:
				return Contact.Bind(this.folder.Session, id, properties);
			case StoreObjectType.DistributionList:
				return DistributionList.Bind(this.folder.Session, id, properties);
			case StoreObjectType.Task:
				return Task.Bind(this.folder.Session, id, true, properties);
			case StoreObjectType.Post:
				return PostItem.Bind(this.folder.Session, id, properties);
			case StoreObjectType.Report:
				return ReportMessage.Bind(this.folder.Session, id, properties);
			}
			return Item.Bind(this.folder.Session, id, properties);
		}

		private ManifestConfigFlags GetConfigFlags()
		{
			ManifestConfigFlags manifestConfigFlags = ManifestConfigFlags.Normal;
			if (this.returnNewestChangesFirst)
			{
				manifestConfigFlags |= ManifestConfigFlags.OrderByDeliveryTime;
			}
			if (!this.trackReadFlagChanges)
			{
				manifestConfigFlags |= ManifestConfigFlags.NoReadUnread;
			}
			if (this.trackAssociatedMessageChanges)
			{
				manifestConfigFlags |= ManifestConfigFlags.Associated;
			}
			if (this.trackConversations)
			{
				manifestConfigFlags |= ManifestConfigFlags.Conversations;
			}
			return manifestConfigFlags;
		}

		private byte[] CatchUpIcsState(MailboxSyncWatermark currentWatermark)
		{
			byte[] result = null;
			MapiFolder mapiFolder = this.folder.MapiFolder;
			MemoryStream memoryStream = null;
			MemoryStream memoryStream2 = null;
			try
			{
				if (currentWatermark.IcsState == null)
				{
					memoryStream = new MemoryStream();
				}
				else
				{
					memoryStream = new MemoryStream(currentWatermark.IcsState);
				}
				StoreSession session = this.folder.Session;
				bool flag = false;
				try
				{
					if (session != null)
					{
						session.BeginMapiCall();
						session.BeginServerHealthCall();
						flag = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					try
					{
						using (MapiManifest mapiManifest = mapiFolder.CreateExportManifest())
						{
							IcsCallback iMapiManifestCallback = new IcsCallback(this, null, 0, null);
							mapiManifest.Configure(this.GetConfigFlags() | ManifestConfigFlags.Catchup, null, memoryStream, iMapiManifestCallback, IcsCallback.PropTags);
							mapiManifest.Synchronize();
							memoryStream2 = new MemoryStream();
							mapiManifest.GetState(memoryStream2);
							result = memoryStream2.ToArray();
						}
					}
					catch (MapiExceptionCorruptData innerException)
					{
						throw new CorruptSyncStateException(ServerStrings.ExSyncStateCorrupted("ICS"), innerException);
					}
					catch (CorruptDataException innerException2)
					{
						throw new CorruptSyncStateException(ServerStrings.ExSyncStateCorrupted("ICS"), innerException2);
					}
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.ICSSynchronizationFailed, ex, session, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MailboxSyncProvider::CatchUpIcsState()", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.ICSSynchronizationFailed, ex2, session, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MailboxSyncProvider::CatchUpIcsState()", new object[0]),
						ex2
					});
				}
				finally
				{
					try
					{
						if (session != null)
						{
							session.EndMapiCall();
							if (flag)
							{
								session.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
			}
			finally
			{
				Util.DisposeIfPresent(memoryStream);
				Util.DisposeIfPresent(memoryStream2);
			}
			return result;
		}

		private void Dispose(bool disposing)
		{
			StorageGlobals.TraceDispose(this, this.disposed, disposing);
			if (!this.disposed)
			{
				this.disposed = true;
				this.InternalDispose(disposing);
			}
		}

		private bool GetNewOperationsWithFilter(MailboxSyncWatermark minWatermark, MailboxSyncWatermark maxWatermark, int numOperations, QueryFilter filter, Dictionary<ISyncItemId, ServerManifestEntry> newServerManifest)
		{
			this.SyncLogger.Information(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "MailboxSyncProvider.GetNewOperationsWithFilter. numOperations {0}, newServerManifest count {1}, minWatermark is New? {2}. Starting change number {3}", new object[]
			{
				numOperations,
				newServerManifest.Count,
				minWatermark.IsNew,
				minWatermark.ChangeNumber
			});
			bool result = false;
			ComparisonFilter comparisonFilter = null;
			if (!minWatermark.IsNew)
			{
				comparisonFilter = new ComparisonFilter(ComparisonOperator.GreaterThan, InternalSchema.ArticleId, minWatermark.RawChangeNumber);
			}
			if (this.syncQueryResult == null)
			{
				this.syncQueryResult = MailboxSyncQueryProcessor.ItemQuery(this.folder, ItemQueryType.None, filter, this.itemQueryOptimizationFilter, MailboxSyncProvider.sortByArticleIdAscending, MailboxSyncProvider.queryColumns, this.allowTableRestrict, this.UseSortOrder);
			}
			else if (comparisonFilter == null)
			{
				this.syncQueryResult.SeekToOffset(SeekReference.OriginBeginning, 0);
			}
			if (comparisonFilter != null)
			{
				this.syncQueryResult.SeekToCondition(SeekReference.OriginBeginning, comparisonFilter);
			}
			bool flag = false;
			while (!flag)
			{
				int num;
				if (numOperations == -1)
				{
					num = 10000;
				}
				else
				{
					int num2 = numOperations - newServerManifest.Count;
					num = num2 + 1;
				}
				if (num < 0)
				{
					throw new InvalidOperationException(ServerStrings.ExNumberOfRowsToFetchInvalid(num.ToString()));
				}
				object[][] rows = this.syncQueryResult.GetRows(num);
				flag = (this.syncQueryResult.CurrentRow == this.syncQueryResult.EstimatedRowCount);
				this.SyncLogger.Information<int, int, bool>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "MailboxSyncProvider.GetNewOperationsWithFilter. Requested {0} rows, Received {1} rows, All fetched? {2}", num, rows.Length, flag);
				for (int i = 0; i < rows.Length; i++)
				{
					try
					{
						StoreObjectId objectId = ((VersionedId)rows[i][1]).ObjectId;
						int changeNumber = (int)rows[i][0];
						MailboxSyncWatermark mailboxSyncWatermark = (MailboxSyncWatermark)this.CreateNewWatermark();
						mailboxSyncWatermark.UpdateWithChangeNumber(changeNumber, (bool)rows[i][2]);
						if (maxWatermark == null || maxWatermark.CompareTo(mailboxSyncWatermark) >= 0)
						{
							ISyncItemId syncItemId = this.CreateISyncItemIdForNewItem(objectId);
							ServerManifestEntry serverManifestEntry = this.CreateItemChangeManifestEntry(syncItemId, mailboxSyncWatermark);
							serverManifestEntry.ConversationId = (rows[i][3] as ConversationId);
							byte[] bytes = rows[i][4] as byte[];
							ConversationIndex index;
							if (ConversationIndex.TryCreate(bytes, out index) && index != ConversationIndex.Empty && index.Components != null && index.Components.Count == 1)
							{
								serverManifestEntry.FirstMessageInConversation = true;
							}
							if (rows[i][5] is ExDateTime)
							{
								serverManifestEntry.FilterDate = new ExDateTime?((ExDateTime)rows[i][5]);
							}
							serverManifestEntry.MessageClass = (rows[i][6] as string);
							if (numOperations != -1 && newServerManifest.Count >= numOperations)
							{
								result = true;
								goto IL_2B9;
							}
							newServerManifest[serverManifestEntry.Id] = serverManifestEntry;
							minWatermark.ChangeNumber = changeNumber;
						}
					}
					catch
					{
						throw;
					}
				}
			}
			IL_2B9:
			this.SyncLogger.Information<int>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "MailboxSyncProvider.GetNewOperationsWithFilter. Ending change number {0}", minWatermark.ChangeNumber);
			return result;
		}

		private bool IcsGetNewOperations(MailboxSyncWatermark minWatermark, bool enumerateDeletes, int numOperations, Dictionary<ISyncItemId, ServerManifestEntry> newServerManifest)
		{
			this.SyncLogger.Information<int, int>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "MailboxSyncProvider.IcsGetNewOperations.  NumOperations {0}, newServerManifest.Count {1}", numOperations, newServerManifest.Count);
			bool flag = false;
			MemoryStream memoryStream = null;
			try
			{
				StoreSession session = this.folder.Session;
				bool flag2 = false;
				try
				{
					if (session != null)
					{
						session.BeginMapiCall();
						session.BeginServerHealthCall();
						flag2 = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					try
					{
						flag = this.EnsureMapiManifest(minWatermark, numOperations, newServerManifest);
						if (flag)
						{
							this.SyncLogger.Information(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "MailboxSyncProvider.IcsGetNewOperations.  EnsureMapiManifest returned true (moreAvailable).  Bailing out.");
							return flag;
						}
						memoryStream = new MemoryStream();
						ManifestStatus manifestStatus = ManifestStatus.Yielded;
						while (manifestStatus == ManifestStatus.Yielded)
						{
							manifestStatus = this.manifest.Synchronize();
							switch (manifestStatus)
							{
							case ManifestStatus.Done:
							case ManifestStatus.Yielded:
								this.manifest.GetState(memoryStream);
								minWatermark.IcsState = memoryStream.ToArray();
								this.icsState = minWatermark.IcsState;
								break;
							case ManifestStatus.Stopped:
								this.extraServerManifestEntry = this.icsCallback.ExtraServerManiferEntry;
								this.SyncLogger.Information<string>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "MailboxSyncProvider::IcsGetNewOperations Got additional change, id = {0}", (this.extraServerManifestEntry == null) ? "NULL" : this.extraServerManifestEntry.Id.ToString());
								break;
							}
						}
						flag = this.icsCallback.MoreAvailable;
						this.SyncLogger.Information<bool>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "MailboxSyncProvider.IcsGetNewOperations.  More available after ICS sync? {0}", flag);
					}
					catch (MapiExceptionCorruptData innerException)
					{
						throw new CorruptSyncStateException(ServerStrings.ExSyncStateCorrupted("ICS"), innerException);
					}
					catch (CorruptDataException innerException2)
					{
						throw new CorruptSyncStateException(ServerStrings.ExSyncStateCorrupted("ICS"), innerException2);
					}
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.ICSSynchronizationFailed, ex, session, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MailboxSyncProvider::IcsGetNewOperations()", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.ICSSynchronizationFailed, ex2, session, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MailboxSyncProvider::IcsGetNewOperations()", new object[0]),
						ex2
					});
				}
				finally
				{
					try
					{
						if (session != null)
						{
							session.EndMapiCall();
							if (flag2)
							{
								session.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
			}
			finally
			{
				Util.DisposeIfPresent(memoryStream);
			}
			return flag;
		}

		private bool EnsureMapiManifest(MailboxSyncWatermark minWatermark, int numOperations, Dictionary<ISyncItemId, ServerManifestEntry> newServerManifest)
		{
			MemoryStream memoryStream = null;
			MapiFolder mapiFolder = this.folder.MapiFolder;
			if (this.manifest != null)
			{
				if (MailboxSyncProvider.IcsStateEquals(this.icsState, minWatermark.IcsState))
				{
					if (this.extraServerManifestEntry != null)
					{
						if (numOperations == 0)
						{
							return true;
						}
						newServerManifest.Add(this.extraServerManifestEntry.Id, this.extraServerManifestEntry);
						if (this.extraServerManifestEntry.Watermark != null)
						{
							minWatermark.ChangeNumber = ((MailboxSyncWatermark)this.extraServerManifestEntry.Watermark).ChangeNumber;
						}
						this.SyncLogger.Information<ISyncItemId>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "MailboxSyncProvider::EnsureMapiManifest Adding cached change, id = {0}", this.extraServerManifestEntry.Id);
						this.extraServerManifestEntry = null;
					}
					this.icsCallback.Bind(minWatermark, numOperations, newServerManifest);
					this.SyncLogger.Information<int, int>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "MailboxSyncProvider::EnsureMapiManifest Reusing ICS manifest, numOperations = {0}, newServerManifest.Count = {1}", numOperations, newServerManifest.Count);
					return false;
				}
				this.manifest.Dispose();
				this.manifest = null;
				this.SyncLogger.Information<int, int>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "MailboxSyncProvider::EnsureMapiManifest Tossed old ICS manifest, numOperations = {0}, newServerManifest.Count = {1}", numOperations, newServerManifest.Count);
			}
			try
			{
				memoryStream = ((minWatermark.IcsState == null) ? new MemoryStream() : new MemoryStream(minWatermark.IcsState));
				this.manifest = mapiFolder.CreateExportManifest();
				this.icsCallback = new IcsCallback(this, newServerManifest, numOperations, minWatermark);
				Restriction restriction = (this.icsPropertyGroupFilter == null) ? null : FilterRestrictionConverter.CreateRestriction(this.folder.Session, this.folder.Session.ExTimeZone, this.folder.MapiFolder, this.icsPropertyGroupFilter);
				this.manifest.Configure(this.GetConfigFlags(), restriction, memoryStream, this.icsCallback, IcsCallback.PropTags);
				this.SyncLogger.Information<int, int>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "MailboxSyncProvider::EnsureMapiManifest Created new ICS manifest, numOperations = {0}, newServerManifest.Count = {1}", numOperations, newServerManifest.Count);
			}
			finally
			{
				Util.DisposeIfPresent(memoryStream);
			}
			return false;
		}

		protected const int MaxNumOperations = 10240;

		private static readonly SortBy[] sortByArticleIdAscending = new SortBy[]
		{
			new SortBy(InternalSchema.ArticleId, SortOrder.Ascending)
		};

		private static readonly SortBy[] sortByArticleIdDescending = new SortBy[]
		{
			new SortBy(InternalSchema.ArticleId, SortOrder.Descending)
		};

		protected static readonly PropertyDefinition[] queryColumns = new PropertyDefinition[]
		{
			InternalSchema.ArticleId,
			InternalSchema.ItemId,
			MessageItemSchema.IsRead,
			ItemSchema.ConversationId,
			ItemSchema.ConversationIndex,
			ItemSchema.ReceivedTime,
			InternalSchema.ItemClass
		};

		private static readonly PropertyDefinition[] conversationPrefetchProperties = new PropertyDefinition[]
		{
			ItemSchema.Id,
			StoreObjectSchema.ParentItemId,
			ItemSchema.ConversationId,
			StoreObjectSchema.ItemClass
		};

		private static readonly PropertyDefinition[] defaultPrefetchProperties = new PropertyDefinition[]
		{
			InternalSchema.ArticleId
		};

		private static readonly QueryFilter falseFilterInstance = new FalseFilter();

		private readonly DisposeTracker disposeTracker;

		private readonly bool trackConversations;

		private readonly bool disposeFolder;

		private bool allowTableRestrict;

		protected Folder folder;

		protected bool trackAssociatedMessageChanges;

		private FolderSync folderSync;

		private bool disposed;

		private MailboxSyncQueryProcessor.IQueryResult syncQueryResult;

		private bool trackReadFlagChanges;

		protected bool returnNewestChangesFirst;

		private QueryFilter itemQueryOptimizationFilter;

		private QueryFilter icsPropertyGroupFilter;

		private MapiManifest manifest;

		private byte[] icsState;

		private ServerManifestEntry extraServerManifestEntry;

		private IcsCallback icsCallback;

		private bool useSortOrder;

		protected enum QueryColumnsEnum
		{
			ArticleId,
			Id,
			IsRead,
			ConversationId,
			ConversationIndex,
			ReceivedTime,
			ItemClass
		}
	}
}
