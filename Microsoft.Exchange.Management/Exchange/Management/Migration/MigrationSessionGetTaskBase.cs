using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Migration;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Management.Migration
{
	public abstract class MigrationSessionGetTaskBase<TIdentityParameter, TDataObject> : MigrationGetTaskBase<TIdentityParameter, TDataObject> where TIdentityParameter : IIdentityParameter, new() where TDataObject : IConfigurable, new()
	{
		protected new OrganizationIdParameter Organization
		{
			get
			{
				return base.Organization;
			}
			set
			{
				base.Organization = value;
			}
		}

		protected override void InternalStateReset()
		{
			base.CurrentOrganizationId = this.ResolveCurrentOrganization();
			this.partitionMailbox = MigrationObjectTaskBase<TIdentityParameter>.ResolvePartitionMailbox(base.Partition, base.TenantGlobalCatalogSession, base.ServerSettings, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.ErrorLoggerDelegate(base.WriteError), base.CurrentOrganizationId == OrganizationId.ForestWideOrgId && MapiTaskHelper.IsDatacenter);
			base.InternalStateReset();
		}

		protected override IConfigDataProvider CreateSession()
		{
			MigrationLogger.Initialize();
			MigrationLogContext.Current.Source = "Get-MigrationSession";
			MigrationLogContext.Current.Organization = base.CurrentOrganizationId.OrganizationalUnit;
			MigrationSessionDataProvider migrationSessionDataProvider = MigrationSessionDataProvider.CreateDataProvider(this.Action, base.TenantGlobalCatalogSession, this.partitionMailbox);
			if (base.Diagnostic || !string.IsNullOrEmpty(base.DiagnosticArgument))
			{
				migrationSessionDataProvider.EnableDiagnostics(base.DiagnosticArgument);
			}
			return migrationSessionDataProvider;
		}

		protected override bool IsKnownException(Exception exception)
		{
			return MigrationSessionDataProvider.IsKnownException(exception) || base.IsKnownException(exception);
		}

		protected override void InternalProcessRecord()
		{
			MigrationSessionDataProvider migrationSessionDataProvider = (MigrationSessionDataProvider)base.DataSession;
			if (migrationSessionDataProvider.MigrationSession.HasJobs)
			{
				MigrationObjectTaskBase<MigrationBatchIdParameter>.RegisterMigrationBatch(this, migrationSessionDataProvider.MailboxSession, base.CurrentOrganizationId, false, false);
			}
			base.InternalProcessRecord();
		}
	}
}
