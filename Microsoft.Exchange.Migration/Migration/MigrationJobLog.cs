using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	internal static class MigrationJobLog
	{
		public static void LogStatusEvent(MigrationJob migrationObject)
		{
			MigrationJobLog.MigrationJobLogger.LogEvent(migrationObject);
		}

		internal static string GetMigrationTypeString(bool isStaged, MigrationType migrationType)
		{
			if (migrationType != MigrationType.ExchangeOutlookAnywhere || !isStaged)
			{
				return migrationType.ToString();
			}
			return "StagedExchangeOutlookAnywhere";
		}

		private const int MaxInternalErrorSize = 8192;

		private class MigrationJobLogger : MigrationObjectLog<MigrationJob, MigrationJobLog.MigrationJobLogSchema, MigrationJobLog.MigrationJobLogConfiguration>
		{
			public static void LogEvent(MigrationJob migrationObject)
			{
				MigrationObjectLog<MigrationJob, MigrationJobLog.MigrationJobLogSchema, MigrationJobLog.MigrationJobLogConfiguration>.Write(migrationObject);
			}
		}

		private class MigrationJobLogSchema : ObjectLogSchema
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
					return "Migration Job";
				}
			}

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> JobId = new ObjectLogSimplePropertyDefinition<MigrationJob>("JobId", (MigrationJob d) => d.JobId);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> TenantName = new ObjectLogSimplePropertyDefinition<MigrationJob>("TenantName", (MigrationJob d) => d.TenantName);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> MigrationType = new ObjectLogSimplePropertyDefinition<MigrationJob>("MigrationType", (MigrationJob d) => MigrationJobLog.GetMigrationTypeString(d.IsStaged, d.MigrationType));

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> ObjectVersion = new ObjectLogSimplePropertyDefinition<MigrationJob>("ObjectVersion", (MigrationJob d) => d.Version);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> Status = new ObjectLogSimplePropertyDefinition<MigrationJob>("Status", (MigrationJob d) => d.Status);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> JobName = new ObjectLogSimplePropertyDefinition<MigrationJob>("JobName", (MigrationJob d) => d.JobName);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> LocalizedError = new ObjectLogSimplePropertyDefinition<MigrationJob>("LocalizedError", (MigrationJob d) => d.StatusData.LocalizedError);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> InternalError = new ObjectLogSimplePropertyDefinition<MigrationJob>("InternalError", delegate(MigrationJob d)
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

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> WatsonHash = new ObjectLogSimplePropertyDefinition<MigrationJob>("WatsonHash", (MigrationJob d) => d.StatusData.WatsonHash);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> Direction = new ObjectLogSimplePropertyDefinition<MigrationJob>("Direction", (MigrationJob d) => d.JobDirection);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> CreationTime = new ObjectLogSimplePropertyDefinition<MigrationJob>("CreationTime", (MigrationJob d) => d.OriginalCreationTime);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> StartTime = new ObjectLogSimplePropertyDefinition<MigrationJob>("StartTime", (MigrationJob d) => d.StartTime);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> InitialSyncTime = new ObjectLogSimplePropertyDefinition<MigrationJob>("InitialSyncTime", (MigrationJob d) => d.InitialSyncDateTime);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> FinalizedTime = new ObjectLogSimplePropertyDefinition<MigrationJob>("FinalizedTime", (MigrationJob d) => d.FinalizeTime);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> LastSyncTime = new ObjectLogSimplePropertyDefinition<MigrationJob>("LastSyncTime", (MigrationJob d) => d.LastSyncTime);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> StartAfterTime = new ObjectLogSimplePropertyDefinition<MigrationJob>("StartAfterTime", delegate(MigrationJob d)
			{
				MoveJobSubscriptionSettings moveJobSubscriptionSettings = d.SubscriptionSettings as MoveJobSubscriptionSettings;
				ExchangeJobSubscriptionSettings exchangeJobSubscriptionSettings = d.SubscriptionSettings as ExchangeJobSubscriptionSettings;
				return (moveJobSubscriptionSettings != null) ? moveJobSubscriptionSettings.StartAfter : ((exchangeJobSubscriptionSettings != null) ? exchangeJobSubscriptionSettings.StartAfter : null);
			});

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> CompleteAfterTime = new ObjectLogSimplePropertyDefinition<MigrationJob>("CompleteAfter", delegate(MigrationJob d)
			{
				MoveJobSubscriptionSettings moveJobSubscriptionSettings = d.SubscriptionSettings as MoveJobSubscriptionSettings;
				return (moveJobSubscriptionSettings != null) ? moveJobSubscriptionSettings.CompleteAfter : null;
			});

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> InitialSyncDuration = new ObjectLogSimplePropertyDefinition<MigrationJob>("InitialSyncDuration", (MigrationJob d) => 0);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> TotalCount = new ObjectLogSimplePropertyDefinition<MigrationJob>("TotalCount", (MigrationJob d) => d.TotalCount);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> ActiveCount = new ObjectLogSimplePropertyDefinition<MigrationJob>("ActiveCount", (MigrationJob d) => d.ActiveItemCount);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> StoppedCount = new ObjectLogSimplePropertyDefinition<MigrationJob>("StoppedCount", (MigrationJob d) => d.StoppedItemCount);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> SyncedCount = new ObjectLogSimplePropertyDefinition<MigrationJob>("SyncedCount", (MigrationJob d) => d.SyncedItemCount);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> FinalizedCount = new ObjectLogSimplePropertyDefinition<MigrationJob>("FinalizedCount", (MigrationJob d) => d.FinalizedItemCount);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> FailedCount = new ObjectLogSimplePropertyDefinition<MigrationJob>("FailedCount", (MigrationJob d) => d.FailedItemCount);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> PendingCount = new ObjectLogSimplePropertyDefinition<MigrationJob>("PendingCount", (MigrationJob d) => d.PendingCount);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> ProvisionedCount = new ObjectLogSimplePropertyDefinition<MigrationJob>("ProvisionedCount", (MigrationJob d) => d.ProvisionedItemCount);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> ValidationWarningCount = new ObjectLogSimplePropertyDefinition<MigrationJob>("ValidationWarningCount", (MigrationJob d) => d.ValidationWarningCount);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> Locale = new ObjectLogSimplePropertyDefinition<MigrationJob>("Locale", (MigrationJob d) => d.AdminCulture);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> BatchFlags = new ObjectLogSimplePropertyDefinition<MigrationJob>("BatchFlags", (MigrationJob d) => d.BatchFlags);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> AutoRetryCount = new ObjectLogSimplePropertyDefinition<MigrationJob>("AutoRetryCount", (MigrationJob d) => d.MaxAutoRunCount);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> CurrentRetryCount = new ObjectLogSimplePropertyDefinition<MigrationJob>("CurrentRetryCount", (MigrationJob d) => d.AutoRunCount);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> BadItemLimit = new ObjectLogSimplePropertyDefinition<MigrationJob>("BadItemLimit", delegate(MigrationJob d)
			{
				MoveJobSubscriptionSettings moveJobSubscriptionSettings = d.SubscriptionSettings as MoveJobSubscriptionSettings;
				return (moveJobSubscriptionSettings != null) ? moveJobSubscriptionSettings.BadItemLimit : null;
			});

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> LargeItemLimit = new ObjectLogSimplePropertyDefinition<MigrationJob>("LargeItemLimit", delegate(MigrationJob d)
			{
				MoveJobSubscriptionSettings moveJobSubscriptionSettings = d.SubscriptionSettings as MoveJobSubscriptionSettings;
				return (moveJobSubscriptionSettings != null) ? moveJobSubscriptionSettings.LargeItemLimit : null;
			});

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> PrimaryOnly = new ObjectLogSimplePropertyDefinition<MigrationJob>("PrimaryOnly", delegate(MigrationJob d)
			{
				MoveJobSubscriptionSettings moveJobSubscriptionSettings = d.SubscriptionSettings as MoveJobSubscriptionSettings;
				return (moveJobSubscriptionSettings != null) ? moveJobSubscriptionSettings.PrimaryOnly : null;
			});

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> ArchiveOnly = new ObjectLogSimplePropertyDefinition<MigrationJob>("ArchiveOnly", delegate(MigrationJob d)
			{
				MoveJobSubscriptionSettings moveJobSubscriptionSettings = d.SubscriptionSettings as MoveJobSubscriptionSettings;
				return (moveJobSubscriptionSettings != null) ? moveJobSubscriptionSettings.ArchiveOnly : null;
			});

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> TargetDeliveryDomain = new ObjectLogSimplePropertyDefinition<MigrationJob>("TargetDeliveryDomain", (MigrationJob d) => d.TargetDomainName);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> SkipSteps = new ObjectLogSimplePropertyDefinition<MigrationJob>("SkipSteps", (MigrationJob d) => d.SkipSteps);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> SourceEndpointGuid = new ObjectLogSimplePropertyDefinition<MigrationJob>("SourceEndpointGuid", (MigrationJob d) => (d.SourceEndpoint == null) ? null : new Guid?(d.SourceEndpoint.Guid));

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> TargetEndpointGuid = new ObjectLogSimplePropertyDefinition<MigrationJob>("TargetEndpointGuid", (MigrationJob d) => (d.TargetEndpoint == null) ? null : new Guid?(d.TargetEndpoint.Guid));

			public static readonly ObjectLogSimplePropertyDefinition<MigrationJob> ProcessingDuration = new ObjectLogSimplePropertyDefinition<MigrationJob>("ProcessingDuration", (MigrationJob d) => d.ProcessingDuration.TotalMilliseconds);
		}

		private class MigrationJobLogConfiguration : MigrationObjectLogConfiguration
		{
			public override string LoggingFolder
			{
				get
				{
					return base.LoggingFolder + "\\MigrationJob";
				}
			}

			public override long MaxLogDirSize
			{
				get
				{
					return ConfigBase<MigrationServiceConfigSchema>.GetConfig<long>("MigrationReportingJobMaxDirSize");
				}
			}

			public override long MaxLogFileSize
			{
				get
				{
					return ConfigBase<MigrationServiceConfigSchema>.GetConfig<long>("MigrationReportingJobMaxFileSize");
				}
			}

			public override string LogComponentName
			{
				get
				{
					return "MigrationJobLog";
				}
			}

			public override string FilenamePrefix
			{
				get
				{
					return "MigrationJob_Log_";
				}
			}
		}
	}
}
