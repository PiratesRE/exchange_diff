using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Migration;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Management.Migration
{
	[Cmdlet("Get", "MigrationUserStatistics", DefaultParameterSetName = "Identity")]
	public sealed class GetMigrationUserStatistics : MigrationGetTaskBase<MigrationUserIdParameter, MigrationUserStatistics>
	{
		[Parameter(Mandatory = true, ParameterSetName = "Identity", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public override MigrationUserIdParameter Identity
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

		[Parameter(Mandatory = false)]
		public int? LimitSkippedItemsTo
		{
			get
			{
				return (int?)base.Fields["LimitSkippedItemsTo"];
			}
			set
			{
				base.Fields["LimitSkippedItemsTo"] = value;
			}
		}

		public override string Action
		{
			get
			{
				return "GetMigrationUserStatistics";
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			MigrationLogger.Initialize();
			MigrationLogContext.Current.Source = "Get-MigrationUserStatistics";
			MigrationLogContext.Current.Organization = base.CurrentOrganizationId.OrganizationalUnit;
			IConfigDataProvider result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				MigrationUserDataProvider migrationUserDataProvider = MigrationUserDataProvider.CreateDataProvider(this.Action, base.TenantGlobalCatalogSession, this.partitionMailbox, null);
				disposeGuard.Add<MigrationUserDataProvider>(migrationUserDataProvider);
				migrationUserDataProvider.LimitSkippedItemsTo = this.LimitSkippedItemsTo;
				migrationUserDataProvider.IncludeReport = this.IncludeReport;
				if (base.Diagnostic || !string.IsNullOrEmpty(base.DiagnosticArgument))
				{
					migrationUserDataProvider.EnableDiagnostics(base.DiagnosticArgument);
				}
				disposeGuard.Success();
				result = migrationUserDataProvider;
			}
			return result;
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			base.WriteResult(dataObject);
			MigrationUserDataProvider migrationUserDataProvider = base.DataSession as MigrationUserDataProvider;
			if (migrationUserDataProvider != null && migrationUserDataProvider.LastError != null)
			{
				this.WriteWarning(Strings.MigrationUserSubscriptionInaccessible(dataObject.Identity.ToString(), migrationUserDataProvider.LastError.LocalizedString));
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				throw new NotSupportedException();
			}
		}
	}
}
