using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Hybrid.Entity;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal class PowerShellTenantSession : PowerShellCommonSession, ITenantSession, ICommonSession, IDisposable
	{
		public PowerShellTenantSession(ILogger logger, string targetServer, PSCredential credentials) : base(logger, targetServer, PowershellConnectionType.Tenant, credentials)
		{
		}

		public void EnableOrganizationCustomization()
		{
			base.RemotePowershellSession.RunCommand("Enable-OrganizationCustomization", null);
		}

		public IEnumerable<IInboundConnector> GetInboundConnectors()
		{
			return (from c in base.RemotePowershellSession.RunOneCommand<TenantInboundConnector>("Get-InboundConnector", null, true)
			select new InboundConnector(c)).ToList<InboundConnector>();
		}

		public IInboundConnector GetInboundConnector(string identity)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("Identity", identity);
			TenantInboundConnector tenantInboundConnector = base.RemotePowershellSession.RunOneCommandSingleResult<TenantInboundConnector>("Get-InboundConnector", sessionParameters, true);
			if (tenantInboundConnector != null)
			{
				return new InboundConnector(tenantInboundConnector);
			}
			return null;
		}

		public IntraOrganizationConfiguration GetIntraOrganizationConfiguration()
		{
			SessionParameters parameters = new SessionParameters();
			return base.RemotePowershellSession.RunOneCommandSingleResult<IntraOrganizationConfiguration>("Get-IntraOrganizationConfiguration", parameters, true);
		}

		public IntraOrganizationConnector GetIntraOrganizationConnector(string identity)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("Identity", identity);
			return base.RemotePowershellSession.RunOneCommandSingleResult<IntraOrganizationConnector>("Get-IntraOrganizationConnector", sessionParameters, true);
		}

		public IOrganizationalUnit GetOrganizationalUnit()
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("SingleNodeOnly", true);
			ExtendedOrganizationalUnit extendedOrganizationalUnit = base.RemotePowershellSession.RunOneCommandSingleResult<ExtendedOrganizationalUnit>("Get-OrganizationalUnit", sessionParameters, true);
			if (extendedOrganizationalUnit != null)
			{
				return new OrganizationalUnit
				{
					Name = extendedOrganizationalUnit.Name
				};
			}
			return null;
		}

		public IOnPremisesOrganization GetOnPremisesOrganization(Guid organizationGuid)
		{
			return (from o in base.RemotePowershellSession.RunOneCommand<Microsoft.Exchange.Data.Directory.SystemConfiguration.OnPremisesOrganization>("Get-OnPremisesOrganization", null, true)
			select new Microsoft.Exchange.Management.Hybrid.Entity.OnPremisesOrganization
			{
				Identity = (ADObjectId)o.Identity,
				OrganizationGuid = o.OrganizationGuid,
				OrganizationName = o.OrganizationName,
				HybridDomains = o.HybridDomains,
				InboundConnector = o.InboundConnector,
				OutboundConnector = o.OutboundConnector,
				Name = o.Name,
				OrganizationRelationship = o.OrganizationRelationship
			}).FirstOrDefault((Microsoft.Exchange.Management.Hybrid.Entity.OnPremisesOrganization o) => o.OrganizationGuid == organizationGuid);
		}

		public IEnumerable<IOutboundConnector> GetOutboundConnectors()
		{
			return (from c in base.RemotePowershellSession.RunOneCommand<TenantOutboundConnector>("Get-OutboundConnector", null, true)
			select new OutboundConnector(c)).ToList<OutboundConnector>();
		}

		public IOutboundConnector GetOutboundConnector(string identity)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("Identity", identity);
			TenantOutboundConnector tenantOutboundConnector = base.RemotePowershellSession.RunOneCommandSingleResult<TenantOutboundConnector>("Get-OutboundConnector", sessionParameters, true);
			if (tenantOutboundConnector != null)
			{
				return new OutboundConnector(tenantOutboundConnector);
			}
			return null;
		}

		public IInboundConnector NewInboundConnector(IInboundConnector configuration)
		{
			SessionParameters parameters = this.BuildParameters(configuration);
			TenantInboundConnector tenantInboundConnector = base.RemotePowershellSession.RunOneCommandSingleResult<TenantInboundConnector>("New-InboundConnector", parameters, false);
			if (tenantInboundConnector != null)
			{
				return new InboundConnector(tenantInboundConnector);
			}
			return null;
		}

		public void NewIntraOrganizationConnector(string name, string discoveryEndpoint, MultiValuedProperty<SmtpDomain> targetAddressDomains, bool enabled)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("Name", name);
			sessionParameters.Set("DiscoveryEndpoint", discoveryEndpoint);
			sessionParameters.Set<SmtpDomain>("TargetAddressDomains", targetAddressDomains);
			sessionParameters.Set("Enabled", enabled);
			base.RemotePowershellSession.RunOneCommand("New-IntraOrganizationConnector", sessionParameters, false);
		}

		public IOnPremisesOrganization NewOnPremisesOrganization(IOrganizationConfig onPremisesOrgConfig, MultiValuedProperty<SmtpDomain> hybridDomains, IInboundConnector inboundConnector, IOutboundConnector outboundConnector, OrganizationRelationship tenantOrgRel)
		{
			Microsoft.Exchange.Management.Hybrid.Entity.OnPremisesOrganization onPremisesOrganization = new Microsoft.Exchange.Management.Hybrid.Entity.OnPremisesOrganization(onPremisesOrgConfig.Guid, onPremisesOrgConfig.Name, hybridDomains, inboundConnector.Identity, outboundConnector.Identity, onPremisesOrgConfig.Guid.ToString(), (ADObjectId)tenantOrgRel.Identity);
			SessionParameters sessionParameters = this.BuildParameters(onPremisesOrganization);
			sessionParameters.Set("Name", onPremisesOrganization.Name);
			sessionParameters.Set("OrganizationGuid", onPremisesOrganization.OrganizationGuid);
			Microsoft.Exchange.Data.Directory.SystemConfiguration.OnPremisesOrganization onPremisesOrganization2 = base.RemotePowershellSession.RunOneCommandSingleResult<Microsoft.Exchange.Data.Directory.SystemConfiguration.OnPremisesOrganization>("New-OnPremisesOrganization", sessionParameters, false);
			if (onPremisesOrganization2 != null)
			{
				return new Microsoft.Exchange.Management.Hybrid.Entity.OnPremisesOrganization
				{
					Identity = (ADObjectId)onPremisesOrganization2.Identity,
					OrganizationGuid = onPremisesOrganization2.OrganizationGuid,
					OrganizationName = onPremisesOrganization2.OrganizationName,
					HybridDomains = onPremisesOrganization2.HybridDomains,
					InboundConnector = onPremisesOrganization2.InboundConnector,
					OutboundConnector = onPremisesOrganization2.OutboundConnector,
					Name = onPremisesOrganization2.Name,
					OrganizationRelationship = onPremisesOrganization2.OrganizationRelationship
				};
			}
			return null;
		}

		public IOutboundConnector NewOutboundConnector(IOutboundConnector configuration)
		{
			SessionParameters parameters = this.BuildParameters(configuration);
			TenantOutboundConnector tenantOutboundConnector = base.RemotePowershellSession.RunOneCommandSingleResult<TenantOutboundConnector>("New-OutboundConnector", parameters, false);
			if (tenantOutboundConnector != null)
			{
				return new OutboundConnector(tenantOutboundConnector);
			}
			return null;
		}

		public void RemoveInboundConnector(ADObjectId identity)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("Identity", identity.ToString());
			sessionParameters.Set("Confirm", false);
			base.RemotePowershellSession.RunOneCommand("Remove-IntraOrganizationConnector", sessionParameters, false);
		}

		public void RemoveIntraOrganizationConnector(string identity)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("Identity", identity);
			sessionParameters.Set("Confirm", false);
			base.RemotePowershellSession.RunOneCommand("Remove-IntraOrganizationConnector", sessionParameters, false);
		}

		public void RemoveOutboundConnector(ADObjectId identity)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("Identity", identity.ToString());
			sessionParameters.Set("Confirm", false);
			base.RemotePowershellSession.RunOneCommand("Remove-OutboundConnector", sessionParameters, false);
		}

		public void SetFederatedOrganizationIdentifier(string defaultDomain)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("DefaultDomain", defaultDomain);
			sessionParameters.Set("Enabled", true);
			base.RemotePowershellSession.RunOneCommand("Set-FederatedOrganizationIdentifier", sessionParameters, false);
		}

		public void SetInboundConnector(IInboundConnector configuration)
		{
			SessionParameters sessionParameters = this.BuildParameters(configuration);
			sessionParameters.Set("Identity", configuration.Identity.ToString());
			base.RemotePowershellSession.RunCommand("Set-InboundConnector", sessionParameters);
		}

		public void SetIntraOrganizationConnector(string identity, string discoveryEndpoint, MultiValuedProperty<SmtpDomain> targetAddressDomains, bool enabled)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("Identity", identity);
			sessionParameters.Set("DiscoveryEndpoint", discoveryEndpoint);
			sessionParameters.Set<SmtpDomain>("TargetAddressDomains", targetAddressDomains);
			sessionParameters.Set("Enabled", enabled);
			base.RemotePowershellSession.RunOneCommand("Set-IntraOrganizationConnector", sessionParameters, false);
		}

		public void SetOnPremisesOrganization(IOnPremisesOrganization configuration, IOrganizationConfig onPremisesOrgConfig, MultiValuedProperty<SmtpDomain> hybridDomains, IInboundConnector inboundConnector, IOutboundConnector outboundConnector, OrganizationRelationship tenantOrgRel)
		{
			Microsoft.Exchange.Management.Hybrid.Entity.OnPremisesOrganization onPremisesOrganization = (Microsoft.Exchange.Management.Hybrid.Entity.OnPremisesOrganization)configuration;
			onPremisesOrganization.HybridDomains = hybridDomains;
			onPremisesOrganization.InboundConnector = inboundConnector.Identity;
			onPremisesOrganization.OutboundConnector = outboundConnector.Identity;
			onPremisesOrganization.OrganizationName = onPremisesOrgConfig.Name;
			onPremisesOrganization.OrganizationRelationship = (ADObjectId)tenantOrgRel.Identity;
			SessionParameters sessionParameters = this.BuildParameters(configuration);
			sessionParameters.Set("Identity", configuration.Identity.ToString());
			base.RemotePowershellSession.RunCommand("Set-OnPremisesOrganization", sessionParameters);
		}

		public void SetOutboundConnector(IOutboundConnector configuration)
		{
			SessionParameters sessionParameters = this.BuildParameters(configuration);
			sessionParameters.Set("Identity", configuration.Identity.ToString());
			base.RemotePowershellSession.RunCommand("Set-OutboundConnector", sessionParameters);
		}

		public void RenameInboundConnector(IInboundConnector c, string name)
		{
			((InboundConnector)c).Name = name;
			this.SetInboundConnector(c);
		}

		public void RenameOutboundConnector(IOutboundConnector c, string name)
		{
			((OutboundConnector)c).Name = name;
			this.SetOutboundConnector(c);
		}

		public IInboundConnector BuildExpectedInboundConnector(ADObjectId identity, string name, SmtpX509Identifier tlsCertificateName)
		{
			return new InboundConnector(name, tlsCertificateName)
			{
				Identity = identity
			};
		}

		public IOutboundConnector BuildExpectedOutboundConnector(ADObjectId identity, string name, string tlsCertificateSubjectDomainName, MultiValuedProperty<SmtpDomain> hybridDomains, string smartHost, bool centralizedTransportFeatureEnabled)
		{
			MultiValuedProperty<SmtpDomainWithSubdomains> multiValuedProperty = new MultiValuedProperty<SmtpDomainWithSubdomains>();
			if (centralizedTransportFeatureEnabled)
			{
				multiValuedProperty.Add(new SmtpDomainWithSubdomains("*"));
			}
			else
			{
				foreach (SmtpDomainWithSubdomains item in from s in hybridDomains
				select new SmtpDomainWithSubdomains(s.Domain))
				{
					multiValuedProperty.Add(item);
				}
			}
			MultiValuedProperty<SmartHost> smartHosts = new MultiValuedProperty<SmartHost>
			{
				new SmartHost(smartHost)
			};
			return new OutboundConnector(name, tlsCertificateSubjectDomainName, multiValuedProperty, smartHosts, centralizedTransportFeatureEnabled)
			{
				Identity = identity
			};
		}

		private SessionParameters BuildParameters(IInboundConnector configuration)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("Name", configuration.Name);
			sessionParameters.Set("ConnectorType", configuration.ConnectorType);
			sessionParameters.Set("RequireTLS", configuration.RequireTls);
			sessionParameters.Set<AddressSpace>("SenderDomains", configuration.SenderDomains);
			sessionParameters.Set("TLSSenderCertificateName", configuration.TLSSenderCertificateName);
			sessionParameters.Set("CloudServicesMailEnabled", configuration.CloudServicesMailEnabled);
			return sessionParameters;
		}

		private SessionParameters BuildParameters(IOutboundConnector configuration)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("Name", configuration.Name);
			sessionParameters.Set<SmtpDomainWithSubdomains>("RecipientDomains", configuration.RecipientDomains);
			sessionParameters.Set<SmartHost>("SmartHosts", configuration.SmartHosts);
			sessionParameters.Set("ConnectorType", configuration.ConnectorType);
			sessionParameters.Set("TLSSettings", (Enum)configuration.TlsSettings);
			sessionParameters.Set("TLSDomain", configuration.TlsDomain);
			sessionParameters.Set("CloudServicesMailEnabled", configuration.CloudServicesMailEnabled);
			sessionParameters.Set("RouteAllMessagesViaOnPremises", configuration.RouteAllMessagesViaOnPremises);
			sessionParameters.Set("UseMxRecord", false);
			return sessionParameters;
		}

		private SessionParameters BuildParameters(IOnPremisesOrganization configuration)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set<SmtpDomain>("HybridDomains", configuration.HybridDomains, (SmtpDomain d) => d.Domain);
			sessionParameters.Set("InboundConnector", TaskCommon.ToStringOrNull(configuration.InboundConnector));
			sessionParameters.Set("OutboundConnector", TaskCommon.ToStringOrNull(configuration.OutboundConnector));
			sessionParameters.Set("OrganizationRelationship", TaskCommon.ToStringOrNull(configuration.OrganizationRelationship));
			sessionParameters.Set("OrganizationName", configuration.OrganizationName);
			return sessionParameters;
		}

		private const string Enable_OrganizationCustomization = "Enable-OrganizationCustomization";

		private const string Get_AcceptedDomain = "Get-AcceptedDomain";

		private const string Get_FederatedOrganizationIdentifier = "Get-FederatedOrganizationIdentifier";

		private const string Get_InboundConnector = "Get-InboundConnector";

		private const string Get_IntraOrganizationConfiguration = "Get-IntraOrganizationConfiguration";

		private const string Get_IntraOrganizationConnector = "Get-IntraOrganizationConnector";

		private const string Get_OrganizationalUnit = "Get-OrganizationalUnit";

		private const string Get_OnPremisesOrganization = "Get-OnPremisesOrganization";

		private const string Get_OutboundConnector = "Get-OutboundConnector";

		private const string New_InboundConnector = "New-InboundConnector";

		private const string New_IntraOrganizationConnector = "New-IntraOrganizationConnector";

		private const string New_OnPremisesOrganization = "New-OnPremisesOrganization";

		private const string New_OutboundConnector = "New-OutboundConnector";

		private const string Remove_InboundConnector = "Remove-InboundConnector";

		private const string Remove_IntraOrganizationConnector = "Remove-IntraOrganizationConnector";

		private const string Remove_OutboundConnector = "Remove-OutboundConnector";

		private const string Set_InboundConnector = "Set-InboundConnector";

		private const string Set_IntraOrganizationConnector = "Set-IntraOrganizationConnector";

		private const string Set_OnPremisesOrganization = "Set-OnPremisesOrganization";

		private const string Set_OutboundConnector = "Set-OutboundConnector";
	}
}
