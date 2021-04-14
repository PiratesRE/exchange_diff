using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Mime.Internal;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.Protocols.DeltaSync;
using Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail;
using Microsoft.Exchange.Net.Protocols.DeltaSync.StatelessResponse;
using Microsoft.Exchange.Net.Protocols.DeltaSync.SyncResponse;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Worker;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class DeltaSyncStorageProvider : ISyncStorageProvider, ISyncStorageProviderItemRetriever, ICloudStatisticsProvider
	{
		public AggregationSubscriptionType SubscriptionType
		{
			get
			{
				return AggregationSubscriptionType.DeltaSyncMail;
			}
		}

		public SyncStorageProviderState Bind(ISyncWorkerData subscription, SyncLogSession syncLogSession, bool underRecovery)
		{
			long maxDownloadSizePerConnection = (long)AggregationConfiguration.Instance.GetMaxDownloadSizePerConnection(subscription.AggregationType).ToBytes();
			int maxDownloadItemsPerConnection = AggregationConfiguration.Instance.GetMaxDownloadItemsPerConnection(subscription.AggregationType);
			return new DeltaSyncStorageProviderState(subscription, syncLogSession, underRecovery, AggregationConfiguration.Instance.ContentAggregationProxyServer, AggregationConfiguration.Instance.RemoteConnectionTimeout, new EventHandler<DownloadCompleteEventArgs>(FrameworkPerfCounterHandler.Instance.OnDeltaSyncDownloadCompletion), new EventHandler<EventArgs>(FrameworkPerfCounterHandler.Instance.OnDeltaSyncMessageDownloadCompletion), new EventHandler<EventArgs>(FrameworkPerfCounterHandler.Instance.OnDeltaSyncMessageUploadCompletion), maxDownloadItemsPerConnection, maxDownloadSizePerConnection, AggregationConfiguration.Instance.MaxDownloadSizePerItem, AggregationConfiguration.Instance.HttpProtocolLog, FrameworkAggregationConfiguration.Instance.DeltaSyncSettingsUpdateInterval, AggregationConfiguration.Instance.MaxDownloadItemsInFirstDeltaSyncConnection, DeltaSyncClientFactory.Instance);
		}

		public void Unbind(SyncStorageProviderState state)
		{
		}

		public IAsyncResult BeginCheckForChanges(SyncStorageProviderState state, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			DeltaSyncStorageProviderState deltaSyncStorageProviderState = (DeltaSyncStorageProviderState)state;
			AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData> asyncResult = new AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData>(this, deltaSyncStorageProviderState, callback, callbackState, syncPoisonContext);
			deltaSyncStorageProviderState.UpdateDeltaSyncClientWithWaterMark();
			asyncResult.PendingAsyncResult = deltaSyncStorageProviderState.DeltaSyncClient.BeginGetChanges(deltaSyncStorageProviderState.MaxEmailChangesEnumeratedInThisSync, new AsyncCallback(DeltaSyncStorageProvider.OnCheckForChangesCompleted), asyncResult, asyncResult.SyncPoisonContext);
			return asyncResult;
		}

		public AsyncOperationResult<SyncProviderResultData> EndCheckForChanges(IAsyncResult asyncResult)
		{
			AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public IAsyncResult BeginAuthenticate(SyncStorageProviderState state, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			DeltaSyncStorageProviderState deltaSyncStorageProviderState = (DeltaSyncStorageProviderState)state;
			AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData> asyncResult = new AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData>(this, deltaSyncStorageProviderState, callback, callbackState, syncPoisonContext);
			if (deltaSyncStorageProviderState.Subscription.IsMirrored && !deltaSyncStorageProviderState.HasLatestSettings)
			{
				asyncResult.PendingAsyncResult = deltaSyncStorageProviderState.DeltaSyncClient.BeginGetSettings(new AsyncCallback(DeltaSyncStorageProvider.OnGetSettingsCompleted), asyncResult, asyncResult.SyncPoisonContext);
				return asyncResult;
			}
			asyncResult.SetCompletedSynchronously();
			asyncResult.ProcessCompleted();
			return asyncResult;
		}

		public AsyncOperationResult<SyncProviderResultData> EndAuthenticate(IAsyncResult asyncResult)
		{
			AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public IAsyncResult BeginEnumerateChanges(SyncStorageProviderState state, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			DeltaSyncStorageProviderState deltaSyncStorageProviderState = (DeltaSyncStorageProviderState)state;
			AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData> asyncResult = new AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData>(this, deltaSyncStorageProviderState, callback, callbackState, syncPoisonContext);
			ExTraceGlobals.FaultInjectionTracer.TraceTest(3697683773U);
			if (deltaSyncStorageProviderState.HasFolderAndEmailCollectionCached)
			{
				DeltaSyncStorageProvider.EnumerateChangeFromCollection(deltaSyncStorageProviderState);
				asyncResult.SetCompletedSynchronously();
				DeltaSyncStorageProvider.ProcessCompleted(asyncResult, deltaSyncStorageProviderState, null);
			}
			else
			{
				deltaSyncStorageProviderState.UpdateDeltaSyncClientWithWaterMark();
				asyncResult.PendingAsyncResult = deltaSyncStorageProviderState.DeltaSyncClient.BeginGetChanges(deltaSyncStorageProviderState.MaxEmailChangesEnumeratedInThisSync, new AsyncCallback(DeltaSyncStorageProvider.OnEnumerateChangesCompleted), asyncResult, asyncResult.SyncPoisonContext);
			}
			return asyncResult;
		}

		public AsyncOperationResult<SyncProviderResultData> EndEnumerateChanges(IAsyncResult asyncResult)
		{
			AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData>)asyncResult;
			ExTraceGlobals.FaultInjectionTracer.TraceTest(2892377405U);
			return asyncResult2.WaitForCompletion();
		}

		public IAsyncResult BeginGetItem(object itemRetrieverState, SyncChangeEntry item, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			DeltaSyncStorageProviderState deltaSyncStorageProviderState = (DeltaSyncStorageProviderState)itemRetrieverState;
			AsyncResult<DeltaSyncStorageProviderState, SyncChangeEntry> asyncResult = new AsyncResult<DeltaSyncStorageProviderState, SyncChangeEntry>(this, deltaSyncStorageProviderState, callback, callbackState, syncPoisonContext);
			deltaSyncStorageProviderState.ItemBeingRetrieved = item;
			Exception exception = null;
			if (item.SchemaType == SchemaType.Email && item.ChangeType == ChangeType.Add)
			{
				DeltaSyncMail deltaSyncMail = (DeltaSyncMail)deltaSyncStorageProviderState.ItemBeingRetrieved.CloudObject;
				long num = (long)((deltaSyncStorageProviderState.BytesDownloaded.ToBytes() > 9223372036854775807UL) ? 9223372036854775807UL : deltaSyncStorageProviderState.BytesDownloaded.ToBytes());
				if ((long)deltaSyncMail.Size <= deltaSyncStorageProviderState.MaxDownloadSizePerItem)
				{
					if (num + (long)deltaSyncMail.Size < deltaSyncStorageProviderState.MaxDownloadSizePerConnection)
					{
						asyncResult.PendingAsyncResult = deltaSyncStorageProviderState.DeltaSyncClient.BeginFetchMessage(new Guid(deltaSyncStorageProviderState.ItemBeingRetrieved.CloudId), new AsyncCallback(DeltaSyncStorageProvider.OnFetchMessageCompleted), asyncResult, asyncResult.SyncPoisonContext);
						return asyncResult;
					}
					deltaSyncStorageProviderState.SyncLogSession.LogVerbose((TSLID)588UL, DeltaSyncStorageProvider.Tracer, "Skipping Delta Sync Email ({0}), will make per connection download size ({1}) overlimit ({2})", new object[]
					{
						deltaSyncStorageProviderState.ItemBeingRetrieved.CloudId,
						deltaSyncStorageProviderState.MaxDownloadSizePerConnection,
						num + (long)deltaSyncMail.Size
					});
					exception = new ConnectionDownloadedLimitExceededException();
				}
				else
				{
					deltaSyncStorageProviderState.SyncLogSession.LogVerbose((TSLID)589UL, DeltaSyncStorageProvider.Tracer, "Skipping Delta Sync Email ({0}) because it's size ({1}) is over limit ({2})", new object[]
					{
						deltaSyncStorageProviderState.ItemBeingRetrieved.CloudId,
						deltaSyncMail.Size,
						deltaSyncStorageProviderState.MaxDownloadSizePerItem
					});
					exception = new MessageSizeLimitExceededException();
				}
			}
			DeltaSyncStorageProvider.ProcessCompleted(asyncResult, deltaSyncStorageProviderState, exception);
			return asyncResult;
		}

		public AsyncOperationResult<SyncChangeEntry> EndGetItem(IAsyncResult asyncResult)
		{
			AsyncResult<DeltaSyncStorageProviderState, SyncChangeEntry> asyncResult2 = (AsyncResult<DeltaSyncStorageProviderState, SyncChangeEntry>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public IAsyncResult BeginAcknowledgeChanges(SyncStorageProviderState state, IList<SyncChangeEntry> changeList, bool hasPermanentSyncErrors, bool hasTransientSyncErrors, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			DeltaSyncStorageProviderState deltaSyncStorageProviderState = (DeltaSyncStorageProviderState)state;
			deltaSyncStorageProviderState.Changes = changeList;
			deltaSyncStorageProviderState.HasPermanentSyncErrors = hasPermanentSyncErrors;
			deltaSyncStorageProviderState.HasTransientSyncErrors = hasTransientSyncErrors;
			AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData> asyncResult = new AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData>(this, deltaSyncStorageProviderState, callback, callbackState, syncPoisonContext);
			if (!hasTransientSyncErrors)
			{
				deltaSyncStorageProviderState.UpdateWaterMark();
			}
			else
			{
				deltaSyncStorageProviderState.CloudMoreItemsAvailable = true;
				deltaSyncStorageProviderState.SyncLogSession.LogVerbose((TSLID)590UL, DeltaSyncStorageProvider.Tracer, "Watermark Update skipped due to Transient Item Level Errors.", new object[0]);
			}
			asyncResult.SetCompletedSynchronously();
			asyncResult.ProcessCompleted(SyncProviderResultData.CreateAcknowledgeChangesResult(deltaSyncStorageProviderState.Changes, deltaSyncStorageProviderState.HasPermanentSyncErrors, deltaSyncStorageProviderState.HasTransientSyncErrors, deltaSyncStorageProviderState.CloudItemsSynced, deltaSyncStorageProviderState.CloudMoreItemsAvailable));
			return asyncResult;
		}

		public AsyncOperationResult<SyncProviderResultData> EndAcknowledgeChanges(IAsyncResult asyncResult)
		{
			AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public IAsyncResult BeginApplyChanges(SyncStorageProviderState state, IList<SyncChangeEntry> changeList, ISyncStorageProviderItemRetriever itemRetriever, object itemRetrieverState, AsyncCallback engineCallback, object engineCallbackState, object syncPoisonContext)
		{
			state.Changes = changeList;
			state.ItemRetriever = itemRetriever;
			state.ItemRetrieverState = itemRetrieverState;
			AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData> asyncResult = new AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData>(this, (DeltaSyncStorageProviderState)state, engineCallback, engineCallbackState, syncPoisonContext);
			ThreadPool.QueueUserWorkItem(asyncResult.GetWaitCallbackWithPoisonContext(new WaitCallback(DeltaSyncStorageProvider.ProcessApplyChanges)), asyncResult);
			return asyncResult;
		}

		public AsyncOperationResult<SyncProviderResultData> EndApplyChanges(IAsyncResult asyncResult)
		{
			AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public void CancelGetItem(IAsyncResult asyncResult)
		{
		}

		public void Cancel(IAsyncResult asyncResult)
		{
		}

		public IAsyncResult BeginGetStatistics(SyncStorageProviderState state, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			DeltaSyncStorageProviderState deltaSyncStorageProviderState = (DeltaSyncStorageProviderState)state;
			AsyncResult<DeltaSyncStorageProviderState, CloudStatistics> asyncResult = new AsyncResult<DeltaSyncStorageProviderState, CloudStatistics>(this, deltaSyncStorageProviderState, callback, callbackState, syncPoisonContext);
			asyncResult.PendingAsyncResult = deltaSyncStorageProviderState.DeltaSyncClient.BeginGetStatistics(new AsyncCallback(DeltaSyncStorageProvider.OnGetStatisticsCompleted), asyncResult, asyncResult.SyncPoisonContext);
			return asyncResult;
		}

		public AsyncOperationResult<CloudStatistics> EndGetStatistics(IAsyncResult asyncResult)
		{
			AsyncResult<DeltaSyncStorageProviderState, CloudStatistics> asyncResult2 = (AsyncResult<DeltaSyncStorageProviderState, CloudStatistics>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		private static void OnEnumerateChangesCompleted(IAsyncResult asyncResult)
		{
			AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData>)asyncResult.AsyncState;
			DeltaSyncStorageProviderState state = asyncResult2.State;
			Exception exception;
			if (DeltaSyncStorageProvider.TryProcessOnGetChangesCompleted(asyncResult, state, out exception))
			{
				DeltaSyncStorageProvider.EnumerateChangeFromCollection(state);
			}
			DeltaSyncStorageProvider.ProcessCompleted(asyncResult2, state, exception);
		}

		private static void OnGetStatisticsCompleted(IAsyncResult asyncResult)
		{
			AsyncResult<DeltaSyncStorageProviderState, CloudStatistics> asyncResult2 = (AsyncResult<DeltaSyncStorageProviderState, CloudStatistics>)asyncResult.AsyncState;
			DeltaSyncStorageProviderState state = asyncResult2.State;
			Exception ex;
			if (!DeltaSyncStorageProvider.TryProcessGetStatisticsCompleted(asyncResult, state.DeltaSyncClient, state.SyncLogSession, state.CloudStatistics, out ex))
			{
				state.SyncLogSession.LogInformation((TSLID)592UL, DeltaSyncStorageProvider.Tracer, "Failed to get the Statistics due to exception {0}", new object[]
				{
					ex
				});
			}
			asyncResult2.ProcessCompleted(state.CloudStatistics, ex);
		}

		private static bool TryProcessOnGetChangesCompleted(IAsyncResult asyncResult, DeltaSyncStorageProviderState deltaSyncState, out Exception exception)
		{
			AsyncOperationResult<DeltaSyncResultData> asyncOperationResult = deltaSyncState.DeltaSyncClient.EndGetChanges(asyncResult);
			ExTraceGlobals.FaultInjectionTracer.TraceTest(2758159677U);
			if (!asyncOperationResult.IsSucceeded)
			{
				exception = asyncOperationResult.Exception;
			}
			else
			{
				deltaSyncState.CacheDeltaSyncServerUrl();
				DeltaSyncResultData data = asyncOperationResult.Data;
				if (!data.IsTopLevelOperationSuccessful)
				{
					exception = data.GetStatusException();
					deltaSyncState.SyncLogSession.LogError((TSLID)591UL, DeltaSyncStorageProvider.Tracer, "Delta Sync Response received with Top Level Error:{0}", new object[]
					{
						data.TopLevelStatusCode
					});
				}
				else
				{
					deltaSyncState.DeltaSyncResultData = data;
					Collection collection;
					Collection collection2;
					if (DeltaSyncResultData.TryGetFolderEmailCollections(data.SyncResponse, out collection, out collection2, out exception))
					{
						deltaSyncState.LatestFolderSyncKey = collection.SyncKey;
						deltaSyncState.LatestEmailSyncKey = collection2.SyncKey;
						int num = collection.Commands.AddCollection.Count + collection.Commands.ChangeCollection.Count + collection.Commands.DeleteCollection.Count;
						int num2 = collection2.Commands.AddCollection.Count + collection2.Commands.ChangeCollection.Count + collection2.Commands.DeleteCollection.Count;
						deltaSyncState.CloudItemsSynced = num2;
						deltaSyncState.CloudMoreItemsAvailable = (collection2.internalMoreAvailable != null);
						deltaSyncState.CacheGetChangesResults(collection, collection2);
						deltaSyncState.HasNoChangesOnCloud = (num == 0 && num2 == 0);
						return true;
					}
					deltaSyncState.CloudItemsSynced = 0;
					deltaSyncState.SyncLogSession.LogError((TSLID)609UL, DeltaSyncStorageProvider.Tracer, "TryGetFolderEmailCollections failed with error: {0}", new object[]
					{
						exception
					});
				}
			}
			return false;
		}

		private static bool TryProcessGetStatisticsCompleted(IAsyncResult asyncResult, IDeltaSyncClient deltaSyncClient, SyncLogSession syncLogSession, CloudStatistics cloudStatistics, out Exception exception)
		{
			AsyncOperationResult<DeltaSyncResultData> asyncOperationResult = deltaSyncClient.EndGetStatistics(asyncResult);
			if (!asyncOperationResult.IsSucceeded)
			{
				syncLogSession.LogError((TSLID)1089UL, DeltaSyncStorageProvider.Tracer, "Delta Sync result is not succeeded. Not collecting any cloud statistics", new object[0]);
				exception = asyncOperationResult.Exception;
				return false;
			}
			DeltaSyncResultData data = asyncOperationResult.Data;
			if (!data.IsTopLevelOperationSuccessful)
			{
				syncLogSession.LogError((TSLID)1090UL, DeltaSyncStorageProvider.Tracer, "Delta Sync Response received with Top Level Error:{0}", new object[]
				{
					data.TopLevelStatusCode
				});
				exception = data.GetStatusException();
				return false;
			}
			Stateless statelessResponse = data.StatelessResponse;
			exception = null;
			if (statelessResponse == null)
			{
				syncLogSession.LogError((TSLID)1091UL, DeltaSyncStorageProvider.Tracer, "Delta Sync stateless Response is null", new object[0]);
				return false;
			}
			if (statelessResponse.Collections == null)
			{
				syncLogSession.LogError((TSLID)1092UL, DeltaSyncStorageProvider.Tracer, "Delta Sync stateless Response collection is null", new object[0]);
				return false;
			}
			cloudStatistics.TotalItemsInSourceMailbox = new long?(0L);
			cloudStatistics.TotalSizeOfSourceMailbox = new long?(0L);
			cloudStatistics.TotalFoldersInSourceMailbox = new long?(0L);
			StatelessCollection[] collections = statelessResponse.Collections;
			int i = 0;
			while (i < collections.Length)
			{
				StatelessCollection statelessCollection = collections[i];
				bool result;
				if (statelessCollection == null)
				{
					syncLogSession.LogError((TSLID)1093UL, DeltaSyncStorageProvider.Tracer, "Delta Sync stateless response collection is null", new object[0]);
					result = false;
				}
				else if (statelessCollection.Commands == null)
				{
					syncLogSession.LogError((TSLID)1094UL, DeltaSyncStorageProvider.Tracer, "Delta Sync stateless response collection commands is null", new object[0]);
					result = false;
				}
				else
				{
					if (statelessCollection.Commands.Add != null)
					{
						foreach (StatelessCollectionCommandsAdd statelessCollectionCommandsAdd in statelessCollection.Commands.Add)
						{
							if (statelessCollectionCommandsAdd == null)
							{
								syncLogSession.LogError((TSLID)1096UL, DeltaSyncStorageProvider.Tracer, "Delta Sync stateless response add collection command is null", new object[0]);
								return false;
							}
							ApplicationDataTypeResponse applicationData = statelessCollectionCommandsAdd.ApplicationData;
							if (applicationData == null)
							{
								syncLogSession.LogError((TSLID)1097UL, DeltaSyncStorageProvider.Tracer, "ApplicationDataTypeResponse is null", new object[0]);
								return false;
							}
							if (applicationData.Items == null)
							{
								syncLogSession.LogError((TSLID)1098UL, DeltaSyncStorageProvider.Tracer, "ApplicationDataTypeResponse items is null", new object[0]);
								return false;
							}
							if (applicationData.ItemsElementName == null)
							{
								syncLogSession.LogError((TSLID)1099UL, DeltaSyncStorageProvider.Tracer, "ApplicationDataTypeResponse item element name is null", new object[0]);
								return false;
							}
							cloudStatistics.TotalFoldersInSourceMailbox += 1L;
							for (int k = 0; k < applicationData.Items.Length; k++)
							{
								if (applicationData.Items[k] == null)
								{
									syncLogSession.LogError((TSLID)1100UL, DeltaSyncStorageProvider.Tracer, "ApplicationDataTypeResponse items is null", new object[0]);
									return false;
								}
								if (applicationData.ItemsElementName[k] == ItemsChoiceType1.TotalMessageCount)
								{
									string text = applicationData.Items[k].ToString();
									long num;
									if (!long.TryParse(text, out num))
									{
										syncLogSession.LogError((TSLID)1101UL, DeltaSyncStorageProvider.Tracer, "Couldn't parse the message count {0} to a long ", new object[]
										{
											text
										});
										return false;
									}
									syncLogSession.LogInformation((TSLID)1102UL, DeltaSyncStorageProvider.Tracer, "Total Message count is {0}", new object[]
									{
										text
									});
									cloudStatistics.TotalItemsInSourceMailbox += num;
								}
								else if (applicationData.ItemsElementName[k] == ItemsChoiceType1.Size)
								{
									string text = applicationData.Items[k].ToString();
									long num;
									if (!long.TryParse(text, out num))
									{
										syncLogSession.LogError((TSLID)1103UL, DeltaSyncStorageProvider.Tracer, "Couldn't parse the folder size {0} to a long ", new object[]
										{
											text
										});
										return false;
									}
									syncLogSession.LogInformation((TSLID)1104UL, DeltaSyncStorageProvider.Tracer, "Total Folder Size is {0}", new object[]
									{
										text
									});
									cloudStatistics.TotalSizeOfSourceMailbox += num;
								}
							}
						}
						i++;
						continue;
					}
					syncLogSession.LogError((TSLID)1095UL, DeltaSyncStorageProvider.Tracer, "Delta Sync stateless response add command collection is null", new object[0]);
					result = false;
				}
				return result;
			}
			syncLogSession.LogInformation((TSLID)1105UL, DeltaSyncStorageProvider.Tracer, "Following Statistics are collected. Total Item count: {0} Total Folder count: {1} Total Mailbox Size: {2}", new object[]
			{
				cloudStatistics.TotalItemsInSourceMailbox,
				cloudStatistics.TotalFoldersInSourceMailbox,
				cloudStatistics.TotalSizeOfSourceMailbox
			});
			return true;
		}

		private static void EnumerateChangeFromCollection(DeltaSyncStorageProviderState deltaSyncState)
		{
			ISyncSourceSession deltaSyncState2 = deltaSyncState;
			Collection cachedFolderCollection = deltaSyncState.CachedFolderCollection;
			Collection cachedEmailCollection = deltaSyncState.CachedEmailCollection;
			int num = cachedFolderCollection.Commands.AddCollection.Count + cachedFolderCollection.Commands.ChangeCollection.Count + cachedFolderCollection.Commands.DeleteCollection.Count;
			int num2 = cachedEmailCollection.Commands.AddCollection.Count + cachedEmailCollection.Commands.ChangeCollection.Count + cachedEmailCollection.Commands.DeleteCollection.Count;
			deltaSyncState.Changes = new List<SyncChangeEntry>(num + num2);
			deltaSyncState.SyncLogSession.LogInformation((TSLID)297UL, DeltaSyncStorageProvider.Tracer, "FolderCollection::[AddCount:{0}],[ChangeCount:{1}],[DeleteCount:{2}]", new object[]
			{
				cachedFolderCollection.Commands.AddCollection.Count,
				cachedFolderCollection.Commands.ChangeCollection.Count,
				cachedFolderCollection.Commands.DeleteCollection.Count
			});
			string text = "00000000-0000-0000-0000-000000000000";
			if (!deltaSyncState.StateStorage.ContainsFolder(text))
			{
				deltaSyncState.SyncLogSession.LogDebugging((TSLID)593UL, DeltaSyncStorageProvider.Tracer, "Adding Default Root Folder Entry with CloudId: {0}", new object[]
				{
					text
				});
				SyncChangeEntry syncChangeEntry = new SyncChangeEntry(ChangeType.Add, SchemaType.Folder, text);
				syncChangeEntry.CloudFolderId = null;
				syncChangeEntry.SyncObject = new SyncFolder(string.Empty, DeltaSyncStorageProvider.GetDefaultFolderType(text));
				deltaSyncState.Add(syncChangeEntry);
			}
			foreach (object obj in cachedFolderCollection.Commands.AddCollection)
			{
				Add add = (Add)obj;
				text = add.ServerId.ToLowerInvariant();
				string text2 = add.ApplicationData.ParentId.Value.ToLowerInvariant();
				deltaSyncState.SyncLogSession.LogDebugging((TSLID)594UL, DeltaSyncStorageProvider.Tracer, "Add Folder: {0} Parent: {1}", new object[]
				{
					text,
					text2
				});
				if (deltaSyncState.StateStorage.ContainsFolder(text))
				{
					deltaSyncState.SyncLogSession.LogVerbose((TSLID)595UL, DeltaSyncStorageProvider.Tracer, "Skipping Folder since it already exists, cloudId: {0}", new object[]
					{
						text
					});
				}
				else
				{
					string text3 = DeltaSyncResultData.DecodeValue(add.ApplicationData.DisplayName);
					if (DeltaSyncFolder.IsSystemFolder(text3))
					{
						deltaSyncState.SyncLogSession.LogDebugging((TSLID)596UL, DeltaSyncStorageProvider.Tracer, "System Folder ({0}) shouldn't be synced", new object[]
						{
							text3
						});
					}
					else
					{
						DefaultFolderType defaultFolderType = DeltaSyncStorageProvider.GetDefaultFolderType(text);
						SyncChangeEntry syncChangeEntry = new SyncChangeEntry(ChangeType.Add, SchemaType.Folder, text);
						syncChangeEntry.CloudFolderId = text2;
						syncChangeEntry.SyncObject = new SyncFolder(text3, defaultFolderType);
						deltaSyncState.Add(syncChangeEntry);
					}
				}
			}
			List<SyncChangeEntry> changes = null;
			List<SyncChangeEntry> list = null;
			ChangeListSorter changeListSorter = new ChangeListSorter((SyncChangeEntry changeEntry) => deltaSyncState.StateStorage.ContainsFolder(changeEntry.CloudFolderId));
			changeListSorter.Run(deltaSyncState.Changes, out changes, out list);
			deltaSyncState.Changes = changes;
			foreach (SyncChangeEntry syncChangeEntry2 in list)
			{
				deltaSyncState.SyncLogSession.LogError((TSLID)1378UL, DeltaSyncStorageProvider.Tracer, "Invalid Parent Heirarchy for the folder change: {0}.", new object[]
				{
					syncChangeEntry2
				});
				deltaSyncState.Add(syncChangeEntry2);
			}
			foreach (object obj2 in cachedFolderCollection.Commands.ChangeCollection)
			{
				Change change = (Change)obj2;
				text = change.ServerId.ToLowerInvariant();
				deltaSyncState.SyncLogSession.LogDebugging((TSLID)597UL, DeltaSyncStorageProvider.Tracer, "Change folder: {0}", new object[]
				{
					text
				});
				if (DeltaSyncFolder.IsSystemFolder(change.ApplicationData.DisplayName.Value))
				{
					deltaSyncState.SyncLogSession.LogVerbose((TSLID)598UL, DeltaSyncStorageProvider.Tracer, "System Folder ({0}) shouldn't be synced", new object[]
					{
						change.ApplicationData.DisplayName.Value
					});
				}
				else
				{
					DefaultFolderType defaultFolderType2 = DeltaSyncStorageProvider.GetDefaultFolderType(text);
					SyncChangeEntry syncChangeEntry;
					if (deltaSyncState.StateStorage.ContainsFolder(text))
					{
						syncChangeEntry = new SyncChangeEntry(ChangeType.Change, SchemaType.Folder, text);
					}
					else
					{
						deltaSyncState.SyncLogSession.LogVerbose((TSLID)599UL, DeltaSyncStorageProvider.Tracer, "Converting Change folder entry to an Add: {0}", new object[]
						{
							text
						});
						syncChangeEntry = new SyncChangeEntry(ChangeType.Add, SchemaType.Folder, text);
					}
					syncChangeEntry.SyncObject = new SyncFolder(change.ApplicationData.DisplayName.Value, defaultFolderType2);
					syncChangeEntry.CloudFolderId = change.ApplicationData.ParentId.Value.ToLowerInvariant();
					deltaSyncState.Add(syncChangeEntry);
				}
			}
			foreach (object obj3 in cachedFolderCollection.Commands.DeleteCollection)
			{
				Delete delete = (Delete)obj3;
				text = delete.ServerId.ToLowerInvariant();
				deltaSyncState.SyncLogSession.LogDebugging((TSLID)600UL, DeltaSyncStorageProvider.Tracer, "Soft Delete Folder: {0}", new object[]
				{
					text
				});
				deltaSyncState.Add(new SyncChangeEntry(ChangeType.SoftDelete, SchemaType.Folder, text));
			}
			deltaSyncState.SyncLogSession.LogInformation((TSLID)601UL, DeltaSyncStorageProvider.Tracer, "EmailCollection::[AddCount:{0}],[ChangeCount:{1}],[DeleteCount:{2}]", new object[]
			{
				cachedEmailCollection.Commands.AddCollection.Count,
				cachedEmailCollection.Commands.ChangeCollection.Count,
				cachedEmailCollection.Commands.DeleteCollection.Count
			});
			foreach (object obj4 in cachedEmailCollection.Commands.AddCollection)
			{
				Add add2 = (Add)obj4;
				text = add2.ServerId.ToLowerInvariant();
				deltaSyncState.SyncLogSession.LogDebugging((TSLID)602UL, DeltaSyncStorageProvider.Tracer, "Add Email: {0}", new object[]
				{
					text
				});
				if (deltaSyncState.StateStorage.ContainsItem(text))
				{
					deltaSyncState.SyncLogSession.LogVerbose((TSLID)603UL, DeltaSyncStorageProvider.Tracer, "Skipping Email since it already exists, cloudId: {0}", new object[]
					{
						text
					});
				}
				else if (deltaSyncState.StateStorage.ContainsFailedItem(text))
				{
					deltaSyncState.SyncLogSession.LogVerbose((TSLID)604UL, DeltaSyncStorageProvider.Tracer, "Skipping Email since we already failed on it, cloudId: {0}", new object[]
					{
						text
					});
				}
				else
				{
					DeltaSyncMail deltaSyncMail = new DeltaSyncMail(new Guid(text));
					DeltaSyncStorageProvider.LoadDeltaSyncMail(ref deltaSyncMail, add2.ApplicationData, deltaSyncState);
					SyncChangeEntry syncChangeEntry = new SyncChangeEntry(ChangeType.Add, SchemaType.Email, text, deltaSyncMail);
					syncChangeEntry.CloudFolderId = deltaSyncMail.Parent.ServerId.ToString().ToLowerInvariant();
					deltaSyncState.Add(syncChangeEntry);
					syncChangeEntry.SyncReportObject = new DeltaSyncReportObject(deltaSyncMail);
				}
			}
			foreach (object obj5 in cachedEmailCollection.Commands.ChangeCollection)
			{
				Change change2 = (Change)obj5;
				text = change2.ServerId.ToLowerInvariant();
				deltaSyncState.SyncLogSession.LogDebugging((TSLID)605UL, DeltaSyncStorageProvider.Tracer, "Change Email: {0}", new object[]
				{
					text
				});
				DeltaSyncMail deltaSyncMail = new DeltaSyncMail(new Guid(text));
				DeltaSyncStorageProvider.LoadDeltaSyncMail(ref deltaSyncMail, change2.ApplicationData, deltaSyncState);
				SyncChangeEntry syncChangeEntry;
				if (deltaSyncState.StateStorage.ContainsItem(text))
				{
					syncChangeEntry = new SyncChangeEntry(ChangeType.Change, SchemaType.Email, text);
					syncChangeEntry.SyncObject = new DeltaSyncEmail(deltaSyncState2, deltaSyncMail);
				}
				else
				{
					if (deltaSyncState.StateStorage.ContainsFailedItem(text))
					{
						deltaSyncState.SyncLogSession.LogVerbose((TSLID)607UL, DeltaSyncStorageProvider.Tracer, "Skipping Email Change since we already failed permanently while adding it, cloudId: {0}", new object[]
						{
							text
						});
						continue;
					}
					syncChangeEntry = new SyncChangeEntry(ChangeType.Add, SchemaType.Email, text);
					syncChangeEntry.CloudObject = deltaSyncMail;
					deltaSyncState.SyncLogSession.LogVerbose((TSLID)606UL, DeltaSyncStorageProvider.Tracer, "Converting Change email entry to an Add: {0}", new object[]
					{
						text
					});
				}
				syncChangeEntry.CloudFolderId = deltaSyncMail.Parent.ServerId.ToString().ToLowerInvariant();
				deltaSyncState.Add(syncChangeEntry);
				syncChangeEntry.SyncReportObject = new DeltaSyncReportObject(deltaSyncMail);
			}
			foreach (object obj6 in cachedEmailCollection.Commands.DeleteCollection)
			{
				Delete delete2 = (Delete)obj6;
				text = delete2.ServerId.ToLowerInvariant();
				deltaSyncState.SyncLogSession.LogDebugging((TSLID)608UL, DeltaSyncStorageProvider.Tracer, "Delete Email: {0}", new object[]
				{
					text
				});
				deltaSyncState.Add(new SyncChangeEntry(ChangeType.Delete, SchemaType.Email, text));
			}
		}

		private static void OnCheckForChangesCompleted(IAsyncResult asyncResult)
		{
			AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData>)asyncResult.AsyncState;
			DeltaSyncStorageProviderState state = asyncResult2.State;
			Exception exception;
			DeltaSyncStorageProvider.TryProcessOnGetChangesCompleted(asyncResult, state, out exception);
			state.Changes = new List<SyncChangeEntry>(0);
			DeltaSyncStorageProvider.ProcessCompleted(asyncResult2, state, exception);
		}

		private static void OnFetchMessageCompleted(IAsyncResult asyncResult)
		{
			AsyncResult<DeltaSyncStorageProviderState, SyncChangeEntry> asyncResult2 = (AsyncResult<DeltaSyncStorageProviderState, SyncChangeEntry>)asyncResult.AsyncState;
			DeltaSyncStorageProviderState state = asyncResult2.State;
			ISyncSourceSession sourceSession = state;
			AsyncOperationResult<DeltaSyncResultData> asyncOperationResult = state.DeltaSyncClient.EndFetchMessage(asyncResult);
			Exception ex;
			if (!asyncOperationResult.IsSucceeded)
			{
				ex = asyncOperationResult.Exception;
			}
			else
			{
				state.CacheDeltaSyncServerUrl();
				DeltaSyncResultData data = asyncOperationResult.Data;
				if (!data.IsTopLevelOperationSuccessful)
				{
					ex = data.GetStatusException();
					state.SyncLogSession.LogError((TSLID)610UL, DeltaSyncStorageProvider.Tracer, "Delta Sync Response received with Top Level Error:{0}", new object[]
					{
						data.TopLevelStatusCode
					});
				}
				else
				{
					state.DeltaSyncResultData = data;
					Stream emailMessage;
					if (DeltaSyncResultData.TryGetMessageStream(data.ItemOperationsResponse, out emailMessage, out ex))
					{
						DeltaSyncMail deltaSyncMail = (DeltaSyncMail)state.ItemBeingRetrieved.CloudObject;
						deltaSyncMail.EmailMessage = emailMessage;
						state.ItemBeingRetrieved.SyncObject = new DeltaSyncEmail(sourceSession, deltaSyncMail);
						state.TriggerMessageDownloadedEvent(state, null);
						state.SyncLogSession.LogDebugging((TSLID)611UL, DeltaSyncStorageProvider.Tracer, "Item Retrieved successfully: {0}", new object[]
						{
							state.ItemBeingRetrieved
						});
					}
					else
					{
						state.SyncLogSession.LogError((TSLID)612UL, DeltaSyncStorageProvider.Tracer, "TryGetMessageStream failed with error: {0}", new object[]
						{
							ex
						});
					}
				}
			}
			DeltaSyncStorageProvider.ProcessCompleted(asyncResult2, state, ex);
		}

		private static void OnApplyChangesCompleted(IAsyncResult asyncResult)
		{
			AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData>)asyncResult.AsyncState;
			DeltaSyncStorageProviderState state = asyncResult2.State;
			AsyncOperationResult<DeltaSyncResultData> asyncOperationResult = state.DeltaSyncClient.EndApplyChanges(asyncResult);
			Exception ex = null;
			if (!asyncOperationResult.IsSucceeded)
			{
				ex = asyncOperationResult.Exception;
			}
			else
			{
				state.CacheDeltaSyncServerUrl();
				DeltaSyncResultData data = asyncOperationResult.Data;
				if (!data.IsTopLevelOperationSuccessful)
				{
					ex = data.GetStatusException();
					state.SyncLogSession.LogError((TSLID)613UL, DeltaSyncStorageProvider.Tracer, "Delta Sync Response received with Top Level Error:{0}", new object[]
					{
						data.TopLevelStatusCode
					});
				}
				else
				{
					state.DeltaSyncResultData = data;
					Collection collection = null;
					Collection collection2 = null;
					SyncChangeEntry syncChangeEntry = null;
					if (DeltaSyncResultData.TryGetFolderEmailCollections(data.SyncResponse, out collection, out collection2, out ex))
					{
						state.LatestFolderSyncKey = collection.SyncKey;
						state.LatestEmailSyncKey = collection2.SyncKey;
						state.SyncLogSession.LogInformation((TSLID)614UL, DeltaSyncStorageProvider.Tracer, "FolderCollection::[AddCount:{0}],[ChangeCount:{1}],[DelteCount:{2}]", new object[]
						{
							collection.Responses.AddCollection.Count,
							collection.Responses.ChangeCollection.Count,
							collection.Responses.DeleteCollection.Count
						});
						foreach (object obj in collection.Responses.AddCollection)
						{
							ResponsesAdd responsesAdd = (ResponsesAdd)obj;
							if (!state.FolderAddList.TryGetValue(responsesAdd.ClientId, out syncChangeEntry))
							{
								state.SyncLogSession.LogError((TSLID)615UL, DeltaSyncStorageProvider.Tracer, "Unknown Client Id: {0} found in the response, skipping entry.", new object[]
								{
									responsesAdd.ClientId
								});
							}
							else
							{
								if (responsesAdd.Status == 1)
								{
									string text = responsesAdd.ServerId.ToLowerInvariant();
									state.SyncLogSession.LogDebugging((TSLID)616UL, DeltaSyncStorageProvider.Tracer, "Folder Added successfully: {0}", new object[]
									{
										text
									});
									syncChangeEntry.CloudId = text;
								}
								else
								{
									state.SyncLogSession.LogError((TSLID)617UL, DeltaSyncStorageProvider.Tracer, "Folder Add Failed for:{0}, Status Code:{1}", new object[]
									{
										responsesAdd.ClientId,
										responsesAdd.Status
									});
									Exception ex2 = DeltaSyncResultData.GetStatusException(responsesAdd.Status);
									state.UpdateSyncChangeEntry(syncChangeEntry, ex2);
								}
								syncChangeEntry.SyncObject.Dispose();
								syncChangeEntry.SyncObject = null;
								state.FolderAddList.Remove(responsesAdd.ClientId);
							}
						}
						foreach (SyncChangeEntry syncEntry in state.FolderAddList.Values)
						{
							state.UpdateSyncChangeEntry(syncEntry, new MissingServerResponseException());
						}
						foreach (object obj2 in collection.Responses.ChangeCollection)
						{
							ResponsesChange responsesChange = (ResponsesChange)obj2;
							string text = responsesChange.ServerId.ToLowerInvariant();
							if (!state.ChangeList.TryGetValue(text, out syncChangeEntry))
							{
								state.SyncLogSession.LogError((TSLID)618UL, DeltaSyncStorageProvider.Tracer, "Unknown Folder Change Entry found with Server Id: {0}, skipping it.", new object[]
								{
									text
								});
							}
							else if (responsesChange.Status != 1)
							{
								state.SyncLogSession.LogError((TSLID)619UL, DeltaSyncStorageProvider.Tracer, "Folder Change Failed CloudId:{0}, Status Code:{1}", new object[]
								{
									text,
									responsesChange.Status
								});
								Exception ex2 = DeltaSyncResultData.GetStatusException(responsesChange.Status);
								DataOutOfSyncException ex3 = ex2 as DataOutOfSyncException;
								if (ex3 != null && ex3.StatusCode == 4404)
								{
									ex2 = new SyncConflictException(ex3);
								}
								state.UpdateSyncChangeEntry(syncChangeEntry, ex2);
							}
							else
							{
								state.SyncLogSession.LogDebugging((TSLID)620UL, DeltaSyncStorageProvider.Tracer, "Folder Changed successfully: {0}", new object[]
								{
									text
								});
							}
						}
						state.SyncLogSession.LogInformation((TSLID)621UL, DeltaSyncStorageProvider.Tracer, "EmailCollection::[AddCount:{0}],[ChangeCount:{1}],[DelteCount:{2}]", new object[]
						{
							collection2.Responses.AddCollection.Count,
							collection2.Responses.ChangeCollection.Count,
							collection2.Responses.DeleteCollection.Count
						});
						foreach (object obj3 in collection2.Responses.ChangeCollection)
						{
							ResponsesChange responsesChange2 = (ResponsesChange)obj3;
							string text = responsesChange2.ServerId.ToLowerInvariant();
							if (!state.ChangeList.TryGetValue(text, out syncChangeEntry))
							{
								state.SyncLogSession.LogError((TSLID)622UL, DeltaSyncStorageProvider.Tracer, "Unknown Email Change Entry found wirh Server Id: {0}, skipping it.", new object[]
								{
									text
								});
							}
							else if (responsesChange2.Status != 1)
							{
								state.SyncLogSession.LogError((TSLID)623UL, DeltaSyncStorageProvider.Tracer, "Email Change Failed CloudId:{0}, Status Code:{1}", new object[]
								{
									text,
									responsesChange2.Status
								});
								Exception ex2 = DeltaSyncResultData.GetStatusException(responsesChange2.Status);
								DataOutOfSyncException ex4 = ex2 as DataOutOfSyncException;
								if (ex4 != null && ex4.StatusCode == 4404)
								{
									ex2 = new SyncConflictException(ex4);
								}
								state.UpdateSyncChangeEntry(syncChangeEntry, ex2);
							}
							else
							{
								state.SyncLogSession.LogDebugging((TSLID)624UL, DeltaSyncStorageProvider.Tracer, "Email Changed successfully: {0}", new object[]
								{
									text
								});
							}
						}
						foreach (object obj4 in collection2.Responses.AddCollection)
						{
							ResponsesAdd responsesAdd2 = (ResponsesAdd)obj4;
							if (!state.EmailIdMapping.TryGetValue(responsesAdd2.ClientId, out syncChangeEntry))
							{
								state.SyncLogSession.LogError((TSLID)625UL, DeltaSyncStorageProvider.Tracer, "Unknown Client Id: {0} found in the response, skipping entry.", new object[]
								{
									responsesAdd2.ClientId
								});
							}
							else
							{
								if (responsesAdd2.Status == 1)
								{
									string text = responsesAdd2.ServerId.ToLowerInvariant();
									state.SyncLogSession.LogDebugging((TSLID)626UL, DeltaSyncStorageProvider.Tracer, "Email Added successfully: {0}", new object[]
									{
										text
									});
									syncChangeEntry.CloudId = text;
									state.TriggerMessageUploadedEvent(state, null);
								}
								else
								{
									state.SyncLogSession.LogError((TSLID)627UL, DeltaSyncStorageProvider.Tracer, "Email Add Failed for:{0}, Status Code:{1}", new object[]
									{
										responsesAdd2.ClientId,
										responsesAdd2.Status
									});
									Exception ex2 = DeltaSyncResultData.GetStatusException(responsesAdd2.Status);
									state.UpdateSyncChangeEntry(syncChangeEntry, ex2);
								}
								syncChangeEntry.SyncObject.Dispose();
								syncChangeEntry.SyncObject = null;
								state.EmailIdMapping.Remove(responsesAdd2.ClientId);
							}
						}
						using (Dictionary<string, SyncChangeEntry>.ValueCollection.Enumerator enumerator6 = state.EmailIdMapping.Values.GetEnumerator())
						{
							while (enumerator6.MoveNext())
							{
								SyncChangeEntry syncEntry2 = enumerator6.Current;
								state.UpdateSyncChangeEntry(syncEntry2, new MissingServerResponseException());
							}
							goto IL_813;
						}
					}
					state.SyncLogSession.LogError((TSLID)628UL, DeltaSyncStorageProvider.Tracer, "TryGetFolderEmailCollections failed with error: {0}", new object[]
					{
						ex
					});
				}
			}
			IL_813:
			if (ex != null)
			{
				if (ex is SettingsViolationException)
				{
					state.SyncLogSession.LogVerbose((TSLID)629UL, DeltaSyncStorageProvider.Tracer, "We failed with Settings Violation error, lets get the last settings in the next sync.", new object[0]);
					state.ForceGetSettingsInNextSync();
				}
				DeltaSyncStorageProvider.ProcessCompleted(asyncResult2, state, ex);
				return;
			}
			state.UpdateWaterMark();
			DeltaSyncStorageProvider.ProcessCompleted(asyncResult2, state, null);
		}

		private static DefaultFolderType GetDefaultFolderType(string serverId)
		{
			string key;
			switch (key = serverId.ToLowerInvariant())
			{
			case "00000000-0000-0000-0000-000000000000":
				return DefaultFolderType.Root;
			case "00000000-0000-0000-0000-000000000001":
				return DefaultFolderType.Inbox;
			case "00000000-0000-0000-0000-000000000002":
				return DefaultFolderType.DeletedItems;
			case "00000000-0000-0000-0000-000000000003":
				return DefaultFolderType.SentItems;
			case "00000000-0000-0000-0000-000000000004":
				return DefaultFolderType.Drafts;
			case "00000000-0000-0000-0000-000000000005":
				return DefaultFolderType.JunkEmail;
			}
			return DefaultFolderType.None;
		}

		private static void OnGetSettingsCompleted(IAsyncResult asyncResult)
		{
			AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData>)asyncResult.AsyncState;
			DeltaSyncStorageProviderState state = asyncResult2.State;
			AsyncOperationResult<DeltaSyncResultData> asyncOperationResult = state.DeltaSyncClient.EndGetSettings(asyncResult);
			Exception ex = null;
			if (!asyncOperationResult.IsSucceeded)
			{
				ex = asyncOperationResult.Exception;
			}
			else
			{
				state.CacheDeltaSyncServerUrl();
				DeltaSyncResultData data = asyncOperationResult.Data;
				if (!data.IsTopLevelOperationSuccessful)
				{
					ex = data.GetStatusException();
					state.SyncLogSession.LogError((TSLID)630UL, DeltaSyncStorageProvider.Tracer, "Delta Sync Response received with Top Level Error:{0}", new object[]
					{
						data.TopLevelStatusCode
					});
				}
				else
				{
					state.DeltaSyncResultData = data;
					DeltaSyncSettings deltaSyncSettings = null;
					if (DeltaSyncResultData.TryGetSettings(data.SettingsResponse, out deltaSyncSettings, out ex))
					{
						state.SyncLogSession.LogDebugging((TSLID)631UL, DeltaSyncStorageProvider.Tracer, "Settings Retrieved successfully", new object[0]);
						state.UpdateSubscriptionSettings(deltaSyncSettings);
					}
					else
					{
						state.SyncLogSession.LogError((TSLID)632UL, DeltaSyncStorageProvider.Tracer, "TryGetSettings failed with error: {0}", new object[]
						{
							ex
						});
					}
				}
			}
			if (ex == null)
			{
				state.RecordSettingsSyncTime();
			}
			DeltaSyncStorageProvider.ProcessCompleted(asyncResult2, state, ex);
		}

		private static void OnItemRetrieved(IAsyncResult asyncResult)
		{
			AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData>)asyncResult.AsyncState;
			DeltaSyncStorageProviderState state = asyncResult2.State;
			AsyncOperationResult<SyncChangeEntry> asyncOperationResult = state.ItemRetriever.EndGetItem(asyncResult);
			if (!asyncOperationResult.IsSucceeded)
			{
				SyncChangeEntry syncChangeEntry = state.EmailAddList[state.EmailIndex];
				state.UpdateSyncChangeEntry(syncChangeEntry, asyncOperationResult.Exception);
				state.SyncLogSession.LogError((TSLID)633UL, DeltaSyncStorageProvider.Tracer, "Failed to get item: {0}", new object[]
				{
					asyncOperationResult.Exception
				});
			}
			else
			{
				SyncChangeEntry syncChangeEntry = asyncOperationResult.Data;
				DeltaSyncMail deltaSyncMail = new DeltaSyncMail(Guid.NewGuid().ToString());
				if (DeltaSyncStorageProvider.TryLoadParentFolder(syncChangeEntry, deltaSyncMail, state))
				{
					ISyncEmail syncEmail = (ISyncEmail)syncChangeEntry.SyncObject;
					if (DeltaSyncMail.IsSupportedMessageClass(syncEmail.MessageClass))
					{
						DeltaSyncStorageProvider.LoadDeltaSyncObject(deltaSyncMail, syncChangeEntry);
						if ((long)deltaSyncMail.Size <= state.MaxMessageSize)
						{
							state.EmailIdMapping.Add(deltaSyncMail.ClientId, syncChangeEntry);
							state.EmailAddList[state.EmailIndex].SyncObject = syncChangeEntry.SyncObject;
							state.DeltaSyncOperations.Add(new DeltaSyncOperation(DeltaSyncOperation.Type.Add, deltaSyncMail));
						}
						else
						{
							state.SyncLogSession.LogVerbose((TSLID)634UL, DeltaSyncStorageProvider.Tracer, "Skipping Email ({0}) because it's size ({1}) was over limit ({2})", new object[]
							{
								syncChangeEntry.NativeId.ToBase64String(),
								deltaSyncMail.Size,
								state.MaxMessageSize
							});
							state.UpdateSyncChangeEntry(syncChangeEntry, new SettingsViolationException(4309));
						}
					}
					else
					{
						state.SyncLogSession.LogInformation((TSLID)635UL, DeltaSyncStorageProvider.Tracer, "Message Class ({0}) not supported by Delta Sync, silently skipping entry: {1}", new object[]
						{
							deltaSyncMail.MessageClass,
							syncChangeEntry
						});
						syncChangeEntry.Persist = false;
					}
				}
			}
			DeltaSyncStorageProvider.LoadItems(asyncResult2);
		}

		private static void BeginGetSettings(AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData> syncOperationAsyncResult)
		{
			DeltaSyncStorageProviderState state = syncOperationAsyncResult.State;
			state.SyncLogSession.LogDebugging((TSLID)636UL, DeltaSyncStorageProvider.Tracer, "Begin Get Settings", new object[0]);
			syncOperationAsyncResult.PendingAsyncResult = state.DeltaSyncClient.BeginGetSettings(new AsyncCallback(DeltaSyncStorageProvider.OnGetSettingsCompleted), syncOperationAsyncResult, syncOperationAsyncResult.SyncPoisonContext);
		}

		private static void ApplyChanges(AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData> syncOperationAsyncResult)
		{
			DeltaSyncStorageProviderState state = syncOperationAsyncResult.State;
			state.UpdateDeltaSyncClientWithWaterMark();
			syncOperationAsyncResult.PendingAsyncResult = state.DeltaSyncClient.BeginApplyChanges(state.DeltaSyncOperations, DeltaSyncStorageProviderState.ConflictResolution, new AsyncCallback(DeltaSyncStorageProvider.OnApplyChangesCompleted), syncOperationAsyncResult, syncOperationAsyncResult.SyncPoisonContext);
		}

		private static void LoadItems(AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData> syncOperationAsyncResult)
		{
			DeltaSyncStorageProviderState state = syncOperationAsyncResult.State;
			state.EmailIndex++;
			if (state.EmailIndex < state.EmailAddList.Count)
			{
				SyncChangeEntry item = state.EmailAddList[state.EmailIndex];
				syncOperationAsyncResult.PendingAsyncResult = state.ItemRetriever.BeginGetItem(state.ItemRetrieverState, item, new AsyncCallback(DeltaSyncStorageProvider.OnItemRetrieved), syncOperationAsyncResult, syncOperationAsyncResult.SyncPoisonContext);
				return;
			}
			state.ChangesLoaded = true;
			DeltaSyncStorageProvider.ProcessApplyChanges(syncOperationAsyncResult);
		}

		private static void LoadDeltaSyncMail(ref DeltaSyncMail deltaSyncMail, ApplicationData emailData, DeltaSyncStorageProviderState deltaSyncState)
		{
			if (emailData.internalReadSpecified && (emailData.Read == 0 || emailData.Read == 1))
			{
				deltaSyncMail.Read = (emailData.Read != 0);
			}
			if (emailData.internalReplyToOrForwardStateSpecified && EnumValidator.IsValidValue<DeltaSyncMail.ReplyToOrForwardState>((DeltaSyncMail.ReplyToOrForwardState)emailData.ReplyToOrForwardState))
			{
				deltaSyncMail.ReplyToOrForward = new DeltaSyncMail.ReplyToOrForwardState?((DeltaSyncMail.ReplyToOrForwardState)emailData.ReplyToOrForwardState);
			}
			deltaSyncMail.Parent = new DeltaSyncFolder(new Guid(emailData.FolderId.Value));
			deltaSyncMail.Subject = MimeInternalHelpers.Rfc2047Decode(emailData.Subject.Value);
			deltaSyncMail.From = DeltaSyncDataConversions.DecodeEmailAddress(emailData.From.Value);
			deltaSyncMail.ConversationIndex = emailData.ConversationIndex.Value;
			deltaSyncMail.ConversationTopic = emailData.ConversationTopic.Value;
			if (emailData.internalHasAttachmentsSpecified && (emailData.HasAttachments == bitType.zero || emailData.HasAttachments == bitType.one))
			{
				deltaSyncMail.HasAttachments = (emailData.HasAttachments != bitType.zero);
			}
			if (emailData.internalImportanceSpecified && EnumValidator.IsValidValue<DeltaSyncMail.ImportanceLevel>((DeltaSyncMail.ImportanceLevel)emailData.Importance))
			{
				deltaSyncMail.Importance = (DeltaSyncMail.ImportanceLevel)emailData.Importance;
			}
			deltaSyncMail.MessageClass = emailData.MessageClass;
			if (emailData.internalSensitivitySpecified && EnumValidator.IsValidValue<DeltaSyncMail.SensitivityLevel>((DeltaSyncMail.SensitivityLevel)emailData.Sensitivity))
			{
				deltaSyncMail.Sensitivity = (DeltaSyncMail.SensitivityLevel)emailData.Sensitivity;
			}
			if (emailData.internalSizeSpecified && emailData.Size >= 0)
			{
				deltaSyncMail.Size = emailData.Size;
			}
			ExDateTime utcNow = ExDateTime.UtcNow;
			if (!ExDateTime.TryParse(emailData.DateReceived, out utcNow))
			{
				utcNow = ExDateTime.UtcNow;
				deltaSyncState.SyncLogSession.LogError((TSLID)637UL, DeltaSyncStorageProvider.Tracer, "Invalid Email Received Date ({0}) found, setting it to current DateTime(Utc): {1}", new object[]
				{
					emailData.DateReceived,
					utcNow
				});
			}
			deltaSyncMail.DateReceived = utcNow;
		}

		private static void ProcessApplyChanges(object state)
		{
			AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData> asyncResult = (AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData>)state;
			if (!asyncResult.State.ChangesLoaded)
			{
				DeltaSyncStorageProvider.LoadChanges(asyncResult);
				return;
			}
			DeltaSyncStorageProvider.ApplyChanges(asyncResult);
		}

		private static void LoadChanges(AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData> syncOperationAsyncResult)
		{
			DeltaSyncStorageProviderState state = syncOperationAsyncResult.State;
			int capacity = state.Changes.Count / 3;
			state.DeltaSyncOperations = new List<DeltaSyncOperation>(capacity);
			state.ChangeList = new Dictionary<string, SyncChangeEntry>(capacity);
			state.FolderAddList = new Dictionary<string, SyncChangeEntry>(capacity);
			state.NativeToCloudFolderIdMapping = new Dictionary<StoreObjectId, string>(capacity);
			state.NativeToTempFolderIdMapping = new Dictionary<StoreObjectId, string>(capacity);
			state.EmailAddList = new List<SyncChangeEntry>(capacity);
			state.EmailIdMapping = new Dictionary<string, SyncChangeEntry>(capacity);
			foreach (SyncChangeEntry syncChangeEntry in state.Changes)
			{
				if (syncChangeEntry.SchemaType != SchemaType.Folder && syncChangeEntry.SchemaType != SchemaType.Email)
				{
					state.SyncLogSession.LogDebugging((TSLID)638UL, DeltaSyncStorageProvider.Tracer, "Skipped change with SchemaType {0}", new object[]
					{
						syncChangeEntry.SchemaType
					});
				}
				else
				{
					switch (syncChangeEntry.ChangeType)
					{
					case ChangeType.Add:
						if (syncChangeEntry.SchemaType == SchemaType.Folder)
						{
							DeltaSyncStorageProvider.LoadFolderAddOperation(state.ItemRetriever, state.ItemRetrieverState, state.DeltaSyncOperations, state.FolderAddList, state.NativeToCloudFolderIdMapping, state.NativeToTempFolderIdMapping, syncChangeEntry, syncOperationAsyncResult.SyncPoisonContext);
							continue;
						}
						state.EmailAddList.Add(syncChangeEntry);
						continue;
					case ChangeType.Change:
					case ChangeType.ReadFlagChange:
						if (DeltaSyncStorageProvider.TryGetItemSynchronously(syncChangeEntry, state.ItemRetriever, state.ItemRetrieverState, syncOperationAsyncResult.SyncPoisonContext))
						{
							DeltaSyncObject deltaSyncObject;
							if (syncChangeEntry.SchemaType == SchemaType.Folder)
							{
								deltaSyncObject = new DeltaSyncFolder(new Guid(syncChangeEntry.CloudId));
							}
							else
							{
								deltaSyncObject = new DeltaSyncMail(new Guid(syncChangeEntry.CloudId));
							}
							DeltaSyncStorageProvider.LoadDeltaSyncObject(deltaSyncObject, syncChangeEntry);
							state.DeltaSyncOperations.Add(new DeltaSyncOperation(DeltaSyncOperation.Type.Change, deltaSyncObject));
							state.ChangeList.Add(syncChangeEntry.CloudId, syncChangeEntry);
							continue;
						}
						continue;
					case ChangeType.Delete:
					{
						DeltaSyncObject deltaSyncObject;
						if (syncChangeEntry.SchemaType == SchemaType.Folder)
						{
							deltaSyncObject = new DeltaSyncFolder(new Guid(syncChangeEntry.CloudId));
						}
						else
						{
							deltaSyncObject = new DeltaSyncMail(new Guid(syncChangeEntry.CloudId));
						}
						state.DeltaSyncOperations.Add(new DeltaSyncOperation(DeltaSyncOperation.Type.Delete, deltaSyncObject));
						continue;
					}
					}
					state.SyncLogSession.LogDebugging((TSLID)639UL, DeltaSyncStorageProvider.Tracer, "Skipped change with changeType {0}", new object[]
					{
						syncChangeEntry.ChangeType
					});
				}
			}
			state.EmailIndex = -1;
			DeltaSyncStorageProvider.LoadItems(syncOperationAsyncResult);
		}

		private static void LoadFolderAddOperation(ISyncStorageProviderItemRetriever itemRetriever, object itemRetrieverState, List<DeltaSyncOperation> deltaSyncOperations, Dictionary<string, SyncChangeEntry> folderAddList, Dictionary<StoreObjectId, string> folderIdMapping, Dictionary<StoreObjectId, string> nativeToTempFolderIdMapping, SyncChangeEntry change, object syncPoisonContext)
		{
			if (DeltaSyncStorageProvider.TryGetItemSynchronously(change, itemRetriever, itemRetrieverState, syncPoisonContext))
			{
				string cloudId = null;
				if (DeltaSyncStorageProvider.TryGetDefaultFolderServerId(((SyncFolder)change.SyncObject).DefaultFolderType, out cloudId))
				{
					change.CloudId = cloudId;
					folderIdMapping.Add(change.NativeId, change.CloudId);
					change.SyncObject.Dispose();
					change.SyncObject = null;
					return;
				}
				string text = Guid.NewGuid().ToString();
				DeltaSyncFolder deltaSyncFolder = new DeltaSyncFolder(text);
				if (DeltaSyncStorageProvider.TryLoadParentFolder(change, deltaSyncFolder))
				{
					DeltaSyncStorageProvider.LoadDeltaSyncObject(deltaSyncFolder, change);
					deltaSyncOperations.Add(new DeltaSyncOperation(DeltaSyncOperation.Type.Add, deltaSyncFolder));
					folderAddList.Add(text, change);
					nativeToTempFolderIdMapping.Add(change.NativeId, text);
					return;
				}
				change.SyncObject.Dispose();
				change.SyncObject = null;
			}
		}

		private static void LoadDeltaSyncObject(DeltaSyncObject deltaSyncObject, SyncChangeEntry syncChangeEntry)
		{
			if (syncChangeEntry.SchemaType == SchemaType.Folder)
			{
				((DeltaSyncFolder)deltaSyncObject).DisplayName = ((SyncFolder)syncChangeEntry.SyncObject).DisplayName;
				return;
			}
			DeltaSyncMail deltaSyncMail = (DeltaSyncMail)deltaSyncObject;
			DeltaSyncStorageProvider.LoadDeltaSyncMail(ref deltaSyncMail, (ISyncEmail)syncChangeEntry.SyncObject, syncChangeEntry.ChangeType);
		}

		private static bool TryGetDefaultFolderServerId(DefaultFolderType defaultFolderType, out string serverId)
		{
			serverId = null;
			switch (defaultFolderType)
			{
			case DefaultFolderType.DeletedItems:
				serverId = "00000000-0000-0000-0000-000000000002";
				return true;
			case DefaultFolderType.Drafts:
				serverId = "00000000-0000-0000-0000-000000000004";
				return true;
			case DefaultFolderType.Inbox:
				serverId = "00000000-0000-0000-0000-000000000001";
				return true;
			case DefaultFolderType.JunkEmail:
				serverId = "00000000-0000-0000-0000-000000000005";
				return true;
			case DefaultFolderType.Journal:
			case DefaultFolderType.Notes:
			case DefaultFolderType.Outbox:
				break;
			case DefaultFolderType.SentItems:
				serverId = "00000000-0000-0000-0000-000000000003";
				return true;
			default:
				if (defaultFolderType == DefaultFolderType.Root)
				{
					serverId = "00000000-0000-0000-0000-000000000000";
					return true;
				}
				break;
			}
			return false;
		}

		private static void LoadDeltaSyncMail(ref DeltaSyncMail deltaSyncMail, ISyncEmail nativeEmail, ChangeType changeType)
		{
			deltaSyncMail.Read = nativeEmail.IsRead.Value;
			if (changeType == ChangeType.ReadFlagChange || changeType == ChangeType.Change)
			{
				return;
			}
			if (nativeEmail.SyncMessageResponseType != null)
			{
				deltaSyncMail.ReplyToOrForward = new DeltaSyncMail.ReplyToOrForwardState?(DeltaSyncStorageProvider.GetDeltaSyncMailReplyToOrForwardState(nativeEmail.SyncMessageResponseType.Value));
			}
			else
			{
				deltaSyncMail.ReplyToOrForward = new DeltaSyncMail.ReplyToOrForwardState?(DeltaSyncMail.ReplyToOrForwardState.None);
			}
			deltaSyncMail.Subject = MimeInternalHelpers.Rfc2047Encode(nativeEmail.Subject, Encoding.UTF8);
			deltaSyncMail.From = DeltaSyncDataConversions.EncodeEmailAddress(nativeEmail.From);
			if (nativeEmail.ReceivedTime != null)
			{
				deltaSyncMail.DateReceived = nativeEmail.ReceivedTime.Value;
			}
			else
			{
				deltaSyncMail.DateReceived = ExDateTime.UtcNow;
			}
			if (nativeEmail.IsDraft != null && nativeEmail.IsDraft.Value)
			{
				deltaSyncMail.MessageClass = DeltaSyncCommon.DraftMessageClass;
			}
			else
			{
				deltaSyncMail.MessageClass = nativeEmail.MessageClass;
			}
			deltaSyncMail.ConversationTopic = nativeEmail.ConversationTopic;
			deltaSyncMail.ConversationIndex = nativeEmail.ConversationIndex;
			deltaSyncMail.Size = nativeEmail.Size.Value;
			deltaSyncMail.HasAttachments = nativeEmail.HasAttachments.Value;
			deltaSyncMail.EmailMessage = nativeEmail.MimeStream;
			deltaSyncMail.Importance = DeltaSyncStorageProvider.GetDeltaSyncMailImportance(nativeEmail.Importance.Value);
			deltaSyncMail.Sensitivity = DeltaSyncStorageProvider.GetDeltaSyncMailSensitivity(nativeEmail.Sensitivity.Value);
		}

		private static DeltaSyncMail.ReplyToOrForwardState GetDeltaSyncMailReplyToOrForwardState(SyncMessageResponseType syncMessageResponseType)
		{
			switch (syncMessageResponseType)
			{
			case SyncMessageResponseType.None:
				return DeltaSyncMail.ReplyToOrForwardState.None;
			case SyncMessageResponseType.Replied:
				return DeltaSyncMail.ReplyToOrForwardState.RepliedTo;
			case SyncMessageResponseType.Forwarded:
				return DeltaSyncMail.ReplyToOrForwardState.Forwarded;
			default:
				throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture, "Unknown Sync Message Response Type: {0}", new object[]
				{
					syncMessageResponseType
				}));
			}
		}

		private static DeltaSyncMail.ImportanceLevel GetDeltaSyncMailImportance(Importance importance)
		{
			switch (importance)
			{
			case Importance.Low:
				return DeltaSyncMail.ImportanceLevel.Low;
			case Importance.Normal:
				return DeltaSyncMail.ImportanceLevel.Normal;
			case Importance.High:
				return DeltaSyncMail.ImportanceLevel.High;
			default:
				throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture, "Unknown Importance: {0}", new object[]
				{
					importance
				}));
			}
		}

		private static DeltaSyncMail.SensitivityLevel GetDeltaSyncMailSensitivity(Sensitivity sensitivity)
		{
			switch (sensitivity)
			{
			case Sensitivity.Normal:
				return DeltaSyncMail.SensitivityLevel.Normal;
			case Sensitivity.Personal:
				return DeltaSyncMail.SensitivityLevel.Personal;
			case Sensitivity.Private:
				return DeltaSyncMail.SensitivityLevel.Private;
			case Sensitivity.CompanyConfidential:
				return DeltaSyncMail.SensitivityLevel.Confidential;
			default:
				throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture, "Unknown Sensitivity: {0}", new object[]
				{
					sensitivity
				}));
			}
		}

		private static bool TryLoadParentFolder(SyncChangeEntry syncChangeEntry, DeltaSyncMail deltaSyncMail, DeltaSyncStorageProviderState deltaSyncState)
		{
			if (syncChangeEntry.CloudFolderId == null && syncChangeEntry.NativeFolderId == null)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "parent folder is mandatory for item with native id:{0}", new object[]
				{
					syncChangeEntry.NativeId.ToBase64String()
				}));
			}
			if (syncChangeEntry.CloudFolderId != null)
			{
				deltaSyncMail.Parent = new DeltaSyncFolder(new Guid(syncChangeEntry.CloudFolderId));
				return true;
			}
			string g = null;
			if (deltaSyncState.NativeToCloudFolderIdMapping.TryGetValue(syncChangeEntry.NativeFolderId, out g))
			{
				deltaSyncMail.Parent = new DeltaSyncFolder(new Guid(g));
				return true;
			}
			string clientId = null;
			if (deltaSyncState.NativeToTempFolderIdMapping.TryGetValue(syncChangeEntry.NativeFolderId, out clientId))
			{
				deltaSyncMail.Parent = new DeltaSyncFolder(clientId);
				return true;
			}
			deltaSyncState.SyncLogSession.LogError((TSLID)640UL, DeltaSyncStorageProvider.Tracer, "Parent Folder not found: {0}", new object[]
			{
				syncChangeEntry
			});
			deltaSyncState.UpdateSyncChangeEntry(syncChangeEntry, new DataOutOfSyncException(4402));
			return false;
		}

		private static bool TryLoadParentFolder(SyncChangeEntry syncChangeEntry, DeltaSyncFolder deltaSyncFolder)
		{
			if (syncChangeEntry.CloudFolderId == null && syncChangeEntry.NativeFolderId == null)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "parent folder is mandatory for add item (native id:{0})", new object[]
				{
					syncChangeEntry.NativeId.ToBase64String()
				}));
			}
			if (syncChangeEntry.CloudFolderId != null && syncChangeEntry.CloudFolderId.Equals("00000000-0000-0000-0000-000000000000", StringComparison.OrdinalIgnoreCase))
			{
				deltaSyncFolder.Parent = DeltaSyncFolder.DefaultRootFolder;
				return true;
			}
			syncChangeEntry.Exception = SyncPermanentException.CreateItemLevelException(new NestedFoldersNotAllowedException());
			return false;
		}

		private static bool TryGetItemSynchronously(SyncChangeEntry change, ISyncStorageProviderItemRetriever itemRetriever, object itemRetrieverState, object syncPoisonContext)
		{
			IAsyncResult asyncResult = itemRetriever.BeginGetItem(itemRetrieverState, change, null, null, syncPoisonContext);
			AsyncOperationResult<SyncChangeEntry> asyncOperationResult = itemRetriever.EndGetItem(asyncResult);
			if (asyncOperationResult.IsSucceeded)
			{
				change.SyncObject = asyncOperationResult.Data.SyncObject;
			}
			else
			{
				change.Exception = asyncOperationResult.Exception;
			}
			return change.Exception == null;
		}

		private static void ProcessCompleted(IAsyncResult asyncResult, DeltaSyncStorageProviderState deltaSyncState, Exception exception)
		{
			AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData> asyncResult2 = asyncResult as AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData>;
			AsyncResult<DeltaSyncStorageProviderState, SyncChangeEntry> asyncResult3 = asyncResult as AsyncResult<DeltaSyncStorageProviderState, SyncChangeEntry>;
			if (asyncResult3 != null)
			{
				DeltaSyncStorageProvider.HandleItemLevelProcessComplete(deltaSyncState, exception, asyncResult3);
				return;
			}
			if (asyncResult2 != null)
			{
				DeltaSyncStorageProvider.HandleOperationLevelProcessComplete(deltaSyncState, exception, asyncResult2);
				return;
			}
			throw new InvalidOperationException(string.Format("Invalid type of AsyncResult: {0}.", asyncResult));
		}

		private static void HandleOperationLevelProcessComplete(DeltaSyncStorageProviderState deltaSyncState, Exception exception, AsyncResult<DeltaSyncStorageProviderState, SyncProviderResultData> syncOperationAsyncResult)
		{
			if (exception == null)
			{
				deltaSyncState.ClearApplyChangesState();
				syncOperationAsyncResult.ProcessCompleted(new SyncProviderResultData(deltaSyncState.Changes, deltaSyncState.HasPermanentSyncErrors, deltaSyncState.HasTransientSyncErrors, deltaSyncState.CloudItemsSynced, deltaSyncState.CloudMoreItemsAvailable, deltaSyncState.HasNoChangesOnCloud));
				return;
			}
			deltaSyncState.SyncLogSession.LogDebugging((TSLID)641UL, DeltaSyncStorageProvider.Tracer, "DeltaSyncStorageProvider:OperationLevelProcessCompleted with error: {0}", new object[]
			{
				exception
			});
			bool flag = DeltaSyncStorageProvider.HandleInvalidSyncKeyError(deltaSyncState, exception);
			bool flag2 = false;
			DetailedAggregationStatus detailedAggregationAndIsPermanentStatusFromDeltaSyncException = DeltaSyncStorageProvider.GetDetailedAggregationAndIsPermanentStatusFromDeltaSyncException(exception, out flag2);
			Exception exception2;
			if (flag2)
			{
				SyncPermanentException ex = SyncPermanentException.CreateOperationLevelException(detailedAggregationAndIsPermanentStatusFromDeltaSyncException, exception);
				EventLogEntry eventLogEntry = null;
				if (DeltaSyncStorageProvider.TryGetEventLogEntry(exception, out eventLogEntry))
				{
					ex.EventLogEntry = eventLogEntry;
				}
				exception2 = ex;
			}
			else
			{
				SyncTransientException ex2;
				if (exception is TransientException || flag)
				{
					ex2 = SyncTransientException.CreateOperationLevelException(detailedAggregationAndIsPermanentStatusFromDeltaSyncException, exception);
				}
				else
				{
					ex2 = SyncTransientException.CreateOperationLevelException(detailedAggregationAndIsPermanentStatusFromDeltaSyncException, exception, true);
					deltaSyncState.ClearApplyChangesState();
				}
				EventLogEntry eventLogEntry2 = null;
				if (DeltaSyncStorageProvider.TryGetEventLogEntry(exception, out eventLogEntry2))
				{
					ex2.EventLogEntry = eventLogEntry2;
				}
				exception2 = ex2;
			}
			syncOperationAsyncResult.ProcessCompleted(new SyncProviderResultData(deltaSyncState.Changes, deltaSyncState.HasPermanentSyncErrors, deltaSyncState.HasTransientSyncErrors, deltaSyncState.CloudItemsSynced, deltaSyncState.CloudMoreItemsAvailable, deltaSyncState.HasNoChangesOnCloud), exception2);
		}

		private static DetailedAggregationStatus GetDetailedAggregationAndIsPermanentStatusFromDeltaSyncException(Exception exception, out bool isPermanentError)
		{
			isPermanentError = false;
			if (exception is AuthenticationException)
			{
				return DetailedAggregationStatus.AuthenticationError;
			}
			if (exception is DownloadTransientException || exception is DownloadPermanentException || exception is DeltaSyncServerException)
			{
				return DetailedAggregationStatus.ConnectionError;
			}
			UserAccessException ex = exception as UserAccessException;
			if (ex != null)
			{
				switch (ex.StatusCode)
				{
				case 3201:
					isPermanentError = true;
					return DetailedAggregationStatus.RemoteAccountDoesNotExist;
				case 3202:
				case 3204:
					isPermanentError = true;
					return DetailedAggregationStatus.AuthenticationError;
				case 3205:
					return DetailedAggregationStatus.RemoteMailboxQuotaWarning;
				case 3206:
					isPermanentError = true;
					return DetailedAggregationStatus.MaxedOutSyncRelationshipsError;
				}
				return DetailedAggregationStatus.ConnectionError;
			}
			return DetailedAggregationStatus.CommunicationError;
		}

		private static void HandleItemLevelProcessComplete(DeltaSyncStorageProviderState deltaSyncState, Exception exception, AsyncResult<DeltaSyncStorageProviderState, SyncChangeEntry> itemRetrieverAsyncResult)
		{
			if (exception == null)
			{
				itemRetrieverAsyncResult.ProcessCompleted(deltaSyncState.ItemBeingRetrieved);
				return;
			}
			DeltaSyncStorageProvider.HandleInvalidSyncKeyError(deltaSyncState, exception);
			Exception ex;
			if (DeltaSyncStorageProvider.IsPermanentItemLevelException(exception))
			{
				ex = SyncPermanentException.CreateItemLevelException(exception);
			}
			else
			{
				ex = SyncTransientException.CreateItemLevelException(exception);
			}
			deltaSyncState.SyncLogSession.LogDebugging((TSLID)642UL, DeltaSyncStorageProvider.Tracer, "DeltaSyncStorageProvider:ItemLevelProcessCompleted with error: {0}", new object[]
			{
				ex
			});
			itemRetrieverAsyncResult.ProcessCompleted(deltaSyncState.ItemBeingRetrieved, ex);
		}

		private static bool IsPermanentItemLevelException(Exception exception)
		{
			return exception is MessageSizeLimitExceededException;
		}

		private static bool HandleInvalidSyncKeyError(DeltaSyncStorageProviderState deltaSyncState, Exception exception)
		{
			RequestFormatException ex = exception as RequestFormatException;
			if (ex != null && ex.StatusCode == 4104)
			{
				deltaSyncState.SyncLogSession.LogError((TSLID)643UL, DeltaSyncStorageProvider.Tracer, "Invalid Sync Keys encountered, delta sync state: {0}", new object[]
				{
					deltaSyncState
				});
				deltaSyncState.LatestFolderSyncKey = DeltaSyncCommon.DefaultSyncKey;
				deltaSyncState.LatestEmailSyncKey = DeltaSyncCommon.DefaultSyncKey;
				deltaSyncState.UpdateWaterMark();
				deltaSyncState.SyncLogSession.LogDebugging((TSLID)644UL, DeltaSyncStorageProvider.Tracer, "Sync Keys updated with Default value: {0}, delta sync state: {1}", new object[]
				{
					DeltaSyncCommon.DefaultSyncKey,
					deltaSyncState
				});
				return true;
			}
			return false;
		}

		private static bool TryGetEventLogEntry(Exception exception, out EventLogEntry eventLogEntry)
		{
			PartnerAuthenticationException ex = exception as PartnerAuthenticationException;
			if (ex != null)
			{
				eventLogEntry = new EventLogEntry(TransportSyncWorkerEventLogConstants.Tuple_DeltaSyncPartnerAuthenticationFailed, ex.StatusCode.ToString(), new object[]
				{
					ex.StatusCode
				});
				EventNotificationHelper.PublishTransportEventNotificationItem(TransportSyncNotificationEvent.DeltaSyncPartnerAuthenticationFailed.ToString(), ex);
				return true;
			}
			RequestFormatException ex2 = exception as RequestFormatException;
			if (ex2 != null)
			{
				eventLogEntry = new EventLogEntry(TransportSyncWorkerEventLogConstants.Tuple_DeltaSyncRequestFormatError, ex2.StatusCode.ToString(), new object[]
				{
					ex2.StatusCode
				});
				return true;
			}
			DeltaSyncServiceEndpointsLoadException ex3 = exception as DeltaSyncServiceEndpointsLoadException;
			if (ex3 != null)
			{
				eventLogEntry = new EventLogEntry(TransportSyncWorkerEventLogConstants.Tuple_DeltaSyncServiceEndpointsLoadFailed, string.Empty, new object[]
				{
					ex3
				});
				EventNotificationHelper.PublishTransportEventNotificationItem(TransportSyncNotificationEvent.DeltaSyncServiceEndpointsLoadFailed.ToString(), ex3);
				return true;
			}
			eventLogEntry = null;
			return false;
		}

		private static readonly Trace Tracer = ExTraceGlobals.DeltaSyncStorageProviderTracer;
	}
}
