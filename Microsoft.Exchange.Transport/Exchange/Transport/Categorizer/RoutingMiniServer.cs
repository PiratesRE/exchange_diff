using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class RoutingMiniServer
	{
		public RoutingMiniServer(Server server)
		{
			ArgumentValidator.ThrowIfNull("server", server);
			this.Id = server.Id;
			this.ServerSite = server.ServerSite;
			this.DatabaseAvailabilityGroup = server.DatabaseAvailabilityGroup;
			this.ExchangeLegacyDN = server.ExchangeLegacyDN;
			this.Fqdn = server.Fqdn;
			this.HomeRoutingGroup = server.HomeRoutingGroup;
			this.IsExchange2007OrLater = server.IsExchange2007OrLater;
			this.IsEdgeServer = server.IsEdgeServer;
			this.IsFrontendTransportServer = server.IsFrontendTransportServer;
			this.IsHubTransportServer = server.IsHubTransportServer;
			this.IsMailboxServer = server.IsMailboxServer;
			this.MajorVersion = server.MajorVersion;
			this.CurrentServerRole = server.CurrentServerRole;
			this.IsE15OrLater = server.IsE15OrLater;
			this.ComponentStates = new MultiValuedProperty<string>(server.ComponentStates);
		}

		public RoutingMiniServer(TopologyServer server)
		{
			ArgumentValidator.ThrowIfNull("server", server);
			this.Id = server.Id;
			this.ServerSite = server.ServerSite;
			this.DatabaseAvailabilityGroup = server.DatabaseAvailabilityGroup;
			this.ExchangeLegacyDN = server.ExchangeLegacyDN;
			this.Fqdn = server.Fqdn;
			this.HomeRoutingGroup = server.HomeRoutingGroup;
			this.IsExchange2007OrLater = server.IsExchange2007OrLater;
			this.IsEdgeServer = server.IsEdgeServer;
			this.IsFrontendTransportServer = server.IsFrontendTransportServer;
			this.IsHubTransportServer = server.IsHubTransportServer;
			this.IsMailboxServer = server.IsMailboxServer;
			this.MajorVersion = server.MajorVersion;
			this.CurrentServerRole = server.CurrentServerRole;
			this.IsE15OrLater = server.IsE15OrLater;
			this.ComponentStates = new MultiValuedProperty<string>(server.ComponentStates);
		}

		public ADObjectId Id { get; private set; }

		public ADObjectId ServerSite { get; private set; }

		public ADObjectId DatabaseAvailabilityGroup { get; private set; }

		public string ExchangeLegacyDN { get; private set; }

		public string Fqdn { get; private set; }

		public ADObjectId HomeRoutingGroup { get; private set; }

		public bool IsExchange2007OrLater { get; private set; }

		public bool IsEdgeServer { get; private set; }

		public bool IsFrontendTransportServer { get; private set; }

		public bool IsHubTransportServer { get; private set; }

		public bool IsMailboxServer { get; private set; }

		public int MajorVersion { get; private set; }

		public ServerRole CurrentServerRole { get; private set; }

		public bool IsE15OrLater { get; private set; }

		public MultiValuedProperty<string> ComponentStates { get; private set; }
	}
}
