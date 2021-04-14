using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model
{
	internal interface IExecutionSettings
	{
		VariantConfigurationSnapshot Snapshot { get; }

		bool DiscoveryUseFastSearch { get; }

		bool DiscoveryAggregateLogs { get; }

		bool DiscoveryExecutesInParallel { get; }

		bool DiscoveryLocalSearchIsParallel { get; }

		int DiscoveryMaxMailboxes { get; }

		uint DiscoveryMaxAllowedExecutorItems { get; }

		int DiscoveryMaxAllowedMailboxQueriesPerRequest { get; }

		uint DiscoverySynchronousConcurrency { get; }

		uint DiscoveryADLookupConcurrency { get; }

		uint DiscoveryFanoutConcurrency { get; }

		uint DiscoveryServerLookupConcurrency { get; }

		uint DiscoveryLocalSearchConcurrency { get; }

		uint DiscoveryServerLookupBatch { get; }

		uint DiscoveryFanoutBatch { get; }

		uint DiscoveryLocalSearchBatch { get; }

		int DiscoveryKeywordsBatchSize { get; }

		uint DiscoveryDisplaySearchBatchSize { get; }

		uint DiscoveryDisplaySearchPageSize { get; }

		int DiscoveryMaxAllowedResultsPageSize { get; }

		int DiscoveryDefaultPageSize { get; }

		uint DiscoveryADPageSize { get; }

		TimeSpan SearchTimeout { get; }

		TimeSpan ServiceTopologyTimeout { get; }

		TimeSpan MailboxServerLocatorTimeout { get; }

		List<DefaultFolderType> ExcludedFolders { get; }
	}
}
