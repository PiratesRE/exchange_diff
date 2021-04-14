using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;
using Microsoft.Exchange.Transport.Sync.Worker.Throttling;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class XSOSyncStorageProvider : NativeSyncStorageProvider
	{
		public override NativeSyncStorageProviderState Bind(SyncMailboxSession syncMailboxSession, ISyncWorkerData subscription, INativeStateStorage stateStorage, MailSubmitter mailSubmitter, SyncLogSession syncLogSession, bool underRecovery)
		{
			return new XSOSyncStorageProviderState(syncMailboxSession, subscription, stateStorage, syncLogSession, underRecovery);
		}

		public override void Unbind(NativeSyncStorageProviderState state)
		{
			XSOSyncStorageProviderState xsosyncStorageProviderState = (XSOSyncStorageProviderState)state;
			if (xsosyncStorageProviderState.ItemsToProcess != null)
			{
				while (xsosyncStorageProviderState.ItemsToProcess.Count > 0)
				{
					SyncChangeEntry syncChangeEntry = xsosyncStorageProviderState.ItemsToProcess.Dequeue();
					if (syncChangeEntry.SyncObject != null)
					{
						syncChangeEntry.SyncObject.Dispose();
						syncChangeEntry.SyncObject = null;
					}
				}
			}
		}

		public override IAsyncResult BeginApplyChanges(NativeSyncStorageProviderState state, IList<SyncChangeEntry> changeList, ISyncStorageProviderItemRetriever itemRetriever, object itemRetrieverState, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			Exception ex = null;
			XSOSyncStorageProviderState xsosyncStorageProviderState = (XSOSyncStorageProviderState)state;
			AsyncResult<XSOSyncStorageProviderState, SyncProviderResultData> asyncResult = new AsyncResult<XSOSyncStorageProviderState, SyncProviderResultData>(this, xsosyncStorageProviderState, callback, callbackState, syncPoisonContext);
			if (changeList == null || changeList.Count == 0)
			{
				xsosyncStorageProviderState.SyncLogSession.LogDebugging((TSLID)1113UL, XSOSyncStorageProvider.Tracer, "Empty Change List, so we're skipping applying changes.", new object[0]);
				asyncResult.SetCompletedSynchronously();
				asyncResult.ProcessCompleted(new SyncProviderResultData(changeList, false, false), ex);
			}
			else
			{
				xsosyncStorageProviderState.ItemRetriever = itemRetriever;
				xsosyncStorageProviderState.ItemRetrieverState = itemRetrieverState;
				xsosyncStorageProviderState.Changes = changeList;
				if (xsosyncStorageProviderState.ItemsToProcess == null)
				{
					xsosyncStorageProviderState.ItemsToProcess = new Queue<SyncChangeEntry>(changeList.Count);
					if (xsosyncStorageProviderState.UnderRecovery)
					{
						try
						{
							StoragePermanentException ex2;
							this.HandleItemsRecovery(asyncResult, xsosyncStorageProviderState, changeList, out ex2);
							if (ex2 != null)
							{
								xsosyncStorageProviderState.SyncLogSession.LogError((TSLID)1394UL, XSOSyncStorageProvider.Tracer, "Hit permanent exception to treat as transient in BeginApplyChanges for HandleItemsRecovery: {0}.", new object[]
								{
									ex2
								});
								ex = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, ex2);
							}
						}
						catch (StoragePermanentException ex3)
						{
							if (ExceptionUtilities.ShouldPermanentMapiOrXsoExceptionBeTreatedAsTransient(xsosyncStorageProviderState.SyncLogSession, ex3))
							{
								xsosyncStorageProviderState.SyncLogSession.LogError((TSLID)1395UL, XSOSyncStorageProvider.Tracer, "Hit permanent exception to be treated as transient in BeginApplyChanges for HandleItemsRecovery: {0}.", new object[]
								{
									ex3
								});
								ex = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, ex3);
							}
							else
							{
								xsosyncStorageProviderState.SyncLogSession.LogError((TSLID)1116UL, XSOSyncStorageProvider.Tracer, "Hit permanent exception in BeginApplyChanges for HandleItemsRecovery: {0}.", new object[]
								{
									ex3
								});
								xsosyncStorageProviderState.StateStorage.SetForceRecoverySyncNext(true);
								ex = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, ex3, true);
							}
						}
						catch (StorageTransientException ex4)
						{
							xsosyncStorageProviderState.SyncLogSession.LogError((TSLID)1117UL, XSOSyncStorageProvider.Tracer, "Hit transient exception in BeginApplyChanges for HandleItemsRecovery: {0}.", new object[]
							{
								ex4
							});
							ex = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, ex4);
						}
						catch (SyncStoreUnhealthyException ex5)
						{
							xsosyncStorageProviderState.SyncLogSession.LogError((TSLID)1355UL, XSOSyncStorageProvider.Tracer, "Hit Store Unhealthy exception in BeginApplyChanges for HandleItemsRecovery: {0}.", new object[]
							{
								ex5
							});
							ex = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, ex5, true);
						}
						if (ex != null)
						{
							asyncResult.SetCompletedSynchronously();
							asyncResult.ProcessCompleted(new SyncProviderResultData(changeList, false, false), ex);
							return asyncResult;
						}
					}
					foreach (SyncChangeEntry item in changeList)
					{
						xsosyncStorageProviderState.ItemsToProcess.Enqueue(item);
					}
				}
				ThreadPool.QueueUserWorkItem(asyncResult.GetWaitCallbackWithPoisonContext(new WaitCallback(this.BeginApplyItem)), asyncResult);
			}
			return asyncResult;
		}

		public override AsyncOperationResult<SyncProviderResultData> EndApplyChanges(IAsyncResult asyncResult)
		{
			AsyncResult<XSOSyncStorageProviderState, SyncProviderResultData> asyncResult2 = asyncResult as AsyncResult<XSOSyncStorageProviderState, SyncProviderResultData>;
			return asyncResult2.WaitForCompletion();
		}

		public override void Cancel(IAsyncResult asyncResult)
		{
			AsyncResult<XSOSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<XSOSyncStorageProviderState, SyncProviderResultData>)asyncResult;
			if (asyncResult2.State.TryCancel)
			{
				return;
			}
			asyncResult2.State.SetTryCancel();
			IAsyncResult pendingAsyncResult = asyncResult2.PendingAsyncResult;
			if (pendingAsyncResult != null && asyncResult2.State.ItemRetriever != null)
			{
				asyncResult2.State.ItemRetriever.CancelGetItem(pendingAsyncResult);
			}
		}

		protected override void UpdateMapping(NativeSyncStorageProviderState providerState, SyncChangeEntry change)
		{
			if (!change.Persist)
			{
				providerState.SyncLogSession.LogDebugging((TSLID)1228UL, XSOSyncStorageProvider.Tracer, "Skipping mapping table update operation as persist is false for: {0}.", new object[]
				{
					change
				});
				return;
			}
			if (change.Exception != null && (change.ChangeType != ChangeType.Delete || !(change.Exception is SyncPermanentException)))
			{
				providerState.SyncLogSession.LogDebugging((TSLID)1229UL, XSOSyncStorageProvider.Tracer, "Failed to apply change and so skipping mapping table update operation for: {0}.", new object[]
				{
					change
				});
				return;
			}
			switch (change.ChangeType)
			{
			case ChangeType.Add:
				providerState.SyncLogSession.LogDebugging((TSLID)1230UL, XSOSyncStorageProvider.Tracer, "Adding change to sync state: {0}.", new object[]
				{
					change
				});
				if (SchemaType.Folder == change.SchemaType)
				{
					if (providerState.UnderRecovery)
					{
						providerState.StateStorage.TryRemoveFolder(change.CloudId);
					}
					bool isInbox = providerState.IsInboxFolderId(change.NativeId);
					if (!providerState.StateStorage.TryAddFolder(isInbox, change.CloudId, change.CloudFolderId, change.NativeId, change.ChangeKey, change.NativeFolderId, change.CloudVersion, change.Properties))
					{
						providerState.SyncLogSession.LogError((TSLID)1231UL, XSOSyncStorageProvider.Tracer, "Could not map change as it conflicts with an existing mapping. This generally means a provider bug or malicious content.", new object[0]);
						change.Exception = SyncPermanentException.CreateItemLevelException(new SyncPoisonItemFoundException(change.ToString(), providerState.Subscription.SubscriptionGuid));
						return;
					}
				}
				else
				{
					if (providerState.UnderRecovery)
					{
						providerState.StateStorage.TryRemoveItem(change.CloudId);
					}
					if (!providerState.StateStorage.TryAddItem(change.CloudId, change.CloudFolderId, change.NativeId, change.ChangeKey, change.NativeFolderId, change.CloudVersion, change.Properties))
					{
						providerState.SyncLogSession.LogError((TSLID)1232UL, XSOSyncStorageProvider.Tracer, "Could not map change as it conflicts with an existing mapping. This generally means a provider bug or malicious content.", new object[0]);
						change.Exception = SyncPermanentException.CreateItemLevelException(new SyncPoisonItemFoundException(change.ToString(), providerState.Subscription.SubscriptionGuid));
						return;
					}
				}
				break;
			case ChangeType.Change:
			case ChangeType.ReadFlagChange:
			case ChangeType.SoftDelete:
				if (change.NewNativeFolderId != null)
				{
					if (SchemaType.Folder == change.SchemaType)
					{
						bool isInbox2 = providerState.IsInboxFolderId(change.NewNativeId ?? change.NativeId);
						if (!providerState.StateStorage.TryUpdateFolder(isInbox2, change.NativeId, change.NewNativeId, change.CloudId, change.NewCloudId, change.NewCloudFolderId, change.ChangeKey, change.NewNativeFolderId, change.CloudVersion, change.Properties))
						{
							providerState.SyncLogSession.LogError((TSLID)1238UL, XSOSyncStorageProvider.Tracer, "Could not map change as it conflicts with an existing mapping. This generally means a provider bug or malicious content.", new object[0]);
							change.Exception = SyncPermanentException.CreateItemLevelException(new SyncPoisonItemFoundException(change.ToString(), providerState.Subscription.SubscriptionGuid));
							return;
						}
					}
					else
					{
						providerState.SyncLogSession.LogDebugging((TSLID)1235UL, XSOSyncStorageProvider.Tracer, "Removing and then adding change in sync state as it's a folder move: {0}.", new object[]
						{
							change
						});
						providerState.StateStorage.TryRemoveItem(change.CloudId);
						if (!providerState.StateStorage.TryAddItem(change.CloudId, (change.NewCloudFolderId != null) ? change.NewCloudFolderId : change.CloudFolderId, (change.NewNativeId != null) ? change.NewNativeId : change.NativeId, change.ChangeKey, change.NewNativeFolderId, change.CloudVersion, change.Properties))
						{
							providerState.SyncLogSession.LogError((TSLID)1236UL, XSOSyncStorageProvider.Tracer, "Could not map change as it conflicts with an existing mapping. This generally means a provider bug or malicious content.", new object[0]);
							change.Exception = SyncPermanentException.CreateItemLevelException(new SyncPoisonItemFoundException(change.ToString(), providerState.Subscription.SubscriptionGuid));
							return;
						}
					}
				}
				else
				{
					providerState.SyncLogSession.LogDebugging((TSLID)1237UL, XSOSyncStorageProvider.Tracer, "Updating change in sync state: {0}.", new object[]
					{
						change
					});
					if (SchemaType.Folder == change.SchemaType)
					{
						bool isInbox3 = providerState.IsInboxFolderId(change.NewNativeId ?? change.NativeId);
						if (!providerState.StateStorage.TryUpdateFolder(isInbox3, change.NativeId, change.NewNativeId, change.CloudId, change.NewCloudId, null, change.ChangeKey, null, change.CloudVersion, change.Properties))
						{
							providerState.SyncLogSession.LogError((TSLID)1388UL, XSOSyncStorageProvider.Tracer, "Could not map change as it conflicts with an existing mapping. This generally means a provider bug or malicious content.", new object[0]);
							change.Exception = SyncPermanentException.CreateItemLevelException(new SyncPoisonItemFoundException(change.ToString(), providerState.Subscription.SubscriptionGuid));
							return;
						}
					}
					else
					{
						if (change.NewNativeId == null)
						{
							providerState.StateStorage.TryUpdateItem(change.NativeId, change.ChangeKey, change.CloudVersion, change.Properties);
							return;
						}
						providerState.StateStorage.TryRemoveItem(change.CloudId);
						if (!providerState.StateStorage.TryAddItem(change.CloudId, change.CloudFolderId, change.NewNativeId, change.ChangeKey, change.NativeFolderId, change.CloudVersion, change.Properties))
						{
							providerState.SyncLogSession.LogError((TSLID)1389UL, XSOSyncStorageProvider.Tracer, "Could not map change as it conflicts with an existing mapping. This generally means a provider bug or malicious content.", new object[0]);
							change.Exception = SyncPermanentException.CreateItemLevelException(new SyncPoisonItemFoundException(change.ToString(), providerState.Subscription.SubscriptionGuid));
							return;
						}
					}
				}
				break;
			case (ChangeType)3:
				break;
			case ChangeType.Delete:
				providerState.SyncLogSession.LogDebugging((TSLID)1239UL, XSOSyncStorageProvider.Tracer, "Removing change from sync state: {0}.", new object[]
				{
					change
				});
				if (SchemaType.Folder == change.SchemaType)
				{
					providerState.StateStorage.TryRemoveFolder(change.CloudId);
					return;
				}
				providerState.StateStorage.TryRemoveItem(change.CloudId);
				break;
			default:
				return;
			}
		}

		private static void CreateContact(XSOSyncStorageProviderState providerState, SyncChangeEntry change)
		{
			XSOSyncStorageProvider.<>c__DisplayClass1 CS$<>8__locals1 = new XSOSyncStorageProvider.<>c__DisplayClass1();
			CS$<>8__locals1.providerState = providerState;
			CS$<>8__locals1.change = change;
			StoreObjectId storeId = CS$<>8__locals1.change.NewNativeFolderId ?? CS$<>8__locals1.change.NativeFolderId;
			using (Contact contact = SyncStoreLoadManager.ContactCreate(CS$<>8__locals1.providerState.SyncMailboxSession.MailboxSession, storeId, new EventHandler<RoundtripCompleteEventArgs>(CS$<>8__locals1.providerState.OnRoundtripComplete)))
			{
				XSOSyncStorageProvider.PopulateItemProperties(CS$<>8__locals1.providerState, CS$<>8__locals1.change, contact);
				if (CS$<>8__locals1.providerState.Subscription.AggregationType == AggregationType.PeopleConnection)
				{
					XSOSyncStorageProvider.SaveAndReloadAfterLinking(CS$<>8__locals1.providerState, CS$<>8__locals1.change, contact, () => XSOSyncStorageProvider.TrySaveAndReloadNewlyCreatedContact(CS$<>8__locals1.providerState, CS$<>8__locals1.change, contact));
				}
				else
				{
					XSOSyncStorageProvider.TrySaveAndReloadNewlyCreatedContact(CS$<>8__locals1.providerState, CS$<>8__locals1.change, contact);
				}
				if (CS$<>8__locals1.change.NativeId != null)
				{
					CS$<>8__locals1.change.NewNativeId = contact.Id.ObjectId;
				}
				else
				{
					CS$<>8__locals1.change.NativeId = contact.Id.ObjectId;
				}
				CS$<>8__locals1.change.ChangeKey = contact.Id.ChangeKeyAsByteArray();
			}
		}

		private static void ResolveConflicts(SyncLogSession syncLogSession, Item item, ExDateTime itemLastModificationTimeAtNativeServer, ConflictResolutionResult conflictResolutionResult)
		{
			foreach (PropertyConflict propertyConflict in conflictResolutionResult.PropertyConflicts)
			{
				syncLogSession.LogDebugging((TSLID)1119UL, XSOSyncStorageProvider.Tracer, "Resolving Conflict:{0}", new object[]
				{
					propertyConflict
				});
				if (propertyConflict.ConflictResolvable)
				{
					if (propertyConflict.ResolvedValue == null || PropertyError.IsPropertyError(propertyConflict.ResolvedValue))
					{
						syncLogSession.LogDebugging((TSLID)1120UL, XSOSyncStorageProvider.Tracer, "Invalid Resolved Value: {0}, keeping item property value as is.", new object[]
						{
							propertyConflict.ResolvedValue
						});
					}
					else
					{
						item[propertyConflict.PropertyDefinition] = propertyConflict.ResolvedValue;
					}
				}
				else if (item.LastModifiedTime > itemLastModificationTimeAtNativeServer)
				{
					syncLogSession.LogDebugging((TSLID)1121UL, XSOSyncStorageProvider.Tracer, "Applying Irresolvable Change with ClientValue", new object[0]);
					if (propertyConflict.ClientValue == null || PropertyError.IsPropertyError(propertyConflict.ClientValue))
					{
						syncLogSession.LogDebugging((TSLID)1122UL, XSOSyncStorageProvider.Tracer, "Invalid ClientValue: {0}, keeping item property value as is.", new object[]
						{
							propertyConflict.ClientValue
						});
					}
					else
					{
						item[propertyConflict.PropertyDefinition] = propertyConflict.ClientValue;
					}
				}
				else
				{
					syncLogSession.LogDebugging((TSLID)1123UL, XSOSyncStorageProvider.Tracer, "Applying Irresolvable Change with ServerValue", new object[0]);
					if (propertyConflict.ServerValue == null || PropertyError.IsPropertyError(propertyConflict.ServerValue))
					{
						syncLogSession.LogDebugging((TSLID)1124UL, XSOSyncStorageProvider.Tracer, "Invalid ServerValue: {0}, keeping item property value as is.", new object[]
						{
							propertyConflict.ServerValue
						});
					}
					else
					{
						item[propertyConflict.PropertyDefinition] = propertyConflict.ServerValue;
					}
				}
			}
		}

		private static void PopulateItemProperties(XSOSyncStorageProviderState providerState, SyncChangeEntry change, Item newItem)
		{
			if (change.ChangeType == ChangeType.ReadFlagChange)
			{
				SchemaType schemaType = change.SchemaType;
			}
			switch (change.SchemaType)
			{
			case SchemaType.Email:
				XSOSyncStorageProvider.PopulateMessageProperties(providerState, change.ChangeType, (ISyncEmail)change.SyncObject, newItem);
				break;
			case SchemaType.Contact:
				XSOSyncStorageProvider.PopulateContactProperties(providerState, change.ChangeType, (ISyncContact)change.SyncObject, newItem);
				break;
			default:
				throw new NotImplementedException();
			}
			newItem[MessageItemSchema.SharingInstanceGuid] = providerState.Subscription.SubscriptionGuid;
			newItem[ItemSchema.CloudId] = change.CloudId;
			if (change.SyncObject.LastModifiedTime != null)
			{
				newItem[StoreObjectSchema.LastModifiedTime] = change.SyncObject.LastModifiedTime.Value;
			}
			if (change.CloudVersion != null)
			{
				newItem[ItemSchema.CloudVersion] = change.CloudVersion;
			}
		}

		private static void PopulateMessageProperties(XSOSyncStorageProviderState providerState, ChangeType changeType, ISyncEmail syncEmail, Item newItem)
		{
			if (syncEmail.IsRead != null)
			{
				newItem[MessageItemSchema.IsRead] = syncEmail.IsRead.Value;
			}
			else if (changeType == ChangeType.ReadFlagChange)
			{
				throw new InvalidOperationException("'IsRead' must have a value in the sync email for ReadFlagChange.");
			}
			if (changeType == ChangeType.ReadFlagChange)
			{
				return;
			}
			if (syncEmail.MimeStream != null)
			{
				providerState.SyncLogSession.LogDebugging((TSLID)1125UL, XSOSyncStorageProvider.Tracer, "Parsing MIME stream to Message Item.", new object[0]);
				XSOSyncContentConversion.ConvertAnyMimeToItem(syncEmail.MimeStream, newItem, providerState.ScopedInboundConversionOptions);
			}
			if (syncEmail.SyncMessageResponseType != null)
			{
				XSOSyncStorageProvider.UpdateIconIndex(newItem, syncEmail.SyncMessageResponseType.Value, providerState.SyncLogSession);
			}
			if (syncEmail.Importance != null)
			{
				newItem[ItemSchema.Importance] = syncEmail.Importance.Value;
			}
			if (syncEmail.Sensitivity != null)
			{
				newItem[ItemSchema.Sensitivity] = syncEmail.Sensitivity.Value;
			}
			if (syncEmail.IsDraft != null && syncEmail.IsDraft.Value)
			{
				newItem.ClassName = "IPM.Note";
				newItem[MessageItemSchema.IsDraft] = true;
			}
			else
			{
				if (syncEmail.MessageClass != null)
				{
					newItem.ClassName = syncEmail.MessageClass;
				}
				newItem[MessageItemSchema.IsDraft] = false;
			}
			newItem[ItemSchema.SpamConfidenceLevel] = -1;
			if (syncEmail.ReceivedTime != null)
			{
				newItem[ItemSchema.ReceivedTime] = syncEmail.ReceivedTime.Value.ToUtc();
			}
			else if (changeType == ChangeType.Add)
			{
				newItem[ItemSchema.ReceivedTime] = ExDateTime.UtcNow;
			}
			if (!string.IsNullOrEmpty(syncEmail.InternetMessageId))
			{
				newItem[ItemSchema.InternetMessageId] = syncEmail.InternetMessageId;
			}
		}

		private static void UpdateIconIndex(Item newItem, SyncMessageResponseType syncMessageResponseType, SyncLogSession syncLogSession)
		{
			switch (syncMessageResponseType)
			{
			case SyncMessageResponseType.None:
			{
				IconIndex iconIndex = SyncUtilities.SafeGetProperty<IconIndex>(newItem, ItemSchema.IconIndex);
				if (iconIndex != IconIndex.MailReplied && iconIndex != IconIndex.MailForwarded)
				{
					syncLogSession.LogDebugging((TSLID)1127UL, XSOSyncStorageProvider.Tracer, "Not modifying iconIndex because previous icon index was not plain Replied/Forwarded but was {0}.  New SyncMessageResponseType is None.", new object[]
					{
						iconIndex
					});
					return;
				}
				newItem[ItemSchema.IconIndex] = IconIndex.BaseMail;
				return;
			}
			case SyncMessageResponseType.Replied:
				newItem[ItemSchema.IconIndex] = IconIndex.MailReplied;
				return;
			case SyncMessageResponseType.Forwarded:
				newItem[ItemSchema.IconIndex] = IconIndex.MailForwarded;
				return;
			default:
				throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture, "Unknown Response Type: {0}", new object[]
				{
					syncMessageResponseType
				}));
			}
		}

		private static void PopulateContactProperties(XSOSyncStorageProviderState providerState, ChangeType changeType, ISyncContact syncContact, Item contact)
		{
			XSOSyncContact.CopyPropertiesFromISyncContact(contact, syncContact);
		}

		private static bool ShouldCancel<TState, TResult>(AsyncResult<TState, TResult> asyncResult, XSOSyncStorageProviderState providerState) where TState : class where TResult : class
		{
			if (providerState.TryCancel)
			{
				asyncResult.ProcessCanceled();
				return true;
			}
			return false;
		}

		private static bool TryFindItem(XSOSyncStorageProviderState providerState, SyncChangeEntry change, out VersionedId itemId)
		{
			itemId = null;
			using (Folder folder = SyncStoreLoadManager.FolderBind(providerState.SyncMailboxSession.MailboxSession, change.NativeFolderId, null, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete)))
			{
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.CloudId, change.CloudId);
				SortBy[] sort = new SortBy[1];
				sort[0] = new SortBy(ItemSchema.CloudId, SortOrder.Ascending);
				QueryResult queryResult = null;
				SyncStoreLoadManager.ThrottleAndExecuteStoreCall(providerState.SyncMailboxSession.MailboxSession, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete), "ItemQuery", delegate
				{
					queryResult = folder.ItemQuery(ItemQueryType.None, null, sort, XSOSyncStorageProvider.RecoveryItemPropertiesStartingWithCloudId);
				});
				using (queryResult)
				{
					bool rowsExist = false;
					SyncStoreLoadManager.ThrottleAndExecuteStoreCall(providerState.SyncMailboxSession.MailboxSession, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete), "SeekToCondition", delegate
					{
						rowsExist = queryResult.SeekToCondition(SeekReference.OriginBeginning, filter);
					});
					if (!rowsExist)
					{
						providerState.SyncLogSession.LogVerbose((TSLID)1075UL, XSOSyncStorageProvider.Tracer, "Search for item {0} failed.", new object[]
						{
							change
						});
						return false;
					}
					object[][] partialResults = null;
					SyncStoreLoadManager.ThrottleAndExecuteStoreCall(providerState.SyncMailboxSession.MailboxSession, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete), "GetRows", delegate
					{
						partialResults = queryResult.GetRows(1);
					});
					if (partialResults.Length != 1)
					{
						providerState.SyncLogSession.LogVerbose((TSLID)1129UL, XSOSyncStorageProvider.Tracer, "Search for item {0} failed.", new object[]
						{
							change
						});
						return false;
					}
					itemId = (VersionedId)partialResults[0][1];
				}
			}
			providerState.SyncLogSession.LogDebugging((TSLID)1152UL, XSOSyncStorageProvider.Tracer, "Search for item {0} passed. Found new id: {1}.", new object[]
			{
				change,
				itemId
			});
			return true;
		}

		private static bool SaveAndReloadAfterLinking(XSOSyncStorageProviderState providerState, SyncChangeEntry change, Contact contact, Func<bool> saveAndReload)
		{
			SyncStoreLoadManager.Link(providerState.SyncMailboxSession.MailboxSession, providerState.BulkAutomaticLink, contact, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete));
			bool flag = SyncUtilities.SafeGetProperty<bool>(contact, ContactSchema.Linked);
			providerState.SyncLogSession.LogDebugging((TSLID)1438UL, XSOSyncStorageProvider.Tracer, "Contact {0} has been linked: {1}", new object[]
			{
				change,
				flag
			});
			bool flag2 = saveAndReload();
			if (flag2)
			{
				SyncStoreLoadManager.NotifyContactSaved(providerState.SyncMailboxSession.MailboxSession, providerState.BulkAutomaticLink, contact, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete));
			}
			return flag2;
		}

		private static bool TrySaveAndReloadNewlyCreatedContact(XSOSyncStorageProviderState providerState, SyncChangeEntry change, Contact contact)
		{
			SyncStoreLoadManager.ContactSave(contact, providerState.SyncMailboxSession.MailboxSession, SaveMode.ResolveConflicts, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete));
			providerState.SyncLogSession.LogDebugging((TSLID)1118UL, XSOSyncStorageProvider.Tracer, "Added Contact: {0}", new object[]
			{
				change
			});
			SyncStoreLoadManager.ContactLoad(contact, providerState.SyncMailboxSession.MailboxSession, XSOSyncStorageProvider.ClientOperationProperties, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete));
			return true;
		}

		private static bool TrySaveAndReloadExistingItem(XSOSyncStorageProviderState providerState, SyncChangeEntry change, Item item, StoreObjectId itemId, StoreObjectId folderId, ExDateTime itemLastModificationTimeAtNativeServer)
		{
			ConflictResolutionResult conflictResolutionResult = SyncStoreLoadManager.ItemSave(item, providerState.SyncMailboxSession.MailboxSession, SaveMode.ResolveConflicts, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete));
			if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
			{
				SyncStoreLoadManager.ItemLoad(item, providerState.SyncMailboxSession.MailboxSession, null, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete));
				XSOSyncStorageProvider.PopulateItemProperties(providerState, change, item);
				providerState.SyncLogSession.LogDebugging((TSLID)1135UL, XSOSyncStorageProvider.Tracer, "IrresolvableConflict detected for change: {0}, having Item Message Class: {1}", new object[]
				{
					change,
					item.ClassName
				});
				XSOSyncStorageProvider.ResolveConflicts(providerState.SyncLogSession, item, itemLastModificationTimeAtNativeServer, conflictResolutionResult);
				SyncStoreLoadManager.ItemSave(item, providerState.SyncMailboxSession.MailboxSession, SaveMode.NoConflictResolution, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete));
			}
			if (change.ChangeType == ChangeType.ReadFlagChange)
			{
				providerState.SyncLogSession.LogDebugging((TSLID)1136UL, XSOSyncStorageProvider.Tracer, "Changed read-flag state for Message: {0}", new object[]
				{
					change
				});
			}
			else
			{
				providerState.SyncLogSession.LogDebugging((TSLID)1137UL, XSOSyncStorageProvider.Tracer, "Applied change for Message: {0}", new object[]
				{
					change
				});
			}
			if (conflictResolutionResult.SaveStatus == SaveResult.Success)
			{
				SyncStoreLoadManager.ItemLoad(item, providerState.SyncMailboxSession.MailboxSession, XSOSyncStorageProvider.ClientOperationProperties, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete));
				change.ChangeKey = item.Id.ChangeKeyAsByteArray();
				return true;
			}
			providerState.SyncLogSession.LogDebugging((TSLID)1138UL, XSOSyncStorageProvider.Tracer, "Skipping RecordNativeItemOperation and UpdateChangeKey for change: {0} so that resolved change can propagate to cloud provider.", new object[]
			{
				change
			});
			return false;
		}

		private bool TryFindFolder(XSOSyncStorageProviderState providerState, SyncChangeEntry change, out VersionedId folderId)
		{
			folderId = null;
			SyncChangeEntry syncChangeEntry = new SyncChangeEntry(ChangeType.Change, SchemaType.Folder, StoreObjectId.DummyId);
			syncChangeEntry.CloudFolderId = change.CloudId;
			syncChangeEntry.NativeFolderId = change.NativeId;
			if (base.TryEnsureFullParentFolderHierarchy(providerState, syncChangeEntry))
			{
				string text;
				StoreObjectId storeObjectId;
				byte[] changeKey;
				StoreObjectId storeObjectId2;
				string text2;
				Dictionary<string, string> dictionary;
				providerState.StateStorage.TryFindFolder(change.CloudId, out text, out storeObjectId, out changeKey, out storeObjectId2, out text2, out dictionary);
				folderId = VersionedId.FromStoreObjectId(syncChangeEntry.NativeFolderId, changeKey);
				providerState.SyncLogSession.LogDebugging((TSLID)1153UL, XSOSyncStorageProvider.Tracer, "Search for folder {0} passed. Found new id: {1}.", new object[]
				{
					change,
					folderId
				});
				return true;
			}
			return false;
		}

		private void DeleteItem(XSOSyncStorageProviderState providerState, SyncChangeEntry change)
		{
			XSOSyncStorageProvider.<>c__DisplayClass11 CS$<>8__locals1 = new XSOSyncStorageProvider.<>c__DisplayClass11();
			CS$<>8__locals1.providerState = providerState;
			StoreObjectId storeId = change.NewNativeFolderId ?? change.NativeFolderId;
			using (Folder folder = SyncStoreLoadManager.FolderBind(CS$<>8__locals1.providerState.SyncMailboxSession.MailboxSession, storeId, null, null))
			{
				StoreObjectId storeObjectId;
				if (!this.TryItemOperation<ArgumentException>(CS$<>8__locals1.providerState, change, delegate(StoreId itemStoreObjectId)
				{
					SyncStoreLoadManager.ThrottleAndExecuteStoreCall(CS$<>8__locals1.providerState.SyncMailboxSession.MailboxSession, new EventHandler<RoundtripCompleteEventArgs>(CS$<>8__locals1.providerState.OnRoundtripComplete), "DeleteObjects", delegate
					{
						folder.DeleteObjects(DeleteItemFlags.HardDelete, new StoreId[]
						{
							itemStoreObjectId
						});
					});
				}, out storeObjectId))
				{
					CS$<>8__locals1.providerState.SyncLogSession.LogVerbose((TSLID)1217UL, XSOSyncStorageProvider.Tracer, "Ignoring the error on delete for item {0}.", new object[]
					{
						change
					});
					change.Exception = null;
				}
			}
		}

		private void CreateEmail(XSOSyncStorageProviderState providerState, SyncChangeEntry change)
		{
			ExTraceGlobals.FaultInjectionTracer.TraceTest(3127258429U);
			StoreObjectId storeId = change.NewNativeFolderId ?? change.NativeFolderId;
			using (MessageItem messageItem = SyncStoreLoadManager.MessageItemCreateAggregated(providerState.SyncMailboxSession.MailboxSession, storeId, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete)))
			{
				XSOSyncStorageProvider.PopulateItemProperties(providerState, change, messageItem);
				SyncStoreLoadManager.ItemSave(messageItem, providerState.SyncMailboxSession.MailboxSession, SaveMode.ResolveConflicts, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete));
				providerState.SyncLogSession.LogDebugging((TSLID)1130UL, XSOSyncStorageProvider.Tracer, "Added Message: {0}", new object[]
				{
					change
				});
				SyncStoreLoadManager.ItemLoad(messageItem, providerState.SyncMailboxSession.MailboxSession, XSOSyncStorageProvider.ClientOperationProperties, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete));
				if (change.NativeId != null)
				{
					change.NewNativeId = messageItem.Id.ObjectId;
				}
				else
				{
					change.NativeId = messageItem.Id.ObjectId;
				}
				change.ChangeKey = messageItem.Id.ChangeKeyAsByteArray();
			}
		}

		private void AddItem(XSOSyncStorageProviderState providerState, SyncChangeEntry change)
		{
			switch (change.SchemaType)
			{
			case SchemaType.Email:
				this.CreateEmail(providerState, change);
				return;
			case SchemaType.Contact:
				XSOSyncStorageProvider.CreateContact(providerState, change);
				return;
			default:
				providerState.SyncLogSession.LogError((TSLID)1131UL, XSOSyncStorageProvider.Tracer, "Invalid schema type found in change : {0}.", new object[]
				{
					change
				});
				throw new NotImplementedException();
			}
		}

		private void ApplyItemChange(XSOSyncStorageProviderState providerState, SyncChangeEntry change)
		{
			switch (change.ChangeType)
			{
			case ChangeType.Change:
			case ChangeType.ReadFlagChange:
			case ChangeType.SoftDelete:
			{
				if (change.NewNativeFolderId != null)
				{
					bool flag = false;
					try
					{
						this.MoveItem(providerState, change, providerState.UnderRecovery);
						flag = true;
					}
					finally
					{
						if (!flag)
						{
							NativeSyncStorageProvider.SetRecoverySync(providerState);
						}
					}
				}
				if (change.ChangeType == ChangeType.Change && (providerState.Subscription.SyncQuirks & SyncQuirks.ApplyItemChangeAsDeleteAndAdd) != SyncQuirks.None)
				{
					providerState.SyncLogSession.LogDebugging((TSLID)1132UL, XSOSyncStorageProvider.Tracer, "Translating change to delete and add: {0}.", new object[]
					{
						change
					});
					this.DeleteItem(providerState, change);
					providerState.SyncLogSession.LogDebugging((TSLID)1133UL, XSOSyncStorageProvider.Tracer, "Change deleted: {0}. Now recreating.", new object[]
					{
						change
					});
					this.AddItem(providerState, change);
					providerState.SyncLogSession.LogDebugging((TSLID)1134UL, XSOSyncStorageProvider.Tracer, "Changed added: {0}.", new object[]
					{
						change
					});
					return;
				}
				if (change.ChangeType == ChangeType.SoftDelete)
				{
					return;
				}
				StoreObjectId folderId = (change.NewNativeFolderId != null) ? change.NewNativeFolderId : change.NativeFolderId;
				StoreObjectId itemId = (change.NewNativeId != null) ? change.NewNativeId : change.NativeId;
				Item item = null;
				StoreObjectId itemId2;
				if (!this.TryItemOperation<ObjectNotFoundException>(providerState, change, true, delegate(StoreId itemStoreObjectId)
				{
					item = SyncStoreLoadManager.ItemBind(providerState.SyncMailboxSession.MailboxSession, itemStoreObjectId, null, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete));
				}, out itemId2))
				{
					return;
				}
				itemId = itemId2;
				using (item)
				{
					item.OpenAsReadWrite();
					ExDateTime itemLastModificationTimeAtNativeServer = item.LastModifiedTime;
					XSOSyncStorageProvider.PopulateItemProperties(providerState, change, item);
					if (providerState.Subscription.AggregationType == AggregationType.PeopleConnection)
					{
						XSOSyncStorageProvider.SaveAndReloadAfterLinking(providerState, change, (Contact)item, () => XSOSyncStorageProvider.TrySaveAndReloadExistingItem(providerState, change, item, itemId, folderId, itemLastModificationTimeAtNativeServer));
					}
					else
					{
						XSOSyncStorageProvider.TrySaveAndReloadExistingItem(providerState, change, item, itemId, folderId, itemLastModificationTimeAtNativeServer);
					}
					return;
				}
				break;
			}
			case (ChangeType)3:
				goto IL_337;
			case ChangeType.Delete:
				break;
			default:
				goto IL_337;
			}
			this.DeleteItem(providerState, change);
			return;
			IL_337:
			throw new NotImplementedException("Invalid ChangeType: " + change.ChangeType);
		}

		private void ApplyFolderChange(XSOSyncStorageProviderState providerState, SyncChangeEntry change)
		{
			XSOSyncStorageProvider.<>c__DisplayClass25 CS$<>8__locals1 = new XSOSyncStorageProvider.<>c__DisplayClass25();
			CS$<>8__locals1.providerState = providerState;
			CS$<>8__locals1.providerState.SyncLogSession.LogDebugging((TSLID)1139UL, XSOSyncStorageProvider.Tracer, "Writing folder change to the store", new object[0]);
			SyncFolder syncFolder = (SyncFolder)change.SyncObject;
			switch (change.ChangeType)
			{
			case ChangeType.Add:
				this.ApplyAddFolderChange(CS$<>8__locals1.providerState, change);
				return;
			case ChangeType.Change:
			case ChangeType.SoftDelete:
			{
				if (change.ChangeType != ChangeType.SoftDelete && syncFolder.DefaultFolderType != DefaultFolderType.None)
				{
					CS$<>8__locals1.providerState.SyncLogSession.LogInformation((TSLID)1140UL, XSOSyncStorageProvider.Tracer, "Ignoring change to default folder: {0}", new object[]
					{
						change
					});
					return;
				}
				if (change.NewNativeFolderId != null)
				{
					bool flag = false;
					try
					{
						this.MoveFolder(CS$<>8__locals1.providerState, change);
						flag = true;
					}
					finally
					{
						if (!flag)
						{
							NativeSyncStorageProvider.SetRecoverySync(CS$<>8__locals1.providerState);
						}
					}
				}
				if (change.ChangeType == ChangeType.SoftDelete)
				{
					return;
				}
				Folder folder = null;
				StoreObjectId nativeId;
				if (!this.TryFolderOperation(CS$<>8__locals1.providerState, change, delegate(StoreId folderStoreObjectId)
				{
					folder = SyncStoreLoadManager.FolderBind(CS$<>8__locals1.providerState.SyncMailboxSession.MailboxSession, folderStoreObjectId, null, new EventHandler<RoundtripCompleteEventArgs>(CS$<>8__locals1.providerState.OnRoundtripComplete));
				}, out nativeId))
				{
					CS$<>8__locals1.providerState.SyncLogSession.LogVerbose((TSLID)1220UL, XSOSyncStorageProvider.Tracer, "ApplyFolderChange failed for {0}", new object[]
					{
						change
					});
					return;
				}
				using (folder)
				{
					base.PopulateFolderProperties(folder, syncFolder);
					base.RecordNativeFolderName(change, folder.DisplayName);
					SyncStoreLoadManager.FolderSave(folder, CS$<>8__locals1.providerState.SyncMailboxSession.MailboxSession, new EventHandler<RoundtripCompleteEventArgs>(CS$<>8__locals1.providerState.OnRoundtripComplete));
					SyncStoreLoadManager.FolderLoad(folder, CS$<>8__locals1.providerState.SyncMailboxSession.MailboxSession, NativeSyncStorageProvider.FolderProperties, new EventHandler<RoundtripCompleteEventArgs>(CS$<>8__locals1.providerState.OnRoundtripComplete));
					CS$<>8__locals1.providerState.SyncLogSession.LogDebugging((TSLID)1141UL, XSOSyncStorageProvider.Tracer, "Changed Folder: {0}", new object[]
					{
						change
					});
					change.ChangeKey = folder.Id.ChangeKeyAsByteArray();
					this.RecordNativeFolderOperation(CS$<>8__locals1.providerState, change, nativeId, change.ChangeType, folder);
					return;
				}
				break;
			}
			case (ChangeType)3:
				goto IL_5A9;
			case ChangeType.Delete:
				break;
			case ChangeType.ReadFlagChange:
				goto IL_5A3;
			default:
				goto IL_5A9;
			}
			if ((CS$<>8__locals1.providerState.Subscription.SyncQuirks & SyncQuirks.OnlyDeleteFoldersIfNoSubFolders) != SyncQuirks.None)
			{
				Folder folder = null;
				StoreObjectId nativeId;
				if (!this.TryFolderOperation(CS$<>8__locals1.providerState, change, delegate(StoreId folderStoreObjectId)
				{
					folder = SyncStoreLoadManager.FolderBind(CS$<>8__locals1.providerState.SyncMailboxSession.MailboxSession, folderStoreObjectId, null, new EventHandler<RoundtripCompleteEventArgs>(CS$<>8__locals1.providerState.OnRoundtripComplete));
				}, out nativeId))
				{
					CS$<>8__locals1.providerState.SyncLogSession.LogVerbose((TSLID)1226UL, XSOSyncStorageProvider.Tracer, "Ignoring failure to perform delete on folder: {0}", new object[]
					{
						change
					});
					change.Exception = null;
					return;
				}
				using (folder)
				{
					if (folder.SubfolderCount > 0)
					{
						using (IEnumerator<string> cloudItemFilteredByCloudFolderIdEnumerator = CS$<>8__locals1.providerState.StateStorage.GetCloudItemFilteredByCloudFolderIdEnumerator(change.CloudId))
						{
							cloudItemFilteredByCloudFolderIdEnumerator.Reset();
							Dictionary<StoreObjectId, string> dictionary = new Dictionary<StoreObjectId, string>(folder.ItemCount);
							while (cloudItemFilteredByCloudFolderIdEnumerator.MoveNext())
							{
								string text = cloudItemFilteredByCloudFolderIdEnumerator.Current;
								string text2;
								StoreObjectId key;
								byte[] array;
								StoreObjectId storeObjectId;
								string text3;
								Dictionary<string, string> dictionary2;
								if (CS$<>8__locals1.providerState.StateStorage.TryFindItem(text, out text2, out key, out array, out storeObjectId, out text3, out dictionary2))
								{
									dictionary.Add(key, text);
								}
							}
							try
							{
								StoreObjectId[] array2 = new StoreObjectId[dictionary.Count];
								dictionary.Keys.CopyTo(array2, 0);
								folder.DeleteObjects(DeleteItemFlags.HardDelete, array2);
							}
							catch (ObjectNotFoundException)
							{
								CS$<>8__locals1.providerState.SyncLogSession.LogDebugging((TSLID)1142UL, XSOSyncStorageProvider.Tracer, "Hit ObjectNotFoundException when deleting all items in a folder.", new object[0]);
							}
							foreach (StoreObjectId key2 in dictionary.Keys)
							{
								string text4 = dictionary[key2];
								if (!CS$<>8__locals1.providerState.StateStorage.TryRemoveItem(text4))
								{
									CS$<>8__locals1.providerState.SyncLogSession.LogError((TSLID)1144UL, XSOSyncStorageProvider.Tracer, "Could not remove item from mapping table: {0}.", new object[]
									{
										text4
									});
								}
							}
							change.Persist = false;
							return;
						}
					}
				}
			}
			using (Folder parentFolder = SyncStoreLoadManager.FolderBind(CS$<>8__locals1.providerState.SyncMailboxSession.MailboxSession, change.NativeFolderId, null, new EventHandler<RoundtripCompleteEventArgs>(CS$<>8__locals1.providerState.OnRoundtripComplete)))
			{
				StoreObjectId nativeId;
				if (!this.TryFolderOperation(CS$<>8__locals1.providerState, change, delegate(StoreId correctObjectId)
				{
					SyncStoreLoadManager.ThrottleAndExecuteStoreCall(CS$<>8__locals1.providerState.SyncMailboxSession.MailboxSession, new EventHandler<RoundtripCompleteEventArgs>(CS$<>8__locals1.providerState.OnRoundtripComplete), "DeleteObjects", delegate
					{
						parentFolder.DeleteObjects(DeleteItemFlags.HardDelete, new StoreId[]
						{
							correctObjectId
						});
					});
				}, out nativeId))
				{
					CS$<>8__locals1.providerState.SyncLogSession.LogVerbose((TSLID)1233UL, XSOSyncStorageProvider.Tracer, "Ignoring failure to perform delete on folder: {0}", new object[]
					{
						change
					});
					change.Exception = null;
					return;
				}
				CS$<>8__locals1.providerState.SyncLogSession.LogDebugging((TSLID)1145UL, XSOSyncStorageProvider.Tracer, "Hard Deleted Folder: {0}", new object[]
				{
					change
				});
				this.RecordNativeFolderOperation(CS$<>8__locals1.providerState, change, nativeId, change.ChangeType, null);
				return;
			}
			IL_5A3:
			throw new NotSupportedException();
			IL_5A9:
			throw new NotImplementedException();
		}

		private void MoveItem(XSOSyncStorageProviderState providerState, SyncChangeEntry change, bool underRecovery)
		{
			string text = change.CloudId;
			Item item = null;
			StoreObjectId correctItemId;
			if (!this.TryItemOperation(providerState, change, delegate(StoreId itemStoreObjectId)
			{
				item = SyncStoreLoadManager.ItemBind(providerState.SyncMailboxSession.MailboxSession, itemStoreObjectId, XSOSyncStorageProvider.MoveItemProperties, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete));
			}, out correctItemId))
			{
				providerState.SyncLogSession.LogVerbose((TSLID)1234UL, XSOSyncStorageProvider.Tracer, "Move failed for change {0}", new object[]
				{
					change
				});
				return;
			}
			try
			{
				using (item)
				{
					text = SyncUtilities.SafeGetProperty<string>(item, ItemSchema.CloudId);
					if (string.IsNullOrEmpty(text) || change.CloudId.GetHashCode() != text.GetHashCode())
					{
						item.OpenAsReadWrite();
						text = change.CloudId;
						item[ItemSchema.CloudId] = change.CloudId;
						item[MessageItemSchema.SharingInstanceGuid] = providerState.Subscription.SubscriptionGuid;
						SyncStoreLoadManager.ItemSave(item, providerState.SyncMailboxSession.MailboxSession, SaveMode.ResolveConflicts, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete));
					}
				}
				using (Folder origin = SyncStoreLoadManager.FolderBind(providerState.SyncMailboxSession.MailboxSession, change.NativeFolderId, null, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete)))
				{
					using (Folder destination = SyncStoreLoadManager.FolderBind(providerState.SyncMailboxSession.MailboxSession, change.NewNativeFolderId, null, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete)))
					{
						GroupOperationResult result = null;
						SyncStoreLoadManager.ThrottleAndExecuteStoreCall(providerState.SyncMailboxSession.MailboxSession, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete), "MoveItems", delegate
						{
							result = origin.MoveItems(destination, new StoreObjectId[]
							{
								correctItemId
							}, null, null, true);
						});
						if (result != null && result.ResultObjectIds != null && result.ResultObjectIds.Count == 1)
						{
							change.NewNativeId = result.ResultObjectIds[0];
							providerState.SyncLogSession.LogDebugging((TSLID)1146UL, XSOSyncStorageProvider.Tracer, "Got ID from move operation.", new object[0]);
						}
						else
						{
							providerState.SyncLogSession.LogError((TSLID)1147UL, XSOSyncStorageProvider.Tracer, "Failed to get ID from move operation. Will proceed to find the item.", new object[0]);
						}
					}
				}
			}
			catch (ObjectNotFoundException ex)
			{
				if (!underRecovery)
				{
					providerState.SyncLogSession.LogError((TSLID)1148UL, XSOSyncStorageProvider.Tracer, "Failed to move item change: {0} - {1}.", new object[]
					{
						change,
						ex
					});
					change.NewNativeFolderId = null;
					return;
				}
				providerState.SyncLogSession.LogVerbose((TSLID)1149UL, XSOSyncStorageProvider.Tracer, "Failed to move item, will keep recovering: {0} - {1}.", new object[]
				{
					change,
					ex
				});
			}
			if (change.NewNativeId == null)
			{
				using (Folder folder = SyncStoreLoadManager.FolderBind(providerState.SyncMailboxSession.MailboxSession, change.NewNativeFolderId, null, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete)))
				{
					ComparisonFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.CloudId, text);
					SortBy[] sort = new SortBy[1];
					sort[0] = new SortBy(ItemSchema.CloudId, SortOrder.Ascending);
					QueryResult queryResult = null;
					SyncStoreLoadManager.ThrottleAndExecuteStoreCall(providerState.SyncMailboxSession.MailboxSession, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete), "ItemQuery", delegate
					{
						queryResult = folder.ItemQuery(ItemQueryType.None, null, sort, XSOSyncStorageProvider.MoveItemPropertiesStartingWithCloudIdAndId);
					});
					using (queryResult)
					{
						bool result = false;
						SyncStoreLoadManager.ThrottleAndExecuteStoreCall(providerState.SyncMailboxSession.MailboxSession, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete), "SeekToCondition", delegate
						{
							result = queryResult.SeekToCondition(SeekReference.OriginBeginning, filter);
						});
						if (!result)
						{
							providerState.SyncLogSession.LogError((TSLID)1150UL, XSOSyncStorageProvider.Tracer, "Could not find the item after move: {0}.", new object[]
							{
								change
							});
						}
						else
						{
							object[][] partialResults = null;
							SyncStoreLoadManager.ThrottleAndExecuteStoreCall(providerState.SyncMailboxSession.MailboxSession, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete), "GetRows", delegate
							{
								partialResults = queryResult.GetRows(1);
							});
							if (partialResults == null || partialResults.Length != 1)
							{
								providerState.SyncLogSession.LogError((TSLID)1151UL, XSOSyncStorageProvider.Tracer, "Failed to find item after move: {0}.", new object[]
								{
									change
								});
							}
							else
							{
								VersionedId versionedId = (VersionedId)partialResults[0][1];
								change.NewNativeId = versionedId.ObjectId;
							}
						}
					}
				}
			}
		}

		private void MoveFolder(XSOSyncStorageProviderState providerState, SyncChangeEntry change)
		{
			Folder folder = null;
			StoreObjectId correctFolderId;
			if (!this.TryFolderOperation(providerState, change, delegate(StoreId folderStoreObjectId)
			{
				folder = SyncStoreLoadManager.FolderBind(providerState.SyncMailboxSession.MailboxSession, folderStoreObjectId, null, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete));
			}, out correctFolderId))
			{
				providerState.SyncLogSession.LogVerbose((TSLID)1258UL, XSOSyncStorageProvider.Tracer, "MoveFolder failed for {0}", new object[]
				{
					change
				});
				return;
			}
			using (folder)
			{
				SyncStoreLoadManager.ThrottleAndExecuteStoreCall(providerState.SyncMailboxSession.MailboxSession, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete), "MoveFolder", delegate
				{
					folder.MoveFolder(change.NewNativeFolderId, correctFolderId);
				});
				change.ChangeKey = folder.Id.ChangeKeyAsByteArray();
				base.RecordNativeFolderName(change, folder.DisplayName);
				this.RecordNativeFolderOperation(providerState, change, correctFolderId, ChangeType.Change, folder);
			}
		}

		private void HandleItemsRecovery(AsyncResult<XSOSyncStorageProviderState, SyncProviderResultData> asyncResult, XSOSyncStorageProviderState providerState, IList<SyncChangeEntry> changeList, out StoragePermanentException toTreatAsTransient)
		{
			providerState.SyncLogSession.LogDebugging((TSLID)1179UL, XSOSyncStorageProvider.Tracer, "Handling multi-item recovery.", new object[0]);
			toTreatAsTransient = null;
			Dictionary<StoreObjectId, Dictionary<string, SyncChangeEntry>> dictionary = new Dictionary<StoreObjectId, Dictionary<string, SyncChangeEntry>>(10);
			List<string> list = new List<string>(10);
			foreach (SyncChangeEntry syncChangeEntry in changeList)
			{
				if (XSOSyncStorageProvider.ShouldCancel<XSOSyncStorageProviderState, SyncProviderResultData>(asyncResult, providerState))
				{
					return;
				}
				if (syncChangeEntry.SchemaType == SchemaType.Folder)
				{
					list.Add(syncChangeEntry.CloudId);
				}
				else if (syncChangeEntry.ChangeType == ChangeType.Add && (syncChangeEntry.CloudFolderId == null || !list.Contains(syncChangeEntry.CloudFolderId)))
				{
					if (!this.ResolveChange(providerState, syncChangeEntry))
					{
						providerState.SyncLogSession.LogVerbose((TSLID)1180UL, XSOSyncStorageProvider.Tracer, "Ignoring unresolved change in multi-item recovery: {0}.", new object[]
						{
							syncChangeEntry
						});
					}
					else if (syncChangeEntry.NativeFolderId != null)
					{
						if (!dictionary.ContainsKey(syncChangeEntry.NativeFolderId))
						{
							dictionary[syncChangeEntry.NativeFolderId] = new Dictionary<string, SyncChangeEntry>(changeList.Count / 10);
						}
						providerState.SyncLogSession.LogDebugging((TSLID)1181UL, XSOSyncStorageProvider.Tracer, "Adding change for multi-item recovery: {0}.", new object[]
						{
							syncChangeEntry
						});
						if (dictionary[syncChangeEntry.NativeFolderId].ContainsKey(syncChangeEntry.CloudId))
						{
							providerState.SyncLogSession.LogError((TSLID)1182UL, XSOSyncStorageProvider.Tracer, "Could handle recovery for item ADD as there is already an ADD for this item in the changelist. This generally means a provider bug or malicious content.", new object[0]);
							syncChangeEntry.Recovered = true;
							syncChangeEntry.Exception = SyncPermanentException.CreateItemLevelException(new SyncPoisonItemFoundException(syncChangeEntry.ToString(), providerState.Subscription.SubscriptionGuid));
						}
						else
						{
							dictionary[syncChangeEntry.NativeFolderId].Add(syncChangeEntry.CloudId, syncChangeEntry);
						}
					}
				}
			}
			foreach (KeyValuePair<StoreObjectId, Dictionary<string, SyncChangeEntry>> keyValuePair in dictionary)
			{
				StoreObjectId storeObjectId = keyValuePair.Key;
				Dictionary<string, SyncChangeEntry> value = keyValuePair.Value;
				providerState.SyncLogSession.LogDebugging((TSLID)1183UL, XSOSyncStorageProvider.Tracer, "Considering multi-item recovery changes for folder: {0}.", new object[]
				{
					storeObjectId
				});
				SyncChangeEntry syncChangeEntry2 = null;
				using (Dictionary<string, SyncChangeEntry>.ValueCollection.Enumerator enumerator3 = value.Values.GetEnumerator())
				{
					if (enumerator3.MoveNext())
					{
						SyncChangeEntry syncChangeEntry3 = enumerator3.Current;
						syncChangeEntry2 = syncChangeEntry3;
					}
				}
				if (!base.TryEnsureFullParentFolderHierarchy(providerState, syncChangeEntry2))
				{
					providerState.SyncLogSession.LogVerbose((TSLID)1259UL, XSOSyncStorageProvider.Tracer, "Could not ensure folder hierarchy for change {0} and so skipping recovering changes for all items related to folder {1}.", new object[]
					{
						syncChangeEntry2,
						storeObjectId
					});
				}
				else
				{
					if (!object.Equals(storeObjectId, syncChangeEntry2.NativeFolderId))
					{
						providerState.SyncLogSession.LogVerbose((TSLID)1030UL, XSOSyncStorageProvider.Tracer, "As part of ensuring parent folder hierarchy, parent folder has been recreated. Old Id '{0}' mapped to new Id '{1}'.", new object[]
						{
							storeObjectId,
							syncChangeEntry2.NativeFolderId
						});
						storeObjectId = syncChangeEntry2.NativeFolderId;
						foreach (SyncChangeEntry syncChangeEntry4 in value.Values)
						{
							syncChangeEntry4.NativeFolderId = storeObjectId;
						}
					}
					using (Folder folder = SyncStoreLoadManager.FolderBind(providerState.SyncMailboxSession.MailboxSession, storeObjectId, null, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete)))
					{
						SortBy[] sort = new SortBy[1];
						sort[0] = new SortBy(ItemSchema.CloudId, SortOrder.Descending);
						QueryResult queryResult = null;
						SyncStoreLoadManager.ThrottleAndExecuteStoreCall(providerState.SyncMailboxSession.MailboxSession, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete), "ItemQuery", delegate
						{
							queryResult = folder.ItemQuery(ItemQueryType.None, null, sort, XSOSyncStorageProvider.RecoveryItemPropertiesStartingWithCloudId);
						});
						using (queryResult)
						{
							foreach (string text in value.Keys)
							{
								QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.CloudId, text);
								bool rowsExist = false;
								SyncStoreLoadManager.ThrottleAndExecuteStoreCall(providerState.SyncMailboxSession.MailboxSession, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete), "SeekToCondition", delegate
								{
									rowsExist = queryResult.SeekToCondition(SeekReference.OriginBeginning, filter);
								});
								while (rowsExist)
								{
									object[][] partialResults = null;
									SyncStoreLoadManager.ThrottleAndExecuteStoreCall(providerState.SyncMailboxSession.MailboxSession, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete), "GetRows", delegate
									{
										partialResults = queryResult.GetRows(10000);
									});
									if (XSOSyncStorageProvider.ShouldCancel<XSOSyncStorageProviderState, SyncProviderResultData>(asyncResult, providerState))
									{
										return;
									}
									if (partialResults.Length <= 0)
									{
										break;
									}
									int i = 0;
									while (i < partialResults.Length)
									{
										string value2 = partialResults[i][0] as string;
										StoreObjectId objectId = ((VersionedId)partialResults[i][1]).ObjectId;
										if (!text.Equals(value2))
										{
											break;
										}
										SyncChangeEntry syncChangeEntry5 = value[text];
										if (syncChangeEntry5.Recovered)
										{
											syncChangeEntry5.Exception = SyncPermanentException.CreateItemLevelException(new MultipleNativeItemsHaveSameCloudIdException(text, providerState.Subscription.SubscriptionGuid));
											break;
										}
										providerState.SyncLogSession.LogDebugging((TSLID)1186UL, XSOSyncStorageProvider.Tracer, "Recovering change: {0}.", new object[]
										{
											syncChangeEntry5
										});
										syncChangeEntry5.NativeId = objectId;
										try
										{
											this.UpdateMapping(providerState, syncChangeEntry5);
										}
										catch (StoragePermanentException ex)
										{
											if (ExceptionUtilities.ShouldPermanentMapiOrXsoExceptionBeTreatedAsTransient(providerState.SyncLogSession, ex))
											{
												toTreatAsTransient = ex;
												return;
											}
											providerState.SyncLogSession.LogError((TSLID)1187UL, XSOSyncStorageProvider.Tracer, "Ignoring found change because of exception in multi-item recovery: {0} - {1}.", new object[]
											{
												syncChangeEntry5,
												ex
											});
											goto IL_5EF;
										}
										goto IL_5E7;
										IL_5EF:
										i++;
										continue;
										IL_5E7:
										syncChangeEntry5.Recovered = true;
										goto IL_5EF;
									}
								}
							}
						}
					}
				}
			}
		}

		private bool HandleItemRecovery(AsyncResult<XSOSyncStorageProviderState, SyncProviderResultData> asyncResult, XSOSyncStorageProviderState providerState, SyncChangeEntry change, out StoragePermanentException toTreatAsTransient)
		{
			providerState.SyncLogSession.LogDebugging((TSLID)1188UL, XSOSyncStorageProvider.Tracer, "Handling item recovery for change: {0}.", new object[]
			{
				change
			});
			toTreatAsTransient = null;
			if (XSOSyncStorageProvider.ShouldCancel<XSOSyncStorageProviderState, SyncProviderResultData>(asyncResult, providerState))
			{
				return false;
			}
			if (!base.TryEnsureFullParentFolderHierarchy(providerState, change))
			{
				providerState.SyncLogSession.LogVerbose((TSLID)1382UL, XSOSyncStorageProvider.Tracer, "Could not ensure folder hierarchy for change {0} and so skipping recovering change.", new object[]
				{
					change
				});
				return false;
			}
			if (XSOSyncStorageProvider.ShouldCancel<XSOSyncStorageProviderState, SyncProviderResultData>(asyncResult, providerState))
			{
				return false;
			}
			if (change.SchemaType == SchemaType.Folder)
			{
				SyncFolder syncFolder = (SyncFolder)change.SyncObject;
				if (syncFolder.DefaultFolderType != DefaultFolderType.None)
				{
					providerState.SyncLogSession.LogDebugging((TSLID)1383UL, XSOSyncStorageProvider.Tracer, "Skipping recovery for default folders: {0}", new object[]
					{
						change
					});
					return false;
				}
				try
				{
					using (Folder parentFolder = SyncStoreLoadManager.FolderBind(providerState.SyncMailboxSession.MailboxSession, change.NativeFolderId, null, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete)))
					{
						QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, FolderSchema.DisplayName, syncFolder.DisplayName);
						SortBy[] sortByDisplayName = new SortBy[]
						{
							new SortBy(FolderSchema.DisplayName, SortOrder.Ascending)
						};
						QueryResult queryResult = null;
						SyncStoreLoadManager.ThrottleAndExecuteStoreCall(providerState.SyncMailboxSession.MailboxSession, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete), "FolderQuery", delegate
						{
							queryResult = parentFolder.FolderQuery(FolderQueryFlags.None, null, sortByDisplayName, NativeSyncStorageProvider.FolderPropertiesStartingWithDisplayName);
						});
						using (queryResult)
						{
							bool rowsExist = false;
							SyncStoreLoadManager.ThrottleAndExecuteStoreCall(providerState.SyncMailboxSession.MailboxSession, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete), "SeekToCondition", delegate
							{
								rowsExist = queryResult.SeekToCondition(SeekReference.OriginBeginning, filter);
							});
							if (!rowsExist)
							{
								return false;
							}
							object[][] partialResults = null;
							SyncStoreLoadManager.ThrottleAndExecuteStoreCall(providerState.SyncMailboxSession.MailboxSession, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete), "GetRows", delegate
							{
								partialResults = queryResult.GetRows(1);
							});
							if (partialResults.Length != 1)
							{
								return false;
							}
							VersionedId versionedId = (VersionedId)partialResults[0][1];
							StoreObjectId objectId = versionedId.ObjectId;
							using (Folder folder = SyncStoreLoadManager.FolderBind(providerState.SyncMailboxSession.MailboxSession, versionedId, NativeSyncStorageProvider.FolderProperties, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete)))
							{
								this.RecordNativeFolderOperation(providerState, change, objectId, change.ChangeType, folder);
							}
							return this.TryRecoverFolder(providerState, change, versionedId);
						}
					}
				}
				catch (StoragePermanentException ex)
				{
					if (ExceptionUtilities.ShouldPermanentMapiOrXsoExceptionBeTreatedAsTransient(providerState.SyncLogSession, ex))
					{
						toTreatAsTransient = ex;
					}
					else
					{
						providerState.SyncLogSession.LogError((TSLID)1190UL, XSOSyncStorageProvider.Tracer, "Ignoring folder change because of exception in single-item recovery: {0} - {1}.", new object[]
						{
							change,
							ex
						});
					}
					return false;
				}
			}
			bool result;
			try
			{
				VersionedId foundItemId;
				if (!XSOSyncStorageProvider.TryFindItem(providerState, change, out foundItemId))
				{
					result = false;
				}
				else
				{
					result = this.TryRecoverItem(providerState, change, foundItemId);
				}
			}
			catch (StoragePermanentException ex2)
			{
				if (ExceptionUtilities.ShouldPermanentMapiOrXsoExceptionBeTreatedAsTransient(providerState.SyncLogSession, ex2))
				{
					toTreatAsTransient = ex2;
				}
				else
				{
					providerState.SyncLogSession.LogError((TSLID)1194UL, XSOSyncStorageProvider.Tracer, "Ignoring item change because of exception in single-item recovery: {0} - {1}.", new object[]
					{
						change,
						ex2
					});
				}
				result = false;
			}
			return result;
		}

		private bool TryRecoverFolder(XSOSyncStorageProviderState providerState, SyncChangeEntry change, VersionedId foundFolderId)
		{
			StoreObjectId objectId = foundFolderId.ObjectId;
			foundFolderId.ChangeKeyAsByteArray();
			string text;
			bool flag = base.IsNativeIdMappedToADifferentFolder(providerState, objectId, change.CloudId, out text);
			if (flag)
			{
				if (change.NewCloudId == null || change.NewCloudId != text)
				{
					providerState.SyncLogSession.LogDebugging((TSLID)1033UL, XSOSyncStorageProvider.Tracer, "DisplayName for Change {0} is already mapped to a different cloud id {1}.", new object[]
					{
						change,
						text
					});
					return false;
				}
				providerState.SyncLogSession.LogDebugging((TSLID)1034UL, XSOSyncStorageProvider.Tracer, "Folder for Change {0} is already mapped to the new cloud id {1}.", new object[]
				{
					change,
					text
				});
			}
			return this.TryRecoverItem(providerState, change, foundFolderId);
		}

		private bool TryRecoverItem(XSOSyncStorageProviderState providerState, SyncChangeEntry change, VersionedId foundItemId)
		{
			StoreObjectId objectId = foundItemId.ObjectId;
			byte[] changeKey = foundItemId.ChangeKeyAsByteArray();
			SyncChangeEntry syncChangeEntry;
			if (change.NativeId == null)
			{
				syncChangeEntry = new SyncChangeEntry(ChangeType.Add, change.SchemaType, change.CloudId);
				syncChangeEntry.CloudFolderId = change.CloudFolderId;
				syncChangeEntry.NativeFolderId = change.NativeFolderId;
				syncChangeEntry.Properties = change.Properties;
				syncChangeEntry.NativeId = objectId;
				syncChangeEntry.ChangeKey = changeKey;
			}
			else if (!objectId.Equals(change.NativeId))
			{
				syncChangeEntry = new SyncChangeEntry(ChangeType.Change, change.SchemaType, change.CloudId);
				if (!this.ResolveChange(providerState, syncChangeEntry))
				{
					string text = string.Format("Failed to resolve mappingTableChange when updating nativeId for {0} to {1}", change, objectId);
					providerState.SyncLogSession.LogError((TSLID)1042UL, XSOSyncStorageProvider.Tracer, text, new object[0]);
					providerState.SyncLogSession.ReportWatson(text);
					return false;
				}
				syncChangeEntry.NewNativeId = objectId;
				syncChangeEntry.ChangeKey = changeKey;
				change.ChangeType = ChangeType.Change;
			}
			else
			{
				syncChangeEntry = null;
			}
			if (syncChangeEntry != null)
			{
				providerState.SyncLogSession.LogDebugging((TSLID)1189UL, XSOSyncStorageProvider.Tracer, "Recovering change: {0} via updating mapping table to {1}.", new object[]
				{
					change,
					syncChangeEntry
				});
				this.UpdateMapping(providerState, syncChangeEntry);
				change.NativeId = objectId;
				change.ChangeKey = changeKey;
			}
			change.Recovered = (change.ChangeType == ChangeType.Add);
			return change.Recovered;
		}

		private void BeginApplyItem(object state)
		{
			Exception ex = null;
			AsyncResult<XSOSyncStorageProviderState, SyncProviderResultData> asyncResult = (AsyncResult<XSOSyncStorageProviderState, SyncProviderResultData>)state;
			XSOSyncStorageProviderState state2 = asyncResult.State;
			state2.SyncLogSession.LogDebugging((TSLID)1201UL, XSOSyncStorageProvider.Tracer, "Processing Items", new object[0]);
			bool flag = true;
			while (state2.ItemsToProcess.Count > 0)
			{
				if (XSOSyncStorageProvider.ShouldCancel<XSOSyncStorageProviderState, SyncProviderResultData>(asyncResult, state2))
				{
					return;
				}
				SyncChangeEntry syncChangeEntry = state2.ItemsToProcess.Dequeue();
				try
				{
					flag = true;
					if (syncChangeEntry.Recovered)
					{
						state2.SyncLogSession.LogVerbose((TSLID)1044UL, XSOSyncStorageProvider.Tracer, "Skipping item as already recovered: {0}", new object[]
						{
							syncChangeEntry
						});
						continue;
					}
					try
					{
						if (!this.ResolveChange(state2, syncChangeEntry))
						{
							state2.SyncLogSession.LogVerbose((TSLID)1046UL, XSOSyncStorageProvider.Tracer, "Skipping item as failed to resolve: {0}", new object[]
							{
								syncChangeEntry
							});
							continue;
						}
					}
					catch (StorageTransientException ex2)
					{
						state2.SyncLogSession.LogError((TSLID)1384UL, XSOSyncStorageProvider.Tracer, "Hit transient exception in ResolveChange: {0}.", new object[]
						{
							ex2
						});
						flag = false;
						ex = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, ex2);
					}
					catch (SyncStoreUnhealthyException ex3)
					{
						state2.SyncLogSession.LogError((TSLID)1385UL, XSOSyncStorageProvider.Tracer, "Hit Store Unhealthy exception in ResolveChange: {0}.", new object[]
						{
							ex3
						});
						flag = false;
						ex = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, ex3, true);
					}
					catch (StoragePermanentException ex4)
					{
						if (ExceptionUtilities.ShouldPermanentMapiOrXsoExceptionBeTreatedAsTransient(state2.SyncLogSession, ex4))
						{
							state2.SyncLogSession.LogError((TSLID)1396UL, XSOSyncStorageProvider.Tracer, "Hit permanent exception to be treated as transient in ResolveChange: {0}.", new object[]
							{
								ex4
							});
							ex = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, ex4);
						}
						else
						{
							state2.SyncLogSession.LogError((TSLID)1386UL, XSOSyncStorageProvider.Tracer, "Hit permanent exception in ResolveChange: {0}.", new object[]
							{
								ex4
							});
							ex = SyncPermanentException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, ex4);
						}
						flag = false;
					}
					if (syncChangeEntry.ResolvedSuccessfully && state2.UnderRecovery)
					{
						if (syncChangeEntry.ChangeType != ChangeType.Add)
						{
							if (syncChangeEntry.ChangeType != ChangeType.Change || syncChangeEntry.NewNativeFolderId == null)
							{
								goto IL_32E;
							}
						}
						try
						{
							StoragePermanentException ex5;
							if (this.HandleItemRecovery(asyncResult, state2, syncChangeEntry, out ex5))
							{
								state2.SyncLogSession.LogVerbose((TSLID)1047UL, XSOSyncStorageProvider.Tracer, "Item applied via recovery: {0}", new object[]
								{
									syncChangeEntry
								});
								continue;
							}
							if (ex5 != null)
							{
								state2.SyncLogSession.LogError((TSLID)1397UL, XSOSyncStorageProvider.Tracer, "Hit permanent exception that should be treated as transient for HandleItemRecovery: {0}.", new object[]
								{
									ex5
								});
								flag = false;
								ex = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, ex5);
							}
							else
							{
								ex = syncChangeEntry.Exception;
							}
						}
						catch (StorageTransientException ex6)
						{
							state2.SyncLogSession.LogError((TSLID)1202UL, XSOSyncStorageProvider.Tracer, "Hit transient exception in BeginApplyItem for HandleItemRecovery: {0}.", new object[]
							{
								ex6
							});
							flag = false;
							ex = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, ex6);
						}
						catch (SyncStoreUnhealthyException ex7)
						{
							state2.SyncLogSession.LogError((TSLID)1356UL, XSOSyncStorageProvider.Tracer, "Hit Store Unhealthy exception in BeginApplyItem for HandleItemRecovery: {0}.", new object[]
							{
								ex7
							});
							flag = false;
							ex = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, ex7, true);
						}
					}
					IL_32E:
					if (ex != null)
					{
						state2.SyncLogSession.LogVerbose((TSLID)1048UL, XSOSyncStorageProvider.Tracer, "Exiting ApplyItem due to exception: {0} while applying the change {1}", new object[]
						{
							ex,
							syncChangeEntry
						});
						asyncResult.SetCompletedSynchronously();
						asyncResult.ProcessCompleted(new SyncProviderResultData(state2.Changes, state2.HasPermanentSyncErrors, state2.HasTransientSyncErrors), ex);
						return;
					}
					state2.SyncLogSession.LogDebugging((TSLID)1203UL, XSOSyncStorageProvider.Tracer, "Processing change with Cloud ID: {0}", new object[]
					{
						syncChangeEntry.CloudId
					});
					if (syncChangeEntry.ChangeType == ChangeType.Delete || syncChangeEntry.ChangeType == ChangeType.SoftDelete)
					{
						state2.SyncLogSession.LogDebugging((TSLID)1204UL, XSOSyncStorageProvider.Tracer, "Writing item without needing to get it.", new object[0]);
						this.ApplyChange(state2, syncChangeEntry);
						state2.SyncLogSession.LogVerbose((TSLID)1049UL, XSOSyncStorageProvider.Tracer, "Item applied without GetItem: {0}", new object[]
						{
							syncChangeEntry
						});
						continue;
					}
					flag = false;
				}
				finally
				{
					if (flag && syncChangeEntry != null && syncChangeEntry.SyncObject != null)
					{
						syncChangeEntry.SyncObject.Dispose();
						syncChangeEntry.SyncObject = null;
					}
				}
				state2.SyncLogSession.LogDebugging((TSLID)1050UL, XSOSyncStorageProvider.Tracer, "GetItem called for: {0}", new object[]
				{
					syncChangeEntry
				});
				asyncResult.PendingAsyncResult = state2.ItemRetriever.BeginGetItem(state2.ItemRetrieverState, syncChangeEntry, new AsyncCallback(this.EndApplyItem), asyncResult, asyncResult.SyncPoisonContext);
				return;
			}
			state2.SyncLogSession.LogDebugging((TSLID)1056UL, XSOSyncStorageProvider.Tracer, "All items have been applied", new object[0]);
			state2.ItemRetriever = null;
			state2.ItemRetrieverState = null;
			state2.ItemsToProcess = null;
			asyncResult.PendingAsyncResult = null;
			asyncResult.ProcessCompleted(new SyncProviderResultData(state2.Changes, state2.HasPermanentSyncErrors, state2.HasTransientSyncErrors), ex);
		}

		private void EndApplyItem(IAsyncResult asyncResult)
		{
			AsyncResult<XSOSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<XSOSyncStorageProviderState, SyncProviderResultData>)asyncResult.AsyncState;
			XSOSyncStorageProviderState state = asyncResult2.State;
			SyncChangeEntry syncChangeEntry = null;
			AsyncOperationResult<SyncChangeEntry> asyncOperationResult = null;
			Exception ex = null;
			try
			{
				asyncOperationResult = state.ItemRetriever.EndGetItem(asyncResult);
				syncChangeEntry = asyncOperationResult.Data;
				if (!asyncOperationResult.IsSucceeded)
				{
					ex = asyncOperationResult.Exception;
					if (ex is SyncPermanentException)
					{
						state.HasPermanentSyncErrors = true;
					}
					else
					{
						state.HasTransientSyncErrors = true;
					}
					state.SyncLogSession.LogError((TSLID)1207UL, XSOSyncStorageProvider.Tracer, "Failed to get item: {0}", new object[]
					{
						ex
					});
				}
				else
				{
					if (syncChangeEntry == null || syncChangeEntry.SyncObject == null)
					{
						throw new InvalidOperationException("After calling GetItem, we should have a change with a SyncObject set.");
					}
					this.ApplyChange(state, syncChangeEntry);
				}
			}
			catch (IOException innerException)
			{
				state.HasTransientSyncErrors = true;
				ex = SyncTransientException.CreateItemLevelException(innerException);
			}
			catch (ExchangeDataException innerException2)
			{
				state.HasTransientSyncErrors = true;
				ex = SyncTransientException.CreateItemLevelException(innerException2);
			}
			catch (ADTransientException innerException3)
			{
				state.HasTransientSyncErrors = true;
				ex = SyncTransientException.CreateItemLevelException(innerException3);
			}
			catch (StorageTransientException innerException4)
			{
				state.HasTransientSyncErrors = true;
				ex = SyncTransientException.CreateItemLevelException(innerException4);
			}
			catch (SyncStoreUnhealthyException innerException5)
			{
				state.HasTransientSyncErrors = true;
				ex = SyncTransientException.CreateItemLevelException(innerException5);
			}
			catch (StoragePermanentException ex2)
			{
				if (ExceptionUtilities.ShouldPermanentMapiOrXsoExceptionBeTreatedAsTransient(state.SyncLogSession, ex2))
				{
					state.HasTransientSyncErrors = true;
					ex = SyncTransientException.CreateItemLevelException(ex2);
				}
				else
				{
					state.HasPermanentSyncErrors = true;
					ex = SyncPermanentException.CreateItemLevelException(ex2);
				}
			}
			finally
			{
				if (asyncOperationResult != null && syncChangeEntry != null)
				{
					if (asyncOperationResult.Data.Exception != null)
					{
						ex = asyncOperationResult.Data.Exception;
					}
					if (syncChangeEntry.SyncObject != null)
					{
						syncChangeEntry.SyncObject.Dispose();
						syncChangeEntry.SyncObject = null;
					}
				}
			}
			if (ex != null)
			{
				if (syncChangeEntry != null)
				{
					syncChangeEntry.Exception = ex;
					if (syncChangeEntry.SchemaType == SchemaType.Folder)
					{
						state.AddFailedFolderCreation(syncChangeEntry.CloudId, ex);
					}
				}
				state.SyncLogSession.LogError((TSLID)1208UL, XSOSyncStorageProvider.Tracer, "Hit exception while ending apply item: {0}", new object[]
				{
					syncChangeEntry
				});
			}
			else
			{
				state.SyncLogSession.LogVerbose((TSLID)1087UL, XSOSyncStorageProvider.Tracer, "Finished applying item: {0}", new object[]
				{
					syncChangeEntry
				});
			}
			this.BeginApplyItem(asyncResult2);
		}

		private void ApplyChange(XSOSyncStorageProviderState providerState, SyncChangeEntry change)
		{
			change.ApplyAttempted = true;
			Exception ex = null;
			try
			{
				if (base.TryEnsureFullParentFolderHierarchy(providerState, change))
				{
					if (change.SchemaType == SchemaType.Folder)
					{
						if (base.ContinueWithFolder(providerState, change, out ex))
						{
							this.ApplyFolderChange(providerState, change);
						}
						else
						{
							change.Exception = ex;
						}
					}
					else if (change.ChangeType == ChangeType.Add)
					{
						this.AddItem(providerState, change);
					}
					else
					{
						this.ApplyItemChange(providerState, change);
					}
				}
			}
			catch (ExchangeDataException innerException)
			{
				ex = SyncTransientException.CreateItemLevelException(innerException);
			}
			catch (StorageTransientException innerException2)
			{
				ex = SyncTransientException.CreateItemLevelException(innerException2);
			}
			catch (ObjectExistedException innerException3)
			{
				ex = SyncPermanentException.CreateItemLevelException(innerException3);
			}
			catch (ObjectNotFoundException innerException4)
			{
				ex = SyncPermanentException.CreateItemLevelException(innerException4);
			}
			catch (ObjectValidationException innerException5)
			{
				ex = SyncPermanentException.CreateItemLevelException(innerException5);
			}
			catch (ConversionFailedException innerException6)
			{
				ex = SyncPermanentException.CreateItemLevelException(innerException6);
			}
			catch (StoragePermanentException ex2)
			{
				if (ExceptionUtilities.ShouldPermanentMapiOrXsoExceptionBeTreatedAsTransient(providerState.SyncLogSession, ex2))
				{
					ex = SyncTransientException.CreateItemLevelException(ex2);
				}
				else
				{
					ex = SyncPermanentException.CreateItemLevelException(ex2);
				}
			}
			catch (SyncStoreUnhealthyException innerException7)
			{
				ex = SyncTransientException.CreateItemLevelException(innerException7);
			}
			if (ex != null)
			{
				change.Exception = ex;
			}
			Exception ex3 = null;
			try
			{
				this.UpdateMapping(providerState, change);
			}
			catch (SyncStoreUnhealthyException innerException8)
			{
				ex3 = SyncTransientException.CreateItemLevelException(innerException8);
			}
			catch (StorageTransientException innerException9)
			{
				ex3 = SyncTransientException.CreateItemLevelException(innerException9);
			}
			catch (StoragePermanentException ex4)
			{
				if (ExceptionUtilities.ShouldPermanentMapiOrXsoExceptionBeTreatedAsTransient(providerState.SyncLogSession, ex4))
				{
					ex3 = SyncTransientException.CreateItemLevelException(ex4);
				}
				else
				{
					ex3 = SyncPermanentException.CreateItemLevelException(ex4);
				}
			}
			if (ex3 != null)
			{
				providerState.SyncLogSession.LogError((TSLID)1443UL, XSOSyncStorageProvider.Tracer, "Stamping Update Mapping Exception {0} on the change: {1}.", new object[]
				{
					ex3,
					change
				});
				change.Exception = ex3;
			}
		}

		private bool ResolveChange(NativeSyncStorageProviderState providerState, SyncChangeEntry change)
		{
			if (change.ResolvedSuccessfully)
			{
				providerState.SyncLogSession.LogDebugging((TSLID)1209UL, XSOSyncStorageProvider.Tracer, "Change {0} has already been resolved successfully.", new object[]
				{
					change
				});
				return true;
			}
			string cloudId = change.CloudId;
			StoreObjectId nativeId = change.NativeId;
			bool flag = false;
			bool result;
			try
			{
				if (change.SchemaType == SchemaType.Folder && providerState.Subscription.FolderSupport != FolderSupport.FullHierarchy)
				{
					FolderSupport folderSupport = providerState.Subscription.FolderSupport;
				}
				if (!string.IsNullOrEmpty(change.NewCloudId) && change.SchemaType != SchemaType.Folder && change.ChangeType != ChangeType.Change)
				{
					throw new InvalidOperationException("We only support the use of NewCloudId in a CHANGE for a folder.");
				}
				if (change.ChangeType == ChangeType.SoftDelete)
				{
					change.NewNativeFolderId = providerState.SyncMailboxSession.MailboxSession.GetDefaultFolderId(DefaultFolderType.DeletedItems);
					string newCloudFolderId;
					string text;
					byte[] array;
					StoreObjectId storeObjectId;
					string text2;
					Dictionary<string, string> dictionary;
					if (!providerState.StateStorage.TryFindFolder(change.NewNativeFolderId, out newCloudFolderId, out text, out array, out storeObjectId, out text2, out dictionary))
					{
						throw new InvalidOperationException("Providers should not describe soft-deletes before the deleted items folder is mapped.");
					}
					change.NewCloudFolderId = newCloudFolderId;
				}
				string text3;
				byte[] changeKey;
				StoreObjectId nativeFolderId;
				Dictionary<string, string> properties;
				bool flag2;
				if (SchemaType.Folder == change.SchemaType)
				{
					string text4;
					flag2 = providerState.StateStorage.TryFindFolder(cloudId, out text3, out nativeId, out changeKey, out nativeFolderId, out text4, out properties);
				}
				else
				{
					string text4;
					flag2 = providerState.StateStorage.TryFindItem(cloudId, out text3, out nativeId, out changeKey, out nativeFolderId, out text4, out properties);
				}
				if (flag2)
				{
					change.NativeId = nativeId;
					change.NativeFolderId = nativeFolderId;
					change.ChangeKey = changeKey;
					change.Properties = properties;
				}
				if (change.NewCloudFolderId != null)
				{
					string text5;
					StoreObjectId newNativeFolderId;
					byte[] array2;
					StoreObjectId storeObjectId2;
					string text6;
					Dictionary<string, string> dictionary2;
					if (!providerState.StateStorage.TryFindFolder(change.NewCloudFolderId, out text5, out newNativeFolderId, out array2, out storeObjectId2, out text6, out dictionary2))
					{
						providerState.SyncLogSession.LogError((TSLID)1210UL, XSOSyncStorageProvider.Tracer, "NewCloudFolderId could not be mapped to any native folder ID. Change: {0}", new object[]
						{
							change
						});
						throw new InvalidOperationException("NewCloudFolderId could not be mapped to any native folder ID.");
					}
					change.NewNativeFolderId = newNativeFolderId;
					providerState.SyncLogSession.LogDebugging((TSLID)1211UL, XSOSyncStorageProvider.Tracer, "Mapped NewCloudFolderId {0} to native folder ID {1}", new object[]
					{
						change.NewCloudFolderId,
						change.NewNativeFolderId
					});
				}
				if (change.CloudFolderId == null && change.NativeFolderId == null)
				{
					if (change.SchemaType == SchemaType.Folder)
					{
						if (change.ChangeType != ChangeType.Add)
						{
							providerState.SyncLogSession.LogError((TSLID)1212UL, XSOSyncStorageProvider.Tracer, "Invalid change could not be mapped to any cloud folder or native folder ID. Change: {0}", new object[]
							{
								change
							});
							return false;
						}
						providerState.SyncLogSession.LogDebugging((TSLID)1213UL, XSOSyncStorageProvider.Tracer, "Folder could not be mapped to any cloud folder or native folder ID (expected if it's the root folder). Change: {0}", new object[]
						{
							change
						});
					}
					else
					{
						if (providerState.Subscription.FolderSupport == FolderSupport.FullHierarchy || providerState.Subscription.FolderSupport == FolderSupport.RootFoldersOnly)
						{
							providerState.SyncLogSession.LogError((TSLID)1214UL, XSOSyncStorageProvider.Tracer, "Invalid change could not be mapped to any cloud folder or native folder ID. Change: {0}", new object[]
							{
								change
							});
							return false;
						}
						DefaultFolderType defaultFolderTypeForSubscription = providerState.GetDefaultFolderTypeForSubscription();
						change.NativeFolderId = providerState.EnsureDefaultFolder(defaultFolderTypeForSubscription);
						providerState.SyncLogSession.LogDebugging((TSLID)1215UL, XSOSyncStorageProvider.Tracer, "Mapping to default folder type: {0}.", new object[]
						{
							defaultFolderTypeForSubscription
						});
						string text4;
						if (providerState.StateStorage.TryFindFolder(change.NativeFolderId, out cloudId, out text3, out changeKey, out nativeId, out text4, out properties))
						{
							change.CloudFolderId = cloudId;
						}
						else
						{
							providerState.SyncLogSession.LogDebugging((TSLID)1216UL, XSOSyncStorageProvider.Tracer, "Using default folder but could not map to a cloud folder ID. Change: {0}", new object[]
							{
								change
							});
						}
					}
				}
				else if (change.CloudFolderId != null)
				{
					string cloudFolderId = text3;
					string text4;
					if (providerState.StateStorage.TryFindFolder(change.CloudFolderId, out text3, out nativeId, out changeKey, out nativeFolderId, out text4, out properties))
					{
						if (change.NativeFolderId == null)
						{
							change.NativeFolderId = nativeId;
						}
						else if (!object.Equals(change.NativeFolderId, nativeId))
						{
							change.NewNativeFolderId = nativeId;
							change.NewCloudFolderId = change.CloudFolderId;
							change.CloudFolderId = cloudFolderId;
							providerState.SyncLogSession.LogDebugging((TSLID)1218UL, XSOSyncStorageProvider.Tracer, "Found parent folders we're moving under: {0}, {1}", new object[]
							{
								change.NewCloudFolderId,
								change.NewNativeFolderId
							});
						}
					}
					else
					{
						Exception ex;
						if (providerState.TryGetFailedFolderCreation(change.CloudFolderId, out ex) && ex is SyncTransientException)
						{
							change.Exception = SyncTransientException.CreateItemLevelException(new SyncFailedDependencyException("Failed to create parent folder hierarchy.", ex.InnerException));
							providerState.SyncLogSession.LogError((TSLID)1219UL, XSOSyncStorageProvider.Tracer, "Parent Folder missing for the Change and failing the operation with error: {0}", new object[]
							{
								change
							});
							return false;
						}
						providerState.SyncLogSession.LogVerbose((TSLID)1387UL, XSOSyncStorageProvider.Tracer, "Mapping the change to default folder for cloud folder id {0} for change {1}", new object[]
						{
							change.CloudFolderId,
							change
						});
					}
				}
				else if (change.CloudFolderId == null && change.NativeFolderId != null)
				{
					string text4;
					if (providerState.StateStorage.TryFindFolder(change.NativeFolderId, out cloudId, out text3, out changeKey, out nativeFolderId, out text4, out properties))
					{
						change.CloudFolderId = cloudId;
					}
					else
					{
						providerState.SyncLogSession.LogDebugging((TSLID)1222UL, XSOSyncStorageProvider.Tracer, "Could not find Cloud Folder ID matching the Native Folder ID. Change: {0}", new object[]
						{
							change
						});
					}
				}
				if (flag2 && change.ChangeType == ChangeType.Add)
				{
					change.ChangeType = ChangeType.Change;
					providerState.SyncLogSession.LogVerbose((TSLID)1223UL, XSOSyncStorageProvider.Tracer, "Translated ADD operation into CHANGE for existing item: {0}", new object[]
					{
						change
					});
				}
				switch (change.ChangeType)
				{
				case ChangeType.Add:
					if (change.CloudId != null && change.NativeId != null)
					{
						providerState.SyncLogSession.LogInformation((TSLID)1224UL, XSOSyncStorageProvider.Tracer, "Ignoring Add change for existing item: {0}", new object[]
						{
							change
						});
						return false;
					}
					goto IL_66E;
				case ChangeType.Change:
				case ChangeType.Delete:
				case ChangeType.ReadFlagChange:
				case ChangeType.SoftDelete:
					if (!flag2 || change.NativeId == null)
					{
						providerState.SyncLogSession.LogInformation((TSLID)1225UL, XSOSyncStorageProvider.Tracer, "Ignoring change for non-existing item: {0}", new object[]
						{
							change
						});
						return false;
					}
					goto IL_66E;
				}
				providerState.SyncLogSession.LogInformation((TSLID)1227UL, XSOSyncStorageProvider.Tracer, "Ignoring change for existing item: {0}", new object[]
				{
					change
				});
				IL_66E:
				StoreObjectId nativeId2 = change.NativeId;
				string cloudId2 = change.CloudId;
				flag = true;
				result = true;
			}
			finally
			{
				if (flag)
				{
					change.SetResolvedSuccessfully();
				}
			}
			return result;
		}

		private bool TryItemOperation(XSOSyncStorageProviderState providerState, SyncChangeEntry change, XSOSyncStorageProvider.Operation itemOperation, out StoreObjectId correctId)
		{
			return this.TryItemOperation<ObjectNotFoundException>(providerState, change, itemOperation, out correctId);
		}

		private bool TryItemOperation<TException>(XSOSyncStorageProviderState providerState, SyncChangeEntry change, XSOSyncStorageProvider.Operation itemOperation, out StoreObjectId correctItemId) where TException : Exception
		{
			return this.TryObjectOperation<TException>(providerState, change, false, itemOperation, new XSOSyncStorageProvider.TryFindObject(XSOSyncStorageProvider.TryFindItem), out correctItemId);
		}

		private bool TryItemOperation<TException>(XSOSyncStorageProviderState providerState, SyncChangeEntry change, bool useChangeKey, XSOSyncStorageProvider.Operation itemOperation, out StoreObjectId correctItemId) where TException : Exception
		{
			return this.TryObjectOperation<TException>(providerState, change, useChangeKey, itemOperation, new XSOSyncStorageProvider.TryFindObject(XSOSyncStorageProvider.TryFindItem), out correctItemId);
		}

		private bool TryFolderOperation(XSOSyncStorageProviderState providerState, SyncChangeEntry change, XSOSyncStorageProvider.Operation folderOperation, out StoreObjectId correctFolderId)
		{
			return this.TryObjectOperation<ObjectNotFoundException>(providerState, change, false, folderOperation, new XSOSyncStorageProvider.TryFindObject(this.TryFindFolder), out correctFolderId);
		}

		private bool TryObjectOperation<TException>(XSOSyncStorageProviderState providerState, SyncChangeEntry change, bool useChangeKey, XSOSyncStorageProvider.Operation objectOperation, XSOSyncStorageProvider.TryFindObject tryFindObject, out StoreObjectId correctObjectId) where TException : Exception
		{
			correctObjectId = null;
			StoreId id;
			StoreObjectId storeObjectId;
			if (change.NewNativeId != null)
			{
				storeObjectId = (id = change.NewNativeId);
			}
			else if (useChangeKey && change.ChangeKey != null)
			{
				id = VersionedId.FromStoreObjectId(change.NativeId, change.ChangeKey);
				storeObjectId = change.NativeId;
			}
			else
			{
				storeObjectId = (id = change.NativeId);
			}
			bool flag = false;
			bool result;
			try
			{
				IL_48:
				objectOperation(id);
				correctObjectId = storeObjectId;
				result = true;
			}
			catch (TException ex)
			{
				TException ex2 = (TException)((object)ex);
				bool flag2 = false;
				if (!flag)
				{
					flag = true;
					providerState.SyncLogSession.LogDebugging((TSLID)1390UL, XSOSyncStorageProvider.Tracer, "Searching for object {0}, Ignoring {1}", new object[]
					{
						change,
						ex2
					});
					VersionedId versionedId;
					flag2 = tryFindObject(providerState, change, out versionedId);
					if (flag2)
					{
						StoreObjectId objectId = versionedId.ObjectId;
						byte[] changeKey = versionedId.ChangeKeyAsByteArray();
						if (change.NativeId.Equals(storeObjectId))
						{
							change.NativeId = objectId;
						}
						else
						{
							change.NewNativeId = objectId;
						}
						change.ChangeKey = changeKey;
						id = versionedId;
						storeObjectId = objectId;
					}
				}
				if (flag2)
				{
					goto IL_48;
				}
				change.Exception = SyncPermanentException.CreateItemLevelException(ex2);
				providerState.SyncLogSession.LogVerbose((TSLID)1391UL, XSOSyncStorageProvider.Tracer, "TryObjectOperation failed for {0}", new object[]
				{
					change
				});
				result = false;
			}
			return result;
		}

		private XSOSyncContact CreateXSOSyncContact(XSOSyncStorageProviderState providerState, StoreObjectId nativeId, ChangeType changeType)
		{
			PropertyDefinition[] properties;
			switch (changeType)
			{
			case ChangeType.Add:
			case ChangeType.Change:
			case ChangeType.ReadFlagChange:
				properties = ContactPropertyManager.Instance.AllProperties;
				goto IL_2F;
			}
			properties = null;
			IL_2F:
			Item item = null;
			XSOSyncContact xsosyncContact = null;
			try
			{
				item = SyncStoreLoadManager.ItemBind(providerState.SyncMailboxSession.MailboxSession, nativeId, properties, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete));
				xsosyncContact = new XSOSyncContact(item);
			}
			finally
			{
				if (xsosyncContact == null && item != null)
				{
					item.Dispose();
					item = null;
				}
			}
			return xsosyncContact;
		}

		private const int DefaultFolderCount = 10;

		private static readonly Trace Tracer = ExTraceGlobals.XSOSyncStorageProviderTracer;

		private static readonly PropertyDefinition[] ClientOperationProperties = new PropertyDefinition[]
		{
			ItemSchema.ArticleId,
			ItemSchema.Id,
			StoreObjectSchema.ChangeKey,
			MessageItemSchema.IsRead,
			ItemSchema.ConversationId,
			ItemSchema.ConversationIndex
		};

		private static readonly PropertyDefinition[] MoveItemProperties = new PropertyDefinition[]
		{
			ItemSchema.CloudId
		};

		private static readonly PropertyDefinition[] MoveItemPropertiesStartingWithCloudIdAndId = new PropertyDefinition[]
		{
			ItemSchema.CloudId,
			ItemSchema.Id
		};

		private static readonly PropertyDefinition[] RecoveryItemPropertiesStartingWithCloudId = new PropertyDefinition[]
		{
			ItemSchema.CloudId,
			ItemSchema.Id
		};

		private delegate void Operation(StoreId id);

		private delegate bool TryFindObject(XSOSyncStorageProviderState providerState, SyncChangeEntry change, out VersionedId newObjectId);
	}
}
