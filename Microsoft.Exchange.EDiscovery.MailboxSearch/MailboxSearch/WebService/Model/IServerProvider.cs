using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model
{
	internal interface IServerProvider
	{
		IEnumerable<FanoutParameters> GetServer(ISearchPolicy policy, IEnumerable<SearchSource> sources);
	}
}
