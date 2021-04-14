using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal interface IEwsEndpointDiscovery
	{
		Dictionary<GroupId, List<MailboxInfo>> FindEwsEndpoints(out long localDiscoveryTime, out long autoDiscoveryTime);
	}
}
