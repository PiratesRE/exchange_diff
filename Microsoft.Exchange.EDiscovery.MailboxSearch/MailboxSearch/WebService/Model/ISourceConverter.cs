using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model
{
	internal interface ISourceConverter
	{
		IEnumerable<SearchSource> Convert(ISearchPolicy policy, IEnumerable<SearchSource> sources);
	}
}
