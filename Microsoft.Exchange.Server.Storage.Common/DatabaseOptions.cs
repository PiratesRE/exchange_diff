using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class DatabaseOptions
	{
		public DatabaseOptions Clone()
		{
			return new DatabaseOptions
			{
				MinCachePages = this.MinCachePages,
				MaxCachePages = this.MaxCachePages,
				EnableOnlineDefragmentation = this.EnableOnlineDefragmentation,
				BackgroundDatabaseMaintenance = this.BackgroundDatabaseMaintenance,
				ReplayBackgroundDatabaseMaintenance = this.ReplayBackgroundDatabaseMaintenance,
				BackgroundDatabaseMaintenanceSerialization = this.BackgroundDatabaseMaintenanceSerialization,
				BackgroundDatabaseMaintenanceDelay = this.BackgroundDatabaseMaintenanceDelay,
				ReplayBackgroundDatabaseMaintenanceDelay = this.ReplayBackgroundDatabaseMaintenanceDelay,
				MimimumBackgroundDatabaseMaintenanceInterval = this.MimimumBackgroundDatabaseMaintenanceInterval,
				MaximumBackgroundDatabaseMaintenanceInterval = this.MaximumBackgroundDatabaseMaintenanceInterval,
				TemporaryDataFolderPath = this.TemporaryDataFolderPath,
				LogBuffers = this.LogBuffers,
				MaximumOpenTables = this.MaximumOpenTables,
				MaximumTemporaryTables = this.MaximumTemporaryTables,
				MaximumCursors = this.MaximumCursors,
				MaximumSessions = this.MaximumSessions,
				MaximumVersionStorePages = this.MaximumVersionStorePages,
				PreferredVersionStorePages = this.PreferredVersionStorePages,
				DatabaseExtensionSize = this.DatabaseExtensionSize,
				LogCheckpointDepth = this.LogCheckpointDepth,
				ReplayCheckpointDepth = this.ReplayCheckpointDepth,
				CachedClosedTables = this.CachedClosedTables,
				CachePriority = this.CachePriority,
				ReplayCachePriority = this.ReplayCachePriority,
				MaximumPreReadPages = this.MaximumPreReadPages,
				MaximumReplayPreReadPages = this.MaximumReplayPreReadPages,
				LogFilePrefix = this.LogFilePrefix,
				TotalDatabasesOnServer = this.TotalDatabasesOnServer,
				MaxActiveDatabases = this.MaxActiveDatabases
			};
		}

		public int? MinCachePages;

		public int? MaxCachePages;

		public bool? EnableOnlineDefragmentation;

		public bool BackgroundDatabaseMaintenance = true;

		public bool? ReplayBackgroundDatabaseMaintenance = new bool?(true);

		public bool? BackgroundDatabaseMaintenanceSerialization;

		public int? BackgroundDatabaseMaintenanceDelay;

		public int? ReplayBackgroundDatabaseMaintenanceDelay;

		public int? MimimumBackgroundDatabaseMaintenanceInterval;

		public int? MaximumBackgroundDatabaseMaintenanceInterval;

		public string TemporaryDataFolderPath;

		public string LogFilePrefix;

		public int? LogBuffers;

		public int? MaximumOpenTables;

		public int? MaximumTemporaryTables;

		public int? MaximumCursors;

		public int? MaximumSessions;

		public int? MaximumVersionStorePages;

		public int? PreferredVersionStorePages;

		public int? DatabaseExtensionSize;

		public int? LogCheckpointDepth;

		public int? ReplayCheckpointDepth;

		public int? CachedClosedTables;

		public int? CachePriority;

		public int? ReplayCachePriority;

		public int? MaximumPreReadPages;

		public int? MaximumReplayPreReadPages;

		public int? TotalDatabasesOnServer;

		public int? MaxActiveDatabases;
	}
}
