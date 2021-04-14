using System;
using System.Net;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class RoutingServerInfo : INextHopServer
	{
		public RoutingServerInfo(RoutingMiniServer server)
		{
			ArgumentValidator.ThrowIfNull("server", server);
			this.server = server;
		}

		public ADObjectId Id
		{
			get
			{
				return this.server.Id;
			}
		}

		public ADObjectId ADSite
		{
			get
			{
				return this.server.ServerSite;
			}
		}

		public bool DagRoutingEnabled
		{
			get
			{
				return this.server.IsE15OrLater && this.server.DatabaseAvailabilityGroup != null;
			}
		}

		public ADObjectId DatabaseAvailabilityGroup
		{
			get
			{
				return this.server.DatabaseAvailabilityGroup;
			}
		}

		public string ExchangeLegacyDN
		{
			get
			{
				return this.server.ExchangeLegacyDN;
			}
		}

		public string Fqdn
		{
			get
			{
				return this.server.Fqdn;
			}
		}

		public ADObjectId HomeRoutingGroup
		{
			get
			{
				return this.server.HomeRoutingGroup;
			}
		}

		public bool IsExchange2007OrLater
		{
			get
			{
				return this.server.IsExchange2007OrLater;
			}
		}

		public bool IsEdgeTransportServer
		{
			get
			{
				return this.server.IsEdgeServer;
			}
		}

		public bool IsFrontendTransportServer
		{
			get
			{
				return this.server.IsFrontendTransportServer;
			}
		}

		public bool IsFrontendTransportActive
		{
			get
			{
				if (this.frontendComponentState == RoutingServerInfo.ComponentState.Unknown)
				{
					this.frontendComponentState = (RoutingServerInfo.IsFrontendComponentActive(this.server.IsFrontendTransportServer, this.server.ComponentStates) ? RoutingServerInfo.ComponentState.Active : RoutingServerInfo.ComponentState.Inactive);
				}
				return this.frontendComponentState == RoutingServerInfo.ComponentState.Active;
			}
		}

		public bool IsHubTransportServer
		{
			get
			{
				return this.server.IsHubTransportServer;
			}
		}

		public bool IsHubTransportActive
		{
			get
			{
				if (this.hubComponentState == RoutingServerInfo.ComponentState.Unknown)
				{
					this.hubComponentState = (RoutingServerInfo.IsHubComponentActive(this.server.IsHubTransportServer, this.server.IsE15OrLater, this.server.ComponentStates) ? RoutingServerInfo.ComponentState.Active : RoutingServerInfo.ComponentState.Inactive);
				}
				return this.hubComponentState == RoutingServerInfo.ComponentState.Active;
			}
		}

		public bool IsMailboxServer
		{
			get
			{
				return this.server.IsMailboxServer;
			}
		}

		public int MajorVersion
		{
			get
			{
				return this.server.MajorVersion;
			}
		}

		bool INextHopServer.IsIPAddress
		{
			get
			{
				return false;
			}
		}

		IPAddress INextHopServer.Address
		{
			get
			{
				throw new InvalidOperationException("INextHopServer.IPAddress must not be requested from RoutingServerInfo objects");
			}
		}

		public bool IsFrontendAndHubColocatedServer
		{
			get
			{
				return this.IsHubTransportServer && this.IsFrontendTransportServer;
			}
		}

		public static bool IsFrontendComponentActive(Server server)
		{
			return RoutingServerInfo.IsFrontendComponentActive(server.IsFrontendTransportServer, server.ComponentStates);
		}

		public static bool IsFrontendComponentActive(TopologyServer topologyServer)
		{
			return RoutingServerInfo.IsFrontendComponentActive(topologyServer.IsFrontendTransportServer, topologyServer.ComponentStates);
		}

		public static bool IsHubComponentActive(Server server)
		{
			return RoutingServerInfo.IsHubComponentActive(server.IsHubTransportServer, server.IsE15OrLater, server.ComponentStates);
		}

		public static bool IsHubComponentActive(TopologyServer topologyServer)
		{
			return RoutingServerInfo.IsHubComponentActive(topologyServer.IsHubTransportServer, topologyServer.IsE15OrLater, topologyServer.ComponentStates);
		}

		public bool IsSameServerAs(Server topologyServer)
		{
			return string.Equals(this.server.Fqdn, topologyServer.Fqdn, StringComparison.CurrentCultureIgnoreCase);
		}

		public bool IsSameServerAs(TopologyServer topologyServer)
		{
			return string.Equals(this.server.Fqdn, topologyServer.Fqdn, StringComparison.CurrentCultureIgnoreCase);
		}

		public bool IsInSameSite(Server topologyServer)
		{
			return this.ADSite.Equals(topologyServer.ServerSite);
		}

		public bool IsInSameSite(TopologyServer topologyServer)
		{
			return this.ADSite.Equals(topologyServer.ServerSite);
		}

		public bool IsSameVersionAs(Server topologyServer)
		{
			return this.MajorVersion == topologyServer.MajorVersion;
		}

		public bool IsSameVersionAs(TopologyServer topologyServer)
		{
			return this.MajorVersion == topologyServer.MajorVersion;
		}

		public bool IsSameSiteAndVersionAs(Server topologyServer)
		{
			return this.IsSameVersionAs(topologyServer) && this.IsInSameSite(topologyServer);
		}

		public bool IsSameSiteAndVersionAs(TopologyServer topologyServer)
		{
			return this.IsSameVersionAs(topologyServer) && this.IsInSameSite(topologyServer);
		}

		public bool Match(RoutingServerInfo other)
		{
			if (!ADObjectId.Equals(this.Id, other.Id) || !RoutingUtils.MatchStrings(this.Fqdn, other.Fqdn) || this.MajorVersion != other.MajorVersion || this.server.CurrentServerRole != other.server.CurrentServerRole || !RoutingUtils.MatchStrings(this.ExchangeLegacyDN, other.ExchangeLegacyDN))
			{
				return false;
			}
			if (this.IsExchange2007OrLater)
			{
				if (!ADObjectId.Equals(this.ADSite, other.ADSite))
				{
					return false;
				}
				if (this.server.IsE15OrLater && !ADObjectId.Equals(this.DatabaseAvailabilityGroup, other.DatabaseAvailabilityGroup))
				{
					return false;
				}
			}
			else if (!ADObjectId.Equals(this.HomeRoutingGroup, other.HomeRoutingGroup))
			{
				return false;
			}
			return true;
		}

		private static bool IsFrontendComponentActive(bool isFrontendTransportServer, MultiValuedProperty<string> componentStates)
		{
			if (!isFrontendTransportServer)
			{
				throw new InvalidOperationException("Provided server is not a FrontendTransport server");
			}
			return RoutingServerInfo.IsServerComponentActive(componentStates, RoutingServerInfo.FrontendComponent);
		}

		private static bool IsHubComponentActive(bool isHubTransportServer, bool isE15OrLater, MultiValuedProperty<string> componentStates)
		{
			if (!isHubTransportServer)
			{
				throw new InvalidOperationException("Provided server is not a HubTransport server");
			}
			return !isE15OrLater || RoutingServerInfo.IsServerComponentActive(componentStates, RoutingServerInfo.HubComponent);
		}

		private static bool IsServerComponentActive(MultiValuedProperty<string> componentStates, string component)
		{
			return ServiceState.Active == ServerComponentStates.ReadEffectiveComponentState(null, componentStates, ServerComponentStateSources.AD, component, ServiceStateHelper.GetDefaultServiceState());
		}

		private static readonly string FrontendComponent = ServerComponentStates.GetComponentId(ServerComponentEnum.FrontendTransport);

		private static readonly string HubComponent = ServerComponentStates.GetComponentId(ServerComponentEnum.HubTransport);

		private readonly RoutingMiniServer server;

		private RoutingServerInfo.ComponentState frontendComponentState;

		private RoutingServerInfo.ComponentState hubComponentState;

		private enum ComponentState : byte
		{
			Unknown,
			Inactive,
			Active
		}
	}
}
