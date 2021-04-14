using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Common;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class ReceiveConnectorManager
	{
		public ReceiveConnectorManager(ISmtpReceiveConfiguration configuration)
		{
			ArgumentValidator.ThrowIfNull("configuration", configuration);
			this.role = configuration.TransportConfiguration.ProcessTransportRole;
			this.mailboxDeliveryAcceptAnonymousUsers = configuration.TransportConfiguration.MailboxDeliveryAcceptAnonymousUsers;
			this.minimumAvailabilityConnectionsToMonitor = configuration.TransportConfiguration.SmtpAvailabilityMinConnectionsToMonitor;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Connectors: {0}\r\n", string.Join(", ", from c in this.connectorsByName
			select c.Value.Connector.Name));
			stringBuilder.AppendFormat("Bindings: {0}\r\n", string.Join<IPEndPoint>(", ", this.Bindings));
			return stringBuilder.ToString();
		}

		public List<IPEndPoint> Bindings
		{
			get
			{
				return this.bindings;
			}
		}

		public InMemoryReceiveConnector DeliveryReceiveConnector { get; private set; }

		public void ApplyReceiveConnectors(IEnumerable<ReceiveConnector> connectorsFromActiveDirectoryForAllRoles, Server localServer)
		{
			ArgumentValidator.ThrowIfNull("connectorsFromActiveDirectoryForAllRoles", connectorsFromActiveDirectoryForAllRoles);
			ArgumentValidator.ThrowIfNull("localServer", localServer);
			List<ReceiveConnector> list = Util.EnabledReceiveConnectorsForRole(connectorsFromActiveDirectoryForAllRoles, this.role);
			this.AddRoleSpecificReceiveConnectors(list, localServer);
			SmtpInConnectorMap<SmtpReceiveConnectorStub> smtpInConnectorMap = new SmtpInConnectorMap<SmtpReceiveConnectorStub>();
			Dictionary<string, SmtpReceiveConnectorStub> currentEntries = new Dictionary<string, SmtpReceiveConnectorStub>();
			foreach (ReceiveConnector receiveConnector in list)
			{
				SmtpReceiveConnectorStub smtpReceiveConnectorStub = new SmtpReceiveConnectorStub(receiveConnector, Util.CreateReceivePerfCounters(receiveConnector, this.role), Util.GetOrCreateAvailabilityPerfCounters(this.cachedAvailabilityPerfCounters, receiveConnector, this.role, this.minimumAvailabilityConnectionsToMonitor));
				SmtpReceiveConnectorStub smtpReceiveConnectorStub2;
				if (this.connectorsByName.TryGetValue(receiveConnector.Name, out smtpReceiveConnectorStub2))
				{
					smtpReceiveConnectorStub.ConnectionTable = smtpReceiveConnectorStub2.ConnectionTable;
				}
				smtpInConnectorMap.AddEntry(receiveConnector.Bindings.ToArray(), receiveConnector.RemoteIPRanges.ToArray(), smtpReceiveConnectorStub);
				TransportHelpers.AttemptAddToDictionary<string, SmtpReceiveConnectorStub>(currentEntries, receiveConnector.Name, smtpReceiveConnectorStub, null);
			}
			this.bindings = Util.BindingsFromReceiveConnectors(list, this.role);
			this.connectorMap = smtpInConnectorMap;
			this.connectorsByName = currentEntries;
		}

		public void ApplyLocalServerConfiguration(Server transportServer)
		{
			ArgumentValidator.ThrowIfNull("transportServer", transportServer);
			if (this.DeliveryReceiveConnector != null)
			{
				this.DeliveryReceiveConnector.ApplyLocalServerConfiguration(transportServer);
			}
		}

		public bool TryLookupIncomingConnection(IPEndPoint localEndpoint, IPEndPoint remoteEndpoint, out SmtpReceiveConnectorStub receiveConnectorStub)
		{
			ArgumentValidator.ThrowIfNull("localEndpoint", localEndpoint);
			ArgumentValidator.ThrowIfNull("remoteEndpoint", remoteEndpoint);
			receiveConnectorStub = this.connectorMap.Lookup(localEndpoint.Address, localEndpoint.Port, remoteEndpoint.Address);
			return receiveConnectorStub != null;
		}

		protected virtual InMemoryReceiveConnector CreateDeliveryReceiveConnector(Server localServer)
		{
			return new MailboxDeliveryReceiveConnector(Util.FormatMailboxDeliveryReceiveConnectorName(localServer.Name), localServer, this.mailboxDeliveryAcceptAnonymousUsers);
		}

		private void AddRoleSpecificReceiveConnectors(List<ReceiveConnector> connectors, Server localServer)
		{
			if (this.role == ProcessTransportRole.MailboxDelivery)
			{
				this.DeliveryReceiveConnector = this.CreateDeliveryReceiveConnector(localServer);
				connectors.Add(this.DeliveryReceiveConnector);
			}
		}

		private readonly ProcessTransportRole role;

		private readonly bool mailboxDeliveryAcceptAnonymousUsers;

		private readonly int minimumAvailabilityConnectionsToMonitor;

		private List<IPEndPoint> bindings = new List<IPEndPoint>();

		private SmtpInConnectorMap<SmtpReceiveConnectorStub> connectorMap = new SmtpInConnectorMap<SmtpReceiveConnectorStub>();

		private Dictionary<string, SmtpReceiveConnectorStub> connectorsByName = new Dictionary<string, SmtpReceiveConnectorStub>();

		private readonly ConcurrentDictionary<string, ISmtpAvailabilityPerfCounters> cachedAvailabilityPerfCounters = new ConcurrentDictionary<string, ISmtpAvailabilityPerfCounters>();
	}
}
