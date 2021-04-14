using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model
{
	internal class GetSearchableMailboxesResults
	{
		public GetSearchableMailboxesResults()
		{
			this.Sources = new List<SearchSource>();
		}

		public List<SearchSource> Sources { get; set; }
	}
}
