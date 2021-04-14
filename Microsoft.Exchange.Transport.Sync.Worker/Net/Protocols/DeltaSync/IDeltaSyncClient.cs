using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.ContentAggregation;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IDeltaSyncClient : IDisposable
	{
		IAsyncResult BeginGetChanges(int windowSize, AsyncCallback callback, object asyncState, object syncPoisonContext);

		IAsyncResult BeginApplyChanges(List<DeltaSyncOperation> deltaSyncOperations, ConflictResolution conflictResolution, AsyncCallback callback, object asyncState, object syncPoisonContext);

		IAsyncResult BeginSendMessage(DeltaSyncMail deltaSyncEmail, bool saveInSentItems, DeltaSyncRecipients deltaSyncRecipients, AsyncCallback callback, object asyncState, object syncPoisonContext);

		IAsyncResult BeginFetchMessage(Guid serverId, AsyncCallback callback, object asyncState, object syncPoisonContext);

		IAsyncResult BeginVerifyAccount(AsyncCallback callback, object asyncState, object syncPoisonContext);

		IAsyncResult BeginGetSettings(AsyncCallback callback, object asyncState, object syncPoisonContext);

		IAsyncResult BeginGetStatistics(AsyncCallback callback, object asyncState, object syncPoisonContext);

		AsyncOperationResult<DeltaSyncResultData> EndVerifyAccount(IAsyncResult asyncResult);

		AsyncOperationResult<DeltaSyncResultData> EndGetChanges(IAsyncResult asyncResult);

		AsyncOperationResult<DeltaSyncResultData> EndApplyChanges(IAsyncResult asyncResult);

		AsyncOperationResult<DeltaSyncResultData> EndSendMessage(IAsyncResult asyncResult);

		AsyncOperationResult<DeltaSyncResultData> EndFetchMessage(IAsyncResult asyncResult);

		AsyncOperationResult<DeltaSyncResultData> EndGetSettings(IAsyncResult asyncResult);

		AsyncOperationResult<DeltaSyncResultData> EndGetStatistics(IAsyncResult asyncResult);

		void SubscribeDownloadCompletedEvent(EventHandler<DownloadCompleteEventArgs> eventHandler);

		void NotifyRoundtripComplete(object sender, RoundtripCompleteEventArgs roundtripCompleteEventArgs);
	}
}
