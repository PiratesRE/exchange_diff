using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface ISyncStorageProviderItemRetriever
	{
		IAsyncResult BeginGetItem(object itemRetrieverState, SyncChangeEntry item, AsyncCallback callback, object callbackState, object syncPoisonContext);

		AsyncOperationResult<SyncChangeEntry> EndGetItem(IAsyncResult asyncResult);

		void CancelGetItem(IAsyncResult asyncResult);
	}
}
