using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Migration;
using Microsoft.Exchange.Migration.DataAccessLayer;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Management.Migration
{
	[Cmdlet("Set", "MigrationBatch", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetMigrationBatch : SetMigrationObjectTaskBase<MigrationBatchIdParameter, MigrationBatch, MigrationBatch>
	{
		[Parameter(Mandatory = false)]
		public bool? AllowIncrementalSyncs
		{
			get
			{
				return (bool?)base.Fields["AllowIncrementalSyncs"];
			}
			set
			{
				base.Fields["AllowIncrementalSyncs"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? AutoRetryCount
		{
			get
			{
				return (int?)base.Fields["AutoRetryCount"];
			}
			set
			{
				base.Fields["AutoRetryCount"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte[] CSVData
		{
			get
			{
				return (byte[])base.Fields["dataBlob"];
			}
			set
			{
				base.Fields["dataBlob"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowUnknownColumnsInCsv
		{
			get
			{
				return (bool)(base.Fields["AllowUnknownColumnsInCsv"] ?? false);
			}
			set
			{
				base.Fields["AllowUnknownColumnsInCsv"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
		public MultiValuedProperty<SmtpAddress> NotificationEmails
		{
			get
			{
				return (MultiValuedProperty<SmtpAddress>)base.Fields["NotificationEmails"];
			}
			set
			{
				base.Fields["NotificationEmails"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> BadItemLimit
		{
			get
			{
				return (Unlimited<int>)(base.Fields["BadItemLimit"] ?? new Unlimited<int>(0));
			}
			set
			{
				base.Fields["BadItemLimit"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> LargeItemLimit
		{
			get
			{
				return (Unlimited<int>)(base.Fields["LargeItemLimit"] ?? new Unlimited<int>(0));
			}
			set
			{
				base.Fields["LargeItemLimit"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? StartAfter
		{
			get
			{
				return (DateTime?)base.Fields["StartAfter"];
			}
			set
			{
				base.Fields["StartAfter"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? CompleteAfter
		{
			get
			{
				return (DateTime?)base.Fields["CompleteAfter"];
			}
			set
			{
				base.Fields["CompleteAfter"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public TimeSpan? ReportInterval
		{
			get
			{
				return (TimeSpan?)base.Fields["ReportInterval"];
			}
			set
			{
				base.Fields["ReportInterval"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool UseAdvancedValidation
		{
			get
			{
				return (bool)(base.Fields["UseAdvancedValidation"] ?? false);
			}
			set
			{
				base.Fields["UseAdvancedValidation"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DatabaseIdParameter SourcePublicFolderDatabase
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["SourcePublicFolderDatabase"];
			}
			set
			{
				base.Fields["SourcePublicFolderDatabase"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (this.DataObject != null)
				{
					return Strings.ConfirmationMessageSetMigrationBatch(this.DataObject.Identity.ToString());
				}
				return Strings.ConfirmationMessageSetMigrationBatch(this.Identity.ToString());
			}
		}

		protected override bool IsObjectStateChanged()
		{
			return this.changed;
		}

		protected override IConfigDataProvider CreateSession()
		{
			MigrationLogger.Initialize();
			MigrationLogContext.Current.Source = "Set-MigrationBatch";
			MigrationLogContext.Current.Organization = base.CurrentOrganizationId.OrganizationalUnit;
			return MigrationBatchDataProvider.CreateDataProvider("SetMigrationBatch", base.TenantGlobalCatalogSession, null, this.partitionMailbox);
		}

		protected override void InternalStateReset()
		{
			this.DisposeSession();
			base.InternalStateReset();
		}

		protected override void InternalValidate()
		{
			MigrationBatchDataProvider migrationBatchDataProvider = (MigrationBatchDataProvider)base.DataSession;
			bool flag = migrationBatchDataProvider.MigrationSession.Config.IsSupported(MigrationFeature.PAW);
			migrationBatchDataProvider.MigrationJob = MigrationObjectTaskBase<MigrationBatchIdParameter>.GetAndValidateMigrationJob(this, (MigrationBatchDataProvider)base.DataSession, this.Identity, true, true);
			if (migrationBatchDataProvider.MigrationJob == null)
			{
				MigrationObjectTaskBase<MigrationBatchIdParameter>.WriteJobNotFoundError(this, this.Identity.RawIdentity);
			}
			LocalizedString? localizedString;
			if (!migrationBatchDataProvider.MigrationJob.SupportsSetting(out localizedString))
			{
				if (localizedString == null)
				{
					localizedString = new LocalizedString?(Strings.MigrationOperationFailed);
				}
				base.WriteError(new MigrationPermanentException(localizedString.Value));
				migrationBatchDataProvider.MigrationJob = null;
			}
			if (this.CSVData != null && !migrationBatchDataProvider.MigrationJob.SupportsAppendingUsers(out localizedString))
			{
				if (localizedString == null)
				{
					localizedString = new LocalizedString?(Strings.MigrationOperationFailed);
				}
				base.WriteError(new MigrationPermanentException(localizedString.Value));
				migrationBatchDataProvider.MigrationJob = null;
			}
			if (this.AllowIncrementalSyncs != null)
			{
				if (migrationBatchDataProvider.MigrationJob.Status == MigrationJobStatus.Stopped && !migrationBatchDataProvider.MigrationJob.AutoStop && this.AllowIncrementalSyncs.Value)
				{
					base.WriteError(new MigrationPermanentException(Strings.MigrationPleaseUseStartForReenablingIncremental));
				}
				else if (migrationBatchDataProvider.MigrationJob.Status == MigrationJobStatus.SyncCompleted && !this.AllowIncrementalSyncs.Value)
				{
					base.WriteError(new MigrationPermanentException(Strings.MigrationPleaseUseStopForDisablingIncremental));
				}
				if (migrationBatchDataProvider.MigrationJob.Status != MigrationJobStatus.Created && !MigrationJobStage.Sync.IsStatusSupported(migrationBatchDataProvider.MigrationJob.Status) && !MigrationJobStage.Incremental.IsStatusSupported(migrationBatchDataProvider.MigrationJob.Status))
				{
					base.WriteError(new MigrationPermanentException(Strings.MigrationAutoStopForInProgressOnly));
				}
				if (this.AllowIncrementalSyncs.Value != migrationBatchDataProvider.MigrationJob.AutoStop)
				{
					base.WriteError(new MigrationPermanentException(Strings.MigrationAutoStopAlreadySet));
				}
			}
			if (migrationBatchDataProvider.MigrationJob.MigrationType == MigrationType.ExchangeLocalMove && base.IsFieldSet("LargeItemLimit"))
			{
				base.WriteError(new MigrationPermanentException(Strings.MigrationNoLargeItemLimitForLocalBatches));
			}
			if (this.ReportInterval != null && !flag)
			{
				base.WriteError(new MigrationPermanentException(Strings.MigrationReportIntervalParameterInvalid));
			}
			if (base.IsFieldSet("SourcePublicFolderDatabase"))
			{
				this.ValidateSourcePublicFolderDatabase(migrationBatchDataProvider.MigrationJob);
			}
			this.ValidateSchedulingParameters(migrationBatchDataProvider.MigrationJob);
			base.InternalValidate();
		}

		protected override void InternalProcessRecord()
		{
			MigrationBatchDataProvider batchProvider = (MigrationBatchDataProvider)base.DataSession;
			bool flag = batchProvider.MigrationSession.Config.IsSupported(MigrationFeature.PAW);
			bool flag2 = false;
			bool updateEmails = false;
			bool flag3 = false;
			bool flag4 = false;
			if (flag && this.ReportInterval != null)
			{
				this.DataObject.ReportInterval = new TimeSpan?(this.ReportInterval.Value);
				flag2 = true;
			}
			if (base.Fields.IsModified("NotificationEmails"))
			{
				MultiValuedProperty<SmtpAddress> updatedNotificationEmails = MigrationObjectTaskBase<MigrationBatchIdParameter>.GetUpdatedNotificationEmails(this, base.TenantGlobalCatalogSession, this.NotificationEmails);
				if (updatedNotificationEmails != null && updatedNotificationEmails.Count != 0)
				{
					this.DataObject.NotificationEmails = updatedNotificationEmails;
					flag2 = true;
					updateEmails = true;
				}
			}
			if (base.Fields.IsModified("AllowUnknownColumnsInCsv"))
			{
				this.DataObject.AllowUnknownColumnsInCsv = this.AllowUnknownColumnsInCsv;
				flag2 = true;
			}
			if (this.CSVData != null)
			{
				this.InternalProcessCsv(batchProvider);
				flag2 = true;
			}
			if (this.AllowIncrementalSyncs != null)
			{
				if (!this.AllowIncrementalSyncs.Value)
				{
					this.DataObject.BatchFlags |= MigrationBatchFlags.AutoStop;
				}
				else
				{
					this.DataObject.BatchFlags &= ~MigrationBatchFlags.AutoStop;
				}
				flag2 = true;
			}
			if (base.IsFieldSet("UseAdvancedValidation"))
			{
				if (this.UseAdvancedValidation)
				{
					this.DataObject.BatchFlags |= MigrationBatchFlags.UseAdvancedValidation;
				}
				else
				{
					this.DataObject.BatchFlags &= ~MigrationBatchFlags.UseAdvancedValidation;
				}
				flag2 = true;
			}
			if (this.AutoRetryCount != null)
			{
				this.DataObject.AutoRetryCount = this.AutoRetryCount;
				flag2 = true;
			}
			if (base.IsFieldSet("BadItemLimit") && !this.BadItemLimit.Equals(this.DataObject.BadItemLimit))
			{
				this.DataObject.BadItemLimit = this.BadItemLimit;
				flag3 = true;
				flag2 = true;
			}
			if (base.IsFieldSet("LargeItemLimit") && !this.LargeItemLimit.Equals(this.DataObject.LargeItemLimit))
			{
				this.DataObject.LargeItemLimit = this.LargeItemLimit;
				flag3 = true;
				flag2 = true;
			}
			if (base.IsFieldSet("StartAfter"))
			{
				this.DataObject.StartAfter = this.StartAfter;
				this.DataObject.StartAfterUTC = (DateTime?)MigrationHelper.GetUniversalDateTime((ExDateTime?)this.StartAfter);
				flag3 = true;
				flag2 = true;
				flag4 = true;
			}
			if (base.IsFieldSet("CompleteAfter"))
			{
				DateTime? completeAfterUTC = (DateTime?)MigrationHelper.GetUniversalDateTime((ExDateTime?)this.CompleteAfter);
				this.DataObject.CompleteAfter = this.CompleteAfter;
				this.DataObject.CompleteAfterUTC = completeAfterUTC;
				flag3 = true;
				flag2 = true;
				flag4 = true;
			}
			if (base.IsFieldSet("SourcePublicFolderDatabase"))
			{
				this.DataObject.SourcePublicFolderDatabase = this.SourcePublicFolderDatabase.RawIdentity;
				flag2 = true;
				flag4 = true;
				flag3 = true;
			}
			if (flag3)
			{
				this.DataObject.SubscriptionSettingsModified = (DateTime)ExDateTime.UtcNow;
			}
			if (flag2)
			{
				MigrationHelper.RunUpdateOperation(delegate
				{
					batchProvider.MigrationJob.UpdateJob(batchProvider.MailboxProvider, updateEmails, this.DataObject);
				});
				batchProvider.MigrationJob.ReportData.Append(Strings.MigrationReportJobModifiedByUser(base.ExecutingUserIdentityName));
				this.changed = true;
				if (flag4)
				{
					MigrationObjectTaskBase<MigrationBatchIdParameter>.RegisterMigrationBatch(this, batchProvider.MailboxSession, base.CurrentOrganizationId, false, false);
				}
			}
			base.InternalProcessRecord();
		}

		private void InternalProcessCsv(MigrationBatchDataProvider batchProvider)
		{
			MigrationCsvSchemaBase migrationCsvSchemaBase = MigrationCSVDataRowProvider.CreateCsvSchema(batchProvider.MigrationJob);
			if (migrationCsvSchemaBase == null)
			{
				base.WriteError(new MigrationPermanentException(Strings.MigrationCSVNotAllowed));
			}
			LocalizedException ex = MigrationObjectTaskBase<MigrationBatchIdParameter>.ProcessCsv(((MigrationBatchDataProvider)base.DataSession).MailboxProvider, this.DataObject, migrationCsvSchemaBase, this.CSVData);
			if (ex != null)
			{
				base.WriteError(ex);
			}
		}

		private void ValidateSchedulingParameters(MigrationJob migrationJob)
		{
			DateTime? dateTime = null;
			DateTime? dateTime2 = null;
			bool flag = false;
			bool flag2 = false;
			MigrationType migrationType = migrationJob.MigrationType;
			if (migrationType <= MigrationType.ExchangeOutlookAnywhere)
			{
				if (migrationType != MigrationType.IMAP)
				{
					if (migrationType == MigrationType.ExchangeOutlookAnywhere)
					{
						ExchangeJobSubscriptionSettings exchangeJobSubscriptionSettings = migrationJob.SubscriptionSettings as ExchangeJobSubscriptionSettings;
						if (exchangeJobSubscriptionSettings != null)
						{
							dateTime = (DateTime?)exchangeJobSubscriptionSettings.StartAfter;
						}
						flag = true;
					}
				}
				else if (migrationJob.IsPAW)
				{
					IMAPPAWJobSubscriptionSettings imappawjobSubscriptionSettings = migrationJob.SubscriptionSettings as IMAPPAWJobSubscriptionSettings;
					if (imappawjobSubscriptionSettings != null)
					{
						dateTime = (DateTime?)imappawjobSubscriptionSettings.StartAfter;
						dateTime2 = (DateTime?)imappawjobSubscriptionSettings.CompleteAfter;
					}
					flag = true;
					flag2 = true;
				}
			}
			else if (migrationType == MigrationType.ExchangeRemoteMove || migrationType == MigrationType.ExchangeLocalMove)
			{
				MoveJobSubscriptionSettings moveJobSubscriptionSettings = migrationJob.SubscriptionSettings as MoveJobSubscriptionSettings;
				if (moveJobSubscriptionSettings != null)
				{
					dateTime = (DateTime?)moveJobSubscriptionSettings.StartAfter;
					dateTime2 = (DateTime?)moveJobSubscriptionSettings.CompleteAfter;
				}
				flag = true;
				flag2 = true;
			}
			if (base.IsFieldSet("StartAfter") && !flag)
			{
				base.WriteError(new LocalizedException(Strings.MigrationStartAfterIncorrectMigrationType));
			}
			if (base.IsFieldSet("CompleteAfter") && !flag2)
			{
				base.WriteError(new LocalizedException(Strings.MigrationCompleteAfterIncorrectMigrationType));
			}
			bool flag3 = !migrationJob.IsPAW && !migrationJob.AutoComplete;
			if (base.IsFieldSet("StartAfter") && flag3)
			{
				base.WriteError(new LocalizedException(Strings.MigrationStartAfterScheduledBatchesOnly));
			}
			if (base.IsFieldSet("CompleteAfter") && flag3)
			{
				base.WriteError(new LocalizedException(Strings.MigrationCompleteAfterScheduledBatchesOnly));
			}
			if (base.IsFieldSet("StartAfter"))
			{
				if (migrationJob.Status != MigrationJobStatus.Created && migrationJob.Status != MigrationJobStatus.Failed && !migrationJob.IsPAW)
				{
					base.WriteError(new LocalizedException(Strings.MigrationStartAfterIncorrectState(migrationJob.Status.ToString())));
				}
				if (this.StartAfter != null)
				{
					RequestTaskHelper.ValidateStartAfterTime(this.StartAfter.Value.ToUniversalTime(), new Task.TaskErrorLoggingDelegate(base.WriteError), DateTime.UtcNow);
				}
			}
			if (base.IsFieldSet("CompleteAfter"))
			{
				if (migrationJob.Status != MigrationJobStatus.Created && migrationJob.Status != MigrationJobStatus.Failed && migrationJob.Status != MigrationJobStatus.SyncInitializing && migrationJob.Status != MigrationJobStatus.SyncStarting && migrationJob.Status != MigrationJobStatus.SyncCompleting && migrationJob.Status != MigrationJobStatus.SyncCompleted && migrationJob.Status != MigrationJobStatus.ProvisionStarting && migrationJob.Status != MigrationJobStatus.Validating && migrationJob.Status != MigrationJobStatus.Stopped && !migrationJob.IsPAW)
				{
					base.WriteError(new LocalizedException(Strings.MigrationCompleteAfterIncorrectState));
				}
				if (this.CompleteAfter != null)
				{
					RequestTaskHelper.ValidateCompleteAfterTime(this.CompleteAfter.Value.ToUniversalTime(), new Task.TaskErrorLoggingDelegate(base.WriteError), DateTime.UtcNow);
				}
				if (dateTime2 != null && DateTime.UtcNow.AddHours(1.0) > dateTime2.Value.ToUniversalTime())
				{
					this.WriteWarning(Strings.MigrationSettingCompeteAfterWithCurrentCompleteAfterInLessThanOneHour);
				}
				if (dateTime2 != null && this.CompleteAfter != null && this.CompleteAfter.Value.ToUniversalTime() < dateTime2.Value.ToUniversalTime())
				{
					this.WriteWarning(Strings.MigrationCompleteAfterChangedToEarlierTime);
				}
			}
			if ((base.IsFieldSet("StartAfter") || base.IsFieldSet("CompleteAfter")) && (this.StartAfter != null || dateTime != null) && (this.CompleteAfter != null || dateTime2 != null))
			{
				DateTime? dateTime3 = this.StartAfter ?? dateTime;
				DateTime? dateTime4 = this.CompleteAfter ?? dateTime2;
				RequestTaskHelper.ValidateStartAfterComesBeforeCompleteAfter(new DateTime?(dateTime3.Value.ToUniversalTime()), new DateTime?(dateTime4.Value.ToUniversalTime()), new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
		}

		private void ValidateSourcePublicFolderDatabase(MigrationJob job)
		{
			if (job.MigrationType != MigrationType.PublicFolder || job.JobDirection != MigrationBatchDirection.Local)
			{
				base.WriteError(new MigrationPermanentException(Strings.ErrorInvalidBatchParameter("SourcePublicFolderDatabase", job.MigrationType.ToString(), job.JobDirection.ToString())));
			}
			PublicFolderDatabase publicFolderDatabase = (PublicFolderDatabase)base.GetDataObject<PublicFolderDatabase>(this.SourcePublicFolderDatabase, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(this.SourcePublicFolderDatabase.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(this.SourcePublicFolderDatabase.ToString())));
			using (IMailbox mailbox = PublicFolderEndpoint.ConnectToLocalSourceDatabase(publicFolderDatabase.ExchangeObjectId))
			{
				mailbox.Disconnect();
			}
		}

		private const string ParameterBadItemLimit = "BadItemLimit";

		private const string ParameterLargeItemLimit = "LargeItemLimit";

		private const string ParameterStartAfter = "StartAfter";

		private const string ParameterCompleteAfter = "CompleteAfter";

		private const string ParameterReportInterval = "ReportInterval";

		private const string ParameterNotificationEmails = "NotificationEmails";

		private const string ParameterAllowUnknownColumnsInCsv = "AllowUnknownColumnsInCsv";

		private const string ParameterUseAdvancedValidation = "UseAdvancedValidation";

		private const string ParameterNameSourcePublicFolderDatabase = "SourcePublicFolderDatabase";

		private bool changed;
	}
}
