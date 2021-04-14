using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Migration;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Management.Migration
{
	[Cmdlet("Get", "MigrationBatch", DefaultParameterSetName = "Identity")]
	public sealed class GetMigrationBatch : MigrationGetTaskBase<MigrationBatchIdParameter, MigrationBatch>
	{
		[Parameter(Mandatory = false, ParameterSetName = "BatchesFromEndpoint")]
		public MigrationEndpointIdParameter Endpoint
		{
			get
			{
				return (MigrationEndpointIdParameter)base.Fields["Endpoint"];
			}
			set
			{
				base.Fields["Endpoint"] = value;
			}
		}

		public override string Action
		{
			get
			{
				return "GetMigrationBatch";
			}
		}

		[Parameter(Mandatory = false)]
		public MigrationBatchStatus? Status
		{
			get
			{
				return (MigrationBatchStatus?)base.Fields["Status"];
			}
			set
			{
				base.Fields["Status"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeReport
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludeReport"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["IncludeReport"] = value;
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			MigrationBatch migrationBatch = dataObject as MigrationBatch;
			if (migrationBatch != null && migrationBatch.Status == MigrationBatchStatus.Corrupted)
			{
				this.WriteWarning(Strings.MigrationBatchIsCorrupt(migrationBatch.Identity.ToString()));
			}
			base.WriteResult(dataObject);
		}

		protected override IConfigDataProvider CreateSession()
		{
			MigrationLogger.Initialize();
			MigrationLogContext.Current.Source = "Get-MigrationBatch";
			MigrationLogContext.Current.Organization = base.CurrentOrganizationId.OrganizationalUnit;
			IConfigDataProvider result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				MigrationBatchDataProvider migrationBatchDataProvider = MigrationBatchDataProvider.CreateDataProvider(this.Action, base.TenantGlobalCatalogSession, this.Status, this.partitionMailbox);
				disposeGuard.Add<MigrationBatchDataProvider>(migrationBatchDataProvider);
				if (base.Diagnostic || !string.IsNullOrEmpty(base.DiagnosticArgument))
				{
					migrationBatchDataProvider.EnableDiagnostics(base.DiagnosticArgument);
				}
				if (this.IncludeReport)
				{
					migrationBatchDataProvider.IncludeReport = true;
				}
				disposeGuard.Success();
				result = migrationBatchDataProvider;
			}
			return result;
		}

		protected override bool IsKnownException(Exception exception)
		{
			return MigrationBatchDataProvider.IsKnownException(exception) || base.IsKnownException(exception);
		}

		protected override void InternalProcessRecord()
		{
			MigrationBatchDataProvider migrationBatchDataProvider = (MigrationBatchDataProvider)base.DataSession;
			if (migrationBatchDataProvider.MigrationSession.HasJobs)
			{
				MigrationObjectTaskBase<MigrationBatchIdParameter>.RegisterMigrationBatch(this, migrationBatchDataProvider.MailboxSession, base.CurrentOrganizationId, false, false);
			}
			base.InternalProcessRecord();
		}

		private const string ParameterSetNameBatchesFromEndpoint = "BatchesFromEndpoint";
	}
}
