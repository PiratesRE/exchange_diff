using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal interface IMailRouter
	{
		event RoutingTablesChangedHandler RoutingTablesChanged;

		IList<RoutingAddress> ExternalPostmasterAddresses { get; }

		void RouteToMultipleDestinations(TransportMailItem mailItem, TaskContext taskContext);

		bool TryGetServersForNextHop(NextHopSolutionKey nextHopKey, out IEnumerable<INextHopServer> servers, out SmtpSendConnectorConfig connector);

		bool TryGetOutboundFrontendServers(out IEnumerable<INextHopServer> servers, out bool externalOutboundFrontendProxyEnabled);

		void ApplyDelayedFanout(TransportMailItem mailItem);

		bool TrySelectHubServersForDatabases(IList<ADObjectId> databaseIds, Guid? externalOrganizationId, out IEnumerable<INextHopServer> hubServers);

		bool TrySelectHubServersUsingDagSelector(Guid externalOrganizationId, out IEnumerable<INextHopServer> hubServers);

		bool TrySelectHubServersForShadow(ShadowRoutingConfiguration shadowRoutingConfig, out IEnumerable<INextHopServer> hubServers);

		bool TryGetLocalSendConnector<ConnectorType>(Guid connectorGuid, out ConnectorType connector) where ConnectorType : MailGateway;

		IList<ConnectorType> GetLocalSendConnectors<ConnectorType>() where ConnectorType : MailGateway;

		bool IsHubTransportServer(string serverFqdn);

		bool IsInLocalSite(string serverFqdn);

		bool TryGetServerFqdnByLegacyDN(string serverLegacyDN, out string serverFqdn);

		bool TryGetServerLegacyDNByFqdn(string serverFqdn, out string serverLegacyDN);

		bool TryGetRelatedServersForShadowQueue(NextHopSolutionKey shadowQueueKey, out IEnumerable<INextHopServer> servers);

		bool IsJournalMessage(IReadOnlyMailItem mailItem);
	}
}
