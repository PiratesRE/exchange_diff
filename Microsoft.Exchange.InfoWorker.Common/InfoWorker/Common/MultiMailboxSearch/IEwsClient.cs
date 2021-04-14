using System;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal interface IEwsClient
	{
		IAsyncResult BeginEwsCall(AsyncCallback callback, object state);

		ServiceResponse EndEwsCall(IAsyncResult result);
	}
}
