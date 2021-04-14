using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface ISyncStorageProvider : ISyncStorageProviderItemRetriever
	{
		AggregationSubscriptionType SubscriptionType { get; }

		SyncStorageProviderState Bind(ISyncWorkerData subscription, SyncLogSession syncLogSession, bool underRecovery);

		void Unbind(SyncStorageProviderState state);

		IAsyncResult BeginAuthenticate(SyncStorageProviderState state, AsyncCallback callback, object callbackState, object syncPoisonContext);

		AsyncOperationResult<SyncProviderResultData> EndAuthenticate(IAsyncResult asyncResult);

		IAsyncResult BeginCheckForChanges(SyncStorageProviderState state, AsyncCallback callback, object callbackState, object syncPoisonContext);

		AsyncOperationResult<SyncProviderResultData> EndCheckForChanges(IAsyncResult asyncResult);

		IAsyncResult BeginEnumerateChanges(SyncStorageProviderState state, AsyncCallback callback, object callbackState, object syncPoisonContext);

		AsyncOperationResult<SyncProviderResultData> EndEnumerateChanges(IAsyncResult asyncResult);

		IAsyncResult BeginAcknowledgeChanges(SyncStorageProviderState state, IList<SyncChangeEntry> changeList, bool hasPermanentSyncErrors, bool hasTransientSyncErrors, AsyncCallback callback, object callbackState, object syncPoisonContext);

		AsyncOperationResult<SyncProviderResultData> EndAcknowledgeChanges(IAsyncResult asyncResult);

		IAsyncResult BeginApplyChanges(SyncStorageProviderState state, IList<SyncChangeEntry> changeList, ISyncStorageProviderItemRetriever itemRetriever, object itemRetrieverState, AsyncCallback callback, object callbackState, object syncPoisonContext);

		AsyncOperationResult<SyncProviderResultData> EndApplyChanges(IAsyncResult asyncResult);

		void Cancel(IAsyncResult asyncResult);
	}
}
