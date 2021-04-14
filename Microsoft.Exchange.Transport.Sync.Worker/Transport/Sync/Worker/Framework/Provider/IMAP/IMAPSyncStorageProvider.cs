using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.MailboxTransport.ContentAggregation;
using Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;
using Microsoft.Exchange.Transport.Sync.Worker.Framework.Provider.IMAP.Client;

namespace Microsoft.Exchange.Transport.Sync.Worker.Framework.Provider.IMAP
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class IMAPSyncStorageProvider : ISyncStorageProvider, ISyncStorageProviderItemRetriever
	{
		public AggregationSubscriptionType SubscriptionType
		{
			get
			{
				return AggregationSubscriptionType.IMAP;
			}
		}

		public SyncStorageProviderState Bind(ISyncWorkerData subscription, SyncLogSession syncLogSession, bool underRecovery)
		{
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			IMAPMailbox.EnsureDefaultFolderMappingTable(syncLogSession);
			return new IMAPSyncStorageProviderState(subscription, syncLogSession, underRecovery);
		}

		public void Unbind(SyncStorageProviderState state)
		{
			SyncUtilities.ThrowIfArgumentNull("state", state);
			IMAPSyncStorageProviderState imapsyncStorageProviderState = (IMAPSyncStorageProviderState)state;
			if (imapsyncStorageProviderState.RequiresLogOff)
			{
				AsyncResult<IMAPClientState, DBNull> asyncResult = (AsyncResult<IMAPClientState, DBNull>)IMAPClient.BeginLogOff(imapsyncStorageProviderState.ClientState, null, null, null);
				imapsyncStorageProviderState.RequiresLogOff = false;
			}
		}

		public IAsyncResult BeginAuthenticate(SyncStorageProviderState state, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			SyncUtilities.ThrowIfArgumentNull("state", state);
			IMAPSyncStorageProviderState imapsyncStorageProviderState = (IMAPSyncStorageProviderState)state;
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult = new AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>(this, imapsyncStorageProviderState, callback, callbackState, syncPoisonContext);
			asyncResult.PendingAsyncResult = IMAPClient.BeginConnectAndAuthenticate(imapsyncStorageProviderState.ClientState, new AsyncCallback(IMAPSyncStorageProvider.OnEndConnectAndAuthenticate), asyncResult, syncPoisonContext);
			return asyncResult;
		}

		public AsyncOperationResult<SyncProviderResultData> EndAuthenticate(IAsyncResult asyncResult)
		{
			SyncUtilities.ThrowIfArgumentNull("asyncResult", asyncResult);
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public IAsyncResult BeginCheckForChanges(SyncStorageProviderState state, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			SyncUtilities.ThrowIfArgumentNull("state", state);
			IMAPSyncStorageProviderState imapsyncStorageProviderState = (IMAPSyncStorageProviderState)state;
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult = new AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>(this, imapsyncStorageProviderState, callback, callbackState, syncPoisonContext);
			imapsyncStorageProviderState.SetInitialFolderLevel();
			asyncResult.PendingAsyncResult = IMAPClient.BeginListImapMailboxesByLevel(imapsyncStorageProviderState.ClientState, imapsyncStorageProviderState.CurrentFolderListLevel, imapsyncStorageProviderState.GetSeparatorCharacter(), new AsyncCallback(IMAPSyncStorageProvider.OnEndListLevelForCheckForChangesInternal), asyncResult, syncPoisonContext);
			return asyncResult;
		}

		public AsyncOperationResult<SyncProviderResultData> EndCheckForChanges(IAsyncResult asyncResult)
		{
			SyncUtilities.ThrowIfArgumentNull("asyncResult", asyncResult);
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public IAsyncResult BeginEnumerateChanges(SyncStorageProviderState state, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			SyncUtilities.ThrowIfArgumentNull("state", state);
			IMAPMailbox.EnsureDefaultFolderMappingTable(state.SyncLogSession);
			IMAPSyncStorageProviderState imapsyncStorageProviderState = (IMAPSyncStorageProviderState)state;
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult = new AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>(this, imapsyncStorageProviderState, callback, callbackState, syncPoisonContext);
			bool wasEnumerateEntered = imapsyncStorageProviderState.WasEnumerateEntered;
			imapsyncStorageProviderState.WasEnumerateEntered = true;
			if (!imapsyncStorageProviderState.IsListLevelsComplete || wasEnumerateEntered)
			{
				imapsyncStorageProviderState.SetInitialFolderLevel();
				asyncResult.PendingAsyncResult = IMAPClient.BeginListImapMailboxesByLevel(imapsyncStorageProviderState.ClientState, imapsyncStorageProviderState.CurrentFolderListLevel, imapsyncStorageProviderState.GetSeparatorCharacter(), new AsyncCallback(IMAPSyncStorageProvider.OnEndListLevelInternal), asyncResult, syncPoisonContext);
			}
			else
			{
				IMAPSyncStorageProvider.StartEnumeratingCloudChangesByFolder(asyncResult, imapsyncStorageProviderState);
			}
			return asyncResult;
		}

		public AsyncOperationResult<SyncProviderResultData> EndEnumerateChanges(IAsyncResult asyncResult)
		{
			SyncUtilities.ThrowIfArgumentNull("asyncResult", asyncResult);
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public IAsyncResult BeginAcknowledgeChanges(SyncStorageProviderState state, IList<SyncChangeEntry> changeList, bool hasPermanentSyncErrors, bool hasTransientSyncErrors, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			SyncUtilities.ThrowIfArgumentNull("state", state);
			SyncUtilities.ThrowIfArgumentNull("changeList", changeList);
			state.SyncLogSession.LogDebugging((TSLID)677UL, IMAPSyncStorageProvider.Tracer, "BeginAcknowledgeChanges called with {0} items", new object[]
			{
				changeList.Count
			});
			IMAPSyncStorageProviderState imapsyncStorageProviderState = (IMAPSyncStorageProviderState)state;
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult = new AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>(this, imapsyncStorageProviderState, callback, callbackState, syncPoisonContext);
			if (hasTransientSyncErrors)
			{
				state.SyncLogSession.LogError((TSLID)678UL, IMAPSyncStorageProvider.Tracer, "Sync had transient errors, watermark not updating and setting imapState.MoreItemsAvailable = true.", new object[0]);
				imapsyncStorageProviderState.MoreItemsAvailable = true;
			}
			else
			{
				imapsyncStorageProviderState.UpdateSubscriptionSyncWatermarkIfNeeded();
			}
			asyncResult.SetCompletedSynchronously();
			IDictionary<IMAPFolder, bool> dictionary = new Dictionary<IMAPFolder, bool>(5);
			foreach (SyncChangeEntry syncChangeEntry in changeList)
			{
				string text2;
				string text3;
				if (hasTransientSyncErrors)
				{
					if (syncChangeEntry.HasException)
					{
						if (syncChangeEntry.Exception is SyncTransientException)
						{
							string text = null;
							if (syncChangeEntry.SchemaType == SchemaType.Folder)
							{
								text = syncChangeEntry.CloudId;
							}
							else if (syncChangeEntry.SchemaType == SchemaType.Email)
							{
								text = syncChangeEntry.CloudFolderId;
							}
							IMAPFolder key;
							if (text != null && imapsyncStorageProviderState.CloudIdToFolder.TryGetValue(text, out key))
							{
								dictionary[key] = false;
							}
						}
					}
					else if (syncChangeEntry.SchemaType == SchemaType.Folder && syncChangeEntry.ChangeType == ChangeType.Delete)
					{
						if (state.StateStorage.TryUpdateFolderCloudVersion(syncChangeEntry.CloudId, "DeletedFolder"))
						{
							state.SyncLogSession.LogDebugging((TSLID)679UL, IMAPSyncStorageProvider.Tracer, "Updated cloud folder version to deletion marker.  CloudId = [{0}].", new object[]
							{
								syncChangeEntry.CloudId
							});
						}
						else
						{
							state.SyncLogSession.LogError((TSLID)680UL, IMAPSyncStorageProvider.Tracer, "Failed to update cloud folder version.  CloudId=[{0}].  NewCloudFolderVersion=[{1}]", new object[]
							{
								syncChangeEntry.CloudId,
								"DeletedFolder"
							});
						}
					}
				}
				else if (syncChangeEntry.SchemaType == SchemaType.Email && syncChangeEntry.ChangeType == ChangeType.Change && !syncChangeEntry.HasException && state.StateStorage.TryFindItem(syncChangeEntry.CloudId, out text2, out text3) && text3 != syncChangeEntry.CloudVersion)
				{
					if (!state.StateStorage.TryUpdateItemCloudVersion(syncChangeEntry.CloudId, syncChangeEntry.CloudVersion))
					{
						state.SyncLogSession.LogError((TSLID)681UL, IMAPSyncStorageProvider.Tracer, "Failed to update item cloud version.  CloudId=[{0}].  OldCloudVersion=[{1}], NewCloudVersion=[{2}]", new object[]
						{
							syncChangeEntry.CloudId,
							text3,
							syncChangeEntry.CloudVersion
						});
					}
					else
					{
						state.SyncLogSession.LogDebugging((TSLID)682UL, IMAPSyncStorageProvider.Tracer, "Updated item cloud version.  CloudId=[{0}].  OldCloudVersion=[{1}], NewCloudVersion=[{2}]", new object[]
						{
							syncChangeEntry.CloudId,
							text3,
							syncChangeEntry.CloudVersion
						});
					}
				}
			}
			foreach (IMAPFolder imapfolder in imapsyncStorageProviderState.CloudIdToFolder.Values)
			{
				if (imapfolder.Visited && imapfolder.HasCloudVersionChanged)
				{
					string text4 = imapfolder.GenerateFolderCloudVersion();
					bool flag = imapfolder.UpdateFolderCloudVersion(dictionary.ContainsKey(imapfolder));
					string text5 = imapfolder.GenerateFolderCloudVersion();
					if (imapsyncStorageProviderState.StateStorage.TryUpdateFolderCloudVersion(imapfolder.CloudId, text5))
					{
						state.SyncLogSession.LogDebugging((TSLID)683UL, IMAPSyncStorageProvider.Tracer, "Updating folder cloud version.  CloudFolderId = [{0}].  OldCloudVersion = [{1}].  NewCloudVersion = [{2}].  {3}.", new object[]
						{
							imapfolder.CloudId,
							text4,
							text5,
							flag ? "Previous high/low range was discarded" : "Previous range was kept or extended"
						});
					}
					else
					{
						state.SyncLogSession.LogError((TSLID)684UL, IMAPSyncStorageProvider.Tracer, "Failed to update cloud folder version.  CloudFolderId=[{0}].  NewCloudFolderVersion=[{1}]", new object[]
						{
							imapfolder.CloudId,
							text5
						});
					}
				}
			}
			IMAPSyncStorageProvider.DisposeEnumerationChanges(changeList);
			asyncResult.ProcessCompleted(SyncProviderResultData.CreateAcknowledgeChangesResult(changeList, hasPermanentSyncErrors, hasTransientSyncErrors, (changeList == null) ? 0 : changeList.Count, imapsyncStorageProviderState.MoreItemsAvailable));
			return asyncResult;
		}

		public AsyncOperationResult<SyncProviderResultData> EndAcknowledgeChanges(IAsyncResult asyncResult)
		{
			SyncUtilities.ThrowIfArgumentNull("asyncResult", asyncResult);
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public IAsyncResult BeginApplyChanges(SyncStorageProviderState state, IList<SyncChangeEntry> changeList, ISyncStorageProviderItemRetriever itemRetriever, object itemRetrieverState, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			SyncUtilities.ThrowIfArgumentNull("state", state);
			SyncUtilities.ThrowIfArgumentNull("changeList", changeList);
			SyncUtilities.ThrowIfArgumentNull("itemRetriever", itemRetriever);
			IMAPSyncStorageProviderState imapsyncStorageProviderState = (IMAPSyncStorageProviderState)state;
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult = new AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>(this, imapsyncStorageProviderState, callback, callbackState, syncPoisonContext);
			imapsyncStorageProviderState.SyncLogSession.LogDebugging((TSLID)685UL, IMAPSyncStorageProvider.Tracer, "BeginApplyChanges with {0} changes.", new object[]
			{
				changeList.Count
			});
			imapsyncStorageProviderState.Changes = changeList;
			imapsyncStorageProviderState.ItemRetriever = itemRetriever;
			imapsyncStorageProviderState.ItemRetrieverState = itemRetrieverState;
			imapsyncStorageProviderState.HasTransientSyncErrors = false;
			imapsyncStorageProviderState.HasPermanentSyncErrors = false;
			imapsyncStorageProviderState.LastSelectFailedFolder = null;
			if (changeList == null || changeList.Count == 0)
			{
				asyncResult.SetCompletedSynchronously();
				asyncResult.ProcessCompleted(new SyncProviderResultData(imapsyncStorageProviderState.Changes, false, false), null);
			}
			else
			{
				imapsyncStorageProviderState.ApplyChangesEnumerator = changeList.GetEnumerator();
				IMAPSyncStorageProvider.ApplyChangeAndContinue(asyncResult);
			}
			return asyncResult;
		}

		public AsyncOperationResult<SyncProviderResultData> EndApplyChanges(IAsyncResult asyncResult)
		{
			SyncUtilities.ThrowIfArgumentNull("asyncResult", asyncResult);
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public void Cancel(IAsyncResult asyncResult)
		{
			SyncUtilities.ThrowIfArgumentNull("asyncResult", asyncResult);
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>)asyncResult;
			IMAPSyncStorageProviderState state = asyncResult2.State;
			IMAPClient.Cancel(state.ClientState);
			asyncResult2.ProcessCanceled();
		}

		public IAsyncResult BeginGetItem(object itemRetrieverState, SyncChangeEntry item, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			SyncUtilities.ThrowIfArgumentNull("itemRetrieverState", itemRetrieverState);
			SyncUtilities.ThrowIfArgumentNull("item", item);
			IMAPSyncStorageProviderState imapsyncStorageProviderState = (IMAPSyncStorageProviderState)itemRetrieverState;
			imapsyncStorageProviderState.ItemBeingRetrieved = item;
			AsyncResult<IMAPSyncStorageProviderState, SyncChangeEntry> asyncResult = new AsyncResult<IMAPSyncStorageProviderState, SyncChangeEntry>(this, imapsyncStorageProviderState, callback, callbackState, syncPoisonContext);
			string uid;
			if (imapsyncStorageProviderState.CurrentFolder == null || imapsyncStorageProviderState.CurrentFolder.CloudId != item.CloudFolderId)
			{
				IMAPFolder imapfolder;
				if (imapsyncStorageProviderState.CloudIdToFolder.TryGetValue(item.CloudFolderId, out imapfolder) && imapfolder != null)
				{
					if (imapfolder == imapsyncStorageProviderState.LastSelectFailedFolder)
					{
						IMAPUtils.LogExceptionDetails(imapsyncStorageProviderState.SyncLogSession, IMAPSyncStorageProvider.Tracer, "Failing change due to previous select failure", imapsyncStorageProviderState.LastSelectFailedFolderException);
						if (IMAPSyncStorageProvider.IsOperationError(imapsyncStorageProviderState.LastSelectFailedFolderException))
						{
							asyncResult.ProcessCompleted(imapsyncStorageProviderState.ItemBeingRetrieved, imapsyncStorageProviderState.LastSelectFailedFolderException);
						}
						else
						{
							asyncResult.ProcessCompleted(imapsyncStorageProviderState.ItemBeingRetrieved, SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, imapsyncStorageProviderState.LastSelectFailedFolderException, true));
						}
					}
					else
					{
						imapsyncStorageProviderState.CurrentFolder = imapfolder;
						imapsyncStorageProviderState.SyncLogSession.LogDebugging((TSLID)686UL, IMAPSyncStorageProvider.Tracer, "Begin Select folder to retrieve item: CloudFolderId={0}.", new object[]
						{
							item.CloudFolderId
						});
						asyncResult.PendingAsyncResult = IMAPClient.BeginSelectImapMailbox(imapsyncStorageProviderState.ClientState, imapsyncStorageProviderState.CurrentFolder.Mailbox, new AsyncCallback(IMAPSyncStorageProvider.OnEndSelectContinueValidateAndGetItem), asyncResult, asyncResult.SyncPoisonContext);
					}
				}
				else
				{
					asyncResult.ProcessCompleted(imapsyncStorageProviderState.ItemBeingRetrieved, IMAPSyncStorageProvider.BuildAndLogMissingMailboxException(imapsyncStorageProviderState, item.CloudFolderId));
				}
			}
			else if (!IMAPUtils.GetUidFromEmailCloudVersion(item.CloudVersion, out uid))
			{
				asyncResult.ProcessCompleted(imapsyncStorageProviderState.ItemBeingRetrieved, IMAPSyncStorageProvider.BuildAndLogCloudVersionParseException(imapsyncStorageProviderState, item.CloudId, item.CloudVersion, true));
			}
			else
			{
				asyncResult.PendingAsyncResult = IMAPClient.BeginGetMessageItemByUid(imapsyncStorageProviderState.ClientState, uid, new AsyncCallback(IMAPSyncStorageProvider.OnEndGetMessageItem), asyncResult, syncPoisonContext);
			}
			return asyncResult;
		}

		public AsyncOperationResult<SyncChangeEntry> EndGetItem(IAsyncResult asyncResult)
		{
			SyncUtilities.ThrowIfArgumentNull("asyncResult", asyncResult);
			AsyncResult<IMAPSyncStorageProviderState, SyncChangeEntry> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncChangeEntry>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public void CancelGetItem(IAsyncResult asyncResult)
		{
			SyncUtilities.ThrowIfArgumentNull("asyncResult", asyncResult);
			AsyncResult<IMAPSyncStorageProviderState, SyncChangeEntry> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncChangeEntry>)asyncResult;
			IMAPSyncStorageProviderState state = asyncResult2.State;
			IMAPClient.Cancel(state.ClientState);
			asyncResult2.ProcessCanceled();
		}

		private static void OnEndConnectAndAuthenticate(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>)asyncResult.AsyncState;
			IMAPSyncStorageProviderState state = asyncResult2.State;
			AsyncOperationResult<DBNull> asyncOperationResult = IMAPClient.EndConnectAndAuthenticate(asyncResult);
			if (asyncResult.CompletedSynchronously)
			{
				asyncResult2.SetCompletedSynchronously();
			}
			if (!asyncOperationResult.IsSucceeded)
			{
				IMAPUtils.LogExceptionDetails(state.SyncLogSession, IMAPSyncStorageProvider.Tracer, "IMAP Connection failed with error", asyncOperationResult.Exception);
				asyncResult2.ProcessCompleted(new SyncProviderResultData(null, false, false), IMAPSyncStorageProvider.BuildConnectAndAuthenticateException(state, asyncOperationResult.Exception));
				return;
			}
			state.RequiresLogOff = true;
			state.SyncLogSession.LogConnect((TSLID)687UL, "sessionId={0}, auth={1}, security={2}", new object[]
			{
				state.SessionId,
				state.ClientState.IMAPAuthenticationMechanism,
				state.ClientState.IMAPSecurityMechanism
			});
			asyncResult2.PendingAsyncResult = IMAPClient.BeginCapabilities(state.ClientState, new AsyncCallback(IMAPSyncStorageProvider.OnEndCapabilities), asyncResult2, asyncResult2.SyncPoisonContext);
		}

		private static void OnEndCapabilities(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>)asyncResult.AsyncState;
			IMAPSyncStorageProviderState state = asyncResult2.State;
			AsyncOperationResult<IList<string>> asyncOperationResult = IMAPClient.EndCapabilities(asyncResult);
			Exception exception = null;
			if (!asyncOperationResult.IsSucceeded)
			{
				IMAPUtils.LogExceptionDetails(state.SyncLogSession, IMAPSyncStorageProvider.Tracer, "Getting IMAP Capabilities failed with error", asyncOperationResult.Exception);
				exception = IMAPSyncStorageProvider.BuildConnectAndAuthenticateException(state, asyncOperationResult.Exception);
			}
			else
			{
				bool flag = false;
				if (asyncOperationResult.Data != null)
				{
					foreach (string value in asyncOperationResult.Data)
					{
						if ("imap4rev1".Equals(value, StringComparison.OrdinalIgnoreCase))
						{
							flag = true;
						}
					}
				}
				if (!flag)
				{
					state.SyncLogSession.LogError((TSLID)688UL, IMAPSyncStorageProvider.Tracer, "IMAP server not capable of supporting expected version.", new object[0]);
					exception = SyncPermanentException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, new IMAPUnsupportedVersionException());
				}
			}
			asyncResult2.ProcessCompleted(new SyncProviderResultData(null, false, false), exception);
		}

		private static void OnEndListLevelInternal(IAsyncResult asyncResult)
		{
			IMAPSyncStorageProvider.ProcessListLevelResultWithNextActions(asyncResult, new AsyncCallback(IMAPSyncStorageProvider.OnEndListLevelInternal), new Action<AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>, IMAPSyncStorageProviderState>(IMAPSyncStorageProvider.StartEnumeratingCloudChangesByFolder), new Action<AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>, IMAPSyncStorageProviderState, AsyncOperationResult<IList<IMAPMailbox>>>(IMAPSyncStorageProvider.HandleFailedListOperationResult));
		}

		private static void OnEndListLevelForCheckForChangesInternal(IAsyncResult asyncResult)
		{
			IMAPSyncStorageProvider.ProcessListLevelResultWithNextActions(asyncResult, new AsyncCallback(IMAPSyncStorageProvider.OnEndListLevelForCheckForChangesInternal), new Action<AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>, IMAPSyncStorageProviderState>(IMAPSyncStorageProvider.StartSelectCloudFoldersAndBuildWatermark), new Action<AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>, IMAPSyncStorageProviderState, AsyncOperationResult<IList<IMAPMailbox>>>(IMAPSyncStorageProvider.HandleFailedListOperationResultForCheckForChanges));
		}

		private static void OnEndExpungeContinueLogOff(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>)asyncResult.AsyncState;
			IMAPSyncStorageProviderState state = asyncResult2.State;
			AsyncOperationResult<DBNull> asyncOperationResult = IMAPClient.EndExpunge(asyncResult);
			SyncChangeEntry syncChangeEntry = state.ApplyChangesEnumerator.Current;
			if (!asyncOperationResult.IsSucceeded)
			{
				IMAPSyncStorageProvider.RollbackDeletesOnFailedExpunge(state);
			}
			state.PendingExpungeCloudFolderId = null;
			state.SyncLogSession.LogDebugging((TSLID)695UL, IMAPSyncStorageProvider.Tracer, "IMAP Folder expunge succeeded.  Continuing to log off.", new object[0]);
			asyncResult2.PendingAsyncResult = IMAPClient.BeginLogOff(state.ClientState, new AsyncCallback(IMAPSyncStorageProvider.OnEndLogOff), asyncResult2, asyncResult2.SyncPoisonContext);
			state.RequiresLogOff = false;
			asyncResult2.ProcessCompleted(new SyncProviderResultData(state.Changes, state.HasPermanentSyncErrors, state.HasTransientSyncErrors), null);
		}

		private static void OnEndLogOff(IAsyncResult asyncResult)
		{
		}

		private static void OnEndSelectContinueValidateAndGetItem(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPSyncStorageProviderState, SyncChangeEntry> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncChangeEntry>)asyncResult.AsyncState;
			IMAPSyncStorageProviderState state = asyncResult2.State;
			AsyncOperationResult<IMAPMailbox> asyncOperationResult = IMAPClient.EndSelectImapMailbox(asyncResult);
			if (asyncOperationResult.IsSucceeded)
			{
				state.CurrentFolder.Mailbox = asyncOperationResult.Data;
				IMAPSyncStorageProvider.UpdateValidityReconcileAndContinue<SyncChangeEntry>(asyncResult2, new IMAPSyncStorageProviderState.PostProcessor(IMAPSyncStorageProvider.OnEndValidateContinueGetItem));
				return;
			}
			IMAPSyncStorageProvider.FailSelect(state, asyncOperationResult.Exception, false);
			if (IMAPSyncStorageProvider.IsOperationError(asyncOperationResult.Exception))
			{
				asyncResult2.ProcessCompleted(state.ItemBeingRetrieved, asyncOperationResult.Exception);
				return;
			}
			asyncResult2.ProcessCompleted(state.ItemBeingRetrieved, SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, asyncOperationResult.Exception, true));
		}

		private static void OnEndValidateContinueGetItem(IAsyncResult asyncResult, Exception exceptionDuringReconciliation)
		{
			AsyncResult<IMAPSyncStorageProviderState, SyncChangeEntry> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncChangeEntry>)asyncResult;
			IMAPSyncStorageProviderState state = asyncResult2.State;
			if (exceptionDuringReconciliation != null)
			{
				IMAPUtils.LogExceptionDetails(state.SyncLogSession, IMAPSyncStorageProvider.Tracer, "Failed during UIDValidity change reconciliation prior to fetching item", exceptionDuringReconciliation);
				if (IMAPSyncStorageProvider.IsOperationError(exceptionDuringReconciliation))
				{
					asyncResult2.ProcessCompleted(state.ItemBeingRetrieved, exceptionDuringReconciliation);
					return;
				}
				asyncResult2.ProcessCompleted(state.ItemBeingRetrieved, SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, exceptionDuringReconciliation, true));
				return;
			}
			else
			{
				state.SyncLogSession.LogDebugging((TSLID)696UL, IMAPSyncStorageProvider.Tracer, "IMAP Folder selection succeeded, continuing to fetch message.", new object[0]);
				string uid;
				if (!IMAPUtils.GetUidFromEmailCloudVersion(state.ItemBeingRetrieved.CloudVersion, out uid))
				{
					asyncResult2.ProcessCompleted(state.ItemBeingRetrieved, IMAPSyncStorageProvider.BuildAndLogCloudVersionParseException(state, state.ItemBeingRetrieved.CloudId, state.ItemBeingRetrieved.CloudVersion, true));
					return;
				}
				asyncResult2.PendingAsyncResult = IMAPClient.BeginGetMessageItemByUid(state.ClientState, uid, new AsyncCallback(IMAPSyncStorageProvider.OnEndGetMessageItem), asyncResult2, asyncResult2.SyncPoisonContext);
				return;
			}
		}

		private static void OnEndGetMessageItem(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPSyncStorageProviderState, SyncChangeEntry> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncChangeEntry>)asyncResult.AsyncState;
			IMAPSyncStorageProviderState state = asyncResult2.State;
			Exception ex = null;
			AsyncOperationResult<IMAPResultData> asyncOperationResult = IMAPClient.EndGetMessageItemByUid(asyncResult);
			if (asyncResult.CompletedSynchronously)
			{
				asyncResult2.SetCompletedSynchronously();
			}
			if (!asyncOperationResult.IsSucceeded)
			{
				ex = asyncOperationResult.Exception;
				IMAPUtils.LogExceptionDetails(state.SyncLogSession, IMAPSyncStorageProvider.Tracer, "IMAP Fetch message body failed with error", ex);
			}
			else
			{
				state.SyncLogSession.LogDebugging((TSLID)697UL, IMAPSyncStorageProvider.Tracer, "Fetch message body succeeded.", new object[0]);
				if (state.ItemBeingRetrieved.CloudObject != null)
				{
					if (asyncOperationResult.Data == null || asyncOperationResult.Data.MessageStream == null)
					{
						ex = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, new IMAPInvalidItemException());
					}
					else
					{
						state.ItemBeingRetrieved.SyncObject = (ISyncObject)state.ItemBeingRetrieved.CloudObject;
						state.ItemBeingRetrieved.CloudObject = null;
						(state.ItemBeingRetrieved.SyncObject as IMAPEmail).SetItemProperties(state, asyncOperationResult.Data.MessageStream, (asyncOperationResult.Data.MessageInternalDates != null && asyncOperationResult.Data.MessageInternalDates.Count == 1) ? asyncOperationResult.Data.MessageInternalDates[0] : null);
					}
				}
				else
				{
					string text = "When fetching the body for a new message,CloudObject is null.";
					state.SyncLogSession.LogError((TSLID)698UL, IMAPSyncStorageProvider.Tracer, text, new object[0]);
					ex = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, new IMAPException(text), true);
				}
			}
			if (ex != null && !IMAPSyncStorageProvider.IsOperationError(ex))
			{
				string failureReason = "GetItem can't continue";
				ex = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, new IMAPException(failureReason, ex), true);
			}
			asyncResult2.ProcessCompleted(state.ItemBeingRetrieved, ex);
		}

		private static void ApplyChangeAndContinue(AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult)
		{
			IMAPSyncStorageProviderState state = asyncResult.State;
			IEnumerator<SyncChangeEntry> applyChangesEnumerator = state.ApplyChangesEnumerator;
			SyncChangeEntry syncChangeEntry = applyChangesEnumerator.Current;
			IMAPSyncStorageProvider.DisposeChange(syncChangeEntry);
			bool flag = !applyChangesEnumerator.MoveNext();
			while (!flag)
			{
				syncChangeEntry = applyChangesEnumerator.Current;
				if (syncChangeEntry != null)
				{
					bool flag2 = false;
					try
					{
						switch (syncChangeEntry.SchemaType)
						{
						case SchemaType.Email:
							flag2 = IMAPSyncStorageProvider.ApplyEmailChangeAndContinue(asyncResult, state, syncChangeEntry);
							break;
						case SchemaType.Folder:
							flag2 = IMAPSyncStorageProvider.ApplyFolderChangeAndContinue(asyncResult, state, syncChangeEntry);
							break;
						}
					}
					finally
					{
						if (!flag2)
						{
							IMAPSyncStorageProvider.DisposeChange(syncChangeEntry);
						}
					}
					if (flag2)
					{
						return;
					}
				}
				else
				{
					state.SyncLogSession.LogDebugging((TSLID)699UL, IMAPSyncStorageProvider.Tracer, "ProcessApplyChangesForSyncChangeEntry.  SyncChangeEntry is unexpectedly empty.", new object[0]);
				}
				flag = !applyChangesEnumerator.MoveNext();
			}
			state.SyncLogSession.LogDebugging((TSLID)700UL, IMAPSyncStorageProvider.Tracer, "ProcessApplyChangesForSyncChangeEntry.  No more sync changes to apply.", new object[0]);
			IMAPSyncStorageProvider.DisposeChange(syncChangeEntry);
			if (state.PendingExpungeCloudFolderId != null)
			{
				state.SyncLogSession.LogDebugging((TSLID)701UL, IMAPSyncStorageProvider.Tracer, "Finished applying changes, final expunge before LOGOFF.", new object[0]);
				asyncResult.PendingAsyncResult = IMAPClient.BeginExpunge(state.ClientState, new AsyncCallback(IMAPSyncStorageProvider.OnEndExpungeContinueLogOff), asyncResult, asyncResult.SyncPoisonContext);
				return;
			}
			asyncResult.PendingAsyncResult = IMAPClient.BeginLogOff(state.ClientState, new AsyncCallback(IMAPSyncStorageProvider.OnEndLogOff), asyncResult, asyncResult.SyncPoisonContext);
			state.RequiresLogOff = false;
			asyncResult.ProcessCompleted(new SyncProviderResultData(state.Changes, state.HasPermanentSyncErrors, state.HasTransientSyncErrors), null);
		}

		private static bool ApplyEmailChangeAndContinue(AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult, IMAPSyncStorageProviderState imapState, SyncChangeEntry syncChangeEntry)
		{
			imapState.SyncLogSession.LogDebugging((TSLID)702UL, IMAPSyncStorageProvider.Tracer, "Applying email change: {0}", new object[]
			{
				syncChangeEntry
			});
			if (imapState.CurrentFolder == null || imapState.CurrentFolder.CloudId != syncChangeEntry.CloudFolderId)
			{
				if (imapState.CurrentFolder != null && imapState.CurrentFolder == imapState.LastSelectFailedFolder)
				{
					IMAPSyncStorageProvider.UpdateSyncChangeEntry(imapState, syncChangeEntry, imapState.LastSelectFailedFolderException);
					return false;
				}
				IMAPFolder imapfolder = null;
				if (!imapState.CloudIdToFolder.TryGetValue(syncChangeEntry.CloudFolderId, out imapfolder) || imapfolder == null)
				{
					imapState.SyncLogSession.LogDebugging((TSLID)705UL, IMAPSyncStorageProvider.Tracer, "Creating {0} prior to appending message", new object[]
					{
						syncChangeEntry.CloudFolderId
					});
					asyncResult.PendingAsyncResult = IMAPClient.BeginCreateImapMailbox(imapState.ClientState, syncChangeEntry.CloudFolderId, new AsyncCallback(IMAPSyncStorageProvider.OnEndCreateMailboxContinueApplyEmailChange), asyncResult, asyncResult.SyncPoisonContext);
					return true;
				}
				if (imapfolder.Mailbox.IsWritable)
				{
					imapState.SyncLogSession.LogDebugging((TSLID)703UL, IMAPSyncStorageProvider.Tracer, "Selecting {0} prior to appending message", new object[]
					{
						imapfolder.Mailbox.Name
					});
					imapState.CurrentFolder = imapfolder;
					if (imapState.PendingExpungeCloudFolderId != null)
					{
						asyncResult.PendingAsyncResult = IMAPClient.BeginExpunge(imapState.ClientState, new AsyncCallback(IMAPSyncStorageProvider.OnEndExpungeContinueSelectAndApply), asyncResult, asyncResult.SyncPoisonContext);
					}
					else
					{
						asyncResult.PendingAsyncResult = IMAPClient.BeginSelectImapMailbox(imapState.ClientState, imapState.CurrentFolder.Mailbox, new AsyncCallback(IMAPSyncStorageProvider.OnEndSelectMailboxContinueUpdateValidityAndApply), asyncResult, asyncResult.SyncPoisonContext);
					}
					return true;
				}
				imapState.SyncLogSession.LogDebugging((TSLID)704UL, IMAPSyncStorageProvider.Tracer, "Selected folder is read-only.  ChangeType={0}.  CloudFolderId=[{1}].  CloudId=[{2}].", new object[]
				{
					syncChangeEntry.ChangeType,
					syncChangeEntry.CloudFolderId,
					syncChangeEntry.CloudId
				});
				IMAPSyncStorageProvider.UpdateSyncChangeEntry(imapState, syncChangeEntry, new IMAPException("Folder is read-only.  Can't apply updates."), true);
				return false;
			}
			else
			{
				if (syncChangeEntry.ChangeType == ChangeType.Add)
				{
					asyncResult.PendingAsyncResult = imapState.ItemRetriever.BeginGetItem(imapState.ItemRetrieverState, syncChangeEntry, new AsyncCallback(IMAPSyncStorageProvider.OnEndNativeGetItemContinueApply), asyncResult, asyncResult.SyncPoisonContext);
					return true;
				}
				IMAPMailFlags imapmailFlags = IMAPMailFlags.None;
				if (!IMAPUtils.GetFlagsFromCloudVersion(syncChangeEntry.CloudVersion, imapState.CurrentFolder.Mailbox, out imapmailFlags))
				{
					imapState.SyncLogSession.LogError((TSLID)706UL, IMAPSyncStorageProvider.Tracer, "Unable to parse cloud version for flags - defaulting to none.  CloudId = [{0}].  CloudVersion = [{1}].", new object[]
					{
						syncChangeEntry.CloudId,
						syncChangeEntry.CloudVersion
					});
				}
				IMAPMailFlags flagsToStore;
				switch (syncChangeEntry.ChangeType)
				{
				case ChangeType.Change:
					flagsToStore = IMAPSyncStorageProvider.GetFlagsFromSyncEmail((ISyncEmail)syncChangeEntry.SyncObject, imapState.CurrentFolder.Mailbox);
					goto IL_2E8;
				case ChangeType.Delete:
					flagsToStore = (IMAPMailFlags.Deleted | imapmailFlags);
					goto IL_2E8;
				case ChangeType.ReadFlagChange:
					flagsToStore = (imapmailFlags ^ IMAPMailFlags.Seen);
					goto IL_2E8;
				}
				return false;
				IL_2E8:
				if (syncChangeEntry.CloudVersion == "ORPHANED")
				{
					IMAPSyncStorageProvider.UpdateSyncChangeEntry(imapState, syncChangeEntry, IMAPSyncStorageProvider.BuildAndLogOrphanedItemApplyException(imapState, syncChangeEntry.CloudId));
					return false;
				}
				string uid;
				if (!IMAPUtils.GetUidFromEmailCloudVersion(syncChangeEntry.CloudVersion, out uid))
				{
					IMAPSyncStorageProvider.UpdateSyncChangeEntry(imapState, syncChangeEntry, IMAPSyncStorageProvider.BuildAndLogCloudVersionParseException(imapState, syncChangeEntry.CloudId, syncChangeEntry.CloudVersion, false));
					return false;
				}
				asyncResult.PendingAsyncResult = IMAPClient.BeginStoreMessageFlags(imapState.ClientState, uid, flagsToStore, imapmailFlags, new AsyncCallback(IMAPSyncStorageProvider.OnEndStoreMessageFlagsContinueApply), asyncResult, asyncResult.SyncPoisonContext);
				return true;
			}
		}

		private static bool ApplyFolderChangeAndContinue(AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult, IMAPSyncStorageProviderState imapState, SyncChangeEntry syncChangeEntry)
		{
			imapState.SyncLogSession.LogDebugging((TSLID)707UL, IMAPSyncStorageProvider.Tracer, "Applying folder change: {0}", new object[]
			{
				syncChangeEntry
			});
			switch (syncChangeEntry.ChangeType)
			{
			case ChangeType.Add:
			{
				IMAPFolder imapfolder;
				if (syncChangeEntry.CloudFolderId == null)
				{
					imapState.SyncLogSession.LogError((TSLID)708UL, IMAPSyncStorageProvider.Tracer, "ApplyFolderChangeAndContinue.  Unable to associate new folder with parent, since parent does not have a cloud folder id.  Parent has not been acknowledged on native side.  {0}", new object[]
					{
						syncChangeEntry
					});
					IMAPSyncStorageProvider.UpdateSyncChangeEntry(imapState, syncChangeEntry, new IMAPException("Unable to associate new folder with parent, since parent does not have a cloud folder id.  Parent has not been acknowledged on native side."), false);
				}
				else if (imapState.CloudIdToFolder.TryGetValue(syncChangeEntry.CloudFolderId, out imapfolder) && imapfolder != null && imapfolder.Mailbox.NoInferiors)
				{
					imapState.SyncLogSession.LogError((TSLID)709UL, IMAPSyncStorageProvider.Tracer, "ApplyFolderChangeAndContinue.  Can not create child folder of this parent.  NoInferiors set on parent.  ParentCloudFolderId={0}", new object[]
					{
						imapfolder.CloudId
					});
					IMAPSyncStorageProvider.UpdateSyncChangeEntry(imapState, syncChangeEntry, new IMAPException("Can not create child folder of this parent.  NoInferiors set on parent."), false);
				}
				else
				{
					string text = IMAPFolder.CreateNewMailboxName(imapState.GetSeparatorCharacter(syncChangeEntry.CloudFolderId), syncChangeEntry.SyncObject as SyncFolder, syncChangeEntry.CloudFolderId);
					if (text != null && !string.Equals(text, "INBOX", StringComparison.OrdinalIgnoreCase))
					{
						string text2 = text;
						syncChangeEntry.CloudId = text2;
						IMAPFolder imapfolder2;
						if (!imapState.CloudIdToFolder.TryGetValue(text2, out imapfolder2) || imapfolder2 == null)
						{
							asyncResult.PendingAsyncResult = IMAPClient.BeginCreateImapMailbox(imapState.ClientState, text, new AsyncCallback(IMAPSyncStorageProvider.OnEndCreateMailboxContinueApply), asyncResult, asyncResult.SyncPoisonContext);
							return true;
						}
						syncChangeEntry.CloudVersion = imapfolder2.GenerateFolderCloudVersion();
						imapState.SyncLogSession.LogVerbose((TSLID)710UL, IMAPSyncStorageProvider.Tracer, "ApplyFolderChangeAndContinue.  Skip creation of IMAP mailbox because it already exists!  Fake success.  CloudId=[{0}].  CloudFolderId=[{1}].", new object[]
						{
							syncChangeEntry.CloudId,
							syncChangeEntry.CloudFolderId
						});
					}
					else
					{
						imapState.SyncLogSession.LogError((TSLID)711UL, IMAPSyncStorageProvider.Tracer, "ApplyFolderChangeAndContinue.  Unable to create folder, separator or mailbox name invalid: {0}", new object[]
						{
							syncChangeEntry
						});
						IMAPSyncStorageProvider.UpdateSyncChangeEntry(imapState, syncChangeEntry, new IMAPException("Failure to add new folder.  Invalid separator or mailbox name."));
					}
				}
				return false;
			}
			case ChangeType.Change:
			{
				string text3 = syncChangeEntry.NewCloudFolderId ?? syncChangeEntry.CloudFolderId;
				string text4 = IMAPFolder.CreateNewMailboxName(imapState.GetSeparatorCharacter(text3), syncChangeEntry.SyncObject as SyncFolder, text3);
				if (text4 != null && imapState.CloudIdToFolder.ContainsKey(syncChangeEntry.CloudId))
				{
					string cloudId = syncChangeEntry.CloudId;
					syncChangeEntry.NewCloudId = text4;
					asyncResult.PendingAsyncResult = IMAPClient.BeginRenameImapMailbox(imapState.ClientState, cloudId, text4, new AsyncCallback(IMAPSyncStorageProvider.OnEndRenameMailboxContinueApply), asyncResult, asyncResult.SyncPoisonContext);
					return true;
				}
				string text5 = text4;
				IMAPFolder imapfolder3;
				if (imapState.CloudIdToFolder.TryGetValue(text5, out imapfolder3))
				{
					syncChangeEntry.CloudVersion = imapfolder3.GenerateFolderCloudVersion();
					imapState.SyncLogSession.LogVerbose((TSLID)713UL, IMAPSyncStorageProvider.Tracer, "ApplyFolderChangeAndContinue.  Skip rename of IMAP mailbox because new mailbox already exists!  Fake success.  Old={0}.  New={1}.", new object[]
					{
						syncChangeEntry.CloudId,
						text5
					});
					return false;
				}
				imapState.SyncLogSession.LogInformation((TSLID)714UL, IMAPSyncStorageProvider.Tracer, "ApplyFolderChangeAndContinue.  Unable to rename folder, old mailbox no longer exists.  Old={0}.  New={1}.", new object[]
				{
					syncChangeEntry.CloudId,
					text5
				});
				IMAPSyncStorageProvider.UpdateSyncChangeEntry(imapState, syncChangeEntry, new IMAPException("Failure to rename/move folder."));
				return false;
			}
			case ChangeType.Delete:
			{
				IMAPFolder imapfolder4;
				if (imapState.CloudIdToFolder.TryGetValue(syncChangeEntry.CloudId, out imapfolder4) && imapfolder4 != null)
				{
					if (imapfolder4.Mailbox.HasChildren == null || imapfolder4.Mailbox.HasChildren.Value)
					{
						if (imapState.ApplyDeleteFolders == null)
						{
							imapState.ApplyDeleteFolders = new Queue<IMAPFolder>(5);
						}
						else
						{
							imapState.ApplyDeleteFolders.Clear();
						}
						foreach (IMAPFolder imapfolder5 in imapState.CloudIdToFolder.Values)
						{
							if (imapfolder5.IsChildOfCloudFolder(imapfolder4.CloudId))
							{
								imapState.ApplyDeleteFolders.Enqueue(imapfolder5);
							}
						}
					}
					asyncResult.PendingAsyncResult = IMAPClient.BeginDeleteImapMailbox(imapState.ClientState, imapfolder4.Mailbox.Name, new AsyncCallback(IMAPSyncStorageProvider.OnEndDeleteMailboxContinueApply), asyncResult, asyncResult.SyncPoisonContext);
					return true;
				}
				syncChangeEntry.CloudVersion = null;
				imapState.SyncLogSession.LogVerbose((TSLID)712UL, IMAPSyncStorageProvider.Tracer, "ApplyFolderChangeAndContinue.  Skip creation of IMAP mailbox because it already exists!  Fake success.  CloudId=[{0}].  CloudFolderId=[{1}].", new object[]
				{
					syncChangeEntry.CloudId,
					syncChangeEntry.CloudFolderId
				});
				return false;
			}
			}
			return false;
		}

		private static void OnEndExpungeContinueSelectAndApply(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>)asyncResult.AsyncState;
			IMAPSyncStorageProviderState state = asyncResult2.State;
			AsyncOperationResult<DBNull> asyncOperationResult = IMAPClient.EndExpunge(asyncResult);
			SyncChangeEntry syncChangeEntry = state.ApplyChangesEnumerator.Current;
			if (!asyncOperationResult.IsSucceeded)
			{
				IMAPSyncStorageProvider.RollbackDeletesOnFailedExpunge(state);
			}
			state.PendingExpungeCloudFolderId = null;
			state.SyncLogSession.LogDebugging((TSLID)715UL, IMAPSyncStorageProvider.Tracer, "IMAP Folder expunge succeeded.  Continuing select prior to applying changes.", new object[0]);
			asyncResult2.PendingAsyncResult = IMAPClient.BeginSelectImapMailbox(state.ClientState, state.CurrentFolder.Mailbox, new AsyncCallback(IMAPSyncStorageProvider.OnEndSelectMailboxContinueUpdateValidityAndApply), asyncResult2, asyncResult2.SyncPoisonContext);
		}

		private static void OnEndCreateMailboxContinueApplyEmailChange(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>)asyncResult.AsyncState;
			IMAPSyncStorageProviderState state = asyncResult2.State;
			AsyncOperationResult<DBNull> asyncOperationResult = IMAPClient.EndCreateImapMailbox(asyncResult);
			SyncChangeEntry syncChangeEntry = state.ApplyChangesEnumerator.Current;
			if (asyncOperationResult.IsSucceeded)
			{
				IMAPMailbox mailbox = new IMAPMailbox(syncChangeEntry.CloudFolderId);
				IMAPFolder imapfolder = new IMAPFolder(mailbox);
				if (!state.StateStorage.TryUpdateFolderCloudVersion(syncChangeEntry.CloudFolderId, imapfolder.GenerateFolderCloudVersion()))
				{
					state.SyncLogSession.LogError((TSLID)716UL, IMAPSyncStorageProvider.Tracer, "Failed to update cloud version for folder {0} after creating during email change application.", new object[]
					{
						syncChangeEntry.CloudFolderId
					});
				}
				state.CloudIdToFolder.Add(syncChangeEntry.CloudFolderId, imapfolder);
				state.SyncLogSession.LogDebugging((TSLID)717UL, IMAPSyncStorageProvider.Tracer, "Created {0}, continuing to apply email SCE.", new object[]
				{
					syncChangeEntry.CloudFolderId
				});
				IMAPSyncStorageProvider.ApplyEmailChangeAndContinue(asyncResult2, state, syncChangeEntry);
				return;
			}
			IMAPSyncStorageProvider.UpdateSyncChangeEntry(state, syncChangeEntry, asyncOperationResult.Exception);
			if (IMAPSyncStorageProvider.IsOperationError(asyncOperationResult.Exception))
			{
				IMAPSyncStorageProvider.AbortApply(asyncResult2, state, asyncOperationResult.Exception);
				return;
			}
			IMAPSyncStorageProvider.ApplyChangeAndContinue(asyncResult2);
		}

		private static void OnEndSelectMailboxContinueUpdateValidityAndApply(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>)asyncResult.AsyncState;
			IMAPSyncStorageProviderState state = asyncResult2.State;
			AsyncOperationResult<IMAPMailbox> asyncOperationResult = IMAPClient.EndSelectImapMailbox(asyncResult);
			SyncChangeEntry syncEntry = state.ApplyChangesEnumerator.Current;
			if (asyncOperationResult.IsSucceeded)
			{
				state.SyncLogSession.LogDebugging((TSLID)718UL, IMAPSyncStorageProvider.Tracer, "IMAP Folder selection succeeded.  Continuing apply changes operation.", new object[0]);
				state.CurrentFolder.Mailbox = asyncOperationResult.Data;
				IMAPSyncStorageProvider.UpdateValidityReconcileAndContinue<SyncProviderResultData>(asyncResult2, new IMAPSyncStorageProviderState.PostProcessor(IMAPSyncStorageProvider.OnEndUpdateValidityContinueApply));
				return;
			}
			IMAPSyncStorageProvider.UpdateSyncChangeEntry(state, syncEntry, asyncOperationResult.Exception);
			IMAPSyncStorageProvider.FailSelect(state, asyncOperationResult.Exception, false);
			if (IMAPSyncStorageProvider.IsOperationError(asyncOperationResult.Exception))
			{
				IMAPSyncStorageProvider.AbortApply(asyncResult2, state, asyncOperationResult.Exception);
				return;
			}
			IMAPSyncStorageProvider.ApplyChangeAndContinue(asyncResult2);
		}

		private static void OnEndUpdateValidityContinueApply(IAsyncResult asyncResult, Exception exceptionDuringReconciliation)
		{
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>)asyncResult;
			IMAPSyncStorageProviderState state = asyncResult2.State;
			if (exceptionDuringReconciliation == null)
			{
				if (!IMAPSyncStorageProvider.ApplyEmailChangeAndContinue(asyncResult2, state, state.ApplyChangesEnumerator.Current))
				{
					IMAPSyncStorageProvider.ApplyChangeAndContinue(asyncResult2);
				}
				return;
			}
			IMAPUtils.LogExceptionDetails(state.SyncLogSession, IMAPSyncStorageProvider.Tracer, "Failed during UIDValidity change reconciliation", exceptionDuringReconciliation);
			if (IMAPSyncStorageProvider.IsOperationError(exceptionDuringReconciliation))
			{
				IMAPSyncStorageProvider.AbortApply(asyncResult2, state, exceptionDuringReconciliation);
				return;
			}
			IMAPSyncStorageProvider.ApplyChangeAndContinue(asyncResult2);
		}

		private static void OnEndCreateMailboxContinueApply(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>)asyncResult.AsyncState;
			IMAPSyncStorageProviderState state = asyncResult2.State;
			AsyncOperationResult<DBNull> asyncOperationResult = IMAPClient.EndCreateImapMailbox(asyncResult);
			SyncChangeEntry syncChangeEntry = state.ApplyChangesEnumerator.Current;
			if (!asyncOperationResult.IsSucceeded)
			{
				state.SyncLogSession.LogError((TSLID)719UL, IMAPSyncStorageProvider.Tracer, "Failed to create IMAP mailbox: {0}", new object[]
				{
					asyncOperationResult.Exception
				});
				if (IMAPSyncStorageProvider.IsOperationError(asyncOperationResult.Exception))
				{
					IMAPSyncStorageProvider.AbortApply(asyncResult2, state, asyncOperationResult.Exception);
					return;
				}
				IMAPSyncStorageProvider.UpdateSyncChangeEntry(state, syncChangeEntry, asyncOperationResult.Exception);
			}
			else
			{
				IMAPMailbox mailbox = new IMAPMailbox(syncChangeEntry.CloudId);
				IMAPFolder imapfolder = new IMAPFolder(mailbox);
				syncChangeEntry.CloudVersion = imapfolder.GenerateFolderCloudVersion();
				state.CloudIdToFolder[syncChangeEntry.CloudId] = imapfolder;
				state.SyncLogSession.LogDebugging((TSLID)720UL, IMAPSyncStorageProvider.Tracer, "Created IMAP mailbox. Mailbox=[{0}]", new object[]
				{
					syncChangeEntry.CloudId
				});
			}
			IMAPSyncStorageProvider.ApplyChangeAndContinue(asyncResult2);
		}

		private static void OnEndDeleteMailboxContinueApply(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>)asyncResult.AsyncState;
			IMAPSyncStorageProviderState state = asyncResult2.State;
			AsyncOperationResult<DBNull> asyncOperationResult = IMAPClient.EndDeleteImapMailbox(asyncResult);
			SyncChangeEntry syncChangeEntry = state.ApplyChangesEnumerator.Current;
			if (!asyncOperationResult.IsSucceeded)
			{
				state.SyncLogSession.LogError((TSLID)721UL, IMAPSyncStorageProvider.Tracer, "Failed to delete IMAP mailbox: [{0}]", new object[]
				{
					asyncOperationResult.Exception
				});
				if (IMAPSyncStorageProvider.IsOperationError(asyncOperationResult.Exception))
				{
					IMAPSyncStorageProvider.AbortApply(asyncResult2, state, asyncOperationResult.Exception);
					return;
				}
				IMAPSyncStorageProvider.UpdateSyncChangeEntry(state, syncChangeEntry, asyncOperationResult.Exception);
			}
			else
			{
				syncChangeEntry.CloudVersion = null;
				state.CloudIdToFolder.Remove(syncChangeEntry.CloudId);
				state.SyncLogSession.LogDebugging((TSLID)722UL, IMAPSyncStorageProvider.Tracer, "Deleted IMAP mailbox. Mailbox=[{0}]", new object[]
				{
					syncChangeEntry.CloudId
				});
			}
			if (state.ApplyDeleteFolders != null && state.ApplyDeleteFolders.Count > 0)
			{
				IMAPFolder imapfolder = state.ApplyDeleteFolders.Dequeue();
				state.SyncLogSession.LogVerbose((TSLID)723UL, IMAPSyncStorageProvider.Tracer, "Deleting nested children IMAP mailbox. Mailbox=[{0}]", new object[]
				{
					imapfolder.Mailbox.Name
				});
				asyncResult2.PendingAsyncResult = IMAPClient.BeginDeleteImapMailbox(state.ClientState, imapfolder.Mailbox.Name, new AsyncCallback(IMAPSyncStorageProvider.OnEndDeleteMailboxContinueApply), asyncResult2, asyncResult2.SyncPoisonContext);
				return;
			}
			IMAPSyncStorageProvider.ApplyChangeAndContinue(asyncResult2);
		}

		private static void OnEndRenameMailboxContinueApply(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>)asyncResult.AsyncState;
			IMAPSyncStorageProviderState state = asyncResult2.State;
			AsyncOperationResult<DBNull> asyncOperationResult = IMAPClient.EndRenameImapMailbox(asyncResult);
			SyncChangeEntry syncChangeEntry = state.ApplyChangesEnumerator.Current;
			if (!asyncOperationResult.IsSucceeded)
			{
				state.SyncLogSession.LogError((TSLID)724UL, IMAPSyncStorageProvider.Tracer, "Failed to rename IMAP mailbox: {0}", new object[]
				{
					asyncOperationResult.Exception
				});
				if (IMAPSyncStorageProvider.IsOperationError(asyncOperationResult.Exception))
				{
					IMAPSyncStorageProvider.AbortApply(asyncResult2, state, asyncOperationResult.Exception);
					return;
				}
				IMAPSyncStorageProvider.UpdateSyncChangeEntry(state, syncChangeEntry, asyncOperationResult.Exception);
			}
			else
			{
				string newCloudId = syncChangeEntry.NewCloudId;
				string cloudId = syncChangeEntry.CloudId;
				IMAPFolder imapfolder = state.CloudIdToFolder[syncChangeEntry.CloudId];
				state.CloudIdToFolder.Remove(syncChangeEntry.CloudId);
				imapfolder.RenameMailboxName(newCloudId);
				state.CloudIdToFolder[syncChangeEntry.NewCloudId] = imapfolder;
				string cloudId2 = syncChangeEntry.CloudId;
				string newCloudId2 = syncChangeEntry.NewCloudId;
				List<IMAPFolder> list = new List<IMAPFolder>(state.CloudIdToFolder.Values);
				foreach (IMAPFolder imapfolder2 in list)
				{
					if (imapfolder2.IsChildOfCloudFolder(cloudId2))
					{
						string text;
						string cloudVersion;
						if (state.StateStorage.TryFindFolder(imapfolder2.CloudId, out text, out cloudVersion))
						{
							string cloudId3 = imapfolder2.CloudId;
							imapfolder2.ReparentMailboxName(cloudId2, newCloudId2);
							state.CloudIdToFolder.Remove(cloudId3);
							state.CloudIdToFolder[imapfolder2.CloudId] = imapfolder2;
							string text2;
							if (imapfolder2.TryGetParentCloudFolderId(state.GetSeparatorCharacter(imapfolder2), imapfolder2.CloudId, out text2) && text2 != null && state.StateStorage.TryUpdateFolder(state.Subscription, cloudId3, imapfolder2.CloudId, cloudVersion))
							{
								state.SyncLogSession.LogDebugging((TSLID)725UL, IMAPSyncStorageProvider.Tracer, "Updated Folder CloudId for child.  NewCloudFolderId=[{0}].  OldParent=[{1}].  NewParent=[{2}]", new object[]
								{
									imapfolder2.CloudId,
									cloudId2,
									newCloudId2
								});
							}
							else
							{
								state.SyncLogSession.LogDebugging((TSLID)726UL, IMAPSyncStorageProvider.Tracer, "Updating Folder CloudId failed.  NewCloudFolderId=[{0}].  OldParent=[{1}].  NewParent=[{2}]", new object[]
								{
									imapfolder2.CloudId,
									cloudId2,
									newCloudId2
								});
							}
						}
						else
						{
							state.SyncLogSession.LogError((TSLID)727UL, IMAPSyncStorageProvider.Tracer, "Could not find child folder! Unexpected.  CloudFolderId=[{0}].", new object[]
							{
								imapfolder2.CloudId
							});
						}
					}
				}
			}
			IMAPSyncStorageProvider.ApplyChangeAndContinue(asyncResult2);
		}

		private static void OnEndNativeGetItemContinueApply(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>)asyncResult.AsyncState;
			IMAPSyncStorageProviderState state = asyncResult2.State;
			AsyncOperationResult<SyncChangeEntry> asyncOperationResult = state.ItemRetriever.EndGetItem(asyncResult);
			SyncChangeEntry syncChangeEntry = state.ApplyChangesEnumerator.Current;
			if (!asyncOperationResult.IsSucceeded)
			{
				IMAPSyncStorageProvider.UpdateSyncChangeEntry(state, syncChangeEntry, asyncOperationResult.Exception);
				state.SyncLogSession.LogError((TSLID)728UL, IMAPSyncStorageProvider.Tracer, "Failed to retrieve ApplyChanges mail item: {0}", new object[]
				{
					asyncOperationResult.Exception
				});
			}
			else
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest(3882233149U);
				state.SyncLogSession.LogDebugging((TSLID)729UL, IMAPSyncStorageProvider.Tracer, "Adding Message to IMAP Server", new object[0]);
				ISyncEmail syncEmail = syncChangeEntry.SyncObject as ISyncEmail;
				if (syncEmail != null && syncEmail.MimeStream != null)
				{
					long length = syncEmail.MimeStream.Length;
					if (length > state.MaxDownloadSizePerItem)
					{
						state.SyncLogSession.LogError((TSLID)730UL, IMAPSyncStorageProvider.Tracer, "Failed to append message, exceeds max bytes per item.  CloudId=[{0}].  CloudVersionId=[{1}].  Message Length={2}.  MaxDownloadSizePerItem={3}.", new object[]
						{
							syncChangeEntry.CloudId,
							syncChangeEntry.CloudVersion,
							length,
							state.MaxDownloadSizePerItem
						});
						IMAPSyncStorageProvider.UpdateSyncChangeEntry(state, syncChangeEntry, new MessageSizeLimitExceededException(new IMAPException("Failed to append message, exceeds max bytes per item.")), false);
					}
					else if (state.TotalBytesSent + length > state.MaxDownloadBytesAllowed)
					{
						state.SyncLogSession.LogError((TSLID)731UL, IMAPSyncStorageProvider.Tracer, "Failed to append message, exceeds max bytes per session.  CloudId=[{0}].  CloudVersionId=[{1}].  TotalBytesSent={2}.  Message Length={3}.  MaxDownloadBytesAllowed={4}.", new object[]
						{
							syncChangeEntry.CloudId,
							syncChangeEntry.CloudVersion,
							state.TotalBytesSent,
							length,
							state.MaxDownloadBytesAllowed
						});
						IMAPSyncStorageProvider.UpdateSyncChangeEntry(state, syncChangeEntry, new IMAPException("Failed to append message, exceeds max bytes per session."), true);
					}
					else
					{
						IMAPMailFlags flagsFromSyncEmail = IMAPSyncStorageProvider.GetFlagsFromSyncEmail(syncEmail, state.CurrentFolder.Mailbox);
						state.LastAppendMessageFlags = flagsFromSyncEmail;
						string cloudId;
						Stream stream;
						state.LastAppendMessageId = IMAPSyncStorageProvider.FindOrReplaceMessageId(state, syncEmail, syncChangeEntry, out cloudId, out stream);
						syncChangeEntry.CloudId = cloudId;
						if (state.UnderRecovery && syncChangeEntry != state.LastAppendMessageChange)
						{
							state.LastAppendMessageChange = syncChangeEntry;
							state.LastAppendMessageMimeStream = stream;
							asyncResult2.PendingAsyncResult = IMAPClient.BeginSearchForMessageByMessageId(state.ClientState, state.LastAppendMessageId, new AsyncCallback(IMAPSyncStorageProvider.OnEndSearchForMessageUnderRecoveryContinueApply), asyncResult2, asyncResult2.SyncPoisonContext);
							return;
						}
						asyncResult2.PendingAsyncResult = IMAPClient.BeginAppendMessageToIMAPMailbox(state.ClientState, state.CurrentFolder.Name, flagsFromSyncEmail, stream, new AsyncCallback(IMAPSyncStorageProvider.OnEndAppendMessageContinueApply), asyncResult2, asyncResult2.SyncPoisonContext);
						return;
					}
				}
				else
				{
					IMAPSyncStorageProvider.UpdateSyncChangeEntry(state, syncChangeEntry, new IMAPException("Unable to retrieve XSO Email"));
					state.SyncLogSession.LogError((TSLID)732UL, IMAPSyncStorageProvider.Tracer, "Failed to fetch Native message.  CloudId=[{0}].  CloudVersion=[{1}].", new object[]
					{
						syncChangeEntry.CloudId,
						syncChangeEntry.CloudVersion
					});
				}
			}
			IMAPSyncStorageProvider.ApplyChangeAndContinue(asyncResult2);
		}

		private static void OnEndStoreMessageFlagsContinueApply(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>)asyncResult.AsyncState;
			IMAPSyncStorageProviderState state = asyncResult2.State;
			AsyncOperationResult<DBNull> asyncOperationResult = IMAPClient.EndStoreMessageFlags(asyncResult);
			SyncChangeEntry syncChangeEntry = state.ApplyChangesEnumerator.Current;
			if (!asyncOperationResult.IsSucceeded)
			{
				if (state.UnderRecovery && syncChangeEntry.ChangeType == ChangeType.Delete)
				{
					state.SyncLogSession.LogVerbose((TSLID)733UL, IMAPSyncStorageProvider.Tracer, "Storing deleted flag on item failed under recovery sync.  Treating as successful.  CloudId=[{0}].  CloudVersion=[{1}].", new object[]
					{
						syncChangeEntry.CloudId,
						syncChangeEntry.CloudVersion
					});
				}
				else
				{
					IMAPUtils.LogExceptionDetails(state.SyncLogSession, IMAPSyncStorageProvider.Tracer, "Storing message flags failed", asyncOperationResult.Exception);
					if (IMAPSyncStorageProvider.IsOperationError(asyncOperationResult.Exception))
					{
						IMAPSyncStorageProvider.AbortApply(asyncResult2, state, asyncOperationResult.Exception);
						return;
					}
					IMAPSyncStorageProvider.UpdateSyncChangeEntry(state, syncChangeEntry, asyncOperationResult.Exception);
				}
			}
			else
			{
				switch (syncChangeEntry.ChangeType)
				{
				case ChangeType.Change:
				case ChangeType.ReadFlagChange:
				{
					IMAPMailFlags flags;
					if (syncChangeEntry.ChangeType == ChangeType.Change)
					{
						flags = IMAPSyncStorageProvider.GetFlagsFromSyncEmail((ISyncEmail)syncChangeEntry.SyncObject, state.CurrentFolder.Mailbox);
					}
					else
					{
						IMAPMailFlags imapmailFlags = IMAPMailFlags.None;
						if (!IMAPUtils.GetFlagsFromCloudVersion(syncChangeEntry.CloudVersion, state.CurrentFolder.Mailbox, out imapmailFlags))
						{
							state.SyncLogSession.LogError((TSLID)734UL, IMAPSyncStorageProvider.Tracer, "Unable to parse cloud version for flags - defaulting to none.  CloudId = [{0}].  CloudVersion = [{1}].", new object[]
							{
								syncChangeEntry.CloudId,
								syncChangeEntry.CloudVersion
							});
						}
						flags = (imapmailFlags ^ IMAPMailFlags.Seen);
					}
					string cloudVersion = syncChangeEntry.CloudVersion;
					if (IMAPUtils.TryUpdateFlagsInCloudVersion(flags, state.CurrentFolder.Mailbox, ref cloudVersion))
					{
						if (state.StateStorage.TryUpdateItemCloudVersion(syncChangeEntry.CloudId, cloudVersion))
						{
							syncChangeEntry.CloudVersion = cloudVersion;
							state.SyncLogSession.LogDebugging((TSLID)735UL, IMAPSyncStorageProvider.Tracer, "Setting new item cloud version.  CloudId = [{0}].  CloudVersion = [{1}].", new object[]
							{
								syncChangeEntry.CloudId,
								cloudVersion
							});
						}
						else
						{
							IMAPSyncStorageProvider.UpdateSyncChangeEntry(state, syncChangeEntry, new IMAPException("Failed to set cloud version"));
							state.SyncLogSession.LogError((TSLID)736UL, IMAPSyncStorageProvider.Tracer, "Failed to set new item cloud version.  CloudId = [{0}].  CloudVersion = [{1}].", new object[]
							{
								syncChangeEntry.CloudId,
								cloudVersion
							});
						}
					}
					else
					{
						IMAPSyncStorageProvider.UpdateSyncChangeEntry(state, syncChangeEntry, new IMAPException("Invalid cloud version"));
						state.SyncLogSession.LogError((TSLID)737UL, IMAPSyncStorageProvider.Tracer, "Unable to parse cloud version to add flags.  CloudId=[{0}].  CloudVersion=[{1}].", new object[]
						{
							syncChangeEntry.CloudId,
							syncChangeEntry.CloudVersion
						});
					}
					break;
				}
				case ChangeType.Delete:
					state.PendingExpungeCloudFolderId = state.CurrentFolder.CloudId;
					break;
				}
			}
			IMAPSyncStorageProvider.ApplyChangeAndContinue(asyncResult2);
		}

		private static void OnEndAppendMessageContinueApply(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>)asyncResult.AsyncState;
			IMAPSyncStorageProviderState state = asyncResult2.State;
			AsyncOperationResult<string> asyncOperationResult = IMAPClient.EndAppendMessageToIMAPMailbox(asyncResult);
			SyncChangeEntry syncChangeEntry = state.ApplyChangesEnumerator.Current;
			if (!asyncOperationResult.IsSucceeded)
			{
				state.SyncLogSession.LogError((TSLID)738UL, IMAPSyncStorageProvider.Tracer, "Append message failed.  Exception={0}", new object[]
				{
					asyncOperationResult.Exception
				});
				if (IMAPSyncStorageProvider.IsOperationError(asyncOperationResult.Exception))
				{
					IMAPSyncStorageProvider.AbortApply(asyncResult2, state, asyncOperationResult.Exception);
					return;
				}
				IMAPSyncStorageProvider.UpdateSyncChangeEntry(state, syncChangeEntry, asyncOperationResult.Exception);
			}
			else
			{
				if (string.IsNullOrEmpty(asyncOperationResult.Data))
				{
					asyncResult2.PendingAsyncResult = IMAPClient.BeginSearchForMessageByMessageId(state.ClientState, state.LastAppendMessageId, new AsyncCallback(IMAPSyncStorageProvider.OnEndSearchForMessageContinueApply), asyncResult2, asyncResult2.SyncPoisonContext);
					return;
				}
				string data = asyncOperationResult.Data;
				syncChangeEntry.CloudVersion = IMAPUtils.CreateEmailCloudVersion(state.CurrentFolder, data, state.LastAppendMessageFlags);
			}
			IMAPSyncStorageProvider.ApplyChangeAndContinue(asyncResult2);
		}

		private static void OnEndSearchForMessageUnderRecoveryContinueApply(IAsyncResult asyncResult)
		{
			IMAPSyncStorageProvider.OnEndSearchForMessageContinueApply(asyncResult, true);
		}

		private static void OnEndSearchForMessageContinueApply(IAsyncResult asyncResult)
		{
			IMAPSyncStorageProvider.OnEndSearchForMessageContinueApply(asyncResult, false);
		}

		private static void OnEndSearchForMessageContinueApply(IAsyncResult asyncResult, bool underRecovery)
		{
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>)asyncResult.AsyncState;
			IMAPSyncStorageProviderState state = asyncResult2.State;
			AsyncOperationResult<IList<string>> asyncOperationResult = IMAPClient.EndSearchForMessageByMessageId(asyncResult);
			SyncChangeEntry syncChangeEntry = state.ApplyChangesEnumerator.Current;
			bool flag = false;
			if (!asyncOperationResult.IsSucceeded)
			{
				state.SyncLogSession.LogError((TSLID)739UL, IMAPSyncStorageProvider.Tracer, "Search for message-id for message failed.  Exception={0}", new object[]
				{
					asyncOperationResult.Exception
				});
				if (IMAPSyncStorageProvider.IsOperationError(asyncOperationResult.Exception))
				{
					IMAPSyncStorageProvider.AbortApply(asyncResult2, state, asyncOperationResult.Exception);
					return;
				}
				IMAPSyncStorageProvider.UpdateSyncChangeEntry(state, syncChangeEntry, asyncOperationResult.Exception);
			}
			else if (asyncOperationResult.Data.Count == 1)
			{
				string text = asyncOperationResult.Data[0];
				if (text != null)
				{
					syncChangeEntry.CloudVersion = IMAPUtils.CreateEmailCloudVersion(state.CurrentFolder, text, state.LastAppendMessageFlags);
					flag = true;
				}
				else
				{
					state.SyncLogSession.LogError((TSLID)740UL, IMAPSyncStorageProvider.Tracer, "Search for message-id for message failed.  Uid missing in result.", new object[0]);
					IMAPSyncStorageProvider.UpdateSyncChangeEntry(state, syncChangeEntry, new IMAPException("Uid not returned when searching by message-id."));
				}
			}
			else if (asyncOperationResult.Data.Count == 0)
			{
				if (!underRecovery)
				{
					syncChangeEntry.CloudVersion = "ORPHANED";
				}
			}
			else if (asyncOperationResult.Data.Count > 1)
			{
				state.SyncLogSession.LogError((TSLID)741UL, IMAPSyncStorageProvider.Tracer, "Search for message-id retrieved multiple matches.  Failed, can't reconcile.", new object[0]);
				IMAPSyncStorageProvider.UpdateSyncChangeEntry(state, syncChangeEntry, new IMAPException("Multiple Uids returned when searching by message-id."));
			}
			if (underRecovery && !flag && syncChangeEntry.Exception == null)
			{
				Stream lastAppendMessageMimeStream = state.LastAppendMessageMimeStream;
				state.LastAppendMessageMimeStream = null;
				asyncResult2.PendingAsyncResult = IMAPClient.BeginAppendMessageToIMAPMailbox(state.ClientState, state.CurrentFolder.Name, state.LastAppendMessageFlags, lastAppendMessageMimeStream, new AsyncCallback(IMAPSyncStorageProvider.OnEndAppendMessageContinueApply), asyncResult2, asyncResult2.SyncPoisonContext);
				return;
			}
			IMAPSyncStorageProvider.ApplyChangeAndContinue(asyncResult2);
		}

		private static void StartEnumeratingCloudChangesByFolder(AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> curOp, IMAPSyncStorageProviderState state)
		{
			state.Changes = new List<SyncChangeEntry>(5);
			state.HasTransientSyncErrors = false;
			state.HasPermanentSyncErrors = false;
			state.LastSelectFailedFolder = null;
			state.CurrentFolder = null;
			if (!state.StateStorage.ContainsFolder(IMAPFolder.RootCloudFolderId))
			{
				state.SyncLogSession.LogDebugging((TSLID)742UL, IMAPSyncStorageProvider.Tracer, "IMAP SyncChangeEntry to Add Folder: ROOT.", new object[0]);
				SyncChangeEntry syncChangeEntry = new SyncChangeEntry(ChangeType.Add, SchemaType.Folder, IMAPFolder.RootCloudFolderId);
				syncChangeEntry.CloudFolderId = null;
				syncChangeEntry.SyncObject = new SyncFolder(string.Empty, DefaultFolderType.Root);
				state.Changes.Add(syncChangeEntry);
			}
			SortedList<string, string> sortedList = new SortedList<string, string>(5, new IMAPSyncStorageProvider.FolderComparer(state.GetSeparatorCharacter(), true));
			IEnumerator<string> cloudFolderEnumerator = state.StateStorage.GetCloudFolderEnumerator();
			while (cloudFolderEnumerator.MoveNext())
			{
				sortedList.Add(cloudFolderEnumerator.Current, cloudFolderEnumerator.Current);
			}
			state.CloudFolderEnumerator = sortedList.Keys.GetEnumerator();
			state.SortedFolderAddsSyncChangeEntries = null;
			IMAPSyncStorageProvider.EnumerateCloudChangesForNextFolder(curOp, state);
		}

		private static void EnumerateCloudChangesForNextFolder(AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> curOp, IMAPSyncStorageProviderState state)
		{
			IEnumerator<string> cloudFolderEnumerator = state.CloudFolderEnumerator;
			while (cloudFolderEnumerator.MoveNext())
			{
				string text = cloudFolderEnumerator.Current;
				string parentCloudFolderId;
				string cloudFolderVersion;
				if (text != null && !IMAPFolder.RootCloudFolderId.Equals(text) && state.StateStorage.TryFindFolder(text, out parentCloudFolderId, out cloudFolderVersion) && IMAPSyncStorageProvider.ProcessKnownFolderEnumeration(curOp, state, text, parentCloudFolderId, cloudFolderVersion))
				{
					return;
				}
			}
			SortedList<string, IMAPFolder> sortedList = new SortedList<string, IMAPFolder>(state.CloudIdToFolder, StringComparer.Ordinal);
			state.PreprocessedDefaultMappings = new Dictionary<DefaultFolderType, string>(0);
			foreach (IMAPFolder imapfolder in sortedList.Values)
			{
				state.SyncLogSession.LogDebugging((TSLID)743UL, IMAPSyncStorageProvider.Tracer, "Examining IMAP mailboxes for default folder mappings. Folder=[{0}]", new object[]
				{
					imapfolder.Name
				});
				if (!imapfolder.Visited)
				{
					bool flag;
					bool flag2;
					DefaultFolderType defaultFolderType = IMAPMailbox.GetDefaultFolderType(imapfolder.Mailbox.Name, out flag, out flag2);
					if (defaultFolderType != DefaultFolderType.None && state.GetDefaultFolderMapping(defaultFolderType) == null)
					{
						if (state.PreprocessedDefaultMappings.ContainsKey(defaultFolderType))
						{
							if (flag)
							{
								bool flag3 = true;
								if (!flag2)
								{
									bool flag4 = false;
									bool flag5 = false;
									IMAPMailbox.GetDefaultFolderType(state.PreprocessedDefaultMappings[defaultFolderType], out flag4, out flag5);
									if (flag4)
									{
										flag3 = false;
									}
								}
								if (flag3)
								{
									state.PreprocessedDefaultMappings.Remove(defaultFolderType);
									state.PreprocessedDefaultMappings.Add(defaultFolderType, imapfolder.Mailbox.Name);
									state.SyncLogSession.LogDebugging((TSLID)744UL, IMAPSyncStorageProvider.Tracer, "Adding Folder=[{0}] as the new default for [{1}] to the preprocessed default folder mappings", new object[]
									{
										imapfolder.Mailbox.Name,
										defaultFolderType.ToString()
									});
								}
							}
						}
						else
						{
							state.PreprocessedDefaultMappings.Add(defaultFolderType, imapfolder.Mailbox.Name);
							state.SyncLogSession.LogDebugging((TSLID)745UL, IMAPSyncStorageProvider.Tracer, "Adding Folder=[{0}] as the default for [{1}] to the preprocessed default folder mappings", new object[]
							{
								imapfolder.Mailbox.Name,
								defaultFolderType.ToString()
							});
						}
					}
				}
			}
			state.SortedFolderAddsSyncChangeEntries = new SortedList<string, SyncChangeEntry>(StringComparer.Ordinal);
			IList<IMAPFolder> list = IMAPSyncStorageProvider.FilterOutFoldersToExclude(state, sortedList);
			if (list.Count > FrameworkAggregationConfiguration.Instance.ImapMaxFoldersSupported)
			{
				state.SyncLogSession.LogError((TSLID)1358UL, IMAPSyncStorageProvider.Tracer, "The remote server has {0} folders included more than the limit of {1}. The subscription will be marked with a permanent error.", new object[]
				{
					list.Count,
					FrameworkAggregationConfiguration.Instance.ImapMaxFoldersSupported
				});
				curOp.ProcessCompleted(SyncPermanentException.CreateOperationLevelException(DetailedAggregationStatus.TooManyFolders, new TooManyFoldersException(FrameworkAggregationConfiguration.Instance.ImapMaxFoldersSupported)));
				return;
			}
			state.CloudStatistics.TotalFoldersInSourceMailbox = new long?((long)list.Count);
			state.CanTrackItemCount = true;
			SortedList<string, IMAPFolder> sortedList2 = new SortedList<string, IMAPFolder>(5, new IMAPSyncStorageProvider.FolderComparer(state.GetSeparatorCharacter(), false));
			IEnumerator<IMAPFolder> enumerator2 = list.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				sortedList2.Add(enumerator2.Current.CloudId, enumerator2.Current);
			}
			state.NewCloudFolderEnumerator = sortedList2.Values.GetEnumerator();
			IMAPSyncStorageProvider.EnumerateCloudChangesForNextNewFolder(curOp, state);
		}

		private static void EnumerateCloudChangesForNextNewFolder(AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> curOp, IMAPSyncStorageProviderState state)
		{
			IEnumerator<IMAPFolder> newCloudFolderEnumerator = state.NewCloudFolderEnumerator;
			while (newCloudFolderEnumerator.MoveNext())
			{
				IMAPFolder imapfolder = newCloudFolderEnumerator.Current;
				state.SyncLogSession.LogDebugging((TSLID)746UL, IMAPSyncStorageProvider.Tracer, "Examining IMAP mailboxes in sorted order.  Folder=[{0}]", new object[]
				{
					imapfolder.Name
				});
				if (!imapfolder.Visited && !IMAPFolder.RootCloudFolderId.Equals(imapfolder))
				{
					if (!IMAPSyncStorageProvider.ProcessNewCloudFolderEnumeration(curOp, state, imapfolder))
					{
						state.CanTrackItemCount = false;
						break;
					}
					if (curOp.PendingAsyncResult != null)
					{
						return;
					}
				}
			}
			if (!state.CanTrackItemCount)
			{
				state.CloudStatistics.TotalItemsInSourceMailbox = new long?((long)SyncStorageProviderState.NoItemOrFolderCount);
			}
			if (state.Changes.Count >= state.MaxDownloadItemsPerConnection)
			{
				state.SyncLogSession.LogVerbose((TSLID)1401UL, IMAPSyncStorageProvider.Tracer, "Reached MaxDownloadItemsPerConnection {0}. MoreItemsAvailable set to true.", new object[]
				{
					state.MaxDownloadItemsPerConnection
				});
				state.MoreItemsAvailable = true;
			}
			state.SyncLogSession.LogDebugging((TSLID)1452UL, IMAPSyncStorageProvider.Tracer, "New folder enumeration is done. Calling ProcessCompleted for the enumerate step.", new object[0]);
			curOp.ProcessCompleted(new SyncProviderResultData(state.Changes, state.HasPermanentSyncErrors, state.HasTransientSyncErrors), null);
		}

		private static IList<IMAPFolder> FilterOutFoldersToExclude(IMAPSyncStorageProviderState state, SortedList<string, IMAPFolder> sortedFolderList)
		{
			IList<IMAPFolder> list = new List<IMAPFolder>(sortedFolderList.Count);
			foreach (IMAPFolder imapfolder in sortedFolderList.Values)
			{
				string folderName;
				if (!IMAPUtils.FromModifiedUTF7(imapfolder.Name, out folderName))
				{
					state.SyncLogSession.LogDebugging((TSLID)1305UL, IMAPSyncStorageProvider.Tracer, "Ignoring Folder {0} for folder exclusion since we could not decode it", new object[]
					{
						imapfolder.Name
					});
				}
				else if (state.Subscription.ShouldFolderBeExcluded(folderName, state.GetSeparatorCharacter()))
				{
					state.SyncLogSession.LogDebugging((TSLID)749UL, IMAPSyncStorageProvider.Tracer, "Skipping Folder=[{0}] since it is marked for exclusion", new object[]
					{
						imapfolder.Name
					});
					continue;
				}
				list.Add(imapfolder);
			}
			return list;
		}

		private static bool ProcessKnownFolderEnumeration(AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> curOp, IMAPSyncStorageProviderState state, string cloudFolderId, string parentCloudFolderId, string cloudFolderVersion)
		{
			IMAPFolder imapfolder;
			if (!state.CloudIdToFolder.TryGetValue(cloudFolderId, out imapfolder))
			{
				if (cloudFolderVersion != "DeletedFolder")
				{
					SyncChangeEntry syncChangeEntry = new SyncChangeEntry(ChangeType.Delete, SchemaType.Folder, cloudFolderId);
					syncChangeEntry.CloudFolderId = parentCloudFolderId;
					state.Changes.Add(syncChangeEntry);
					state.SyncLogSession.LogDebugging((TSLID)750UL, IMAPSyncStorageProvider.Tracer, "Delete folder: CloudFolderId=[{0}].  ParentCloudFolderId=[{1}]", new object[]
					{
						cloudFolderId,
						parentCloudFolderId
					});
				}
				else
				{
					bool flag = false;
					foreach (IMAPFolder imapfolder2 in state.CloudIdToFolder.Values)
					{
						if (imapfolder2.IsChildOfCloudFolder(cloudFolderId))
						{
							flag = true;
							state.SyncLogSession.LogDebugging((TSLID)751UL, IMAPSyncStorageProvider.Tracer, "Skipped intermediate deleted IMAP mailbox.  CloudFolderId=[{0}].  ParentCloudFolderId=[{1}]", new object[]
							{
								cloudFolderId,
								parentCloudFolderId
							});
							break;
						}
					}
					if (!flag)
					{
						SyncChangeEntry syncChangeEntry2 = new SyncChangeEntry(ChangeType.Delete, SchemaType.Folder, cloudFolderId);
						syncChangeEntry2.CloudFolderId = parentCloudFolderId;
						state.Changes.Add(syncChangeEntry2);
						state.SyncLogSession.LogDebugging((TSLID)752UL, IMAPSyncStorageProvider.Tracer, "Deleted intermediate deleted IMAP mailbox.  CloudFolderId=[{0}].  ParentCloudFolderId=[{1}]", new object[]
						{
							cloudFolderId,
							parentCloudFolderId
						});
					}
				}
			}
			else
			{
				if (imapfolder == null)
				{
					state.SyncLogSession.LogVerbose((TSLID)1453UL, IMAPSyncStorageProvider.Tracer, "Terminate enumeration because missing mailbox. CloudFolderId=[{0}].", new object[]
					{
						cloudFolderId
					});
					curOp.ProcessCompleted(new SyncProviderResultData(state.Changes, state.HasPermanentSyncErrors, state.HasTransientSyncErrors), IMAPSyncStorageProvider.BuildAndLogMissingMailboxException(state, cloudFolderId));
					return true;
				}
				imapfolder.InitializeFromCloudFolder(cloudFolderId, cloudFolderVersion);
				imapfolder.Visited = true;
				if (state.Changes.Count < state.MaxDownloadItemsPerConnection)
				{
					state.CurrentFolder = imapfolder;
					state.SyncLogSession.LogDebugging((TSLID)753UL, IMAPSyncStorageProvider.Tracer, "Begin Select known folder: CloudFolderId=[{0}].", new object[]
					{
						cloudFolderId
					});
					curOp.PendingAsyncResult = IMAPClient.BeginSelectImapMailbox(state.ClientState, state.CurrentFolder.Mailbox, new AsyncCallback(IMAPSyncStorageProvider.OnEndSelectMailboxContinueUpdateValidityAndEnumerate), curOp, curOp.SyncPoisonContext);
					return true;
				}
				state.SyncLogSession.LogVerbose((TSLID)754UL, IMAPSyncStorageProvider.Tracer, "Skip syncing folder because max items reached. CloudFolderId=[{0}].", new object[]
				{
					cloudFolderId
				});
				state.MoreItemsAvailable = true;
			}
			return false;
		}

		private static bool ProcessNewCloudFolderEnumeration(AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> curOp, IMAPSyncStorageProviderState state, IMAPFolder folder)
		{
			curOp.PendingAsyncResult = null;
			if (state.Changes.Count >= state.MaxDownloadItemsPerConnection)
			{
				state.SyncLogSession.LogVerbose((TSLID)755UL, IMAPSyncStorageProvider.Tracer, "Stop processing additional new folders, hard max download items reached.", new object[0]);
				state.MoreItemsAvailable = true;
				return false;
			}
			if (state.StateStorage.ContainsFailedFolder(folder.CloudId))
			{
				state.SyncLogSession.LogVerbose((TSLID)1402UL, IMAPSyncStorageProvider.Tracer, "Skip processing the permanently failed folder {0}", new object[]
				{
					folder.CloudId
				});
				return true;
			}
			state.SyncLogSession.LogDebugging((TSLID)756UL, IMAPSyncStorageProvider.Tracer, "Identified new IMAP mailbox.  Create SyncChangeEntry to add folder.  Folder={0}", new object[]
			{
				folder.Name
			});
			SortedList<string, SyncChangeEntry> sortedList = new SortedList<string, SyncChangeEntry>();
			string text;
			if (folder.TryGetParentCloudFolderId(state.GetSeparatorCharacter(folder), folder.CloudId, out text))
			{
				bool flag = true;
				while (flag && !state.SortedFolderAddsSyncChangeEntries.ContainsKey(text) && !sortedList.ContainsKey(text) && !state.StateStorage.ContainsFolder(text))
				{
					SyncChangeEntry syncChangeEntry = new SyncChangeEntry(ChangeType.Add, SchemaType.Folder, text);
					string shortFolderName = IMAPFolder.GetShortFolderName(folder, text);
					if (string.IsNullOrEmpty(shortFolderName))
					{
						state.SyncLogSession.LogError((TSLID)757UL, "Error, invalid conversion from full to short folder name - short name empty.  Full name = {0}.", new object[]
						{
							folder.Name
						});
						flag = folder.TryGetParentCloudFolderId(state.GetSeparatorCharacter(folder), text, out text);
					}
					else
					{
						syncChangeEntry.SyncObject = new SyncFolder(shortFolderName, DefaultFolderType.None);
						flag = folder.TryGetParentCloudFolderId(state.GetSeparatorCharacter(folder), text, out text);
						syncChangeEntry.CloudFolderId = (text ?? IMAPFolder.RootCloudFolderId);
						syncChangeEntry.CloudVersion = "DeletedFolder";
						sortedList.Add(syncChangeEntry.CloudId, syncChangeEntry);
						state.SyncLogSession.LogDebugging((TSLID)1159UL, IMAPSyncStorageProvider.Tracer, "IMAP SyncChangeEntry to Add parent Folder: {0}.", new object[]
						{
							syncChangeEntry.CloudId
						});
					}
				}
			}
			foreach (SyncChangeEntry syncChangeEntry2 in sortedList.Values)
			{
				if (state.Changes.Count >= state.MaxDownloadItemsPerConnection)
				{
					syncChangeEntry2.SyncObject.Dispose();
					syncChangeEntry2.SyncObject = null;
				}
				else
				{
					state.SortedFolderAddsSyncChangeEntries.Add(syncChangeEntry2.CloudId, syncChangeEntry2);
					state.Changes.Add(syncChangeEntry2);
				}
			}
			if (state.Changes.Count >= state.MaxDownloadItemsPerConnection)
			{
				state.SyncLogSession.LogVerbose((TSLID)1404UL, IMAPSyncStorageProvider.Tracer, "Skip syncing folder because max items reached. CloudFolderId=[{0}].", new object[]
				{
					folder.CloudId
				});
				state.MoreItemsAvailable = true;
				return false;
			}
			DefaultFolderType defaultFolderType = DefaultFolderType.None;
			foreach (KeyValuePair<DefaultFolderType, string> keyValuePair in state.PreprocessedDefaultMappings)
			{
				if (StringComparer.OrdinalIgnoreCase.Equals(keyValuePair.Value, folder.Mailbox.Name))
				{
					defaultFolderType = keyValuePair.Key;
					state.StoreDefaultFolderMapping(defaultFolderType, folder.CloudId);
					break;
				}
			}
			if (state.SortedFolderAddsSyncChangeEntries.ContainsKey(folder.CloudId))
			{
				SyncChangeEntry syncChangeEntry3 = state.SortedFolderAddsSyncChangeEntries[folder.CloudId];
				bool flag2 = false;
				if (syncChangeEntry3.CloudVersion == "DeletedFolder")
				{
					if (defaultFolderType != DefaultFolderType.None)
					{
						SyncFolder syncFolder = (SyncFolder)syncChangeEntry3.SyncObject;
						if (syncFolder.DefaultFolderType != defaultFolderType)
						{
							string displayName = syncFolder.DisplayName;
							syncFolder.Dispose();
							syncChangeEntry3.SyncObject = new SyncFolder(displayName, defaultFolderType);
						}
					}
					syncChangeEntry3.CloudVersion = folder.GenerateFolderCloudVersion();
					flag2 = true;
				}
				string message = string.Format(CultureInfo.InvariantCulture, "IMAP provider encountered a duplicate cloud id.  Fix up needed: {0}, Cloud Id: {1}, CloudIds seen: {2}, CloudIds from list: {3}", new object[]
				{
					flag2,
					folder.CloudId,
					IMAPSyncStorageProvider.FormatForOutput(state.SortedFolderAddsSyncChangeEntries.Keys),
					IMAPSyncStorageProvider.FormatForOutput(state.CloudIdToFolder.Keys)
				});
				state.SyncLogSession.ReportWatson(message);
			}
			else
			{
				SyncChangeEntry syncChangeEntry4 = new SyncChangeEntry(ChangeType.Add, SchemaType.Folder, folder.CloudId);
				string shortFolderName = IMAPFolder.GetShortFolderName(folder, folder.CloudId);
				if (string.IsNullOrEmpty(shortFolderName))
				{
					state.SyncLogSession.LogError((TSLID)758UL, "Error, invalid conversion from full to short folder name - short name empty.  Full name = {0}.", new object[]
					{
						folder.Name
					});
					return true;
				}
				syncChangeEntry4.SyncObject = new SyncFolder(shortFolderName, defaultFolderType);
				string text2;
				folder.TryGetParentCloudFolderId(state.GetSeparatorCharacter(folder), folder.CloudId, out text2);
				syncChangeEntry4.CloudFolderId = (text2 ?? IMAPFolder.RootCloudFolderId);
				syncChangeEntry4.CloudVersion = folder.GenerateFolderCloudVersion();
				state.SortedFolderAddsSyncChangeEntries.Add(syncChangeEntry4.CloudId, syncChangeEntry4);
				state.Changes.Add(syncChangeEntry4);
			}
			folder.Visited = true;
			state.CurrentFolder = folder;
			state.SyncLogSession.LogDebugging((TSLID)1403UL, IMAPSyncStorageProvider.Tracer, "Begin Select New folder: CloudFolderId=[{0}].", new object[]
			{
				folder.CloudId
			});
			curOp.PendingAsyncResult = IMAPClient.BeginSelectImapMailbox(state.ClientState, state.CurrentFolder.Mailbox, new AsyncCallback(IMAPSyncStorageProvider.OnEndSelectNewMailboxContinueEnumerate), curOp, curOp.SyncPoisonContext);
			return true;
		}

		private static string FormatForOutput(IEnumerable<string> keyList)
		{
			StringBuilder stringBuilder = new StringBuilder("[ ", 100);
			foreach (string value in keyList)
			{
				stringBuilder.Append("[").Append(value).Append("] ");
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		private static void OnEndSelectMailboxContinueUpdateValidityAndEnumerate(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>)asyncResult.AsyncState;
			IMAPSyncStorageProviderState state = asyncResult2.State;
			AsyncOperationResult<IMAPMailbox> asyncOperationResult = IMAPClient.EndSelectImapMailbox(asyncResult);
			if (asyncOperationResult.IsSucceeded)
			{
				state.SyncLogSession.LogDebugging((TSLID)759UL, IMAPSyncStorageProvider.Tracer, "IMAP Folder selection succeeded, continuing to fetch range.", new object[0]);
				state.UpdateMailboxItemCountFromCurrentFolderData();
				state.CurrentFolder.Mailbox = asyncOperationResult.Data;
				IMAPSyncStorageProvider.UpdateValidityReconcileAndContinue<SyncProviderResultData>(asyncResult2, new IMAPSyncStorageProviderState.PostProcessor(IMAPSyncStorageProvider.OnEndUpdateValidityContinueGetMessageInfoAndEnumerate));
				return;
			}
			Exception ex = asyncOperationResult.Exception;
			if (IMAPSyncStorageProvider.IsInvalidPathPrefixError(state, ex))
			{
				ex = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, new IMAPInvalidPathPrefixException("Invalid path prefix: " + state.ClientState.RootFolderPath, ex.InnerException, state.ClientState.RootFolderPath), true);
			}
			IMAPSyncStorageProvider.FailSelect(state, ex, true);
			if (IMAPSyncStorageProvider.IsOperationError(ex))
			{
				state.SyncLogSession.LogVerbose((TSLID)1454UL, IMAPSyncStorageProvider.Tracer, "IMAP Folder selection failed.", new object[0]);
				IMAPSyncStorageProvider.AbortEnumerate(asyncResult2, state, ex);
				return;
			}
			IMAPSyncStorageProvider.EnumerateCloudChangesForNextFolder(asyncResult2, state);
		}

		private static void OnEndSelectNewMailboxContinueEnumerate(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>)asyncResult.AsyncState;
			IMAPSyncStorageProviderState state = asyncResult2.State;
			AsyncOperationResult<IMAPMailbox> asyncOperationResult = IMAPClient.EndSelectImapMailbox(asyncResult);
			if (asyncOperationResult.IsSucceeded)
			{
				state.SyncLogSession.LogDebugging((TSLID)1405UL, IMAPSyncStorageProvider.Tracer, "IMAP Folder selection succeeded, continuing to fetch range.", new object[0]);
				if (state.CanTrackItemCount)
				{
					state.UpdateMailboxItemCountFromCurrentFolderData();
				}
				state.CurrentFolder.Mailbox = asyncOperationResult.Data;
				IMAPSyncStorageProvider.ContinueGetMessageInfoAndEnumerate(asyncResult2);
				return;
			}
			Exception ex = asyncOperationResult.Exception;
			if (IMAPSyncStorageProvider.IsInvalidPathPrefixError(state, ex))
			{
				ex = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, new IMAPInvalidPathPrefixException("Invalid path prefix: " + state.ClientState.RootFolderPath, ex.InnerException, state.ClientState.RootFolderPath), true);
			}
			IMAPSyncStorageProvider.FailSelect(state, ex, true);
			if (IMAPSyncStorageProvider.IsOperationError(ex))
			{
				state.SyncLogSession.LogDebugging((TSLID)1455UL, IMAPSyncStorageProvider.Tracer, "IMAP new Folder selection failed.", new object[0]);
				IMAPSyncStorageProvider.AbortEnumerate(asyncResult2, state, ex);
				return;
			}
			IMAPSyncStorageProvider.EnumerateCloudChangesForNextNewFolder(asyncResult2, state);
		}

		private static void OnEndUpdateValidityContinueGetMessageInfoAndEnumerate(IAsyncResult asyncResult, Exception exception)
		{
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>)asyncResult;
			IMAPSyncStorageProviderState state = asyncResult2.State;
			if (exception != null)
			{
				state.SyncLogSession.LogError((TSLID)760UL, IMAPSyncStorageProvider.Tracer, "UIDValidity Check failed.  Exception={0}", new object[]
				{
					exception
				});
				if (IMAPSyncStorageProvider.IsOperationError(exception))
				{
					IMAPSyncStorageProvider.AbortEnumerate(asyncResult2, state, exception);
					return;
				}
			}
			else
			{
				IMAPSyncStorageProvider.ContinueGetMessageInfoAndEnumerate(asyncResult);
			}
		}

		private static void ContinueGetMessageInfoAndEnumerate(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>)asyncResult;
			IMAPSyncStorageProviderState state = asyncResult2.State;
			IMAPFolder currentFolder = state.CurrentFolder;
			IMAPMailbox mailbox = currentFolder.Mailbox;
			long? lowSyncValue = currentFolder.LowSyncValue;
			long? num = currentFolder.HighSyncValue;
			long? uidNext = mailbox.UidNext;
			bool flag = false;
			bool flag2 = true;
			state.CloudItemChangeMap.Clear();
			state.FailedCloudItemsSeen.Clear();
			state.LightFetchDone = false;
			int num2 = Math.Max(0, state.MaxDownloadItemsPerConnection - state.Changes.Count);
			if (uidNext != null && num != null)
			{
				num += 1L;
				long num3 = uidNext.Value - num.Value;
				if (num3 > 0L && num3 < (long)state.MaxDownloadItemsPerConnection)
				{
					if (num3 < (long)num2)
					{
						state.LowestAttemptedSequenceNumber = null;
						asyncResult2.PendingAsyncResult = IMAPClient.BeginGetMessageInfoByRange(state.ClientState, num.Value.ToString(CultureInfo.InvariantCulture), (uidNext - 1L).Value.ToString(CultureInfo.InvariantCulture), true, IMAPClient.MessageInfoDataItemsForNewMessages, new AsyncCallback(IMAPSyncStorageProvider.OnEndGetMessageInfoContinueEnumerate), asyncResult2, asyncResult2.SyncPoisonContext);
						return;
					}
					flag = true;
					state.SyncLogSession.LogVerbose((TSLID)761UL, IMAPSyncStorageProvider.Tracer, "IMAP mailbox has more messages than can sync, deferring folder sync to next session. newMessages={0}.  Maxdownloads={1}", new object[]
					{
						num3,
						state.MaxDownloadItemsPerConnection
					});
				}
				if (num3 <= 0L)
				{
					flag2 = false;
					if (lowSyncValue != null && lowSyncValue.Value == 1L && currentFolder.NumberOfMessages != null && mailbox.NumberOfMessages != null)
					{
						long? numberOfMessages = currentFolder.NumberOfMessages;
						int? numberOfMessages2 = mailbox.NumberOfMessages;
						if (numberOfMessages.GetValueOrDefault() == (long)numberOfMessages2.GetValueOrDefault() && numberOfMessages != null == (numberOfMessages2 != null) && state.Subscription.AggregationType == AggregationType.Aggregation)
						{
							flag = true;
							state.SyncLogSession.LogVerbose((TSLID)1304UL, IMAPSyncStorageProvider.Tracer, "IMAP mailbox number of items ({0}) did not change, deferring folder {1} sync to next session.", new object[]
							{
								mailbox.NumberOfMessages,
								currentFolder.Name
							});
						}
					}
				}
			}
			if (!flag)
			{
				int? numberOfMessages3 = mailbox.NumberOfMessages;
				if (numberOfMessages3 != null && numberOfMessages3.Value > 0)
				{
					int value = Math.Max(1, numberOfMessages3.Value - 100 + 1);
					state.LowestAttemptedSequenceNumber = new int?(value);
					asyncResult2.PendingAsyncResult = IMAPClient.BeginGetMessageInfoByRange(state.ClientState, value.ToString(CultureInfo.InvariantCulture), numberOfMessages3.Value.ToString(CultureInfo.InvariantCulture), false, flag2 ? IMAPClient.MessageInfoDataItemsForNewMessages : IMAPClient.MessageInfoDataItemsForChangesOnly, new AsyncCallback(IMAPSyncStorageProvider.OnEndGetMessageInfoContinueEnumerate), asyncResult2, asyncResult2.SyncPoisonContext);
					return;
				}
				IMAPSyncStorageProvider.EnumerateDeletesAndChanges(state);
				if (uidNext != null && uidNext.Value > 1L)
				{
					currentFolder.NewLowSyncValue = 1L;
					currentFolder.NewHighSyncValue = Math.Max(uidNext.Value - 1L, 1L);
					currentFolder.NewNumberOfMessages = new long?(0L);
				}
				state.SyncLogSession.LogDebugging((TSLID)762UL, IMAPSyncStorageProvider.Tracer, "IMAP mailbox does not have EXISTS or no messages.", new object[0]);
			}
			if (state.SortedFolderAddsSyncChangeEntries == null)
			{
				IMAPSyncStorageProvider.EnumerateCloudChangesForNextFolder(asyncResult2, state);
				return;
			}
			IMAPSyncStorageProvider.EnumerateCloudChangesForNextNewFolder(asyncResult2, state);
		}

		private static void OnEndGetMessageInfoContinueEnumerate(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>)asyncResult.AsyncState;
			IMAPSyncStorageProviderState state = asyncResult2.State;
			AsyncOperationResult<IMAPResultData> asyncOperationResult = IMAPClient.EndGetMessageInfoByRange(asyncResult);
			if (!asyncOperationResult.IsSucceeded)
			{
				IMAPUtils.LogExceptionDetails(state.SyncLogSession, IMAPSyncStorageProvider.Tracer, "IMAP Fetch message headers failed with error", asyncOperationResult.Exception);
				if (IMAPSyncStorageProvider.IsOperationError(asyncOperationResult.Exception))
				{
					IMAPSyncStorageProvider.AbortEnumerate(asyncResult2, state, asyncOperationResult.Exception);
					return;
				}
				if (asyncOperationResult.Exception is SyncPermanentException)
				{
					state.HasPermanentSyncErrors = true;
				}
				else
				{
					state.HasTransientSyncErrors = true;
				}
				if (state.SortedFolderAddsSyncChangeEntries == null)
				{
					IMAPSyncStorageProvider.EnumerateCloudChangesForNextFolder(asyncResult2, state);
					return;
				}
				IMAPSyncStorageProvider.EnumerateCloudChangesForNextNewFolder(asyncResult2, state);
				return;
			}
			else
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest(2808491325U);
				IList<string> messageUids = asyncOperationResult.Data.MessageUids;
				IList<string> list = asyncOperationResult.Data.MessageIds;
				IList<IMAPMailFlags> messageFlags = asyncOperationResult.Data.MessageFlags;
				IList<long> list2 = asyncOperationResult.Data.MessageSizes;
				IList<int> messageSeqNums = asyncOperationResult.Data.MessageSeqNums;
				if (list2 != null && list2.Count != messageUids.Count)
				{
					list2 = null;
				}
				if (list != null && list.Count != messageUids.Count)
				{
					list = null;
				}
				bool flag = false;
				bool flag2 = messageUids != null && messageUids.Count > 0 && messageFlags != null && messageFlags.Count > 0;
				if (flag2)
				{
					SortedList<long, int> sortedList = IMAPSyncStorageProvider.SortAndConvertUids(state, messageUids);
					if (sortedList == null)
					{
						state.SyncLogSession.LogDebugging((TSLID)1456UL, IMAPSyncStorageProvider.Tracer, "Something went wrong.", new object[0]);
						IMAPSyncStorageProvider.AbortEnumerate(asyncResult2, state, SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, new IMAPInvalidServerException()));
						return;
					}
					int num;
					bool flag3;
					flag = IMAPSyncStorageProvider.ProcessMessageEnumerationsAndUpdateHighLowValues(state, sortedList, messageUids, list, messageFlags, list2, messageSeqNums, out num, out flag3);
					if (flag3)
					{
						state.SyncLogSession.LogDebugging((TSLID)1457UL, IMAPSyncStorageProvider.Tracer, "Duplicate UIDs found.", new object[0]);
						IMAPSyncStorageProvider.AbortEnumerate(asyncResult2, state, SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, new IMAPInvalidServerException()));
						return;
					}
					state.SyncLogSession.LogDebugging((TSLID)763UL, IMAPSyncStorageProvider.Tracer, "IMAP Fetch message header.  {0} messages seen, {1} are new and require fetching.", new object[]
					{
						messageUids.Count,
						num
					});
				}
				if ((flag || !flag2) && IMAPSyncStorageProvider.FetchNextRangeForEnumeration(asyncResult2, state, asyncOperationResult.Data))
				{
					return;
				}
				if (state.SortedFolderAddsSyncChangeEntries == null)
				{
					IMAPSyncStorageProvider.EnumerateCloudChangesForNextFolder(asyncResult2, state);
					return;
				}
				IMAPSyncStorageProvider.EnumerateCloudChangesForNextNewFolder(asyncResult2, state);
				return;
			}
		}

		private static bool ProcessMessageEnumerationsAndUpdateHighLowValues(IMAPSyncStorageProviderState state, SortedList<long, int> sortedUidToIndexList, IList<string> uids, IList<string> messageIds, IList<IMAPMailFlags> messageFlags, IList<long> messageSizes, IList<int> messageSeqNums, out int numToFetch, out bool duplicateUidsFound)
		{
			numToFetch = 0;
			duplicateUidsFound = false;
			bool result = false;
			IList<long> keys = sortedUidToIndexList.Keys;
			bool flag = messageIds != null && messageIds.Count > 0 && messageSizes != null && messageSizes.Count > 0;
			long num = state.TotalBytesReceived + state.EstimatedMessageBytesToDownload;
			int i;
			for (i = keys.Count - 1; i >= 0; i--)
			{
				long num2 = keys[i];
				int index = sortedUidToIndexList[num2];
				string text = uids[index];
				IMAPFolder currentFolder = state.CurrentFolder;
				long? lowSyncValue = currentFolder.LowSyncValue;
				if (!flag && num2 < lowSyncValue.Value)
				{
					result = true;
					state.LowestAttemptedSequenceNumber = new int?(messageSeqNums[index] + 1);
					state.LightFetchDone = true;
					state.SyncLogSession.LogDebugging((TSLID)764UL, IMAPSyncStorageProvider.Tracer, "Lightweight batch just crossed the lowUid watermark.  Folder = {0}.  UID = {1}.  Sequence number = {2}.", new object[]
					{
						currentFolder.CloudId,
						text,
						messageSeqNums[index]
					});
					break;
				}
				IMAPMailFlags imapmailFlags = messageFlags[index];
				if ((imapmailFlags & IMAPMailFlags.Deleted) == IMAPMailFlags.Deleted)
				{
					state.SyncLogSession.LogDebugging((TSLID)765UL, IMAPSyncStorageProvider.Tracer, "Excluding item with deleted flag from existing item processing.  Folder = {0}.  UID = {1}.  Sequence number = {2}.", new object[]
					{
						currentFolder.CloudId,
						text,
						messageSeqNums[index]
					});
				}
				else
				{
					string messageId = flag ? messageIds[index] : null;
					long messageSize = flag ? messageSizes[index] : 0L;
					bool flag3;
					bool flag4;
					bool flag2 = IMAPSyncStorageProvider.ProcessMessageEnumeration(state, flag, text, messageSize, messageId, imapmailFlags, ref num, out flag3, out flag4);
					if (flag4)
					{
						duplicateUidsFound = true;
						return false;
					}
					if (flag3)
					{
						numToFetch++;
					}
					if (!flag2)
					{
						break;
					}
				}
			}
			if (i < 0)
			{
				result = true;
			}
			if (keys.Count > 0 && i + 1 < keys.Count)
			{
				long val = keys[i + 1];
				long num3 = keys[keys.Count - 1];
				state.CurrentFolder.NewHighSyncValue = Math.Max(state.CurrentFolder.NewHighSyncValue, (state.CurrentFolder.Mailbox.UidNext != null) ? (state.CurrentFolder.Mailbox.UidNext.Value - 1L) : num3);
				state.CurrentFolder.NewLowSyncValue = Math.Min(state.CurrentFolder.NewLowSyncValue, val);
				IMAPFolder currentFolder2 = state.CurrentFolder;
				int? numberOfMessages = state.CurrentFolder.Mailbox.NumberOfMessages;
				currentFolder2.NewNumberOfMessages = ((numberOfMessages != null) ? new long?((long)numberOfMessages.GetValueOrDefault()) : null);
			}
			return result;
		}

		private static bool ProcessMessageEnumeration(IMAPSyncStorageProviderState state, bool hasFullResults, string uid, long messageSize, string messageId, IMAPMailFlags mailFlags, ref long numBytesReceived, out bool newMessageAdded, out bool duplicateUidsFound)
		{
			newMessageAdded = false;
			duplicateUidsFound = false;
			string text = IMAPUtils.CreateEmailCloudVersion(state.CurrentFolder, uid, mailFlags);
			if (hasFullResults)
			{
				if (messageId == null)
				{
					string str = (state.CurrentFolder.ValidityValue != null) ? state.CurrentFolder.ValidityValue.ToString() : "NIL";
					messageId = str + "." + uid;
					state.SyncLogSession.LogDebugging((TSLID)766UL, IMAPSyncStorageProvider.Tracer, "MessageID missing, using <uidvalidity><uid> instead.  Folder = {0}.  UID = {1}.  MessageId = {2}.", new object[]
					{
						state.CurrentFolder.CloudId,
						uid,
						messageId
					});
				}
				string text2 = IMAPUtils.CreateEmailCloudId(state.CurrentFolder, messageId);
				if (state.StateStorage.ContainsItem(text2))
				{
					return IMAPSyncStorageProvider.ProcessPreviouslyAddedMessageEnumeration(state, uid, text2, text, mailFlags, out duplicateUidsFound);
				}
				if (state.StateStorage.ContainsFailedItem(text2))
				{
					state.SyncLogSession.LogVerbose((TSLID)767UL, IMAPSyncStorageProvider.Tracer, "Skipping Email since we already failed permanently on it, cloudId: {0}", new object[]
					{
						text2
					});
					state.FailedCloudItemsSeen.Add(text2);
					return true;
				}
				return IMAPSyncStorageProvider.ProcessNewMessageEnumeration(state, text2, text, mailFlags, messageSize, ref numBytesReceived, out newMessageAdded);
			}
			else
			{
				if (state.CloudItemChangeMap.ContainsKey(uid))
				{
					state.SyncLogSession.LogError((TSLID)768UL, IMAPSyncStorageProvider.Tracer, "Duplicate UID.  Folder = {0}.  UID = {1}.  MessageId = {2}.  Command = {3}.", new object[]
					{
						state.CurrentFolder.CloudId,
						uid,
						messageId,
						state.ClientState.CachedCommand.ToPiiCleanString()
					});
					duplicateUidsFound = true;
					return false;
				}
				state.CloudItemChangeMap.Add(uid, text);
				return true;
			}
		}

		private static bool ProcessPreviouslyAddedMessageEnumeration(IMAPSyncStorageProviderState state, string uid, string cloudId, string newCloudVersion, IMAPMailFlags mailFlags, out bool duplicateUidsFound)
		{
			duplicateUidsFound = false;
			string text;
			string text2;
			if (!state.StateStorage.TryFindItem(cloudId, out text, out text2))
			{
				state.SyncLogSession.LogError((TSLID)774UL, IMAPSyncStorageProvider.Tracer, "IMAP Fetch message header.  Could not find cloud item.  CloudId=[{0}].  CloudVersion=[{1}]", new object[]
				{
					cloudId,
					newCloudVersion
				});
				return true;
			}
			if (state.CloudItemChangeMap.ContainsKey(uid))
			{
				state.SyncLogSession.LogError((TSLID)1288UL, IMAPSyncStorageProvider.Tracer, "Duplicate UID. Folder = {0}. UID = {1}. Command = {2}.", new object[]
				{
					state.CurrentFolder.CloudId,
					uid,
					state.ClientState.CachedCommand.ToPiiCleanString()
				});
				duplicateUidsFound = true;
				return false;
			}
			if (text2 == "ORPHANED")
			{
				if (!state.StateStorage.TryUpdateItemCloudVersion(cloudId, newCloudVersion))
				{
					state.SyncLogSession.LogError((TSLID)769UL, IMAPSyncStorageProvider.Tracer, "IMAP Fetch message header.  Failed to update item cloud version.  CloudId=[{0}].  CloudVersion=[{1}]", new object[]
					{
						cloudId,
						newCloudVersion
					});
				}
			}
			else if (text2 != newCloudVersion)
			{
				if (state.Changes.Count >= state.MaxDownloadItemsPerConnection)
				{
					state.SyncLogSession.LogVerbose((TSLID)770UL, IMAPSyncStorageProvider.Tracer, "Stop fetching messages, max download items reached.", new object[0]);
					state.MoreItemsAvailable = true;
					return false;
				}
				string a;
				if (IMAPUtils.GetUidFromEmailCloudVersion(text2, out a))
				{
					string text3;
					if (IMAPUtils.GetUidFromEmailCloudVersion(newCloudVersion, out text3) && a != text3)
					{
						state.SyncLogSession.LogVerbose((TSLID)771UL, IMAPSyncStorageProvider.Tracer, "Ignoring message UID {0}, same Message-ID as already-existing message in folder {1}.", new object[]
						{
							text3,
							state.CurrentFolder.Name
						});
						return true;
					}
				}
				else
				{
					state.SyncLogSession.LogError((TSLID)772UL, IMAPSyncStorageProvider.Tracer, "Could not parse UID, inconsistent state for the item.  UID=[{0}]. OldCloudId=[{1}].  OldCloudVersion=[{2}]. NewCloudVersion=[{3}]", new object[]
					{
						uid,
						cloudId,
						text2,
						newCloudVersion
					});
				}
				SyncChangeEntry syncChangeEntry = new SyncChangeEntry(ChangeType.Change, SchemaType.Email, cloudId);
				syncChangeEntry.SyncObject = new IMAPEmail(state, state.CurrentFolder.CloudId, mailFlags, state.CurrentFolder.Mailbox.PermanentFlags);
				syncChangeEntry.CloudVersion = newCloudVersion;
				syncChangeEntry.CloudFolderId = state.CurrentFolder.CloudId;
				state.Changes.Add(syncChangeEntry);
				state.SyncLogSession.LogDebugging((TSLID)773UL, IMAPSyncStorageProvider.Tracer, "IMAP Fetch message header.  Changed Message Flags.  CloudId=[{0}].  OldCloudVersion=[{1}].  CloudVersion=[{2}]", new object[]
				{
					cloudId,
					text2,
					newCloudVersion
				});
			}
			state.CloudItemChangeMap.Add(uid, text2);
			return true;
		}

		private static bool ProcessNewMessageEnumeration(IMAPSyncStorageProviderState state, string cloudId, string newCloudVersion, IMAPMailFlags mailFlags, long messageSize, ref long numBytesReceived, out bool newMessageAdded)
		{
			newMessageAdded = false;
			if (state.Changes.Count >= state.MaxDownloadItemsPerConnection)
			{
				state.SyncLogSession.LogVerbose((TSLID)775UL, IMAPSyncStorageProvider.Tracer, "Stop fetching messages, max download items reached.", new object[0]);
				state.MoreItemsAvailable = true;
				return false;
			}
			int hashCode = cloudId.GetHashCode();
			foreach (SyncChangeEntry syncChangeEntry in state.Changes)
			{
				if (syncChangeEntry.SchemaType == SchemaType.Email && syncChangeEntry.ChangeType == ChangeType.Add && syncChangeEntry.CloudId.GetHashCode() == hashCode && syncChangeEntry.CloudId == cloudId)
				{
					state.SyncLogSession.LogVerbose((TSLID)776UL, IMAPSyncStorageProvider.Tracer, "IMAP Fetch message header.  Ignoring new message because have message-id mapped.  CloudId=[{0}].  CloudVersion=[{1}]", new object[]
					{
						cloudId,
						newCloudVersion
					});
					return true;
				}
			}
			bool flag = messageSize > state.MaxDownloadSizePerItem;
			if (!flag)
			{
				if (numBytesReceived + messageSize > state.MaxDownloadBytesAllowed)
				{
					state.SyncLogSession.LogVerbose((TSLID)777UL, IMAPSyncStorageProvider.Tracer, "Not enumerating item, projected downloads exceeds max bytes per session.  CloudId=[{0}].  CloudVersion=[{1}].  ProjectedBytesReceived={2}.  Message Length={3}.  MaxDownloadBytesAllowed={4}.", new object[]
					{
						cloudId,
						newCloudVersion,
						numBytesReceived,
						messageSize,
						state.MaxDownloadBytesAllowed
					});
					state.MoreItemsAvailable = true;
					return false;
				}
				numBytesReceived += messageSize;
				state.EstimatedMessageBytesToDownload += messageSize;
			}
			SyncChangeEntry syncChangeEntry2 = new SyncChangeEntry(ChangeType.Add, SchemaType.Email, cloudId);
			syncChangeEntry2.CloudObject = new IMAPEmail(state, state.CurrentFolder.CloudId, mailFlags, state.CurrentFolder.Mailbox.PermanentFlags);
			syncChangeEntry2.CloudFolderId = state.CurrentFolder.CloudId;
			syncChangeEntry2.CloudVersion = newCloudVersion;
			if (flag)
			{
				state.SyncLogSession.LogVerbose((TSLID)778UL, IMAPSyncStorageProvider.Tracer, "Oversized message found as it exceeds max bytes per item.  CloudId=[{0}].  CloudVersion=[{1}].  Message Length={2}.  MaxDownloadSizePerItem={3}.", new object[]
				{
					cloudId,
					newCloudVersion,
					messageSize,
					state.MaxDownloadSizePerItem
				});
				syncChangeEntry2.Exception = SyncPermanentException.CreateItemLevelException(new MessageSizeLimitExceededException());
			}
			state.Changes.Add(syncChangeEntry2);
			newMessageAdded = true;
			state.SyncLogSession.LogDebugging((TSLID)779UL, IMAPSyncStorageProvider.Tracer, "IMAP Fetch message header.  New Message.  CloudId=[{0}].  CloudVersion=[{1}]", new object[]
			{
				cloudId,
				newCloudVersion
			});
			return true;
		}

		private static bool FetchNextRangeForEnumeration(AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> curOp, IMAPSyncStorageProviderState state, IMAPResultData resultData)
		{
			bool flag = true;
			int nextSequenceNumberRangeEnd = IMAPSyncStorageProvider.GetNextSequenceNumberRangeEnd(state, resultData);
			int num = Math.Max(1, nextSequenceNumberRangeEnd - 100 + 1);
			state.SyncLogSession.LogDebugging((TSLID)780UL, IMAPSyncStorageProvider.Tracer, "Next batch calculated sequence numbers for Folder = {0}: High Sequence number = {1}, Low Sequence number = {2}.", new object[]
			{
				state.CurrentFolder.CloudId,
				nextSequenceNumberRangeEnd.ToString(),
				num.ToString()
			});
			if (!state.LightFetchDone)
			{
				bool flag2 = state.CurrentFolder.LowSyncValue != null && state.CurrentFolder.HighSyncValue != null;
				if (flag2)
				{
					bool flag3 = state.CurrentFolder.NewLowSyncValue > state.CurrentFolder.LowSyncValue.Value && state.CurrentFolder.NewLowSyncValue - 1L <= state.CurrentFolder.HighSyncValue.Value;
					bool flag4 = state.CurrentFolder.LowSyncValue.Value == 1L;
					bool flag5 = state.CurrentFolder.NewLowSyncValue - state.CurrentFolder.LowSyncValue.Value >= (long)(nextSequenceNumberRangeEnd - num);
					if (flag3 && (flag4 || flag5))
					{
						flag = false;
					}
				}
			}
			if (num <= nextSequenceNumberRangeEnd)
			{
				state.LowestAttemptedSequenceNumber = new int?(num);
				curOp.PendingAsyncResult = IMAPClient.BeginGetMessageInfoByRange(state.ClientState, num.ToString(CultureInfo.InvariantCulture), nextSequenceNumberRangeEnd.ToString(CultureInfo.InvariantCulture), false, flag ? IMAPClient.MessageInfoDataItemsForNewMessages : IMAPClient.MessageInfoDataItemsForChangesOnly, new AsyncCallback(IMAPSyncStorageProvider.OnEndGetMessageInfoContinueEnumerate), curOp, curOp.SyncPoisonContext);
				return true;
			}
			state.CurrentFolder.NewLowSyncValue = 1L;
			IMAPSyncStorageProvider.EnumerateDeletesAndChanges(state);
			return false;
		}

		private static void EnumerateDeletesAndChanges(IMAPSyncStorageProviderState state)
		{
			IMAPFolder currentFolder = state.CurrentFolder;
			IMAPMailbox mailbox = currentFolder.Mailbox;
			using (IEnumerator<string> cloudItemFilteredByCloudFolderIdEnumerator = state.StateStorage.GetCloudItemFilteredByCloudFolderIdEnumerator(currentFolder.CloudId))
			{
				while (cloudItemFilteredByCloudFolderIdEnumerator.MoveNext())
				{
					string text = cloudItemFilteredByCloudFolderIdEnumerator.Current;
					string cloudFolderId;
					string text2;
					if (state.StateStorage.TryFindItem(text, out cloudFolderId, out text2))
					{
						string key;
						if (text2 != "ORPHANED" && IMAPUtils.GetUidFromEmailCloudVersion(text2, out key))
						{
							string text3;
							if (state.CloudItemChangeMap.TryGetValue(key, out text3))
							{
								if (text2 != text3)
								{
									IMAPMailFlags mailFlags;
									if (IMAPUtils.GetFlagsFromCloudVersion(text3, mailbox, out mailFlags))
									{
										SyncChangeEntry syncChangeEntry = new SyncChangeEntry(ChangeType.Change, SchemaType.Email, text);
										syncChangeEntry.SyncObject = new IMAPEmail(state, currentFolder.CloudId, mailFlags, mailbox.PermanentFlags);
										syncChangeEntry.CloudVersion = text3;
										syncChangeEntry.CloudFolderId = cloudFolderId;
										state.Changes.Add(syncChangeEntry);
										state.SyncLogSession.LogDebugging((TSLID)782UL, IMAPSyncStorageProvider.Tracer, "IMAP Fetch message header.  Changed Message Flags.  CloudId=[{0}].  OldCloudVersion=[{1}].  NewCloudVersion=[{2}]", new object[]
										{
											text,
											text2,
											text3
										});
										if (state.Changes.Count >= state.MaxDownloadItemsPerConnection)
										{
											state.SyncLogSession.LogVerbose((TSLID)783UL, IMAPSyncStorageProvider.Tracer, "Stop processing changed messages, max download items reached.", new object[0]);
											break;
										}
									}
									else
									{
										state.SyncLogSession.LogError((TSLID)784UL, IMAPSyncStorageProvider.Tracer, "IMAP Fetch message header.  Unable to parse mail flags.  CloudId=[{0}].  OldCloudVersion=[{1}].  NewCloudVersion=[{2}]", new object[]
										{
											text,
											text2,
											text3
										});
									}
								}
							}
							else
							{
								SyncChangeEntry syncChangeEntry2 = new SyncChangeEntry(ChangeType.Delete, SchemaType.Email, text);
								syncChangeEntry2.CloudFolderId = cloudFolderId;
								syncChangeEntry2.CloudVersion = text2;
								state.Changes.Add(syncChangeEntry2);
								state.SyncLogSession.LogDebugging((TSLID)785UL, IMAPSyncStorageProvider.Tracer, "IMAP Fetch message header.  Delete Message.  CloudId=[{0}].  CloudVersion=[{1}]", new object[]
								{
									text,
									text2
								});
								if (state.Changes.Count >= state.MaxDownloadItemsPerConnection)
								{
									state.SyncLogSession.LogVerbose((TSLID)786UL, IMAPSyncStorageProvider.Tracer, "Stop processing deleted messages, max download items reached.", new object[0]);
									break;
								}
							}
						}
						else
						{
							state.SyncLogSession.LogError((TSLID)787UL, IMAPSyncStorageProvider.Tracer, "IMAP Fetch message header.  Could not parse UID, may be orphaned.  CloudId=[{0}].  CloudVersion=[{1}]", new object[]
							{
								text,
								text2
							});
						}
					}
					else
					{
						state.SyncLogSession.LogError((TSLID)788UL, IMAPSyncStorageProvider.Tracer, "IMAP Fetch message header.  Could not find cloud item.  CloudId=[{0}].", new object[]
						{
							text
						});
					}
				}
			}
			using (IEnumerator<string> failedCloudItemFilteredByCloudFolderIdEnumerator = state.StateStorage.GetFailedCloudItemFilteredByCloudFolderIdEnumerator(currentFolder.CloudId))
			{
				string cloudId = currentFolder.CloudId;
				while (failedCloudItemFilteredByCloudFolderIdEnumerator.MoveNext())
				{
					string text4 = failedCloudItemFilteredByCloudFolderIdEnumerator.Current;
					if (!state.FailedCloudItemsSeen.Contains(text4))
					{
						SyncChangeEntry syncChangeEntry3 = new SyncChangeEntry(ChangeType.Delete, SchemaType.Email, text4);
						syncChangeEntry3.CloudFolderId = cloudId;
						state.Changes.Add(syncChangeEntry3);
						state.SyncLogSession.LogDebugging((TSLID)789UL, IMAPSyncStorageProvider.Tracer, "EnumerateDeletesAndChanges: Delete for failed Item. CloudId=[{0}].  CloudFolderId=[{1}]", new object[]
						{
							text4,
							cloudId
						});
						if (state.Changes.Count >= state.MaxDownloadItemsPerConnection)
						{
							state.SyncLogSession.LogDebugging((TSLID)790UL, IMAPSyncStorageProvider.Tracer, "Stop processing deleted messages for failed items, max download items reached.", new object[0]);
							break;
						}
					}
				}
			}
		}

		private static bool IsInvalidPathPrefixError(IMAPSyncStorageProviderState state, Exception resultException)
		{
			if (string.IsNullOrEmpty(state.ClientState.RootFolderPath) || state.CurrentFolder == null || state.CurrentFolder.Name != IMAPMailbox.Inbox)
			{
				return false;
			}
			SyncTransientException ex = resultException as SyncTransientException;
			return ex != null && ex.IsItemException;
		}

		private static void StartSelectCloudFoldersAndBuildWatermark(AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> curOp, IMAPSyncStorageProviderState state)
		{
			IComparer<string> comparer = new IMAPSyncStorageProvider.FolderComparer(state.GetSeparatorCharacter(), false);
			SortedList<string, IMAPFolder> sortedList = new SortedList<string, IMAPFolder>(state.CloudIdToFolder, comparer);
			state.CheckForChangesCloudFolderEnumerator = sortedList.Values.GetEnumerator();
			IMAPSyncStorageProvider.SelectNextCloudFolderAndBuildWatermark(curOp, state);
		}

		private static void SelectNextCloudFolderAndBuildWatermark(AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> curOp, IMAPSyncStorageProviderState state)
		{
			IEnumerator<IMAPFolder> checkForChangesCloudFolderEnumerator = state.CheckForChangesCloudFolderEnumerator;
			if (!checkForChangesCloudFolderEnumerator.MoveNext())
			{
				string text = state.ComputeSyncWatermark();
				string syncWatermark = state.SyncWatermark;
				state.SyncLogSession.LogVerbose((TSLID)513UL, IMAPSyncStorageProvider.Tracer, "Previous Watermark:[{0}], Current WaterMark:[{1}]", new object[]
				{
					syncWatermark,
					text
				});
				if (syncWatermark == null || !syncWatermark.Equals(text))
				{
					state.HasNoChangesOnCloud = false;
				}
				else
				{
					state.HasNoChangesOnCloud = true;
				}
				curOp.ProcessCompleted(new SyncProviderResultData(null, false, false, state.HasNoChangesOnCloud));
				return;
			}
			IMAPFolder imapfolder = checkForChangesCloudFolderEnumerator.Current;
			state.CurrentFolder = imapfolder;
			state.SyncLogSession.LogDebugging((TSLID)514UL, "Begin selection of folder: CloudFolderId=[{0}]", new object[]
			{
				imapfolder.CloudId
			});
			curOp.PendingAsyncResult = IMAPClient.BeginSelectImapMailbox(state.ClientState, state.CurrentFolder.Mailbox, new AsyncCallback(IMAPSyncStorageProvider.OnEndSelectMailboxBuildWatermarkAndContinue), curOp, curOp.SyncPoisonContext);
		}

		private static void OnEndSelectMailboxBuildWatermarkAndContinue(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>)asyncResult.AsyncState;
			IMAPSyncStorageProviderState state = asyncResult2.State;
			AsyncOperationResult<IMAPMailbox> asyncOperationResult = IMAPClient.EndSelectImapMailbox(asyncResult);
			if (!asyncOperationResult.IsSucceeded)
			{
				Exception exception = asyncOperationResult.Exception;
				IMAPSyncStorageProvider.FailSelect(state, exception, true);
				state.HasNoChangesOnCloud = false;
				asyncResult2.ProcessCompleted(new SyncProviderResultData(null, false, false, state.HasNoChangesOnCloud));
				return;
			}
			state.CurrentFolder.Mailbox = asyncOperationResult.Data;
			state.SyncLogSession.LogDebugging((TSLID)515UL, IMAPSyncStorageProvider.Tracer, "IMAP Folder selection succeeded, continuing to build sync watermark.", new object[0]);
			state.AppendToSyncWatermark(state.CurrentFolder.Name, state.CurrentFolder.Mailbox.UidValidity, state.CurrentFolder.Mailbox.UidNext);
			IMAPSyncStorageProvider.SelectNextCloudFolderAndBuildWatermark(asyncResult2, state);
		}

		private static void UpdateValidityReconcileAndContinue<TResult>(AsyncResult<IMAPSyncStorageProviderState, TResult> curOp, IMAPSyncStorageProviderState.PostProcessor continuationCallback) where TResult : class
		{
			IMAPSyncStorageProviderState state = curOp.State;
			if (state.CurrentFolder.Mailbox.UidValidity == null)
			{
				Exception ex = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, new IMAPException(string.Format(CultureInfo.InvariantCulture, "Mailbox missing UIDVALIDITY value, cannot guarantee validity of UIDs.  Mailbox = {0}.", new object[]
				{
					state.CurrentFolder.Mailbox.Name
				})), true);
				IMAPSyncStorageProvider.FailSelect(state, ex, false);
				continuationCallback(curOp, ex);
				return;
			}
			if (state.CurrentFolder.ValidityValue == null)
			{
				state.CurrentFolder.ValidityValue = new long?(state.CurrentFolder.Mailbox.UidValidity.Value);
				string text = state.CurrentFolder.GenerateFolderCloudVersion();
				if (!state.StateStorage.TryUpdateFolderCloudVersion(state.CurrentFolder.CloudId, text))
				{
					state.SyncLogSession.LogError((TSLID)791UL, IMAPSyncStorageProvider.Tracer, "Failed to update cloud folder version.  CloudFolderId=[{0}].  NewCloudFolderVersion=[{1}]", new object[]
					{
						state.CurrentFolder.CloudId,
						text
					});
				}
				continuationCallback(curOp, null);
				return;
			}
			if (state.CurrentFolder.ValidityValue.Value == state.CurrentFolder.Mailbox.UidValidity.Value)
			{
				continuationCallback(curOp, null);
				return;
			}
			state.SyncLogSession.LogInformation((TSLID)792UL, IMAPSyncStorageProvider.Tracer, "UIDVALIDITY has changed, attempting reconciliation.  Mailbox = {0}", new object[]
			{
				state.CurrentFolder.Mailbox.Name
			});
			state.PostUidReconciliationCallback = continuationCallback;
			int value = state.CurrentFolder.Mailbox.NumberOfMessages.Value;
			if (state.MessageIdToUidMap == null)
			{
				state.MessageIdToUidMap = new Dictionary<string, string>(value);
			}
			else
			{
				state.MessageIdToUidMap.Clear();
			}
			if (value == 0)
			{
				Exception exceptionDuringReconciliation = IMAPSyncStorageProvider.PerformUidReconciliation(state);
				state.PostUidReconciliationCallback(curOp, exceptionDuringReconciliation);
				return;
			}
			int num = Math.Max(1, value - 100);
			state.SyncLogSession.LogVerbose((TSLID)793UL, IMAPSyncStorageProvider.Tracer, "Starting UIDValidity change recovery.  Mailbox = {0}.  Initial fetch from [{1}, {2}]", new object[]
			{
				state.CurrentFolder.Name,
				num,
				value
			});
			state.LowestAttemptedSequenceNumber = new int?(num);
			curOp.PendingAsyncResult = IMAPClient.BeginGetMessageInfoByRange(state.ClientState, num.ToString(CultureInfo.InvariantCulture), value.ToString(CultureInfo.InvariantCulture), false, IMAPClient.MessageInfoDataItemsForUidValidityRecovery, new AsyncCallback(IMAPSyncStorageProvider.OnEndGetRangeForUidValidityRecovery), curOp, curOp.SyncPoisonContext);
		}

		private static void OnEndGetRangeForUidValidityRecovery(IAsyncResult asyncResult)
		{
			IAsyncResult curOp;
			IMAPSyncStorageProviderState state;
			if (asyncResult.AsyncState is AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>)
			{
				AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult2 = asyncResult.AsyncState as AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>;
				curOp = asyncResult2;
				state = asyncResult2.State;
			}
			else
			{
				AsyncResult<IMAPSyncStorageProviderState, SyncChangeEntry> asyncResult3 = asyncResult.AsyncState as AsyncResult<IMAPSyncStorageProviderState, SyncChangeEntry>;
				curOp = asyncResult3;
				state = asyncResult3.State;
			}
			AsyncOperationResult<IMAPResultData> asyncOperationResult = IMAPClient.EndGetMessageInfoByRange(asyncResult);
			if (!asyncOperationResult.IsSucceeded)
			{
				IMAPUtils.LogExceptionDetails(state.SyncLogSession, IMAPSyncStorageProvider.Tracer, "IMAP Fetch message details during UIDValidity recovery failed with error", asyncOperationResult.Exception);
				IMAPSyncStorageProvider.FailSelect(state, asyncOperationResult.Exception, false);
				state.PostUidReconciliationCallback(curOp, asyncOperationResult.Exception);
				return;
			}
			for (int i = 0; i < asyncOperationResult.Data.MessageIds.Count; i++)
			{
				string text = asyncOperationResult.Data.MessageIds[i];
				string text2 = asyncOperationResult.Data.MessageUids[i];
				if (text != null && text2 != null)
				{
					state.MessageIdToUidMap[text] = text2;
				}
			}
			int nextSequenceNumberRangeEnd = IMAPSyncStorageProvider.GetNextSequenceNumberRangeEnd(state, asyncOperationResult.Data);
			int num = Math.Max(1, nextSequenceNumberRangeEnd - 100 + 1);
			if (nextSequenceNumberRangeEnd <= 0 || num >= nextSequenceNumberRangeEnd)
			{
				Exception exceptionDuringReconciliation = IMAPSyncStorageProvider.PerformUidReconciliation(state);
				state.PostUidReconciliationCallback(curOp, exceptionDuringReconciliation);
				return;
			}
			state.SyncLogSession.LogDebugging((TSLID)795UL, IMAPSyncStorageProvider.Tracer, "Continuing UIDValidity change recovery.  Mailbox = {0}.  Subsequent fetch from [{1}, {2}]", new object[]
			{
				state.CurrentFolder.Name,
				num,
				nextSequenceNumberRangeEnd
			});
			state.LowestAttemptedSequenceNumber = new int?(num);
			if (asyncResult.AsyncState is AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>)
			{
				AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult4 = asyncResult.AsyncState as AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>;
				asyncResult4.PendingAsyncResult = IMAPClient.BeginGetMessageInfoByRange(state.ClientState, num.ToString(CultureInfo.InvariantCulture), nextSequenceNumberRangeEnd.ToString(CultureInfo.InvariantCulture), false, IMAPClient.MessageInfoDataItemsForUidValidityRecovery, new AsyncCallback(IMAPSyncStorageProvider.OnEndGetRangeForUidValidityRecovery), asyncResult4, asyncResult4.SyncPoisonContext);
				return;
			}
			AsyncResult<IMAPSyncStorageProviderState, SyncChangeEntry> asyncResult5 = asyncResult.AsyncState as AsyncResult<IMAPSyncStorageProviderState, SyncChangeEntry>;
			asyncResult5.PendingAsyncResult = IMAPClient.BeginGetMessageInfoByRange(state.ClientState, num.ToString(CultureInfo.InvariantCulture), nextSequenceNumberRangeEnd.ToString(CultureInfo.InvariantCulture), false, IMAPClient.MessageInfoDataItemsForUidValidityRecovery, new AsyncCallback(IMAPSyncStorageProvider.OnEndGetRangeForUidValidityRecovery), asyncResult5, asyncResult5.SyncPoisonContext);
		}

		private static Exception PerformUidReconciliation(IMAPSyncStorageProviderState state)
		{
			Exception ex = null;
			IDictionary<string, string> dictionary = new Dictionary<string, string>(state.MessageIdToUidMap.Count);
			int num = state.CurrentFolder.Uniqueness.ToString(CultureInfo.InvariantCulture).Length + 1;
			using (IEnumerator<string> cloudItemFilteredByCloudFolderIdEnumerator = state.StateStorage.GetCloudItemFilteredByCloudFolderIdEnumerator(state.CurrentFolder.CloudId))
			{
				while (ex == null && cloudItemFilteredByCloudFolderIdEnumerator.MoveNext())
				{
					string text = cloudItemFilteredByCloudFolderIdEnumerator.Current;
					string key = text.Substring(num);
					string text2;
					string text3;
					if (state.StateStorage.TryFindItem(text, out text2, out text3))
					{
						string uid;
						if (state.MessageIdToUidMap.TryGetValue(key, out uid))
						{
							IMAPUtils.UpdateUidInCloudVersion(uid, ref text3);
						}
						else
						{
							state.SyncLogSession.LogInformation((TSLID)796UL, IMAPSyncStorageProvider.Tracer, "Item is being orphaned during UID reconciliation.  CloudId = [{0}].", new object[]
							{
								text
							});
							text3 = "ORPHANED";
						}
						if (state.StateStorage.TryUpdateItemCloudVersion(text, text3))
						{
							state.SyncLogSession.LogVerbose((TSLID)797UL, IMAPSyncStorageProvider.Tracer, "Updated cloud version for post-UIDValidity-change recovered item.  CloudId = [{0}].  CloudVersion = [{1}].", new object[]
							{
								text,
								text3
							});
							dictionary[text] = text3;
						}
						else
						{
							ex = SyncTransientException.CreateItemLevelException(new IMAPException(string.Format(CultureInfo.InvariantCulture, "Failed to update cloud version for post-UIDValidity-change recovered item.  CloudId = [{0}].  CloudVersion = [{1}].", new object[]
							{
								text,
								text3
							})));
						}
					}
					else
					{
						ex = SyncTransientException.CreateItemLevelException(new IMAPException(string.Format(CultureInfo.InvariantCulture, "Failed to find item for cloud Id - when we were just given it by enumerator!  CloudId = [{0}].", new object[]
						{
							text
						})));
					}
				}
			}
			if (ex == null)
			{
				if (state.Changes != null)
				{
					foreach (SyncChangeEntry change in state.Changes)
					{
						IMAPSyncStorageProvider.UpdateCloudVersionForItem(state, dictionary, num, change);
					}
				}
				if (state.ItemBeingRetrieved != null)
				{
					IMAPSyncStorageProvider.UpdateCloudVersionForItem(state, dictionary, num, state.ItemBeingRetrieved);
				}
				state.CurrentFolder.ValidityValue = new long?(state.CurrentFolder.Mailbox.UidValidity.Value);
				string text4 = state.CurrentFolder.GenerateFolderCloudVersion();
				if (state.StateStorage.TryUpdateFolderCloudVersion(state.CurrentFolder.CloudId, text4))
				{
					state.SyncLogSession.LogDebugging((TSLID)798UL, IMAPSyncStorageProvider.Tracer, "Updated cloud folder version.  CloudFolderId=[{0}].  NewCloudFolderVersion=[{1}]", new object[]
					{
						state.CurrentFolder.CloudId,
						text4
					});
				}
				else
				{
					state.SyncLogSession.LogError((TSLID)799UL, IMAPSyncStorageProvider.Tracer, "Failed to update cloud folder version.  CloudFolderId=[{0}].  NewCloudFolderVersion=[{1}]", new object[]
					{
						state.CurrentFolder.CloudId,
						text4
					});
				}
			}
			else
			{
				IMAPSyncStorageProvider.FailSelect(state, ex, false);
			}
			state.MessageIdToUidMap.Clear();
			return ex;
		}

		private static void ProcessListLevelResultWithNextActions(IAsyncResult asyncResult, AsyncCallback listImapMailboxesByLevelCallback, Action<AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>, IMAPSyncStorageProviderState> actionWhenFinishedListing, Action<AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>, IMAPSyncStorageProviderState, AsyncOperationResult<IList<IMAPMailbox>>> actionWhenOperationFailed)
		{
			AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData>)asyncResult.AsyncState;
			IMAPSyncStorageProviderState state = asyncResult2.State;
			AsyncOperationResult<IList<IMAPMailbox>> asyncOperationResult = IMAPClient.EndListImapMailboxes(asyncResult);
			if (!asyncOperationResult.IsSucceeded)
			{
				actionWhenOperationFailed(asyncResult2, state, asyncOperationResult);
				return;
			}
			bool flag;
			bool anyFoldersAtThisLevel;
			IMAPSyncStorageProvider.ProcessSuccessfulListResultsAndDetermineNextAction(state, asyncOperationResult, out flag, out anyFoldersAtThisLevel);
			if (flag)
			{
				state.AdvanceToNextFolderLevel(anyFoldersAtThisLevel);
				asyncResult2.PendingAsyncResult = IMAPClient.BeginListImapMailboxesByLevel(state.ClientState, state.CurrentFolderListLevel, state.GetSeparatorCharacter(), listImapMailboxesByLevelCallback, asyncResult2, asyncResult2.SyncPoisonContext);
				return;
			}
			state.IsListLevelsComplete = true;
			actionWhenFinishedListing(asyncResult2, state);
		}

		private static void HandleFailedListOperationResult(AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> curOp, IMAPSyncStorageProviderState state, AsyncOperationResult<IList<IMAPMailbox>> result)
		{
			IMAPUtils.LogExceptionDetails(state.SyncLogSession, IMAPSyncStorageProvider.Tracer, "IMAP List failed with with error", result.Exception);
			if (IMAPSyncStorageProvider.IsOperationError(result.Exception))
			{
				curOp.ProcessCompleted(result.Exception);
				return;
			}
			string failureReason = "Couldn't get the IMAP list";
			curOp.ProcessCompleted(SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, new IMAPException(failureReason, result.Exception), true));
		}

		private static void HandleFailedListOperationResultForCheckForChanges(AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> curOp, IMAPSyncStorageProviderState state, AsyncOperationResult<IList<IMAPMailbox>> result)
		{
			IMAPUtils.LogExceptionDetails(state.SyncLogSession, IMAPSyncStorageProvider.Tracer, "IMAP List failed with with error", result.Exception);
			Exception ex = result.Exception;
			string failureReason = string.Format("IMAP list failed with the error {0} during check changes", ex);
			if (!IMAPSyncStorageProvider.IsOperationError(result.Exception))
			{
				ex = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, new IMAPException(failureReason, result.Exception), true);
			}
			curOp.ProcessCompleted(new SyncProviderResultData(null, false, false, false), ex);
		}

		private static void ProcessSuccessfulListResultsAndDetermineNextAction(IMAPSyncStorageProviderState state, AsyncOperationResult<IList<IMAPMailbox>> result, out bool continueToNextLevel, out bool anyFoldersExistAtThisLevel)
		{
			int currentFolderListLevel = state.CurrentFolderListLevel;
			if (result.Data != null && result.Data.Count != 0)
			{
				anyFoldersExistAtThisLevel = true;
				using (IEnumerator<IMAPMailbox> enumerator = result.Data.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						IMAPMailbox imapmailbox = enumerator.Current;
						if (imapmailbox.IsSelectable)
						{
							IMAPFolder imapfolder = new IMAPFolder(imapmailbox);
							state.CloudIdToFolder[imapfolder.CloudId] = imapfolder;
							state.SyncLogSession.LogDebugging((TSLID)689UL, IMAPSyncStorageProvider.Tracer, "Found IMAP mailbox [{0}] at level {1}.", new object[]
							{
								imapmailbox.Name,
								state.CurrentFolderListLevel
							});
							if (state.CurrentFolder == null && imapfolder.Mailbox.Separator != null)
							{
								state.CurrentFolder = imapfolder;
							}
						}
					}
					goto IL_117;
				}
			}
			anyFoldersExistAtThisLevel = false;
			state.SyncLogSession.LogDebugging((TSLID)690UL, IMAPSyncStorageProvider.Tracer, "IMAP LIST command complete, but no mailboxes found at level {0}.", new object[]
			{
				currentFolderListLevel
			});
			IL_117:
			if (currentFolderListLevel == IMAPSyncStorageProvider.MaxFolderLevelDepth)
			{
				state.SyncLogSession.LogDebugging((TSLID)691UL, IMAPSyncStorageProvider.Tracer, "IMAP List succeeded, finished recursing to max depth.", new object[0]);
				continueToNextLevel = false;
				return;
			}
			if (!anyFoldersExistAtThisLevel && !state.AnyFoldersExistAtPreviousLevel)
			{
				state.SyncLogSession.LogDebugging((TSLID)693UL, IMAPSyncStorageProvider.Tracer, "IMAP List succeeded, no folders found for two levels in a row. Stop LISTing.", new object[0]);
				continueToNextLevel = false;
				return;
			}
			if (anyFoldersExistAtThisLevel)
			{
				state.SyncLogSession.LogDebugging((TSLID)694UL, IMAPSyncStorageProvider.Tracer, "IMAP List succeeded at level {0}, continuing to list more.", new object[]
				{
					state.CurrentFolderListLevel
				});
			}
			else
			{
				state.SyncLogSession.LogDebugging((TSLID)692UL, IMAPSyncStorageProvider.Tracer, "IMAP List succeeded, no folders found, checking one more level down.", new object[0]);
			}
			continueToNextLevel = true;
		}

		private static string FindOrReplaceMessageId(IMAPSyncStorageProviderState imapState, ISyncEmail syncEmail, SyncChangeEntry currentlyAppliedEntry, out string newCloudId, out Stream mimeStream)
		{
			bool flag = false;
			string text = syncEmail.InternetMessageId;
			if (text == null)
			{
				text = Guid.NewGuid().ToString();
				flag = true;
			}
			bool flag2 = true;
			newCloudId = null;
			while (flag2)
			{
				newCloudId = IMAPUtils.CreateEmailCloudId(imapState.CurrentFolder, text);
				bool flag3 = imapState.StateStorage.ContainsItem(newCloudId);
				if (!flag3)
				{
					foreach (SyncChangeEntry syncChangeEntry in imapState.Changes)
					{
						if (syncChangeEntry.SchemaType == SchemaType.Email && syncChangeEntry.ChangeType == ChangeType.Add)
						{
							if (syncChangeEntry == currentlyAppliedEntry)
							{
								break;
							}
							if (syncChangeEntry.CloudId == newCloudId)
							{
								flag3 = true;
								break;
							}
						}
					}
				}
				if (flag3)
				{
					text = Guid.NewGuid().ToString();
					flag = true;
				}
				else
				{
					flag2 = false;
				}
			}
			if (flag)
			{
				using (MimeDocument mimeDocument = new MimeDocument())
				{
					mimeDocument.Load(syncEmail.MimeStream, CachingMode.Copy);
					Header header = mimeDocument.RootPart.Headers.FindFirst(HeaderId.MessageId);
					if (header == null)
					{
						header = new AsciiTextHeader("Message-ID", text);
						mimeDocument.RootPart.Headers.AppendChild(header);
					}
					else
					{
						header.Value = text;
					}
					mimeStream = TemporaryStorage.Create();
					mimeDocument.WriteTo(mimeStream);
				}
				imapState.SyncLogSession.LogVerbose((TSLID)800UL, IMAPSyncStorageProvider.Tracer, "Replacing message id [{0}] with [{1}]", new object[]
				{
					syncEmail.InternetMessageId ?? "Missing",
					text
				});
			}
			else
			{
				mimeStream = syncEmail.MimeStream;
			}
			mimeStream.Position = 0L;
			return text;
		}

		private static int GetNextSequenceNumberRangeEnd(IMAPSyncStorageProviderState state, IMAPResultData result)
		{
			if (state.LowestAttemptedSequenceNumber != null)
			{
				return state.LowestAttemptedSequenceNumber.Value - 1;
			}
			if (result != null && result.LowestSequenceNumber != null)
			{
				return result.LowestSequenceNumber.Value - 1;
			}
			return state.CurrentFolder.Mailbox.NumberOfMessages.Value;
		}

		private static SortedList<long, int> SortAndConvertUids(IMAPSyncStorageProviderState state, IList<string> uids)
		{
			SortedList<long, int> sortedList = new SortedList<long, int>(uids.Count);
			for (int i = 0; i < uids.Count; i++)
			{
				long num = 0L;
				if (!long.TryParse(uids[i], out num))
				{
					state.SyncLogSession.LogError((TSLID)802UL, IMAPSyncStorageProvider.Tracer, "IMAP Fetch message header.  Unable to parse uid = {0}.", new object[]
					{
						uids[i]
					});
					return null;
				}
				if (num >= 2147483647L)
				{
					state.SyncLogSession.LogError((TSLID)801UL, IMAPSyncStorageProvider.Tracer, "Invalid IMAP server response!  Do not handle uids over Int32.MaxValue={0}.  Uid={1}", new object[]
					{
						int.MaxValue,
						uids[i]
					});
					return null;
				}
				if (sortedList.ContainsKey(num))
				{
					state.SyncLogSession.LogError((TSLID)1295UL, IMAPSyncStorageProvider.Tracer, "Invalid IMAP server response. Uid appears more than once: {0} for folder {1}. IMAP Command: {2}", new object[]
					{
						uids[i],
						state.CurrentFolder.Name,
						state.ClientState.CachedCommand.ToPiiCleanString()
					});
					return null;
				}
				sortedList.Add(num, i);
			}
			return sortedList;
		}

		private static IMAPMailFlags GetFlagsFromSyncEmail(ISyncEmail syncEmail, IMAPMailbox mailbox)
		{
			IMAPMailFlags imapmailFlags = IMAPMailFlags.None;
			if (syncEmail.Importance != null && syncEmail.Importance.Value == Importance.High)
			{
				imapmailFlags |= IMAPMailFlags.Flagged;
			}
			if (syncEmail.IsDraft != null && syncEmail.IsDraft.Value)
			{
				imapmailFlags |= IMAPMailFlags.Draft;
			}
			if (syncEmail.IsRead != null && syncEmail.IsRead.Value)
			{
				imapmailFlags |= IMAPMailFlags.Seen;
			}
			if (syncEmail.SyncMessageResponseType != null && syncEmail.SyncMessageResponseType.Value == SyncMessageResponseType.Replied)
			{
				imapmailFlags |= IMAPMailFlags.Answered;
			}
			return IMAPUtils.FilterFlagsAgainstSupported(imapmailFlags, mailbox);
		}

		private static void UpdateCloudVersionForItem(IMAPSyncStorageProviderState state, IDictionary<string, string> newCloudVersionsById, int prefixLength, SyncChangeEntry change)
		{
			if (change != null && change.SchemaType == SchemaType.Email && change.CloudFolderId == state.CurrentFolder.CloudId && change.CloudId != null)
			{
				string cloudVersion;
				if (newCloudVersionsById.TryGetValue(change.CloudId, out cloudVersion))
				{
					change.CloudVersion = cloudVersion;
					state.SyncLogSession.LogVerbose((TSLID)803UL, IMAPSyncStorageProvider.Tracer, "Updating cloud version post-UIDValidity change.  CloudId = [{0}].  CloudVersion = [{1}].", new object[]
					{
						change.CloudId,
						change.CloudVersion
					});
					return;
				}
				string key = change.CloudId.Substring(prefixLength);
				string uid;
				if (state.MessageIdToUidMap.TryGetValue(key, out uid))
				{
					cloudVersion = change.CloudVersion;
					IMAPUtils.UpdateUidInCloudVersion(uid, ref cloudVersion);
					change.CloudVersion = cloudVersion;
					state.SyncLogSession.LogVerbose((TSLID)804UL, IMAPSyncStorageProvider.Tracer, "Updating cloud version post-UIDValidity change.  CloudId = [{0}].  CloudVersion = [{1}].", new object[]
					{
						change.CloudId,
						change.CloudVersion
					});
					return;
				}
				state.SyncLogSession.LogError((TSLID)805UL, IMAPSyncStorageProvider.Tracer, "Orphaning item.  CloudId = [{0}].", new object[]
				{
					change.CloudId
				});
				change.CloudVersion = "ORPHANED";
			}
		}

		private static void DisposeChange(SyncChangeEntry change)
		{
			if (change != null && change.SyncObject != null)
			{
				change.SyncObject.Dispose();
				change.SyncObject = null;
			}
		}

		private static bool IsOperationError(Exception exception)
		{
			SyncPermanentException ex = exception as SyncPermanentException;
			if (ex != null)
			{
				return ex.DetailedAggregationStatus != DetailedAggregationStatus.None;
			}
			SyncTransientException ex2 = exception as SyncTransientException;
			return ex2 != null && ex2.DetailedAggregationStatus != DetailedAggregationStatus.None;
		}

		private static void AbortApply(AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> curOp, IMAPSyncStorageProviderState state, Exception exception)
		{
			if (!(exception is SyncTransientException))
			{
				do
				{
					if (state.ApplyChangesEnumerator.Current != null && state.ApplyChangesEnumerator.Current.SyncObject != null)
					{
						state.ApplyChangesEnumerator.Current.SyncObject.Dispose();
						state.ApplyChangesEnumerator.Current.SyncObject = null;
					}
				}
				while (state.ApplyChangesEnumerator.MoveNext());
			}
			curOp.ProcessCompleted(new SyncProviderResultData(state.Changes, state.HasPermanentSyncErrors, state.HasTransientSyncErrors), exception);
		}

		private static void AbortEnumerate(AsyncResult<IMAPSyncStorageProviderState, SyncProviderResultData> curOp, IMAPSyncStorageProviderState state, Exception exception)
		{
			IMAPSyncStorageProvider.DisposeEnumerationChanges(state.Changes);
			curOp.ProcessCompleted(new SyncProviderResultData(state.Changes, state.HasPermanentSyncErrors, state.HasTransientSyncErrors), exception);
		}

		private static void DisposeEnumerationChanges(IList<SyncChangeEntry> changes)
		{
			if (changes != null && changes.Count > 0)
			{
				foreach (SyncChangeEntry syncChangeEntry in changes)
				{
					if (syncChangeEntry.SchemaType == SchemaType.Email && syncChangeEntry.ChangeType == ChangeType.Add && syncChangeEntry.CloudObject != null)
					{
						((ISyncEmail)syncChangeEntry.CloudObject).Dispose();
						syncChangeEntry.CloudObject = null;
					}
					if (syncChangeEntry.SyncObject != null)
					{
						syncChangeEntry.SyncObject.Dispose();
						syncChangeEntry.SyncObject = null;
					}
				}
			}
		}

		private static void UpdateSyncChangeEntry(IMAPSyncStorageProviderState state, SyncChangeEntry syncEntry, Exception exception)
		{
			IMAPSyncStorageProvider.UpdateSyncChangeEntry(state, syncEntry, exception, false);
		}

		private static void UpdateSyncChangeEntry(IMAPSyncStorageProviderState state, SyncChangeEntry syncEntry, Exception exception, bool defaultTransient)
		{
			if (exception is SyncTransientException)
			{
				state.HasTransientSyncErrors = true;
				syncEntry.Exception = exception;
				return;
			}
			if (exception is SyncPermanentException)
			{
				state.HasPermanentSyncErrors = true;
				syncEntry.Exception = exception;
				return;
			}
			if (exception is TransientException)
			{
				state.HasTransientSyncErrors = true;
				syncEntry.Exception = SyncTransientException.CreateItemLevelException(exception);
				return;
			}
			if (defaultTransient)
			{
				state.HasTransientSyncErrors = true;
				syncEntry.Exception = SyncTransientException.CreateItemLevelException(exception);
				return;
			}
			state.HasPermanentSyncErrors = true;
			syncEntry.Exception = SyncPermanentException.CreateItemLevelException(exception);
		}

		private static Exception BuildExpungeFailureException()
		{
			if (IMAPSyncStorageProvider.expungeFailure == null)
			{
				IMAPSyncStorageProvider.expungeFailure = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, new IMAPException("Expunge failed, delete may not have occurred"), true);
			}
			return IMAPSyncStorageProvider.expungeFailure;
		}

		private static Exception BuildConnectAndAuthenticateException(IMAPSyncStorageProviderState state, Exception resultException)
		{
			SyncPermanentException ex = resultException as SyncPermanentException;
			Exception result;
			if (ex != null)
			{
				result = SyncTransientException.CreateOperationLevelException(ex.DetailedAggregationStatus, ex.InnerException, true);
			}
			else
			{
				SyncTransientException ex2 = resultException as SyncTransientException;
				if (ex2 != null)
				{
					if (!ex2.NeedsBackoff || ex2.IsItemException)
					{
						result = SyncTransientException.CreateOperationLevelException(ex2.DetailedAggregationStatus, ex2.InnerException, true);
					}
					else
					{
						result = resultException;
					}
				}
				else
				{
					result = resultException;
				}
			}
			return result;
		}

		private static Exception BuildAndLogMissingMailboxException(IMAPSyncStorageProviderState state, string cloudFolderId)
		{
			string text = string.Format(CultureInfo.InvariantCulture, "Unable to find IMAP mailbox for folder {0}", new object[]
			{
				cloudFolderId
			});
			state.SyncLogSession.LogError((TSLID)806UL, IMAPSyncStorageProvider.Tracer, text, new object[0]);
			return SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, new IMAPException(text), true);
		}

		private static Exception BuildAndLogCloudVersionParseException(IMAPSyncStorageProviderState state, string cloudId, string cloudVersion, bool isOperationLevel)
		{
			string text = string.Format(CultureInfo.InvariantCulture, "Unable to parse UID value.  CloudId=[{0}].  CloudVersion=[{1}].", new object[]
			{
				cloudId,
				cloudVersion
			});
			state.SyncLogSession.LogError((TSLID)807UL, IMAPSyncStorageProvider.Tracer, text, new object[0]);
			if (isOperationLevel)
			{
				return SyncPermanentException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, new IMAPInvalidItemException());
			}
			return SyncPermanentException.CreateItemLevelException(new IMAPException(text));
		}

		private static Exception BuildAndLogOrphanedItemApplyException(IMAPSyncStorageProviderState state, string cloudId)
		{
			string text = string.Format(CultureInfo.InvariantCulture, "CloudId [{0}] is orphaned, will retry again later.", new object[]
			{
				cloudId
			});
			state.SyncLogSession.LogError((TSLID)808UL, IMAPSyncStorageProvider.Tracer, text, new object[0]);
			return SyncTransientException.CreateItemLevelException(new IMAPException(text));
		}

		private static void FailSelect(IMAPSyncStorageProviderState state, Exception exception, bool setErrorFlags)
		{
			if (setErrorFlags)
			{
				if (exception is SyncPermanentException)
				{
					state.HasPermanentSyncErrors = true;
				}
				else
				{
					state.HasTransientSyncErrors = true;
				}
			}
			if (state.SortedFolderAddsSyncChangeEntries != null && state.SortedFolderAddsSyncChangeEntries.ContainsKey(state.CurrentFolder.CloudId))
			{
				state.SortedFolderAddsSyncChangeEntries[state.CurrentFolder.CloudId].Exception = exception;
			}
			state.LastSelectFailedFolder = state.CurrentFolder;
			state.LastSelectFailedFolderException = exception;
			state.CurrentFolder = null;
			state.PendingExpungeCloudFolderId = null;
			IMAPUtils.LogExceptionDetails(state.SyncLogSession, IMAPSyncStorageProvider.Tracer, "Failure during IMAP Select", exception);
		}

		private static void RollbackDeletesOnFailedExpunge(IMAPSyncStorageProviderState state)
		{
			state.SyncLogSession.LogError((TSLID)809UL, IMAPSyncStorageProvider.Tracer, "Expunge failed, rolling back all pending deletes on mailbox.  Mailbox = {0}.", new object[]
			{
				state.PendingExpungeCloudFolderId
			});
			foreach (SyncChangeEntry syncChangeEntry in state.Changes)
			{
				if (syncChangeEntry.ChangeType == ChangeType.Delete && syncChangeEntry.SchemaType == SchemaType.Email && syncChangeEntry.CloudFolderId.Equals(state.PendingExpungeCloudFolderId))
				{
					IMAPSyncStorageProvider.UpdateSyncChangeEntry(state, syncChangeEntry, IMAPSyncStorageProvider.BuildExpungeFailureException());
				}
			}
		}

		internal static readonly Trace Tracer = ExTraceGlobals.IMAPSyncStorageProviderTracer;

		internal static readonly int MaxFolderLevelDepth = 20;

		private static Exception expungeFailure;

		internal class FolderComparer : IComparer<string>
		{
			public FolderComparer(char separator, bool reverseOrdinal)
			{
				this.separator = separator;
				this.reverseOrdinal = reverseOrdinal;
			}

			public int Compare(string lhs, string rhs)
			{
				int num = this.ValueOf(lhs);
				int num2 = this.ValueOf(rhs);
				int num3 = num - num2;
				if (num3 != 0)
				{
					return num3;
				}
				if (!this.reverseOrdinal)
				{
					return string.Compare(lhs, rhs, StringComparison.Ordinal);
				}
				return string.Compare(rhs, lhs, StringComparison.Ordinal);
			}

			private int ValueOf(string folder)
			{
				bool flag = false;
				bool flag2 = false;
				DefaultFolderType parentDefaultFolderType = this.GetParentDefaultFolderType(folder, out flag, out flag2);
				DefaultFolderType defaultFolderType = parentDefaultFolderType;
				int num;
				switch (defaultFolderType)
				{
				case DefaultFolderType.DeletedItems:
					num = 20;
					goto IL_4C;
				case DefaultFolderType.Drafts:
					break;
				case DefaultFolderType.Inbox:
					num = -10;
					goto IL_4C;
				case DefaultFolderType.JunkEmail:
					num = 30;
					goto IL_4C;
				default:
					if (defaultFolderType == DefaultFolderType.SentItems)
					{
						num = 10;
						goto IL_4C;
					}
					break;
				}
				num = 0;
				IL_4C:
				if (num != 0)
				{
					if (flag)
					{
						if (!flag2)
						{
							num++;
						}
					}
					else
					{
						num += 2;
					}
				}
				return num;
			}

			private DefaultFolderType GetParentDefaultFolderType(string folder, out bool preferred, out bool exactMatch)
			{
				string[] array = folder.Split(new char[]
				{
					this.separator
				});
				DefaultFolderType defaultFolderType = DefaultFolderType.None;
				preferred = false;
				exactMatch = false;
				if (array.Length > 1)
				{
					defaultFolderType = IMAPMailbox.GetDefaultFolderType(array[0] + this.separator + array[1], out preferred, out exactMatch);
				}
				if (defaultFolderType == DefaultFolderType.None)
				{
					defaultFolderType = IMAPMailbox.GetDefaultFolderType(array[0], out preferred, out exactMatch);
				}
				return defaultFolderType;
			}

			private char separator;

			private bool reverseOrdinal;
		}
	}
}
