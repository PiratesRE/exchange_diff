using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	internal static class MigrationJobItemLog
	{
		public static void LogStatusEvent(MigrationJobItem migrationObject)
		{
			if (!MigrationJobItemLog.jobItemStatusForLogging.Contains(migrationObject.Status))
			{
				return;
			}
			MigrationJobItemLog.MigrationJobItemLogger.LogEvent(migrationObject);
		}

		private const int MaxInternalErrorSize = 8192;

		private static HashSet<MigrationUserStatus> jobItemStatusForLogging = new HashSet<MigrationUserStatus>(new MigrationUserStatus[]
		{
			MigrationUserStatus.Queued,
			MigrationUserStatus.Syncing,
			MigrationUserStatus.Failed,
			MigrationUserStatus.Synced,
			MigrationUserStatus.IncrementalSyncing,
			MigrationUserStatus.IncrementalFailed,
			MigrationUserStatus.Completing,
			MigrationUserStatus.Completed,
			MigrationUserStatus.CompletionFailed,
			MigrationUserStatus.CompletedWithWarnings
		});

		private class MigrationJobItemLogger : MigrationObjectLog<MigrationJobItem, MigrationJobItemLog.MigrationJobItemLogSchema, MigrationJobItemLog.MigrationJobItemLogConfiguration>
		{
			public static void LogEvent(MigrationJobItem migrationObject)
			{
				MigrationObjectLog<MigrationJobItem, MigrationJobItemLog.MigrationJobItemLogSchema, MigrationJobItemLog.MigrationJobItemLogConfiguration>.Write(migrationObject);
			}
		}

		private class MigrationJobItemLogSchema : ObjectLogSchema
		{
			public override string Software
			{
				get
				{
					return "Microsoft Exchange Migration";
				}
			}

			public override string LogType
			{
				get
				{
					return "Migration JobItem";
				}
			}

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJobItem> JobItemGuid = new ObjectLogSimplePropertyDefinition<MigrationJobItem>("JobItemGuid", (MigrationJobItem d) => d.JobItemGuid);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJobItem> MigrationJobId = new ObjectLogSimplePropertyDefinition<MigrationJobItem>("MigrationJobId", (MigrationJobItem d) => d.MigrationJobId);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJobItem> ObjectVersion = new ObjectLogSimplePropertyDefinition<MigrationJobItem>("ObjectVersion", (MigrationJobItem d) => d.Version);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJobItem> Status = new ObjectLogSimplePropertyDefinition<MigrationJobItem>("Status", (MigrationJobItem d) => (d.Status == MigrationUserStatus.Synced && d.IncrementalSyncDuration != null) ? MigrationUserStatus.IncrementalSynced : d.Status);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJobItem> ItemsSynced = new ObjectLogSimplePropertyDefinition<MigrationJobItem>("ItemsSynced", (MigrationJobItem d) => d.ItemsSynced);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJobItem> ItemsSkipped = new ObjectLogSimplePropertyDefinition<MigrationJobItem>("ItemsSkipped", (MigrationJobItem d) => d.ItemsSkipped);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJobItem> LocalizedError = new ObjectLogSimplePropertyDefinition<MigrationJobItem>("LocalizedError", (MigrationJobItem d) => d.StatusData.LocalizedError);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJobItem> InternalError = new ObjectLogSimplePropertyDefinition<MigrationJobItem>("InternalError", delegate(MigrationJobItem d)
			{
				if (d.StatusData.InternalError == null)
				{
					return null;
				}
				if (d.StatusData.InternalError.Length <= 8192)
				{
					return d.StatusData.InternalError;
				}
				return d.StatusData.InternalError.Remove(8192);
			});

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJobItem> Organization = new ObjectLogSimplePropertyDefinition<MigrationJobItem>("TenantName", (MigrationJobItem d) => d.TenantName);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJobItem> JobName = new ObjectLogSimplePropertyDefinition<MigrationJobItem>("JobName", (MigrationJobItem d) => d.JobName);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJobItem> MigrationType = new ObjectLogSimplePropertyDefinition<MigrationJobItem>("MigrationType", (MigrationJobItem d) => MigrationJobLog.GetMigrationTypeString(d.IsStaged, d.MigrationType));

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJobItem> SubscriptionId = new ObjectLogSimplePropertyDefinition<MigrationJobItem>("SubscriptionId", (MigrationJobItem d) => d.SubscriptionId);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJobItem> WatsonHash = new ObjectLogSimplePropertyDefinition<MigrationJobItem>("WatsonHash", (MigrationJobItem d) => d.StatusData.WatsonHash);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJobItem> OverallCmdletDuration = new ObjectLogSimplePropertyDefinition<MigrationJobItem>("OverallCmdletDuration", (MigrationJobItem d) => (d.OverallCmdletDuration == null) ? null : new long?((long)d.OverallCmdletDuration.Value.TotalMilliseconds));

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJobItem> SubscriptionInjectionDuration = new ObjectLogSimplePropertyDefinition<MigrationJobItem>("SubscriptionInjectionDuration", (MigrationJobItem d) => (d.SubscriptionInjectionDuration == null) ? null : new long?((long)d.SubscriptionInjectionDuration.Value.TotalMilliseconds));

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJobItem> ProvisioningDuration = new ObjectLogSimplePropertyDefinition<MigrationJobItem>("ProvisioningDuration", (MigrationJobItem d) => (d.ProvisioningDuration == null) ? null : new long?((long)d.ProvisioningDuration.Value.TotalMilliseconds));

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJobItem> ProvisionedTime = new ObjectLogSimplePropertyDefinition<MigrationJobItem>("ProvisionedTime", (MigrationJobItem d) => d.ProvisionedTime);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJobItem> SubscriptionQueuedTime = new ObjectLogSimplePropertyDefinition<MigrationJobItem>("SubscriptionQueuedTime", (MigrationJobItem d) => d.SubscriptionQueuedTime);
		}

		private class MigrationJobItemLogConfiguration : MigrationObjectLogConfiguration
		{
			public override string LoggingFolder
			{
				get
				{
					return base.LoggingFolder + "\\MigrationJobItem";
				}
			}

			public override long MaxLogDirSize
			{
				get
				{
					return ConfigBase<MigrationServiceConfigSchema>.GetConfig<long>("MigrationReportingJobItemMaxDirSize");
				}
			}

			public override long MaxLogFileSize
			{
				get
				{
					return ConfigBase<MigrationServiceConfigSchema>.GetConfig<long>("MigrationReportingJobItemMaxFileSize");
				}
			}

			public override string LogComponentName
			{
				get
				{
					return "MigrationJobItemLog";
				}
			}

			public override string FilenamePrefix
			{
				get
				{
					return "MigrationJobItem_Log_";
				}
			}
		}
	}
}
