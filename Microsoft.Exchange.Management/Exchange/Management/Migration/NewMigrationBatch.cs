using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Migration;
using Microsoft.Exchange.Migration.DataAccessLayer;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Management.Migration
{
	[Cmdlet("New", "MigrationBatch", DefaultParameterSetName = "Onboarding", SupportsShouldProcess = true)]
	public sealed class NewMigrationBatch : NewMigrationObjectTaskBase<MigrationBatch>
	{
		public NewMigrationBatch()
		{
			this.userDataProvider = new Lazy<MigrationUserDataProvider>(() => MigrationUserDataProvider.CreateDataProvider((MigrationDataProvider)((MigrationBatchDataProvider)base.DataSession).MailboxProvider, base.ExecutingUserIdentityName), LazyThreadSafetyMode.PublicationOnly);
		}

		[Parameter(Mandatory = false, ParameterSetName = "Onboarding")]
		[Parameter(Mandatory = true, ParameterSetName = "Offboarding")]
		[Parameter(Mandatory = true, ParameterSetName = "XO1")]
		[Parameter(Mandatory = true, ParameterSetName = "Local")]
		[Parameter(Mandatory = true, ParameterSetName = "LocalPublicFolder")]
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

		[Parameter(Mandatory = true)]
		public string Name
		{
			get
			{
				return (string)base.Fields["Name"];
			}
			set
			{
				base.Fields["Name"] = value;
			}
		}

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

		[Parameter(Mandatory = false, ParameterSetName = "Onboarding")]
		public MultiValuedProperty<string> ExcludeFolders
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields["ExcludeFolders"];
			}
			set
			{
				base.Fields["ExcludeFolders"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ExTimeZoneValue TimeZone
		{
			get
			{
				return (ExTimeZoneValue)base.Fields["TimeZone"];
			}
			set
			{
				base.Fields["TimeZone"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SkippableMigrationSteps[] SkipSteps
		{
			get
			{
				return ((SkippableMigrationSteps[])base.Fields["SkipSteps"]) ?? new SkippableMigrationSteps[0];
			}
			set
			{
				base.Fields["SkipSteps"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "PreexistingUserIds")]
		[Parameter(Mandatory = false, ParameterSetName = "Preexisting")]
		public SwitchParameter DisableOnCopy
		{
			get
			{
				return (SwitchParameter)(base.Fields["DisableOnCopy"] ?? new SwitchParameter(true));
			}
			set
			{
				base.Fields["DisableOnCopy"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Offboarding")]
		[Parameter(Mandatory = false, ParameterSetName = "Local")]
		[Parameter(Mandatory = false, ParameterSetName = "Onboarding")]
		public SwitchParameter DisallowExistingUsers
		{
			get
			{
				return (SwitchParameter)(base.Fields["DisallowExistingUsers"] ?? new SwitchParameter(true));
			}
			set
			{
				base.Fields["DisallowExistingUsers"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public CultureInfo Locale
		{
			get
			{
				return (CultureInfo)base.Fields["Locale"];
			}
			set
			{
				base.Fields["Locale"] = value;
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
		public SwitchParameter AutoStart
		{
			get
			{
				return (SwitchParameter)(base.Fields["AutoStart"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["AutoStart"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter AutoComplete
		{
			get
			{
				return (SwitchParameter)(base.Fields["AutoComplete"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["AutoComplete"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Onboarding")]
		[Parameter(Mandatory = false, ParameterSetName = "Offboarding")]
		[Parameter(Mandatory = false, ParameterSetName = "Local")]
		public SwitchParameter ArchiveOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields["ArchiveOnly"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ArchiveOnly"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Local")]
		public SwitchParameter Local
		{
			get
			{
				return (SwitchParameter)(base.Fields["Local"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Local"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "XO1")]
		public SwitchParameter XO1
		{
			get
			{
				return (SwitchParameter)(base.Fields["XO1"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["XO1"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Onboarding")]
		[Parameter(Mandatory = false, ParameterSetName = "Local")]
		[Parameter(Mandatory = false, ParameterSetName = "Offboarding")]
		public SwitchParameter PrimaryOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields["PrimaryOnly"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["PrimaryOnly"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Onboarding")]
		public MigrationEndpointIdParameter SourceEndpoint
		{
			get
			{
				return (MigrationEndpointIdParameter)base.Fields["SourceEndpoint"];
			}
			set
			{
				base.Fields["SourceEndpoint"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Offboarding")]
		public MigrationEndpointIdParameter TargetEndpoint
		{
			get
			{
				return (MigrationEndpointIdParameter)base.Fields["TargetEndpoint"];
			}
			set
			{
				base.Fields["TargetEndpoint"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Offboarding")]
		[Parameter(Mandatory = false, ParameterSetName = "Local")]
		[Parameter(Mandatory = false, ParameterSetName = "LocalPublicFolder")]
		[Parameter(Mandatory = false, ParameterSetName = "Onboarding")]
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

		[Parameter(Mandatory = false, ParameterSetName = "Onboarding")]
		[Parameter(Mandatory = false, ParameterSetName = "Offboarding")]
		[Parameter(Mandatory = false, ParameterSetName = "LocalPublicFolder")]
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

		[Parameter(Mandatory = false, ParameterSetName = "Onboarding")]
		[Parameter(Mandatory = false, ParameterSetName = "Local")]
		[Parameter(Mandatory = false, ParameterSetName = "Offboarding")]
		public MultiValuedProperty<string> TargetArchiveDatabases
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields["TargetArchiveDatabases"];
			}
			set
			{
				base.Fields["TargetArchiveDatabases"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Offboarding")]
		[Parameter(Mandatory = false, ParameterSetName = "Local")]
		[Parameter(Mandatory = false, ParameterSetName = "Onboarding")]
		public MultiValuedProperty<string> TargetDatabases
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields["TargetDatabases"];
			}
			set
			{
				base.Fields["TargetDatabases"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "LocalPublicFolder")]
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

		[Parameter(Mandatory = false, ParameterSetName = "Onboarding")]
		[Parameter(Mandatory = false, ParameterSetName = "Offboarding")]
		public string TargetDeliveryDomain
		{
			get
			{
				return (string)base.Fields["TargetDeliveryDomain"];
			}
			set
			{
				base.Fields["TargetDeliveryDomain"] = value;
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

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "Preexisting")]
		public MultiValuedProperty<MigrationUser> Users
		{
			get
			{
				return (MultiValuedProperty<MigrationUser>)base.Fields["Users"];
			}
			set
			{
				base.Fields["Users"] = value;
			}
		}

		[Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "PreexistingUserIds")]
		[ValidateNotNullOrEmpty]
		public MultiValuedProperty<MigrationUserIdParameter> UserIds
		{
			get
			{
				return (MultiValuedProperty<MigrationUserIdParameter>)base.Fields["UserIds"];
			}
			set
			{
				base.Fields["UserIds"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewMigrationBatch(this.Name, this.TenantName);
			}
		}

		protected override bool SkipWriteResult
		{
			get
			{
				return true;
			}
		}

		private string TenantName
		{
			get
			{
				if (!(base.CurrentOrganizationId != null) || base.CurrentOrganizationId.OrganizationalUnit == null)
				{
					return string.Empty;
				}
				return base.CurrentOrganizationId.OrganizationalUnit.Name;
			}
		}

		private bool IsNewCutoverBatch
		{
			get
			{
				return this.CSVData == null;
			}
		}

		private bool Onboarding
		{
			get
			{
				return string.Equals(base.ParameterSetName, "Onboarding");
			}
		}

		private bool Offboarding
		{
			get
			{
				return string.Equals(base.ParameterSetName, "Offboarding");
			}
		}

		private bool IsLocalPublicFolderMigration
		{
			get
			{
				return string.Equals(base.ParameterSetName, "LocalPublicFolder");
			}
		}

		private bool PreexistingCopy
		{
			get
			{
				return string.Equals(base.ParameterSetName, "Preexisting") || string.Equals(base.ParameterSetName, "PreexistingUserIds");
			}
		}

		private bool IsTenantOnboarding
		{
			get
			{
				bool flag = !string.IsNullOrEmpty(this.TenantName) || (base.ExchangeRunspaceConfig != null && base.ExchangeRunspaceConfig.IsDedicatedTenantAdmin);
				return this.Onboarding && flag;
			}
		}

		private bool FoundErrors { get; set; }

		private MigrationBatch PreexistingBatch { get; set; }

		private List<Guid> PreexistingUserIds { get; set; }

		private MigrationBatch NewBatch { get; set; }

		protected override IConfigDataProvider CreateSession()
		{
			MigrationLogger.Initialize();
			MigrationLogContext.Current.Source = "New-MigrationBatch";
			MigrationLogContext.Current.Organization = base.CurrentOrganizationId.OrganizationalUnit;
			return MigrationBatchDataProvider.CreateDataProvider("NewMigrationBatch", base.TenantGlobalCatalogSession, null, this.partitionMailbox);
		}

		protected override void InternalStateReset()
		{
			MigrationBatchDataProvider migrationBatchDataProvider = base.DataSession as MigrationBatchDataProvider;
			if (migrationBatchDataProvider != null && migrationBatchDataProvider.MailboxProvider.OrganizationId == base.CurrentOrganizationId)
			{
				return;
			}
			this.DisposeSession();
			base.InternalStateReset();
		}

		protected override void InternalProcessRecord()
		{
			bool flag = true;
			try
			{
				if (this.NewBatch == null)
				{
					this.NewBatch = this.DataObject;
					this.ValidateAndInitialize(this.NewBatch);
				}
				if (this.PreexistingCopy)
				{
					IEnumerable<MigrationUser> enumerable = new List<MigrationUser>();
					if (this.UserIds != null)
					{
						foreach (MigrationUserIdParameter migrationUserIdParameter in this.UserIds)
						{
							MigrationUser[] array = this.userDataProvider.Value.GetByUserId(migrationUserIdParameter.MigrationUserId, 10).ToArray<MigrationUser>();
							if (array.Length == 0)
							{
								base.WriteError(new MigrationUserNotFoundException(migrationUserIdParameter.ToString()));
							}
							enumerable = enumerable.Union(array);
						}
					}
					if (this.Users != null)
					{
						enumerable = enumerable.Union(this.Users);
					}
					this.InternalProcessPreexistingUsers(this.NewBatch, enumerable);
				}
				flag = false;
			}
			finally
			{
				if (flag)
				{
					this.FoundErrors = true;
				}
			}
		}

		protected override void InternalEndProcessing()
		{
			if (!this.FoundErrors && this.NewBatch != null)
			{
				this.InternalCreateJob(this.NewBatch);
				this.DataObject = this.NewBatch;
				this.WriteResult();
			}
			base.InternalEndProcessing();
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (!this.disposed)
				{
					if (disposing)
					{
						this.DisposeSession();
					}
					this.disposed = true;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return MigrationBatchDataProvider.IsKnownException(exception) || base.IsKnownException(exception);
		}

		private void ValidateAndInitialize(MigrationBatch batch)
		{
			if (this.ArchiveOnly && this.PrimaryOnly)
			{
				base.WriteError(new TaskException(Strings.ErrorIncompatibleParameters("PrimaryOnly", "ArchiveOnly")));
			}
			MigrationBatchDataProvider migrationBatchDataProvider = (MigrationBatchDataProvider)base.DataSession;
			if (!migrationBatchDataProvider.MigrationSession.Config.IsSupported(MigrationFeature.MultiBatch))
			{
				base.WriteError(new RequiredMigrationFeatureNotEnabledException(MigrationFeature.MultiBatch));
			}
			MigrationJob andValidateMigrationJob = MigrationObjectTaskBase<MigrationBatchIdParameter>.GetAndValidateMigrationJob(this, migrationBatchDataProvider, new MigrationBatchIdParameter(this.Name), false, false);
			if (andValidateMigrationJob != null)
			{
				base.WriteError(new MigrationPermanentException(Strings.MigrationJobAlreadyExists(this.Name)));
			}
			this.ValidateCommonParameters(migrationBatchDataProvider, batch);
			if (this.Onboarding)
			{
				this.ValidateOnboardingParameters(migrationBatchDataProvider.MailboxProvider, batch);
				this.ValidateOrDiscoverTargetDeliveryDomain(batch);
				if (batch.MigrationType == MigrationType.PublicFolder)
				{
					this.ValidateOnboardingPublicFolderMigrationParameters(batch);
				}
			}
			else if (this.Offboarding)
			{
				this.ValidateOffboardingParameters(batch);
				this.ValidateOrDiscoverTargetDeliveryDomain(batch);
			}
			else if (this.Local)
			{
				this.ValidateLocalParameters(batch);
			}
			else if (this.IsLocalPublicFolderMigration)
			{
				this.ValidateLocalPublicFolderMigrationParameters(batch);
			}
			else if (this.PreexistingCopy)
			{
				this.ValidatePreexistingCopy(migrationBatchDataProvider);
			}
			else if (this.XO1)
			{
				this.ValidateXO1Parameters(batch);
			}
			this.ValidateProtocolParameters(batch);
			this.ValidateSchedulingParameters(batch);
			LocalizedException ex;
			if (!this.PreexistingCopy && !migrationBatchDataProvider.MigrationSession.CanCreateNewJobOfType(batch.MigrationType, this.CSVData != null, out ex))
			{
				base.WriteError(ex ?? new MaximumNumberOfBatchesReachedException());
			}
		}

		private void ValidateOnboardingPublicFolderMigrationParameters(MigrationBatch batch)
		{
			if (base.IsFieldSet("TargetDatabases"))
			{
				base.WriteError(new ParameterNotSupportedForMigrationTypeException("TargetDatabases", batch.MigrationType.ToString()));
			}
			if (base.IsFieldSet("TargetArchiveDatabases"))
			{
				base.WriteError(new ParameterNotSupportedForMigrationTypeException("TargetArchiveDatabases", batch.MigrationType.ToString()));
			}
			if (base.IsFieldSet("DisallowExistingUsers"))
			{
				base.WriteError(new ParameterNotSupportedForMigrationTypeException("DisallowExistingUsers", batch.MigrationType.ToString()));
			}
			if (this.CSVData == null)
			{
				base.WriteError(new MigrationPermanentException(Strings.MigrationCSVRequired));
			}
		}

		private void ValidateCommonParameters(MigrationBatchDataProvider batchProvider, MigrationBatch batch)
		{
			if (!this.Onboarding && !this.Local && !this.Offboarding && !this.PreexistingCopy && !this.XO1 && !this.IsLocalPublicFolderMigration)
			{
				throw new InvalidOperationException("No direction specified for processing new migration batch.");
			}
			bool flag = batchProvider.MigrationSession.Config.IsSupported(MigrationFeature.PAW);
			IMigrationADProvider adprovider = batchProvider.MailboxProvider.ADProvider;
			if (!adprovider.IsMigrationEnabled)
			{
				base.WriteError(new MigrationPermanentException(Strings.MigrationNotEnabledForTenant(this.TenantName)));
			}
			if (this.Locale != null && this.Locale.IsNeutralCulture)
			{
				base.WriteError(new MigrationPermanentException(Strings.MigrationNeutralCultureIsInvalid));
			}
			if (this.ReportInterval != null && !flag)
			{
				base.WriteError(new MigrationPermanentException(Strings.MigrationReportIntervalParameterInvalid));
			}
			this.ResolveEndpoints(batch);
		}

		private void ValidateSchedulingParameters(MigrationBatch batch)
		{
			MigrationBatchDataProvider migrationBatchDataProvider = (MigrationBatchDataProvider)base.DataSession;
			bool flag = migrationBatchDataProvider.MigrationSession.Config.IsSupported(MigrationFeature.PAW);
			bool flag2 = batch.MigrationType == MigrationType.ExchangeRemoteMove || batch.MigrationType == MigrationType.ExchangeLocalMove;
			if (this.StartAfter != null && !flag2 && batch.MigrationType != MigrationType.ExchangeOutlookAnywhere && (batch.MigrationType != MigrationType.IMAP || !flag))
			{
				base.WriteError(new LocalizedException(Strings.MigrationStartAfterIncorrectMigrationType));
			}
			if (this.CompleteAfter != null && !flag2 && (batch.MigrationType != MigrationType.IMAP || !flag))
			{
				base.WriteError(new LocalizedException(Strings.MigrationCompleteAfterIncorrectMigrationType));
			}
			if (this.StartAfter != null && base.IsFieldSet("AutoStart"))
			{
				base.WriteError(new LocalizedException(Strings.MigrationStartAfterAndAutoStartExclusive));
			}
			if (this.CompleteAfter != null && flag2 && base.IsFieldSet("AutoComplete"))
			{
				base.WriteError(new LocalizedException(Strings.MigrationCompleteAfterAndAutoCompleteExclusive));
			}
			if (this.StartAfter != null)
			{
				RequestTaskHelper.ValidateStartAfterTime(this.StartAfter.Value.ToUniversalTime(), new Task.TaskErrorLoggingDelegate(base.WriteError), DateTime.UtcNow);
				this.AutoStart = true;
			}
			if (this.CompleteAfter != null)
			{
				RequestTaskHelper.ValidateCompleteAfterTime(this.CompleteAfter.Value.ToUniversalTime(), new Task.TaskErrorLoggingDelegate(base.WriteError), DateTime.UtcNow);
				this.AutoComplete = true;
			}
			if (this.StartAfter != null && !this.AutoComplete && flag2)
			{
				base.WriteError(new LocalizedException(Strings.MigrationStartAfterRequiresAutoComplete));
			}
			if (this.StartAfter != null && this.CompleteAfter != null)
			{
				RequestTaskHelper.ValidateStartAfterComesBeforeCompleteAfter(new DateTime?(this.StartAfter.Value.ToUniversalTime()), new DateTime?(this.CompleteAfter.Value.ToUniversalTime()), new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
		}

		private void ValidateProtocolParameters(MigrationBatch batch)
		{
			MigrationBatchDataProvider migrationBatchDataProvider = (MigrationBatchDataProvider)base.DataSession;
			migrationBatchDataProvider.MigrationSession.Config.IsSupported(MigrationFeature.PAW);
			SkippableMigrationSteps skippableMigrationSteps = MigrationBatch.ConvertStepsArrayToFlags(this.SkipSteps);
			if (skippableMigrationSteps.HasFlag(SkippableMigrationSteps.SettingTargetAddress) && (batch.MigrationType != MigrationType.ExchangeOutlookAnywhere || this.IsNewCutoverBatch))
			{
				base.WriteError(new SkipStepNotSupportedException(SkippableMigrationSteps.SettingTargetAddress.ToString()));
			}
			if (batch.MigrationType != MigrationType.ExchangeLocalMove && batch.MigrationType != MigrationType.ExchangeRemoteMove)
			{
				if (this.PrimaryOnly)
				{
					base.WriteError(new ParameterNotSupportedForMigrationTypeException("PrimaryOnly", batch.MigrationType.ToString()));
				}
				if (this.ArchiveOnly)
				{
					base.WriteError(new ParameterNotSupportedForMigrationTypeException("ArchiveOnly", batch.MigrationType.ToString()));
				}
				if (this.AutoComplete)
				{
					base.WriteError(new ParameterNotSupportedForMigrationTypeException("AutoComplete", batch.MigrationType.ToString()));
				}
			}
			if (batch.MigrationType != MigrationType.IMAP && base.IsFieldSet("ExcludeFolders"))
			{
				base.WriteError(new ParameterNotSupportedForMigrationTypeException("ExcludeFolders", batch.MigrationType.ToString()));
			}
		}

		private void ValidateLocalParameters(MigrationBatch batch)
		{
			if (batch.SourceEndpoint != null && batch.SourceEndpoint.IsRemote)
			{
				base.WriteError(new RemoteEndpointsCannotBeUsedOnLocalBatchesException(this.SourceEndpoint.RawIdentity));
			}
			if (batch.TargetEndpoint != null && batch.TargetEndpoint.IsRemote)
			{
				base.WriteError(new RemoteEndpointsCannotBeUsedOnLocalBatchesException(this.TargetEndpoint.RawIdentity));
			}
			if (this.TargetDatabases != null)
			{
				this.ValidateDatabasesExistLocally(this.TargetDatabases, "TargetDatabases");
			}
			if (this.TargetArchiveDatabases != null)
			{
				this.ValidateDatabasesExistLocally(this.TargetArchiveDatabases, "TargetArchiveDatabases");
			}
			batch.MigrationType = MigrationType.ExchangeLocalMove;
		}

		private void ValidateDatabasesExistLocally(IEnumerable<string> databases, string parameterName)
		{
			foreach (string text in databases)
			{
				if (string.IsNullOrEmpty(text))
				{
					base.WriteError(new MigrationPermanentException(Strings.ErrorParameterValueRequired(parameterName)));
				}
				MailboxDatabase database = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(DatabaseIdParameter.Parse(text), this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(text)), new LocalizedString?(Strings.ErrorDatabaseNotUnique(text)));
				MailboxTaskHelper.VerifyDatabaseIsWithinScopeForRecipientCmdlets(this.ConfigurationSession.SessionSettings, database, new Task.ErrorLoggerDelegate(base.WriteError));
			}
		}

		private void ValidateLocalPublicFolderMigrationParameters(MigrationBatch batch)
		{
			PublicFolderDatabase publicFolderDatabase = (PublicFolderDatabase)base.GetDataObject<PublicFolderDatabase>(this.SourcePublicFolderDatabase, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(this.SourcePublicFolderDatabase.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(this.SourcePublicFolderDatabase.ToString())));
			using (IMailbox mailbox = PublicFolderEndpoint.ConnectToLocalSourceDatabase(publicFolderDatabase.ExchangeObjectId))
			{
				mailbox.Disconnect();
			}
			batch.MigrationType = MigrationType.PublicFolder;
		}

		private void ValidateXO1Parameters(MigrationBatch batch)
		{
			if (batch.SourceEndpoint != null && batch.SourceEndpoint.IsRemote)
			{
				base.WriteError(new RemoteEndpointsCannotBeUsedOnLocalBatchesException(this.SourceEndpoint.RawIdentity));
			}
			if (batch.TargetEndpoint != null && batch.TargetEndpoint.IsRemote)
			{
				base.WriteError(new RemoteEndpointsCannotBeUsedOnLocalBatchesException(this.TargetEndpoint.RawIdentity));
			}
			batch.MigrationType = MigrationType.XO1;
		}

		private void ValidatePreexistingCopy(MigrationBatchDataProvider batchProvider)
		{
			if (!batchProvider.MigrationSession.Config.IsSupported(MigrationFeature.PAW))
			{
				base.WriteError(new RequiredMigrationFeatureNotEnabledException(MigrationFeature.PAW));
			}
		}

		private void ValidateOnboardingParameters(IMigrationDataProvider dataProvider, MigrationBatch batch)
		{
			if (this.IsTenantOnboarding && (this.TargetDatabases != null || this.TargetArchiveDatabases != null))
			{
				base.WriteError(new TargetDatabasesNotAllowedException());
			}
			if (batch.SourceEndpoint == null || !batch.SourceEndpoint.IsRemote)
			{
				base.WriteError(new SourceEndpointRequiredException());
			}
			batch.MigrationType = MigrationEndpointBase.GetMigrationType(batch.SourceEndpoint, batch.TargetEndpoint);
			if (this.IsNewCutoverBatch)
			{
				if (!MigrationSession.SupportsCutover(dataProvider))
				{
					base.WriteError(new CutoverMigrationNotAllowedException(this.TenantName));
				}
				else if (batch.MigrationType != MigrationType.ExchangeOutlookAnywhere)
				{
					base.WriteError(new CutoverMigrationNotSupportedForProtocolException(batch.MigrationType.ToString()));
				}
			}
			if (this.TargetDatabases != null)
			{
				this.ValidateDatabasesExistLocally(this.TargetDatabases, "TargetDatabases");
			}
			if (this.TargetArchiveDatabases != null)
			{
				this.ValidateDatabasesExistLocally(this.TargetArchiveDatabases, "TargetArchiveDatabases");
			}
		}

		private void ValidateOffboardingParameters(MigrationBatch batch)
		{
			if (this.CSVData == null)
			{
				base.WriteError(new MigrationPermanentException(Strings.MigrationCsvParameterInvalid));
			}
			if (batch.TargetEndpoint == null || !batch.TargetEndpoint.IsRemote)
			{
				base.WriteError(new TargetEndpointRequiredException());
			}
			batch.MigrationType = MigrationEndpointBase.GetMigrationType(batch.SourceEndpoint, batch.TargetEndpoint);
		}

		private void ValidateOrDiscoverTargetDeliveryDomain(MigrationBatch batch)
		{
			if (batch.MigrationType != MigrationType.ExchangeRemoteMove)
			{
				return;
			}
			if (string.IsNullOrEmpty(this.TargetDeliveryDomain))
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorParameterValueRequired("TargetDeliveryDomain")), ErrorCategory.InvalidArgument, this.Name);
			}
		}

		private void InternalCreateJob(MigrationBatch batch)
		{
			MigrationBatchDataProvider migrationBatchDataProvider = (MigrationBatchDataProvider)base.DataSession;
			bool flag = migrationBatchDataProvider.MigrationSession.Config.IsSupported(MigrationFeature.PAW);
			batch.Identity = new MigrationBatchId(this.Name);
			batch.UserTimeZone = this.TimeZone;
			batch.SubscriptionSettingsModified = (DateTime)ExDateTime.UtcNow;
			batch.ExcludedFolders = this.ExcludeFolders;
			batch.SkipSteps = MigrationBatch.ConvertStepsArrayToFlags(this.SkipSteps);
			batch.Locale = this.Locale;
			batch.AutoRetryCount = this.AutoRetryCount;
			batch.AllowUnknownColumnsInCsv = this.AllowUnknownColumnsInCsv;
			if (flag)
			{
				if (!this.AutoStart && this.StartAfter == null)
				{
					batch.Flags |= MigrationFlags.Stop;
				}
				if (this.ReportInterval != null)
				{
					batch.ReportInterval = new TimeSpan?(this.ReportInterval.Value);
				}
				if (this.AutoStart)
				{
					batch.BatchFlags |= MigrationBatchFlags.ReportInitial;
				}
			}
			else
			{
				if (this.DisallowExistingUsers)
				{
					batch.BatchFlags |= MigrationBatchFlags.DisallowExistingUsers;
				}
				if (this.AutoStart && (batch.MigrationType == MigrationType.ExchangeLocalMove || batch.MigrationType == MigrationType.ExchangeRemoteMove))
				{
					batch.BatchFlags |= MigrationBatchFlags.UseAdvancedValidation;
				}
				if (this.AutoComplete)
				{
					batch.BatchFlags |= MigrationBatchFlags.AutoComplete;
				}
				if (this.AllowIncrementalSyncs != null && !this.AllowIncrementalSyncs.Value)
				{
					batch.BatchFlags |= MigrationBatchFlags.AutoStop;
				}
			}
			ADObjectId adobjectId;
			bool flag2 = base.TryGetExecutingUserId(out adobjectId);
			batch.OwnerId = adobjectId;
			batch.DelegatedAdminOwner = string.Empty;
			if (flag2)
			{
				ADUser aduser = (ADUser)base.TenantGlobalCatalogSession.Read(adobjectId);
				if (aduser != null)
				{
					batch.SubmittedByUser = aduser.WindowsEmailAddress.ToString();
					batch.OwnerExchangeObjectId = aduser.ExchangeObjectId;
				}
			}
			else if (base.ExchangeRunspaceConfig.DelegatedPrincipal != null)
			{
				batch.SubmittedByUser = base.ExchangeRunspaceConfig.DelegatedPrincipal.UserId;
				batch.DelegatedAdminOwner = base.ExchangeRunspaceConfig.DelegatedPrincipal.Identity.Name;
			}
			else if (base.ExecutingUserOrganizationId == OrganizationId.ForestWideOrgId)
			{
				batch.SubmittedByUser = base.ExchangeRunspaceConfig.IdentityName;
			}
			else
			{
				base.WriteError(new MigrationPermanentException(Strings.MigrationOperationFailed));
			}
			batch.SubmittedByUserAdminType = MigrationObjectTaskBase<MigrationBatchIdParameter>.GetUserType(base.ExchangeRunspaceConfig, base.ExecutingUserOrganizationId);
			IEnumerable<SmtpAddress> enumerable = this.NotificationEmails;
			if (batch.NotificationEmails != null)
			{
				if (enumerable == null)
				{
					enumerable = batch.NotificationEmails;
				}
				else
				{
					enumerable = enumerable.Concat(batch.NotificationEmails);
				}
			}
			batch.NotificationEmails = MigrationObjectTaskBase<MigrationBatchIdParameter>.GetUpdatedNotificationEmails(this, base.TenantGlobalCatalogSession, enumerable);
			try
			{
				if (this.PreexistingCopy)
				{
					this.InternalEndProcessingPreexistingCopy(migrationBatchDataProvider, batch);
				}
				else if (this.Local)
				{
					this.InternalEndProcessingLocalMoves(migrationBatchDataProvider, batch);
				}
				else if (this.IsLocalPublicFolderMigration)
				{
					this.InternalEndProcessingLocalPublicFolderMigration(migrationBatchDataProvider, batch);
				}
				else if (this.XO1)
				{
					this.InternalEndProcessingXO1(migrationBatchDataProvider, batch);
				}
				else
				{
					this.InternalEndProcessingRemote(migrationBatchDataProvider, batch);
				}
				MigrationJob migrationJob = migrationBatchDataProvider.CreateBatch(batch);
				ConnectivityRec connectivityRec = new ConnectivityRec(ServerKind.Cmdlet, VersionInformation.MRS);
				migrationJob.ReportData.Append(Strings.MigrationReportJobCreated(base.ExecutingUserIdentityName, migrationJob.MigrationType), connectivityRec);
				if (!flag && this.AutoStart)
				{
					if (batch.ValidationWarnings != null && batch.ValidationWarnings.Count > 0)
					{
						this.WriteWarning(Strings.BatchValidationWarningsAutoStart(this.Name));
					}
					else
					{
						LocalizedString? localizedString;
						if (!migrationJob.SupportsStarting(out localizedString))
						{
							if (localizedString == null)
							{
								localizedString = new LocalizedString?(Strings.MigrationJobAlreadyStarted);
							}
							base.WriteError(new MigrationPermanentException(localizedString.Value));
						}
						migrationJob.ReportData.Append(Strings.MigrationReportJobAutoStarted);
						MigrationObjectTaskBase<MigrationBatchIdParameter>.StartJob(this, migrationBatchDataProvider, migrationJob, batch.NotificationEmails, batch.BatchFlags);
					}
				}
				migrationBatchDataProvider.MailboxProvider.FlushReport(migrationJob.ReportData);
			}
			finally
			{
				if (batch.CsvStream != null)
				{
					batch.CsvStream.Dispose();
					batch.CsvStream = null;
				}
			}
			MigrationObjectTaskBase<MigrationBatchIdParameter>.RegisterMigrationBatch(this, migrationBatchDataProvider.MailboxSession, base.CurrentOrganizationId, false, false);
		}

		private void InternalEndProcessingRemote(MigrationBatchDataProvider batchProvider, MigrationBatch batch)
		{
			if (this.Offboarding)
			{
				batch.BatchDirection = MigrationBatchDirection.Offboarding;
			}
			else
			{
				batch.BatchDirection = MigrationBatchDirection.Onboarding;
				batch.TargetDomainName = batchProvider.MailboxProvider.ADProvider.TenantOrganizationName;
			}
			this.ProcessOnboardingCsvParameters(batchProvider, batch);
			this.SetSubscriptionSettings(batch);
		}

		private void InternalProcessPreexistingUsers(MigrationBatch batch, IEnumerable<MigrationUser> users)
		{
			foreach (MigrationUser migrationUser in users)
			{
				if (migrationUser.BatchId == null || migrationUser.BatchId.JobId.Equals(Guid.Empty))
				{
					base.WriteError(new MigrationPermanentException(Strings.MigrationInvalidBatchIdForUser(migrationUser.Identity.ToString())));
				}
				if (this.PreexistingBatch == null)
				{
					MigrationBatchDataProvider migrationBatchDataProvider = (MigrationBatchDataProvider)base.DataSession;
					MigrationJob migrationJobByBatchId = MigrationObjectTaskBase<MigrationBatchIdParameter>.GetMigrationJobByBatchId(this, migrationBatchDataProvider, migrationUser.BatchId, false, true);
					this.PreexistingUserIds = new List<Guid>(migrationJobByBatchId.TotalCount);
					this.PreexistingBatch = MigrationJob.GetMigrationBatch(migrationBatchDataProvider.MailboxProvider, migrationBatchDataProvider.MigrationSession, migrationJobByBatchId);
					batch.MigrationType = this.PreexistingBatch.MigrationType;
					batch.NotificationEmails = this.PreexistingBatch.NotificationEmails;
					batch.BatchFlags = this.PreexistingBatch.BatchFlags;
					batch.AutoRetryCount = this.PreexistingBatch.AutoRetryCount;
					batch.SkipSteps = this.PreexistingBatch.SkipSteps;
					LocalizedException ex;
					if (!migrationBatchDataProvider.MigrationSession.CanCreateNewJobOfType(batch.MigrationType, true, out ex))
					{
						base.WriteError(ex ?? new MaximumNumberOfBatchesReachedException());
					}
				}
				else if (this.PreexistingBatch.Identity.JobId != migrationUser.BatchId.JobId)
				{
					base.WriteError(new MigrationNewBatchUsersShareBatchException());
				}
				this.PreexistingUserIds.Add(migrationUser.Identity.JobItemGuid);
			}
		}

		private void InternalEndProcessingPreexistingCopy(MigrationBatchDataProvider batchProvider, MigrationBatch batch)
		{
			int config = ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("MigrationMaximumJobItemsPerBatch");
			if (this.PreexistingUserIds.Count > config)
			{
				base.WriteError(new MigrationUserLimitExceededException(config));
			}
			batch.CsvStream = new MemoryStream((this.PreexistingUserIds.Count + 2) * 38);
			using (StreamWriter streamWriter = new StreamWriter(batch.CsvStream, Encoding.UTF8, 4096, true))
			{
				MigrationPreexistingBatchCsvSchema migrationPreexistingBatchCsvSchema = new MigrationPreexistingBatchCsvSchema();
				try
				{
					migrationPreexistingBatchCsvSchema.WriteHeader(streamWriter);
					foreach (Guid userId in this.PreexistingUserIds)
					{
						migrationPreexistingBatchCsvSchema.Write(streamWriter, userId);
					}
				}
				finally
				{
					streamWriter.Flush();
				}
			}
			batch.CsvStream.Seek(0L, SeekOrigin.Begin);
			batch.TotalCount = this.PreexistingUserIds.Count;
			batch.OriginalBatchId = new Guid?(this.PreexistingBatch.Identity.JobId);
			if (this.DisableOnCopy)
			{
				batch.BatchFlags |= MigrationBatchFlags.DisableOnCopy;
			}
			batch.ExcludedFolders = this.PreexistingBatch.ExcludedFolders;
			batch.BatchDirection = this.PreexistingBatch.BatchDirection;
			batch.SourceEndpoint = this.PreexistingBatch.SourceEndpoint;
			batch.TargetEndpoint = this.PreexistingBatch.TargetEndpoint;
			batch.TargetDatabases = this.PreexistingBatch.TargetDatabases;
			batch.TargetArchiveDatabases = this.PreexistingBatch.TargetArchiveDatabases;
			batch.PrimaryOnly = this.PreexistingBatch.PrimaryOnly;
			batch.ArchiveOnly = this.PreexistingBatch.ArchiveOnly;
			batch.TargetDeliveryDomain = this.PreexistingBatch.TargetDeliveryDomain;
			batch.TargetDomainName = this.PreexistingBatch.TargetDomainName;
			batch.BadItemLimit = this.PreexistingBatch.BadItemLimit;
			batch.LargeItemLimit = this.PreexistingBatch.LargeItemLimit;
			if (this.StartAfter != null)
			{
				batch.StartAfter = new DateTime?(this.StartAfter.Value);
				batch.StartAfterUTC = new DateTime?(this.StartAfter.Value.ToUniversalTime());
			}
			else if (this.AutoStart)
			{
				batch.StartAfter = new DateTime?(DateTime.MinValue);
				batch.StartAfterUTC = new DateTime?(DateTime.MinValue);
			}
			else
			{
				batch.StartAfter = this.PreexistingBatch.StartAfter;
				batch.StartAfterUTC = this.PreexistingBatch.StartAfterUTC;
			}
			if (this.CompleteAfter != null)
			{
				batch.CompleteAfter = new DateTime?(this.CompleteAfter.Value);
				batch.CompleteAfterUTC = new DateTime?(this.CompleteAfter.Value.ToUniversalTime());
				return;
			}
			if (this.AutoComplete)
			{
				batch.CompleteAfter = new DateTime?(DateTime.MinValue);
				batch.CompleteAfterUTC = new DateTime?(DateTime.MinValue);
				return;
			}
			batch.CompleteAfter = this.PreexistingBatch.CompleteAfter;
			batch.CompleteAfterUTC = this.PreexistingBatch.CompleteAfterUTC;
		}

		private void ProcessOnboardingCsvParameters(MigrationBatchDataProvider batchProvider, MigrationBatch batch)
		{
			if (this.CSVData == null)
			{
				this.ProcessNspiOnboardingInputParameters(batch);
				return;
			}
			MigrationCsvSchemaBase schema = MigrationCSVDataRowProvider.CreateCsvSchema(batch.MigrationType, true, this.IsTenantOnboarding);
			LocalizedException ex = MigrationObjectTaskBase<MigrationBatchIdParameter>.ProcessCsv(batchProvider.MailboxProvider, batch, schema, this.CSVData);
			if (ex != null)
			{
				base.WriteError(ex);
			}
		}

		private void ProcessNspiOnboardingInputParameters(MigrationBatch batch)
		{
			MigrationEndpointBase migrationEndpointBase = batch.SourceEndpoint;
			MigrationObjectsCount migrationObjectsCount = new MigrationObjectsCount(null);
			NspiMigrationDataReader nspiDataReader = migrationEndpointBase.GetNspiDataReader(null);
			try
			{
				migrationObjectsCount = nspiDataReader.GetCounts();
			}
			catch (MigrationTransientException exception)
			{
				base.WriteError(exception);
			}
			int config = ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("MigrationSourceExchangeRecipientMaximumCount");
			if (migrationObjectsCount.GetTotal() > config)
			{
				base.WriteError(new MigrationPermanentException(Strings.MigrationMailboxLimitExceeded(config)));
			}
			int config2 = ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("MigrationSourceExchangeMailboxMaximumCount");
			if (migrationObjectsCount.Mailboxes != null && migrationObjectsCount.Mailboxes.Value > config2)
			{
				base.WriteError(new MigrationPermanentException(Strings.MigrationMailboxLimitExceeded(config2)));
			}
			batch.TotalCount = migrationObjectsCount.GetTotal();
		}

		private void InternalEndProcessingLocalMoves(MigrationBatchDataProvider batchProvider, MigrationBatch batch)
		{
			batch.BatchDirection = MigrationBatchDirection.Local;
			this.SetSubscriptionSettings(batch);
			LocalizedException ex = MigrationObjectTaskBase<MigrationBatchIdParameter>.ProcessCsv(batchProvider.MailboxProvider, batch, new MigrationLocalMoveCsvSchema(), this.CSVData);
			if (ex != null)
			{
				base.WriteError(ex);
			}
		}

		private void InternalEndProcessingLocalPublicFolderMigration(MigrationBatchDataProvider batchProvider, MigrationBatch batch)
		{
			batch.BatchDirection = MigrationBatchDirection.Local;
			batch.BadItemLimit = this.BadItemLimit;
			batch.LargeItemLimit = this.LargeItemLimit;
			batch.SourcePublicFolderDatabase = this.SourcePublicFolderDatabase.RawIdentity;
			LocalizedException ex = MigrationObjectTaskBase<MigrationBatchIdParameter>.ProcessCsv(batchProvider.MailboxProvider, batch, new PublicFolderMigrationCsvSchema(), this.CSVData);
			if (ex != null)
			{
				base.WriteError(ex);
			}
		}

		private void InternalEndProcessingXO1(MigrationBatchDataProvider batchProvider, MigrationBatch batch)
		{
			batch.BatchDirection = MigrationBatchDirection.Local;
			LocalizedException ex = MigrationObjectTaskBase<MigrationBatchIdParameter>.ProcessCsv(batchProvider.MailboxProvider, batch, new XO1CsvSchema(), this.CSVData);
			if (ex != null)
			{
				base.WriteError(ex);
			}
		}

		private void ResolveEndpoints(MigrationBatch batch)
		{
			using (MigrationEndpointDataProvider migrationEndpointDataProvider = MigrationEndpointDataProvider.CreateDataProvider("NewMigrationBatch", base.TenantGlobalCatalogSession, this.partitionMailbox))
			{
				if (this.SourceEndpoint != null)
				{
					batch.SourceEndpoint = this.LoadEndpoint(this.SourceEndpoint, migrationEndpointDataProvider);
				}
				if (this.TargetEndpoint != null)
				{
					batch.TargetEndpoint = this.LoadEndpoint(this.TargetEndpoint, migrationEndpointDataProvider);
				}
			}
		}

		private MigrationEndpoint LoadEndpoint(MigrationEndpointIdParameter endpointId, MigrationEndpointDataProvider endpointProvider)
		{
			MigrationUtil.ThrowOnNullArgument(endpointId, "endpointId");
			List<MigrationEndpoint> list = endpointId.GetObjects<MigrationEndpoint>(null, endpointProvider).ToList<MigrationEndpoint>();
			if (list.Count > 1)
			{
				base.WriteError(new ManagementObjectAmbiguousException(Strings.MigrationEndpointIdentityAmbiguous(endpointId.RawIdentity)));
			}
			MigrationEndpoint migrationEndpoint = list.FirstOrDefault<MigrationEndpoint>();
			if (migrationEndpoint == null)
			{
				base.WriteError(new ManagementObjectNotFoundException(Strings.MigrationEndpointNotFound(endpointId.RawIdentity)));
			}
			return migrationEndpoint;
		}

		private void SetSubscriptionSettings(MigrationBatch batch)
		{
			MigrationBatchDataProvider migrationBatchDataProvider = (MigrationBatchDataProvider)base.DataSession;
			bool flag = migrationBatchDataProvider.MigrationSession.Config.IsSupported(MigrationFeature.PAW);
			batch.TargetArchiveDatabases = this.TargetArchiveDatabases;
			batch.TargetDatabases = this.TargetDatabases;
			batch.PrimaryOnly = new bool?(this.PrimaryOnly);
			batch.ArchiveOnly = new bool?(this.ArchiveOnly);
			batch.TargetDeliveryDomain = this.TargetDeliveryDomain;
			batch.BadItemLimit = this.BadItemLimit;
			if (!this.Local)
			{
				batch.LargeItemLimit = this.LargeItemLimit;
			}
			if (this.StartAfter != null)
			{
				batch.StartAfter = this.StartAfter;
				batch.StartAfterUTC = new DateTime?(this.StartAfter.Value.ToUniversalTime());
			}
			else if (flag && this.AutoStart)
			{
				batch.StartAfter = new DateTime?(DateTime.MinValue);
				batch.StartAfterUTC = new DateTime?(DateTime.MinValue);
			}
			if (this.CompleteAfter != null)
			{
				batch.CompleteAfter = this.CompleteAfter;
				batch.CompleteAfterUTC = new DateTime?(this.CompleteAfter.Value.ToUniversalTime());
				return;
			}
			if (flag)
			{
				if (this.AutoComplete)
				{
					batch.CompleteAfter = new DateTime?(DateTime.MinValue);
					batch.CompleteAfterUTC = new DateTime?(DateTime.MinValue);
					return;
				}
				batch.CompleteAfter = new DateTime?(DateTime.MaxValue);
				batch.CompleteAfterUTC = new DateTime?(DateTime.MaxValue);
			}
		}

		private void DisposeSession()
		{
			IDisposable disposable = base.DataSession as IDisposable;
			if (disposable != null)
			{
				MigrationLogger.Close();
				disposable.Dispose();
			}
		}

		private const string ParameterSetLocal = "Local";

		private const string ParameterSetLocalPublicFolder = "LocalPublicFolder";

		private const string ParameterSetXO1 = "XO1";

		private const string ParameterSetOffboarding = "Offboarding";

		private const string ParameterSetOnboarding = "Onboarding";

		private const string ParameterSetPreexisting = "Preexisting";

		private const string ParameterSetPreexistingUserIds = "PreexistingUserIds";

		private const string ParameterAllowIncrementalSyncs = "AllowIncrementalSyncs";

		private const string ParameterNameTargetArchiveDatabases = "TargetArchiveDatabases";

		private const string ParameterNameSourcePublicFolderDatabase = "SourcePublicFolderDatabase";

		private const string ParameterNameTargetDatabases = "TargetDatabases";

		private const string ParameterArchiveOnly = "ArchiveOnly";

		private const string ParameterBadItemLimit = "BadItemLimit";

		private const string ParameterLargeItemLimit = "LargeItemLimit";

		private const string ParameterLocal = "Local";

		private const string ParameterXO1 = "XO1";

		private const string ParameterPrimaryOnly = "PrimaryOnly";

		private const string ParameterSourceEndpoint = "SourceEndpoint";

		private const string ParameterTargetEndpoint = "TargetEndpoint";

		private const string ParameterTargetDeliveryDomain = "TargetDeliveryDomain";

		private const string ParameterNameExcludeFolders = "ExcludeFolders";

		private const string ParameterAutoComplete = "AutoComplete";

		private const string ParameterAutoStart = "AutoStart";

		private const string ParameterStartAfter = "StartAfter";

		private const string ParameterCompleteAfter = "CompleteAfter";

		private const string ParameterReportInterval = "ReportInterval";

		private const string ParameterUsers = "Users";

		private const string ParameterUserIds = "UserIds";

		private const string ParameterCSVData = "dataBlob";

		private const string ParameterDisallowExistingUsers = "DisallowExistingUsers";

		private const string ParameterAllowUnknownColumnsInCsv = "AllowUnknownColumnsInCsv";

		private bool disposed;

		private readonly Lazy<MigrationUserDataProvider> userDataProvider;
	}
}
