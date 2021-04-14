using System;

namespace Microsoft.Filtering
{
	internal interface IFipsDataStreamFilteringService : IDisposable
	{
		IAsyncResult BeginScan(FipsDataStreamFilteringRequest fipsDataStreamFilteringRequest, FilteringRequest filteringRequest, AsyncCallback callback, object state);

		FilteringResponse EndScan(IAsyncResult ar);
	}
}
