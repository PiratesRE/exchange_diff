using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Infrastructure;
using Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model
{
	internal class SearchMailboxesResults
	{
		public SearchMailboxesResults(IEnumerable<SearchSource> sources = null)
		{
			this.failures = new ConcurrentQueue<Exception>();
			this.sources = ((sources != null) ? new ConcurrentQueue<SearchSource>(sources) : new ConcurrentQueue<SearchSource>());
			this.SearchResult = new ResultAggregator();
		}

		public ISearchResult SearchResult { get; set; }

		public IEnumerable<Exception> Failures
		{
			get
			{
				return this.failures;
			}
		}

		public IEnumerable<SearchSource> Sources
		{
			get
			{
				return this.sources;
			}
		}

		public void AddSources(IEnumerable<SearchSource> sources)
		{
			foreach (SearchSource item in sources)
			{
				this.sources.Enqueue(item);
			}
		}

		public void AddFailures(IEnumerable<Exception> failures)
		{
			foreach (Exception item in failures)
			{
				this.failures.Enqueue(item);
			}
		}

		public void MergeResults(SearchMailboxesResults result)
		{
			this.AddSources(result.Sources);
			this.SearchResult.MergeSearchResult(result.SearchResult);
		}

		public void UpdateResults(IEnumerable<FanoutParameters> parameters, SearchMailboxesInputs input, SearchMailboxesResponse response, Exception exception)
		{
			Recorder.Trace(4L, TraceType.InfoTrace, new object[]
			{
				"SearchMailboxesResults.UpdateResults Parameters:",
				parameters,
				"Input:",
				input,
				"Response:",
				response,
				"Exception:",
				exception
			});
			this.AddSources(from t in parameters
			select t.Source);
			using (WebServiceMailboxSearchGroup webServiceMailboxSearchGroup = new WebServiceMailboxSearchGroup(parameters.First<FanoutParameters>().GroupId, new WebServiceMailboxSearchGroup.FindMailboxInfoHandler(this.FindMailboxInfo), input.Criteria, input.PagingInfo, input.CallerInfo))
			{
				if (exception == null && (response.SearchResult == null || response.Result == 2))
				{
					exception = new ServiceResponseException(response);
				}
				if (exception != null)
				{
					using (IEnumerator<SearchSource> enumerator = this.Sources.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							SearchSource searchSource = enumerator.Current;
							if (searchSource.MailboxInfo != null)
							{
								webServiceMailboxSearchGroup.MergeMailboxResult(searchSource.MailboxInfo, exception);
							}
						}
						goto IL_10D;
					}
				}
				if (response.SearchResult != null)
				{
					webServiceMailboxSearchGroup.MergeSearchResults(response.SearchResult);
				}
				IL_10D:
				this.SearchResult = webServiceMailboxSearchGroup.GetResultAggregator();
			}
		}

		private MailboxInfo FindMailboxInfo(object state)
		{
			Recorder.Trace(4L, TraceType.InfoTrace, "SearchMailboxesResults.FindMailboxInfo State:", state);
			if (this.sources != null && this.sources.Count > 0)
			{
				Recorder.Trace(4L, TraceType.InfoTrace, "SearchMailboxesResults.FindMailboxInfo Count:", this.sources.Count);
				SearchSource searchSource = null;
				SearchPreviewItem previewItem = state as SearchPreviewItem;
				if (previewItem != null)
				{
					searchSource = this.Sources.FirstOrDefault((SearchSource t) => string.Equals(t.OriginalReferenceId, previewItem.Mailbox.MailboxId, StringComparison.InvariantCultureIgnoreCase) || string.Equals(t.ReferenceId, previewItem.Mailbox.MailboxId, StringComparison.InvariantCultureIgnoreCase));
				}
				FailedSearchMailbox failedItem = state as FailedSearchMailbox;
				if (failedItem != null)
				{
					searchSource = this.Sources.FirstOrDefault((SearchSource t) => string.Equals(t.OriginalReferenceId, failedItem.Mailbox, StringComparison.InvariantCultureIgnoreCase) || string.Equals(t.ReferenceId, failedItem.Mailbox, StringComparison.InvariantCultureIgnoreCase));
				}
				MailboxStatisticsItem statisticsItem = state as MailboxStatisticsItem;
				if (statisticsItem != null)
				{
					searchSource = this.Sources.FirstOrDefault((SearchSource t) => string.Equals(t.OriginalReferenceId, statisticsItem.MailboxId, StringComparison.InvariantCultureIgnoreCase) || string.Equals(t.ReferenceId, statisticsItem.MailboxId, StringComparison.InvariantCultureIgnoreCase));
				}
				if (searchSource != null)
				{
					return searchSource.MailboxInfo;
				}
			}
			Recorder.Trace(4L, TraceType.WarningTrace, "SearchMailboxesResults.FindMailboxInfo Source Not Found State:", state);
			return null;
		}

		private ConcurrentQueue<SearchSource> sources;

		private ConcurrentQueue<Exception> failures;
	}
}
