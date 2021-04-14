using System;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model
{
	internal interface ISearchResultProvider
	{
		SearchMailboxesResults Search(ISearchPolicy policy, SearchMailboxesInputs input);
	}
}
