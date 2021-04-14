using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Deployment.XforestTenantMigration
{
	[Cmdlet("Export", "Organization", DefaultParameterSetName = "Identity")]
	public sealed class ExportOrganizationTask : GetTaskBase<ExchangeConfigurationUnit>
	{
		[Parameter(Mandatory = true, ParameterSetName = "Identity", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[Parameter(Mandatory = true, ParameterSetName = "CustomCredentials", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[ValidateNotNullOrEmpty]
		public OrganizationIdParameter Identity
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "CustomCredentials")]
		public new Fqdn DomainController
		{
			get
			{
				return base.DomainController;
			}
			set
			{
				base.DomainController = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "CustomCredentials")]
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
			if (this.Identity == null)
			{
				throw new ArgumentNullException("Identity");
			}
			TaskLogger.LogEnter();
			LocalizedString? localizedString;
			IEnumerable<ExchangeConfigurationUnit> dataObjects = base.GetDataObjects<ExchangeConfigurationUnit>(this.Identity, base.DataSession, this.RootId, base.OptionalIdentityData, out localizedString);
			OrganizationId organizationId = null;
			using (IEnumerator<ExchangeConfigurationUnit> enumerator = dataObjects.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					ExchangeConfigurationUnit exchangeConfigurationUnit = enumerator.Current;
					organizationId = exchangeConfigurationUnit.OrganizationId;
					string originatingServer = exchangeConfigurationUnit.OriginatingServer;
				}
			}
			if (!base.HasErrors && organizationId == null)
			{
				base.WriteError(new ManagementObjectNotFoundException(localizedString ?? base.GetErrorMessageObjectNotFound(this.Identity.ToString(), typeof(ExchangeConfigurationUnit).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), (ErrorCategory)1003, null);
			}
			this.directoryInfo = new DirectoryBindingInfo();
			string name = organizationId.ConfigurationUnit.ToString();
			string distinguishedName = organizationId.OrganizationalUnit.DistinguishedName;
			DirectoryObjectCollection orgUnit;
			using (SearchResultCollection subtree = this.GetSubtree(distinguishedName))
			{
				orgUnit = new DirectoryObjectCollection(distinguishedName, subtree);
			}
			string text = organizationId.ConfigurationUnit.DistinguishedName;
			ADObjectId adobjectId = new ADObjectId(text);
			text = adobjectId.AncestorDN(1).ToDNString();
			DirectoryObjectCollection configUnit;
			using (SearchResultCollection subtree2 = this.GetSubtree(text))
			{
				configUnit = new DirectoryObjectCollection(text, subtree2);
			}
			this.WriteResult(new OrganizationData(name, orgUnit, configUnit, base.RootOrgContainerId.ToString(), ADSession.GetDomainNamingContextForLocalForest().ToString(), this.site.Name));
			TaskLogger.LogExit();
		}

		protected override void InternalEndProcessing()
		{
			base.InternalEndProcessing();
			this.sessionSettings = null;
		}

		protected override IConfigDataProvider CreateSession()
		{
			IConfigurationSession tenantOrTopologyConfigurationSession;
			if (this.Credential == null && string.IsNullOrEmpty(this.DomainController))
			{
				tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(null, false, ConsistencyMode.PartiallyConsistent, this.SessionSettings, 198, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\TenantMigration\\ExportOrganizationTask.cs");
			}
			else if (this.Credential == null && !string.IsNullOrEmpty(this.DomainController))
			{
				tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.DomainController, true, ConsistencyMode.PartiallyConsistent, null, this.SessionSettings, 206, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\TenantMigration\\ExportOrganizationTask.cs");
			}
			else if (this.Credential != null && string.IsNullOrEmpty(this.DomainController))
			{
				tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(null, true, ConsistencyMode.PartiallyConsistent, this.Credential.GetNetworkCredential(), this.SessionSettings, 215, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\TenantMigration\\ExportOrganizationTask.cs");
			}
			else
			{
				tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.DomainController, true, ConsistencyMode.PartiallyConsistent, this.Credential.GetNetworkCredential(), this.SessionSettings, 224, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\TenantMigration\\ExportOrganizationTask.cs");
			}
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 232, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\TenantMigration\\ExportOrganizationTask.cs");
			this.site = topologyConfigurationSession.GetLocalSite();
			return tenantOrTopologyConfigurationSession;
		}

		protected override void InternalStateReset()
		{
			this.sessionSettings = ADSessionSettings.FromCustomScopeSet(base.ScopeSet, base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
			base.InternalStateReset();
		}

		private SearchResultCollection GetSubtree(string dn)
		{
			SearchResultCollection result;
			using (DirectoryEntry directoryEntry = this.directoryInfo.GetDirectoryEntry(this.directoryInfo.LdapBasePath + dn))
			{
				using (DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry))
				{
					directorySearcher.SearchScope = SearchScope.Subtree;
					directorySearcher.PageSize = int.MaxValue;
					result = directorySearcher.FindAll();
				}
			}
			return result;
		}

		private ADSessionSettings sessionSettings;

		private ADSite site;

		private DirectoryBindingInfo directoryInfo;
	}
}
