using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Migration;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Management.Migration
{
	[Cmdlet("Set", "MigrationConfig", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetMigrationConfig : SetTenantADTaskBase<MigrationConfigIdParameter, MigrationConfig, MigrationConfig>
	{
		[Parameter(Mandatory = false, ParameterSetName = "Identity", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		public override MigrationConfigIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxIdParameter Partition
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Partition"];
			}
			set
			{
				base.Fields["Partition"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> MaxConcurrentMigrations
		{
			get
			{
				return (Unlimited<int>)base.Fields["MaxConcurrentMigrations"];
			}
			set
			{
				base.Fields["MaxConcurrentMigrations"] = value;
			}
		}

		[ValidateRange(0, 2147483647)]
		[Parameter(Mandatory = false)]
		public int MaxNumberOfBatches
		{
			get
			{
				return (int)base.Fields["MaxNumberOfBatches"];
			}
			set
			{
				base.Fields["MaxNumberOfBatches"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MigrationFeature Features
		{
			get
			{
				return (MigrationFeature)base.Fields["MigrationFeature"];
			}
			set
			{
				base.Fields["MigrationFeature"] = value;
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return MigrationSessionDataProvider.IsKnownException(exception) || base.IsKnownException(exception);
		}

		protected override IConfigDataProvider CreateSession()
		{
			base.CurrentOrganizationId = this.ResolveCurrentOrganization();
			this.partitionMailbox = MigrationObjectTaskBase<MigrationConfigIdParameter>.ResolvePartitionMailbox(this.Partition, base.TenantGlobalCatalogSession, base.ServerSettings, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.ErrorLoggerDelegate(base.WriteError), base.CurrentOrganizationId == OrganizationId.ForestWideOrgId && MapiTaskHelper.IsDatacenter);
			TenantPartitionHint partitionHint = TenantPartitionHint.FromOrganizationId(base.CurrentOrganizationId);
			MigrationLogger.Initialize();
			MigrationLogContext.Current.Source = "Set-MigrationConfig";
			MigrationLogContext.Current.Organization = base.CurrentOrganizationId.OrganizationalUnit;
			return MigrationSessionDataProvider.CreateDataProvider("SetMigrationConfig", MigrationHelperBase.CreateRecipientSession(partitionHint), this.partitionMailbox);
		}

		protected override void InternalValidate()
		{
			MigrationSessionDataProvider migrationSessionDataProvider = (MigrationSessionDataProvider)base.DataSession;
			if (this.IsFieldSet("MigrationFeature"))
			{
				migrationSessionDataProvider.MigrationSession.CheckFeaturesAvailableToUpgrade(this.Features);
			}
			if (this.IsFieldSet("MaxConcurrentMigrations"))
			{
				ValidationError validationError = MigrationConstraints.MaxConcurrentMigrationsConstraint.Validate(this.MaxConcurrentMigrations, MigrationBatchMessageSchema.MigrationJobMaxConcurrentMigrations, null);
				if (validationError != null)
				{
					this.WriteError(new MigrationMaxConcurrentConnectionsVerificationFailedException(this.MaxConcurrentMigrations.Value.ToString(), MigrationConstraints.MaxConcurrentMigrationsConstraint.MinimumValue.ToString(), MigrationConstraints.MaxConcurrentMigrationsConstraint.MaximumValue.ToString()));
				}
			}
			base.InternalValidate();
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetMigrationConfig(this.Identity.ToString());
			}
		}

		protected override bool IsObjectStateChanged()
		{
			return this.changed;
		}

		protected override IConfigurable PrepareDataObject()
		{
			MigrationSessionDataProvider migrationSessionDataProvider = (MigrationSessionDataProvider)base.DataSession;
			return migrationSessionDataProvider.MigrationSession.GetMigrationConfig(migrationSessionDataProvider.MailboxProvider);
		}

		protected override void InternalStateReset()
		{
			this.changed = false;
			this.DisposeSession();
			base.InternalStateReset();
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

		protected override void InternalProcessRecord()
		{
			MigrationSessionDataProvider migrationSessionDataProvider = (MigrationSessionDataProvider)base.DataSession;
			IMigrationDataProvider mailboxProvider = migrationSessionDataProvider.MailboxProvider;
			if (this.IsFieldSet("MigrationFeature") && migrationSessionDataProvider.MigrationSession.EnableFeatures(mailboxProvider, this.Features))
			{
				this.changed = true;
			}
			MigrationConfig migrationConfig = migrationSessionDataProvider.MigrationSession.GetMigrationConfig(mailboxProvider);
			if (this.IsFieldSet("MaxNumberOfBatches") && this.MaxNumberOfBatches != migrationConfig.MaxNumberOfBatches)
			{
				migrationConfig.MaxNumberOfBatches = this.MaxNumberOfBatches;
				this.changed = true;
			}
			if (this.IsFieldSet("MaxConcurrentMigrations") && this.MaxConcurrentMigrations != migrationConfig.MaxConcurrentMigrations)
			{
				migrationConfig.MaxConcurrentMigrations = this.MaxConcurrentMigrations;
				this.changed = true;
			}
			if (this.changed)
			{
				migrationSessionDataProvider.MigrationSession.SetMigrationConfig(mailboxProvider, migrationConfig);
			}
			base.InternalProcessRecord();
		}

		private bool IsFieldSet(string fieldName)
		{
			return base.Fields.IsChanged(fieldName) || base.Fields.IsModified(fieldName);
		}

		private OrganizationId ResolveCurrentOrganization()
		{
			if (this.Identity == null)
			{
				this.Identity = new MigrationConfigIdParameter();
				return OrganizationId.ForestWideOrgId;
			}
			if (this.Identity.Id != null)
			{
				return this.Identity.Id.OrganizationId;
			}
			ADObjectId rootOrgContainerIdForLocalForest = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(rootOrgContainerIdForLocalForest, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, 386, "ResolveCurrentOrganization", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Migration\\SetMigrationConfig.cs");
			tenantOrTopologyConfigurationSession.UseConfigNC = false;
			ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Identity.OrganizationIdentifier, tenantOrTopologyConfigurationSession, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Identity.OrganizationIdentifier.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Identity.OrganizationIdentifier.ToString())));
			OrganizationId organizationId = adorganizationalUnit.OrganizationId;
			this.Identity.Id = new MigrationConfigId(organizationId);
			return organizationId;
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

		private void WriteError(LocalizedException exception)
		{
			MigrationLogger.Log(MigrationEventType.Warning, MigrationLogger.GetDiagnosticInfo(exception, null), new object[0]);
			base.WriteError(exception, (ErrorCategory)1000, null);
		}

		private const string ParameterNameMaxConcurrentMigrations = "MaxConcurrentMigrations";

		private const string ParameterNameMaxNumberOfBatches = "MaxNumberOfBatches";

		private const string ParameterNameFeatures = "MigrationFeature";

		private bool changed;

		private bool disposed;

		private ADUser partitionMailbox;
	}
}
