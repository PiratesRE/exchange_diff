using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IStorageLimits
	{
		int NamedPropertyNameMaximumLength { get; }

		int UserConfigurationMaxSearched { get; }

		int FindNamesViewResultsLimit { get; }

		int AmbiguousNamesViewResultsLimit { get; }

		int CalendarSingleInstanceLimit { get; }

		int CalendarExpansionInstanceLimit { get; }

		int CalendarExpansionMaxMasters { get; }

		int CalendarMaxNumberVEventsForICalImport { get; }

		int CalendarMaxNumberBytesForICalImport { get; }

		int RecurrenceMaximumInterval { get; }

		int RecurrenceMaximumNumberedOccurrences { get; }

		int DistributionListMaxMembersPropertySize { get; }

		int DistributionListMaxNumberOfEntries { get; }

		int DefaultFolderMaximumSuffix { get; }

		int DefaultFolderMinimumSuffix { get; }

		int DefaultFolderDataCacheMaxRowCount { get; }

		int NotificationsMaxSubscriptions { get; }

		int MaxDelegates { get; }

		BufferPoolCollection.BufferSize PropertyStreamPageSize { get; }

		long ConversionsFolderMaxTotalMessageSize { get; }
	}
}
