using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Infrastructure;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Task
{
	internal class CompleteGetSearchableMailbox : SearchTask<SearchSource>
	{
		public override void Process(IList<SearchSource> item)
		{
			Recorder.Trace(4L, TraceType.InfoTrace, "CompleteGetSearchableMailbox.Process Item:", item);
			GetSearchableMailboxesResults getSearchableMailboxesResults = new GetSearchableMailboxesResults();
			getSearchableMailboxesResults.Sources.AddRange(item);
			base.Executor.EnqueueNext(getSearchableMailboxesResults);
		}
	}
}
