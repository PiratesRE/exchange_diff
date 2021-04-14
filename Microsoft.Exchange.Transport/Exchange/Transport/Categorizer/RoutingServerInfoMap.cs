using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Transport.Common;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class RoutingServerInfoMap
	{
		public RoutingServerInfoMap(RoutingTopologyBase topologyConfig, RoutingContextCore contextCore)
		{
			this.localServer = topologyConfig.LocalServer;
			this.whenCreated = topologyConfig.WhenCreated;
			this.serversByGuid = new Dictionary<Guid, RoutingServerInfo>(64);
			this.serversByDN = new Dictionary<string, RoutingServerInfo>(64, StringComparer.OrdinalIgnoreCase);
			this.serversByFqdn = new Dictionary<string, RoutingServerInfo>(64, StringComparer.OrdinalIgnoreCase);
			this.serversByLegacyDN = new Dictionary<string, RoutingServerInfo>(64, StringComparer.OrdinalIgnoreCase);
			this.externalPostmasterAddresses = new List<RoutingAddress>();
			foreach (TopologyServer topologyServer in topologyConfig.Servers)
			{
				if (!this.ShouldExcludeTopologyServer(topologyServer))
				{
					RoutingServerInfo routingServerInfo = new RoutingServerInfo(new RoutingMiniServer(topologyServer));
					if (TransportHelpers.AttemptAddToDictionary<Guid, RoutingServerInfo>(this.serversByGuid, topologyServer.Guid, routingServerInfo, new TransportHelpers.DiagnosticsHandler<Guid, RoutingServerInfo>(RoutingUtils.LogErrorWhenAddToDictionaryFails<Guid, RoutingServerInfo>)) && TransportHelpers.AttemptAddToDictionary<string, RoutingServerInfo>(this.serversByDN, topologyServer.DistinguishedName, routingServerInfo, new TransportHelpers.DiagnosticsHandler<string, RoutingServerInfo>(RoutingUtils.LogErrorWhenAddToDictionaryFails<string, RoutingServerInfo>)) && TransportHelpers.AttemptAddToDictionary<string, RoutingServerInfo>(this.serversByFqdn, topologyServer.Fqdn, routingServerInfo, new TransportHelpers.DiagnosticsHandler<string, RoutingServerInfo>(RoutingUtils.LogErrorWhenAddToDictionaryFails<string, RoutingServerInfo>)) && TransportHelpers.AttemptAddToDictionary<string, RoutingServerInfo>(this.serversByLegacyDN, topologyServer.ExchangeLegacyDN, routingServerInfo, new TransportHelpers.DiagnosticsHandler<string, RoutingServerInfo>(RoutingUtils.LogErrorWhenAddToDictionaryFails<string, RoutingServerInfo>)))
					{
						if (contextCore.ConnectorRoutingSupported && routingServerInfo.IsFrontendTransportServer && this.IsInLocalSite(routingServerInfo) && (contextCore.Settings.OutboundProxyRoutingXVersionEnabled || routingServerInfo.IsSameVersionAs(this.localServer)) && contextCore.VerifyFrontendComponentStateRestriction(routingServerInfo))
						{
							RoutingUtils.AddItemToLazyList<RoutingServerInfo>(routingServerInfo, contextCore.Settings.RandomLoadBalancingOffsetEnabled, ref this.frontendServersInLocalSite);
						}
					}
					else
					{
						this.serversByGuid.Remove(topologyServer.Guid);
						this.serversByDN.Remove(topologyServer.DistinguishedName);
						this.serversByFqdn.Remove(topologyServer.Fqdn);
					}
				}
				this.AddExternalPostmasterAddress(topologyServer);
			}
			this.externalPostmasterAddresses = new ReadOnlyCollection<RoutingAddress>(this.externalPostmasterAddresses);
			this.siteRelayMap = new ADSiteRelayMap(topologyConfig, this, contextCore);
			this.routingGroupRelayMap = new RoutingGroupRelayMap(topologyConfig, this, contextCore);
		}

		public bool LocalDagExists
		{
			get
			{
				return this.localServer.DatabaseAvailabilityGroup != null;
			}
		}

		public int LocalServerVersion
		{
			get
			{
				return this.localServer.MajorVersion;
			}
		}

		public bool LocalHubSiteEnabled
		{
			get
			{
				return this.localServer.TopologySite.HubSiteEnabled;
			}
		}

		public ADSiteRelayMap SiteRelayMap
		{
			get
			{
				return this.siteRelayMap;
			}
		}

		public RoutingGroupRelayMap RoutingGroupRelayMap
		{
			get
			{
				return this.routingGroupRelayMap;
			}
		}

		public DateTime WhenCreated
		{
			get
			{
				return this.whenCreated;
			}
		}

		public IList<RoutingAddress> ExternalPostmasterAddresses
		{
			get
			{
				return this.externalPostmasterAddresses;
			}
		}

		public bool IsLocalServer(RoutingServerInfo serverInfo)
		{
			return serverInfo.IsSameServerAs(this.localServer);
		}

		public bool IsInLocalSite(RoutingServerInfo serverInfo)
		{
			return serverInfo.IsInSameSite(this.localServer);
		}

		public bool IsInLocalDag(RoutingServerInfo serverInfo)
		{
			ADObjectId databaseAvailabilityGroup = this.localServer.DatabaseAvailabilityGroup;
			ADObjectId databaseAvailabilityGroup2 = serverInfo.DatabaseAvailabilityGroup;
			return databaseAvailabilityGroup != null && databaseAvailabilityGroup2 != null && databaseAvailabilityGroup.ObjectGuid.Equals(databaseAvailabilityGroup2.ObjectGuid);
		}

		public bool TryGetServerInfo(ADObjectId serverId, out RoutingServerInfo serverInfo)
		{
			RoutingUtils.ThrowIfNullOrEmpty(serverId, "serverId");
			return this.TryGetServerInfo(serverId.ObjectGuid, out serverInfo);
		}

		public bool TryGetServerInfo(Guid serverGuid, out RoutingServerInfo serverInfo)
		{
			return this.serversByGuid.TryGetValue(serverGuid, out serverInfo);
		}

		public bool TryGetServerInfoByDN(string serverDN, out RoutingServerInfo serverInfo)
		{
			RoutingUtils.ThrowIfNullOrEmpty(serverDN, "serverDN");
			return this.serversByDN.TryGetValue(serverDN, out serverInfo);
		}

		public bool TryGetServerInfoByFqdn(string serverFqdn, out RoutingServerInfo serverInfo)
		{
			RoutingUtils.ThrowIfNullOrEmpty(serverFqdn, "serverFqdn");
			return this.serversByFqdn.TryGetValue(serverFqdn, out serverInfo);
		}

		public bool TryGetServerInfoByLegacyDN(string serverLegacyDN, out RoutingServerInfo serverInfo)
		{
			RoutingUtils.ThrowIfNullOrEmpty(serverLegacyDN, "serverLegacyDN");
			return this.serversByLegacyDN.TryGetValue(serverLegacyDN, out serverInfo);
		}

		public IEnumerable<RoutingServerInfo> GetHubTransportServers()
		{
			return this.GetServers((RoutingServerInfo server) => server.IsExchange2007OrLater && server.IsHubTransportServer);
		}

		public bool TryGetServerRoute(RoutingServerInfo serverInfo, out RouteInfo routeInfo)
		{
			return this.TryGetServerRoute(serverInfo, this.GetProximity(serverInfo), out routeInfo) != Proximity.None;
		}

		public bool TryGetServerRoute(ADObjectId serverId, out RoutingServerInfo serverInfo, out RouteInfo routeInfo)
		{
			return this.TryGetServerRouteByDN(serverId.DistinguishedName, Proximity.None, out serverInfo, out routeInfo) != Proximity.None;
		}

		public bool TryGetServerRouteByDN(string serverDN, out RoutingServerInfo serverInfo, out RouteInfo routeInfo)
		{
			return this.TryGetServerRouteByDN(serverDN, Proximity.None, out serverInfo, out routeInfo) != Proximity.None;
		}

		public bool TryCreateRoutedServerCollectionForClosestProximity(ICollection<ADObjectId> serverIds, RoutingContextCore contextCore, out RoutedServerCollection collection, out List<ADObjectId> unknownServerIds, out List<RoutingServerInfo> unroutedServers, out List<RoutingServerInfo> nonActiveServers)
		{
			collection = null;
			unknownServerIds = null;
			unroutedServers = null;
			nonActiveServers = null;
			foreach (ADObjectId adobjectId in serverIds)
			{
				Proximity proximity = (collection == null) ? Proximity.None : collection.ClosestProximity;
				RoutingServerInfo routingServerInfo;
				RouteInfo routeInfo;
				Proximity proximity2 = this.TryGetServerRouteByDN(adobjectId.DistinguishedName, proximity, out routingServerInfo, out routeInfo);
				if (routingServerInfo == null)
				{
					RoutingUtils.AddItemToLazyList<ADObjectId>(adobjectId, ref unknownServerIds);
				}
				else if (routingServerInfo.IsHubTransportServer && !contextCore.VerifyHubComponentStateRestriction(routingServerInfo))
				{
					RoutingUtils.AddItemToLazyList<RoutingServerInfo>(routingServerInfo, ref nonActiveServers);
				}
				else if (proximity2 == Proximity.None)
				{
					RoutingUtils.AddItemToLazyList<RoutingServerInfo>(routingServerInfo, ref unroutedServers);
				}
				else if (collection == null)
				{
					collection = new RoutedServerCollection(routeInfo, routingServerInfo, contextCore);
				}
				else if (proximity2 <= proximity)
				{
					collection.AddServerForRoute(routeInfo, routingServerInfo, proximity2 < proximity, contextCore);
					if (proximity2 == Proximity.LocalServer)
					{
						return true;
					}
				}
			}
			return collection != null;
		}

		public bool TryCreateRoutedServerCollection(ICollection<string> serverFqdns, RoutingContextCore contextCore, out RoutedServerCollection collection, out List<string> unknownServers, out List<RoutingServerInfo> unroutedServers)
		{
			collection = null;
			unknownServers = null;
			unroutedServers = null;
			foreach (string text in serverFqdns)
			{
				RoutingServerInfo routingServerInfo;
				RouteInfo routeInfo;
				Proximity proximity = this.TryGetServerRouteByFqdn(text, out routingServerInfo, out routeInfo);
				if (proximity == Proximity.None)
				{
					if (routingServerInfo == null)
					{
						RoutingUtils.AddItemToLazyList<string>(text, ref unknownServers);
					}
					else
					{
						RoutingUtils.AddItemToLazyList<RoutingServerInfo>(routingServerInfo, ref unroutedServers);
					}
				}
				else if (collection == null)
				{
					collection = new RoutedServerCollection(routeInfo, routingServerInfo, contextCore);
				}
				else
				{
					collection.AddServerForRoute(routeInfo, routingServerInfo, contextCore);
				}
			}
			return collection != null;
		}

		public bool TryGetLoadBalancedFrontendServersInLocalSite(out IEnumerable<RoutingServerInfo> servers)
		{
			if (this.frontendServersInLocalSite == null)
			{
				servers = null;
				return false;
			}
			servers = this.frontendServersInLocalSite.LoadBalancedCollection;
			return true;
		}

		public bool QuickMatch(RoutingServerInfoMap other)
		{
			return this.serversByGuid.Count == other.serversByGuid.Count && this.siteRelayMap.QuickMatch(other.siteRelayMap) && this.routingGroupRelayMap.QuickMatch(other.routingGroupRelayMap) && this.QuickFrontendServersInLocalSiteMatch(other.frontendServersInLocalSite);
		}

		public bool FullMatch(RoutingServerInfoMap other)
		{
			return this.FullServersMatch(other) && this.siteRelayMap.FullMatch(other.siteRelayMap) && this.routingGroupRelayMap.FullMatch(other.routingGroupRelayMap);
		}

		private Proximity TryGetServerRouteByDN(string serverDN, Proximity maxProximity, out RoutingServerInfo serverInfo, out RouteInfo routeInfo)
		{
			routeInfo = null;
			RoutingUtils.ThrowIfNullOrEmpty(serverDN, "serverDN");
			if (!this.TryGetServerInfoByDN(serverDN, out serverInfo))
			{
				return Proximity.None;
			}
			Proximity proximity = this.GetProximity(serverInfo);
			if (proximity > maxProximity)
			{
				return proximity;
			}
			return this.TryGetServerRoute(serverInfo, proximity, out routeInfo);
		}

		private Proximity TryGetServerRouteByFqdn(string serverFqdn, out RoutingServerInfo serverInfo, out RouteInfo routeInfo)
		{
			routeInfo = null;
			RoutingUtils.ThrowIfNullOrEmpty(serverFqdn, "serverFqdn");
			if (!this.TryGetServerInfoByFqdn(serverFqdn, out serverInfo))
			{
				return Proximity.None;
			}
			return this.TryGetServerRoute(serverInfo, this.GetProximity(serverInfo), out routeInfo);
		}

		private Proximity TryGetServerRoute(RoutingServerInfo serverInfo, Proximity serverProximity, out RouteInfo routeInfo)
		{
			routeInfo = null;
			switch (serverProximity)
			{
			case Proximity.LocalServer:
				routeInfo = RouteInfo.LocalServerRoute;
				break;
			case Proximity.LocalADSite:
				routeInfo = RouteInfo.LocalSiteRoute;
				break;
			case Proximity.RemoteADSite:
				this.siteRelayMap.TryGetRouteInfo(serverInfo.ADSite, out routeInfo);
				break;
			case Proximity.RemoteRoutingGroup:
				if (this.routingGroupRelayMap != null)
				{
					this.routingGroupRelayMap.TryGetRouteInfo(serverInfo.HomeRoutingGroup, out routeInfo);
				}
				break;
			default:
				throw new ArgumentOutOfRangeException("serverProximity", serverProximity, "Unexpected server proximity value: " + serverProximity);
			}
			if (routeInfo != null)
			{
				return routeInfo.DestinationProximity;
			}
			return Proximity.None;
		}

		private Proximity GetProximity(RoutingServerInfo serverInfo)
		{
			if (this.IsLocalServer(serverInfo))
			{
				return Proximity.LocalServer;
			}
			if (!serverInfo.IsExchange2007OrLater)
			{
				return Proximity.RemoteRoutingGroup;
			}
			if (!this.IsInLocalSite(serverInfo))
			{
				return Proximity.RemoteADSite;
			}
			return Proximity.LocalADSite;
		}

		private IEnumerable<RoutingServerInfo> GetServers(Predicate<RoutingServerInfo> filter)
		{
			foreach (RoutingServerInfo server in this.serversByGuid.Values)
			{
				if (filter(server))
				{
					yield return server;
				}
			}
			yield break;
		}

		private bool ShouldExcludeTopologyServer(TopologyServer server)
		{
			if (server.IsExchange2007OrLater && !server.IsFrontendTransportServer && !server.IsHubTransportServer && !server.IsMailboxServer && !server.IsEdgeServer)
			{
				RoutingDiag.Tracer.TraceDebug<DateTime, string>((long)this.GetHashCode(), "[{0}] Skipping server {1} because it does not contain roles relevant to routing.", this.whenCreated, server.DistinguishedName);
				return true;
			}
			if (string.IsNullOrEmpty(server.Fqdn))
			{
				RoutingDiag.Tracer.TraceError<DateTime, string>((long)this.GetHashCode(), "[{0}] No FQDN for Server object with DN: {1}. Skipping it.", this.whenCreated, server.DistinguishedName);
				RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingNoServerFqdn, null, new object[]
				{
					server.DistinguishedName,
					this.whenCreated
				});
				return true;
			}
			if (server.IsExchange2007OrLater)
			{
				if (server.TopologySite == null)
				{
					RoutingDiag.Tracer.TraceError<DateTime, string>((long)this.GetHashCode(), "[{0}] AD site for server '{1}' was not determined. Skipping the server.", this.whenCreated, server.Fqdn);
					RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingNoServerAdSite, null, new object[]
					{
						server.Fqdn,
						this.whenCreated
					});
					return true;
				}
			}
			else if (server.HomeRoutingGroup == null)
			{
				RoutingDiag.Tracer.TraceError<DateTime, string>((long)this.GetHashCode(), "[{0}] Routing group for server '{1}' was not determined. Skipping the server.", this.whenCreated, server.Fqdn);
				RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingNoServerRg, null, new object[]
				{
					server.Fqdn,
					this.whenCreated
				});
				return true;
			}
			return false;
		}

		private void AddExternalPostmasterAddress(TopologyServer server)
		{
			RoutingAddress item;
			if (RoutingUtils.TryConvertToRoutingAddress(server.ExternalPostmasterAddress, out item) && !this.externalPostmasterAddresses.Contains(item))
			{
				this.externalPostmasterAddresses.Add(item);
			}
		}

		private bool QuickFrontendServersInLocalSiteMatch(ListLoadBalancer<RoutingServerInfo> other)
		{
			return RoutingUtils.NullMatch(this.frontendServersInLocalSite, other) && (this.frontendServersInLocalSite == null || this.frontendServersInLocalSite.Count == other.Count);
		}

		private bool FullServersMatch(RoutingServerInfoMap other)
		{
			return RoutingUtils.MatchDictionaries<Guid, RoutingServerInfo>(this.serversByGuid, other.serversByGuid, (RoutingServerInfo serverInfo1, RoutingServerInfo serverInfo2) => serverInfo1.Match(serverInfo2));
		}

		private const int InitialCapacity = 64;

		private readonly DateTime whenCreated;

		private readonly Dictionary<Guid, RoutingServerInfo> serversByGuid;

		private readonly Dictionary<string, RoutingServerInfo> serversByDN;

		private readonly Dictionary<string, RoutingServerInfo> serversByFqdn;

		private readonly Dictionary<string, RoutingServerInfo> serversByLegacyDN;

		private readonly TopologyServer localServer;

		private readonly ADSiteRelayMap siteRelayMap;

		private readonly RoutingGroupRelayMap routingGroupRelayMap;

		private readonly ListLoadBalancer<RoutingServerInfo> frontendServersInLocalSite;

		private readonly IList<RoutingAddress> externalPostmasterAddresses;
	}
}
