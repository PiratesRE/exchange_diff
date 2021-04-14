using System;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal static class DiscoveryConstants
	{
		public const string EwsTypeNamespace = "http://schemas.microsoft.com/exchange/services/2006/types";

		public const string OpenAsAdminOrSystemService = "OpenAsAdminOrSystemService";

		public const string ConnectingSID = "ConnectingSID";

		public const string SearchScopeType = "SearchScopeType";

		public const string AutoDetectScopeType = "AutoDetect";

		public const string SavedSearchScopeType = "SavedSearchId";

		public const string SearchType = "SearchType";

		public const string ExpandSourcesSearchType = "ExpandSources";

		public const string NonIndexableItemDetailsSearchType = "NonIndexedItemPreview";

		public const string NonIndexableItemStatisticsSearchType = "NonIndexedItemStatistics";

		public const string PublicFolderMarker = "\\";

		public const long DefaultPSTSizeLimitInBytes = 10000000000L;

		public const int DefaultSearchMailboxesPageSize = 500;

		public const int DefaultExportBatchItemCountLimit = 250;

		public const int DefaultExportBatchSizeLimit = 5242880;

		public const int DefaultItemIdListCacheSize = 500;

		public const int DefaultRetryInterval = 30000;

		public const int DefaultMaxCSVLogFileSizeInBytes = 104857600;

		public const bool DefaultPartitionCSVLogFile = true;

		public const int DefaultAutoDiscoverBatchSize = 50;

		public static readonly TimeSpan DefaultTotalRetryTimeWindow = TimeSpan.FromMinutes(15.0);

		public static readonly TimeSpan[] DefaultRetrySchedule = new TimeSpan[]
		{
			TimeSpan.FromSeconds(30.0),
			TimeSpan.FromMinutes(2.0),
			TimeSpan.FromMinutes(6.0)
		};
	}
}
