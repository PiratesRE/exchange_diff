using System;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal interface INonIndexableDiscoveryEwsClient
	{
		IAsyncResult BeginGetNonIndexableItemStatistics(AsyncCallback callback, object state, GetNonIndexableItemStatisticsParameters parameters);

		GetNonIndexableItemStatisticsResponse EndGetNonIndexableItemStatistics(IAsyncResult result);

		IAsyncResult BeginGetNonIndexableItemDetails(AsyncCallback callback, object state, GetNonIndexableItemDetailsParameters parameters);

		GetNonIndexableItemDetailsResponse EndGetNonIndexableItemDetails(IAsyncResult result);
	}
}
