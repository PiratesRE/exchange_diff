using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.MultiMailboxSearch;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Search;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal class Factory : IMultiMailboxSearchFactory
	{
		protected Factory()
		{
		}

		public static IMultiMailboxSearchFactory Current
		{
			get
			{
				return Factory.instance.Value;
			}
		}

		public static Hookable<IMultiMailboxSearchFactory> Instance
		{
			get
			{
				return Factory.instance;
			}
		}

		public ExEventLog EventLog
		{
			get
			{
				return Factory.eventLog;
			}
		}

		public Trace LocalTaskTracer
		{
			get
			{
				return ExTraceGlobals.LocalSearchTracer;
			}
		}

		public Trace MailboxGroupGeneratorTracer
		{
			get
			{
				return ExTraceGlobals.MailboxGroupGeneratorTracer;
			}
		}

		public Trace AutodiscoverTracer
		{
			get
			{
				return ExTraceGlobals.AutoDiscoverTracer;
			}
		}

		public Trace GeneralTracer
		{
			get
			{
				return ExTraceGlobals.GeneralTracer;
			}
		}

		public int MaxAllowedMailboxQueriesPerRequest
		{
			get
			{
				return Factory.DefaultMaxAllowedMailboxQueriesPerRequest;
			}
		}

		public TimeSpan GetDefaultSearchTimeout(IRecipientSession recipientSession)
		{
			int discoverySearchTimeoutPeriod = (int)SearchUtils.GetDiscoverySearchTimeoutPeriod(recipientSession);
			if (discoverySearchTimeoutPeriod <= 0)
			{
				return Factory.DefaultSearchTimeoutInterval;
			}
			return TimeSpan.FromMinutes((double)discoverySearchTimeoutPeriod);
		}

		public MailboxSearchGroup CreateMailboxSearchGroup(GroupId groupId, List<MailboxInfo> mailboxes, SearchCriteria searchCriteria, PagingInfo pagingInfo, CallerInfo executingUser)
		{
			switch (groupId.GroupType)
			{
			case GroupType.Local:
				return this.CreateAggregatedMailboxSearchGroup(mailboxes.ToArray(), searchCriteria, pagingInfo, executingUser);
			case GroupType.CrossServer:
			case GroupType.CrossPremise:
				return new WebServiceMailboxSearchGroup(groupId, mailboxes.ToArray(), searchCriteria, pagingInfo, executingUser);
			default:
				return null;
			}
		}

		public AggregatedMailboxSearchGroup CreateAggregatedMailboxSearchGroup(MailboxInfo[] mailboxes, SearchCriteria searchCriteria, PagingInfo pagingInfo, CallerInfo executingUser)
		{
			return new AggregatedMailboxSearchGroup(mailboxes, searchCriteria, pagingInfo, executingUser);
		}

		public MailboxInfoList CreateMailboxInfoList(MailboxInfo[] mailboxes)
		{
			return new MailboxInfoList(mailboxes);
		}

		public ISearchMailboxTask CreateAggregatedMailboxSearchTask(Guid databaseGuid, MailboxInfoList mailbox, SearchType type, SearchCriteria searchCriteria, PagingInfo pagingInfo, CallerInfo executingUser)
		{
			return new AggregatedMailboxSearchTask(databaseGuid, mailbox, type, searchCriteria, pagingInfo, executingUser);
		}

		public ISearchMailboxTask CreateAggregatedMailboxSearchTask(Guid databaseGuid, MailboxInfoList mailbox, SearchCriteria searchCriteria, PagingInfo pagingInfo, List<string> keywordList, CallerInfo executingUser)
		{
			return new AggregatedMailboxSearchTask(databaseGuid, mailbox, searchCriteria, pagingInfo, keywordList, executingUser);
		}

		public MultiMailboxSearchClient CreateSearchRpcClient(Guid databaseGuid, MailboxInfo[] mailboxes, SearchCriteria criteria, CallerInfo executingUserIdentity, PagingInfo pagingInfo)
		{
			return new MultiMailboxSearchClient(databaseGuid, mailboxes, criteria, executingUserIdentity, pagingInfo);
		}

		public int GetMaximumThreadsForLocalSearch(int numberOfMailboxes, IRecipientSession session)
		{
			int num = 0;
			int num2 = 0;
			ThreadPool.GetAvailableThreads(out num, out num2);
			return Math.Min(numberOfMailboxes, Math.Min(num / 2, this.GetMaxAllowedSearchThreads(session)));
		}

		public int GetMaximumAllowedPageSizeForLocalSearch(int pageSize, IRecipientSession session)
		{
			int num = this.GetPreviewSearchResultsPageSize(session);
			if (num <= 0)
			{
				num = Factory.DefaultMaxAllowedResultsPageSize;
			}
			return Math.Min(pageSize, num);
		}

		public bool IsDiscoverySearchEnabled(IRecipientSession recipientSession)
		{
			return SearchUtils.DiscoveryEnabled(recipientSession);
		}

		public int GetMaxAllowedKeywords(IRecipientSession recipientSession)
		{
			return (int)SearchUtils.GetDiscoveryMaxKeywords(recipientSession);
		}

		public int GetPreviewSearchResultsPageSize(IRecipientSession recipientSession)
		{
			return (int)SearchUtils.GetDiscoveryPreviewSearchResultsPageSize(recipientSession);
		}

		public int GetMaxAllowedKeywordsPerPage(IRecipientSession recipientSession)
		{
			return (int)SearchUtils.GetDiscoveryMaxKeywordsPerPage(recipientSession);
		}

		public int GetMaxAllowedSearchThreads(IRecipientSession recipientSession)
		{
			return (int)SearchUtils.GetDiscoveryMaxSearchQueueDepth(recipientSession);
		}

		public int GetMaxRefinerResults(IRecipientSession recipientSession)
		{
			return (int)SearchUtils.GetDiscoveryMaxRefinerResults(recipientSession);
		}

		public int GetMaxAllowedMailboxes(IRecipientSession recipientSession, SearchType searchType)
		{
			int result = (int)SearchUtils.GetDiscoveryMaxMailboxes(recipientSession);
			if (searchType == SearchType.Preview)
			{
				result = (int)SearchUtils.GetDiscoveryMaxMailboxesForPreviewSearch(recipientSession);
			}
			if (searchType == SearchType.Statistics)
			{
				result = (int)SearchUtils.GetDiscoveryMaxMailboxesForStatsSearch(recipientSession);
			}
			return result;
		}

		public IEwsEndpointDiscovery GetEwsEndpointDiscovery(List<MailboxInfo> mailboxes, OrganizationId orgId, CallerInfo callerInfo)
		{
			return new EwsEndpointDiscovery(mailboxes, orgId, callerInfo);
		}

		public IEwsClient CreateDiscoveryEwsClient(GroupId groupId, MailboxInfo[] mailboxes, SearchCriteria searchCriteria, PagingInfo pagingInfo, CallerInfo caller)
		{
			return new DiscoveryEwsClient(groupId, mailboxes, searchCriteria, pagingInfo, caller);
		}

		public IAutodiscoveryClient CreateUserSettingAutoDiscoveryClient(List<MailboxInfo> crossPremiseMailboxes, Uri autoDiscoveryEndpoint, ICredentials credentials, CallerInfo callerInfo)
		{
			return new UserSettingAutodiscovery(crossPremiseMailboxes, autoDiscoveryEndpoint, credentials, callerInfo);
		}

		public INonIndexableDiscoveryEwsClient CreateNonIndexableDiscoveryEwsClient(GroupId groupId, MailboxInfo[] mailboxes, ExTimeZone timeZone, CallerInfo caller)
		{
			return new NonIndexableDiscoveryEwsClient(groupId, mailboxes, timeZone, caller);
		}

		public bool IsSearchAllowed(IRecipientSession recipientSession, SearchType searchType, int totalMailboxesToSearchCount)
		{
			EDiscoverySearchVerdict ediscoverySearchVerdict = Util.ComputeDiscoverySearchVerdict(recipientSession, searchType, totalMailboxesToSearchCount);
			return ediscoverySearchVerdict.Equals(EDiscoverySearchVerdict.Allowed);
		}

		internal const string EventLogSourceName = "Exchange Discovery Search";

		internal static Guid EventLogComponentGuid = new Guid("d34c8ffd-7201-4ffc-8851-1011b950a219");

		internal static ExEventLog eventLog = new ExEventLog(Factory.EventLogComponentGuid, "Exchange Discovery Search");

		protected static readonly int DefaultKeywordsBatchSize = 6;

		private static readonly TimeSpan DefaultSearchTimeoutInterval = TimeSpan.FromMinutes(4.5);

		private static readonly int DefaultMaxAllowedMailboxQueriesPerRequest = 5;

		private static readonly int DefaultMaxAllowedResultsPageSize = 500;

		private static readonly Hookable<IMultiMailboxSearchFactory> instance = Hookable<IMultiMailboxSearchFactory>.Create(true, new Factory());
	}
}
