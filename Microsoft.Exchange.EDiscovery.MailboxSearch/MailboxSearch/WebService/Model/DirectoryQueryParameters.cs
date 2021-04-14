using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model
{
	internal class DirectoryQueryParameters
	{
		public QueryFilter Query { get; set; }

		public PropertyDefinition[] Properties { get; set; }

		public List<SearchSource> Sources { get; set; }

		public int PageSize { get; set; }

		public bool MatchRecipientsToSources { get; set; }

		public bool ExpandGroups { get; set; }

		public bool RequestGroups { get; set; }

		public bool ExpandPublicFolders { get; set; }
	}
}
