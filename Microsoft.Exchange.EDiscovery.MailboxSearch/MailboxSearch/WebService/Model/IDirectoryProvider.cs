using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model
{
	internal interface IDirectoryProvider
	{
		IEnumerable<SearchRecipient> Query(ISearchPolicy policy, DirectoryQueryParameters request);
	}
}
