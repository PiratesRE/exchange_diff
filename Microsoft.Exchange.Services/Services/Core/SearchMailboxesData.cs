using System;
using System.Collections.Generic;
using Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class SearchMailboxesData
	{
		internal MailboxQuery MailboxQuery { get; set; }

		internal ResultAggregator ResultAggregator { get; set; }

		internal List<FailedSearchMailbox> NonSearchableMailboxes { get; set; }
	}
}
