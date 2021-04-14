using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport.Configuration;

namespace Microsoft.Exchange.Transport
{
	internal interface ITransportConfiguration
	{
		AcceptedDomainTable FirstOrgAcceptedDomainTable { get; }

		event ConfigurationUpdateHandler<AcceptedDomainTable> FirstOrgAcceptedDomainTableChanged;

		RemoteDomainTable RemoteDomainTable { get; }

		event ConfigurationUpdateHandler<RemoteDomainTable> RemoteDomainTableChanged;

		X400AuthoritativeDomainTable X400AuthoritativeDomainTable { get; }

		ReceiveConnectorConfiguration LocalReceiveConnectors { get; }

		event ConfigurationUpdateHandler<ReceiveConnectorConfiguration> LocalReceiveConnectorsChanged;

		TransportServerConfiguration LocalServer { get; }

		event ConfigurationUpdateHandler<TransportServerConfiguration> LocalServerChanged;

		TransportSettingsConfiguration TransportSettings { get; }

		MicrosoftExchangeRecipientPerTenantSettings MicrosoftExchangeRecipient { get; }

		event ConfigurationUpdateHandler<TransportSettingsConfiguration> TransportSettingsChanged;

		TransportAppConfig AppConfig { get; }

		ProcessTransportRole ProcessTransportRole { get; }

		void ClearCaches();

		bool TryGetTransportSettings(OrganizationId orgId, out PerTenantTransportSettings perTenantTransportSettings);

		PerTenantTransportSettings GetTransportSettings(OrganizationId orgId);

		bool TryGetPerimeterSettings(OrganizationId orgId, out PerTenantPerimeterSettings perTenantPerimeterSettings);

		PerTenantPerimeterSettings GetPerimeterSettings(OrganizationId orgId);

		bool TryGetMicrosoftExchangeRecipient(OrganizationId orgId, out MicrosoftExchangeRecipientPerTenantSettings perTenantMicrosoftExchangeRecipient);

		MicrosoftExchangeRecipientPerTenantSettings GetMicrosoftExchangeRecipient(OrganizationId orgId);

		bool TryGetRemoteDomainTable(OrganizationId orgId, out PerTenantRemoteDomainTable perTenantRemoteDomains);

		PerTenantRemoteDomainTable GetRemoteDomainTable(OrganizationId orgId);

		bool TryGetAcceptedDomainTable(OrganizationId orgId, out PerTenantAcceptedDomainTable perTenantAcceptedDomains);

		PerTenantAcceptedDomainTable GetAcceptedDomainTable(OrganizationId orgId);

		bool TryGetJournalingRules(OrganizationId orgId, out JournalingRulesPerTenantSettings perTenantJournalingRules);

		JournalingRulesPerTenantSettings GetJournalingRules(OrganizationId orgId);

		bool TryGetReconciliationAccounts(OrganizationId orgId, out ReconciliationAccountPerTenantSettings perTenantReconciliationSettings);

		ReconciliationAccountPerTenantSettings GetReconciliationAccounts(OrganizationId orgId);

		bool TryGetTenantOutboundConnectors(OrganizationId orgId, out PerTenantOutboundConnectors perTenantOutboundConnectors);
	}
}
