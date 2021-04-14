using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Migration;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Management.Migration
{
	public abstract class MigrationOrganizationTaskBase : DataAccessTask<ADOrganizationalUnit>
	{
		[Parameter(Mandatory = false)]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
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

		internal MigrationBatchDataProvider BatchProvider
		{
			get
			{
				return this.batchDataProvider.Value;
			}
		}

		internal MigrationDataProvider DataProvider { get; set; }

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			if (this.Organization != null)
			{
				base.CurrentOrganizationId = this.GetCurrentOrganizationId();
			}
			this.partitionMailbox = MigrationObjectTaskBase<OrganizationIdParameter>.ResolvePartitionMailbox(this.Partition, base.TenantGlobalCatalogSession, base.ServerSettings, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.ErrorLoggerDelegate(base.WriteError), base.CurrentOrganizationId == OrganizationId.ForestWideOrgId && MapiTaskHelper.IsDatacenter);
		}

		protected override bool IsKnownException(Exception exception)
		{
			return MigrationBatchDataProvider.IsKnownException(exception) || base.IsKnownException(exception);
		}

		protected override IConfigDataProvider CreateSession()
		{
			MigrationLogger.Initialize();
			MigrationLogContext.Current.Source = base.GetType().Name;
			if (base.CurrentOrganizationId != null)
			{
				MigrationLogContext.Current.Organization = base.CurrentOrganizationId.OrganizationalUnit;
			}
			this.initialized = true;
			this.DataProvider = MigrationDataProvider.CreateProviderForMigrationMailbox(base.GetType().Name, base.TenantGlobalCatalogSession, this.partitionMailbox);
			this.batchDataProvider = new Lazy<MigrationBatchDataProvider>(() => new MigrationBatchDataProvider(this.DataProvider, null));
			MigrationADProvider migrationADProvider = (MigrationADProvider)this.DataProvider.ADProvider;
			return migrationADProvider.RecipientSession;
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (!this.disposed && disposing)
				{
					this.DisposeSession();
					this.disposed = true;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		protected override void InternalStateReset()
		{
			if (this.initialized)
			{
				this.DisposeSession();
			}
			base.InternalStateReset();
		}

		protected virtual void WriteError(LocalizedException exception)
		{
			MigrationLogger.Log(MigrationEventType.Warning, MigrationLogger.GetDiagnosticInfo(exception, null), new object[0]);
			base.WriteError(exception, (ErrorCategory)1000, null);
		}

		private OrganizationId GetCurrentOrganizationId()
		{
			OrganizationId organizationId;
			if (this.Organization != null)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, sessionSettings, 236, "GetCurrentOrganizationId", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Migration\\MigrationOrganizationTaskBase.cs");
				tenantOrTopologyConfigurationSession.UseConfigNC = false;
				tenantOrTopologyConfigurationSession.UseGlobalCatalog = false;
				ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Organization, tenantOrTopologyConfigurationSession, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())));
				organizationId = adorganizationalUnit.OrganizationId;
			}
			else
			{
				organizationId = base.CurrentOrganizationId;
			}
			if (organizationId != null && OrganizationId.ForestWideOrgId.Equals(organizationId) && MapiTaskHelper.IsDatacenter)
			{
				organizationId = null;
			}
			return organizationId;
		}

		private void DisposeSession()
		{
			IDisposable disposable = base.DataSession as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
			if (this.DataProvider != null)
			{
				this.DataProvider.Dispose();
				this.DataProvider = null;
			}
			if (this.batchDataProvider.IsValueCreated)
			{
				this.batchDataProvider.Value.Dispose();
			}
			MigrationLogger.Close();
		}

		protected ADUser partitionMailbox;

		private bool disposed;

		private bool initialized;

		private Lazy<MigrationBatchDataProvider> batchDataProvider;
	}
}
