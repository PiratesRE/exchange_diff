using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Infrastructure;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Task
{
	internal class SearchDatabase : SearchTask<SearchSource>
	{
		public override void Process(IList<SearchSource> item)
		{
			Recorder.Trace(4L, TraceType.InfoTrace, "SearchDatabase.Process Item:", item);
			SearchMailboxesInputs searchMailboxesInputs = ((SearchMailboxesInputs)base.Executor.Context.Input).Clone();
			searchMailboxesInputs.Sources = item.ToList<SearchSource>();
			ISearchResultProvider searchResultProvider = SearchFactory.Current.GetSearchResultProvider(base.Policy, searchMailboxesInputs.SearchType);
			SearchMailboxesResults item2 = searchResultProvider.Search(base.Policy, searchMailboxesInputs);
			base.Executor.EnqueueNext(item2);
		}
	}
}
