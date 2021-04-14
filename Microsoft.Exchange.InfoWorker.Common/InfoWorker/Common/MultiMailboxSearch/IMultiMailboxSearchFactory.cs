using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal interface IMultiMailboxSearchFactory
	{
		Trace LocalTaskTracer { get; }

		Trace MailboxGroupGeneratorTracer { get; }

		Trace GeneralTracer { get; }

		Trace AutodiscoverTracer { get; }

		ExEventLog EventLog { get; }

		int MaxAllowedMailboxQueriesPerRequest { get; }

		TimeSpan GetDefaultSearchTimeout(IRecipientSession recipientSession);

		int GetMaxAllowedKeywords(IRecipientSession recipientSession);

		int GetMaxAllowedMailboxes(IRecipientSession recipientSession, SearchType searchType);

		bool IsSearchAllowed(IRecipientSession recipientSession, SearchType searchType, int totalMailboxesToSearchCount);

		bool IsDiscoverySearchEnabled(IRecipientSession recipientSession);

		int GetPreviewSearchResultsPageSize(IRecipientSession recipientSession);

		int GetMaxAllowedKeywordsPerPage(IRecipientSession recipientSession);

		int GetMaxAllowedSearchThreads(IRecipientSession recipientSession);

		int GetMaxRefinerResults(IRecipientSession recipientSession);

		MailboxSearchGroup CreateMailboxSearchGroup(GroupId groupId, List<MailboxInfo> mailboxes, SearchCriteria searchCriteria, PagingInfo pagingInfo, CallerInfo executingUser);

		int GetMaximumThreadsForLocalSearch(int numberOfMailboxes, IRecipientSession session);

		int GetMaximumAllowedPageSizeForLocalSearch(int pageSize, IRecipientSession session);

		MultiMailboxSearchClient CreateSearchRpcClient(Guid databaseGuid, MailboxInfo[] mailboxes, SearchCriteria criteria, CallerInfo executingUserIdentity, PagingInfo pagingInfo);

		ISearchMailboxTask CreateAggregatedMailboxSearchTask(Guid databaseGuid, MailboxInfoList mailbox, SearchType type, SearchCriteria searchCriteria, PagingInfo pagingInfo, CallerInfo executingUser);

		ISearchMailboxTask CreateAggregatedMailboxSearchTask(Guid databaseGuid, MailboxInfoList mailbox, SearchCriteria searchCriteria, PagingInfo pagingInfo, List<string> keywordList, CallerInfo executingUser);

		AggregatedMailboxSearchGroup CreateAggregatedMailboxSearchGroup(MailboxInfo[] mailboxes, SearchCriteria searchCriteria, PagingInfo pagingInfo, CallerInfo executingUser);

		MailboxInfoList CreateMailboxInfoList(MailboxInfo[] mailboxes);

		IEwsEndpointDiscovery GetEwsEndpointDiscovery(List<MailboxInfo> mailboxes, OrganizationId orgId, CallerInfo callerInfo);

		IEwsClient CreateDiscoveryEwsClient(GroupId groupId, MailboxInfo[] mailboxes, SearchCriteria searchCriteria, PagingInfo pagingInfo, CallerInfo caller);

		IAutodiscoveryClient CreateUserSettingAutoDiscoveryClient(List<MailboxInfo> crossPremiseMailboxes, Uri autoDiscoverEndpoint, ICredentials credentials, CallerInfo callerInfo);

		INonIndexableDiscoveryEwsClient CreateNonIndexableDiscoveryEwsClient(GroupId groupId, MailboxInfo[] mailboxes, ExTimeZone timeZone, CallerInfo caller);
	}
}
