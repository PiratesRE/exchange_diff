using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface ICloudStatisticsProvider
	{
		IAsyncResult BeginGetStatistics(SyncStorageProviderState state, AsyncCallback callback, object callbackState, object syncPoisonContext);

		AsyncOperationResult<CloudStatistics> EndGetStatistics(IAsyncResult asyncResult);
	}
}
