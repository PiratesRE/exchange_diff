using System;
using System.Collections.Generic;
using Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model
{
	internal class SearchMailboxesInputs
	{
		public SearchMailboxesInputs()
		{
			this.Sources = new List<SearchSource>();
		}

		public bool IsLocalCall { get; set; }

		public List<SearchSource> Sources { get; set; }

		public PagingInfo PagingInfo { get; set; }

		public CallerInfo CallerInfo { get; set; }

		public SearchCriteria Criteria { get; set; }

		public string SearchConfigurationId { get; set; }

		public string SearchQuery { get; set; }

		public string Language { get; set; }

		public SearchType SearchType { get; set; }

		public Guid RequestId { get; set; }

		internal SearchMailboxesInputs Clone()
		{
			return (SearchMailboxesInputs)base.MemberwiseClone();
		}

		internal void UpdateRequest(SearchMailboxesRequest request, IEnumerable<SearchSource> sources)
		{
			request.Language = this.Language;
			request.PageDirection = DiscoveryEwsClient.GetPageDirection(this.PagingInfo.Direction);
			request.PageItemReference = ((this.PagingInfo.SortValue != null) ? this.PagingInfo.SortValue.ToString() : null);
			request.PageSize = this.PagingInfo.PageSize;
			request.PerformDeduplication = this.PagingInfo.ExcludeDuplicates;
			request.PreviewItemResponseShape = DiscoveryEwsClient.GetPreviewItemResponseShape(this.PagingInfo.BaseShape, this.PagingInfo.AdditionalProperties);
			request.SearchQueries = this.GetSearchQueries(sources);
			request.ResultType = DiscoveryEwsClient.GetSearchType(this.SearchType);
			if (request.SearchQueries.Count > 0 && request.SearchQueries[0].MailboxSearchScopes.Length > 0)
			{
				request.SearchQueries[0].MailboxSearchScopes[0].ExtendedAttributes.Add(new ExtendedAttribute("SearchType", this.SearchType.ToString()));
			}
			request.SortByProperty = (this.PagingInfo.OriginalSortByReference ?? DiscoveryEwsClient.GetSortbyProperty(this.PagingInfo.SortBy));
			request.SortOrder = DiscoveryEwsClient.GetSortDirection(this.PagingInfo.SortBy);
		}

		internal List<MailboxQuery> GetSearchQueries(IEnumerable<SearchSource> sources)
		{
			List<MailboxQuery> list = new List<MailboxQuery>();
			List<MailboxSearchScope> list2 = new List<MailboxSearchScope>();
			foreach (SearchSource searchSource in sources)
			{
				list2.Add(searchSource.GetScope());
			}
			MailboxQuery item = new MailboxQuery(this.SearchQuery, list2.ToArray());
			list.Add(item);
			return list;
		}
	}
}
