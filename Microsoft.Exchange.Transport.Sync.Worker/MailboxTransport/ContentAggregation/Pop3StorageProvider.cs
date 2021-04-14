using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class Pop3StorageProvider : ISyncStorageProvider, ISyncStorageProviderItemRetriever
	{
		public AggregationSubscriptionType SubscriptionType
		{
			get
			{
				return AggregationSubscriptionType.Pop;
			}
		}

		public SyncStorageProviderState Bind(ISyncWorkerData subscription, SyncLogSession syncLogSession, bool underRecovery)
		{
			return new Pop3SyncStorageProviderState(subscription, syncLogSession, underRecovery);
		}

		public void Unbind(SyncStorageProviderState state)
		{
		}

		public IAsyncResult BeginAuthenticate(SyncStorageProviderState state, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			Pop3SyncStorageProviderState state2 = (Pop3SyncStorageProviderState)state;
			AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData> asyncResult = new AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData>(this, state2, callback, callbackState, syncPoisonContext);
			asyncResult.SetCompletedSynchronously();
			asyncResult.ProcessCompleted();
			return asyncResult;
		}

		public AsyncOperationResult<SyncProviderResultData> EndAuthenticate(IAsyncResult asyncResult)
		{
			AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public IAsyncResult BeginCheckForChanges(SyncStorageProviderState state, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			Pop3SyncStorageProviderState pop3SyncStorageProviderState = (Pop3SyncStorageProviderState)state;
			AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData> asyncResult = new AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData>(this, pop3SyncStorageProviderState, callback, callbackState, syncPoisonContext);
			asyncResult.PendingAsyncResult = Pop3Client.BeginGetUniqueIds(pop3SyncStorageProviderState.Pop3Client, new AsyncCallback(this.CompleteGetUniqueIdsBeingCheckedForChanges), asyncResult, syncPoisonContext);
			return asyncResult;
		}

		public AsyncOperationResult<SyncProviderResultData> EndCheckForChanges(IAsyncResult asyncResult)
		{
			AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public IAsyncResult BeginEnumerateChanges(SyncStorageProviderState state, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			Pop3SyncStorageProviderState pop3SyncStorageProviderState = (Pop3SyncStorageProviderState)state;
			AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData> asyncResult = new AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData>(this, pop3SyncStorageProviderState, callback, callbackState, syncPoisonContext);
			ExTraceGlobals.FaultInjectionTracer.TraceTest(3966119229U);
			if (pop3SyncStorageProviderState.UniqueIdsResultDataLoaded)
			{
				this.EnumerateChangesFromResultData(pop3SyncStorageProviderState, pop3SyncStorageProviderState.CachedGetUniqueIdsResult);
				asyncResult.SetCompletedSynchronously();
				asyncResult.ProcessCompleted(new SyncProviderResultData(pop3SyncStorageProviderState.Changes, pop3SyncStorageProviderState.HasPermanentSyncErrors, pop3SyncStorageProviderState.HasTransientSyncErrors));
			}
			else
			{
				asyncResult.PendingAsyncResult = Pop3Client.BeginGetUniqueIds(pop3SyncStorageProviderState.Pop3Client, new AsyncCallback(this.CompleteGetUniqueIdsBeingEnumerated), asyncResult, syncPoisonContext);
			}
			return asyncResult;
		}

		public AsyncOperationResult<SyncProviderResultData> EndEnumerateChanges(IAsyncResult asyncResult)
		{
			AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData>)asyncResult;
			ExTraceGlobals.FaultInjectionTracer.TraceTest(2355506493U);
			return asyncResult2.WaitForCompletion();
		}

		public IAsyncResult BeginGetItem(object itemRetrieverState, SyncChangeEntry item, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			Pop3SyncStorageProviderState pop3SyncStorageProviderState = (Pop3SyncStorageProviderState)itemRetrieverState;
			AsyncResult<Pop3SyncStorageProviderState, SyncChangeEntry> asyncResult = new AsyncResult<Pop3SyncStorageProviderState, SyncChangeEntry>(this, pop3SyncStorageProviderState, callback, callbackState, syncPoisonContext);
			ExTraceGlobals.FaultInjectionTracer.TraceTest(3429248317U);
			pop3SyncStorageProviderState.ItemBeingRetrieved = item;
			string cloudId = pop3SyncStorageProviderState.ItemBeingRetrieved.CloudId;
			int messageId;
			if (pop3SyncStorageProviderState.Pop3ResultData.UidlCommandSupported)
			{
				messageId = pop3SyncStorageProviderState.Pop3ResultData[cloudId];
			}
			else
			{
				messageId = int.Parse(cloudId);
			}
			asyncResult.PendingAsyncResult = Pop3Client.BeginGetEmail(pop3SyncStorageProviderState.Pop3Client, messageId, new AsyncCallback(this.ReceivedItem), asyncResult, syncPoisonContext);
			return asyncResult;
		}

		public AsyncOperationResult<SyncChangeEntry> EndGetItem(IAsyncResult asyncResult)
		{
			AsyncResult<Pop3SyncStorageProviderState, SyncChangeEntry> asyncResult2 = (AsyncResult<Pop3SyncStorageProviderState, SyncChangeEntry>)asyncResult;
			ExTraceGlobals.FaultInjectionTracer.TraceTest(2959486269U);
			return asyncResult2.WaitForCompletion();
		}

		public IAsyncResult BeginAcknowledgeChanges(SyncStorageProviderState state, IList<SyncChangeEntry> changeList, bool hasPermanentSyncErrors, bool hasTransientSyncErrors, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			Pop3SyncStorageProviderState pop3SyncStorageProviderState = (Pop3SyncStorageProviderState)state;
			pop3SyncStorageProviderState.CloudStatistics.TotalItemsInSourceMailbox = new long?((long)pop3SyncStorageProviderState.Pop3Client.EmailDropCount);
			AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData> asyncResult = new AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData>(this, pop3SyncStorageProviderState, callback, callbackState, syncPoisonContext);
			pop3SyncStorageProviderState.Changes = changeList;
			pop3SyncStorageProviderState.HasTransientSyncErrors = hasTransientSyncErrors;
			pop3SyncStorageProviderState.HasPermanentSyncErrors = hasPermanentSyncErrors;
			if (pop3SyncStorageProviderState.PopSubscription.ShouldLeaveOnServer)
			{
				this.Quit(asyncResult);
				return asyncResult;
			}
			pop3SyncStorageProviderState.SyncLogSession.LogDebugging((TSLID)921UL, Pop3StorageProvider.Tracer, "Iterates emails successfully applied in this sync session.", new object[0]);
			List<int> list = new List<int>(changeList.Count);
			foreach (SyncChangeEntry syncChangeEntry in changeList)
			{
				if (!syncChangeEntry.HasException)
				{
					string cloudId = syncChangeEntry.CloudId;
					int item = pop3SyncStorageProviderState.Pop3ResultData[cloudId];
					list.Add(item);
				}
			}
			IEnumerator<string> cloudItemEnumerator = pop3SyncStorageProviderState.StateStorage.GetCloudItemEnumerator();
			while (cloudItemEnumerator.MoveNext())
			{
				string uniqueId = cloudItemEnumerator.Current;
				if (pop3SyncStorageProviderState.Pop3ResultData.Contains(uniqueId))
				{
					list.Add(pop3SyncStorageProviderState.Pop3ResultData[uniqueId]);
				}
			}
			pop3SyncStorageProviderState.SyncLogSession.LogDebugging((TSLID)922UL, Pop3StorageProvider.Tracer, "Iterates emails successfully applied in other sync sessions", new object[0]);
			int num = 0;
			foreach (string uniqueId2 in pop3SyncStorageProviderState.PendingDeletionItems.Values)
			{
				if (pop3SyncStorageProviderState.Pop3ResultData.Contains(uniqueId2))
				{
					list.Add(pop3SyncStorageProviderState.Pop3ResultData[uniqueId2]);
					num++;
				}
			}
			if (list.Count == 0)
			{
				this.Quit(asyncResult);
			}
			else
			{
				pop3SyncStorageProviderState.SyncLogSession.LogDebugging((TSLID)923UL, Pop3StorageProvider.Tracer, "Acknowleged {0} fetched emails (including {1} delayed deletes).", new object[]
				{
					list.Count,
					num
				});
				asyncResult.PendingAsyncResult = Pop3Client.BeginDeleteEmails(pop3SyncStorageProviderState.Pop3Client, list, new AsyncCallback(this.CompleteDeleteEmails), asyncResult, syncPoisonContext);
			}
			return asyncResult;
		}

		public AsyncOperationResult<SyncProviderResultData> EndAcknowledgeChanges(IAsyncResult asyncResult)
		{
			AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public IAsyncResult BeginApplyChanges(SyncStorageProviderState state, IList<SyncChangeEntry> changeList, ISyncStorageProviderItemRetriever itemRetriever, object itemRetrieverState, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			Pop3SyncStorageProviderState pop3SyncStorageProviderState = (Pop3SyncStorageProviderState)state;
			AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData> asyncResult = new AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData>(this, pop3SyncStorageProviderState, callback, callbackState, syncPoisonContext);
			if (changeList == null || changeList.Count == 0 || !pop3SyncStorageProviderState.PopSubscription.ShouldSyncDelete)
			{
				asyncResult.SetCompletedSynchronously();
				asyncResult.ProcessCompleted(new SyncProviderResultData(changeList, false, false));
			}
			else
			{
				pop3SyncStorageProviderState.Changes = changeList;
				asyncResult.PendingAsyncResult = Pop3Client.BeginGetUniqueIds(pop3SyncStorageProviderState.Pop3Client, new AsyncCallback(this.CompleteGetUniqueIdsBeingApplied), asyncResult, syncPoisonContext);
			}
			return asyncResult;
		}

		public AsyncOperationResult<SyncProviderResultData> EndApplyChanges(IAsyncResult asyncResult)
		{
			AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public void CancelGetItem(IAsyncResult asyncResult)
		{
		}

		public void Cancel(IAsyncResult asyncResult)
		{
		}

		private static bool ValidateLeaveOnServerStatus(Pop3SyncStorageProviderState pop3State, Pop3ResultData pop3ResultData, out Exception exception)
		{
			exception = null;
			if (!pop3State.PopSubscription.ShouldLeaveOnServer)
			{
				return true;
			}
			if (!pop3ResultData.UidlCommandSupported || (pop3ResultData.RetentionDays != null && pop3ResultData.RetentionDays.Value == 0))
			{
				pop3State.SyncLogSession.LogInformation((TSLID)925UL, Pop3StorageProvider.Tracer, "The POP server does not support 'leave on server'.", new object[0]);
				exception = SyncPermanentException.CreateOperationLevelException(DetailedAggregationStatus.LeaveOnServerNotSupported, new Pop3LeaveOnServerNotPossibleException());
				return false;
			}
			return true;
		}

		private static string ComputeWatermarkWhenUIDLCommentIsSupported(Pop3SyncStorageProviderState pop3State, AsyncOperationResult<Pop3ResultData> pop3ResultData)
		{
			List<string> list = new List<string>(pop3ResultData.Data.EmailDropCount);
			for (int i = 1; i <= pop3ResultData.Data.EmailDropCount; i++)
			{
				string uniqueId = pop3ResultData.Data.GetUniqueId(i);
				if (uniqueId != null)
				{
					list.Add(uniqueId);
				}
			}
			string text = Pop3StorageProvider.GenerateWaterMarkFromUIDLs(list);
			pop3State.SyncLogSession.LogVerbose((TSLID)38UL, Pop3StorageProvider.Tracer, "Existing Watermark:[{0}], New WaterMark:[{1}]", new object[]
			{
				pop3State.SyncWatermark,
				text
			});
			return text;
		}

		internal static string GenerateWaterMarkFromUIDLs(IList<string> uidls)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string value in uidls)
			{
				stringBuilder.Append(value);
			}
			return SyncUtilities.ComputeSHA512Hash(stringBuilder.ToString());
		}

		private void CompleteGetUniqueIdsBeingApplied(IAsyncResult asyncResult)
		{
			AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData>)asyncResult.AsyncState;
			Pop3SyncStorageProviderState state = asyncResult2.State;
			AsyncOperationResult<Pop3ResultData> asyncOperationResult = Pop3Client.EndGetUniqueIds(asyncResult);
			if (!asyncOperationResult.IsSucceeded)
			{
				asyncResult2.ProcessCompleted(new SyncProviderResultData(state.Changes, state.HasPermanentSyncErrors, state.HasTransientSyncErrors), asyncOperationResult.Exception);
				return;
			}
			Dictionary<string, int> dictionary = new Dictionary<string, int>(state.Changes.Count);
			foreach (SyncChangeEntry syncChangeEntry in state.Changes)
			{
				string cloudId = syncChangeEntry.CloudId;
				dictionary[cloudId] = 0;
			}
			for (int i = 1; i <= asyncOperationResult.Data.EmailDropCount; i++)
			{
				string uniqueId = asyncOperationResult.Data.GetUniqueId(i);
				if (uniqueId != null && dictionary.ContainsKey(uniqueId))
				{
					dictionary[uniqueId] = i;
				}
			}
			List<int> list = new List<int>(dictionary.Count);
			foreach (KeyValuePair<string, int> keyValuePair in dictionary)
			{
				if (keyValuePair.Value != 0)
				{
					list.Add(keyValuePair.Value);
				}
			}
			state.SyncLogSession.LogDebugging((TSLID)924UL, Pop3StorageProvider.Tracer, "Applies {0} deleted emails.", new object[]
			{
				list.Count
			});
			asyncResult2.PendingAsyncResult = Pop3Client.BeginDeleteEmails(state.Pop3Client, list, new AsyncCallback(this.CompleteDeleteEmails), asyncResult2, asyncResult2.SyncPoisonContext);
		}

		private void CompleteGetUniqueIdsBeingEnumerated(IAsyncResult asyncResult)
		{
			AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData>)asyncResult.AsyncState;
			Pop3SyncStorageProviderState state = asyncResult2.State;
			AsyncOperationResult<Pop3ResultData> asyncOperationResult = Pop3Client.EndGetUniqueIds(asyncResult);
			if (!asyncOperationResult.IsSucceeded)
			{
				asyncResult2.ProcessCompleted(asyncOperationResult.Exception);
				return;
			}
			Exception exception;
			if (!Pop3StorageProvider.ValidateLeaveOnServerStatus(state, asyncOperationResult.Data, out exception))
			{
				asyncResult2.ProcessCompleted(exception);
				return;
			}
			this.EnumerateChangesFromResultData(state, asyncOperationResult);
			asyncResult2.ProcessCompleted(new SyncProviderResultData(state.Changes, state.HasPermanentSyncErrors, state.HasTransientSyncErrors));
		}

		private void EnumerateChangesFromResultData(Pop3SyncStorageProviderState pop3State, AsyncOperationResult<Pop3ResultData> pop3ResultData)
		{
			long num = pop3State.MaxDownloadSizePerConnection;
			int num2 = pop3State.MaxDownloadItemsPerConnection;
			HashSet<string> hashSet = new HashSet<string>();
			if (pop3State.PopSubscription.ShouldLeaveOnServer)
			{
				IEnumerator<string> cloudItemEnumerator = pop3State.StateStorage.GetCloudItemEnumerator();
				while (cloudItemEnumerator.MoveNext())
				{
					string item = cloudItemEnumerator.Current;
					hashSet.Add(item);
				}
				IEnumerator<string> failedCloudItemEnumerator = pop3State.StateStorage.GetFailedCloudItemEnumerator();
				while (failedCloudItemEnumerator.MoveNext())
				{
					string item2 = failedCloudItemEnumerator.Current;
					hashSet.Add(item2);
				}
			}
			pop3State.Changes = new List<SyncChangeEntry>(Math.Min(pop3ResultData.Data.EmailDropCount, num2));
			pop3State.Pop3ResultData = pop3ResultData.Data;
			if (!pop3ResultData.Data.ReceivedDateDescends)
			{
				pop3State.SyncLogSession.LogDebugging((TSLID)926UL, Pop3StorageProvider.Tracer, "Enumerates messages in descending order because newest ones are at the end.", new object[0]);
			}
			int num3 = 0;
			bool receivedDateDescends = pop3ResultData.Data.ReceivedDateDescends;
			for (int i = 1; i <= pop3ResultData.Data.EmailDropCount; i++)
			{
				int id = receivedDateDescends ? i : (pop3ResultData.Data.EmailDropCount - i + 1);
				string text;
				if (pop3ResultData.Data.UidlCommandSupported)
				{
					text = pop3ResultData.Data.GetUniqueId(id);
				}
				else
				{
					text = id.ToString();
				}
				if (text != null)
				{
					if (pop3State.PopSubscription.ShouldLeaveOnServer)
					{
						hashSet.Remove(text);
					}
					if (pop3ResultData.Data.Contains(text))
					{
						pop3State.SyncLogSession.LogDebugging((TSLID)927UL, Pop3StorageProvider.Tracer, "Prevented generating duplicate add change for unique id: {0}", new object[]
						{
							text
						});
					}
					else
					{
						pop3ResultData.Data.AddUniqueId(text, id);
						bool flag = num2 == 0;
						if (flag)
						{
							num3++;
						}
						else if (!pop3State.StateStorage.ContainsItem(text) && !pop3State.PendingDeletionItems.Contains(text))
						{
							if (pop3State.StateStorage.ContainsFailedItem(text))
							{
								pop3State.SyncLogSession.LogVerbose((TSLID)928UL, Pop3StorageProvider.Tracer, "Skipping Email since we already failed permanently on it, unique Id: {0}", new object[]
								{
									text
								});
							}
							else
							{
								long emailSize = pop3ResultData.Data.GetEmailSize(id);
								if (emailSize != 0L)
								{
									bool flag2 = pop3State.MaxDownloadSizePerItem < emailSize;
									if (!flag2)
									{
										bool flag3 = num < emailSize;
										if (flag3)
										{
											num3++;
											goto IL_295;
										}
										num -= emailSize;
									}
									num2--;
									SyncChangeEntry syncChangeEntry = new SyncChangeEntry(ChangeType.Add, SchemaType.Email, text);
									syncChangeEntry.Persist = pop3State.PopSubscription.ShouldLeaveOnServer;
									if (flag2)
									{
										syncChangeEntry.Exception = SyncPermanentException.CreateItemLevelException(new MessageSizeLimitExceededException());
									}
									pop3State.Changes.Add(syncChangeEntry);
								}
							}
						}
					}
				}
				IL_295:;
			}
			int count = pop3State.Changes.Count;
			long num4 = pop3State.MaxDownloadSizePerConnection - num;
			if (pop3State.PopSubscription.ShouldLeaveOnServer)
			{
				foreach (string cloudId in hashSet)
				{
					pop3State.Changes.Add(new SyncChangeEntry(ChangeType.Delete, SchemaType.Email, cloudId));
				}
			}
			int num5 = pop3State.Changes.Count - count;
			pop3State.SyncLogSession.LogDebugging((TSLID)929UL, Pop3StorageProvider.Tracer, "Enumerates {0} email adds ({1} total bytes) and {2} email deletes ({3} emails yet to come).", new object[]
			{
				count,
				num4,
				num5,
				num3
			});
			pop3State.EmailsYetToCome = num3;
			pop3State.EmailsWereAdded = count;
		}

		private void CompleteGetUniqueIdsBeingCheckedForChanges(IAsyncResult asyncResult)
		{
			AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData>)asyncResult.AsyncState;
			Pop3SyncStorageProviderState state = asyncResult2.State;
			AsyncOperationResult<Pop3ResultData> asyncOperationResult = Pop3Client.EndGetUniqueIds(asyncResult);
			if (!asyncOperationResult.IsSucceeded)
			{
				asyncResult2.ProcessCompleted(asyncOperationResult.Exception);
				return;
			}
			Exception exception;
			if (!Pop3StorageProvider.ValidateLeaveOnServerStatus(state, asyncOperationResult.Data, out exception))
			{
				asyncResult2.ProcessCompleted(exception);
				return;
			}
			state.CacheGetUniqueIdsResult(asyncOperationResult);
			if (!asyncOperationResult.Data.UidlCommandSupported)
			{
				state.SyncLogSession.LogVerbose((TSLID)521UL, Pop3StorageProvider.Tracer, "No watermark calculation in UidlCommandNOTSupported Mode: EmailDropCount:{0}.", new object[]
				{
					asyncOperationResult.Data.EmailDropCount
				});
				state.HasNoChangesOnCloud = (asyncOperationResult.Data.EmailDropCount == 0);
			}
			else
			{
				string text = Pop3StorageProvider.ComputeWatermarkWhenUIDLCommentIsSupported(state, asyncOperationResult);
				if (text == state.SyncWatermark)
				{
					state.HasNoChangesOnCloud = true;
				}
				else
				{
					state.HasNoChangesOnCloud = false;
					state.CacheNewWatermark(text);
				}
			}
			state.Changes = new List<SyncChangeEntry>(0);
			state.CloudStatistics.TotalItemsInSourceMailbox = new long?((long)state.Pop3Client.EmailDropCount);
			if (state.HasNoChangesOnCloud)
			{
				state.SyncLogSession.LogDebugging((TSLID)930UL, Pop3StorageProvider.Tracer, "NO changes on cloud, issue the quit right away.", new object[0]);
				this.QuitWithNoChanges(asyncResult2);
				return;
			}
			this.ProcessCompletedWithHasNoChangesFlag(asyncResult2, state);
		}

		private void ReceivedItem(IAsyncResult asyncResult)
		{
			AsyncResult<Pop3SyncStorageProviderState, SyncChangeEntry> asyncResult2 = (AsyncResult<Pop3SyncStorageProviderState, SyncChangeEntry>)asyncResult.AsyncState;
			Pop3SyncStorageProviderState state = asyncResult2.State;
			AsyncOperationResult<Pop3ResultData> asyncOperationResult = Pop3Client.EndGetEmail(asyncResult);
			if (!asyncOperationResult.IsSucceeded)
			{
				asyncResult2.ProcessCompleted(state.ItemBeingRetrieved, asyncOperationResult.Exception);
				return;
			}
			state.ItemBeingRetrieved.SyncObject = asyncOperationResult.Data.Email;
			asyncResult2.ProcessCompleted(state.ItemBeingRetrieved);
		}

		private void CompleteDeleteEmails(IAsyncResult asyncResult)
		{
			AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData>)asyncResult.AsyncState;
			Pop3SyncStorageProviderState state = asyncResult2.State;
			AsyncOperationResult<Pop3ResultData> asyncOperationResult = Pop3Client.EndDeleteEmails(asyncResult);
			if (!asyncOperationResult.IsSucceeded)
			{
				state.DeletionError = asyncOperationResult.Exception;
			}
			this.Quit(asyncResult2);
		}

		private void Quit(AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData> syncOperationAsyncResult)
		{
			Pop3SyncStorageProviderState state = syncOperationAsyncResult.State;
			syncOperationAsyncResult.PendingAsyncResult = Pop3Client.BeginQuit(state.Pop3Client, new AsyncCallback(this.QuitCompleted), syncOperationAsyncResult, syncOperationAsyncResult.SyncPoisonContext);
		}

		private void QuitCompleted(IAsyncResult asyncResult)
		{
			AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData>)asyncResult.AsyncState;
			Pop3SyncStorageProviderState state = asyncResult2.State;
			AsyncOperationResult<Pop3ResultData> asyncOperationResult = Pop3Client.EndQuit(asyncResult);
			if (asyncOperationResult.Data != null && asyncOperationResult.Data.EmailDropCount == 0)
			{
				state.PendingDeletionItems.Clear();
			}
			if (!asyncOperationResult.IsSucceeded || state.DeletionError != null)
			{
				if (!state.PopSubscription.ShouldLeaveOnServer && state.Pop3ResultData.UidlCommandSupported)
				{
					state.PendingDeletionItems.SetCapacity(state.Changes.Count);
					int num = 0;
					foreach (SyncChangeEntry syncChangeEntry in state.Changes)
					{
						if (!syncChangeEntry.HasException)
						{
							state.PendingDeletionItems.Add(syncChangeEntry.CloudId);
							num++;
						}
					}
					if (num > 0)
					{
						state.SyncLogSession.LogDebugging((TSLID)295UL, Pop3StorageProvider.Tracer, "Kept {0} applied emails into the sync state.", new object[]
						{
							num
						});
					}
				}
				state.PersistPendingDeletionItems();
				asyncResult2.ProcessCompleted(SyncProviderResultData.CreateAcknowledgeChangesResult(state.Changes, state.HasPermanentSyncErrors, state.HasTransientSyncErrors, state.EmailsWereAdded, state.EmailsYetToCome > 0), state.DeletionError ?? asyncOperationResult.Exception);
				return;
			}
			if (!state.PopSubscription.ShouldLeaveOnServer && state.Pop3ResultData.UidlCommandSupported)
			{
				int num2 = 0;
				if (asyncOperationResult.Data != null)
				{
					foreach (string item in asyncOperationResult.Data.DeletedMessageUniqueIds)
					{
						if (state.PendingDeletionItems.Remove(item))
						{
							num2++;
						}
					}
				}
				if (num2 > 0)
				{
					state.SyncLogSession.LogDebugging((TSLID)931UL, Pop3StorageProvider.Tracer, "Got rid of {0} deleted emails from the sync state.", new object[]
					{
						num2
					});
				}
			}
			state.PersistPendingDeletionItems();
			bool moreItemsAvailable = false;
			if (!state.HasTransientSyncErrors && state.EmailsYetToCome == 0)
			{
				state.UpdateSubscriptionWithCurrentWaterMark();
				state.SyncLogSession.LogVerbose((TSLID)1458UL, Pop3StorageProvider.Tracer, "Watermark Updated. SyncWatermark:[{0}]", new object[]
				{
					state.PopWatermark
				});
			}
			else
			{
				state.SyncLogSession.LogVerbose((TSLID)1459UL, Pop3StorageProvider.Tracer, "Watermark Update skipped, MoreItemsAvailable set to true. HasTransientSyncErrors:{0}, EmailsYetToCome:{1}", new object[]
				{
					state.HasPermanentSyncErrors,
					state.EmailsYetToCome
				});
				moreItemsAvailable = true;
			}
			asyncResult2.ProcessCompleted(SyncProviderResultData.CreateAcknowledgeChangesResult(state.Changes, state.HasPermanentSyncErrors, state.HasTransientSyncErrors, state.EmailsWereAdded, moreItemsAvailable));
		}

		private void QuitWithNoChanges(AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData> syncOperationAsyncResult)
		{
			Pop3SyncStorageProviderState state = syncOperationAsyncResult.State;
			syncOperationAsyncResult.PendingAsyncResult = Pop3Client.BeginQuit(state.Pop3Client, new AsyncCallback(this.QuitCompletedWithNoChanges), syncOperationAsyncResult, syncOperationAsyncResult.SyncPoisonContext);
		}

		private void QuitCompletedWithNoChanges(IAsyncResult asyncResult)
		{
			AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData>)asyncResult.AsyncState;
			Pop3SyncStorageProviderState state = asyncResult2.State;
			AsyncOperationResult<Pop3ResultData> asyncOperationResult = Pop3Client.EndQuit(asyncResult);
			if (!asyncOperationResult.IsSucceeded)
			{
				asyncResult2.ProcessCompleted(asyncOperationResult.Exception);
				return;
			}
			this.ProcessCompletedWithHasNoChangesFlag(asyncResult2, state);
		}

		private void ProcessCompletedWithHasNoChangesFlag(AsyncResult<Pop3SyncStorageProviderState, SyncProviderResultData> syncOperationAsyncResult, Pop3SyncStorageProviderState pop3State)
		{
			syncOperationAsyncResult.ProcessCompleted(new SyncProviderResultData(pop3State.Changes, pop3State.HasPermanentSyncErrors, pop3State.HasTransientSyncErrors, pop3State.HasNoChangesOnCloud));
		}

		private static readonly Trace Tracer = ExTraceGlobals.Pop3StorageProviderTracer;
	}
}
