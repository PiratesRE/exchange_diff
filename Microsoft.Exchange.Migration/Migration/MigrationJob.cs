using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.Management.Migration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Migration.DataAccessLayer;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationJob : MigrationMessagePersistableBase
	{
		private MigrationJob()
		{
			this.currentSupportedVersion = 4L;
		}

		private MigrationJob(MigrationType migrationType)
		{
			this.Initialize(migrationType);
		}

		public MigrationType MigrationType
		{
			get
			{
				return this.migrationJobType;
			}
		}

		public Guid JobId
		{
			get
			{
				return this.jobId;
			}
			private set
			{
				this.jobId = value;
			}
		}

		public string JobName
		{
			get
			{
				return this.jobName;
			}
			private set
			{
				this.jobName = value;
			}
		}

		public MigrationJobStatus Status
		{
			get
			{
				return this.status;
			}
		}

		public MigrationState State
		{
			get
			{
				return this.StatusData.State;
			}
		}

		public MigrationStatusData<MigrationJobStatus> StatusData
		{
			get
			{
				return this.statusData;
			}
			protected set
			{
				this.statusData = value;
				if (this.statusData != null)
				{
					this.status = this.statusData.Status;
				}
			}
		}

		public ExDateTime? LastScheduled
		{
			get
			{
				return this.lastScheduled;
			}
			private set
			{
				this.lastScheduled = value;
			}
		}

		public ExDateTime? NextProcessTime
		{
			get
			{
				return this.nextProcessTime;
			}
			private set
			{
				this.nextProcessTime = value;
			}
		}

		public ExDateTime OriginalCreationTime
		{
			get
			{
				return this.originalCreationTime;
			}
			private set
			{
				this.originalCreationTime = value;
			}
		}

		public ExDateTime? StartTime
		{
			get
			{
				return this.startTime;
			}
			private set
			{
				this.startTime = value;
			}
		}

		public ExDateTime? LastRestartTime { get; private set; }

		public ExDateTime? FinalizeTime
		{
			get
			{
				return this.finalizeTime;
			}
			private set
			{
				this.finalizeTime = value;
			}
		}

		public int LastFinalizationAttempt
		{
			get
			{
				return this.lastFinalizationAttempt;
			}
			private set
			{
				this.lastFinalizationAttempt = value;
			}
		}

		public ExTimeZone UserTimeZone
		{
			get
			{
				return this.userTimeZone;
			}
			private set
			{
				this.userTimeZone = value;
			}
		}

		public string SubmittedByUser
		{
			get
			{
				return this.submittedByUser;
			}
			private set
			{
				this.submittedByUser = value;
			}
		}

		public int TotalCount
		{
			get
			{
				int num = this.TotalRowCount;
				int validationWarningCount = this.ValidationWarningCount;
				int removedItemCount = this.RemovedItemCount;
				if (num < validationWarningCount + removedItemCount)
				{
					return 0;
				}
				return num - (validationWarningCount + removedItemCount);
			}
		}

		public int PendingCount
		{
			get
			{
				if (string.IsNullOrEmpty(this.BatchInputId))
				{
					return 0;
				}
				int num = this.TotalRowCount;
				int validationWarningCount = this.ValidationWarningCount;
				int totalItemCount = this.TotalItemCount;
				if (num < totalItemCount + validationWarningCount)
				{
					return 0;
				}
				return num - (totalItemCount + validationWarningCount);
			}
		}

		public int TotalRowCount
		{
			get
			{
				return this.totalRowCount;
			}
			private set
			{
				this.totalRowCount = value;
			}
		}

		public int TotalItemCount
		{
			get
			{
				return this.cachedItemCounts.GetCachedStatusCount(MigrationJob.AllJobItemsStatuses);
			}
		}

		public int SyncedItemCount
		{
			get
			{
				return this.cachedItemCounts.GetCachedStatusCount(MigrationJob.JobItemsStatusForSynced);
			}
		}

		public int FinalizedItemCount
		{
			get
			{
				return this.cachedItemCounts.GetCachedStatusCount(MigrationJob.JobItemsStatusForFinalized);
			}
		}

		public int ActiveItemCount
		{
			get
			{
				return this.cachedItemCounts.GetCachedStatusCount(MigrationJob.JobItemsStatusForActive);
			}
		}

		public int ActiveInitialItemCount
		{
			get
			{
				return this.cachedItemCounts.GetCachedStatusCount(MigrationJob.JobItemsStatusForActiveInitial);
			}
		}

		public int StartingItemCount
		{
			get
			{
				return this.cachedItemCounts.GetCachedStatusCount(new MigrationUserStatus[]
				{
					MigrationUserStatus.Starting
				});
			}
		}

		public int StoppedItemCount
		{
			get
			{
				return this.cachedItemCounts.GetCachedStatusCount(MigrationJob.JobItemsStatusForStopped);
			}
		}

		public int FailedInitialItemCount
		{
			get
			{
				return this.cachedItemCounts.GetCachedStatusCount(MigrationJob.JobItemsStatusForFailedInitial);
			}
		}

		public int FailedIncrementalItemCount
		{
			get
			{
				return this.cachedItemCounts.GetCachedStatusCount(MigrationJob.JobItemsStatusForFailedIncremental);
			}
		}

		public int FailedOtherItemCount
		{
			get
			{
				return this.cachedItemCounts.GetCachedStatusCount(MigrationJob.JobItemsStatusForFailedOther);
			}
		}

		public int FailedFinalizationItemCount
		{
			get
			{
				return this.cachedItemCounts.GetCachedStatusCount(MigrationJob.JobItemsStatusForFailedFinalization);
			}
		}

		public int FailedItemCount
		{
			get
			{
				return this.FailedInitialItemCount + this.FailedIncrementalItemCount + this.FailedOtherItemCount + this.FailedFinalizationItemCount;
			}
		}

		public int ProvisionedItemCount
		{
			get
			{
				return this.cachedItemCounts.GetCachedOtherCount("Provisioned");
			}
		}

		public int RemovedItemCount
		{
			get
			{
				return this.cachedItemCounts.GetCachedOtherCount("Removed");
			}
		}

		public int ReportSyncCompleteFailedItemCount
		{
			get
			{
				return this.FailedItemCount;
			}
		}

		public int ReportCompleteFailedItemCount
		{
			get
			{
				return this.FailedIncrementalItemCount + this.FailedFinalizationItemCount;
			}
		}

		public bool ShouldAutoRetryStartedJob
		{
			get
			{
				return this.ShouldAutoRetryJob(this.ReportSyncCompleteFailedItemCount);
			}
		}

		public bool ShouldAutoRetryCompletedJob
		{
			get
			{
				return this.ShouldAutoRetryJob(this.ReportCompleteFailedItemCount);
			}
		}

		public ExDateTime? LastSyncTime
		{
			get
			{
				return this.cachedItemCounts.GetCachedTimestamp("LastSync");
			}
		}

		public bool HasCachedCounts
		{
			get
			{
				return !this.cachedItemCounts.IsEmpty;
			}
		}

		public ExDateTime? FullScanTime
		{
			get
			{
				return this.fullScanTime;
			}
			private set
			{
				this.fullScanTime = value;
			}
		}

		public bool ShouldLazyRescan
		{
			get
			{
				TimeSpan config = ConfigBase<MigrationServiceConfigSchema>.GetConfig<TimeSpan>("SyncMigrationLazyCountRescanPollingTimeout");
				return !(config < TimeSpan.Zero) && (this.fullScanTime == null || ExDateTime.UtcNow > this.fullScanTime.Value + config);
			}
		}

		public MultiValuedProperty<SmtpAddress> NotificationEmails
		{
			get
			{
				return this.notificationEmails;
			}
			private set
			{
				this.notificationEmails = value;
			}
		}

		public bool IsCancelled
		{
			get
			{
				return this.JobCancellationStatus != JobCancellationStatus.NotCancelled;
			}
		}

		public JobCancellationStatus JobCancellationStatus
		{
			get
			{
				return this.jobCancellationStatus;
			}
			private set
			{
				this.jobCancellationStatus = value;
			}
		}

		public string LastCursorPosition
		{
			get
			{
				return this.lastCursorPosition;
			}
			private set
			{
				this.lastCursorPosition = value;
			}
		}

		public ExDateTime? StateLastUpdated
		{
			get
			{
				if (this.statusData == null)
				{
					return null;
				}
				return this.statusData.StateLastUpdated;
			}
		}

		public CultureInfo AdminCulture
		{
			get
			{
				return this.adminCulture;
			}
			private set
			{
				this.adminCulture = value;
			}
		}

		public bool StatisticsEnabled
		{
			get
			{
				return this.statisticsEnabled;
			}
			private set
			{
				this.statisticsEnabled = value;
			}
		}

		public ADObjectId OwnerId
		{
			get
			{
				return this.ownerId;
			}
			private set
			{
				this.ownerId = value;
			}
		}

		public Guid OwnerExchangeObjectId
		{
			get
			{
				return this.ownerExchangeObjectId;
			}
			private set
			{
				this.ownerExchangeObjectId = value;
			}
		}

		public DelegatedPrincipal DelegatedAdminOwner
		{
			get
			{
				if (this.delegatedAdminOwner == null && this.OwnerId == null && this.OwnerExchangeObjectId == Guid.Empty)
				{
					DelegatedPrincipal.TryParseDelegatedString(this.DelegatedAdminOwnerId, out this.delegatedAdminOwner);
				}
				return this.delegatedAdminOwner;
			}
		}

		public string DelegatedAdminOwnerId { get; private set; }

		public bool UseAdvancedValidation
		{
			get
			{
				return this.GetBatchFlags(MigrationBatchFlags.UseAdvancedValidation);
			}
		}

		public bool AutoComplete
		{
			get
			{
				return this.GetBatchFlags(MigrationBatchFlags.AutoComplete);
			}
		}

		public bool DisallowExistingUsers
		{
			get
			{
				return this.GetBatchFlags(MigrationBatchFlags.DisallowExistingUsers);
			}
			set
			{
				this.SetBatchFlags(MigrationBatchFlags.DisallowExistingUsers, value);
			}
		}

		public bool AutoStop
		{
			get
			{
				return this.GetBatchFlags(MigrationBatchFlags.AutoStop);
			}
			private set
			{
				this.SetBatchFlags(MigrationBatchFlags.AutoStop, value);
			}
		}

		public bool SkipSettingTargetAddress
		{
			get
			{
				return this.GetShouldSkip(SkippableMigrationSteps.SettingTargetAddress);
			}
		}

		public bool IsProvisioningSupported
		{
			get
			{
				return MigrationJob.MigrationTypeSupportsProvisioning(this.MigrationType);
			}
		}

		public bool UpdateSourceOnFinalization
		{
			get
			{
				return this.MigrationType == MigrationType.ExchangeOutlookAnywhere;
			}
		}

		public SubmittedByUserAdminType SubmittedByUserAdminType
		{
			get
			{
				return this.submittedByUserAdminType;
			}
			private set
			{
				this.submittedByUserAdminType = value;
			}
		}

		public int PoisonCount { get; private set; }

		public override PropertyDefinition[] PropertyDefinitions
		{
			get
			{
				string key = this.MigrationType.ToString() + (this.IsPAW ? "PAW" : "non-PAW");
				PropertyDefinition[] array;
				if (!MigrationJob.PropertyDefinitionsHash.TryGetValue(key, out array))
				{
					array = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
					{
						MigrationJob.MigrationJobPropertyDefinition,
						this.SubscriptionSettingsPropertyDefinitions
					});
					MigrationJob.PropertyDefinitionsHash[key] = array;
				}
				return array;
			}
		}

		public MultiValuedProperty<MigrationReportSet> Reports { get; private set; }

		public bool IsStaged
		{
			get
			{
				return this.isStaged;
			}
			private set
			{
				this.isStaged = value;
			}
		}

		public bool SupportsIncrementalSyncs
		{
			get
			{
				return !this.IsStaged || this.MigrationType != MigrationType.ExchangeOutlookAnywhere;
			}
		}

		public bool SupportsRichRecipientType
		{
			get
			{
				return this.MigrationType == MigrationType.ExchangeOutlookAnywhere;
			}
		}

		public bool SupportsMultiBatchFinalization
		{
			get
			{
				return this.MigrationType == MigrationType.ExchangeLocalMove || this.MigrationType == MigrationType.ExchangeRemoteMove || this.MigrationType == MigrationType.PSTImport || (this.MigrationType == MigrationType.IMAP && this.IsPAW) || this.MigrationType == MigrationType.XO1 || this.MigrationType == MigrationType.PublicFolder;
			}
		}

		public bool SupportsSyncTimeouts
		{
			get
			{
				return this.MigrationType != MigrationType.ExchangeLocalMove;
			}
		}

		public override PropertyDefinition[] InitializationPropertyDefinitions
		{
			get
			{
				return MigrationJob.MigrationJobTypeDefinition;
			}
		}

		public override long MaximumSupportedVersion
		{
			get
			{
				return 5L;
			}
		}

		public override long MinimumSupportedVersion
		{
			get
			{
				return 4L;
			}
		}

		public override long MinimumSupportedPersistableVersion
		{
			get
			{
				return 4L;
			}
		}

		public override long CurrentSupportedVersion
		{
			get
			{
				return this.currentSupportedVersion;
			}
		}

		public bool IsPAW
		{
			get
			{
				return base.Version >= 5L;
			}
		}

		public bool ShouldReport
		{
			get
			{
				if (this.Flags.HasFlag(MigrationFlags.Report))
				{
					return true;
				}
				if (this.ReportInterval == TimeSpan.Zero)
				{
					return false;
				}
				DateTime t = DateTime.UtcNow - this.ReportInterval;
				DateTime dateTime = (DateTime)this.OriginalCreationTime;
				if (this.StartTime != null && this.GetBatchFlags(MigrationBatchFlags.ReportInitial))
				{
					dateTime = (DateTime)this.StartTime.Value;
				}
				foreach (MigrationReportSet migrationReportSet in this.Reports)
				{
					if (migrationReportSet.CreationTimeUTC > dateTime)
					{
						dateTime = migrationReportSet.CreationTimeUTC;
					}
				}
				return dateTime < t;
			}
		}

		public string TenantName { get; private set; }

		public MigrationBatchFlags BatchFlags
		{
			get
			{
				return base.ExtendedProperties.Get<MigrationBatchFlags>("MigrationBatchFlags", MigrationBatchFlags.None);
			}
			private set
			{
				base.ExtendedProperties.Set<MigrationBatchFlags>("MigrationBatchFlags", value);
			}
		}

		public TimeSpan? IncrementalSyncInterval
		{
			get
			{
				return new TimeSpan?(base.ExtendedProperties.Get<TimeSpan>("IncrementalSyncInterval", ConfigBase<MigrationServiceConfigSchema>.GetConfig<TimeSpan>("SyncMigrationPollingTimeout")));
			}
			private set
			{
				TimeSpan? timeSpan = value;
				if (timeSpan != null)
				{
					base.ExtendedProperties.Set<TimeSpan>("IncrementalSyncInterval", timeSpan.Value);
					return;
				}
				base.ExtendedProperties.Remove("IncrementalSyncInterval");
			}
		}

		public TimeSpan ReportInterval
		{
			get
			{
				return base.ExtendedProperties.Get<TimeSpan>("ReportInterval", ConfigBase<MigrationServiceConfigSchema>.GetConfig<TimeSpan>("ReportInterval"));
			}
			private set
			{
				TimeSpan? timeSpan = new TimeSpan?(value);
				if (timeSpan != null)
				{
					base.ExtendedProperties.Set<TimeSpan>("ReportInterval", timeSpan.Value);
					return;
				}
				base.ExtendedProperties.Remove("ReportInterval");
			}
		}

		public ExDateTime? BatchLastUpdated
		{
			get
			{
				return base.ExtendedProperties.Get<ExDateTime?>("BatchLastUpdated", null);
			}
			private set
			{
				base.ExtendedProperties.Set<ExDateTime?>("BatchLastUpdated", value);
			}
		}

		public string BatchInputId
		{
			get
			{
				return base.ExtendedProperties.Get<string>("BatchInputId", null);
			}
			private set
			{
				base.ExtendedProperties.Set<string>("BatchInputId", value);
			}
		}

		public Guid? OriginalJobId
		{
			get
			{
				return base.ExtendedProperties.Get<Guid?>("OriginalJobId", null);
			}
			private set
			{
				base.ExtendedProperties.Set<Guid?>("OriginalJobId", value);
			}
		}

		public ExDateTime? InitialSyncDateTime
		{
			get
			{
				return base.ExtendedProperties.Get<ExDateTime?>("InitialSyncDateTime", null);
			}
			private set
			{
				base.ExtendedProperties.Set<ExDateTime?>("InitialSyncDateTime", value);
			}
		}

		public TimeSpan? InitialSyncDuration
		{
			get
			{
				return base.ExtendedProperties.Get<TimeSpan?>("InitialSyncDuration", null);
			}
			private set
			{
				base.ExtendedProperties.Set<TimeSpan?>("InitialSyncDuration", value);
			}
		}

		public TimeSpan ProcessingDuration
		{
			get
			{
				return base.ExtendedProperties.Get<TimeSpan>("ProcessingDuration", TimeSpan.Zero);
			}
			private set
			{
				base.ExtendedProperties.Set<TimeSpan>("ProcessingDuration", value);
			}
		}

		public int ValidationWarningCount
		{
			get
			{
				return base.ExtendedProperties.Get<int>("ValidationWarningCount", 0);
			}
			private set
			{
				base.ExtendedProperties.Set<int>("ValidationWarningCount", value);
			}
		}

		public string TargetDomainName
		{
			get
			{
				return base.ExtendedProperties.Get<string>("TargetDomainName");
			}
			set
			{
				base.ExtendedProperties.Set<string>("TargetDomainName", value);
			}
		}

		public int? MaxAutoRunCount
		{
			get
			{
				return base.ExtendedProperties.Get<int?>("MaxAutoRunCount");
			}
			private set
			{
				base.ExtendedProperties.Set<int?>("MaxAutoRunCount", value);
			}
		}

		public int AutoRunCount
		{
			get
			{
				return base.ExtendedProperties.Get<int>("AutoRunCount", 0);
			}
			private set
			{
				base.ExtendedProperties.Set<int>("AutoRunCount", value);
			}
		}

		public bool AllowUnknownColumnsInCsv
		{
			get
			{
				return base.ExtendedProperties.Get<bool>("AllowUnknownColumnsInCsv", false);
			}
			private set
			{
				base.ExtendedProperties.Set<bool>("AllowUnknownColumnsInCsv", value);
			}
		}

		public MigrationFlags Flags { get; private set; }

		public MigrationStage Stage { get; private set; }

		public MigrationWorkflow Workflow { get; private set; }

		public MigrationBatchDirection JobDirection { get; private set; }

		public MigrationEndpointBase SourceEndpoint { get; private set; }

		public MigrationEndpointBase TargetEndpoint { get; private set; }

		public IJobSubscriptionSettings SubscriptionSettings { get; private set; }

		public SkippableMigrationSteps SkipSteps { get; private set; }

		public string TroubleshooterNotes
		{
			get
			{
				return base.ExtendedProperties.Get<string>("TroubleshooterNotes", null);
			}
			private set
			{
				base.ExtendedProperties.Set<string>("TroubleshooterNotes", value);
			}
		}

		public bool ShouldProcessDataRows
		{
			get
			{
				return !this.IsStaged || !string.IsNullOrEmpty(this.BatchInputId);
			}
		}

		public bool CompleteAfterMoveSyncCompleted
		{
			get
			{
				MoveJobSubscriptionSettings moveJobSubscriptionSettings = this.SubscriptionSettings as MoveJobSubscriptionSettings;
				return moveJobSubscriptionSettings != null && moveJobSubscriptionSettings.CompleteAfter != null && moveJobSubscriptionSettings.CompleteAfter.Value < ExDateTime.UtcNow;
			}
		}

		public bool CompleteAfterMoveSyncNotCompleted
		{
			get
			{
				MoveJobSubscriptionSettings moveJobSubscriptionSettings = this.SubscriptionSettings as MoveJobSubscriptionSettings;
				return moveJobSubscriptionSettings != null && moveJobSubscriptionSettings.CompleteAfter != null && moveJobSubscriptionSettings.CompleteAfter.Value >= ExDateTime.UtcNow;
			}
		}

		internal Guid? NotificationId
		{
			get
			{
				return base.ExtendedProperties.Get<Guid?>("AsyncNotificationId", null);
			}
			set
			{
				base.ExtendedProperties.Set<Guid?>("AsyncNotificationId", value);
			}
		}

		protected override Guid ReportGuid
		{
			get
			{
				return this.JobId;
			}
		}

		private PropertyDefinition[] SubscriptionSettingsPropertyDefinitions
		{
			get
			{
				return JobSubscriptionSettingsBase.GetPropertyDefinitions(this.MigrationType, this.IsPAW);
			}
		}

		private int MaxConcurrentMigrations { get; set; }

		public static MigrationJob Create(IMigrationDataProvider provider, IMigrationConfig config, MigrationBatch migrationBatch)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.ThrowOnNullArgument(migrationBatch, "migrationBatch");
			bool usePAW = config != null && config.IsSupported(MigrationFeature.PAW);
			MigrationType migrationType = migrationBatch.MigrationType;
			MigrationLogger.Log(MigrationEventType.Warning, "MigrationJob.Create: migrationBatch {0}", new object[]
			{
				MigrationLogger.PropertyBagToString(migrationBatch.propertyBag)
			});
			MigrationJob migrationJob = new MigrationJob(migrationType);
			migrationJob.TenantName = provider.TenantName;
			migrationJob.Initialize(migrationBatch, provider, usePAW);
			migrationJob.CreateInStore(provider, migrationBatch.CsvStream, migrationBatch.ValidationWarnings);
			MigrationLogger.Log(MigrationEventType.Warning, "MigrationJob.Create: job {0}", new object[]
			{
				migrationJob
			});
			return migrationJob;
		}

		public static MigrationJob GetSingleMigrationJob(IMigrationDataProvider provider)
		{
			return MigrationJob.GetUniqueJob(provider, null);
		}

		public static IEnumerable<MigrationJob> Get(IMigrationDataProvider provider, IMigrationConfig config)
		{
			return MigrationJob.GetJobs(provider, null);
		}

		public static IEnumerable<MigrationJob> GetByStatus(IMigrationDataProvider provider, IMigrationConfig config, MigrationJobStatus status)
		{
			return MigrationJob.GetJobs(provider, new MigrationEqualityFilter(MigrationBatchMessageSchema.MigrationUserStatus, status));
		}

		public static MigrationJob GetUniqueByName(IMigrationDataProvider provider, string jobName)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.ThrowOnNullOrEmptyArgument(jobName, "jobName");
			return MigrationJob.GetUniqueJob(provider, new MigrationEqualityFilter(MigrationBatchMessageSchema.MigrationJobName, jobName, true));
		}

		public static IEnumerable<MigrationJob> GetByName(IMigrationDataProvider provider, IMigrationConfig config, string jobName)
		{
			return MigrationJob.GetJobs(provider, new MigrationEqualityFilter(MigrationBatchMessageSchema.MigrationJobName, jobName));
		}

		public static IEnumerable<MigrationJob> GetByEndpoint(IMigrationDataProvider provider, MigrationEndpointId endpointId)
		{
			MigrationUtil.ThrowOnNullArgument(endpointId, "endpointId");
			if (endpointId.Guid == Guid.Empty || MigrationEndpointId.Any.Equals(endpointId))
			{
				throw new ArgumentException("EndpointId should be a non-empty GUID for a specific endpoint.", "endpointId");
			}
			IEnumerable<MigrationJob> jobs = MigrationJob.GetJobs(provider, new MigrationEqualityFilter(MigrationBatchMessageSchema.MigrationJobSourceEndpoint, endpointId.Guid));
			IEnumerable<MigrationJob> jobs2 = MigrationJob.GetJobs(provider, new MigrationEqualityFilter(MigrationBatchMessageSchema.MigrationJobTargetEndpoint, endpointId.Guid));
			return jobs.Union(jobs2);
		}

		public static MigrationJob GetUniqueByJobId(IMigrationDataProvider provider, Guid jobId)
		{
			return MigrationJob.GetUniqueJob(provider, new MigrationEqualityFilter(MigrationBatchMessageSchema.MigrationJobId, jobId));
		}

		public static MigrationJob GetUniqueByBatchId(IMigrationDataProvider provider, MigrationBatchId batchId)
		{
			MigrationUtil.ThrowOnNullArgument(batchId, "batchId");
			MigrationUtil.AssertOrThrow(batchId.JobId != Guid.Empty || !string.IsNullOrWhiteSpace(batchId.Name), "At least one of Name or JobId must be present on the batchId.", new object[0]);
			MigrationJob migrationJob = null;
			if (batchId.JobId != Guid.Empty)
			{
				migrationJob = MigrationJob.GetUniqueByJobId(provider, batchId.JobId);
			}
			return migrationJob ?? MigrationJob.GetUniqueByName(provider, batchId.Name);
		}

		public static MigrationBatch GetMigrationBatch(IMigrationDataProvider provider, MigrationSession session, MigrationJob job)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.ThrowOnNullArgument(job, "job");
			bool supportsMultiBatchFinalization = job.SupportsMultiBatchFinalization;
			MigrationBatch migrationBatch = new MigrationBatch();
			migrationBatch.Identity = new MigrationBatchId(job.JobName, job.JobId);
			migrationBatch.MigrationType = job.MigrationType;
			migrationBatch.BatchDirection = job.JobDirection;
			migrationBatch.BatchFlags = job.BatchFlags;
			migrationBatch.AutoRetryCount = job.MaxAutoRunCount;
			migrationBatch.CurrentRetryCount = job.AutoRunCount;
			migrationBatch.SkipSteps = job.SkipSteps;
			migrationBatch.SubmittedByUser = job.SubmittedByUser;
			migrationBatch.OwnerId = job.OwnerId;
			migrationBatch.OwnerExchangeObjectId = job.OwnerExchangeObjectId;
			migrationBatch.UserTimeZone = new ExTimeZoneValue(job.UserTimeZone);
			ExDateTime value = MigrationHelper.GetUniversalDateTime(new ExDateTime?(job.CreationTime)).Value;
			ExDateTime? universalDateTime = MigrationHelper.GetUniversalDateTime(job.StartTime);
			ExDateTime? universalDateTime2 = MigrationHelper.GetUniversalDateTime(job.FinalizeTime);
			ExDateTime? universalDateTime3 = MigrationHelper.GetUniversalDateTime(job.LastSyncTime);
			ExDateTime? universalDateTime4 = MigrationHelper.GetUniversalDateTime(job.InitialSyncDateTime);
			migrationBatch.CreationDateTimeUTC = (DateTime)value;
			migrationBatch.StartDateTimeUTC = (DateTime?)universalDateTime;
			migrationBatch.FinalizedDateTimeUTC = (DateTime?)universalDateTime2;
			migrationBatch.LastSyncedDateTimeUTC = (DateTime?)universalDateTime3;
			migrationBatch.InitialSyncDateTimeUTC = (DateTime?)universalDateTime4;
			migrationBatch.InitialSyncDuration = job.InitialSyncDuration;
			migrationBatch.CreationDateTime = (DateTime)MigrationHelper.GetLocalizedDateTime(new ExDateTime?(value), job.UserTimeZone).Value;
			migrationBatch.StartDateTime = (DateTime?)MigrationHelper.GetLocalizedDateTime(universalDateTime, job.UserTimeZone);
			migrationBatch.FinalizedDateTime = (DateTime?)MigrationHelper.GetLocalizedDateTime(universalDateTime2, job.UserTimeZone);
			migrationBatch.LastSyncedDateTime = (DateTime?)MigrationHelper.GetLocalizedDateTime(universalDateTime3, job.UserTimeZone);
			migrationBatch.InitialSyncDateTime = (DateTime?)MigrationHelper.GetLocalizedDateTime(universalDateTime4, job.UserTimeZone);
			migrationBatch.Locale = job.AdminCulture;
			migrationBatch.OriginalBatchId = job.OriginalJobId;
			migrationBatch.Report = new Report(job.ReportData);
			MigrationJob.InitializeEndpointsForMigrationBatch(job, migrationBatch);
			migrationBatch.BatchDirection = job.JobDirection;
			migrationBatch.SupportedActions = MigrationBatchSupportedActions.None;
			if (job.IsPAW)
			{
				LocalizedString? localizedString;
				if (job.SupportsFlag(MigrationFlags.Remove, out localizedString))
				{
					migrationBatch.SupportedActions |= MigrationBatchSupportedActions.Remove;
				}
				if (job.SupportsFlag(MigrationFlags.Start, out localizedString))
				{
					migrationBatch.SupportedActions |= MigrationBatchSupportedActions.Start;
				}
				if (job.SupportsFlag(MigrationFlags.Stop, out localizedString))
				{
					migrationBatch.SupportedActions |= MigrationBatchSupportedActions.Stop;
				}
			}
			else
			{
				LocalizedString? localizedString;
				if (job.SupportsStarting(out localizedString))
				{
					migrationBatch.SupportedActions |= MigrationBatchSupportedActions.Start;
				}
				if (job.SupportsSetting(out localizedString))
				{
					migrationBatch.SupportedActions |= MigrationBatchSupportedActions.Set;
				}
				if (job.SupportsAppendingUsers(out localizedString))
				{
					migrationBatch.SupportedActions |= MigrationBatchSupportedActions.Append;
				}
				if (job.SupportsStopping(out localizedString))
				{
					migrationBatch.SupportedActions |= MigrationBatchSupportedActions.Stop;
				}
				if (job.SupportsRemoving(out localizedString))
				{
					migrationBatch.SupportedActions |= MigrationBatchSupportedActions.Remove;
				}
				if (job.SupportsCompleting(out localizedString))
				{
					migrationBatch.SupportedActions |= MigrationBatchSupportedActions.Complete;
				}
			}
			if (job.IsPAW)
			{
				migrationBatch.Flags = job.Flags;
			}
			ExDateTime? exDateTime = null;
			ExDateTime? exDateTime2 = null;
			MigrationType migrationType = job.MigrationType;
			if (migrationType <= MigrationType.ExchangeOutlookAnywhere)
			{
				if (migrationType != MigrationType.IMAP)
				{
					if (migrationType == MigrationType.ExchangeOutlookAnywhere)
					{
						ExchangeJobSubscriptionSettings exchangeJobSubscriptionSettings = job.SubscriptionSettings as ExchangeJobSubscriptionSettings;
						exDateTime = ((exchangeJobSubscriptionSettings != null) ? exchangeJobSubscriptionSettings.StartAfter : null);
					}
				}
				else
				{
					IMAPPAWJobSubscriptionSettings imappawjobSubscriptionSettings = job.SubscriptionSettings as IMAPPAWJobSubscriptionSettings;
					exDateTime = ((imappawjobSubscriptionSettings != null) ? imappawjobSubscriptionSettings.StartAfter : null);
					exDateTime2 = ((imappawjobSubscriptionSettings != null) ? imappawjobSubscriptionSettings.CompleteAfter : null);
				}
			}
			else if (migrationType == MigrationType.ExchangeRemoteMove || migrationType == MigrationType.ExchangeLocalMove)
			{
				MoveJobSubscriptionSettings moveJobSubscriptionSettings = job.SubscriptionSettings as MoveJobSubscriptionSettings;
				exDateTime = ((moveJobSubscriptionSettings != null) ? moveJobSubscriptionSettings.StartAfter : null);
				exDateTime2 = ((moveJobSubscriptionSettings != null) ? moveJobSubscriptionSettings.CompleteAfter : null);
			}
			if (exDateTime != null)
			{
				if (exDateTime.Value >= ExDateTime.UtcNow)
				{
					migrationBatch.StartDateTimeUTC = null;
					migrationBatch.StartDateTime = null;
				}
				else
				{
					migrationBatch.StartDateTimeUTC = (DateTime?)exDateTime;
					migrationBatch.StartDateTime = (DateTime?)MigrationHelper.GetLocalizedDateTime(exDateTime, job.UserTimeZone);
				}
			}
			bool flag = exDateTime != null && exDateTime.Value >= ExDateTime.UtcNow;
			bool flag2 = exDateTime2 != null && exDateTime2.Value >= ExDateTime.UtcNow;
			migrationBatch.Status = MigrationBatchStatus.Failed;
			switch (job.Status)
			{
			case MigrationJobStatus.Created:
				migrationBatch.Status = MigrationBatchStatus.Created;
				break;
			case MigrationJobStatus.SyncInitializing:
			case MigrationJobStatus.SyncStarting:
			case MigrationJobStatus.SyncCompleting:
			case MigrationJobStatus.ProvisionStarting:
			case MigrationJobStatus.Validating:
				migrationBatch.Status = MigrationBatchStatus.Syncing;
				break;
			case MigrationJobStatus.SyncCompleted:
				migrationBatch.Status = MigrationBatchStatus.Synced;
				break;
			case MigrationJobStatus.CompletionInitializing:
			case MigrationJobStatus.CompletionStarting:
			case MigrationJobStatus.Completing:
				if (supportsMultiBatchFinalization || job.IsPAW)
				{
					migrationBatch.Status = MigrationBatchStatus.Completing;
				}
				else
				{
					migrationBatch.Status = MigrationBatchStatus.Removing;
				}
				break;
			case MigrationJobStatus.Completed:
				if (supportsMultiBatchFinalization || job.IsPAW)
				{
					migrationBatch.Status = MigrationBatchStatus.Completed;
				}
				else
				{
					migrationBatch.Status = MigrationBatchStatus.Removing;
				}
				break;
			case MigrationJobStatus.Failed:
				migrationBatch.Status = MigrationBatchStatus.Failed;
				break;
			case MigrationJobStatus.Removing:
				migrationBatch.Status = MigrationBatchStatus.Removing;
				break;
			case MigrationJobStatus.Stopped:
				migrationBatch.Status = MigrationBatchStatus.Stopped;
				break;
			case MigrationJobStatus.Corrupted:
				migrationBatch.Status = MigrationBatchStatus.Corrupted;
				break;
			}
			if (job.IsPAW)
			{
				if (job.Flags.HasFlag(MigrationFlags.Start))
				{
					migrationBatch.Status = MigrationBatchStatus.Starting;
				}
				if (job.Flags.HasFlag(MigrationFlags.Stop))
				{
					migrationBatch.Status = MigrationBatchStatus.Stopping;
				}
				if (job.Flags.HasFlag(MigrationFlags.Remove))
				{
					migrationBatch.Status = MigrationBatchStatus.Removing;
				}
			}
			else if (job.IsCancelled && (MigrationJobStage.Sync.IsStatusSupported(job.Status) || MigrationJobStage.Completion.IsStatusSupported(job.Status) || MigrationJobStage.Incremental.IsStatusSupported(job.Status)))
			{
				migrationBatch.Status = MigrationBatchStatus.Stopping;
			}
			if (flag && (migrationBatch.Status == MigrationBatchStatus.Completing || migrationBatch.Status == MigrationBatchStatus.Syncing))
			{
				migrationBatch.Status = MigrationBatchStatus.Waiting;
			}
			else if (migrationBatch.Status == MigrationBatchStatus.Completing && job.AutoComplete)
			{
				migrationBatch.Status = MigrationBatchStatus.Syncing;
			}
			if (job.IsPAW)
			{
				if (migrationBatch.Status == MigrationBatchStatus.Syncing && job.Stage == MigrationStage.Processing && job.ActiveItemCount == 0 && (job.SyncedItemCount > 0 || job.FailedItemCount > 0) && job.PendingCount == 0 && (flag2 || !supportsMultiBatchFinalization))
				{
					migrationBatch.Status = MigrationBatchStatus.Synced;
					if (supportsMultiBatchFinalization)
					{
						migrationBatch.SupportedActions |= MigrationBatchSupportedActions.Complete;
					}
				}
				if (migrationBatch.Status == MigrationBatchStatus.Completed && (flag2 || !supportsMultiBatchFinalization))
				{
					migrationBatch.Status = MigrationBatchStatus.Synced;
				}
			}
			if (job.FailedItemCount > 0)
			{
				MigrationBatchStatus migrationBatchStatus = migrationBatch.Status;
				if (migrationBatchStatus != MigrationBatchStatus.Completed)
				{
					if (migrationBatchStatus == MigrationBatchStatus.Synced)
					{
						migrationBatch.Status = MigrationBatchStatus.SyncedWithErrors;
					}
				}
				else
				{
					migrationBatch.Status = MigrationBatchStatus.CompletedWithErrors;
				}
			}
			if (job.IsProvisioningSupported && job.Status == MigrationJobStatus.ProvisionStarting)
			{
				migrationBatch.IsProvisioning = true;
			}
			if (job.StatusData.LocalizedError != null)
			{
				migrationBatch.Message = job.StatusData.LocalizedError.Value;
			}
			if (!job.IsPAW)
			{
				migrationBatch.ValidationWarnings = MigrationHelper.ToMultiValuedProperty<MigrationBatchError>(from warning in job.GetValidationWarnings(provider)
				orderby warning.RowIndex
				select warning);
				migrationBatch.ValidationWarningCount = job.ValidationWarningCount;
			}
			if (job.IsPAW && job.HasCachedCounts && job.Stage == MigrationStage.Processing)
			{
				migrationBatch.TotalCount = job.TotalItemCount;
			}
			else
			{
				migrationBatch.TotalCount = job.TotalCount;
			}
			migrationBatch.ActiveCount = job.ActiveItemCount;
			migrationBatch.SyncedCount = job.SyncedItemCount;
			migrationBatch.FinalizedCount = job.FinalizedItemCount;
			migrationBatch.StoppedCount = job.StoppedItemCount;
			migrationBatch.FailedCount = job.FailedItemCount;
			migrationBatch.FailedInitialSyncCount = job.FailedInitialItemCount;
			migrationBatch.FailedIncrementalSyncCount = job.FailedIncrementalItemCount;
			migrationBatch.PendingCount = job.PendingCount;
			migrationBatch.ProvisionedCount = job.ProvisionedItemCount;
			migrationBatch.NotificationEmails = job.NotificationEmails;
			migrationBatch.Reports = job.Reports;
			IJobSubscriptionSettings subscriptionSettings = job.SubscriptionSettings;
			if (subscriptionSettings != null)
			{
				subscriptionSettings.WriteToBatch(migrationBatch);
				migrationBatch.SubscriptionSettingsModified = (DateTime)MigrationHelper.GetUniversalDateTime(new ExDateTime?(subscriptionSettings.LastModifiedTime)).Value;
			}
			return migrationBatch;
		}

		public static bool TryLoad(IMigrationDataProvider provider, StoreObjectId messageId, out MigrationJob migrationJob)
		{
			migrationJob = new MigrationJob();
			try
			{
				if (!migrationJob.TryLoad(provider, messageId))
				{
					migrationJob = null;
				}
			}
			catch (MigrationDataCorruptionException ex)
			{
				MigrationLogger.Log(MigrationEventType.Error, "Tried to get migration job but failed. marking job corrupt. {0}", new object[]
				{
					ex
				});
				if (migrationJob.Status != MigrationJobStatus.Failed)
				{
					MigrationStatusData<MigrationJobStatus> migrationStatusData;
					if (migrationJob.StatusData != null)
					{
						migrationStatusData = new MigrationStatusData<MigrationJobStatus>(migrationJob.StatusData);
					}
					else
					{
						migrationStatusData = new MigrationStatusData<MigrationJobStatus>(MigrationJob.StatusDataVersionMap[migrationJob.Version]);
					}
					migrationStatusData.SetFailedStatus(MigrationJobStatus.Failed, ex, "MigrationJob::TryLoad", null);
					migrationJob.StatusData = migrationStatusData;
					migrationJob.LogStatusEvent();
					MigrationFailureLog.LogFailureEvent(migrationJob, ex, MigrationFailureFlags.Corruption, null);
				}
			}
			catch (ObjectNotFoundException ex2)
			{
				MigrationLogger.Log(MigrationEventType.Warning, "Failed to load a migration job.  {0}", new object[]
				{
					ex2
				});
				migrationJob = null;
			}
			if (migrationJob != null)
			{
				migrationJob.TenantName = provider.TenantName;
			}
			return migrationJob != null;
		}

		public static void DeleteAll(IMigrationDataProvider provider)
		{
			MigrationLogger.Log(MigrationEventType.Warning, "MigrationJob.DeleteAll", new object[0]);
			IEnumerable<StoreObjectId> enumerable = MigrationHelper.FindMessageIds(provider, MigrationJob.MessageClassEqualityFilter, null, null, null);
			enumerable = new List<StoreObjectId>(enumerable);
			foreach (StoreObjectId messageId in enumerable)
			{
				provider.RemoveMessage(messageId);
			}
		}

		public int GetItemCount(params MigrationUserStatus[] statuses)
		{
			return this.cachedItemCounts.GetCachedStatusCount(statuses ?? (this.IsPAW ? MigrationJob.AllUsedPAWJobItemStatuses : MigrationJob.AllJobItemsStatuses));
		}

		public bool SupportsStarting(out LocalizedString? errorMsg)
		{
			errorMsg = null;
			if (MigrationJobStage.Corrupted.IsStatusSupported(this.Status))
			{
				errorMsg = new LocalizedString?(Strings.CorruptedMigrationBatchCannotBeStarted);
				return false;
			}
			if (this.Status == MigrationJobStatus.Removed)
			{
				errorMsg = new LocalizedString?(Strings.RemovedMigrationJobCannotBeStarted);
				return false;
			}
			if (MigrationJobStage.Sync.IsStatusSupported(this.Status))
			{
				errorMsg = new LocalizedString?(this.IsCancelled ? Strings.StoppingMigrationJobCannotBeStarted : Strings.MigrationJobAlreadyStarted);
				return false;
			}
			if (MigrationJobStage.Completion.IsStatusSupported(this.Status))
			{
				errorMsg = new LocalizedString?(this.SupportsMultiBatchFinalization ? Strings.CompletingMigrationJobCannotBeStarted : Strings.RemovedMigrationJobCannotBeStarted);
				return false;
			}
			if (!this.AutoComplete && MigrationJobStage.Completed.IsStatusSupported(this.Status))
			{
				errorMsg = new LocalizedString?(Strings.CompletedMigrationJobCannotBeStartedMultiBatch);
				return false;
			}
			if (!MigrationJobStage.Incremental.IsStatusSupported(this.Status) && !MigrationJobStage.Completed.IsStatusSupported(this.Status))
			{
				return MigrationJobStage.Dormant.IsStatusSupported(this.Status);
			}
			MigrationUtil.AssertOrThrow(!MigrationJobStage.Completed.IsStatusSupported(this.Status) || this.AutoComplete, "expect either NOT completed or if completed that autocomplete is set", new object[0]);
			if (this.IsCancelled)
			{
				errorMsg = new LocalizedString?(Strings.StoppingMigrationJobCannotBeStarted);
				return false;
			}
			return this.SupportsRestarting(out errorMsg);
		}

		public bool SupportsStopping(out LocalizedString? errorMsg)
		{
			errorMsg = null;
			if (MigrationJobStage.Corrupted.IsStatusSupported(this.Status))
			{
				errorMsg = new LocalizedString?(Strings.CorruptedMigrationBatchCannotBeStopped);
				return false;
			}
			if (MigrationJobStage.Incremental.IsStatusSupported(this.Status) && this.MigrationType == MigrationType.IMAP)
			{
				errorMsg = new LocalizedString?(Strings.MigrationJobCannotBeStopped);
				return false;
			}
			if (this.Status == MigrationJobStatus.Stopped)
			{
				errorMsg = new LocalizedString?(Strings.MigrationJobAlreadyStopped);
				return false;
			}
			if (this.IsCancelled)
			{
				errorMsg = new LocalizedString?(Strings.MigrationJobAlreadyStopping);
				return false;
			}
			if (!MigrationJobStage.Incremental.IsStatusSupported(this.Status) && !MigrationJobStage.Sync.IsStatusSupported(this.Status))
			{
				errorMsg = new LocalizedString?(Strings.MigrationJobCannotBeStopped);
				return false;
			}
			return EnumValidator.IsValidValue<MigrationJobStatus>(this.Status);
		}

		public bool SupportsSetting(out LocalizedString? errorMsg)
		{
			errorMsg = null;
			MigrationJobStatus migrationJobStatus = this.Status;
			switch (migrationJobStatus)
			{
			case MigrationJobStatus.Completed:
				if (this.FailedItemCount > 0)
				{
					return true;
				}
				errorMsg = new LocalizedString?(Strings.CompletedMigrationJobCannotBeModified);
				return false;
			case MigrationJobStatus.Failed:
				break;
			case MigrationJobStatus.Removed:
				errorMsg = new LocalizedString?(Strings.RemovedMigrationJobCannotBeModified);
				return false;
			case MigrationJobStatus.Removing:
				errorMsg = new LocalizedString?(Strings.CompletedMigrationJobCannotBeModified);
				return false;
			default:
				if (migrationJobStatus == MigrationJobStatus.Corrupted)
				{
					errorMsg = new LocalizedString?(Strings.CorruptedMigrationBatchCannotBeModified);
					return false;
				}
				break;
			}
			return EnumValidator.IsValidValue<MigrationJobStatus>(this.Status);
		}

		public bool SupportsAppendingUsers(out LocalizedString? errorMsg)
		{
			errorMsg = null;
			if (!this.IsStaged || this.MigrationType == MigrationType.PublicFolder)
			{
				errorMsg = new LocalizedString?(Strings.MigrationJobDoesNotSupportAppendingUserCSV);
				return false;
			}
			if (MigrationJobStage.Corrupted.IsStatusSupported(this.Status))
			{
				errorMsg = new LocalizedString?(Strings.CorruptedMigrationBatchCannotBeModified);
				return false;
			}
			if (this.BatchInputId != null)
			{
				errorMsg = new LocalizedString?(Strings.MigrationJobAlreadyHasPendingCSV);
				return false;
			}
			if (MigrationJobStage.Completed.IsStatusSupported(this.Status))
			{
				errorMsg = new LocalizedString?(Strings.CompletedMigrationJobCannotBeModified);
				return false;
			}
			if (this.Status == MigrationJobStatus.Removed)
			{
				errorMsg = new LocalizedString?(Strings.RemovedMigrationJobCannotBeModified);
				return false;
			}
			if (MigrationJobStage.Completion.IsStatusSupported(this.Status))
			{
				errorMsg = new LocalizedString?(Strings.CompletingMigrationJobCannotBeAppendedTo);
				return false;
			}
			if (MigrationJobStage.Sync.IsStatusSupported(this.Status))
			{
				errorMsg = new LocalizedString?(Strings.SyncingMigrationJobCannotBeAppendedTo);
				return false;
			}
			return this.Status == MigrationJobStatus.Failed || this.Status == MigrationJobStatus.SyncCompleted || this.Status == MigrationJobStatus.Stopped;
		}

		public bool SupportsRemovingUsers(out LocalizedString? errorMsg)
		{
			errorMsg = null;
			if (this.migrationJobType != MigrationType.PublicFolder && (MigrationJobStage.Dormant.IsStatusSupported(this.Status) || MigrationJobStage.Incremental.IsStatusSupported(this.Status) || MigrationJobStage.Corrupted.IsStatusSupported(this.Status) || MigrationJobStage.Completed.IsStatusSupported(this.Status)))
			{
				return true;
			}
			errorMsg = new LocalizedString?(Strings.RemovingMigrationUserBatchMustBeIdle);
			return false;
		}

		public bool SupportsCompleting(out LocalizedString? errorMsg)
		{
			errorMsg = null;
			if (MigrationJobStage.Corrupted.IsStatusSupported(this.Status))
			{
				errorMsg = new LocalizedString?(Strings.CorruptedMigrationBatchCannotBeCompleted);
				return false;
			}
			if (!this.SupportsMultiBatchFinalization)
			{
				errorMsg = new LocalizedString?(Strings.CompleteMigrationBatchNotSupported);
				return false;
			}
			bool flag = MigrationJobStage.Completed.IsStatusSupported(this.Status);
			if (this.Status != MigrationJobStatus.SyncCompleted && !flag)
			{
				errorMsg = new LocalizedString?(Strings.MigrationJobCannotBeCompleted);
				return false;
			}
			if (flag && this.FailedFinalizationItemCount == 0)
			{
				errorMsg = new LocalizedString?(Strings.MigrationJobCannotRetryCompletion);
				return false;
			}
			if (this.MigrationType == MigrationType.PublicFolder)
			{
				int cachedStatusCount = this.cachedItemCounts.GetCachedStatusCount(MigrationJobItem.PreventPublicFolderCompletionErrorStatuses);
				if (cachedStatusCount > 0)
				{
					errorMsg = new LocalizedString?(Strings.PublicFolderMigrationBatchCannotBeCompletedWithErrors);
					return false;
				}
			}
			return true;
		}

		public bool SupportsRemoving(out LocalizedString? errorMsg)
		{
			errorMsg = null;
			if (!MigrationJobStage.Incremental.IsStatusSupported(this.Status) && !MigrationJobStage.Completed.IsStatusSupported(this.Status) && !MigrationJobStage.Dormant.IsStatusSupported(this.Status) && !MigrationJobStage.Corrupted.IsStatusSupported(this.Status))
			{
				errorMsg = new LocalizedString?(Strings.MigrationJobCannotBeRemoved);
				return false;
			}
			return true;
		}

		public bool SupportsFlag(MigrationFlags flag, out LocalizedString? errorMsg)
		{
			errorMsg = null;
			if (flag == MigrationFlags.Remove)
			{
				if (this.Flags.HasFlag(MigrationFlags.Remove))
				{
					errorMsg = new LocalizedString?(Strings.MigrationJobCannotBeRemoved);
					return false;
				}
				return true;
			}
			else if (flag == MigrationFlags.Start)
			{
				if ((this.State == MigrationState.Active || this.State == MigrationState.Waiting) && this.FailedItemCount == 0 && this.StoppedItemCount == 0 && !this.Flags.HasFlag(MigrationFlags.Stop))
				{
					if (!this.IsStaged)
					{
						return true;
					}
					errorMsg = new LocalizedString?(Strings.MigrationJobAlreadyStarted);
					return false;
				}
				else
				{
					if (this.State == MigrationState.Completed && this.FailedItemCount == 0 && this.StoppedItemCount == 0 && !this.Flags.HasFlag(MigrationFlags.Stop))
					{
						errorMsg = new LocalizedString?(Strings.MigrationJobCannotRetryCompletion);
						return false;
					}
					return true;
				}
			}
			else
			{
				if (flag != MigrationFlags.Stop)
				{
					return true;
				}
				if (this.State == MigrationState.Stopped && !this.Flags.HasFlag(MigrationFlags.Start))
				{
					errorMsg = new LocalizedString?(Strings.MigrationJobAlreadyStopped);
					return false;
				}
				if (this.Flags.HasFlag(MigrationFlags.Stop))
				{
					errorMsg = new LocalizedString?(Strings.MigrationJobAlreadyStopping);
					return false;
				}
				if (this.State == MigrationState.Corrupted)
				{
					errorMsg = new LocalizedString?(Strings.CorruptedMigrationBatchCannotBeStopped);
					return false;
				}
				if ((this.State == MigrationState.Completed && !this.Flags.HasFlag(MigrationFlags.Start)) || this.State == MigrationState.Failed)
				{
					errorMsg = new LocalizedString?(Strings.MigrationJobCannotBeStopped);
					return false;
				}
				return true;
			}
		}

		public void ClearTransientErrorCount(IMigrationDataProvider provider)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationLogger.Log(MigrationEventType.Information, "MigrationJob.ClearTransientError: job {0}", new object[]
			{
				this
			});
			MigrationStatusData<MigrationJobStatus> migrationStatusData = new MigrationStatusData<MigrationJobStatus>(this.StatusData);
			migrationStatusData.ClearTransientErrorCount();
			this.SetStatusData(provider, migrationStatusData, false);
		}

		public void SetLastScheduled(IMigrationDataProvider provider, ExDateTime lastScheduled)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationLogger.Log(MigrationEventType.Verbose, "MigrationJob.SetLastScheduled: job {0} time {1}", new object[]
			{
				this,
				lastScheduled
			});
			using (IMigrationStoreObject migrationStoreObject = this.FindStoreObject(provider, base.StoreObjectId, MigrationJob.JobLastScheduledPropertyDefinition))
			{
				migrationStoreObject.OpenAsReadWrite();
				MigrationHelperBase.SetExDateTimeProperty(migrationStoreObject, MigrationBatchMessageSchema.MigrationJobItemSubscriptionLastChecked, new ExDateTime?(lastScheduled));
				migrationStoreObject.Save(SaveMode.ResolveConflicts);
			}
			this.LastScheduled = new ExDateTime?(lastScheduled);
		}

		public void SetMigrationFlags(IMigrationDataProvider provider, MigrationFlags flags)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationLogger.Log(MigrationEventType.Verbose, "MigrationJob.SetMigrationFlags: job {0} flags {1}", new object[]
			{
				this,
				flags
			});
			ExDateTime utcNow = ExDateTime.UtcNow;
			PropertyDefinition[] properties = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
			{
				MigrationJob.JobMigrationFlagsPropertyDefinition,
				MigrationPersistableBase.MigrationBaseDefinitions,
				new StorePropertyDefinition[]
				{
					MigrationBatchMessageSchema.MigrationJobPoisonCount,
					MigrationBatchMessageSchema.MigrationJobStartTime
				}
			});
			using (IMigrationStoreObject migrationStoreObject = this.FindStoreObject(provider, base.StoreObjectId, properties))
			{
				migrationStoreObject.OpenAsReadWrite();
				migrationStoreObject[MigrationBatchMessageSchema.MigrationFlags] = flags;
				migrationStoreObject[MigrationBatchMessageSchema.MigrationJobPoisonCount] = 0;
				if (flags.HasFlag(MigrationFlags.Start))
				{
					MigrationHelperBase.SetExDateTimeProperty(migrationStoreObject, MigrationBatchMessageSchema.MigrationJobStartTime, new ExDateTime?(utcNow));
					if (ConfigBase<MigrationServiceConfigSchema>.GetConfig<bool>("ReportInitial"))
					{
						this.SetBatchFlags(MigrationBatchFlags.ReportInitial, true);
						this.WriteExtendedPropertiesToMessageItem(migrationStoreObject);
					}
				}
				migrationStoreObject.Save(SaveMode.ResolveConflicts);
			}
			if (flags.HasFlag(MigrationFlags.Start))
			{
				this.StartTime = new ExDateTime?(utcNow);
			}
			this.Flags = flags;
			this.PoisonCount = 0;
		}

		public void SetStatus(IMigrationDataProvider provider, MigrationJobStatus status, MigrationState state, MigrationFlags? flags = null, MigrationStage? stage = null, TimeSpan? delayTime = null, LocalizedException exception = null, string lastProcessedRow = null, string batchInputId = null, MigrationCountCache.MigrationStatusChange statusChanges = null, bool clearPoison = true, MigrationBatchFlags? batchFlags = null, TimeSpan? processingDuration = null)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.AssertOrThrow(this.IsPAW, "we should only be running with a PAW job here!", new object[0]);
			string internalError = null;
			if (exception != null)
			{
				internalError = MigrationLogger.GetDiagnosticInfo(exception, null);
			}
			MigrationLogger.Log(MigrationEventType.Information, "MigrationJob.SetStatus: job {0}, status {1}", new object[]
			{
				this,
				status
			});
			MigrationStatusData<MigrationJobStatus> migrationStatusData = new MigrationStatusData<MigrationJobStatus>(this.StatusData);
			if (state == MigrationState.Failed || state == MigrationState.Corrupted)
			{
				migrationStatusData.SetFailedStatus(status, exception, internalError, new MigrationState?(state));
			}
			else if (exception != null)
			{
				migrationStatusData.SetTransientError(exception, new MigrationJobStatus?(status), new MigrationState?(state));
			}
			else
			{
				migrationStatusData.UpdateStatus(status, new MigrationState?(state));
			}
			MigrationCountCache migrationCountCache = null;
			bool flag = false;
			if (statusChanges != null)
			{
				migrationCountCache = this.cachedItemCounts.Clone();
				migrationCountCache.ApplyStatusChange(statusChanges);
				if (!migrationCountCache.IsValid)
				{
					migrationCountCache = null;
					flag = true;
					MigrationLogger.Log(MigrationEventType.Error, "MigrationJob.SetStatus: job {0}, count cache invalid!", new object[]
					{
						this
					});
				}
			}
			PropertyDefinition[] properties = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
			{
				MigrationPersistableBase.MigrationBaseDefinitions,
				MigrationStatusData<MigrationJobStatus>.StatusPropertyDefinition,
				MigrationJob.JobCountCacheDefinition,
				new PropertyDefinition[]
				{
					MigrationBatchMessageSchema.MigrationFlags,
					MigrationBatchMessageSchema.MigrationStage,
					MigrationBatchMessageSchema.MigrationNextProcessTime,
					MigrationBatchMessageSchema.MigrationJobCursorPosition,
					MigrationBatchMessageSchema.MigrationJobPoisonCount
				}
			});
			ExDateTime? exDateTime = null;
			if (delayTime != null)
			{
				exDateTime = new ExDateTime?(ExDateTime.UtcNow + delayTime.Value);
			}
			else
			{
				exDateTime = new ExDateTime?(ExDateTime.UtcNow);
			}
			if (batchFlags != null)
			{
				this.BatchFlags = batchFlags.Value;
			}
			using (IMigrationStoreObject migrationStoreObject = this.FindStoreObject(provider, base.StoreObjectId, properties))
			{
				migrationStoreObject.OpenAsReadWrite();
				migrationStatusData.WriteToMessageItem(migrationStoreObject, true);
				if (flags != null)
				{
					migrationStoreObject[MigrationBatchMessageSchema.MigrationFlags] = flags;
				}
				if (stage != null)
				{
					migrationStoreObject[MigrationBatchMessageSchema.MigrationStage] = stage.Value;
				}
				bool flag2 = string.Equals(lastProcessedRow, "EOF", StringComparison.OrdinalIgnoreCase);
				if (flag2)
				{
					this.BatchInputId = null;
				}
				if (batchInputId != null)
				{
					this.BatchInputId = batchInputId;
				}
				if (processingDuration != null)
				{
					this.ProcessingDuration += processingDuration.Value;
				}
				if (lastProcessedRow != null)
				{
					migrationStoreObject[MigrationBatchMessageSchema.MigrationJobCursorPosition] = lastProcessedRow;
				}
				if (exDateTime != null)
				{
					MigrationHelperBase.SetExDateTimeProperty(migrationStoreObject, MigrationBatchMessageSchema.MigrationNextProcessTime, new ExDateTime?(exDateTime.Value));
				}
				if (flag)
				{
					MigrationHelperBase.SetExDateTimeProperty(migrationStoreObject, MigrationBatchMessageSchema.MigrationJobCountCacheFullScanTime, null);
				}
				if (migrationCountCache != null)
				{
					migrationStoreObject[MigrationBatchMessageSchema.MigrationJobCountCache] = migrationCountCache.Serialize();
				}
				if (clearPoison)
				{
					migrationStoreObject[MigrationBatchMessageSchema.MigrationJobPoisonCount] = 0;
				}
				if (flag2 || batchInputId != null || batchFlags != null || processingDuration != null)
				{
					this.WriteExtendedPropertiesToMessageItem(migrationStoreObject);
				}
				migrationStoreObject.Save(SaveMode.NoConflictResolution);
			}
			this.StatusData = migrationStatusData;
			if (flags != null)
			{
				this.Flags = flags.Value;
			}
			if (stage != null)
			{
				this.Stage = stage.Value;
			}
			if (exDateTime != null)
			{
				this.NextProcessTime = new ExDateTime?(exDateTime.Value);
			}
			if (lastProcessedRow != null)
			{
				this.LastCursorPosition = lastProcessedRow;
			}
			if (flag)
			{
				this.FullScanTime = null;
			}
			if (migrationCountCache != null)
			{
				this.cachedItemCounts = migrationCountCache;
			}
			if (clearPoison)
			{
				this.PoisonCount = 0;
			}
			if (migrationStatusData.Status == MigrationJobStatus.Removed)
			{
				MigrationServiceFactory.Instance.GetAsyncNotificationAdapter().RemoveNotification(provider, this);
			}
			else
			{
				MigrationServiceFactory.Instance.GetAsyncNotificationAdapter().UpdateNotification(provider, this);
			}
			if (state == MigrationState.Failed || state == MigrationState.Corrupted)
			{
				base.ReportData.Append(Strings.UnknownMigrationBatchError, exception, ReportEntryFlags.Failure | ReportEntryFlags.Fatal | ReportEntryFlags.Target);
				provider.FlushReport(base.ReportData);
			}
			this.LogStatusEvent();
			if (exception != null)
			{
				MigrationFailureFlags failureFlags;
				switch (state)
				{
				case MigrationState.Failed:
					failureFlags = MigrationFailureFlags.Fatal;
					break;
				case MigrationState.Corrupted:
					failureFlags = MigrationFailureFlags.Corruption;
					break;
				default:
					failureFlags = MigrationFailureFlags.None;
					break;
				}
				MigrationFailureLog.LogFailureEvent(this, exception, failureFlags, null);
			}
		}

		public void SetNextProcessTime(IMigrationDataProvider provider, ExDateTime nextProcessTime)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationLogger.Log(MigrationEventType.Verbose, "MigrationJob.SetNextProcessTime: job {0} time {1}", new object[]
			{
				this,
				nextProcessTime
			});
			using (IMigrationStoreObject migrationStoreObject = this.FindStoreObject(provider, base.StoreObjectId, new StorePropertyDefinition[]
			{
				MigrationBatchMessageSchema.MigrationNextProcessTime
			}))
			{
				migrationStoreObject.OpenAsReadWrite();
				MigrationHelperBase.SetExDateTimeProperty(migrationStoreObject, MigrationBatchMessageSchema.MigrationNextProcessTime, new ExDateTime?(nextProcessTime));
				migrationStoreObject.Save(SaveMode.ResolveConflicts);
			}
			this.NextProcessTime = new ExDateTime?(nextProcessTime);
		}

		public void UpdateInitialSyncProperties(IMigrationDataProvider provider, TimeSpan initialSyncDuration)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationLogger.Log(MigrationEventType.Verbose, "MigrationJob.UpdateInitialSyncProperties: job {0}", new object[]
			{
				this
			});
			this.InitialSyncDateTime = new ExDateTime?(ExDateTime.UtcNow);
			this.InitialSyncDuration = new TimeSpan?(initialSyncDuration);
			using (IMigrationStoreObject migrationStoreObject = this.FindStoreObject(provider, base.StoreObjectId, MigrationPersistableBase.MigrationBaseDefinitions))
			{
				migrationStoreObject.OpenAsReadWrite();
				this.WriteExtendedPropertiesToMessageItem(migrationStoreObject);
				migrationStoreObject.Save(SaveMode.ResolveConflicts);
			}
		}

		public void SetJobStatus(IMigrationDataProvider provider, MigrationJobStatus jobStatus)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationLogger.Log(MigrationEventType.Information, "MigrationJob.SetJobStatus: job {0}, status {1}", new object[]
			{
				this,
				jobStatus
			});
			MigrationStatusData<MigrationJobStatus> migrationStatusData = new MigrationStatusData<MigrationJobStatus>(this.StatusData);
			migrationStatusData.UpdateStatus(jobStatus, null);
			this.SetStatusData(provider, migrationStatusData, true);
			this.LogStatusEvent();
		}

		public void SetFailedStatus(IMigrationDataProvider provider, Exception exception)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.ThrowOnNullArgument(exception, "exception");
			string diagnosticInfo = MigrationLogger.GetDiagnosticInfo(exception, null);
			MigrationLogger.Log(MigrationEventType.Error, "MigrationJob.SetFailedStatus: job {0}, {1}", new object[]
			{
				this,
				diagnosticInfo
			});
			MigrationStatusData<MigrationJobStatus> migrationStatusData = new MigrationStatusData<MigrationJobStatus>(this.StatusData);
			migrationStatusData.SetFailedStatus((exception is MigrationDataCorruptionException) ? MigrationJobStatus.Corrupted : MigrationJobStatus.Failed, exception, diagnosticInfo, null);
			this.SetStatusData(provider, migrationStatusData, false);
			base.ReportData.Append(Strings.UnknownMigrationBatchError, exception, ReportEntryFlags.Failure | ReportEntryFlags.Fatal | ReportEntryFlags.Target);
			provider.FlushReport(base.ReportData);
			this.LogStatusEvent();
			MigrationFailureFlags migrationFailureFlags = (exception is MigrationDataCorruptionException) ? MigrationFailureFlags.Corruption : MigrationFailureFlags.None;
			MigrationFailureLog.LogFailureEvent(this, exception, MigrationFailureFlags.Fatal | migrationFailureFlags, null);
		}

		public void SetTransientError(IMigrationDataProvider provider, Exception exception)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.ThrowOnNullArgument(exception, "exception");
			string diagnosticInfo = MigrationLogger.GetDiagnosticInfo(exception, null);
			MigrationLogger.Log(MigrationEventType.Error, "MigrationJob.SetTransientError: job {0}, {1}", new object[]
			{
				this,
				diagnosticInfo
			});
			MigrationStatusData<MigrationJobStatus> migrationStatusData = new MigrationStatusData<MigrationJobStatus>(this.StatusData);
			migrationStatusData.SetTransientError(exception, null, null);
			this.SetStatusData(provider, migrationStatusData, false);
			base.ReportData.Append(Strings.MigrationReportJobTransientError, exception, ReportEntryFlags.Failure | ReportEntryFlags.Target);
			provider.FlushReport(base.ReportData);
			MigrationFailureLog.LogFailureEvent(this, exception, MigrationFailureFlags.None, null);
		}

		public void StopJob(IMigrationDataProvider provider, IMigrationConfig config, JobCancellationStatus jobCancellationReason)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			if (jobCancellationReason == JobCancellationStatus.NotCancelled)
			{
				throw new ArgumentException(string.Format("The job cancellation reason specified cannot be {0}", jobCancellationReason));
			}
			MigrationJob.CheckOperationIsSupported(new MigrationJob.SupportsActionDelegate(this.SupportsStopping), Strings.MigrationJobCannotBeStopped);
			base.CheckVersion();
			MigrationLogger.Log(MigrationEventType.Warning, "MigrationJob.CancelJob: job {0}", new object[]
			{
				this
			});
			using (IMigrationStoreObject migrationStoreObject = this.FindStoreObject(provider, base.StoreObjectId, MigrationJob.JobCancelPropertyDefinition))
			{
				migrationStoreObject.OpenAsReadWrite();
				migrationStoreObject[MigrationBatchMessageSchema.MigrationJobCancelledFlag] = true;
				migrationStoreObject[MigrationBatchMessageSchema.MigrationJobCancellationReason] = jobCancellationReason;
				this.WriteExtendedPropertiesToMessageItem(migrationStoreObject);
				migrationStoreObject.Save(SaveMode.NoConflictResolution);
			}
			this.JobCancellationStatus = jobCancellationReason;
		}

		public void StartJob(IMigrationDataProvider provider, MultiValuedProperty<SmtpAddress> emails, MigrationBatchFlags batchFlags, TimeSpan? incrementalSyncInterval)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationJob.CheckOperationIsSupported(new MigrationJob.SupportsActionDelegate(this.SupportsStarting), Strings.CompletedMigrationJobCannotBeStarted);
			if (this.MigrationType == MigrationType.PublicFolder && !provider.ADProvider.CheckPublicFoldersLockedForMigration())
			{
				throw new PublicFolderMailboxesNotProvisionedForMigrationException();
			}
			base.CheckVersion();
			MigrationLogger.Log(MigrationEventType.Warning, "MigrationJob.StartJob: job {0}", new object[]
			{
				this
			});
			MigrationStatusData<MigrationJobStatus> migrationStatusData = new MigrationStatusData<MigrationJobStatus>(this.StatusData);
			migrationStatusData.UpdateStatus(MigrationJobStatus.SyncInitializing, null);
			this.BatchFlags = batchFlags;
			this.IncrementalSyncInterval = incrementalSyncInterval;
			this.AutoRunCount = 0;
			ExDateTime utcNow = ExDateTime.UtcNow;
			bool shouldProcessDataRows = this.ShouldProcessDataRows;
			string value = "";
			using (IMigrationMessageItem migrationMessageItem = base.FindMessageItem(provider, MigrationJob.JobStartPropertyDefinition))
			{
				migrationMessageItem.OpenAsReadWrite();
				migrationStatusData.WriteToMessageItem(migrationMessageItem, true);
				if (emails != null)
				{
					MigrationJob.SaveNotificationEmails(migrationMessageItem, emails);
				}
				MigrationHelperBase.SetExDateTimeProperty(migrationMessageItem, MigrationBatchMessageSchema.MigrationJobStartTime, new ExDateTime?(utcNow));
				MigrationHelperBase.SetExDateTimeProperty(migrationMessageItem, MigrationBatchMessageSchema.MigrationJobLastRestartTime, new ExDateTime?(utcNow));
				MigrationHelperBase.SetExDateTimeProperty(migrationMessageItem, MigrationBatchMessageSchema.MigrationNextProcessTime, new ExDateTime?(utcNow));
				if (this.AutoComplete)
				{
					MigrationHelperBase.SetExDateTimeProperty(migrationMessageItem, MigrationBatchMessageSchema.MigrationJobFinalizeTime, new ExDateTime?(utcNow));
				}
				migrationMessageItem[MigrationBatchMessageSchema.MigrationJobCancelledFlag] = false;
				migrationMessageItem[MigrationBatchMessageSchema.MigrationJobCancellationReason] = 0;
				if (shouldProcessDataRows)
				{
					migrationMessageItem[MigrationBatchMessageSchema.MigrationJobCursorPosition] = value;
					this.CreateValidationWarningAttachment(migrationMessageItem, null, true);
				}
				migrationMessageItem[MigrationBatchMessageSchema.MigrationJobPoisonCount] = 0;
				this.WriteExtendedPropertiesToMessageItem(migrationMessageItem);
				migrationMessageItem.Save(SaveMode.NoConflictResolution);
			}
			this.JobCancellationStatus = JobCancellationStatus.NotCancelled;
			this.StatusData = migrationStatusData;
			this.StartTime = new ExDateTime?(utcNow);
			this.LastRestartTime = new ExDateTime?(utcNow);
			this.NextProcessTime = new ExDateTime?(utcNow);
			if (this.AutoComplete)
			{
				this.FinalizeTime = new ExDateTime?(utcNow);
			}
			this.NotificationEmails = emails;
			this.PoisonCount = 0;
			if (shouldProcessDataRows)
			{
				this.LastCursorPosition = value;
			}
			MigrationServiceFactory.Instance.GetAsyncNotificationAdapter().UpdateNotification(provider, this);
			this.LogStatusEvent();
		}

		public void SetReportUrls(IMigrationDataProvider provider, MigrationReportSet reportSet)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			this.Reports.Add(reportSet);
			base.CheckVersion();
			using (IMigrationStoreObject migrationStoreObject = this.FindStoreObject(provider, base.StoreObjectId, MigrationJob.JobSetReportIDDefinition))
			{
				migrationStoreObject.OpenAsReadWrite();
				this.SaveReports(migrationStoreObject, this.Reports, true);
				migrationStoreObject.Save(SaveMode.NoConflictResolution);
			}
		}

		public void FinalizeJob(IMigrationDataProvider provider, MultiValuedProperty<SmtpAddress> emails)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			LocalizedString? localizedString;
			if (!this.SupportsCompleting(out localizedString) && (this.Status != MigrationJobStatus.Failed || this.FinalizeTime == null))
			{
				if (localizedString == null)
				{
					MigrationLogger.Log(MigrationEventType.Information, "Cannot complete batch '{0}' but SupportCompleting didn't return a reason.", new object[]
					{
						this
					});
					localizedString = new LocalizedString?(Strings.MigrationJobCannotBeCompleted);
				}
				throw new MigrationPermanentException(localizedString.Value);
			}
			MigrationLogger.Log(MigrationEventType.Warning, "MigrationJob.FinalizeJob: job {0}", new object[]
			{
				this
			});
			this.AutoRunCount = 0;
			int num = 0;
			ExDateTime utcNow = ExDateTime.UtcNow;
			MigrationStatusData<MigrationJobStatus> migrationStatusData = new MigrationStatusData<MigrationJobStatus>(this.StatusData);
			if (this.MigrationType == MigrationType.PublicFolder && this.StatusData.Status == MigrationJobStatus.Completed)
			{
				migrationStatusData.UpdateStatus(MigrationJobStatus.CompletionStarting, null);
			}
			else
			{
				migrationStatusData.UpdateStatus(MigrationJobStatus.CompletionInitializing, null);
			}
			using (IMigrationStoreObject migrationStoreObject = this.FindStoreObject(provider, base.StoreObjectId, MigrationJob.JobFinalizePropertyDefinition))
			{
				migrationStoreObject.OpenAsReadWrite();
				migrationStatusData.WriteToMessageItem(migrationStoreObject, true);
				if (this.MigrationType == MigrationType.PublicFolder)
				{
					num = migrationStoreObject.GetValueOrDefault<int>(MigrationBatchMessageSchema.MigrationJobLastFinalizationAttempt, 0) + 1;
					migrationStoreObject[MigrationBatchMessageSchema.MigrationJobLastFinalizationAttempt] = num;
				}
				MigrationHelperBase.SetExDateTimeProperty(migrationStoreObject, MigrationBatchMessageSchema.MigrationJobFinalizeTime, new ExDateTime?(utcNow));
				MigrationHelperBase.SetExDateTimeProperty(migrationStoreObject, MigrationBatchMessageSchema.MigrationJobItemStateLastUpdated, new ExDateTime?(utcNow));
				MigrationHelperBase.SetExDateTimeProperty(migrationStoreObject, MigrationBatchMessageSchema.MigrationJobLastRestartTime, new ExDateTime?(utcNow));
				if (emails != null)
				{
					MigrationJob.SaveNotificationEmails(migrationStoreObject, emails);
				}
				migrationStoreObject[MigrationBatchMessageSchema.MigrationJobCancelledFlag] = false;
				migrationStoreObject[MigrationBatchMessageSchema.MigrationJobCancellationReason] = 0;
				migrationStoreObject[MigrationBatchMessageSchema.MigrationJobPoisonCount] = 0;
				this.WriteExtendedPropertiesToMessageItem(migrationStoreObject);
				migrationStoreObject.Save(SaveMode.NoConflictResolution);
			}
			this.StatusData = migrationStatusData;
			this.FinalizeTime = new ExDateTime?(utcNow);
			this.LastFinalizationAttempt = num;
			this.LastRestartTime = new ExDateTime?(utcNow);
			if (emails != null)
			{
				this.NotificationEmails = emails;
			}
			MigrationServiceFactory.Instance.GetAsyncNotificationAdapter().UpdateNotification(provider, this);
			this.LogStatusEvent();
		}

		public void UpdateCachedItemCounts(IMigrationDataProvider provider)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationLogger.Log(MigrationEventType.Verbose, "MigrationJob.UpdateCachedItemCounts: job {0}", new object[]
			{
				this
			});
			ExDateTime utcNow = ExDateTime.UtcNow;
			MigrationCountCache migrationCountCache = new MigrationCountCache();
			int num = this.TotalRowCount;
			MigrationUserStatus[] array;
			if (!this.IsPAW)
			{
				num -= this.RemovedItemCount;
				array = MigrationJob.AllJobItemsStatuses;
				int provisionedCount = MigrationJobItem.GetProvisionedCount(provider, this.JobId);
				migrationCountCache.SetCachedOtherCount("Provisioned", provisionedCount);
			}
			else
			{
				array = MigrationJob.AllUsedPAWJobItemStatuses;
			}
			foreach (MigrationUserStatus migrationUserStatus in array)
			{
				int itemCount = this.GetItemCount(provider, new MigrationUserStatus[]
				{
					migrationUserStatus
				});
				migrationCountCache.SetCachedStatusCount(migrationUserStatus, itemCount);
			}
			ExDateTime? oldestLastSyncSubscriptionTime = MigrationJobItem.GetOldestLastSyncSubscriptionTime(provider, this.MigrationType, this.JobId);
			migrationCountCache.SetCachedTimestamp("LastSync", oldestLastSyncSubscriptionTime);
			this.SetCountCache(provider, new ExDateTime?(utcNow), migrationCountCache, new int?(num));
		}

		public void IncrementRemovedUserCount(IMigrationDataProvider provider)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationLogger.Log(MigrationEventType.Verbose, "MigrationJob.UpdateCachedItemCounts: job {0}", new object[]
			{
				this
			});
			MigrationCountCache migrationCountCache = this.cachedItemCounts.Clone();
			migrationCountCache.IncrementCachedOtherCount("Removed", 1);
			this.SetCountCache(provider, null, migrationCountCache, null);
		}

		public void SetCountCache(IMigrationDataProvider provider, ExDateTime? modifiedTime, MigrationCountCache newCountCache, int? totalCount)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			base.CheckVersion();
			using (IMigrationStoreObject migrationStoreObject = this.FindStoreObject(provider, base.StoreObjectId, MigrationJob.JobCountCacheDefinition))
			{
				migrationStoreObject.OpenAsReadWrite();
				migrationStoreObject[MigrationBatchMessageSchema.MigrationJobCountCache] = newCountCache.Serialize();
				MigrationHelperBase.SetExDateTimeProperty(migrationStoreObject, MigrationBatchMessageSchema.MigrationJobCountCacheFullScanTime, modifiedTime);
				if (totalCount != null)
				{
					migrationStoreObject[MigrationBatchMessageSchema.MigrationJobTotalRowCount] = totalCount.Value;
				}
				migrationStoreObject.Save(SaveMode.NoConflictResolution);
			}
			this.cachedItemCounts = newCountCache;
			this.FullScanTime = modifiedTime;
			if (totalCount != null)
			{
				this.TotalRowCount = totalCount.Value;
			}
		}

		public void SaveBatchFlagsAndNotificationId(IMigrationDataProvider provider)
		{
			this.SaveExtendedProperties(provider);
		}

		public void RemoveJob(IMigrationDataProvider provider)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationJob.CheckOperationIsSupported(new MigrationJob.SupportsActionDelegate(this.SupportsRemoving), Strings.MigrationJobCannotBeRemoved);
			MigrationLogger.Log(MigrationEventType.Information, "MigrationJob.RemoveJob: job {0}", new object[]
			{
				this
			});
			MigrationStatusData<MigrationJobStatus> migrationStatusData = new MigrationStatusData<MigrationJobStatus>(this.StatusData);
			migrationStatusData.UpdateStatus(MigrationJobStatus.Removing, null);
			using (IMigrationStoreObject migrationStoreObject = this.FindStoreObject(provider, base.StoreObjectId, this.PropertyDefinitions))
			{
				migrationStoreObject.OpenAsReadWrite();
				migrationStatusData.WriteToMessageItem(migrationStoreObject, true);
				migrationStoreObject[MigrationBatchMessageSchema.MigrationJobPoisonCount] = 0;
				migrationStoreObject.Save(SaveMode.NoConflictResolution);
			}
			this.PoisonCount = 0;
			this.StatusData = migrationStatusData;
			MigrationServiceFactory.Instance.GetAsyncNotificationAdapter().RemoveNotification(provider, this);
			this.LogStatusEvent();
		}

		public void UpdateJob(IMigrationDataProvider provider, bool updateEmails, MigrationBatch batch)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.ThrowOnNullArgument(batch, "batch");
			MigrationJob.CheckOperationIsSupported(new MigrationJob.SupportsActionDelegate(this.SupportsSetting), Strings.CompletedMigrationJobCannotBeModified);
			MigrationLogger.Log(MigrationEventType.Information, "MigrationJob.Update: overriding batch flags {0} with {1}", new object[]
			{
				this.BatchFlags,
				batch.BatchFlags
			});
			if (batch.ReportInterval != null)
			{
				this.ReportInterval = batch.ReportInterval.Value;
			}
			IJobSubscriptionSettings jobSubscriptionSettings = JobSubscriptionSettingsBase.CreateFromBatch(batch, this.IsPAW);
			this.BatchFlags = batch.BatchFlags;
			this.MaxAutoRunCount = batch.AutoRetryCount;
			this.AllowUnknownColumnsInCsv = batch.AllowUnknownColumnsInCsv;
			MultiValuedProperty<SmtpAddress> multiValuedProperty = this.NotificationEmails;
			using (IMigrationMessageItem migrationMessageItem = base.FindMessageItem(provider, this.PropertyDefinitions))
			{
				migrationMessageItem.OpenAsReadWrite();
				if (updateEmails)
				{
					multiValuedProperty = batch.NotificationEmails;
					MigrationJob.SaveNotificationEmails(migrationMessageItem, multiValuedProperty);
				}
				if (batch.CsvStream != null)
				{
					batch.CsvStream.Seek(0L, SeekOrigin.Begin);
					MigrationLogger.Log(MigrationEventType.Information, "MigrationJob.Update: job {0}, overriding CSV attachment", new object[]
					{
						this
					});
					int validationWarningCount = this.ValidationWarningCount;
					this.CreateValidationWarningAttachment(migrationMessageItem, batch.ValidationWarnings, true);
					MigrationLogger.Log(MigrationEventType.Verbose, "MigrationJob.Update: job {0}, wrote {1} warnings (removed {2} warnings)", new object[]
					{
						this,
						this.ValidationWarningCount,
						validationWarningCount
					});
					using (IMigrationAttachment migrationAttachment = migrationMessageItem.CreateAttachment("Request.csv"))
					{
						this.SaveCsvStream(migrationMessageItem, migrationAttachment, batch.CsvStream);
					}
					this.TotalRowCount = this.TotalRowCount + batch.TotalCount - validationWarningCount;
					migrationMessageItem[MigrationBatchMessageSchema.MigrationJobTotalRowCount] = this.TotalRowCount;
				}
				migrationMessageItem[MigrationBatchMessageSchema.MigrationJobPoisonCount] = 0;
				if (jobSubscriptionSettings != null)
				{
					jobSubscriptionSettings.WriteExtendedProperties(base.ExtendedProperties);
					jobSubscriptionSettings.WriteToMessageItem(migrationMessageItem, true);
				}
				this.WriteExtendedPropertiesToMessageItem(migrationMessageItem);
				migrationMessageItem.Save(SaveMode.NoConflictResolution);
			}
			this.SubscriptionSettings = jobSubscriptionSettings;
			this.PoisonCount = 0;
			this.NotificationEmails = multiValuedProperty;
		}

		public void Delete(IMigrationDataProvider provider, bool force)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.AssertOrThrow(base.StoreObjectId != null, "Should only try to delete an item that's been persisted", new object[0]);
			if (!force && !this.IsPAW)
			{
				int count = MigrationJobItem.GetCount(provider, this.JobId, new MigrationUserStatus[0]);
				if (count > 0)
				{
					string format = string.Format("MigrationJob.Delete: job {0} still has items", this);
					MigrationLogger.Log(MigrationEventType.Error, format, new object[0]);
					throw new MigrationJobCannotBeDeletedWithPendingItemsException(count);
				}
			}
			this.StatusData.UpdateStatus(MigrationJobStatus.Removed, null);
			this.LogStatusEvent();
			MigrationLogger.Log(MigrationEventType.Warning, "MigrationJob.Delete: job {0}, force {1}", new object[]
			{
				this,
				force
			});
			provider.RemoveMessage(base.StoreObjectId);
			MigrationServiceFactory.Instance.GetAsyncNotificationAdapter().RemoveNotification(provider, this);
			CommonUtils.CatchKnownExceptions(delegate
			{
				provider.DeleteReport(this.ReportData);
			}, delegate(Exception exception)
			{
				MigrationLogger.Log(MigrationEventType.Warning, exception, "Failed to remove report for job '{0}'.", new object[]
				{
					this.JobId
				});
			});
		}

		public bool IsDataRowProcessingDone()
		{
			return string.Equals(this.LastCursorPosition, "EOF", StringComparison.OrdinalIgnoreCase);
		}

		public void SetDataRowProcessingDone(IMigrationDataProvider provider, ICollection<MigrationBatchError> warnings, int updatesEncountered)
		{
			this.UpdateLastProcessedRow(provider, "EOF", warnings, updatesEncountered);
		}

		public void UpdateLastProcessedRow(IMigrationDataProvider provider, string rowIndex, ICollection<MigrationBatchError> warnings, int updatesEncountered)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			int num = this.TotalRowCount - updatesEncountered;
			base.CheckVersion();
			using (IMigrationMessageItem migrationMessageItem = base.FindMessageItem(provider, this.PropertyDefinitions))
			{
				migrationMessageItem.OpenAsReadWrite();
				bool flag = string.Equals(rowIndex, "EOF", StringComparison.OrdinalIgnoreCase);
				bool flag2 = warnings != null && warnings.Count > 0;
				if (flag)
				{
					this.BatchInputId = null;
				}
				if (flag2)
				{
					IMigrationAttachment migrationAttachment = null;
					if (migrationMessageItem.TryGetAttachment("Errors.csv", PropertyOpenMode.Modify, out migrationAttachment))
					{
						using (migrationAttachment)
						{
							long num2 = migrationAttachment.Stream.Seek(0L, SeekOrigin.End);
							using (StreamWriter streamWriter = new StreamWriter(migrationAttachment.Stream))
							{
								if (num2 != 0L)
								{
									MigrationErrorCsvSchema.WriteErrors(streamWriter, warnings);
								}
								else
								{
									MigrationErrorCsvSchema.WriteHeaderAndErrors(streamWriter, warnings);
								}
								streamWriter.Flush();
							}
							migrationAttachment.Save(null);
						}
						this.ValidationWarningCount += warnings.Count;
					}
					else
					{
						this.CreateValidationWarningAttachment(migrationMessageItem, warnings, false);
						this.ValidationWarningCount = warnings.Count;
					}
				}
				if (rowIndex != null)
				{
					migrationMessageItem[MigrationBatchMessageSchema.MigrationJobCursorPosition] = rowIndex;
				}
				migrationMessageItem[MigrationBatchMessageSchema.MigrationJobTotalRowCount] = num;
				if (flag || flag2)
				{
					this.WriteExtendedPropertiesToMessageItem(migrationMessageItem);
				}
				migrationMessageItem.Save(SaveMode.NoConflictResolution);
			}
			this.TotalRowCount = num;
			if (rowIndex != null)
			{
				this.LastCursorPosition = rowIndex;
			}
		}

		public void UpdatePoisonCount(IMigrationDataProvider provider, int count)
		{
			using (IMigrationStoreObject migrationStoreObject = this.FindStoreObject(provider, base.StoreObjectId, this.PropertyDefinitions))
			{
				migrationStoreObject.OpenAsReadWrite();
				migrationStoreObject[MigrationBatchMessageSchema.MigrationJobPoisonCount] = count;
				migrationStoreObject.Save(SaveMode.NoConflictResolution);
				this.PoisonCount = count;
			}
		}

		public bool TryAutoRetryStartedJob(IMigrationDataProvider provider)
		{
			return this.TryAutoRetryJob(provider, this.ReportSyncCompleteFailedItemCount);
		}

		public bool TryAutoRetryCompletedJob(IMigrationDataProvider provider)
		{
			return this.TryAutoRetryJob(provider, this.ReportCompleteFailedItemCount);
		}

		public IEnumerable<MigrationBatchError> GetValidationWarnings(IMigrationDataProvider provider)
		{
			IEnumerable<MigrationBatchError> result;
			try
			{
				using (IMigrationMessageItem migrationMessageItem = base.FindMessageItem(provider, this.PropertyDefinitions))
				{
					IMigrationAttachment migrationAttachment = null;
					if (migrationMessageItem.TryGetAttachment("Errors.csv", PropertyOpenMode.ReadOnly, out migrationAttachment))
					{
						MigrationUtil.AssertOrThrow(migrationAttachment != null, "attachment shouldn't be null if TryGetAttachment returns true", new object[0]);
						using (migrationAttachment)
						{
							return new List<MigrationBatchError>(MigrationErrorCsvSchema.ReadErrors(migrationAttachment.Stream));
						}
					}
					result = new List<MigrationBatchError>(0);
				}
			}
			catch (CsvValidationException ex)
			{
				result = new MigrationBatchError[]
				{
					new MigrationBatchError
					{
						EmailAddress = "ValidationWarningsAttachment",
						LocalizedErrorMessage = ex.LocalizedString
					}
				};
			}
			return result;
		}

		public override XElement GetDiagnosticInfo(IMigrationDataProvider dataProvider, MigrationDiagnosticArgument argument)
		{
			XElement xelement = new XElement("MigrationJob", new object[]
			{
				new XAttribute("name", this.JobName),
				new XElement("migrationJobType", this.MigrationType),
				new XElement("id", this.JobId)
			});
			if (this.StatusData != null)
			{
				xelement.Add(this.StatusData.GetDiagnosticInfo(dataProvider, argument));
			}
			if (this.IsPAW)
			{
				xelement.Add(new object[]
				{
					new XElement("flags", this.Flags),
					new XElement("stage", this.Stage),
					new XElement("nextProcessTime", this.NextProcessTime ?? ExDateTime.MinValue),
					new XElement("workflow", this.Workflow.Serialize(false))
				});
			}
			xelement.Add(new object[]
			{
				new XElement("messageId", base.StoreObjectId),
				new XElement("cancelled", this.IsCancelled),
				new XElement("JobCancellationStatus", this.JobCancellationStatus),
				new XElement("orginallyCreated", this.OriginalCreationTime),
				new XElement("started", this.StartTime),
				new XElement("restarted", this.LastRestartTime),
				new XElement("finalized", this.FinalizeTime),
				new XElement("isStaged", this.IsStaged),
				new XElement("poisonCount", this.PoisonCount),
				new XElement("batchFlags", this.BatchFlags),
				new XElement("totalRowCount", this.TotalRowCount),
				new XElement("cachedCounts", this.cachedItemCounts.GetDiagnosticInfo(dataProvider, argument)),
				new XElement("fullScanTime", this.FullScanTime),
				new XElement("lastProcessedRowIndex", this.LastCursorPosition),
				new XElement("submittedBy", this.SubmittedByUser),
				new XElement("timeZone", this.UserTimeZone),
				new XElement("adminCulture", this.AdminCulture),
				new XElement("notificationEmails", this.NotificationEmails),
				new XElement("direction", this.JobDirection),
				new XElement("targetDomainName", this.TargetDomainName),
				new XElement("skipSteps", this.SkipSteps)
			});
			if (this.SourceEndpoint != null)
			{
				xelement.Add(new XElement("SourceEndpoint", this.SourceEndpoint.GetDiagnosticInfo(dataProvider, argument)));
			}
			if (this.TargetEndpoint != null)
			{
				xelement.Add(new XElement("TargetEndpoint", this.TargetEndpoint.GetDiagnosticInfo(dataProvider, argument)));
			}
			if (this.SubscriptionSettings != null)
			{
				xelement.Add(new XElement("SubscriptionSettings", this.SubscriptionSettings.GetDiagnosticInfo(dataProvider, argument)));
			}
			if (this.MigrationType == MigrationType.PublicFolder)
			{
				xelement.Add(new XElement("LastFinalizationAttempt", this.LastFinalizationAttempt));
			}
			if (!this.IsPAW && dataProvider != null && argument.HasArgument("verbose"))
			{
				XElement xelement2 = new XElement("ValidationWarnings", new XElement("count", this.ValidationWarningCount));
				foreach (MigrationBatchError content in this.GetValidationWarnings(dataProvider))
				{
					xelement2.Add(new XElement("ValidationWarning", content));
				}
				xelement.Add(xelement2);
			}
			if (argument.HasArgument("reports"))
			{
				using (IMigrationDataProvider providerForFolder = dataProvider.GetProviderForFolder(MigrationFolderName.SyncMigrationReports))
				{
					foreach (MigrationReportItem migrationReportItem in MigrationReportItem.GetByJobId(providerForFolder, new Guid?(this.JobId), 50))
					{
						xelement.Add(migrationReportItem.GetDiagnosticInfo(providerForFolder, argument));
					}
				}
			}
			base.GetDiagnosticInfo(dataProvider, argument, xelement);
			return xelement;
		}

		public override string ToString()
		{
			if (this.IsPAW)
			{
				return string.Format("{0} ({1}) {2}:{3}:{4}:{5} {6}", new object[]
				{
					this.JobName,
					this.JobId,
					this.MigrationType,
					this.IsStaged ? "Staged" : "Cutover",
					base.Version,
					this.SubmittedByUser,
					this.Stage
				});
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
			{
				this.JobName,
				this.JobId,
				this.MigrationType,
				this.IsStaged ? "Staged" : "Cutover",
				base.Version,
				this.SubmittedByUser,
				this.StatusData
			});
		}

		public override void WriteToMessageItem(IMigrationStoreObject message, bool loaded)
		{
			message[StoreObjectSchema.ItemClass] = MigrationBatchMessageSchema.MigrationJobClass;
			message[MigrationBatchMessageSchema.MigrationJobId] = this.JobId;
			message[MigrationBatchMessageSchema.MigrationJobName] = this.JobName;
			message[MigrationBatchMessageSchema.MigrationJobSubmittedBy] = this.SubmittedByUser;
			message[MigrationBatchMessageSchema.MigrationJobTotalRowCount] = this.TotalRowCount;
			message[MigrationBatchMessageSchema.MigrationJobCountCache] = this.cachedItemCounts.Serialize();
			MigrationHelperBase.SetExDateTimeProperty(message, MigrationBatchMessageSchema.MigrationJobCountCacheFullScanTime, this.FullScanTime);
			message[MigrationBatchMessageSchema.MigrationJobCancelledFlag] = this.IsCancelled;
			message[MigrationBatchMessageSchema.MigrationJobCursorPosition] = this.LastCursorPosition;
			message[MigrationBatchMessageSchema.MigrationJobAdminCulture] = this.AdminCulture.ToString();
			this.StatusData.WriteToMessageItem(message, loaded);
			if (this.OriginalCreationTime == ExDateTime.MinValue)
			{
				this.OriginalCreationTime = ExDateTime.UtcNow;
			}
			MigrationHelperBase.SetExDateTimeProperty(message, MigrationBatchMessageSchema.MigrationJobOriginalCreationTime, new ExDateTime?(this.OriginalCreationTime));
			MigrationHelperBase.SetExDateTimeProperty(message, MigrationBatchMessageSchema.MigrationJobStartTime, this.StartTime);
			MigrationHelperBase.SetExDateTimeProperty(message, MigrationBatchMessageSchema.MigrationJobLastRestartTime, this.LastRestartTime);
			if (this.UserTimeZone == null)
			{
				message[MigrationBatchMessageSchema.MigrationJobUserTimeZone] = ExTimeZone.CurrentTimeZone.Id;
			}
			else
			{
				message[MigrationBatchMessageSchema.MigrationJobUserTimeZone] = this.UserTimeZone.Id;
			}
			MigrationJob.SaveNotificationEmails(message, this.NotificationEmails);
			this.SaveReports(message, this.Reports, loaded);
			MigrationHelperBase.SetExDateTimeProperty(message, MigrationBatchMessageSchema.MigrationJobFinalizeTime, this.FinalizeTime);
			message[MigrationBatchMessageSchema.MigrationType] = this.MigrationType;
			message[MigrationBatchMessageSchema.MigrationSubmittedByUserAdminType] = this.SubmittedByUserAdminType;
			message[MigrationBatchMessageSchema.MigrationJobStatisticsEnabled] = this.StatisticsEnabled;
			message[MigrationBatchMessageSchema.MigrationJobCancellationReason] = 0;
			message[MigrationBatchMessageSchema.MigrationJobIsStaged] = this.IsStaged;
			if (this.IsPAW)
			{
				message[MigrationBatchMessageSchema.MigrationFlags] = this.Flags;
				message[MigrationBatchMessageSchema.MigrationStage] = this.Stage;
				message[MigrationBatchMessageSchema.MigrationWorkflow] = this.Workflow.Serialize(false);
			}
			else
			{
				message[MigrationBatchMessageSchema.MigrationJobSuppressErrors] = false;
			}
			message[MigrationBatchMessageSchema.MigrationExchangeObjectId] = this.OwnerExchangeObjectId;
			if (this.OwnerId != null)
			{
				message[MigrationBatchMessageSchema.MigrationJobOwnerId] = this.OwnerId.GetBytes();
			}
			else
			{
				message[MigrationBatchMessageSchema.MigrationJobDelegatedAdminOwnerId] = this.DelegatedAdminOwnerId;
			}
			MigrationHelperBase.SetExDateTimeProperty(message, MigrationBatchMessageSchema.MigrationJobItemSubscriptionLastChecked, this.LastScheduled);
			MigrationHelperBase.SetExDateTimeProperty(message, MigrationBatchMessageSchema.MigrationNextProcessTime, this.NextProcessTime);
			if (this.MigrationType == MigrationType.PublicFolder)
			{
				message[MigrationBatchMessageSchema.MigrationJobLastFinalizationAttempt] = this.LastFinalizationAttempt;
			}
			if (this.SourceEndpoint != null)
			{
				message[MigrationBatchMessageSchema.MigrationJobSourceEndpoint] = this.SourceEndpoint.Guid;
			}
			if (this.TargetEndpoint != null)
			{
				message[MigrationBatchMessageSchema.MigrationJobTargetEndpoint] = this.TargetEndpoint.Guid;
			}
			message[MigrationBatchMessageSchema.MigrationJobDirection] = this.JobDirection;
			message[MigrationBatchMessageSchema.MigrationJobSkipSteps] = this.SkipSteps;
			if (this.SubscriptionSettings != null)
			{
				this.SubscriptionSettings.WriteExtendedProperties(base.ExtendedProperties);
				this.SubscriptionSettings.WriteToMessageItem(message, loaded);
			}
			base.WriteToMessageItem(message, loaded);
		}

		public void SetStatusData(IMigrationDataProvider provider, MigrationStatusData<MigrationJobStatus> newStatusData)
		{
			this.SetStatusData(provider, newStatusData, false);
		}

		public void SetTroubleshooterNotes(IMigrationDataProvider provider, string notes)
		{
			this.TroubleshooterNotes = notes;
			using (IMigrationMessageItem migrationMessageItem = base.FindMessageItem(provider, MigrationPersistableBase.MigrationBaseDefinitions))
			{
				migrationMessageItem.OpenAsReadWrite();
				this.WriteExtendedPropertiesToMessageItem(migrationMessageItem);
				migrationMessageItem.Save(SaveMode.ResolveConflicts);
			}
		}

		public override bool ReadFromMessageItem(IMigrationStoreObject message)
		{
			MigrationUtil.ThrowOnNullArgument(message, "message");
			base.ReadFromMessageItem(message);
			if (this.IsPAW)
			{
				this.Flags = MigrationHelper.GetEnumProperty<MigrationFlags>(message, MigrationBatchMessageSchema.MigrationFlags);
				this.Stage = MigrationHelper.GetEnumProperty<MigrationStage>(message, MigrationBatchMessageSchema.MigrationStage);
				string content = (string)message[MigrationBatchMessageSchema.MigrationWorkflow];
				this.Workflow = MigrationWorkflow.Deserialize(content);
			}
			MigrationHelper.VerifyMigrationTypeEquality(this.MigrationType, (MigrationType)message[MigrationBatchMessageSchema.MigrationType]);
			this.LastCursorPosition = (string)message[MigrationBatchMessageSchema.MigrationJobCursorPosition];
			this.StatisticsEnabled = (bool)message[MigrationBatchMessageSchema.MigrationJobStatisticsEnabled];
			this.ownerExchangeObjectId = MigrationHelper.GetGuidProperty(message, MigrationBatchMessageSchema.MigrationExchangeObjectId, false);
			if (!MigrationHelper.TryGetADObjectId(message, MigrationBatchMessageSchema.MigrationJobOwnerId, out this.ownerId))
			{
				this.DelegatedAdminOwnerId = message.GetValueOrDefault<string>(MigrationBatchMessageSchema.MigrationJobDelegatedAdminOwnerId, string.Empty);
			}
			this.LastFinalizationAttempt = message.GetValueOrDefault<int>(MigrationBatchMessageSchema.MigrationJobLastFinalizationAttempt, 0);
			this.FinalizeTime = MigrationHelperBase.GetExDateTimeProperty(message, MigrationBatchMessageSchema.MigrationJobFinalizeTime);
			this.SubmittedByUserAdminType = MigrationHelper.GetEnumProperty<SubmittedByUserAdminType>(message, MigrationBatchMessageSchema.MigrationSubmittedByUserAdminType);
			this.JobCancellationStatus = MigrationHelper.GetEnumProperty<JobCancellationStatus>(message, MigrationBatchMessageSchema.MigrationJobCancellationReason);
			this.StatusData = MigrationStatusData<MigrationJobStatus>.Create(message, MigrationJob.StatusDataVersionMap[base.Version]);
			this.JobId = MigrationHelper.GetGuidProperty(message, MigrationBatchMessageSchema.MigrationJobId, true);
			this.JobName = (string)message[MigrationBatchMessageSchema.MigrationJobName];
			this.SubmittedByUser = message.GetValueOrDefault<string>(MigrationBatchMessageSchema.MigrationJobSubmittedBy, string.Empty);
			this.TotalRowCount = message.GetValueOrDefault<int>(MigrationBatchMessageSchema.MigrationJobTotalRowCount, 0);
			string serializedData = (string)message[MigrationBatchMessageSchema.MigrationJobCountCache];
			this.cachedItemCounts = MigrationCountCache.Deserialize(serializedData);
			this.FullScanTime = MigrationHelperBase.GetExDateTimeProperty(message, MigrationBatchMessageSchema.MigrationJobCountCacheFullScanTime);
			this.OriginalCreationTime = MigrationHelper.GetExDateTimePropertyOrDefault(message, MigrationBatchMessageSchema.MigrationJobOriginalCreationTime, message.CreationTime);
			this.StartTime = MigrationHelperBase.GetExDateTimeProperty(message, MigrationBatchMessageSchema.MigrationJobStartTime);
			this.LastRestartTime = MigrationHelperBase.GetExDateTimeProperty(message, MigrationBatchMessageSchema.MigrationJobLastRestartTime);
			this.UserTimeZone = MigrationHelper.GetExTimeZoneProperty(message, MigrationBatchMessageSchema.MigrationJobUserTimeZone);
			this.AdminCulture = MigrationHelper.GetCultureInfoPropertyOrDefault(message, MigrationBatchMessageSchema.MigrationJobAdminCulture);
			object property = MigrationHelper.GetProperty<object>(message, MigrationBatchMessageSchema.MigrationJobMaxConcurrentMigrations, false);
			if (property != null)
			{
				this.MaxConcurrentMigrations = (int)property;
			}
			this.PoisonCount = message.GetValueOrDefault<int>(MigrationBatchMessageSchema.MigrationJobPoisonCount, 0);
			this.IsStaged = message.GetValueOrDefault<bool>(MigrationBatchMessageSchema.MigrationJobIsStaged, true);
			this.LastScheduled = MigrationHelper.GetExDateTimePropertyOrNull(message, MigrationBatchMessageSchema.MigrationJobItemSubscriptionLastChecked);
			this.NextProcessTime = MigrationHelper.GetExDateTimePropertyOrNull(message, MigrationBatchMessageSchema.MigrationNextProcessTime);
			this.Reports = MigrationJob.LoadReports(message);
			this.NotificationEmails = MigrationJob.LoadNotificationEmails(message);
			this.JobDirection = message.GetValueOrDefault<MigrationBatchDirection>(MigrationBatchMessageSchema.MigrationJobDirection, MigrationBatchDirection.Onboarding);
			this.SkipSteps = message.GetValueOrDefault<SkippableMigrationSteps>(MigrationBatchMessageSchema.MigrationJobSkipSteps, SkippableMigrationSteps.None);
			this.SubscriptionSettings = JobSubscriptionSettingsBase.CreateFromMessage(message, this.MigrationType, base.ExtendedProperties, this.IsPAW);
			return true;
		}

		public IEnumerable<MigrationJobItem> GetItemsByStatus(IMigrationDataProvider provider, MigrationUserStatus status, int? maxCount)
		{
			return MigrationJobItem.GetByStatus(provider, this, status, maxCount);
		}

		public ExDateTime GetEffectiveFinalizationTime()
		{
			if (this.FinalizeTime != null)
			{
				return this.FinalizeTime.Value;
			}
			MigrationLogger.Log(MigrationEventType.Warning, "Job {0} has no finalized time, setting to last updated time", new object[]
			{
				this
			});
			if (this.StateLastUpdated != null)
			{
				return this.StateLastUpdated.Value;
			}
			MigrationLogger.Log(MigrationEventType.Error, "At finalized state job {0} should have a StateLastUpdated value set", new object[]
			{
				this
			});
			throw new MigrationDataCorruptionException("StateLastUpdated value should be set for finalized job " + this);
		}

		internal static IEnumerable<StoreObjectId> GetIdsByState(IMigrationDataProvider provider, MigrationState state, ExDateTime? nextProcessTime = null, int? maxCount = null)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			List<QueryFilter> list = new List<QueryFilter>
			{
				new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, MigrationBatchMessageSchema.MigrationJobClass),
				new ComparisonFilter(ComparisonOperator.Equal, MigrationBatchMessageSchema.MigrationState, state)
			};
			List<PropertyDefinition> list2 = new List<PropertyDefinition>
			{
				StoreObjectSchema.ItemClass,
				MigrationBatchMessageSchema.MigrationState
			};
			if (nextProcessTime != null)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.LessThanOrEqual, MigrationBatchMessageSchema.MigrationNextProcessTime, nextProcessTime));
				list2.Add(MigrationBatchMessageSchema.MigrationNextProcessTime);
			}
			return provider.FindMessageIds(QueryFilter.AndTogether(list.ToArray()), list2.ToArray(), MigrationJob.StateSort, delegate(IDictionary<PropertyDefinition, object> row)
			{
				if (!object.Equals(row[StoreObjectSchema.ItemClass], MigrationBatchMessageSchema.MigrationJobClass))
				{
					return MigrationRowSelectorResult.RejectRowStopProcessing;
				}
				if ((MigrationState)row[MigrationBatchMessageSchema.MigrationState] != state)
				{
					return MigrationRowSelectorResult.RejectRowStopProcessing;
				}
				if (nextProcessTime != null && ExDateTime.Compare((ExDateTime)row[MigrationBatchMessageSchema.MigrationNextProcessTime], nextProcessTime.Value) > 0)
				{
					return MigrationRowSelectorResult.RejectRowStopProcessing;
				}
				return MigrationRowSelectorResult.AcceptRow;
			}, maxCount);
		}

		internal static IEnumerable<StoreObjectId> GetIdsWithFlagPresence(IMigrationDataProvider provider, bool present, int? maxCount = null)
		{
			List<QueryFilter> list = new List<QueryFilter>
			{
				new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, MigrationBatchMessageSchema.MigrationJobClass),
				new ComparisonFilter(present ? ComparisonOperator.NotEqual : ComparisonOperator.Equal, MigrationBatchMessageSchema.MigrationFlags, MigrationFlags.None),
				new ComparisonFilter(ComparisonOperator.NotEqual, MigrationBatchMessageSchema.MigrationState, MigrationState.Disabled)
			};
			List<PropertyDefinition> list2 = new List<PropertyDefinition>
			{
				StoreObjectSchema.ItemClass,
				MigrationBatchMessageSchema.MigrationFlags,
				MigrationBatchMessageSchema.MigrationState
			};
			return provider.FindMessageIds(QueryFilter.AndTogether(list.ToArray()), list2.ToArray(), MigrationJob.FlagSort, delegate(IDictionary<PropertyDefinition, object> row)
			{
				if (!object.Equals(row[StoreObjectSchema.ItemClass], MigrationBatchMessageSchema.MigrationJobClass))
				{
					return MigrationRowSelectorResult.RejectRowStopProcessing;
				}
				bool flag = (MigrationFlags)row[MigrationBatchMessageSchema.MigrationFlags] != MigrationFlags.None;
				if (present != flag)
				{
					return MigrationRowSelectorResult.RejectRowStopProcessing;
				}
				if ((MigrationState)row[MigrationBatchMessageSchema.MigrationState] == MigrationState.Disabled)
				{
					return MigrationRowSelectorResult.RejectRowContinueProcessing;
				}
				return MigrationRowSelectorResult.AcceptRow;
			}, maxCount);
		}

		internal static bool MigrationTypeSupportsProvisioning(MigrationType migrationType)
		{
			if (migrationType <= MigrationType.ExchangeRemoteMove)
			{
				if (migrationType != MigrationType.IMAP && migrationType != MigrationType.ExchangeRemoteMove)
				{
					return true;
				}
			}
			else if (migrationType != MigrationType.ExchangeLocalMove && migrationType != MigrationType.PSTImport && migrationType != MigrationType.PublicFolder)
			{
				return true;
			}
			return false;
		}

		protected override bool InitializeFromMessageItem(IMigrationStoreObject message)
		{
			if (!base.InitializeFromMessageItem(message))
			{
				return false;
			}
			this.Initialize((MigrationType)message[MigrationBatchMessageSchema.MigrationType]);
			return true;
		}

		protected override void LoadLinkedStoredObjects(IMigrationStoreObject item, IMigrationDataProvider dataProvider)
		{
			Guid guidProperty = MigrationHelper.GetGuidProperty(item, MigrationBatchMessageSchema.MigrationJobSourceEndpoint, false);
			Guid guidProperty2 = MigrationHelper.GetGuidProperty(item, MigrationBatchMessageSchema.MigrationJobTargetEndpoint, false);
			if (guidProperty != Guid.Empty)
			{
				this.SourceEndpoint = MigrationEndpointBase.Get(guidProperty, dataProvider);
			}
			if (guidProperty2 != Guid.Empty)
			{
				this.TargetEndpoint = MigrationEndpointBase.Get(guidProperty2, dataProvider);
			}
			if (this.TargetEndpoint != null || this.SourceEndpoint != null || this.JobDirection == MigrationBatchDirection.Local)
			{
				return;
			}
			if (this.TargetEndpoint == null && this.SourceEndpoint == null)
			{
				throw new MigrationDataCorruptionException("Either source or target endpoint must be set.");
			}
		}

		private static MigrationJob GetUniqueJob(IMigrationDataProvider provider, MigrationEqualityFilter primaryFilter)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationJob migrationJob = null;
			foreach (MigrationJob migrationJob2 in MigrationJob.GetJobs(provider, primaryFilter))
			{
				if (migrationJob != null)
				{
					MigrationLogger.Log(MigrationEventType.Warning, "GetMigrationJob: delete {0} because of dup with {1}", new object[]
					{
						migrationJob2,
						migrationJob
					});
					try
					{
						provider.MoveMessageItems(new StoreObjectId[]
						{
							migrationJob2.StoreObjectId
						}, MigrationFolderName.CorruptedItems);
						continue;
					}
					catch (StoragePermanentException exception)
					{
						MigrationLogger.Log(MigrationEventType.Error, exception, "GetMigrationJob: couldn't delete migration job {0}", new object[]
						{
							migrationJob2
						});
						continue;
					}
				}
				migrationJob = migrationJob2;
			}
			return migrationJob;
		}

		private static IEnumerable<MigrationJob> GetJobs(IMigrationDataProvider provider, MigrationEqualityFilter primaryFilter)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationEqualityFilter[] secondaryFilters = null;
			if (primaryFilter == null)
			{
				primaryFilter = MigrationJob.MessageClassEqualityFilter;
			}
			else
			{
				secondaryFilters = new MigrationEqualityFilter[]
				{
					MigrationJob.MessageClassEqualityFilter
				};
			}
			IEnumerable<StoreObjectId> messageIds = MigrationHelper.FindMessageIds(provider, primaryFilter, secondaryFilters, MigrationJob.SortByCreationTime, null);
			return MigrationJob.Load(provider, messageIds);
		}

		private static IEnumerable<MigrationJob> Load(IMigrationDataProvider provider, IEnumerable<StoreObjectId> messageIds)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.ThrowOnNullArgument(messageIds, "messageIds");
			foreach (StoreObjectId messageId in messageIds)
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "MigrationJob.LoadFromMessageItem: Loading migration job from messageId: {0}.", new object[]
				{
					messageId
				});
				MigrationJob job;
				if (!MigrationJob.TryLoad(provider, messageId, out job))
				{
					throw new CouldNotLoadMigrationPersistedItemTransientException(messageId.ToHexEntryId());
				}
				yield return job;
			}
			yield break;
		}

		private static MultiValuedProperty<MigrationReportSet> LoadReports(IMigrationStoreObject message)
		{
			MultiValuedProperty<MigrationReportSet> multiValuedProperty = new MultiValuedProperty<MigrationReportSet>();
			string text = (string)message[MigrationBatchMessageSchema.MigrationReportSets];
			if (!string.IsNullOrEmpty(text))
			{
				try
				{
					using (StringReader stringReader = new StringReader(text))
					{
						using (XmlTextReader xmlTextReader = new XmlTextReader(stringReader))
						{
							if (xmlTextReader.MoveToContent() == XmlNodeType.Element && xmlTextReader.LocalName == "Reports")
							{
								MigrationReportSet item;
								while (xmlTextReader.ReadToFollowing("MigrationReportSet") && MigrationReportSet.TryCreate(xmlTextReader, out item))
								{
									multiValuedProperty.Add(item);
								}
							}
						}
					}
				}
				catch (XmlException innerException)
				{
					throw new MigrationDataCorruptionException("cannot read xml reports for job:" + text, innerException);
				}
			}
			return multiValuedProperty;
		}

		private static void SaveNotificationEmails(IMigrationStoreObject message, MultiValuedProperty<SmtpAddress> notificationEmails)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			if (notificationEmails != null && notificationEmails.Count > 0)
			{
				foreach (SmtpAddress smtpAddress in notificationEmails)
				{
					if (smtpAddress.Length > 0)
					{
						stringBuilder.Append(smtpAddress.ToString());
						stringBuilder.Append(MigrationBatchMessageSchema.ListSeparator[0]);
					}
				}
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Remove(stringBuilder.Length - 1, 1);
				}
			}
			message[MigrationBatchMessageSchema.MigrationJobNotificationEmails] = stringBuilder.ToString();
		}

		private static MultiValuedProperty<SmtpAddress> LoadNotificationEmails(IMigrationStoreObject message)
		{
			MultiValuedProperty<SmtpAddress> multiValuedProperty = new MultiValuedProperty<SmtpAddress>();
			string valueOrDefault = message.GetValueOrDefault<string>(MigrationBatchMessageSchema.MigrationJobNotificationEmails, string.Empty);
			if (!string.IsNullOrEmpty(valueOrDefault))
			{
				string[] array = valueOrDefault.Split(MigrationBatchMessageSchema.ListSeparator, StringSplitOptions.RemoveEmptyEntries);
				foreach (string smtpAddress in array)
				{
					SmtpAddress smtpAddress2 = MigrationHelper.GetSmtpAddress(smtpAddress, MigrationBatchMessageSchema.MigrationJobNotificationEmails);
					if (!multiValuedProperty.Contains(smtpAddress2))
					{
						multiValuedProperty.Add(smtpAddress2);
					}
				}
			}
			return multiValuedProperty;
		}

		private static void InitializeEndpointsForMigrationBatch(MigrationJob job, MigrationBatch batch)
		{
			if (job.SourceEndpoint != null)
			{
				batch.SourceEndpoint = job.SourceEndpoint;
			}
			if (job.TargetEndpoint != null)
			{
				batch.TargetEndpoint = job.TargetEndpoint;
			}
		}

		private static void CheckOperationIsSupported(MigrationJob.SupportsActionDelegate supportsActionsDelegate, LocalizedString safetyErrorMessage)
		{
			LocalizedString? localizedString;
			if (!supportsActionsDelegate(out localizedString))
			{
				throw new MigrationTransientException(localizedString ?? safetyErrorMessage);
			}
		}

		private void SetRestartTime(IMigrationDataProvider provider)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			base.CheckVersion();
			ExDateTime utcNow = ExDateTime.UtcNow;
			using (IMigrationMessageItem migrationMessageItem = base.FindMessageItem(provider, MigrationJob.JobStartPropertyDefinition))
			{
				migrationMessageItem.OpenAsReadWrite();
				MigrationHelperBase.SetExDateTimeProperty(migrationMessageItem, MigrationBatchMessageSchema.MigrationJobLastRestartTime, new ExDateTime?(utcNow));
				this.AutoRunCount++;
				this.WriteExtendedPropertiesToMessageItem(migrationMessageItem);
				migrationMessageItem.Save(SaveMode.NoConflictResolution);
			}
			this.LastRestartTime = new ExDateTime?(utcNow);
		}

		private bool SupportsRestarting(out LocalizedString? errorMsg)
		{
			errorMsg = null;
			if (MigrationJobStage.Corrupted.IsStatusSupported(this.Status))
			{
				errorMsg = new LocalizedString?(Strings.CorruptedMigrationBatchCannotBeStarted);
				return false;
			}
			if (this.StartTime == null)
			{
				errorMsg = new LocalizedString?(Strings.CompletedMigrationJobCannotBeStarted);
				return false;
			}
			if (!this.IsStaged && this.migrationJobType != MigrationType.PublicFolder)
			{
				return true;
			}
			bool flag = this.BatchLastUpdated != null && this.BatchLastUpdated.Value > this.StartTime.Value;
			if (!flag)
			{
				int num = this.FailedItemCount + this.StoppedItemCount;
				flag = (num > 0);
				MigrationLogger.Log(MigrationEventType.Verbose, "can start job? failed + stopped count {0}", new object[]
				{
					num
				});
			}
			foreach (MigrationUserStatus migrationUserStatus in MigrationJob.JobItemStatusEligibleForJobStarting)
			{
				if (flag)
				{
					break;
				}
				int cachedStatusCount = this.cachedItemCounts.GetCachedStatusCount(new MigrationUserStatus[]
				{
					migrationUserStatus
				});
				flag = (cachedStatusCount > 0);
				MigrationLogger.Log(MigrationEventType.Verbose, "can start job? item {0} count {1}", new object[]
				{
					migrationUserStatus,
					cachedStatusCount
				});
			}
			if (!flag)
			{
				errorMsg = new LocalizedString?(Strings.CompletedMigrationJobCannotBeStarted);
				return false;
			}
			return true;
		}

		private void SetBatchFlags(MigrationBatchFlags flagsToSet, bool enable)
		{
			if (enable)
			{
				this.BatchFlags |= flagsToSet;
				return;
			}
			this.BatchFlags &= ~flagsToSet;
		}

		private bool GetBatchFlags(MigrationBatchFlags flagsToGet)
		{
			return (this.BatchFlags & flagsToGet) == flagsToGet;
		}

		private void SetShouldSkip(SkippableMigrationSteps stepsToSet, bool shouldSkip)
		{
			if (shouldSkip)
			{
				this.SkipSteps |= stepsToSet;
				return;
			}
			this.SkipSteps &= ~stepsToSet;
		}

		private bool GetShouldSkip(SkippableMigrationSteps stepsToGet)
		{
			return (this.SkipSteps & stepsToGet) == stepsToGet;
		}

		private void SaveCsvStream(IMigrationMessageItem message, IMigrationAttachment attachment, Stream csvStream)
		{
			Util.StreamHandler.CopyStreamData(csvStream, attachment.Stream);
			attachment.Save(null);
			MigrationLogger.Log(MigrationEventType.Verbose, "the attachment last modified time: {0} and id {1}", new object[]
			{
				attachment.LastModifiedTime,
				attachment.Id
			});
			this.BatchLastUpdated = new ExDateTime?(ExDateTime.UtcNow);
			this.BatchInputId = Guid.NewGuid().ToString();
		}

		private void CreateValidationWarningAttachment(IMigrationMessageItem message, IEnumerable<MigrationBatchError> validationWarnings, bool clearExisting)
		{
			if (clearExisting)
			{
				message.DeleteAttachment("Errors.csv");
				this.ValidationWarningCount = 0;
			}
			int validationWarningCount = 0;
			using (IMigrationAttachment migrationAttachment = message.CreateAttachment("Errors.csv"))
			{
				using (StreamWriter streamWriter = new StreamWriter(migrationAttachment.Stream))
				{
					validationWarningCount = MigrationErrorCsvSchema.WriteHeaderAndErrors(streamWriter, validationWarnings);
					streamWriter.Flush();
				}
				migrationAttachment.Save(null);
			}
			this.ValidationWarningCount = validationWarningCount;
		}

		private void SaveReports(IMigrationStoreObject message, MultiValuedProperty<MigrationReportSet> reports, bool loaded)
		{
			XElement xelement = new XElement("Reports");
			if (reports != null && reports.Count > 0)
			{
				try
				{
					using (XmlWriter xmlWriter = xelement.CreateWriter())
					{
						foreach (MigrationReportSet migrationReportSet in reports)
						{
							migrationReportSet.WriteXml(xmlWriter);
						}
					}
				}
				catch (XmlException innerException)
				{
					throw new MigrationDataCorruptionException("cannot write xml reports for job", innerException);
				}
			}
			message[MigrationBatchMessageSchema.MigrationReportSets] = xelement.ToString();
		}

		private int GetItemCount(IMigrationDataProvider provider, params MigrationUserStatus[] statuses)
		{
			return MigrationJobItem.GetCount(provider, this.JobId, statuses);
		}

		private bool ShouldAutoRetryJob(int failureCount)
		{
			return this.MaxAutoRunCount != null && this.AutoRunCount < this.MaxAutoRunCount.Value && failureCount > 0;
		}

		private bool TryAutoRetryJob(IMigrationDataProvider provider, int failureCount)
		{
			if (!this.ShouldAutoRetryJob(failureCount))
			{
				return false;
			}
			MigrationLogger.Log(MigrationEventType.Information, "Rerunning batch {0} runcount {1} of max {2} b.c. found errors {3}", new object[]
			{
				this,
				this.AutoRunCount,
				this.MaxAutoRunCount,
				failureCount
			});
			this.SetRestartTime(provider);
			base.ReportData.Append(Strings.MigrationReportJobAutomaticallyRestarting(failureCount, this.AutoRunCount, this.MaxAutoRunCount.Value));
			provider.FlushReport(base.ReportData);
			return true;
		}

		private void Initialize(MigrationType migrationType)
		{
			this.migrationJobType = migrationType;
			this.Reports = new MultiValuedProperty<MigrationReportSet>();
			this.cachedItemCounts = new MigrationCountCache();
		}

		private void SetStatusData(IMigrationDataProvider provider, MigrationStatusData<MigrationJobStatus> newStatusData, bool clearLastCursorPosition)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.ThrowOnNullArgument(newStatusData, "statusData");
			PropertyDefinition[] properties = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
			{
				MigrationStatusData<MigrationJobStatus>.StatusPropertyDefinition,
				new PropertyDefinition[]
				{
					MigrationBatchMessageSchema.MigrationJobCursorPosition
				}
			});
			using (IMigrationStoreObject migrationStoreObject = this.FindStoreObject(provider, base.StoreObjectId, properties))
			{
				migrationStoreObject.OpenAsReadWrite();
				newStatusData.WriteToMessageItem(migrationStoreObject, true);
				if (clearLastCursorPosition)
				{
					migrationStoreObject[MigrationBatchMessageSchema.MigrationJobCursorPosition] = string.Empty;
				}
				migrationStoreObject.Save(SaveMode.NoConflictResolution);
			}
			this.StatusData = newStatusData;
			if (clearLastCursorPosition)
			{
				this.LastCursorPosition = string.Empty;
			}
			if (newStatusData.Status == MigrationJobStatus.Removed)
			{
				MigrationServiceFactory.Instance.GetAsyncNotificationAdapter().RemoveNotification(provider, this);
				return;
			}
			MigrationServiceFactory.Instance.GetAsyncNotificationAdapter().UpdateNotification(provider, this);
		}

		private void CreateInStore(IMigrationDataProvider provider, Stream csvStream, IEnumerable<MigrationBatchError> validationWarnings)
		{
			this.CreateInStore(provider, delegate(IMigrationStoreObject storeObject)
			{
				IMigrationMessageItem migrationMessageItem = storeObject as IMigrationMessageItem;
				if (validationWarnings != null)
				{
					this.CreateValidationWarningAttachment(migrationMessageItem, validationWarnings, false);
				}
				if (csvStream != null)
				{
					using (IMigrationAttachment migrationAttachment = migrationMessageItem.CreateAttachment("Request.csv"))
					{
						this.SaveCsvStream(migrationMessageItem, migrationAttachment, csvStream);
					}
				}
			});
		}

		private void Initialize(MigrationBatch migrationBatch, IMigrationDataProvider dataProvider, bool usePAW)
		{
			MigrationHelper.VerifyMigrationTypeEquality(this.MigrationType, migrationBatch.MigrationType);
			MigrationEndpointBase migrationEndpointBase = (migrationBatch.SourceEndpoint == null) ? null : MigrationEndpointBase.Get(migrationBatch.SourceEndpoint.Identity, dataProvider, false).First<MigrationEndpointBase>();
			MigrationEndpointBase migrationEndpointBase2 = (migrationBatch.TargetEndpoint == null) ? null : MigrationEndpointBase.Get(migrationBatch.TargetEndpoint.Identity, dataProvider, false).First<MigrationEndpointBase>();
			if (migrationEndpointBase == null && migrationEndpointBase2 == null && migrationBatch.BatchDirection != MigrationBatchDirection.Local)
			{
				throw new MigrationDataCorruptionException("Only local batches can have no endpoints.");
			}
			MigrationState? state;
			MigrationJobStatus migrationJobStatus;
			if (usePAW)
			{
				this.currentSupportedVersion = 5L;
				this.Flags = MigrationFlags.None;
				this.Stage = MigrationStage.Discovery;
				this.Workflow = MigrationServiceFactory.Instance.GetMigrationWorkflow(migrationBatch.MigrationType);
				if (migrationBatch.Flags.HasFlag(MigrationFlags.Stop))
				{
					state = new MigrationState?(MigrationState.Stopped);
					migrationJobStatus = MigrationJobStatus.Stopped;
				}
				else
				{
					state = new MigrationState?(MigrationState.Active);
					migrationJobStatus = MigrationJobStatus.SyncStarting;
					this.StartTime = new ExDateTime?(ExDateTime.UtcNow);
				}
			}
			else
			{
				this.currentSupportedVersion = 4L;
				state = null;
				migrationJobStatus = MigrationJobStatus.Created;
			}
			this.JobId = Guid.NewGuid();
			this.OriginalJobId = migrationBatch.OriginalBatchId;
			this.StatusData = new MigrationStatusData<MigrationJobStatus>(migrationJobStatus, MigrationJob.StatusDataVersionMap[this.currentSupportedVersion], state);
			this.JobName = migrationBatch.Identity.Name;
			this.SubmittedByUser = migrationBatch.SubmittedByUser;
			this.OwnerId = migrationBatch.OwnerId;
			this.OwnerExchangeObjectId = migrationBatch.OwnerExchangeObjectId;
			this.DelegatedAdminOwnerId = (migrationBatch.DelegatedAdminOwner ?? string.Empty);
			this.SubmittedByUserAdminType = migrationBatch.SubmittedByUserAdminType;
			this.NotificationEmails = migrationBatch.NotificationEmails;
			this.SkipSteps = migrationBatch.SkipSteps;
			this.TotalRowCount = migrationBatch.TotalCount;
			this.cachedItemCounts = new MigrationCountCache();
			this.FullScanTime = new ExDateTime?((ExDateTime)migrationBatch.OriginalCreationTime);
			this.OriginalCreationTime = (ExDateTime)migrationBatch.OriginalCreationTime;
			this.StatisticsEnabled = migrationBatch.OriginalStatisticsEnabled;
			this.IsStaged = (migrationBatch.CsvStream != null);
			if (migrationBatch.UserTimeZone != null)
			{
				this.UserTimeZone = migrationBatch.UserTimeZone.ExTimeZone;
			}
			else
			{
				this.UserTimeZone = ExTimeZone.CurrentTimeZone;
			}
			this.AdminCulture = (migrationBatch.Locale ?? CultureInfo.CurrentCulture);
			this.LastCursorPosition = "";
			this.BatchFlags = migrationBatch.BatchFlags;
			this.AllowUnknownColumnsInCsv = migrationBatch.AllowUnknownColumnsInCsv;
			this.SourceEndpoint = migrationEndpointBase;
			this.TargetEndpoint = migrationEndpointBase2;
			this.JobDirection = migrationBatch.BatchDirection;
			this.TargetDomainName = migrationBatch.TargetDomainName;
			this.SubscriptionSettings = JobSubscriptionSettingsBase.CreateFromBatch(migrationBatch, usePAW);
			this.NotificationId = MigrationServiceFactory.Instance.GetAsyncNotificationAdapter().CreateNotification(dataProvider, this);
			this.MaxAutoRunCount = migrationBatch.AutoRetryCount;
			if (migrationBatch.ReportInterval != null)
			{
				this.ReportInterval = migrationBatch.ReportInterval.Value;
			}
		}

		private void LogStatusEvent()
		{
			if (MigrationServiceFactory.Instance.ShouldLog)
			{
				MigrationJobLog.LogStatusEvent(this);
			}
		}

		private void SaveRuntimeJobData(IMigrationDataProvider dataProvider)
		{
			this.SaveExtendedProperties(dataProvider);
		}

		public const int MaxErrorCollectionSize = 500;

		internal const string EndOfDataRow = "EOF";

		internal const string InitialCursorPosition = "";

		private const string ReportsRootSerializedTag = "Reports";

		private const long MigrationJobEndpointVersion = 4L;

		private const long MigrationJobPAWVersion = 5L;

		private const string MigrationBatchFlagsKey = "MigrationBatchFlags";

		private const string IncrementalSyncIntervalKey = "IncrementalSyncInterval";

		private const string ReportIntervalKey = "ReportInterval";

		private const string BatchLastUpdatedKey = "BatchLastUpdated";

		private const string BatchInputIdKey = "BatchInputId";

		private const string OriginalJobIdKey = "OriginalJobId";

		private const string InitialSyncDateTimeKey = "InitialSyncDateTime";

		private const string InitialSyncDurationKey = "InitialSyncDuration";

		private const string ProcessingDurationKey = "ProcessingDuration";

		private const string ValidationWarningCountKey = "ValidationWarningCount";

		private const string TroubleshooterNotesKey = "TroubleshooterNotes";

		private const string RuntimeDataKey = "RuntimeData";

		private const string AutoRunCountKey = "AutoRunCount";

		private const string MaxAutoRunCountKey = "MaxAutoRunCount";

		private const string AllowUnknownColumnsInCsvKey = "AllowUnknownColumnsInCsv";

		internal static readonly MigrationUserStatus[] JobItemStatusForBatchCompletionErrors = new MigrationUserStatus[]
		{
			MigrationUserStatus.Failed,
			MigrationUserStatus.IncrementalFailed,
			MigrationUserStatus.Corrupted,
			MigrationUserStatus.Stopped,
			MigrationUserStatus.IncrementalStopped
		};

		internal static readonly MigrationUserStatus[] JobItemsStatusForActive = MigrationUser.MapFromSummaryToStatus[MigrationUserStatusSummary.Active];

		internal static readonly MigrationUserStatus[] JobItemsStatusForStopped = MigrationUser.MapFromSummaryToStatus[MigrationUserStatusSummary.Stopped];

		internal static readonly MigrationUserStatus[] JobItemsStatusForSynced = MigrationUser.MapFromSummaryToStatus[MigrationUserStatusSummary.Synced];

		internal static readonly MigrationUserStatus[] JobItemsStatusForFinalized = MigrationUser.MapFromSummaryToStatus[MigrationUserStatusSummary.Completed];

		internal static readonly MigrationUserStatus[] JobItemsStatusForFailedInitial = new MigrationUserStatus[]
		{
			MigrationUserStatus.Failed
		};

		internal static readonly MigrationUserStatus[] JobItemsStatusForFailedIncremental = new MigrationUserStatus[]
		{
			MigrationUserStatus.IncrementalFailed
		};

		internal static readonly MigrationUserStatus[] JobItemsStatusForFailedFinalization = new MigrationUserStatus[]
		{
			MigrationUserStatus.CompletionFailed
		};

		internal static readonly MigrationUserStatus[] JobItemsStatusForFailedOther = new MigrationUserStatus[]
		{
			MigrationUserStatus.Corrupted,
			MigrationUserStatus.CompletedWithWarnings
		};

		internal static readonly MigrationUserStatus[] JobItemsStatusForActiveInitial = new MigrationUserStatus[]
		{
			MigrationUserStatus.Validating,
			MigrationUserStatus.Provisioning,
			MigrationUserStatus.ProvisionUpdating,
			MigrationUserStatus.Syncing
		};

		internal static readonly MigrationUserStatus[] AllJobItemsStatuses = (MigrationUserStatus[])Enum.GetValues(typeof(MigrationUserStatus));

		internal static readonly MigrationUserStatus[] AllUsedPAWJobItemStatuses = new MigrationUserStatus[]
		{
			MigrationUserStatus.Validating,
			MigrationUserStatus.Provisioning,
			MigrationUserStatus.ProvisionUpdating,
			MigrationUserStatus.Syncing,
			MigrationUserStatus.Synced,
			MigrationUserStatus.Completed,
			MigrationUserStatus.CompletedWithWarnings,
			MigrationUserStatus.Starting,
			MigrationUserStatus.Stopping,
			MigrationUserStatus.Stopped,
			MigrationUserStatus.Failed,
			MigrationUserStatus.Removing
		};

		private static readonly ConcurrentDictionary<string, PropertyDefinition[]> PropertyDefinitionsHash = new ConcurrentDictionary<string, PropertyDefinition[]>();

		private static readonly SortBy[] StateSort = new SortBy[]
		{
			new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending),
			new SortBy(MigrationBatchMessageSchema.MigrationState, SortOrder.Ascending),
			new SortBy(MigrationBatchMessageSchema.MigrationNextProcessTime, SortOrder.Ascending)
		};

		private static readonly SortBy[] FlagSort = new SortBy[]
		{
			new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending),
			new SortBy(MigrationBatchMessageSchema.MigrationFlags, SortOrder.Ascending),
			new SortBy(MigrationBatchMessageSchema.MigrationState, SortOrder.Ascending)
		};

		private static readonly Dictionary<long, long> StatusDataVersionMap = new Dictionary<long, long>
		{
			{
				4L,
				1L
			},
			{
				5L,
				2L
			}
		};

		private static readonly MigrationUserStatus[] JobItemStatusEligibleForJobStarting = new MigrationUserStatus[]
		{
			MigrationUserStatus.Queued,
			MigrationUserStatus.Provisioning,
			MigrationUserStatus.ProvisionUpdating
		};

		private static readonly PropertyDefinition[] MigrationJobPropertyDefinition = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
		{
			new PropertyDefinition[]
			{
				StoreObjectSchema.ItemClass,
				MigrationBatchMessageSchema.MigrationJobId,
				MigrationBatchMessageSchema.MigrationJobName,
				MigrationBatchMessageSchema.MigrationJobSubmittedBy,
				MigrationBatchMessageSchema.MigrationJobTotalRowCount,
				MigrationBatchMessageSchema.MigrationJobNotificationEmails,
				MigrationBatchMessageSchema.MigrationJobOriginalCreationTime,
				MigrationBatchMessageSchema.MigrationJobStartTime,
				MigrationBatchMessageSchema.MigrationJobUserTimeZone,
				MigrationBatchMessageSchema.MigrationJobCancelledFlag,
				MigrationBatchMessageSchema.MigrationJobItemRowIndex,
				MigrationBatchMessageSchema.MigrationJobAdminCulture,
				MigrationBatchMessageSchema.MigrationJobMaxConcurrentMigrations,
				MigrationBatchMessageSchema.MigrationJobCursorPosition,
				MigrationBatchMessageSchema.MigrationJobOwnerId,
				MigrationBatchMessageSchema.MigrationJobDelegatedAdminOwnerId,
				MigrationBatchMessageSchema.MigrationJobSuppressErrors,
				MigrationBatchMessageSchema.MigrationJobFinalizeTime,
				MigrationBatchMessageSchema.MigrationSubmittedByUserAdminType,
				MigrationBatchMessageSchema.MigrationJobStatisticsEnabled,
				MigrationBatchMessageSchema.MigrationJobCancellationReason,
				MigrationBatchMessageSchema.MigrationJobPoisonCount,
				MigrationBatchMessageSchema.MigrationSuccessReportUrl,
				MigrationBatchMessageSchema.MigrationErrorReportUrl,
				MigrationBatchMessageSchema.MigrationJobIsStaged,
				MigrationBatchMessageSchema.MigrationJobItemSubscriptionLastChecked,
				MigrationBatchMessageSchema.MigrationReportSets,
				MigrationBatchMessageSchema.MigrationJobSourceEndpoint,
				MigrationBatchMessageSchema.MigrationJobTargetEndpoint,
				MigrationBatchMessageSchema.MigrationJobDirection,
				MigrationBatchMessageSchema.MigrationJobSkipSteps,
				MigrationBatchMessageSchema.MigrationJobCountCache,
				MigrationBatchMessageSchema.MigrationJobCountCacheFullScanTime,
				MigrationBatchMessageSchema.MigrationJobLastRestartTime,
				MigrationBatchMessageSchema.MigrationJobIsRunning,
				MigrationBatchMessageSchema.MigrationNextProcessTime,
				MigrationBatchMessageSchema.MigrationFlags,
				MigrationBatchMessageSchema.MigrationStage,
				MigrationBatchMessageSchema.MigrationWorkflow,
				MigrationBatchMessageSchema.MigrationExchangeObjectId,
				MigrationBatchMessageSchema.MigrationJobLastFinalizationAttempt
			},
			MigrationStatusData<MigrationJobStatus>.StatusPropertyDefinition,
			MigrationPersistableBase.MigrationBaseDefinitions
		});

		private static readonly PropertyDefinition[] MigrationJobTypeDefinition = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
		{
			new PropertyDefinition[]
			{
				MigrationBatchMessageSchema.MigrationType
			},
			MigrationPersistableBase.VersionPropertyDefinitions
		});

		private static readonly MigrationEqualityFilter MessageClassEqualityFilter = new MigrationEqualityFilter(StoreObjectSchema.ItemClass, MigrationBatchMessageSchema.MigrationJobClass);

		private static readonly PropertyDefinition[] JobLastScheduledPropertyDefinition = new PropertyDefinition[]
		{
			MigrationBatchMessageSchema.MigrationJobItemSubscriptionLastChecked
		};

		private static readonly PropertyDefinition[] JobMigrationFlagsPropertyDefinition = new PropertyDefinition[]
		{
			MigrationBatchMessageSchema.MigrationFlags
		};

		private static readonly PropertyDefinition[] JobCancelPropertyDefinition = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
		{
			new PropertyDefinition[]
			{
				MigrationBatchMessageSchema.MigrationJobCancelledFlag,
				MigrationBatchMessageSchema.MigrationJobCancellationReason
			},
			MigrationPersistableBase.MigrationBaseDefinitions
		});

		private static readonly PropertyDefinition[] JobFinalizePropertyDefinition = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
		{
			new PropertyDefinition[]
			{
				MigrationBatchMessageSchema.MigrationJobFinalizeTime,
				MigrationBatchMessageSchema.MigrationJobNotificationEmails,
				MigrationBatchMessageSchema.MigrationJobCancelledFlag,
				MigrationBatchMessageSchema.MigrationJobCancellationReason,
				MigrationBatchMessageSchema.MigrationJobPoisonCount,
				MigrationBatchMessageSchema.MigrationJobLastFinalizationAttempt
			},
			MigrationStatusData<MigrationJobStatus>.StatusPropertyDefinition
		});

		private static readonly PropertyDefinition[] JobStartPropertyDefinition = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
		{
			new PropertyDefinition[]
			{
				MigrationBatchMessageSchema.MigrationJobItemRowIndex,
				MigrationBatchMessageSchema.MigrationJobNotificationEmails,
				MigrationBatchMessageSchema.MigrationJobFinalizeTime,
				MigrationBatchMessageSchema.MigrationJobCancelledFlag,
				MigrationBatchMessageSchema.MigrationJobCancellationReason,
				MigrationBatchMessageSchema.MigrationJobPoisonCount
			},
			MigrationStatusData<MigrationJobStatus>.StatusPropertyDefinition,
			MigrationPersistableBase.MigrationBaseDefinitions
		});

		private static readonly PropertyDefinition[] JobSetReportIDDefinition = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
		{
			new PropertyDefinition[]
			{
				MigrationBatchMessageSchema.MigrationErrorReportUrl,
				MigrationBatchMessageSchema.MigrationSuccessReportUrl,
				MigrationBatchMessageSchema.MigrationReportSets
			},
			MigrationPersistableBase.MigrationBaseDefinitions
		});

		private static readonly PropertyDefinition[] JobCountCacheDefinition = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
		{
			new PropertyDefinition[]
			{
				MigrationBatchMessageSchema.MigrationJobCountCache,
				MigrationBatchMessageSchema.MigrationJobCountCacheFullScanTime
			},
			MigrationPersistableBase.MigrationBaseDefinitions
		});

		private static readonly SortBy[] SortByCreationTime = new SortBy[]
		{
			new SortBy(StoreObjectSchema.CreationTime, SortOrder.Ascending)
		};

		private MigrationType migrationJobType;

		private Guid jobId;

		private string jobName;

		private ADObjectId ownerId;

		private Guid ownerExchangeObjectId;

		private DelegatedPrincipal delegatedAdminOwner;

		private SubmittedByUserAdminType submittedByUserAdminType;

		private MigrationStatusData<MigrationJobStatus> statusData;

		private MigrationJobStatus status;

		private ExDateTime originalCreationTime;

		private ExDateTime? startTime;

		private ExDateTime? finalizeTime;

		private int lastFinalizationAttempt;

		private ExTimeZone userTimeZone;

		private string submittedByUser;

		private int totalRowCount;

		private MigrationCountCache cachedItemCounts;

		private ExDateTime? fullScanTime;

		private MultiValuedProperty<SmtpAddress> notificationEmails;

		private JobCancellationStatus jobCancellationStatus;

		private string lastCursorPosition;

		private CultureInfo adminCulture;

		private bool statisticsEnabled = true;

		private bool isStaged;

		private ExDateTime? lastScheduled;

		private ExDateTime? nextProcessTime;

		private long currentSupportedVersion;

		private delegate bool SupportsActionDelegate(out LocalizedString? errorMessage);
	}
}
