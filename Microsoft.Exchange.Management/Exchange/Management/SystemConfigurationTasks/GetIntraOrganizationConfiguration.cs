using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "IntraOrganizationConfiguration")]
	public sealed class GetIntraOrganizationConfiguration : GetTaskBase<Server>
	{
		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		[Parameter(Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
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

		[Parameter(Position = 1, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public OnPremisesOrganizationIdParameter OrganizationGuid
		{
			get
			{
				return (OnPremisesOrganizationIdParameter)base.Fields["OrganizationGuid"];
			}
			set
			{
				base.Fields["OrganizationGuid"] = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			this.CheckTopologyType();
			if (this.isRunningOnDatacenter && this.Organization != null)
			{
				string acceptedDomain = this.Organization.ToString();
				base.CurrentOrganizationId = OrganizationId.FromAcceptedDomain(acceptedDomain);
			}
			TaskLogger.LogExit();
		}

		private void CheckTopologyType()
		{
			try
			{
				this.isRunningOnDatacenter = AcceptedDomainUtility.IsDatacenter;
			}
			catch (CannotDetermineExchangeModeException exception)
			{
				base.ThrowTerminatingError(exception, ErrorCategory.InvalidOperation, null);
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromCustomScopeSet(base.ScopeSet, base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, sessionSettings, 142, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\IntraOrganizationConnector\\GetIntraOrganizationConfiguration.cs");
		}

		protected override void InternalProcessRecord()
		{
			if (this.isRunningOnDatacenter)
			{
				this.SetOnlineConfiguration();
				return;
			}
			base.InternalProcessRecord();
		}

		private void SetOnlineConfiguration()
		{
			string uriString = Datacenter.IsGallatinDatacenter() ? GetIntraOrganizationConfiguration.GallatinAutoDiscoveryEndpoint : GetIntraOrganizationConfiguration.ProdAutoDiscoveryEndpoint;
			this.configuration.OnlineDiscoveryEndpoint = new Uri(uriString);
			IConfigurationSession configurationSession = base.DataSession as IConfigurationSession;
			AcceptedDomain[] source = configurationSession.Find<AcceptedDomain>(base.CurrentOrganizationId.ConfigurationUnit, QueryScope.SubTree, null, null, 0);
			List<AcceptedDomain> list = (from domain in source
			where domain.IsCoexistenceDomain
			select domain).ToList<AcceptedDomain>();
			if (list.Count == 1)
			{
				this.configuration.OnlineTargetAddress = list.First<AcceptedDomain>().DomainName.ToString();
			}
			else if (list.Count > 1)
			{
				base.ThrowTerminatingError(new MultipleCoexistenceDomainsFoundException(), ErrorCategory.InvalidData, null);
			}
			QueryFilter filter = null;
			if (this.OrganizationGuid != null)
			{
				Guid guid;
				if (Guid.TryParse(this.OrganizationGuid.ToString(), out guid))
				{
					filter = new ComparisonFilter(ComparisonOperator.Equal, OnPremisesOrganizationSchema.OrganizationGuid, guid);
				}
				else
				{
					base.ThrowTerminatingError(new ArgumentException(Strings.InvalidOrganizationGuid(this.OrganizationGuid.ToString())), ErrorCategory.InvalidData, null);
				}
			}
			OnPremisesOrganization[] source2 = configurationSession.Find<OnPremisesOrganization>(base.CurrentOrganizationId.ConfigurationUnit, QueryScope.SubTree, filter, null, 0);
			int num = source2.Count<OnPremisesOrganization>();
			if (num == 1)
			{
				this.configuration.OnPremiseTargetAddresses = source2.First<OnPremisesOrganization>().HybridDomains;
				return;
			}
			if (num > 1)
			{
				base.ThrowTerminatingError(new MultipleOnPremisesOrganizationsFoundException(), ErrorCategory.ObjectNotFound, null);
				return;
			}
			this.WriteWarning(Strings.OnPremisesConfigurationObjectNotFound);
		}

		protected override void InternalEndProcessing()
		{
			TaskLogger.LogEnter();
			if (!this.isRunningOnDatacenter)
			{
				if (!this.atLeastOneE15SP1CASFound)
				{
					base.ThrowTerminatingError(new NoCASE15SP1ServersFoundException(), ErrorCategory.ObjectNotFound, null);
				}
				if (this.configuration.OnPremiseWebServiceEndpoint == null)
				{
					base.ThrowTerminatingError(new NoWebServicesExternalUrlFoundException(), ErrorCategory.ObjectNotFound, null);
				}
				this.WriteWarning(Strings.DiscoveryEndpointWasConstructed(this.configuration.OnPremiseDiscoveryEndpoint.ToString()));
			}
			base.WriteResult(this.configuration);
			base.InternalEndProcessing();
			TaskLogger.LogExit();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			Server server = (Server)dataObject;
			bool flag = server.VersionNumber >= GetIntraOrganizationConfiguration.E15SP1MinVersion;
			bool flag2 = (server.CurrentServerRole & (ServerRole.Cafe | ServerRole.ClientAccess)) != ServerRole.None;
			bool hasMailboxRole = (server.CurrentServerRole & ServerRole.Mailbox) != ServerRole.None;
			this.SetConfigurationFlags(flag, flag2, hasMailboxRole);
			if (flag)
			{
				this.atLeastOneE15SP1CASFound = true;
			}
			if (flag2 && flag)
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 282, "WriteResult", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\IntraOrganizationConnector\\GetIntraOrganizationConfiguration.cs");
				ADObjectId descendantId = server.Id.GetDescendantId("Protocols", "HTTP", new string[0]);
				ADWebServicesVirtualDirectory[] source = topologyConfigurationSession.Find<ADWebServicesVirtualDirectory>(descendantId, QueryScope.SubTree, null, null, 10);
				IEnumerable<ADWebServicesVirtualDirectory> source2 = (from v in source
				where v.ExternalUrl != null
				select v).ToList<ADWebServicesVirtualDirectory>();
				if (source2.Any<ADWebServicesVirtualDirectory>())
				{
					Uri externalUrl = source2.First<ADWebServicesVirtualDirectory>().ExternalUrl;
					if (this.configuration.OnPremiseWebServiceEndpoint != null)
					{
						if (string.Equals(externalUrl.ToString(), this.configuration.OnPremiseDiscoveryEndpoint.ToString(), StringComparison.OrdinalIgnoreCase))
						{
							this.WriteWarning(Strings.MultipleWebServicesExternalUrlsFound);
						}
						return;
					}
					string scheme = externalUrl.Scheme;
					string authority = externalUrl.Authority;
					string uriString = string.Format("{0}://{1}/autodiscover/autodiscover.svc", scheme, authority);
					this.configuration.OnPremiseWebServiceEndpoint = externalUrl;
					this.configuration.OnPremiseDiscoveryEndpoint = new Uri(uriString);
				}
			}
			TaskLogger.LogExit();
		}

		private void SetConfigurationFlags(bool isAtLeastE15SP1, bool hasCasRole, bool hasMailboxRole)
		{
			if (isAtLeastE15SP1)
			{
				if (this.configuration.DeploymentIsCompleteIOCReady == null)
				{
					this.configuration.DeploymentIsCompleteIOCReady = new bool?(true);
				}
				if (hasCasRole && this.configuration.HasNonIOCReadyExchangeCASServerVersions == null)
				{
					this.configuration.HasNonIOCReadyExchangeCASServerVersions = new bool?(false);
				}
				if (hasMailboxRole && this.configuration.HasNonIOCReadyExchangeMailboxServerVersions == null)
				{
					this.configuration.HasNonIOCReadyExchangeMailboxServerVersions = new bool?(false);
					return;
				}
			}
			else
			{
				this.configuration.DeploymentIsCompleteIOCReady = new bool?(false);
				if (hasCasRole)
				{
					this.configuration.HasNonIOCReadyExchangeCASServerVersions = new bool?(true);
				}
				if (hasMailboxRole)
				{
					this.configuration.HasNonIOCReadyExchangeMailboxServerVersions = new bool?(true);
				}
			}
		}

		private static readonly string ProdAutoDiscoveryEndpoint = "https://autodiscover-s.outlook.com/autodiscover/autodiscover.svc";

		private static readonly string GallatinAutoDiscoveryEndpoint = "https://autodiscover-s.partner.outlook.cn/autodiscover/autodiscover.svc";

		private static readonly int E15SP1MinVersion = new ServerVersion(15, 0, 847, 0).ToInt();

		private IntraOrganizationConfiguration configuration = new IntraOrganizationConfiguration();

		private bool atLeastOneE15SP1CASFound;

		private bool isRunningOnDatacenter;
	}
}
