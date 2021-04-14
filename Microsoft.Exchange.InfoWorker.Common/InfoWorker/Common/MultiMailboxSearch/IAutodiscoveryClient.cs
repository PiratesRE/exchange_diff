using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal interface IAutodiscoveryClient
	{
		IAsyncResult BeginAutodiscover(AsyncCallback callback, object state);

		Dictionary<GroupId, List<MailboxInfo>> EndAutodiscover(IAsyncResult result);

		void CancelAutodiscover();
	}
}
