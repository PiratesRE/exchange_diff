using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Migration;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationSession : MigrationPersistableBase, IMigrationConfig
	{
		protected MigrationSession()
		{
			this.Jobs = new ConcurrentDictionary<Guid, MigrationJob>();
		}

		public IMigrationConfig Config
		{
			get
			{
				return this;
			}
		}

		public int RunnableJobCount
		{
			get
			{
				this.CheckQueueInitialization();
				ExDateTime utcNow = ExDateTime.UtcNow;
				return this.Jobs.Values.Count((MigrationJob job) => MigrationSession.RunnableJobStatuses.Contains(job.Status) && job.NextProcessTime <= utcNow && MigrationApplication.IsMigrationTypeEnabled(job.MigrationType));
			}
		}

		public int ActiveJobCount
		{
			get
			{
				int num = 0;
				foreach (MigrationJob migrationJob in this.Jobs.Values)
				{
					if (migrationJob.Status != MigrationJobStatus.Removed && migrationJob.Status != MigrationJobStatus.Failed && migrationJob.Status != MigrationJobStatus.Corrupted)
					{
						num++;
					}
				}
				return num;
			}
		}

		public int TotalJobCount
		{
			get
			{
				this.CheckQueueInitialization();
				return this.Jobs.Count;
			}
		}

		public override long CurrentSupportedVersion
		{
			get
			{
				long config = ConfigBase<MigrationServiceConfigSchema>.GetConfig<long>("SessionCurrentVersion");
				long maximumSupportedVersion = this.MaximumSupportedVersion;
				MigrationLogger.Log(MigrationEventType.Verbose, "Current configured version is '{0}' max is {1}", new object[]
				{
					config,
					maximumSupportedVersion
				});
				return Math.Min(config, maximumSupportedVersion);
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
				return 1L;
			}
		}

		public long SupportedVersionUpgrade
		{
			get
			{
				if (!this.CanBeOverwritten)
				{
					return base.Version;
				}
				return this.CurrentSupportedVersion;
			}
		}

		public int MaxNumberOfBatches
		{
			get
			{
				int? num = base.ExtendedProperties.Get<int?>("MaxNumberOfBatches", null);
				if (num != null)
				{
					return num.Value;
				}
				return this.GetMaxNumberOfBatchesDefault();
			}
			set
			{
				base.ExtendedProperties.Set<int?>("MaxNumberOfBatches", new int?(value));
			}
		}

		public override PropertyDefinition[] PropertyDefinitions
		{
			get
			{
				return MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
				{
					base.PropertyDefinitions,
					new StorePropertyDefinition[]
					{
						MigrationBatchMessageSchema.MigrationRuntimeJobData
					}
				});
			}
		}

		public bool HasJobs
		{
			get
			{
				return this.Jobs.Count > 0;
			}
		}

		public MigrationFeature EnabledFeatures
		{
			get
			{
				MigrationFeature migrationFeature = base.ExtendedProperties.Get<MigrationFeature>("EnabledFeatures", MigrationFeature.None);
				if (migrationFeature == MigrationFeature.None && base.Version >= 2L)
				{
					migrationFeature = ConfigBase<MigrationServiceConfigSchema>.GetConfig<MigrationFeature>("PublishedMigrationFeatures");
					base.ExtendedProperties.Set<MigrationFeature>("EnabledFeatures", migrationFeature);
				}
				return migrationFeature;
			}
			set
			{
				base.ExtendedProperties.Set<MigrationFeature>("EnabledFeatures", value);
			}
		}

		public ExDateTime LastUpgradeConstraintEnforcedTimestamp
		{
			get
			{
				return base.ExtendedProperties.Get<ExDateTime>("LastUpdateConstraintCheckTimestamp", ExDateTime.MinValue);
			}
			private set
			{
				base.ExtendedProperties.Set<ExDateTime>("LastUpdateConstraintCheckTimestamp", value);
			}
		}

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

		public Unlimited<int> MaxConcurrentMigrations
		{
			get
			{
				string text = base.ExtendedProperties.Get<string>("MaxConcurrentMigrations", string.Empty);
				Unlimited<int> result;
				if (!string.IsNullOrWhiteSpace(text) && Unlimited<int>.TryParse(text, out result))
				{
					return result;
				}
				if (!MigrationServiceFactory.Instance.IsMultiTenantEnabled())
				{
					return Unlimited<int>.UnlimitedValue;
				}
				return new Unlimited<int>(100);
			}
			set
			{
				base.ExtendedProperties.Set<string>("MaxConcurrentMigrations", value.ToString());
			}
		}

		internal bool CanBeOverwritten
		{
			get
			{
				return !this.HasJobs;
			}
		}

		private static HashSet<MigrationJobStatus> RunnableJobStatuses
		{
			get
			{
				if (MigrationSession.runnableJobStatuses == null)
				{
					HashSet<MigrationJobStatus> hashSet = new HashSet<MigrationJobStatus>();
					foreach (MigrationSession.JobStage jobStage in MigrationSession.RunnableJobStages)
					{
						foreach (MigrationJobStatus item in jobStage.SupportedStatuses)
						{
							hashSet.Add(item);
						}
					}
					MigrationSession.runnableJobStatuses = hashSet;
				}
				return MigrationSession.runnableJobStatuses;
			}
		}

		private ExDateTime? InitializationTime
		{
			get
			{
				return base.ExtendedProperties.Get<ExDateTime?>("InitializationTime");
			}
			set
			{
				base.ExtendedProperties.Set<ExDateTime?>("InitializationTime", value);
			}
		}

		private ConcurrentDictionary<Guid, MigrationJob> Jobs { get; set; }

		public static MigrationSession Get(IMigrationDataProvider dataProvider)
		{
			return MigrationSession.Get(dataProvider, true);
		}

		public static MigrationSession Get(IMigrationDataProvider dataProvider, bool intializeQueue)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			MigrationSession migrationSession = new MigrationSession();
			if (!migrationSession.TryLoad(dataProvider, dataProvider.Folder.Id))
			{
				migrationSession.ReinitializeSession();
				migrationSession.Create(dataProvider);
			}
			if (intializeQueue)
			{
				migrationSession.RefreshJobQueueIfNeeded(dataProvider);
			}
			return migrationSession;
		}

		public static IMigrationConfig GetConfig(IMigrationDataProvider dataProvider)
		{
			return MigrationSession.Get(dataProvider).Config;
		}

		public static bool SupportsCutover(IMigrationDataProvider dataProvider)
		{
			return dataProvider.ADProvider.IsMSOSyncEnabled && !dataProvider.ADProvider.IsDirSyncEnabled;
		}

		public MigrationStatistics GetMigrationStatistics(IMigrationDataProvider provider)
		{
			MigrationStatistics migrationStatistics = new MigrationStatistics();
			migrationStatistics.Identity = new MigrationStatisticsId(provider.OrganizationId);
			migrationStatistics.TotalCount = 0;
			migrationStatistics.FinalizedCount = 0;
			migrationStatistics.ProvisionedCount = 0;
			migrationStatistics.SyncedCount = 0;
			migrationStatistics.FailedCount = 0;
			migrationStatistics.ActiveCount = 0;
			migrationStatistics.StoppedCount = 0;
			migrationStatistics.PendingCount = 0;
			migrationStatistics.MigrationType = MigrationType.None;
			foreach (MigrationJob migrationJob in this.Jobs.Values)
			{
				migrationStatistics.MigrationType |= migrationJob.MigrationType;
				migrationStatistics.TotalCount += migrationJob.TotalCount;
				migrationStatistics.SyncedCount += migrationJob.SyncedItemCount;
				migrationStatistics.FinalizedCount += migrationJob.FinalizedItemCount;
				migrationStatistics.ActiveCount += migrationJob.ActiveItemCount;
				migrationStatistics.StoppedCount += migrationJob.StoppedItemCount;
				migrationStatistics.ProvisionedCount += migrationJob.ProvisionedItemCount;
				migrationStatistics.FailedCount += migrationJob.FailedItemCount;
				migrationStatistics.PendingCount += migrationJob.PendingCount;
			}
			return migrationStatistics;
		}

		public MigrationConfig GetMigrationConfig(IMigrationDataProvider provider)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationConfig migrationConfig = new MigrationConfig();
			migrationConfig.Identity = new MigrationConfigId(provider.OrganizationId);
			migrationConfig.MaxNumberOfBatches = this.MaxNumberOfBatches;
			migrationConfig.SupportsCutover = MigrationSession.SupportsCutover(provider);
			migrationConfig.MaxConcurrentMigrations = this.MaxConcurrentMigrations;
			migrationConfig.CanSubmitNewBatch = true;
			if (!provider.ADProvider.IsMigrationEnabled || this.MaxNumberOfBatches <= this.Jobs.Count)
			{
				migrationConfig.CanSubmitNewBatch = false;
			}
			migrationConfig.Features = MigrationFeature.None;
			foreach (MigrationFeature migrationFeature in MigrationSession.FeatureVersionMap.Keys)
			{
				if (this.IsSupported(migrationFeature))
				{
					migrationConfig.Features |= migrationFeature;
				}
			}
			return migrationConfig;
		}

		public string GetJobName(Guid jobId)
		{
			return this.GetJob(null, jobId, true).JobName;
		}

		public bool IsSupported(MigrationFeature features)
		{
			long num = (base.Version == -1L) ? this.CurrentSupportedVersion : base.Version;
			MigrationFeature enabledFeatures = this.EnabledFeatures;
			return !MigrationUtil.IsFeatureBlocked(features) && num >= MigrationSession.GetMinimumSupportedVersion(features) && (enabledFeatures & features) == features;
		}

		public bool IsDisplaySupported(MigrationFeature features)
		{
			return this.MaximumSupportedVersion >= MigrationSession.GetMinimumSupportedVersion(features);
		}

		public IEnumerable<MigrationJob> FindJobsToPickUp(IMigrationDataProvider dataProvider)
		{
			this.CheckQueueInitialization();
			this.RefreshJobQueueIfNeeded(dataProvider);
			List<MigrationJob> localJobQueue = new List<MigrationJob>(from job in this.Jobs.Values
			orderby job.NextProcessTime
			select job);
			ExDateTime utcNow = ExDateTime.UtcNow;
			foreach (MigrationSession.JobStage stage in MigrationSession.RunnableJobStages)
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "looking for jobs in stage:{0}", new object[]
				{
					stage.Name
				});
				foreach (MigrationJob job2 in localJobQueue)
				{
					if (job2 != null)
					{
						if (!MigrationApplication.IsMigrationTypeEnabled(job2.MigrationType))
						{
							MigrationLogger.Log(MigrationEventType.Information, "Skipping job '{0}' as migration type is not enabled: {1}", new object[]
							{
								job2.JobId,
								job2.MigrationType
							});
						}
						else
						{
							if (job2.NextProcessTime > utcNow)
							{
								break;
							}
							if (stage.IsStatusSupported(job2.Status))
							{
								yield return job2;
							}
						}
					}
				}
			}
			yield break;
		}

		public IEnumerable<MigrationJob> GetOrderedJobs(IMigrationDataProvider dataProvider)
		{
			List<MigrationJob> source = new List<MigrationJob>(from job in this.Jobs.Values
			orderby job.OriginalCreationTime
			select job);
			return from job in source
			where job != null
			select job;
		}

		public void CheckFeaturesAvailableToUpgrade(MigrationFeature features)
		{
			long minimumSupportedVersion = MigrationSession.GetMinimumSupportedVersion(features);
			if (minimumSupportedVersion > base.Version && !this.CanBeOverwritten)
			{
				throw new MigrationPermanentException(Strings.CannotUpgradeMigrationVersion("there are active jobs"));
			}
			if (MigrationUtil.IsFeatureBlocked(features))
			{
				throw new MigrationPermanentException(Strings.CannotUpgradeMigrationVersion(string.Format("feature {0} is currently blocked.", features)));
			}
		}

		public void SetMigrationConfig(IMigrationDataProvider dataProvider, MigrationConfig config)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			MigrationUtil.ThrowOnNullArgument(config, "config");
			bool flag = false;
			if (config.MaxNumberOfBatches != this.MaxNumberOfBatches)
			{
				MigrationLogger.Log(MigrationEventType.Information, "Setting max number of batches from {0} to {1} for session {2}", new object[]
				{
					this.MaxNumberOfBatches,
					config.MaxNumberOfBatches,
					this
				});
				this.MaxNumberOfBatches = config.MaxNumberOfBatches;
				flag = true;
			}
			if (config.MaxConcurrentMigrations != this.MaxConcurrentMigrations)
			{
				MigrationLogger.Log(MigrationEventType.Information, "Setting max concurrent migrations from {0} to {1} for session {2}", new object[]
				{
					this.MaxConcurrentMigrations,
					config.MaxConcurrentMigrations,
					this
				});
				this.MaxConcurrentMigrations = config.MaxConcurrentMigrations;
				flag = true;
			}
			if (flag)
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "updating config with new info", new object[0]);
				this.SaveExtendedProperties(dataProvider);
			}
		}

		public bool EnableFeatures(IMigrationDataProvider dataProvider, MigrationFeature features)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			this.CheckFeaturesAvailableToUpgrade(features);
			if (base.StoreObjectId == null)
			{
				this.Create(dataProvider);
			}
			long minimumSupportedVersion = MigrationSession.GetMinimumSupportedVersion(features);
			if (base.Version < minimumSupportedVersion)
			{
				this.UpgradeVersion(dataProvider, minimumSupportedVersion);
			}
			this.EnabledFeatures |= features;
			this.SaveExtendedProperties(dataProvider);
			return true;
		}

		public void DisableFeatures(IMigrationDataProvider dataProvider, MigrationFeature features, bool force)
		{
			if ((features & MigrationFeature.MultiBatch) == MigrationFeature.MultiBatch && !force)
			{
				throw new MultiBatchCannotBeDisabledPermanentException(features.ToString());
			}
			this.EnabledFeatures &= ~features;
			this.SaveExtendedProperties(dataProvider);
		}

		public void CheckAndUpgradeToSupportedFeaturesAndVersion(IMigrationDataProvider dataProvider)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			long supportedVersionUpgrade = this.SupportedVersionUpgrade;
			if (supportedVersionUpgrade > base.Version)
			{
				this.UpgradeVersion(dataProvider, supportedVersionUpgrade);
			}
			MigrationFeature migrationFeature = MigrationFeature.None;
			string settingName = "PublishedMigrationFeatures";
			MigrationFeature config = ConfigBase<MigrationServiceConfigSchema>.GetConfig<MigrationFeature>(settingName);
			foreach (object obj in Enum.GetValues(typeof(MigrationFeature)))
			{
				MigrationFeature migrationFeature2 = (MigrationFeature)obj;
				if ((config & migrationFeature2) != MigrationFeature.None && !this.IsSupported(migrationFeature2) && !MigrationUtil.IsFeatureBlocked(migrationFeature2))
				{
					try
					{
						this.CheckFeaturesAvailableToUpgrade(migrationFeature2);
						migrationFeature |= migrationFeature2;
					}
					catch (MigrationPermanentException exception)
					{
						MigrationLogger.Log(MigrationEventType.Verbose, exception, "Not applying feature {0} to session {1} because CheckAndUpgradeToSupportedFeature failed.", new object[]
						{
							migrationFeature2,
							this
						});
					}
				}
			}
			if (migrationFeature != MigrationFeature.None)
			{
				MigrationLogger.Log(MigrationEventType.Information, "The session '{0}' will get the features '{1}' enabled.", new object[]
				{
					this,
					migrationFeature
				});
				this.EnableFeatures(dataProvider, migrationFeature);
			}
		}

		public bool CanCreateNewJobOfType(MigrationType migrationType, bool isStaged, out LocalizedException exception)
		{
			if (migrationType == MigrationType.None)
			{
				exception = new UnsupportedMigrationTypeException(migrationType);
				return false;
			}
			if (migrationType == MigrationType.BulkProvisioning)
			{
				exception = new UnsupportedMigrationTypeException(migrationType);
				return false;
			}
			if (this.ActiveJobCount >= this.MaxNumberOfBatches)
			{
				exception = new MaximumNumberOfBatchesReachedException();
				return false;
			}
			if (migrationType == MigrationType.PublicFolder)
			{
				bool flag = this.Jobs.Values.Any((MigrationJob job) => job.MigrationType == MigrationType.PublicFolder);
				if (flag)
				{
					exception = new OnlyOnePublicFolderBatchIsAllowedException();
					return false;
				}
			}
			bool flag2 = this.Jobs.Values.Any((MigrationJob job) => job.IsStaged);
			if (isStaged != flag2 && this.Jobs.Count > 0)
			{
				exception = new CutoverAndStagedBatchesCannotCoexistException();
				return false;
			}
			if (!isStaged && this.Jobs.Count > 0)
			{
				exception = new OnlyOneCutoverBatchIsAllowedException();
				return false;
			}
			exception = null;
			return true;
		}

		public void AddMigrationJob(IMigrationDataProvider dataProvider, MigrationJob job)
		{
			if (!this.queueInitialized)
			{
				this.RefreshJobQueueIfNeeded(dataProvider);
			}
			this.AddMigrationJob(job);
			this.SaveExtendedProperties(dataProvider);
		}

		public void Create(IMigrationDataProvider dataProvider)
		{
			MigrationUtil.AssertOrThrow(this.CanBeOverwritten, "Session cannot be re-created while active.", new object[0]);
			this.Initialize();
			this.InitializationTime = new ExDateTime?(ExDateTime.UtcNow);
			this.CreateInStore(dataProvider);
		}

		public override XElement GetDiagnosticInfo(IMigrationDataProvider dataProvider, MigrationDiagnosticArgument argument)
		{
			return base.GetDiagnosticInfo(dataProvider, argument, new XElement("MigrationSession"));
		}

		public bool RemoveJob(IMigrationDataProvider dataProvider, Guid jobId)
		{
			MigrationJob job = this.GetJob(dataProvider, jobId, true);
			job.Delete(dataProvider, true);
			if (this.Jobs.ContainsKey(jobId))
			{
				this.Jobs.TryRemove(jobId, out job);
			}
			if (this.Jobs.Count > 0)
			{
				return false;
			}
			IUpgradeConstraintAdapter upgradeConstraintAdapter = MigrationServiceFactory.Instance.GetUpgradeConstraintAdapter(this);
			upgradeConstraintAdapter.MarkUpgradeConstraintForExpiry(dataProvider, null);
			return false;
		}

		public MigrationJob CreateJob(IMigrationDataProvider dataProvider, MigrationBatch migrationBatch)
		{
			if (!base.IsPersisted)
			{
				this.ReinitializeSession();
				this.Create(dataProvider);
			}
			if (this.BatchFlags != MigrationBatchFlags.None)
			{
				MigrationLogger.Log(MigrationEventType.Information, "applying batch flags {0} to current flags {1} for batch", new object[]
				{
					migrationBatch.BatchFlags,
					this.BatchFlags,
					migrationBatch
				});
				migrationBatch.BatchFlags |= this.BatchFlags;
			}
			MigrationJob migrationJob = MigrationJob.Create(dataProvider, this, migrationBatch);
			MigrationLogger.Log(MigrationEventType.Verbose, "session already existed, adding job {0} to back of queue", new object[]
			{
				migrationJob
			});
			this.AddMigrationJob(dataProvider, migrationJob);
			this.RefreshJobQueueIfNeeded(dataProvider);
			return migrationJob;
		}

		public void Initialize()
		{
			MigrationLogger.Log(MigrationEventType.Verbose, "resetting extended properties {0} for {1}", new object[]
			{
				base.ExtendedProperties,
				this
			});
			base.ExtendedProperties = new PersistableDictionary();
		}

		public void SetLastUpdateConstraintEnforcedTimestamp(IMigrationDataProvider dataProvider, ExDateTime whenEnforced)
		{
			this.LastUpgradeConstraintEnforcedTimestamp = whenEnforced;
			if (base.IsPersisted)
			{
				this.SaveExtendedProperties(dataProvider);
			}
		}

		public override IMigrationStoreObject FindStoreObject(IMigrationDataProvider dataProvider, StoreObjectId id, PropertyDefinition[] properties)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			MigrationUtil.ThrowOnNullArgument(id, "id");
			IMigrationStoreObject result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				IMigrationStoreObject rootFolder = dataProvider.GetRootFolder(MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
				{
					MigrationFolder.FolderIdPropertyDefinition,
					properties
				}));
				disposeGuard.Add<IMigrationStoreObject>(rootFolder);
				ExAssert.RetailAssert(MigrationFolderName.SyncMigration.ToString().Equals(rootFolder.Name, StringComparison.OrdinalIgnoreCase), "folder names don't match, expected {0}, found {1}", new object[]
				{
					MigrationFolderName.SyncMigration,
					rootFolder.Name
				});
				disposeGuard.Success();
				result = rootFolder;
			}
			return result;
		}

		protected override IMigrationStoreObject CreateStoreObject(IMigrationDataProvider dataProvider)
		{
			return this.FindStoreObject(dataProvider, dataProvider.Folder.Id, null);
		}

		private static long GetMinimumSupportedVersion(MigrationFeature features)
		{
			long num = 1L;
			foreach (object obj in Enum.GetValues(typeof(MigrationFeature)))
			{
				MigrationFeature migrationFeature = (MigrationFeature)obj;
				if ((features & migrationFeature) == migrationFeature)
				{
					long val;
					if (!MigrationSession.FeatureVersionMap.TryGetValue(migrationFeature, out val))
					{
						throw new MigrationFeatureNotSupportedException(migrationFeature.ToString());
					}
					num = Math.Max(num, val);
				}
			}
			return num;
		}

		private int GetMaxNumberOfBatchesDefault()
		{
			return ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("MaximumNumberOfBatchesPerSession");
		}

		private void CreateInStore(IMigrationDataProvider dataProvider)
		{
			base.StoreObjectId = null;
			base.Version = -1L;
			this.CreateInStore(dataProvider, null);
		}

		private void ReinitializeSession()
		{
			MigrationLogger.Log(MigrationEventType.Verbose, "Reinitializing session {0} to be reused elsewhere", new object[]
			{
				this
			});
			base.Version = -1L;
		}

		private void AddMigrationJob(MigrationJob job)
		{
			MigrationUtil.ThrowOnNullArgument(job, "job");
			if (this.Jobs.ContainsKey(job.JobId))
			{
				return;
			}
			MigrationLogger.Log(MigrationEventType.Verbose, "Adding job {0} to the job map.", new object[]
			{
				job.JobId
			});
			this.Jobs[job.JobId] = job;
		}

		private void UpgradeVersion(IMigrationDataProvider dataProvider, long version)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			if (base.Version >= version)
			{
				return;
			}
			ExAssert.RetailAssert(this.MaximumSupportedVersion >= version, "Cannot update to a version that is greater than the maximum supported.");
			MigrationLogger.Log(MigrationEventType.Information, "upgrading from version {0} to {1}", new object[]
			{
				base.Version,
				version
			});
			base.SetVersion(dataProvider, version);
		}

		private MigrationJob GetJob(IMigrationDataProvider dataProvider, Guid jobId, bool failIfNotFound)
		{
			this.CheckQueueInitialization();
			MigrationJob migrationJob;
			if (!this.Jobs.TryGetValue(jobId, out migrationJob) && dataProvider != null)
			{
				this.RefreshJobQueueIfNeeded(dataProvider);
				this.Jobs.TryGetValue(jobId, out migrationJob);
			}
			if (migrationJob == null && failIfNotFound)
			{
				throw new MigrationJobNotFoundException(jobId);
			}
			return migrationJob;
		}

		private void RefreshJobQueueIfNeeded(IMigrationDataProvider dataProvider)
		{
			if (this.queueInitialized && this.jobCacheLastUpdated != null && DateTime.UtcNow - this.jobCacheLastUpdated.Value < MigrationSession.JobCacheInterval)
			{
				return;
			}
			MigrationLogger.Log(MigrationEventType.Verbose, "Refreshing job queue for session.", new object[0]);
			this.Jobs.Clear();
			this.jobCacheLastUpdated = new DateTime?(DateTime.UtcNow);
			foreach (MigrationJob migrationJob in MigrationJob.Get(dataProvider, this.Config))
			{
				this.Jobs[migrationJob.JobId] = migrationJob;
			}
			this.queueInitialized = true;
			MigrationLogger.Log(MigrationEventType.Verbose, "After refreshing, found {0} jobs.", new object[]
			{
				this.Jobs.Count
			});
		}

		private void CheckQueueInitialization()
		{
			MigrationUtil.AssertOrThrow(this.queueInitialized, "Expected the job queue to be initialized already.", new object[0]);
		}

		public const long MinimumSessionVersion = 1L;

		public const long MultiBatchVersion = 2L;

		public const long EndpointsVersion = 4L;

		public const long PAWVersion = 5L;

		public const long MaximumSessionVersion = 5L;

		private const string InitializationTimeKey = "InitializationTime";

		private const string MaxNumberOfBatchesKey = "MaxNumberOfBatches";

		private const string MaxConcurrentMigrationsKey = "MaxConcurrentMigrations";

		private const string EnabledFeaturesKey = "EnabledFeatures";

		private const string LastUpgradeConstraintCheckTimestampKey = "LastUpdateConstraintCheckTimestamp";

		private const string MigrationBatchFlagsKey = "MigrationBatchFlags";

		private static readonly MigrationSession.JobStage InitialSyncStage = new MigrationSession.JobStage("Initial Syncing", new MigrationJobStatus[]
		{
			MigrationJobStatus.SyncInitializing,
			MigrationJobStatus.Validating,
			MigrationJobStatus.ProvisionStarting,
			MigrationJobStatus.SyncStarting,
			MigrationJobStatus.SyncCompleting
		});

		private static readonly MigrationSession.JobStage[] RunnableJobStages = new MigrationSession.JobStage[]
		{
			new MigrationSession.JobStage("Finalizing", new MigrationJobStatus[]
			{
				MigrationJobStatus.CompletionInitializing,
				MigrationJobStatus.CompletionStarting,
				MigrationJobStatus.Completing,
				MigrationJobStatus.Completed,
				MigrationJobStatus.Removing
			}),
			MigrationSession.InitialSyncStage,
			new MigrationSession.JobStage("Incremental Syncing", new MigrationJobStatus[]
			{
				MigrationJobStatus.SyncCompleted
			})
		};

		private static readonly Dictionary<MigrationFeature, long> FeatureVersionMap = new Dictionary<MigrationFeature, long>
		{
			{
				MigrationFeature.None,
				1L
			},
			{
				MigrationFeature.MultiBatch,
				2L
			},
			{
				MigrationFeature.UpgradeBlock,
				1L
			},
			{
				MigrationFeature.Endpoints,
				4L
			},
			{
				MigrationFeature.PAW,
				5L
			}
		};

		private static readonly TimeSpan JobCacheInterval = TimeSpan.FromMinutes(1.0);

		private static HashSet<MigrationJobStatus> runnableJobStatuses;

		private DateTime? jobCacheLastUpdated;

		private bool queueInitialized;

		private class JobStage
		{
			public JobStage(string name, MigrationJobStatus[] supportedStatuses)
			{
				this.Name = name;
				this.SupportedStatuses = supportedStatuses;
			}

			public string Name { get; private set; }

			public MigrationJobStatus[] SupportedStatuses { get; private set; }

			public bool IsStatusSupported(MigrationJobStatus status)
			{
				foreach (MigrationJobStatus migrationJobStatus in this.SupportedStatuses)
				{
					if (status == migrationJobStatus)
					{
						return true;
					}
				}
				return false;
			}

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder(this.Name);
				stringBuilder.Append(" states:");
				foreach (MigrationJobStatus migrationJobStatus in this.SupportedStatuses)
				{
					stringBuilder.Append(" ");
					stringBuilder.Append(migrationJobStatus.ToString());
				}
				return stringBuilder.ToString();
			}
		}
	}
}
