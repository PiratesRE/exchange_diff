using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Transport.Common;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class ADSiteRelayMap : TopologyRelayMap<ADSiteRelayMap.ADTopologyPath>
	{
		public ADSiteRelayMap(RoutingTopologyBase topologyConfig, RoutingServerInfoMap serverMap, RoutingContextCore contextCore) : base(topologyConfig.Sites.Count, topologyConfig.WhenCreated)
		{
			ITopologySite topologySite = topologyConfig.LocalServer.TopologySite;
			if (topologyConfig.Sites.Count != 1)
			{
				base.CalculateRoutes(topologySite, contextCore);
				this.PopulateSitesWithHubServers(topologyConfig, serverMap);
				this.Normalize(contextCore);
				this.MapRoutesBySiteGuid();
				return;
			}
			if (topologyConfig.Sites[0] != topologySite)
			{
				throw new InvalidOperationException("Sites property does not contain LocalSite");
			}
			base.DebugTrace("No remote AD sites found; ADSiteRelayMap is empty");
			this.routesByGuid = new Dictionary<Guid, RouteInfo>();
		}

		public bool TryGetRouteInfo(ADObjectId siteId, out RouteInfo routeInfo)
		{
			RoutingUtils.ThrowIfNullOrEmpty(siteId, "siteId");
			return this.routesByGuid.TryGetValue(siteId.ObjectGuid, out routeInfo);
		}

		public bool TryGetPath(Guid siteGuid, out ADSiteRelayMap.ADTopologyPath path)
		{
			path = null;
			RouteInfo routeInfo;
			if (!this.routesByGuid.TryGetValue(siteGuid, out routeInfo))
			{
				return false;
			}
			path = ((ADSiteRelayMap.SiteNextHop)routeInfo.NextHop).TargetPath;
			return true;
		}

		public bool TryGetNextHop(NextHopSolutionKey nextHopKey, out RoutingNextHop nextHop)
		{
			nextHop = null;
			RouteInfo routeInfo;
			if (!this.routesByGuid.TryGetValue(nextHopKey.NextHopConnector, out routeInfo))
			{
				RoutingDiag.Tracer.TraceError<DateTime, NextHopSolutionKey>(0L, "[{0}] Target AD Site is not found for next hop key <{1}>", this.timestamp, nextHopKey);
				return false;
			}
			nextHop = routeInfo.NextHop;
			return true;
		}

		public bool QuickMatch(ADSiteRelayMap other)
		{
			return this.routesByGuid.Count == other.routesByGuid.Count;
		}

		public bool FullMatch(ADSiteRelayMap other)
		{
			return RoutingUtils.MatchRouteDictionaries<Guid>(this.routesByGuid, other.routesByGuid);
		}

		protected override ADSiteRelayMap.ADTopologyPath CreateTopologyPath(ADSiteRelayMap.ADTopologyPath prePath, ITopologySite targetSite, ITopologySiteLink link, RoutingContextCore contextCore)
		{
			return new ADSiteRelayMap.ADTopologyPath(prePath, this.timestamp, targetSite, link, contextCore);
		}

		private void PopulateSitesWithHubServers(RoutingTopologyBase topologyConfig, RoutingServerInfoMap serverMap)
		{
			base.DebugTrace("Populating remote AD sites with Hub servers");
			Dictionary<Guid, TopologySite> dictionary = new Dictionary<Guid, TopologySite>(topologyConfig.Sites.Count);
			foreach (TopologySite topologySite in topologyConfig.Sites)
			{
				TransportHelpers.AttemptAddToDictionary<Guid, TopologySite>(dictionary, topologySite.Guid, topologySite, new TransportHelpers.DiagnosticsHandler<Guid, TopologySite>(RoutingUtils.LogErrorWhenAddToDictionaryFails<Guid, TopologySite>));
			}
			foreach (RoutingServerInfo routingServerInfo in serverMap.GetHubTransportServers())
			{
				if (!serverMap.IsInLocalSite(routingServerInfo))
				{
					TopologySite key;
					ADSiteRelayMap.ADTopologyPath adtopologyPath;
					if (dictionary.TryGetValue(routingServerInfo.ADSite.ObjectGuid, out key) && this.routes.TryGetValue(key, out adtopologyPath))
					{
						adtopologyPath.TargetSite.AddHubServer(routingServerInfo);
						RoutingDiag.Tracer.TraceDebug<DateTime, string, string>((long)this.GetHashCode(), "[{0}] Added Hub server {1} to target AD site {2}", this.timestamp, routingServerInfo.Fqdn, routingServerInfo.ADSite.Name);
					}
					else
					{
						RoutingDiag.Tracer.TraceError<DateTime, string, string>((long)this.GetHashCode(), "[{0}] Route for Hub server {1} from AD site {2} not found while populating sites with Hub servers", this.timestamp, routingServerInfo.Fqdn, routingServerInfo.ADSite.Name);
						RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingNoRouteToAdSite, null, new object[]
						{
							routingServerInfo.ADSite.Name,
							this.timestamp,
							routingServerInfo.Fqdn
						});
					}
				}
			}
		}

		private void Normalize(RoutingContextCore context)
		{
			base.DebugTrace("Normalizing AD site relay map");
			List<ITopologySite> list = new List<ITopologySite>();
			foreach (KeyValuePair<ITopologySite, ADSiteRelayMap.ADTopologyPath> keyValuePair in this.routes)
			{
				if (keyValuePair.Value.TargetSite.HasHubServers)
				{
					keyValuePair.Value.TargetSite.UpdateSiteStateAndRemoveInactiveServers(context);
				}
				else
				{
					list.Add(keyValuePair.Key);
				}
			}
			if (list.Count > 0)
			{
				foreach (ITopologySite topologySite in list)
				{
					RoutingDiag.Tracer.TraceDebug<DateTime, string>((long)this.GetHashCode(), "[{0}] Removing AD site {1} from the relay map because it does not have any Hub servers", this.timestamp, topologySite.Name);
					this.routes.Remove(topologySite);
				}
				foreach (ADSiteRelayMap.ADTopologyPath adtopologyPath in this.routes.Values)
				{
					adtopologyPath.Normalize(this.timestamp);
				}
			}
		}

		private void MapRoutesBySiteGuid()
		{
			this.routesByGuid = new Dictionary<Guid, RouteInfo>(this.routes.Count);
			foreach (ADSiteRelayMap.ADTopologyPath adtopologyPath in this.routes.Values)
			{
				ADSiteRelayMap.SiteNextHop nextHop = new ADSiteRelayMap.SiteNextHop(adtopologyPath.NextHopPath);
				RouteInfo routeInfo = RouteInfo.CreateForRemoteSite(adtopologyPath.TargetSite.Name, nextHop, adtopologyPath.MaxMessageSize, adtopologyPath.TotalCost);
				adtopologyPath.RouteInfo = routeInfo;
				TransportHelpers.AttemptAddToDictionary<Guid, RouteInfo>(this.routesByGuid, adtopologyPath.TargetSite.Guid, routeInfo, new TransportHelpers.DiagnosticsHandler<Guid, RouteInfo>(RoutingUtils.LogErrorWhenAddToDictionaryFails<Guid, RouteInfo>));
			}
		}

		public const int BinaryBackoffThreshold = 4;

		private Dictionary<Guid, RouteInfo> routesByGuid;

		internal class ADTopologyPath : TopologyPath
		{
			public ADTopologyPath(ADSiteRelayMap.ADTopologyPath prePath, DateTime timestamp, ITopologySite targetSite, ITopologySiteLink link, RoutingContextCore contextCore)
			{
				this.targetSite = new ADSiteRelayMap.TargetSite(targetSite, contextCore);
				this.prePath = prePath;
				this.link = link;
				this.totalCost = link.Cost;
				this.maxMessageSize = (long)((link.AbsoluteMaxMessageSize > 9223372036854775807UL) ? 9223372036854775807UL : link.AbsoluteMaxMessageSize);
				if (prePath != null)
				{
					this.totalCost += prePath.totalCost;
					this.SetHubPath(timestamp);
					if (prePath.MaxMessageSize < this.maxMessageSize)
					{
						this.maxMessageSize = prePath.MaxMessageSize;
					}
				}
			}

			public ADSiteRelayMap.TargetSite TargetSite
			{
				get
				{
					return this.targetSite;
				}
			}

			public ADSiteRelayMap.ADTopologyPath NextHopPath
			{
				get
				{
					if (this.hubPath == null)
					{
						return this;
					}
					return this.hubPath;
				}
			}

			public ADSiteRelayMap.TargetSite NextHopSite
			{
				get
				{
					return this.NextHopPath.targetSite;
				}
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

			public RouteInfo RouteInfo
			{
				get
				{
					return this.routeInfo;
				}
				set
				{
					this.routeInfo = value;
				}
			}

			public static ADSiteRelayMap.ADTopologyPath GetCommonPath(ADSiteRelayMap.ADTopologyPath path1, ADSiteRelayMap.ADTopologyPath path2)
			{
				if (object.ReferenceEquals(path1, path2))
				{
					return path1;
				}
				int num = path1.SegmentCount();
				int num2 = path2.SegmentCount();
				if (num != num2)
				{
					ADSiteRelayMap.ADTopologyPath adtopologyPath;
					int num3;
					if (num > num2)
					{
						adtopologyPath = path1;
						num3 = num - num2;
					}
					else
					{
						adtopologyPath = path2;
						num3 = num2 - num;
					}
					while (num3-- > 0)
					{
						adtopologyPath = adtopologyPath.prePath;
					}
					if (num > num2)
					{
						path1 = adtopologyPath;
					}
					else
					{
						path2 = adtopologyPath;
					}
				}
				while (!object.ReferenceEquals(path1, path2))
				{
					path1 = path1.prePath;
					path2 = path2.prePath;
				}
				return path1;
			}

			public Guid FirstHopSiteGuid()
			{
				if (this.prePath != null)
				{
					return this.prePath.FirstHopSiteGuid();
				}
				return this.targetSite.Guid;
			}

			public override string ToString()
			{
				return string.Format("{0} Link:'{1}',cost={2} Site:'{3}'", new object[]
				{
					this.prePath ?? "Site:<Local>",
					this.link.Name,
					this.link.Cost,
					this.targetSite.Site.Name
				});
			}

			public override void ReplaceIfBetter(TopologyPath newPrePath, ITopologySiteLink newLink, DateTime timestamp)
			{
				ADSiteRelayMap.ADTopologyPath adtopologyPath = (ADSiteRelayMap.ADTopologyPath)newPrePath;
				int num = this.TotalCost;
				int num2 = this.SegmentCount();
				int num3 = newLink.Cost;
				int num4 = 1;
				if (adtopologyPath != null)
				{
					num3 += adtopologyPath.TotalCost;
					num4 += adtopologyPath.SegmentCount();
				}
				if (num > num3 || (num == num3 && (num2 > num4 || (num2 == num4 && num2 > 1 && RoutingUtils.CompareNames(this.prePath.targetSite.Site.Name, adtopologyPath.targetSite.Site.Name) > 0))))
				{
					this.prePath = adtopologyPath;
					this.link = newLink;
					this.totalCost = num3;
					this.maxMessageSize = (long)((newLink.AbsoluteMaxMessageSize > 9223372036854775807UL) ? 9223372036854775807UL : newLink.AbsoluteMaxMessageSize);
					if (adtopologyPath != null && adtopologyPath.MaxMessageSize < this.maxMessageSize)
					{
						this.maxMessageSize = adtopologyPath.MaxMessageSize;
					}
					RoutingDiag.Tracer.TraceDebug<DateTime, ADSiteRelayMap.ADTopologyPath>((long)this.GetHashCode(), "[{0}] [LCP] Replaced with better path: {1}", timestamp, this);
					this.SetHubPath(timestamp);
				}
			}

			public void Normalize(DateTime timestamp)
			{
				while (this.prePath != null && !this.prePath.targetSite.HasHubServers)
				{
					RoutingDiag.Tracer.TraceDebug((long)this.GetHashCode(), "[{0}] Removing site '{1}' from the path to site '{2}' and combining links '{3}' and '{4}'", new object[]
					{
						timestamp,
						this.prePath.targetSite.Name,
						this.targetSite.Name,
						this.prePath.link.Name,
						this.link.Name
					});
					this.link = new ADSiteRelayMap.CombinedLink(this.link, this.prePath.link);
					this.prePath = this.prePath.prePath;
				}
				if (this.prePath != null && !this.prePath.normalized)
				{
					this.prePath.Normalize(timestamp);
				}
				this.normalized = true;
				this.SetHubPath(timestamp);
				RoutingDiag.Tracer.TraceDebug<DateTime, string>((long)this.GetHashCode(), "[{0}] Normalized path to AD site '{1}'", timestamp, this.targetSite.Name);
			}

			public IEnumerable<RoutingServerInfo> GetLoadBalancedHubServers()
			{
				foreach (ADSiteRelayMap.TargetSite site in this.GetBackoffSites())
				{
					foreach (RoutingServerInfo serverInfo in site.LoadBalancedHubServers)
					{
						yield return serverInfo;
					}
				}
				yield break;
			}

			public bool Match(ADSiteRelayMap.ADTopologyPath other)
			{
				return this.NextHopSite.Guid == other.NextHopSite.Guid && this.TargetSite.Match(other.TargetSite) && RoutingUtils.NullMatch(this.prePath, other.prePath);
			}

			private int SegmentCount()
			{
				int num = 0;
				for (ADSiteRelayMap.ADTopologyPath adtopologyPath = this; adtopologyPath != null; adtopologyPath = adtopologyPath.prePath)
				{
					num++;
				}
				return num;
			}

			private IEnumerable<ADSiteRelayMap.TargetSite> GetBackoffSites()
			{
				int segmentsLeft = this.SegmentCount();
				int numSkip = 0;
				bool destinationSite = true;
				for (ADSiteRelayMap.ADTopologyPath path = this; path != null; path = path.prePath)
				{
					segmentsLeft--;
					if (numSkip == 0)
					{
						if (destinationSite || path.targetSite.IsActive)
						{
							if (segmentsLeft > 4)
							{
								numSkip = segmentsLeft / 2;
								if (segmentsLeft - numSkip < 4)
								{
									numSkip = segmentsLeft - 4;
								}
							}
							destinationSite = false;
							yield return path.targetSite;
						}
					}
					else
					{
						numSkip--;
					}
				}
				yield break;
			}

			private void SetHubPath(DateTime timestamp)
			{
				this.hubPath = null;
				if (this.prePath != null)
				{
					if (this.prePath.hubPath != null)
					{
						this.hubPath = this.prePath.hubPath;
					}
					else
					{
						TopologySite site = this.prePath.TargetSite.Site;
						if (site.HubSiteEnabled)
						{
							this.hubPath = this.prePath;
						}
					}
				}
				if (this.hubPath != null)
				{
					RoutingDiag.Tracer.TraceDebug<DateTime, string, string>((long)this.GetHashCode(), "[{0}] Path to AD site '{1}' contains hub site {2}", timestamp, this.targetSite.Name, this.hubPath.targetSite.Name);
					return;
				}
				RoutingDiag.Tracer.TraceDebug<DateTime, string>((long)this.GetHashCode(), "[{0}] Path to AD site '{1}' does not contain a hub site", timestamp, this.targetSite.Name);
			}

			private ADSiteRelayMap.TargetSite targetSite;

			private ADSiteRelayMap.ADTopologyPath prePath;

			private ITopologySiteLink link;

			private int totalCost;

			private long maxMessageSize;

			private bool normalized;

			private ADSiteRelayMap.ADTopologyPath hubPath;

			private RouteInfo routeInfo;
		}

		internal class TargetSite
		{
			public TargetSite(ITopologySite site, RoutingContextCore contextCore)
			{
				this.Site = (site as TopologySite);
				this.hubServers = new ListLoadBalancer<RoutingServerInfo>(contextCore.Settings.RandomLoadBalancingOffsetEnabled);
			}

			public string Name
			{
				get
				{
					return this.Site.Name;
				}
			}

			public Guid Guid
			{
				get
				{
					return this.Site.Guid;
				}
			}

			public ADObjectId Id
			{
				get
				{
					return this.Site.Id;
				}
			}

			public bool HasHubServers
			{
				get
				{
					return !this.hubServers.IsEmpty;
				}
			}

			public int HubServerCount
			{
				get
				{
					return this.hubServers.Count;
				}
			}

			public ICollection<RoutingServerInfo> HubServers
			{
				get
				{
					return this.hubServers.NonLoadBalancedCollection;
				}
			}

			public ICollection<RoutingServerInfo> LoadBalancedHubServers
			{
				get
				{
					return this.hubServers.LoadBalancedCollection;
				}
			}

			public bool IsActive
			{
				get
				{
					if (this.isActive == null)
					{
						throw new InvalidOperationException("IsActive not computed");
					}
					return this.isActive.Value;
				}
			}

			public void AddHubServer(RoutingServerInfo server)
			{
				this.hubServers.AddItem(server);
			}

			public void UpdateSiteStateAndRemoveInactiveServers(RoutingContextCore context)
			{
				bool flag = false;
				List<RoutingServerInfo> list = null;
				foreach (RoutingServerInfo routingServerInfo in this.hubServers.NonLoadBalancedCollection)
				{
					if (context.VerifyHubComponentStateRestriction(routingServerInfo))
					{
						flag = true;
					}
					else
					{
						RoutingUtils.AddItemToLazyList<RoutingServerInfo>(routingServerInfo, ref list);
					}
				}
				if (flag && list != null)
				{
					foreach (RoutingServerInfo item in list)
					{
						this.hubServers.RemoveItem(item);
					}
				}
				this.isActive = new bool?(flag);
			}

			public bool Match(ADSiteRelayMap.TargetSite other)
			{
				return this.Site.InboundMailEnabled == other.Site.InboundMailEnabled && RoutingUtils.MatchStrings(this.Site.Name, other.Site.Name);
			}

			public readonly TopologySite Site;

			private ListLoadBalancer<RoutingServerInfo> hubServers;

			private bool? isActive;
		}

		private class CombinedLink : ITopologySiteLink
		{
			public CombinedLink(ITopologySiteLink link1, ITopologySiteLink link2)
			{
				this.link1 = link1;
				this.link2 = link2;
			}

			public string Name
			{
				get
				{
					return string.Format("Combined({0}, {1})", this.link1.Name, this.link2.Name);
				}
			}

			public int Cost
			{
				get
				{
					return this.link1.Cost + this.link2.Cost;
				}
			}

			public Unlimited<ByteQuantifiedSize> MaxMessageSize
			{
				get
				{
					if (this.link1.MaxMessageSize.CompareTo(this.link2.MaxMessageSize) >= 0)
					{
						return this.link2.MaxMessageSize;
					}
					return this.link1.MaxMessageSize;
				}
			}

			public ulong AbsoluteMaxMessageSize
			{
				get
				{
					return Math.Min(this.link1.AbsoluteMaxMessageSize, this.link2.AbsoluteMaxMessageSize);
				}
			}

			public ReadOnlyCollection<ITopologySite> TopologySites
			{
				get
				{
					return null;
				}
			}

			private ITopologySiteLink link1;

			private ITopologySiteLink link2;
		}

		private class SiteNextHop : DeliveryGroup
		{
			public SiteNextHop(ADSiteRelayMap.ADTopologyPath targetPath)
			{
				RoutingUtils.ThrowIfNull(targetPath, "targetPath");
				this.targetPath = targetPath;
			}

			public ADSiteRelayMap.ADTopologyPath TargetPath
			{
				get
				{
					return this.targetPath;
				}
			}

			public override IEnumerable<RoutingServerInfo> AllServersNoFallback
			{
				get
				{
					return this.targetPath.TargetSite.HubServers;
				}
			}

			public override string Name
			{
				get
				{
					return this.targetPath.TargetSite.Name;
				}
			}

			public override bool IsActive
			{
				get
				{
					return this.targetPath.TargetSite.IsActive;
				}
			}

			public override DeliveryType DeliveryType
			{
				get
				{
					return DeliveryType.SmtpRelayToRemoteAdSite;
				}
			}

			public override Guid NextHopGuid
			{
				get
				{
					return this.targetPath.TargetSite.Guid;
				}
			}

			public override bool IsMandatoryTopologyHop
			{
				get
				{
					return this.targetPath.TargetSite.Site.HubSiteEnabled;
				}
			}

			public override RouteInfo PrimaryRoute
			{
				get
				{
					return this.targetPath.NextHopPath.RouteInfo;
				}
			}

			public override IEnumerable<INextHopServer> GetLoadBalancedNextHopServers(string nextHopDomain)
			{
				return this.targetPath.GetLoadBalancedHubServers();
			}

			public override IEnumerable<RoutingServerInfo> GetServersForProxyTarget(ProxyRoutingEnumeratorContext context)
			{
				return context.PostLoadbalanceFilter(from serverInfo in this.targetPath.TargetSite.LoadBalancedHubServers
				where context.PreLoadbalanceFilter(serverInfo)
				select serverInfo, new bool?(true));
			}

			public override bool Match(RoutingNextHop other)
			{
				ADSiteRelayMap.SiteNextHop siteNextHop = other as ADSiteRelayMap.SiteNextHop;
				return siteNextHop != null && this.targetPath.Match(siteNextHop.targetPath);
			}

			private ADSiteRelayMap.ADTopologyPath targetPath;
		}
	}
}
