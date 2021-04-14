using System;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model
{
	internal interface IThrottlingSettings
	{
		uint DiscoveryMaxConcurrency { get; }

		uint DiscoveryMaxKeywords { get; }

		uint DiscoveryMaxKeywordsPerPage { get; }

		uint DiscoveryMaxMailboxes { get; }

		uint DiscoveryMaxPreviewSearchMailboxes { get; }

		uint DiscoveryMaxRefinerResults { get; }

		uint DiscoveryMaxSearchQueueDepth { get; }

		uint DiscoveryMaxStatsSearchMailboxes { get; }

		uint DiscoveryPreviewSearchResultsPageSize { get; }

		uint DiscoverySearchTimeoutPeriod { get; }
	}
}
