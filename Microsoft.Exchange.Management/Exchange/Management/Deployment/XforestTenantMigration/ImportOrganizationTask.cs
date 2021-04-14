using System;
using System.Management.Automation;
using System.Net;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Deployment.XforestTenantMigration
{
	[Cmdlet("Import", "Organization", DefaultParameterSetName = "Identity")]
	public sealed class ImportOrganizationTask : GetTaskBase<ExchangeConfigurationUnit>
	{
		[Parameter(Mandatory = true, ParameterSetName = "Identity", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public OrganizationData Data
		{
			get
			{
				return (OrganizationData)base.Fields["Data"];
			}
			set
			{
				base.Fields["Data"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Identity")]
		public PSCredential Credential
		{
			get
			{
				return (PSCredential)base.Fields["Credential"];
			}
			set
			{
				base.Fields["Credential"] = value;
			}
		}

		private ADSessionSettings SessionSettings
		{
			get
			{
				return this.sessionSettings;
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				QueryFilter queryFilter = new ExistsFilter(ExchangeConfigurationUnitSchema.OrganizationalUnitLink);
				if (base.InternalFilter != null)
				{
					queryFilter = new AndFilter(new QueryFilter[]
					{
						base.InternalFilter,
						queryFilter
					});
				}
				return queryFilter;
			}
		}

		protected override void InternalProcessRecord()
		{
			if (this.Data == null)
			{
				throw new ArgumentNullException("Data");
			}
			TaskLogger.LogEnter();
			this.directoryInfo = new DirectoryBindingInfo((NetworkCredential)this.Credential);
			DirectoryObjectCollection organizationalUnit = this.Data.OrganizationalUnit;
			organizationalUnit.AddRange(this.Data.ConfigurationUnit);
			OrganizationConfigurationTree organizationConfigurationTree = OrganizationMigrationManager.CalculateImportOrder(organizationalUnit);
			organizationConfigurationTree.WriteVerboseDelegate = new Task.TaskVerboseLoggingDelegate(base.WriteVerbose);
			organizationConfigurationTree.WriteErrorDelegate = new Task.TaskErrorLoggingDelegate(base.WriteError);
			organizationConfigurationTree.WriteWarningDelegate = new Task.TaskWarningLoggingDelegate(this.WriteWarning);
			OrganizationMigrationManager.UpdateDirectoryObjectProperties(organizationalUnit, this.directoryInfo);
			OrganizationMigrationManager.Import(this.directoryInfo, organizationalUnit, organizationConfigurationTree, this.Data.SourceFqdn, this.directoryInfo.Credential.Domain, this.Data.RootOrgName, base.RootOrgContainerId.ToString(), this.Data.SourceADSite, this.site.Name);
			TaskLogger.LogExit();
		}

		protected override void InternalEndProcessing()
		{
			base.InternalEndProcessing();
			this.sessionSettings = null;
		}

		protected override IConfigDataProvider CreateSession()
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(null, false, ConsistencyMode.PartiallyConsistent, this.SessionSettings, 147, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\TenantMigration\\ImportOrganizationTask.cs");
			this.site = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(null, true, ConsistencyMode.FullyConsistent, null, ADSessionSettings.FromRootOrgScopeSet(), 153, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\TenantMigration\\ImportOrganizationTask.cs").GetLocalSite();
			return tenantOrTopologyConfigurationSession;
		}

		protected override void InternalStateReset()
		{
			this.sessionSettings = ADSessionSettings.FromCustomScopeSet(base.ScopeSet, base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
			base.InternalStateReset();
		}

		private ADSessionSettings sessionSettings;

		private ADSite site;

		private DirectoryBindingInfo directoryInfo;
	}
}
