using System;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model
{
	internal class GetSearchableMailboxesInputs
	{
		public bool ExpandGroups { get; set; }

		public string Filter { get; set; }
	}
}
