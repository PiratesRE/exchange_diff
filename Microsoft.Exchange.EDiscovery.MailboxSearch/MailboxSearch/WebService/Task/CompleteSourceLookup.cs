using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Infrastructure;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Task
{
	internal class CompleteSourceLookup : SearchTask<SearchSource>
	{
		public override void Process(IList<SearchSource> item)
		{
			Recorder.Trace(4L, TraceType.InfoTrace, "CompleteSourceLookup.Process Item:", item);
			Guid guid = new Guid("aca47e7e-5bb8-43ed-976a-f3158f6d4df7");
			List<PreviewItem> list = new List<PreviewItem>();
			List<MailboxStatistics> list2 = new List<MailboxStatistics>();
			SearchMailboxesInputs searchMailboxesInputs = base.Context.TaskContext as SearchMailboxesInputs;
			Uri baseUri = new Uri("http://local/");
			StoreId value = new VersionedId(new byte[]
			{
				3,
				1,
				2,
				3,
				3,
				1,
				2,
				3,
				byte.MaxValue
			});
			UniqueItemHash itemHash = new UniqueItemHash(string.Empty, string.Empty, null, false);
			PagingInfo pagingInfo = ((SearchMailboxesInputs)base.Executor.Context.Input).PagingInfo;
			ReferenceItem sortValue = new ReferenceItem(pagingInfo.SortBy, ExDateTime.Now, 0L);
			Dictionary<PropertyDefinition, object> properties = new Dictionary<PropertyDefinition, object>
			{
				{
					ItemSchema.Id,
					value
				}
			};
			foreach (SearchSource searchSource in item)
			{
				if (searchSource.MailboxInfo != null && (searchSource.MailboxInfo.MailboxGuid != Guid.Empty || searchSource.MailboxInfo.IsRemoteMailbox))
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					if (searchSource.MailboxInfo != null && searchSource.MailboxInfo.MailboxGuid != Guid.Empty)
					{
						Dictionary<string, string> dictionary2 = dictionary;
						string key = "Name";
						string value2;
						if ((value2 = searchSource.OriginalReferenceId) == null)
						{
							value2 = (searchSource.MailboxInfo.DisplayName ?? string.Empty);
						}
						dictionary2[key] = value2;
						dictionary["Smtp"] = (searchSource.MailboxInfo.PrimarySmtpAddress.ToString() ?? string.Empty);
						dictionary["DN"] = (searchSource.MailboxInfo.LegacyExchangeDN ?? string.Empty);
					}
					if (searchMailboxesInputs != null)
					{
						dictionary["Lang"] = (searchMailboxesInputs.Language ?? string.Empty);
						dictionary["Config"] = (searchMailboxesInputs.SearchConfigurationId ?? string.Empty);
					}
					Uri owaLink = LinkUtils.AppendQueryString(baseUri, dictionary);
					Recorder.Trace(4L, TraceType.InfoTrace, new object[]
					{
						"CompleteSourceLookup.Process Found:",
						searchSource.ReferenceId,
						"MailboxInfo:",
						searchSource.MailboxInfo
					});
					list.Add(new PreviewItem(properties, (searchSource.MailboxInfo.MailboxGuid != Guid.Empty) ? searchSource.MailboxInfo.MailboxGuid : guid, owaLink, sortValue, itemHash)
					{
						MailboxInfo = searchSource.MailboxInfo
					});
					list2.Add(new MailboxStatistics(searchSource.MailboxInfo, 0UL, new ByteQuantifiedSize(0UL)));
				}
			}
			SortedResultPage resultPage = new SortedResultPage(list.ToArray(), pagingInfo);
			ISearchResult searchResult = new ResultAggregator(resultPage, null, 0UL, new ByteQuantifiedSize(0UL), null, null, list2);
			base.Executor.EnqueueNext(new SearchMailboxesResults(item)
			{
				SearchResult = searchResult
			});
		}
	}
}
