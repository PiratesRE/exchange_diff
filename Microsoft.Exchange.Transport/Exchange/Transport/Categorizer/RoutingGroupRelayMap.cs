using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Transport.Common;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class RoutingGroupRelayMap : TopologyRelayMap<RoutingGroupRelayMap.RGTopologyPath>
	{
		public RoutingGroupRelayMap(RoutingTopologyBase topologyConfig, RoutingServerInfoMap serverMap, RoutingContextCore contextCore) : base(topologyConfig.RoutingGroups.Count, topologyConfig.WhenCreated)
		{
			RoutingGroupRelayMap.RGTopologySite localSite = this.CalculateRGTopology(topologyConfig, serverMap, contextCore);
			base.CalculateRoutes(localSite, contextCore);
			this.MapRoutesByRoutingGroupDN();
		}

		public IEnumerable<KeyValuePair<Guid, RouteInfo>> RoutesToRGConnectors
		{
			get
			{
				return this.routesToRGConnectors;
			}
		}

		public static void ValidateTopologyConfig(RoutingTopologyBase topologyConfig)
		{
			if (topologyConfig.RoutingGroups.Count == 0)
			{
				RoutingDiag.Tracer.TraceError<DateTime>(0L, "[{0}] No Routing Groups found", topologyConfig.WhenCreated);
				throw new TransientRoutingException(Strings.RoutingNoRoutingGroups);
			}
			if (topologyConfig.LocalServer.HomeRoutingGroup == null)
			{
				RoutingDiag.Tracer.TraceError<DateTime>(0L, "[{0}] Local server is not a member of any RG", topologyConfig.WhenCreated);
				throw new TransientRoutingException(Strings.RoutingLocalRgNotSet);
			}
			bool flag = false;
			Guid objectGuid = topologyConfig.LocalServer.HomeRoutingGroup.ObjectGuid;
			foreach (RoutingGroup routingGroup in topologyConfig.RoutingGroups)
			{
				if (objectGuid.Equals(routingGroup.Guid))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				RoutingDiag.Tracer.TraceError<DateTime, ADObjectId>(0L, "[{0}] Local RG object '{1}' not found.", topologyConfig.WhenCreated, topologyConfig.LocalServer.HomeRoutingGroup);
				throw new TransientRoutingException(Strings.RoutingNoLocalRgObject);
			}
		}

		public bool TryGetRouteInfo(ADObjectId routingGroupId, out RouteInfo routeInfo)
		{
			routeInfo = null;
			RoutingUtils.ThrowIfNull(routingGroupId, "routingGroupId");
			RoutingUtils.ThrowIfNullOrEmpty(routingGroupId.DistinguishedName, "routingGroupId.DistinguishedName");
			return this.routesByDN.TryGetValue(routingGroupId.DistinguishedName, out routeInfo);
		}

		public bool QuickMatch(RoutingGroupRelayMap other)
		{
			return this.routesByDN.Count == other.routesByDN.Count && this.routesToRGConnectors.Count == other.routesToRGConnectors.Count;
		}

		public bool FullMatch(RoutingGroupRelayMap other)
		{
			return RoutingUtils.MatchRouteDictionaries<string>(this.routesByDN, other.routesByDN, NextHopMatch.GuidOnly) && RoutingUtils.MatchRouteDictionaries<Guid>(this.routesToRGConnectors, other.routesToRGConnectors);
		}

		protected override RoutingGroupRelayMap.RGTopologyPath CreateTopologyPath(RoutingGroupRelayMap.RGTopologyPath prePath, ITopologySite targetSite, ITopologySiteLink link, RoutingContextCore contextCore)
		{
			return new RoutingGroupRelayMap.RGTopologyPath(prePath, targetSite, link, this.routesToRGConnectors);
		}

		private RoutingGroupRelayMap.RGTopologySite CalculateRGTopology(RoutingTopologyBase topologyConfig, RoutingServerInfoMap serverMap, RoutingContextCore contextCore)
		{
			base.DebugTrace("Calculating routing group topology");
			Dictionary<string, RoutingGroupRelayMap.RGTopologySite> dictionary = new Dictionary<string, RoutingGroupRelayMap.RGTopologySite>(topologyConfig.RoutingGroups.Count, StringComparer.OrdinalIgnoreCase);
			Dictionary<Guid, RoutingGroupRelayMap.RGTopologySite> dictionary2 = new Dictionary<Guid, RoutingGroupRelayMap.RGTopologySite>(topologyConfig.RoutingGroups.Count);
			foreach (RoutingGroup routingGroup in topologyConfig.RoutingGroups)
			{
				RoutingGroupRelayMap.RGTopologySite valueToAdd = new RoutingGroupRelayMap.RGTopologySite(routingGroup);
				if (TransportHelpers.AttemptAddToDictionary<string, RoutingGroupRelayMap.RGTopologySite>(dictionary, routingGroup.DistinguishedName, valueToAdd, new TransportHelpers.DiagnosticsHandler<string, RoutingGroupRelayMap.RGTopologySite>(RoutingUtils.LogErrorWhenAddToDictionaryFails<string, RoutingGroupRelayMap.RGTopologySite>)) && !TransportHelpers.AttemptAddToDictionary<Guid, RoutingGroupRelayMap.RGTopologySite>(dictionary2, routingGroup.Guid, valueToAdd, new TransportHelpers.DiagnosticsHandler<Guid, RoutingGroupRelayMap.RGTopologySite>(RoutingUtils.LogErrorWhenAddToDictionaryFails<Guid, RoutingGroupRelayMap.RGTopologySite>)))
				{
					dictionary.Remove(routingGroup.DistinguishedName);
				}
			}
			ADObjectId homeRoutingGroup = topologyConfig.LocalServer.HomeRoutingGroup;
			RoutingGroupRelayMap.RGTopologySite rgtopologySite = dictionary2[homeRoutingGroup.ObjectGuid];
			this.routesToRGConnectors = new Dictionary<Guid, RouteInfo>();
			foreach (RoutingGroupConnector connector in topologyConfig.RoutingGroupConnectors)
			{
				this.AddRGConnector(connector, rgtopologySite, dictionary, topologyConfig, serverMap, contextCore);
			}
			foreach (MailGateway mailGateway in topologyConfig.SendConnectors)
			{
				if (!MultiValuedPropertyBase.IsNullOrEmpty(mailGateway.ConnectedDomains))
				{
					this.AddConnectorWithConnectedDomains(mailGateway, rgtopologySite, dictionary, dictionary2);
				}
			}
			base.DebugTrace("Calculated routing group topology");
			return rgtopologySite;
		}

		private void AddRGConnector(RoutingGroupConnector connector, RoutingGroupRelayMap.RGTopologySite localRoutingGroup, Dictionary<string, RoutingGroupRelayMap.RGTopologySite> routingGroupsByDN, RoutingTopologyBase topologyConfig, RoutingServerInfoMap serverMap, RoutingContextCore contextCore)
		{
			RoutingGroupRelayMap.RGTopologySite rgtopologySite = null;
			if (!routingGroupsByDN.TryGetValue(connector.SourceRoutingGroup.DistinguishedName, out rgtopologySite))
			{
				RoutingDiag.Tracer.TraceError<DateTime, string, string>((long)this.GetHashCode(), "[{0}] Source routing group '{1}' not found for routing group connector '{2}'; skipping the connector.", this.timestamp, connector.SourceRoutingGroup.DistinguishedName, connector.DistinguishedName);
				RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingNoSourceRgForRgConnector, null, new object[]
				{
					connector.SourceRoutingGroup.DistinguishedName,
					connector.DistinguishedName,
					this.timestamp
				});
				return;
			}
			RoutingGroupRelayMap.RGTopologySite rgtopologySite2 = null;
			ADObjectId targetRoutingGroup = connector.TargetRoutingGroup;
			string text;
			if (targetRoutingGroup != null)
			{
				RoutingUtils.ThrowIfEmpty(targetRoutingGroup, "rgc.TargetRoutingGroup");
				text = targetRoutingGroup.DistinguishedName;
				routingGroupsByDN.TryGetValue(text, out rgtopologySite2);
			}
			else
			{
				text = "<<null>>";
			}
			if (rgtopologySite2 == null)
			{
				RoutingDiag.Tracer.TraceError<DateTime, string, string>((long)this.GetHashCode(), "[{0}] Target routing group '{1}' not found for routing group connector '{2}'; skipping the connector.", this.timestamp, text, connector.DistinguishedName);
				RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingNoTargetRgForRgConnector, null, new object[]
				{
					text,
					connector.DistinguishedName,
					this.timestamp
				});
				return;
			}
			if (object.ReferenceEquals(localRoutingGroup, rgtopologySite))
			{
				RouteInfo valueToAdd = null;
				if (!this.TryCalculateRGConnectorRoute(connector, topologyConfig, serverMap, contextCore, out valueToAdd))
				{
					return;
				}
				TransportHelpers.AttemptAddToDictionary<Guid, RouteInfo>(this.routesToRGConnectors, connector.Guid, valueToAdd, new TransportHelpers.DiagnosticsHandler<Guid, RouteInfo>(RoutingUtils.LogErrorWhenAddToDictionaryFails<Guid, RouteInfo>));
			}
			RoutingGroupRelayMap.RGTopologyLink rgtopologyLink = new RoutingGroupRelayMap.RGTopologyLink(connector, rgtopologySite2, connector.Cost);
			rgtopologySite.AddLink(rgtopologyLink);
			RoutingDiag.Tracer.TraceDebug((long)this.GetHashCode(), "[{0}] Added link for routing group connector '{1}'; source RG '{2}'; target RG '{3}'; cost={4}.", new object[]
			{
				this.timestamp,
				connector.DistinguishedName,
				rgtopologySite.Name,
				rgtopologySite2.Name,
				rgtopologyLink.Cost
			});
		}

		private void AddConnectorWithConnectedDomains(MailGateway connector, RoutingGroupRelayMap.RGTopologySite localRoutingGroup, Dictionary<string, RoutingGroupRelayMap.RGTopologySite> routingGroupsByDN, Dictionary<Guid, RoutingGroupRelayMap.RGTopologySite> routingGroupsByGuid)
		{
			RoutingGroupRelayMap.RGTopologySite rgtopologySite;
			if (!routingGroupsByDN.TryGetValue(connector.SourceRoutingGroup.DistinguishedName, out rgtopologySite))
			{
				RoutingDiag.Tracer.TraceError<DateTime, string, string>((long)this.GetHashCode(), "[{0}] Source routing group '{1}' not found for non-routing-group connector '{2}'; skipping the connector.", this.timestamp, connector.SourceRoutingGroup.DistinguishedName, connector.DistinguishedName);
				RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingNoSourceRgForNonRgConnector, null, new object[]
				{
					connector.SourceRoutingGroup.DistinguishedName,
					connector.DistinguishedName,
					this.timestamp
				});
				return;
			}
			if (object.ReferenceEquals(localRoutingGroup, rgtopologySite))
			{
				RoutingDiag.Tracer.TraceError<DateTime, string, string>((long)this.GetHashCode(), "[{0}] Source routing group '{1}' is local for non-routing-group connector '{2}'; skipping the connector.", this.timestamp, rgtopologySite.Name, connector.DistinguishedName);
				RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingLocalConnectorWithConnectedDomains, null, new object[]
				{
					rgtopologySite.Name,
					connector.DistinguishedName,
					this.timestamp
				});
				return;
			}
			foreach (ConnectedDomain connectedDomain in connector.ConnectedDomains)
			{
				RoutingGroupRelayMap.RGTopologySite rgtopologySite2;
				if (!routingGroupsByGuid.TryGetValue(connectedDomain.RoutingGroupGuid, out rgtopologySite2))
				{
					RoutingDiag.Tracer.TraceError<DateTime, Guid, string>((long)this.GetHashCode(), "[{0}] Target routing group '{1}' not found for non-routing-group connector '{2}'; skipping the connector.", this.timestamp, connectedDomain.RoutingGroupGuid, connector.DistinguishedName);
					RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingNoConnectedRg, null, new object[]
					{
						connectedDomain.RoutingGroupGuid,
						connector.DistinguishedName,
						this.timestamp
					});
					break;
				}
				RoutingGroupRelayMap.RGTopologyLink rgtopologyLink = new RoutingGroupRelayMap.RGTopologyLink(connector, rgtopologySite2, connectedDomain.Cost);
				rgtopologySite.AddLink(rgtopologyLink);
				RoutingDiag.Tracer.TraceDebug((long)this.GetHashCode(), "[{0}] Added link for non-routing-group connector '{1}'; source RG '{2}'; target RG '{3}'; cost={4}.", new object[]
				{
					this.timestamp,
					connector.DistinguishedName,
					rgtopologySite.Name,
					rgtopologySite2.Name,
					rgtopologyLink.Cost
				});
			}
		}

		private void MapRoutesByRoutingGroupDN()
		{
			this.routesByDN = new Dictionary<string, RouteInfo>(this.routes.Count);
			foreach (RoutingGroupRelayMap.RGTopologyPath rgtopologyPath in this.routes.Values)
			{
				RouteInfo valueToAdd = RouteInfo.CreateForRemoteRG(rgtopologyPath.TargetObjectId.DistinguishedName, rgtopologyPath.MaxMessageSize, rgtopologyPath.TotalCost, rgtopologyPath.FirstRGConnectorRoute);
				TransportHelpers.AttemptAddToDictionary<string, RouteInfo>(this.routesByDN, rgtopologyPath.TargetObjectId.DistinguishedName, valueToAdd, new TransportHelpers.DiagnosticsHandler<string, RouteInfo>(RoutingUtils.LogErrorWhenAddToDictionaryFails<string, RouteInfo>));
			}
		}

		private bool TryCalculateRGConnectorRoute(RoutingGroupConnector connector, RoutingTopologyBase topologyConfig, RoutingServerInfoMap serverMap, RoutingContextCore contextCore, out RouteInfo routeInfo)
		{
			ADObjectId id = topologyConfig.LocalServer.Id;
			if (!ConnectorRouteFactory.TryCalculateConnectorRoute(connector, id, serverMap, contextCore, out routeInfo))
			{
				return false;
			}
			if (routeInfo.DestinationProximity == Proximity.LocalServer)
			{
				ListLoadBalancer<INextHopServer> targetBridgeheads;
				if (!this.TryGetTargetBridgeheads(connector, serverMap, contextCore, out targetBridgeheads))
				{
					routeInfo = null;
					return false;
				}
				((ConnectorDeliveryHop)routeInfo.NextHop).SetTargetBridgeheads(targetBridgeheads);
			}
			return true;
		}

		private bool TryGetTargetBridgeheads(RoutingGroupConnector connector, RoutingServerInfoMap serverMap, RoutingContextCore contextCore, out ListLoadBalancer<INextHopServer> targetBridgeheads)
		{
			targetBridgeheads = null;
			if (!MultiValuedPropertyBase.IsNullOrEmpty(connector.TargetTransportServers))
			{
				foreach (ADObjectId adobjectId in connector.TargetTransportServers)
				{
					RoutingServerInfo item;
					if (serverMap.TryGetServerInfoByDN(adobjectId.DistinguishedName, out item))
					{
						RoutingUtils.AddItemToLazyList<INextHopServer>(item, contextCore.Settings.RandomLoadBalancingOffsetEnabled, ref targetBridgeheads);
					}
					else
					{
						RoutingDiag.Tracer.TraceError<DateTime, string, string>((long)this.GetHashCode(), "[{0}] Target Bridgehead server '{1}' not found for RG connector '{2}'. Skipping the server.", this.timestamp, adobjectId.DistinguishedName, connector.DistinguishedName);
						RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingNoTargetBhServer, null, new object[]
						{
							adobjectId.DistinguishedName,
							connector.DistinguishedName,
							this.timestamp
						});
					}
				}
			}
			if (targetBridgeheads == null)
			{
				RoutingDiag.Tracer.TraceError<DateTime, string>((long)this.GetHashCode(), "[{0}] No target Bridgehead servers found for RG connector '{1}'. Skipping the connector.", this.timestamp, connector.DistinguishedName);
				RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingNoTargetBhServers, null, new object[]
				{
					connector.DistinguishedName,
					this.timestamp
				});
				return false;
			}
			return true;
		}

		private Dictionary<string, RouteInfo> routesByDN;

		private Dictionary<Guid, RouteInfo> routesToRGConnectors;

		internal class RGTopologyPath : TopologyPath
		{
			public RGTopologyPath(RoutingGroupRelayMap.RGTopologyPath prePath, ITopologySite targetRoutingGroup, ITopologySiteLink connector, Dictionary<Guid, RouteInfo> routesToRGConnectors)
			{
				this.targetRoutingGroup = (RoutingGroupRelayMap.RGTopologySite)targetRoutingGroup;
				this.routesToRGConnectors = routesToRGConnectors;
				if (prePath != null)
				{
					this.firstRGConnectorRoute = prePath.firstRGConnectorRoute;
					this.totalCost = prePath.totalCost + connector.Cost;
					this.maxMessageSize = Math.Min(prePath.MaxMessageSize, (long)connector.AbsoluteMaxMessageSize);
					return;
				}
				this.firstRGConnectorRoute = this.routesToRGConnectors[((RoutingGroupRelayMap.RGTopologyLink)connector).ConnectorGuid];
				this.totalCost = connector.Cost;
				this.maxMessageSize = Math.Min((long)connector.AbsoluteMaxMessageSize, this.firstRGConnectorRoute.MaxMessageSize);
			}

			public override int TotalCost
			{
				get
				{
					return this.totalCost;
				}
			}

			public long MaxMessageSize
			{
				get
				{
					return this.maxMessageSize;
				}
			}

			public ADObjectId TargetObjectId
			{
				get
				{
					return this.targetRoutingGroup.ObjectId;
				}
			}

			public RouteInfo FirstRGConnectorRoute
			{
				get
				{
					return this.firstRGConnectorRoute;
				}
			}

			public override void ReplaceIfBetter(TopologyPath newPrePath, ITopologySiteLink newLink, DateTime timestamp)
			{
				RoutingGroupRelayMap.RGTopologyPath rgtopologyPath = (RoutingGroupRelayMap.RGTopologyPath)newPrePath;
				int num = this.TotalCost;
				int num2 = newLink.Cost;
				if (rgtopologyPath != null)
				{
					num2 += rgtopologyPath.TotalCost;
				}
				if (num2 > num)
				{
					return;
				}
				RouteInfo other;
				if (rgtopologyPath != null)
				{
					other = rgtopologyPath.firstRGConnectorRoute;
				}
				else
				{
					other = this.routesToRGConnectors[((RoutingGroupRelayMap.RGTopologyLink)newLink).ConnectorGuid];
				}
				if (num2 == num && this.firstRGConnectorRoute.CompareTo(other, RouteComparison.CompareNames) <= 0)
				{
					return;
				}
				this.firstRGConnectorRoute = other;
				this.totalCost = num2;
				this.maxMessageSize = (long)newLink.AbsoluteMaxMessageSize;
				if (rgtopologyPath != null)
				{
					this.maxMessageSize = Math.Min(this.maxMessageSize, rgtopologyPath.MaxMessageSize);
				}
				RoutingDiag.Tracer.TraceDebug<DateTime, RoutingGroupRelayMap.RGTopologyPath, long>((long)this.GetHashCode(), "[{0}] [LCP] Replaced with better RG path: {1}, MaxMessageSize:{2}", timestamp, this, this.maxMessageSize);
			}

			private RoutingGroupRelayMap.RGTopologySite targetRoutingGroup;

			private RouteInfo firstRGConnectorRoute;

			private int totalCost;

			private long maxMessageSize;

			private Dictionary<Guid, RouteInfo> routesToRGConnectors;
		}

		internal class RGTopologySite : ITopologySite
		{
			public RGTopologySite(RoutingGroup rg)
			{
				this.rg = rg;
				this.connectors = new List<ITopologySiteLink>();
				this.siteLinks = new ReadOnlyCollection<ITopologySiteLink>(this.connectors);
			}

			public Guid Guid
			{
				get
				{
					return this.rg.Guid;
				}
			}

			public string Name
			{
				get
				{
					return this.rg.DistinguishedName;
				}
			}

			public ADObjectId ObjectId
			{
				get
				{
					return this.rg.Id;
				}
			}

			public ReadOnlyCollection<ITopologySiteLink> TopologySiteLinks
			{
				get
				{
					return this.siteLinks;
				}
			}

			public void AddLink(RoutingGroupRelayMap.RGTopologyLink connector)
			{
				this.connectors.Add(connector);
			}

			private RoutingGroup rg;

			private List<ITopologySiteLink> connectors;

			private ReadOnlyCollection<ITopologySiteLink> siteLinks;
		}

		internal class RGTopologyLink : ITopologySiteLink
		{
			public RGTopologyLink(SendConnector connector, RoutingGroupRelayMap.RGTopologySite targetRoutingGroup, int cost)
			{
				this.connector = connector;
				this.cost = cost;
				this.topologySites = new ReadOnlyCollection<ITopologySite>(new ITopologySite[]
				{
					targetRoutingGroup
				});
			}

			public string Name
			{
				get
				{
					return this.connector.DistinguishedName;
				}
			}

			public int Cost
			{
				get
				{
					return this.cost;
				}
			}

			public Unlimited<ByteQuantifiedSize> MaxMessageSize
			{
				get
				{
					return this.connector.MaxMessageSize;
				}
			}

			public ulong AbsoluteMaxMessageSize
			{
				get
				{
					return this.connector.AbsoluteMaxMessageSize;
				}
			}

			public ReadOnlyCollection<ITopologySite> TopologySites
			{
				get
				{
					return this.topologySites;
				}
			}

			public Guid ConnectorGuid
			{
				get
				{
					return this.connector.Guid;
				}
			}

			private readonly int cost;

			private SendConnector connector;

			private ReadOnlyCollection<ITopologySite> topologySites;
		}
	}
}
