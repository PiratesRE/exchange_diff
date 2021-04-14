using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.Management.Migration;
using Microsoft.Exchange.Migration;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Management.Migration
{
	[Cmdlet("Get", "MigrationEndpoint", DefaultParameterSetName = "Identity", SupportsShouldProcess = false)]
	public sealed class GetMigrationEndpoint : MigrationGetTaskBase<MigrationEndpointIdParameter, MigrationEndpoint>
	{
		[Parameter(Mandatory = true, ParameterSetName = "TypeFilter")]
		public MigrationType Type
		{
			get
			{
				return (MigrationType)(base.Fields["Type"] ?? MigrationType.None);
			}
			set
			{
				base.Fields["Type"] = value;
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = true, ParameterSetName = "ConnectionSettingsFilter")]
		public ExchangeConnectionSettings ConnectionSettings
		{
			get
			{
				return (ExchangeConnectionSettings)base.Fields["ConnectionSettings"];
			}
			set
			{
				base.Fields["ConnectionSettings"] = value;
			}
		}

		public override string Action
		{
			get
			{
				return "GetMigrationEndpoint";
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				string parameterSetName;
				if ((parameterSetName = base.ParameterSetName) != null)
				{
					if (parameterSetName == "Identity")
					{
						MigrationEndpointId migrationEndpointId = (this.Identity == null) ? MigrationEndpointId.Any : this.Identity.MigrationEndpointId;
						return migrationEndpointId.GetFilter();
					}
					if (parameterSetName == "ConnectionSettingsFilter")
					{
						return MigrationEndpointDataProvider.GetFilterFromConnectionSettings(this.ConnectionSettings);
					}
					if (parameterSetName == "TypeFilter")
					{
						return MigrationEndpointDataProvider.GetFilterFromEndpointType(this.Type);
					}
				}
				return null;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			MigrationLogger.Initialize();
			MigrationLogContext.Current.Source = "Get-MigrationEndpoint";
			MigrationLogContext.Current.Organization = base.CurrentOrganizationId.OrganizationalUnit;
			MigrationEndpointDataProvider migrationEndpointDataProvider = MigrationEndpointDataProvider.CreateDataProvider(this.Action, base.TenantGlobalCatalogSession, this.partitionMailbox);
			if (base.Diagnostic || !string.IsNullOrEmpty(base.DiagnosticArgument))
			{
				migrationEndpointDataProvider.EnableDiagnostics(base.DiagnosticArgument);
			}
			return migrationEndpointDataProvider;
		}

		private const string ParameterNameType = "Type";

		private const string ParameterNameConnectionSettings = "ConnectionSettings";

		private const string ParameterSetNameTypeFilter = "TypeFilter";

		private const string ParameterSetNameConnectionSettingsFilter = "ConnectionSettingsFilter";
	}
}
