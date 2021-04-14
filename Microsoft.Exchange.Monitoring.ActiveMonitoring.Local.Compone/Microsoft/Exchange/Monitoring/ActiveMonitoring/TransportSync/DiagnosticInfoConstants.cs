using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TransportSync
{
	public static class DiagnosticInfoConstants
	{
		public const string ComponentName = "syncmanager";

		public const string Database = "Database";

		public const string DatabaseId = "databaseId";

		public const string DatabaseQueueManager = "DatabaseQueueManager";

		public const string DefaultArgument = "basic";

		public const string DispatchManager = "DispatchManager";

		public const string Enabled = "enabled";

		public const string GlobalDatabaseHandler = "GlobalDatabaseHandler";

		public const string InfoArgument = "info";

		public const string ItemsOutOfSla = "itemsOutOfSla";

		public const string ItemsOutOfSlaPercent = "itemsOutOfSlaPercent";

		public const string LastDatabaseDiscoveryStartTime = "LastDatabaseDiscoveryStartTime";

		public const string NextPollingTime = "nextPollingTime";

		public const string PollingQueue = "PollingQueue";

		public const string SubComponents = "databasemanager dispatchmanager";

		public const string WorkType = "workType";

		public static readonly int AverageNumberOfMdbsPerServer = 200;

		public static readonly TimeSpan DiagnosticInfoCacheTimeout = TimeSpan.FromMinutes(1.0);

		public static readonly string WorkTypeName = "AggregationIncremental";
	}
}
