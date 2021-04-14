using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class SyncConversation : SingleStepServiceCommand<SyncConversationRequest, SyncConversationResponseMessage>
	{
		public SyncConversation(CallContext callContext, SyncConversationRequest request) : base(callContext, request)
		{
			OwsLogRegistry.Register(base.GetType().Name, typeof(SyncConversationMetadata), new Type[0]);
			this.responseShape = (this.GetResponseShape(request) as ConversationResponseShape);
		}

		private ResponseShape GetResponseShape(SyncConversationRequest request)
		{
			if (request.ShapeName != null || request.ConversationShape != null)
			{
				return Global.ResponseShapeResolver.GetResponseShape<ConversationResponseShape>(request.ShapeName, request.ConversationShape, base.CallContext.FeaturesManager);
			}
			return SyncConversation.defaultSyncConversationResponseShape;
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			this.responseMessage.Initialize(base.Result.Code, base.Result.Error);
			return this.responseMessage;
		}

		internal override ServiceResult<SyncConversationResponseMessage> Execute()
		{
			ExTraceGlobals.SyncConversationCallTracer.TraceDebug<string>((long)this.GetHashCode(), "SyncConversation.Execute: User '{0}'", base.CallContext.EffectiveCaller.PrimarySmtpAddress);
			Stopwatch stopwatch = new Stopwatch();
			Stopwatch stopwatch2 = new Stopwatch();
			Stopwatch stopwatch3 = new Stopwatch();
			Stopwatch stopwatch4 = new Stopwatch();
			Stopwatch stopwatch5 = new Stopwatch();
			stopwatch.Start();
			base.CallContext.UpdateLastSyncAttemptTime();
			this.TranslateFolderIds();
			this.SplitSyncStates(base.Request.SyncState);
			base.CallContext.ProtocolLog.Set(SyncConversationMetadata.FolderCount, base.Request.FolderIds.Length);
			if (base.Request.SyncState != null)
			{
				base.CallContext.ProtocolLog.Set(SyncConversationMetadata.SyncStateSize, base.Request.SyncState.Length);
				base.CallContext.ProtocolLog.Set(SyncConversationMetadata.SyncStateHash, base.Request.SyncState.GetHashCode());
			}
			this.responseMessage.IncludesLastItemInRange = true;
			try
			{
				stopwatch2.Start();
				this.DoIcsSyncs();
				stopwatch2.Stop();
				if (base.Request.DoQuickSync)
				{
					this.responseMessage.IncludesLastItemInRange = false;
				}
				else
				{
					stopwatch3.Start();
					this.DoQuerySyncs();
					stopwatch3.Stop();
				}
			}
			finally
			{
				foreach (SyncConversation.SyncStatePerFolder syncStatePerFolder in this.syncStates.Values)
				{
					if (syncStatePerFolder.Folder != null)
					{
						syncStatePerFolder.Folder.Dispose();
						syncStatePerFolder.Folder = null;
					}
				}
			}
			ConversationType[] array = new List<ConversationType>(this.conversations.Values).ToArray();
			if (array.Length > 0)
			{
				StoreId defaultFolderId = base.MailboxIdentityMailboxSession.GetDefaultFolderId(DefaultFolderType.Configuration);
				using (Folder folder = Folder.Bind(base.MailboxIdentityMailboxSession, defaultFolderId))
				{
					stopwatch4.Start();
					this.FetchReadFlagAndChangeKeys(folder);
					stopwatch4.Stop();
				}
				stopwatch5.Start();
				NormalQueryView.PrepareDraftItemIds(base.MailboxIdentityMailboxSession, array);
				stopwatch5.Stop();
			}
			this.responseMessage.Conversations = array;
			this.responseMessage.DeletedConversations = new List<DeletedConversationType>(this.deletedConversations.Values).ToArray();
			this.responseMessage.ConversationViewDataList = new List<ConversationViewDataType>(this.viewData.Values).ToArray();
			this.responseMessage.SyncState = this.JoinSyncStates();
			base.CallContext.UpdateLastSyncSuccessTime();
			stopwatch.Stop();
			base.CallContext.ProtocolLog.Set(SyncConversationMetadata.IcsTime, stopwatch2.ElapsedMilliseconds);
			base.CallContext.ProtocolLog.Set(SyncConversationMetadata.QueryTime, stopwatch3.ElapsedMilliseconds);
			base.CallContext.ProtocolLog.Set(SyncConversationMetadata.DraftTime, stopwatch5.ElapsedMilliseconds);
			base.CallContext.ProtocolLog.Set(SyncConversationMetadata.ReadFlagTime, stopwatch4.ElapsedMilliseconds);
			base.CallContext.ProtocolLog.Set(SyncConversationMetadata.TotalTime, stopwatch.ElapsedMilliseconds);
			base.CallContext.ProtocolLog.Set(SyncConversationMetadata.FolderCount, base.Request.FolderIds.Length);
			base.CallContext.ProtocolLog.Set(SyncConversationMetadata.ConversationCount, this.responseMessage.Conversations.Length);
			base.CallContext.ProtocolLog.Set(SyncConversationMetadata.DeletedConversationCount, this.responseMessage.DeletedConversations.Length);
			base.CallContext.ProtocolLog.Set(SyncConversationMetadata.IncludesLastItemInRange, this.responseMessage.IncludesLastItemInRange);
			ExTraceGlobals.SyncConversationCallTracer.TraceDebug<int, int, bool>((long)this.GetHashCode(), "SyncConversation.Execute: End, {0} conversations, {1} deleted, last = {2}", this.conversations.Count, this.deletedConversations.Count, this.responseMessage.IncludesLastItemInRange);
			return new ServiceResult<SyncConversationResponseMessage>(this.responseMessage);
		}

		private IdAndSession GetIdAndSessionFromFolderId(BaseFolderId folderId)
		{
			FolderId folderId2 = new FolderId(folderId.GetId(), IdConverter.BuildChangeKeyString(null, StoreObjectType.Folder));
			return base.IdConverter.ConvertFolderIdToIdAndSessionReadOnly(folderId2);
		}

		private void TranslateFolderIds()
		{
			for (int i = 0; i < base.Request.FolderIds.Length; i++)
			{
				if (base.Request.FolderIds[i] is DistinguishedFolderId)
				{
					ExTraceGlobals.SyncConversationCallTracer.TraceDebug<string>((long)this.GetHashCode(), "SyncConversation.TranslateFolderIds: Translating '{0}'", (base.Request.FolderIds[i] as DistinguishedFolderId).IdString);
					IdAndSession idAndSession = base.IdConverter.ConvertFolderIdToIdAndSessionReadOnly(base.Request.FolderIds[i]);
					using (Folder folder = Folder.Bind(idAndSession.Session, idAndSession.Id, null))
					{
						ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(folder.Id.ObjectId, idAndSession, null);
						FolderId folderId = new FolderId(concatenatedId.Id, concatenatedId.ChangeKey);
						base.Request.FolderIds[i] = folderId;
					}
				}
				BaseFolderId baseFolderId = base.Request.FolderIds[i];
				if (!this.requestFolderIds.ContainsKey(baseFolderId.GetId()))
				{
					this.requestFolderIds.Add(baseFolderId.GetId(), baseFolderId);
				}
			}
		}

		private void SplitSyncStates(string syncStateString)
		{
			ExTraceGlobals.SyncConversationCallTracer.TraceDebug<int>((long)this.GetHashCode(), "SyncConversation.SplitSyncStates: Length = {0}", (syncStateString == null) ? 0 : syncStateString.Length);
			if (string.IsNullOrEmpty(syncStateString))
			{
				return;
			}
			string[] array = syncStateString.Split(new char[]
			{
				','
			});
			if (array[0] != "SS5" || array.Length % 9 != 1)
			{
				ExTraceGlobals.SyncConversationCallTracer.TraceWarning((long)this.GetHashCode(), "SyncConversation.SplitSyncStates: Received invalid sync state format, gonna continue with no sync state (full sync)");
				return;
			}
			for (int i = 1; i < array.Length; i += 9)
			{
				string text = array[i];
				if (base.Request.IsPartialFolderList || this.requestFolderIds.ContainsKey(text))
				{
					SyncConversation.SyncStatePerFolder syncStatePerFolder = new SyncConversation.SyncStatePerFolder();
					syncStatePerFolder.FolderId = (this.requestFolderIds.ContainsKey(text) ? this.requestFolderIds[text] : new FolderId
					{
						Id = text
					});
					this.syncStates.Add(syncStatePerFolder.FolderId, syncStatePerFolder);
					if (!string.IsNullOrEmpty(array[i + 1]))
					{
						syncStatePerFolder.LastDeliveryTime = ExDateTime.ParseISO(ExTimeZone.UtcTimeZone, array[i + 1]);
					}
					if (!string.IsNullOrEmpty(array[i + 2]))
					{
						syncStatePerFolder.LastInstanceKey = Convert.FromBase64String(array[i + 2]);
					}
					if (!bool.TryParse(array[i + 3], out syncStatePerFolder.MoreItemsOnServer) || !int.TryParse(array[i + 4], out syncStatePerFolder.LastNumberOfDays) || !int.TryParse(array[i + 5], out syncStatePerFolder.LastMinimumCount) || !int.TryParse(array[i + 6], out syncStatePerFolder.LastMaximumCount) || !bool.TryParse(array[i + 7], out syncStatePerFolder.NeedQuerySync))
					{
						throw new InvalidSyncStateDataException();
					}
					syncStatePerFolder.IcsSyncState = array[i + 8];
				}
			}
		}

		private string JoinSyncStates()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SS5");
			foreach (SyncConversation.SyncStatePerFolder syncStatePerFolder in this.syncStates.Values)
			{
				stringBuilder.Append(',');
				stringBuilder.Append(syncStatePerFolder.FolderId.GetId());
				stringBuilder.Append(',');
				if (syncStatePerFolder.LastDeliveryTime != ExDateTime.MinValue)
				{
					stringBuilder.Append(syncStatePerFolder.LastDeliveryTime.ToISOString());
				}
				stringBuilder.Append(',');
				if (syncStatePerFolder.LastInstanceKey != null)
				{
					stringBuilder.Append(Convert.ToBase64String(syncStatePerFolder.LastInstanceKey));
				}
				stringBuilder.Append(',');
				stringBuilder.Append(syncStatePerFolder.MoreItemsOnServer.ToString());
				stringBuilder.Append(',');
				stringBuilder.Append(syncStatePerFolder.LastNumberOfDays.ToString());
				stringBuilder.Append(',');
				stringBuilder.Append(syncStatePerFolder.LastMinimumCount.ToString());
				stringBuilder.Append(',');
				stringBuilder.Append(syncStatePerFolder.LastMaximumCount.ToString());
				stringBuilder.Append(',');
				stringBuilder.Append(syncStatePerFolder.NeedQuerySync.ToString());
				stringBuilder.Append(',');
				if (syncStatePerFolder.IcsSyncState != null)
				{
					stringBuilder.Append(syncStatePerFolder.IcsSyncState);
				}
			}
			ExTraceGlobals.SyncConversationCallTracer.TraceDebug<int>((long)this.GetHashCode(), "SyncConversation.JoinSyncStates: Length = {0}", stringBuilder.Length);
			return stringBuilder.ToString();
		}

		private void AddFolderToResponse(SyncConversation.SyncStatePerFolder syncState, int totalConversationsInView)
		{
			ExDateTime exDateTime = syncState.LastDeliveryTime;
			exDateTime = EWSSettings.RequestTimeZone.ConvertDateTime(exDateTime);
			if (EWSSettings.DateTimePrecision == DateTimePrecision.Seconds)
			{
				exDateTime = exDateTime.AddMilliseconds((double)(-(double)exDateTime.Millisecond));
			}
			ConversationViewDataType conversationViewDataType = new ConversationViewDataType();
			conversationViewDataType.FolderId = (syncState.FolderId as FolderId);
			conversationViewDataType.TotalConversationsInView = totalConversationsInView;
			conversationViewDataType.OldestDeliveryTime = exDateTime.ToISOString();
			conversationViewDataType.MoreItemsOnServer = syncState.MoreItemsOnServer;
			this.viewData[syncState.FolderId] = conversationViewDataType;
		}

		private void DoIcsSyncs()
		{
			ExTraceGlobals.SyncConversationCallTracer.TraceDebug((long)this.GetHashCode(), "SyncConversation.DoIcsSyncs: Start");
			Stopwatch stopwatch = new Stopwatch();
			Stopwatch stopwatch2 = new Stopwatch();
			Stopwatch stopwatch3 = new Stopwatch();
			Stopwatch stopwatch4 = new Stopwatch();
			Stopwatch stopwatch5 = new Stopwatch();
			Stopwatch stopwatch6 = new Stopwatch();
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			foreach (BaseFolderId baseFolderId in base.Request.FolderIds)
			{
				if (this.conversations.Count + this.deletedConversations.Count >= base.Request.MaxChangesReturned)
				{
					break;
				}
				ExTraceGlobals.SyncConversationCallTracer.TraceDebug<string>((long)this.GetHashCode(), "SyncConversation.DoIcsSyncs: folderId = '{0}'", baseFolderId.GetId());
				SyncConversation.SyncStatePerFolder syncStatePerFolder;
				if (!this.syncStates.TryGetValue(baseFolderId, out syncStatePerFolder))
				{
					ExTraceGlobals.SyncConversationCallTracer.TraceDebug((long)this.GetHashCode(), "SyncConversation.DoIcsSyncs: Creating new sync state");
					syncStatePerFolder = new SyncConversation.SyncStatePerFolder();
					syncStatePerFolder.FolderId = baseFolderId;
					syncStatePerFolder.IcsSyncState = null;
					syncStatePerFolder.LastDeliveryTime = ExDateTime.UtcNow;
					syncStatePerFolder.LastInstanceKey = null;
					syncStatePerFolder.MoreItemsOnServer = true;
					syncStatePerFolder.NeedQuerySync = true;
					this.syncStates.Add(syncStatePerFolder.FolderId, syncStatePerFolder);
				}
				IdAndSession idAndSessionFromFolderId = this.GetIdAndSessionFromFolderId(baseFolderId);
				stopwatch.Start();
				Folder folder = this.TryBindToFolder(syncStatePerFolder);
				stopwatch.Stop();
				if (folder != null)
				{
					StoreObjectId asStoreObjectId = idAndSessionFromFolderId.GetAsStoreObjectId();
					MailboxSyncProviderFactory mailboxSyncProviderFactory = new MailboxSyncProviderFactory(idAndSessionFromFolderId.Session, asStoreObjectId);
					mailboxSyncProviderFactory.ReturnNewestChangesFirst();
					mailboxSyncProviderFactory.GenerateConversationChanges();
					using (MailboxSyncProvider mailboxSyncProvider = (MailboxSyncProvider)mailboxSyncProviderFactory.CreateSyncProvider(folder, null))
					{
						ServicesFolderSyncState servicesFolderSyncState = new ServicesFolderSyncState(asStoreObjectId, mailboxSyncProvider, syncStatePerFolder.IcsSyncState);
						ISyncWatermark syncWatermark = servicesFolderSyncState.Watermark;
						if (syncStatePerFolder.IcsSyncState == null)
						{
							ExTraceGlobals.SyncConversationCallTracer.TraceDebug((long)this.GetHashCode(), "SyncConversation.DoIcsSyncs: Requesting catch-up sync state from ICS");
							stopwatch3.Start();
							syncWatermark = mailboxSyncProvider.GetMaxItemWatermark(syncWatermark);
							stopwatch3.Stop();
							syncStatePerFolder.NeedQuerySync = true;
						}
						else
						{
							bool flag = true;
							while (flag && this.conversations.Count + this.deletedConversations.Count < base.Request.MaxChangesReturned)
							{
								int num5 = base.Request.MaxChangesReturned - (this.conversations.Count + this.deletedConversations.Count);
								ExTraceGlobals.SyncConversationCallTracer.TraceDebug<int>((long)this.GetHashCode(), "SyncConversation.DoIcsSyncs: Requesting {0} changes from ICS", num5);
								stopwatch2.Start();
								num++;
								Dictionary<ISyncItemId, ServerManifestEntry> dictionary = new Dictionary<ISyncItemId, ServerManifestEntry>();
								flag = mailboxSyncProvider.GetNewOperations(syncWatermark, null, true, num5, null, dictionary);
								stopwatch2.Stop();
								if (dictionary.Count > 0)
								{
									syncStatePerFolder.NeedQuerySync = true;
								}
								ExTraceGlobals.SyncConversationCallTracer.TraceDebug<int>((long)this.GetHashCode(), "SyncConversation.DoIcsSyncs: Received {0} entries from ICS", dictionary.Count);
								List<ConversationId> list = new List<ConversationId>();
								foreach (ServerManifestEntry serverManifestEntry in dictionary.Values)
								{
									ExTraceGlobals.SyncConversationCallTracer.TraceDebug<ChangeType, ConversationId>((long)this.GetHashCode(), "SyncConversation.DoIcsSyncs: {0} for conversation item {1}", serverManifestEntry.ChangeType, serverManifestEntry.ConversationId);
									if (serverManifestEntry.ConversationId == null)
									{
										ExTraceGlobals.SyncConversationCallTracer.TraceDebug((long)this.GetHashCode(), "SyncConversation.DoIcsSyncs: Received a null ConversationId from ICS");
									}
									else
									{
										switch (serverManifestEntry.ChangeType)
										{
										case ChangeType.Add:
										case ChangeType.Change:
											list.Add(serverManifestEntry.ConversationId);
											continue;
										case ChangeType.Delete:
										case ChangeType.SoftDelete:
										{
											ItemId convItemId = new ItemId(IdConverter.ConversationIdToEwsId(idAndSessionFromFolderId.Session.MailboxGuid, serverManifestEntry.ConversationId), null);
											this.AddDeletedConversation(baseFolderId as FolderId, convItemId);
											continue;
										}
										}
										ExTraceGlobals.SyncConversationCallTracer.TraceDebug<ChangeType>((long)this.GetHashCode(), "SyncConversation.DoIcsSyncs unknown/unexpected sync change type {0}", serverManifestEntry.ChangeType);
									}
								}
								if (list.Count > 0)
								{
									stopwatch4.Start();
									num2 += list.Count;
									this.FetchModifiedConversations(folder, idAndSessionFromFolderId, baseFolderId as FolderId, syncStatePerFolder, list, stopwatch5, stopwatch6, ref num3, ref num4);
									stopwatch4.Stop();
								}
							}
							if (flag)
							{
								ExTraceGlobals.SyncConversationCallTracer.TraceDebug((long)this.GetHashCode(), "SyncConversation.DoIcsSyncs: There are more ICS changes to enumerate on the next request");
								syncStatePerFolder.MoreItemsOnServer = true;
								this.responseMessage.IncludesLastItemInRange = false;
							}
						}
						servicesFolderSyncState.Watermark = syncWatermark;
						syncStatePerFolder.IcsSyncState = servicesFolderSyncState.SerializeAsBase64String();
					}
				}
			}
			base.CallContext.ProtocolLog.Set(SyncConversationMetadata.IcsBindTime, stopwatch.ElapsedMilliseconds);
			base.CallContext.ProtocolLog.Set(SyncConversationMetadata.ChangesTime, stopwatch2.ElapsedMilliseconds);
			base.CallContext.ProtocolLog.Set(SyncConversationMetadata.ChangesCallCount, num);
			base.CallContext.ProtocolLog.Set(SyncConversationMetadata.CatchUpTime, stopwatch3.ElapsedMilliseconds);
			base.CallContext.ProtocolLog.Set(SyncConversationMetadata.FetchTime, stopwatch4.ElapsedMilliseconds);
			base.CallContext.ProtocolLog.Set(SyncConversationMetadata.FetchCount, num2);
			base.CallContext.ProtocolLog.Set(SyncConversationMetadata.FetchQueryTime, stopwatch5.ElapsedMilliseconds);
			base.CallContext.ProtocolLog.Set(SyncConversationMetadata.FetchUnneededCount, num3);
			base.CallContext.ProtocolLog.Set(SyncConversationMetadata.LeftOverCount, num4);
			base.CallContext.ProtocolLog.Set(SyncConversationMetadata.LeftOverQueryTime, stopwatch6.ElapsedMilliseconds);
			ExTraceGlobals.SyncConversationCallTracer.TraceDebug((long)this.GetHashCode(), "SyncConversation.DoIcsSyncs: End");
		}

		private void FetchModifiedConversations(Folder folder, IdAndSession idAndSession, FolderId folderId, SyncConversation.SyncStatePerFolder syncState, List<ConversationId> modifiedConversations, Stopwatch queryTime, Stopwatch leftOverTime, ref int unneededRows, ref int leftOverCount)
		{
			HashSet<ConversationId> hashSet = new HashSet<ConversationId>(modifiedConversations);
			queryTime.Start();
			using (QueryResult queryResult = folder.ConversationItemQuery(null, new SortBy[]
			{
				new SortBy(ConversationItemSchema.ConversationLastDeliveryTime, SortOrder.Descending)
			}, this.GetFetchList(folder)))
			{
				IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(hashSet.Count + 10);
				queryTime.Stop();
				for (int i = 0; i < propertyBags.Length; i++)
				{
					ConversationId item = propertyBags[i].TryGetProperty(ConversationItemSchema.ConversationId) as ConversationId;
					if (hashSet.Contains(item))
					{
						this.AddRowBagToResponse(propertyBags[i], folder, idAndSession, folderId, syncState);
						hashSet.Remove(item);
						unneededRows += propertyBags.Length - i - 1;
					}
				}
			}
			if (hashSet.Count > 0)
			{
				leftOverCount += hashSet.Count;
				QueryFilter[] array = new QueryFilter[hashSet.Count];
				int num = 0;
				foreach (ConversationId propertyValue in hashSet)
				{
					array[num++] = new ComparisonFilter(ComparisonOperator.Equal, ConversationItemSchema.ConversationId, propertyValue);
				}
				leftOverTime.Start();
				using (QueryResult queryResult2 = folder.ConversationItemQuery(new OrFilter(array.ToArray<QueryFilter>()), new SortBy[]
				{
					new SortBy(ConversationItemSchema.ConversationId, SortOrder.Ascending)
				}, this.GetFetchList(folder)))
				{
					IStorePropertyBag[] propertyBags2 = queryResult2.GetPropertyBags(10000);
					leftOverTime.Stop();
					foreach (IStorePropertyBag rowBag in propertyBags2)
					{
						this.AddRowBagToResponse(rowBag, folder, idAndSession, folderId, syncState);
					}
				}
			}
		}

		private void AddRowBagToResponse(IStorePropertyBag rowBag, Folder folder, IdAndSession idAndSession, FolderId folderId, SyncConversation.SyncStatePerFolder syncState)
		{
			ConversationId conversationId = rowBag.TryGetProperty(ConversationItemSchema.ConversationId) as ConversationId;
			ItemId convItemId = new ItemId(IdConverter.ConversationIdToEwsId(idAndSession.Session.MailboxGuid, conversationId), null);
			ExDateTime t = ((ExDateTime)rowBag.TryGetProperty(ConversationItemSchema.ConversationLastDeliveryTime)).ToUtc();
			if (t >= syncState.LastDeliveryTime)
			{
				this.AddConversation(idAndSession, folder, folderId, convItemId, rowBag);
				return;
			}
			ExTraceGlobals.SyncConversationCallTracer.TraceDebug((long)this.GetHashCode(), "SyncConversation.DoIcsSyncs: Conversation is out of window");
			this.AddDeletedConversation(folderId, convItemId);
			syncState.MoreItemsOnServer = true;
		}

		private void DoQuerySyncs()
		{
			ExTraceGlobals.SyncConversationCallTracer.TraceDebug((long)this.GetHashCode(), "SyncConversation.DoQuerySyncs: Start");
			Stopwatch stopwatch = new Stopwatch();
			int num = (base.Request.MinimumCount > 0) ? base.Request.MinimumCount : 0;
			int num2 = (base.Request.MaximumCount > 0) ? base.Request.MaximumCount : int.MaxValue;
			ExDateTime exDateTime = (base.Request.NumberOfDays == 0) ? ExDateTime.MinValue : ExDateTime.UtcNow.AddDays((double)(-(double)base.Request.NumberOfDays));
			SortBy[] sortColumns = new SortBy[]
			{
				new SortBy(ConversationItemSchema.ConversationLastDeliveryTime, SortOrder.Descending)
			};
			foreach (SyncConversation.SyncStatePerFolder syncStatePerFolder in this.syncStates.Values)
			{
				if (!this.requestFolderIds.ContainsKey(syncStatePerFolder.FolderId.GetId()))
				{
					ExTraceGlobals.SyncConversationCallTracer.TraceDebug<string>((long)this.GetHashCode(), "SyncConversation.DoQuerySyncs: syncState.FolderId = '{0}' is not a part of the requested folders, hence not performing query sync.", syncStatePerFolder.FolderId.GetId());
				}
				else
				{
					if (base.Request.NumberOfDays != syncStatePerFolder.LastNumberOfDays || base.Request.MinimumCount != syncStatePerFolder.LastMinimumCount || base.Request.MaximumCount != syncStatePerFolder.LastMaximumCount)
					{
						syncStatePerFolder.NeedQuerySync = true;
						syncStatePerFolder.LastNumberOfDays = base.Request.NumberOfDays;
						syncStatePerFolder.LastMinimumCount = base.Request.MinimumCount;
						syncStatePerFolder.LastMaximumCount = base.Request.MaximumCount;
					}
					if (syncStatePerFolder.NeedQuerySync)
					{
						ExTraceGlobals.SyncConversationCallTracer.TraceDebug<string>((long)this.GetHashCode(), "SyncConversation.DoQuerySyncs: syncState.FolderId = '{0}'", syncStatePerFolder.FolderId.GetId());
						IdAndSession idAndSessionFromFolderId = this.GetIdAndSessionFromFolderId(syncStatePerFolder.FolderId);
						stopwatch.Start();
						Folder folder = this.TryBindToFolder(syncStatePerFolder);
						stopwatch.Stop();
						if (folder != null)
						{
							using (QueryResult queryResult = folder.ConversationItemQuery(null, sortColumns, this.GetFetchList(syncStatePerFolder.Folder)))
							{
								int estimatedRowCount = queryResult.EstimatedRowCount;
								int num3;
								ExDateTime t;
								this.SeekToLastRowAlreadyOnClient(syncStatePerFolder, queryResult, estimatedRowCount, out num3, out t);
								if (num3 < num || (num3 < num2 && t > exDateTime))
								{
									this.ReadMoreRows(idAndSessionFromFolderId, syncStatePerFolder, queryResult, num3, exDateTime, num, num2);
								}
								else if (num3 > num && (num3 > num2 || t < exDateTime))
								{
									this.TrimRowsFromClient(syncStatePerFolder, queryResult, estimatedRowCount, exDateTime, num, num2);
								}
								else
								{
									syncStatePerFolder.MoreItemsOnServer = (queryResult.CurrentRow < estimatedRowCount);
								}
								this.AddFolderToResponse(syncStatePerFolder, estimatedRowCount);
							}
						}
					}
				}
			}
			base.CallContext.ProtocolLog.Set(SyncConversationMetadata.QueryBindTime, stopwatch.ElapsedMilliseconds);
			ExTraceGlobals.SyncConversationCallTracer.TraceDebug((long)this.GetHashCode(), "SyncConversation.DoQuerySyncs: End");
		}

		private void SeekToLastRowAlreadyOnClient(SyncConversation.SyncStatePerFolder syncState, QueryResult queryResult, int totalConversationsInView, out int currentIndex, out ExDateTime currentTime)
		{
			currentIndex = -1;
			currentTime = ExDateTime.UtcNow;
			if (syncState.LastInstanceKey != null && totalConversationsInView > 0)
			{
				ExTraceGlobals.SyncConversationCallTracer.TraceDebug((long)this.GetHashCode(), "SyncConversation.SeekToLastRowAlreadyOnClient: Seeking to instance key");
				if (queryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.InstanceKey, syncState.LastInstanceKey)))
				{
					IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(1);
					if (propertyBags.Length > 0)
					{
						currentTime = ((ExDateTime)propertyBags[0].TryGetProperty(ConversationItemSchema.ConversationLastDeliveryTime)).ToUtc();
						if (ExDateTime.Compare(currentTime, syncState.LastDeliveryTime, TimeSpan.FromMilliseconds(1.0)) == 0)
						{
							currentIndex = queryResult.CurrentRow;
						}
						else
						{
							ExTraceGlobals.SyncConversationCallTracer.TraceDebug((long)this.GetHashCode(), "SyncConversation.SeekToLastRowAlreadyOnClient: The row was moved, we can't trust its location");
							syncState.LastInstanceKey = null;
							currentTime = ExDateTime.UtcNow;
						}
					}
				}
				else
				{
					ExTraceGlobals.SyncConversationCallTracer.TraceDebug((long)this.GetHashCode(), "SyncConversation.SeekToLastRowAlreadyOnClient: Could not find the row");
					syncState.LastInstanceKey = null;
				}
			}
			if (currentIndex == -1 && totalConversationsInView > 0)
			{
				ExTraceGlobals.SyncConversationCallTracer.TraceDebug((long)this.GetHashCode(), "SyncConversation.SeekToLastRowAlreadyOnClient: Seeking to last received time");
				currentTime = syncState.LastDeliveryTime.AddMilliseconds(1.0);
				if (queryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(ComparisonOperator.LessThanOrEqual, ConversationItemSchema.ConversationLastDeliveryTime, currentTime)))
				{
					ExTraceGlobals.SyncConversationCallTracer.TraceDebug((long)this.GetHashCode(), "SyncConversation.SeekToLastRowAlreadyOnClient: Found a row while seeking by LastDeliveryTime");
					currentIndex = queryResult.CurrentRow;
					return;
				}
				ExTraceGlobals.SyncConversationCallTracer.TraceDebug((long)this.GetHashCode(), "SyncConversation.SeekToLastRowAlreadyOnClient: Reached the bottom of the folder while seeking by LastDeliveryTime");
				currentIndex = totalConversationsInView;
				currentTime = syncState.LastDeliveryTime;
			}
		}

		private void ReadMoreRows(IdAndSession idAndSession, SyncConversation.SyncStatePerFolder syncState, QueryResult queryResult, int currentIndex, ExDateTime windowTime, int minIndex, int maxIndex)
		{
			bool flag = true;
			bool flag2 = true;
			while (flag2 && this.conversations.Count + this.deletedConversations.Count < base.Request.MaxChangesReturned)
			{
				int num = Math.Min(base.Request.MaxChangesReturned - (this.conversations.Count + this.deletedConversations.Count) + 1, 100);
				ExTraceGlobals.SyncConversationCallTracer.TraceDebug<int>((long)this.GetHashCode(), "SyncConversation.ReadMoreRows: Fetching {0} rows", num);
				IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(num);
				syncState.MoreItemsOnServer = (propertyBags.Length == num);
				if (propertyBags.Length == 0)
				{
					syncState.MoreItemsOnServer = false;
					flag = false;
					break;
				}
				int i = 0;
				while (i < propertyBags.Length)
				{
					ExDateTime exDateTime = ((ExDateTime)propertyBags[i].TryGetProperty(ConversationItemSchema.ConversationLastDeliveryTime)).ToUtc();
					if (this.conversations.Count + this.deletedConversations.Count >= base.Request.MaxChangesReturned)
					{
						syncState.MoreItemsOnServer = true;
						flag2 = false;
						break;
					}
					if (currentIndex >= minIndex && (currentIndex >= maxIndex || exDateTime < windowTime))
					{
						syncState.MoreItemsOnServer = true;
						syncState.NeedQuerySync = false;
						flag2 = false;
						flag = false;
						break;
					}
					syncState.LastInstanceKey = (byte[])propertyBags[i].TryGetProperty(ItemSchema.InstanceKey);
					syncState.LastDeliveryTime = exDateTime;
					ConversationId conversationId = (ConversationId)propertyBags[i].TryGetProperty(ConversationItemSchema.ConversationId);
					if (ExTraceGlobals.SyncConversationCallTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						object obj = propertyBags[i].TryGetProperty(ConversationItemSchema.ConversationTopic);
						ExTraceGlobals.SyncConversationCallTracer.TraceDebug<ConversationId, string>((long)this.GetHashCode(), "SyncConversation.ReadMoreRows: Conversation {0} '{1}'", conversationId, (obj is PropertyError) ? "null" : ((string)obj));
					}
					ItemId convItemId = new ItemId(IdConverter.ConversationIdToEwsId(idAndSession.Session.MailboxGuid, conversationId), null);
					this.AddConversation(idAndSession, syncState.Folder, syncState.FolderId as FolderId, convItemId, propertyBags[i]);
					i++;
					currentIndex++;
				}
			}
			if (flag)
			{
				this.responseMessage.IncludesLastItemInRange = false;
			}
		}

		private void TrimRowsFromClient(SyncConversation.SyncStatePerFolder syncState, QueryResult queryResult, int totalConversationsInView, ExDateTime windowTime, int minIndex, int maxIndex)
		{
			int num;
			if (queryResult.SeekToCondition(SeekReference.BackwardFromEnd, new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ConversationItemSchema.ConversationLastDeliveryTime, windowTime), SeekToConditionFlags.AllowExtendedSeekReferences))
			{
				num = queryResult.CurrentRow;
			}
			else
			{
				ExTraceGlobals.SyncConversationCallTracer.TraceDebug((long)this.GetHashCode(), "SyncConversation.TrimRowsFromClient: There are no items within the sync window");
				num = -1;
			}
			if (num + 1 < minIndex)
			{
				num = queryResult.SeekToOffset(SeekReference.OriginBeginning, minIndex - 1);
			}
			if (num + 1 > maxIndex)
			{
				num = queryResult.SeekToOffset(SeekReference.OriginBeginning, maxIndex - 1);
			}
			if (num == -1)
			{
				syncState.LastInstanceKey = null;
				syncState.LastDeliveryTime = ExDateTime.UtcNow;
				syncState.MoreItemsOnServer = (totalConversationsInView > 0);
				return;
			}
			IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(2);
			if (propertyBags.Length > 0)
			{
				syncState.LastInstanceKey = (byte[])propertyBags[0].TryGetProperty(ItemSchema.InstanceKey);
				syncState.LastDeliveryTime = ((ExDateTime)propertyBags[0].TryGetProperty(ConversationItemSchema.ConversationLastDeliveryTime)).ToUtc();
				syncState.MoreItemsOnServer = (propertyBags.Length > 1);
				return;
			}
			ExTraceGlobals.SyncConversationCallTracer.TraceError((long)this.GetHashCode(), "SyncConversation.TrimRowsFromClient: Unable to read the desired row after seeking to it. This should be very rare.");
		}

		private void AddDeletedConversation(FolderId folderId, ItemId convItemId)
		{
			string key = folderId.Id + "," + convItemId.Id;
			if (!this.deletedConversations.ContainsKey(key))
			{
				this.deletedConversations.Add(key, new DeletedConversationType(convItemId, folderId));
			}
		}

		private void AddConversation(IdAndSession idAndSession, Folder folder, FolderId folderId, ItemId convItemId, IStorePropertyBag rowBag)
		{
			string convKey = folderId.Id + "," + convItemId.Id;
			if (this.conversations.ContainsKey(convKey))
			{
				return;
			}
			ExTraceGlobals.SyncConversationCallTracer.TraceDebug((long)this.GetHashCode(), "SyncConversation.AddConversation: Adding conversation to response");
			try
			{
				GrayException.MapAndReportGrayExceptions(delegate()
				{
					this.deletedConversations.Remove(convKey);
					ConversationType conversationType = new ConversationType();
					conversationType.FolderId = folderId;
					conversationType.ConversationId = convItemId;
					Dictionary<PropertyDefinition, object> dictionary = new Dictionary<PropertyDefinition, object>();
					foreach (PropertyDefinition propertyDefinition in this.GetFetchList(folder))
					{
						object obj = rowBag.TryGetProperty(propertyDefinition);
						if (!(obj is PropertyError))
						{
							dictionary[propertyDefinition] = obj;
						}
					}
					StoreObjectType storeObjectType;
					ToServiceObjectForPropertyBagPropertyList toServiceObjectPropertyList = this.GetDeterminer(folder).GetToServiceObjectPropertyList(dictionary, out storeObjectType);
					toServiceObjectPropertyList.ConvertPropertiesToServiceObject(conversationType, dictionary, idAndSession);
					this.conversations.Add(convKey, conversationType);
				}, new GrayException.IsGrayExceptionDelegate(GrayException.IsSystemGrayException));
			}
			catch (LocalizedException ex)
			{
				ExTraceGlobals.SyncConversationCallTracer.TraceWarning<ItemId, LocalizedException>((long)this.GetHashCode(), "SyncConversation.AddConversation: Exception thrown while processing conversation {0}: {1}", convItemId, ex);
				base.CallContext.ProtocolLog.Set(SyncConversationMetadata.ExceptionConversationId, convItemId.Id);
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(base.CallContext.ProtocolLog, ex, "SyncConversation_Exception");
			}
		}

		private Folder TryBindToFolder(SyncConversation.SyncStatePerFolder syncState)
		{
			if (syncState.Folder == null)
			{
				IdAndSession idAndSessionFromFolderId = this.GetIdAndSessionFromFolderId(syncState.FolderId);
				try
				{
					syncState.Folder = Folder.Bind(idAndSessionFromFolderId.Session, idAndSessionFromFolderId.Id, null);
				}
				catch (ObjectNotFoundException arg)
				{
					ExTraceGlobals.SyncConversationCallTracer.TraceWarning<string, ObjectNotFoundException>((long)this.GetHashCode(), "SyncConversation.TryBindToFolder: Could not find the folder ID {0}. Exception: {1}", idAndSessionFromFolderId.Id.ToBase64String(), arg);
					syncState.Folder = null;
					this.syncStates.Remove(syncState.FolderId);
				}
			}
			return syncState.Folder;
		}

		private void FetchReadFlagAndChangeKeys(Folder rootFolder)
		{
			ExTraceGlobals.SyncConversationCallTracer.TraceDebug((long)this.GetHashCode(), "SyncConversation.FetchReadFlagAndChangeKeys: Start");
			try
			{
				GrayException.MapAndReportGrayExceptions(delegate()
				{
					Dictionary<string, QueryFilter> dictionary = new Dictionary<string, QueryFilter>();
					foreach (ConversationType conversationType in this.conversations.Values)
					{
						foreach (BaseItemId baseItemId in conversationType.GlobalItemIds)
						{
							if (!dictionary.ContainsKey(baseItemId.GetId()))
							{
								IdAndSession idAndSession = this.IdConverter.ConvertItemIdToIdAndSessionReadOnly(baseItemId);
								QueryFilter value = new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.Id, idAndSession.Id);
								dictionary.Add(baseItemId.GetId(), value);
							}
						}
					}
					ExTraceGlobals.SyncConversationCallTracer.TraceDebug<int>((long)this.GetHashCode(), "SyncConversation.FetchReadFlagAndChangeKeys: Filter contains {0} items", dictionary.Count);
					if (dictionary.Count > 0)
					{
						Dictionary<string, IStorePropertyBag> dictionary2 = new Dictionary<string, IStorePropertyBag>();
						MailboxSession mailboxIdentityMailboxSession = this.MailboxIdentityMailboxSession;
						PropertyDefinition[] dataColumns = new PropertyDefinition[]
						{
							ItemSchema.Id,
							StoreObjectSchema.ChangeKey,
							MessageItemSchema.IsRead
						};
						ExTraceGlobals.SyncConversationCallTracer.TraceDebug((long)this.GetHashCode(), "SyncConversation.FetchReadFlagAndChangeKeys: Binding to configuration folder");
						QueryFilter[] array = new QueryFilter[dictionary.Count];
						int num = 0;
						foreach (QueryFilter queryFilter in dictionary.Values)
						{
							array[num++] = queryFilter;
						}
						QueryFilter queryFilter2 = new OrFilter(array);
						ExTraceGlobals.SyncConversationCallTracer.TraceDebug((long)this.GetHashCode(), "SyncConversation.FetchReadFlagAndChangeKeys: Querying for rows");
						using (QueryResult queryResult = rootFolder.ItemQuery(ItemQueryType.DocumentIdView, queryFilter2, null, dataColumns))
						{
							if (queryResult != null)
							{
								ExTraceGlobals.SyncConversationCallTracer.TraceDebug((long)this.GetHashCode(), "SyncConversation.FetchReadFlagAndChangeKeys: Reading rows");
								IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(10000);
								ExTraceGlobals.SyncConversationCallTracer.TraceDebug<int>((long)this.GetHashCode(), "SyncConversation.FetchReadFlagAndChangeKeys: Read {0} rows", propertyBags.Length);
								foreach (IStorePropertyBag storePropertyBag in propertyBags)
								{
									VersionedId storeItemId = storePropertyBag[ItemSchema.Id] as VersionedId;
									BaseItemId baseItemId2 = IdConverter.ConvertStoreItemIdToItemId(storeItemId, this.MailboxIdentityMailboxSession);
									dictionary2[baseItemId2.GetId()] = storePropertyBag;
								}
							}
						}
						ExTraceGlobals.SyncConversationCallTracer.TraceDebug((long)this.GetHashCode(), "SyncConversation.FetchReadFlagAndChangeKeys: Updating conversations in response");
						foreach (ConversationType conversationType2 in this.conversations.Values)
						{
							conversationType2.GlobalItemChangeKeys = new string[conversationType2.GlobalItemIds.Length];
							conversationType2.GlobalItemReadFlags = new bool[conversationType2.GlobalItemIds.Length];
							for (int k = 0; k < conversationType2.GlobalItemIds.Length; k++)
							{
								BaseItemId baseItemId3 = conversationType2.GlobalItemIds[k];
								IStorePropertyBag storePropertyBag2 = null;
								if (dictionary2.TryGetValue(baseItemId3.GetId(), out storePropertyBag2))
								{
									byte[] inArray = storePropertyBag2[StoreObjectSchema.ChangeKey] as byte[];
									bool flag = (bool)storePropertyBag2[MessageItemSchema.IsRead];
									conversationType2.GlobalItemChangeKeys[k] = Convert.ToBase64String(inArray);
									conversationType2.GlobalItemReadFlags[k] = flag;
								}
							}
						}
					}
				}, new GrayException.IsGrayExceptionDelegate(GrayException.IsSystemGrayException));
			}
			catch (LocalizedException ex)
			{
				ExTraceGlobals.SyncConversationCallTracer.TraceWarning<LocalizedException>((long)this.GetHashCode(), "SyncConversation.FetchReadFlagAndChangeKeys: Exception thrown: {0}", ex);
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(base.CallContext.ProtocolLog, ex, "SyncConversation_FetchReadFlagAndChangeKeys");
			}
			ExTraceGlobals.SyncConversationCallTracer.TraceDebug((long)this.GetHashCode(), "SyncConversation.FetchReadFlagAndChangeKeys: End");
		}

		private PropertyListForViewRowDeterminer GetDeterminer(Folder folder)
		{
			if (this.UseInferenceShape(folder))
			{
				this.InitServiceCommandObjectsIfRequired(true);
				return this.determinerForInference;
			}
			this.InitServiceCommandObjectsIfRequired(false);
			return this.determiner;
		}

		private List<PropertyDefinition> GetFetchList(Folder folder)
		{
			if (this.UseInferenceShape(folder))
			{
				this.InitServiceCommandObjectsIfRequired(true);
				return this.fetchListForInference;
			}
			this.InitServiceCommandObjectsIfRequired(false);
			return this.fetchList;
		}

		private bool UseInferenceShape(Folder folder)
		{
			return this.responseShape.InferenceEnabled && ServiceCommandBase.IsDefaultFolderId(folder.Id.ObjectId, base.MailboxIdentityMailboxSession, DefaultFolderType.Inbox);
		}

		private void InitServiceCommandObjectsIfRequired(bool useInferenceShape)
		{
			if (!useInferenceShape && this.determiner == null)
			{
				this.determiner = PropertyListForViewRowDeterminer.BuildForConversation(this.responseShape);
				this.fetchList = new List<PropertyDefinition>(this.determiner.GetPropertiesToFetch());
				return;
			}
			if (useInferenceShape && this.determinerForInference == null)
			{
				List<PropertyPath> list = new List<PropertyPath>(this.responseShape.AdditionalProperties);
				list.Add(new PropertyUri(PropertyUriEnum.ConversationHasClutter));
				ConversationResponseShape conversationResponseShape = new ConversationResponseShape(this.responseShape.BaseShape, list.ToArray());
				this.determinerForInference = PropertyListForViewRowDeterminer.BuildForConversation(conversationResponseShape);
				this.fetchListForInference = new List<PropertyDefinition>(this.determinerForInference.GetPropertiesToFetch());
			}
		}

		private const string SyncStateVersionPrefix = "SS5";

		private const int RowBatchSize = 100;

		private static ConversationResponseShape defaultSyncConversationResponseShape = new ConversationResponseShape(ShapeEnum.Default, new PropertyPath[]
		{
			new PropertyUri(PropertyUriEnum.ConversationPreview),
			new PropertyUri(PropertyUriEnum.ConversationDraftItemIds)
		});

		private Dictionary<BaseFolderId, SyncConversation.SyncStatePerFolder> syncStates = new Dictionary<BaseFolderId, SyncConversation.SyncStatePerFolder>();

		private Dictionary<string, BaseFolderId> requestFolderIds = new Dictionary<string, BaseFolderId>();

		private SyncConversationResponseMessage responseMessage = new SyncConversationResponseMessage();

		private Dictionary<string, ConversationType> conversations = new Dictionary<string, ConversationType>();

		private Dictionary<string, DeletedConversationType> deletedConversations = new Dictionary<string, DeletedConversationType>();

		private Dictionary<BaseFolderId, ConversationViewDataType> viewData = new Dictionary<BaseFolderId, ConversationViewDataType>();

		private ConversationResponseShape responseShape;

		private PropertyListForViewRowDeterminer determiner;

		private List<PropertyDefinition> fetchList;

		private PropertyListForViewRowDeterminer determinerForInference;

		private List<PropertyDefinition> fetchListForInference;

		private class SyncStatePerFolder
		{
			public const int NumberOfFields = 9;

			public BaseFolderId FolderId;

			public string IcsSyncState;

			public ExDateTime LastDeliveryTime;

			public byte[] LastInstanceKey;

			public bool MoreItemsOnServer = true;

			public int LastNumberOfDays;

			public int LastMinimumCount;

			public int LastMaximumCount;

			public bool NeedQuerySync;

			public Folder Folder;
		}
	}
}
