using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal interface ITenantSession : ICommonSession, IDisposable
	{
		void EnableOrganizationCustomization();

		IEnumerable<IInboundConnector> GetInboundConnectors();

		IInboundConnector GetInboundConnector(string identity);

		IntraOrganizationConfiguration GetIntraOrganizationConfiguration();

		IntraOrganizationConnector GetIntraOrganizationConnector(string identity);

		IOrganizationalUnit GetOrganizationalUnit();

		IOnPremisesOrganization GetOnPremisesOrganization(Guid organizationGuid);

		IEnumerable<IOutboundConnector> GetOutboundConnectors();

		IOutboundConnector GetOutboundConnector(string identity);

		IInboundConnector NewInboundConnector(IInboundConnector configuration);

		void NewIntraOrganizationConnector(string name, string discoveryEndpoint, MultiValuedProperty<SmtpDomain> targetAddressDomains, bool enabled);

		IOnPremisesOrganization NewOnPremisesOrganization(IOrganizationConfig onPremisesOrgConfig, MultiValuedProperty<SmtpDomain> hybridDomains, IInboundConnector inboundConnector, IOutboundConnector outboundConnector, OrganizationRelationship tenantOrgRel);

		IOutboundConnector NewOutboundConnector(IOutboundConnector configuration);

		void RemoveInboundConnector(ADObjectId identity);

		void RemoveIntraOrganizationConnector(string identity);

		void RemoveOutboundConnector(ADObjectId identity);

		void SetFederatedOrganizationIdentifier(string defaultDomain);

		void SetInboundConnector(IInboundConnector configuration);

		void SetIntraOrganizationConnector(string identity, string discoveryEndpoint, MultiValuedProperty<SmtpDomain> targetAddressDomains, bool enabled);

		void SetOnPremisesOrganization(IOnPremisesOrganization configuration, IOrganizationConfig onPremisesOrgConfig, MultiValuedProperty<SmtpDomain> hybridDomains, IInboundConnector inboundConnector, IOutboundConnector outboundConnector, OrganizationRelationship tenantOrgRel);

		void SetOutboundConnector(IOutboundConnector configuration);

		void RenameInboundConnector(IInboundConnector configuration, string name);

		void RenameOutboundConnector(IOutboundConnector configuration, string name);

		IInboundConnector BuildExpectedInboundConnector(ADObjectId identity, string name, SmtpX509Identifier tlsCertificateName);

		IOutboundConnector BuildExpectedOutboundConnector(ADObjectId identity, string name, string tlsCertificateSubjectDomainName, MultiValuedProperty<SmtpDomain> hybridDomains, string smartHost, bool centralizedTransportFeatureEnabled);
	}
}
