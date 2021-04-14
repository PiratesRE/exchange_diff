using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Infrastructure;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model;
using Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Engine;
using Microsoft.Exchange.Search.Fast;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.External
{
	internal class LocalNonIndexedResultProvider : ISearchResultProvider
	{
		public SearchMailboxesResults Search(ISearchPolicy policy, SearchMailboxesInputs input)
		{
			ulong num = 0UL;
			SortedResultPage resultPage = null;
			new List<SearchSource>(input.Sources);
			List<MailboxStatistics> list = new List<MailboxStatistics>();
			Dictionary<Guid, List<KeyValuePair<int, long>>> dictionary = new Dictionary<Guid, List<KeyValuePair<int, long>>>();
			Recorder.Record record = policy.Recorder.Start("NonIndexableItemProvider", TraceType.InfoTrace, true);
			Recorder.Trace(5L, TraceType.InfoTrace, new object[]
			{
				"NonIndexableItemProvider.Search Input:",
				input,
				"Type:",
				input.SearchType
			});
			try
			{
				SearchSource searchSource = input.Sources.FirstOrDefault<SearchSource>();
				if (searchSource != null)
				{
					Guid guid = searchSource.MailboxInfo.IsArchive ? searchSource.MailboxInfo.ArchiveDatabase : searchSource.MailboxInfo.MdbGuid;
					long num2 = 0L;
					if (input.PagingInfo.SortValue != null && input.PagingInfo.SortValue.SortColumnValue != null)
					{
						num2 = input.PagingInfo.SortValue.SecondarySortValue;
						num2 += 1L;
					}
					string indexSystemName = FastIndexVersion.GetIndexSystemName(guid);
					using (IFailedItemStorage failedItemStorage = Factory.Current.CreateFailedItemStorage(Factory.Current.CreateSearchServiceConfig(), indexSystemName))
					{
						foreach (SearchSource searchSource2 in input.Sources)
						{
							FailedItemParameters failedItemParameters = new FailedItemParameters(FailureMode.All, FieldSet.Default);
							failedItemParameters.MailboxGuid = new Guid?(searchSource2.MailboxInfo.IsArchive ? searchSource2.MailboxInfo.ArchiveGuid : searchSource2.MailboxInfo.MailboxGuid);
							long failedItemsCount = failedItemStorage.GetFailedItemsCount(failedItemParameters);
							num += (ulong)failedItemsCount;
							list.Add(new MailboxStatistics(searchSource2.MailboxInfo, (ulong)failedItemsCount, ByteQuantifiedSize.Zero));
							if (input.SearchType == SearchType.NonIndexedItemPreview)
							{
								failedItemParameters.StartingIndexId = num2;
								failedItemParameters.ResultLimit = input.PagingInfo.PageSize;
								ICollection<IFailureEntry> failedItems = failedItemStorage.GetFailedItems(failedItemParameters);
								dictionary[failedItemParameters.MailboxGuid.Value] = (from t in failedItems
								select new KeyValuePair<int, long>(t.DocumentId, t.IndexId)).ToList<KeyValuePair<int, long>>();
							}
						}
					}
					if (input.SearchType == SearchType.NonIndexedItemPreview)
					{
						MultiMailboxSearchClient multiMailboxSearchClient = new MultiMailboxSearchClient(guid, (from t in input.Sources
						select t.MailboxInfo).ToArray<MailboxInfo>(), input.Criteria, input.CallerInfo, input.PagingInfo);
						resultPage = multiMailboxSearchClient.FetchPreviewProperties(dictionary);
					}
				}
			}
			finally
			{
				policy.Recorder.End(record);
			}
			return new SearchMailboxesResults(input.Sources)
			{
				SearchResult = new ResultAggregator(resultPage, null, num, ByteQuantifiedSize.Zero, null, null, list)
			};
		}
	}
}
