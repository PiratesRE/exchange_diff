using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class RouteInfo
	{
		private RouteInfo(string destinationName, RoutingNextHop nextHop, long maxMessageSize, Proximity siteBasedProximity, int siteRelayCost, int routingGroupRelayCost)
		{
			RoutingUtils.ThrowIfNullOrEmpty(destinationName, "destinationName");
			if (maxMessageSize < 0L)
			{
				throw new ArgumentOutOfRangeException("maxMessageSize", maxMessageSize, "maxMessageSize must not be a negative value");
			}
			this.DestinationName = destinationName;
			this.NextHop = nextHop;
			this.MaxMessageSize = maxMessageSize;
			this.siteBasedProximity = siteBasedProximity;
			this.SiteRelayCost = siteRelayCost;
			this.RGRelayCost = routingGroupRelayCost;
		}

		public Proximity DestinationProximity
		{
			get
			{
				if (this.RGRelayCost != -1)
				{
					return Proximity.RemoteRoutingGroup;
				}
				return this.siteBasedProximity;
			}
		}

		public bool HasMandatoryTopologyHop
		{
			get
			{
				return this.NextHop != null && (this.RGRelayCost != -1 || this.NextHop.IsMandatoryTopologyHop);
			}
		}

		public bool InLocalADSite
		{
			get
			{
				Proximity destinationProximity = this.DestinationProximity;
				return destinationProximity == Proximity.LocalADSite || destinationProximity == Proximity.LocalServer;
			}
		}

		public static RouteInfo CreateForLocalServer(string destinationName, RoutingNextHop nextHop)
		{
			return new RouteInfo(destinationName, nextHop, long.MaxValue, Proximity.LocalServer, -1, -1);
		}

		public static RouteInfo CreateForLocalSite(string destinationName, RoutingNextHop nextHop)
		{
			return new RouteInfo(destinationName, nextHop, long.MaxValue, Proximity.LocalADSite, -1, -1);
		}

		public static RouteInfo CreateForRemoteSite(string destinationName, RoutingNextHop nextHop, long maxMessageSize, int cost)
		{
			RoutingUtils.ThrowIfNull(nextHop, "nextHop");
			if (cost == -1)
			{
				throw new ArgumentOutOfRangeException("cost", "Route cost must be provided");
			}
			return new RouteInfo(destinationName, nextHop, maxMessageSize, Proximity.RemoteADSite, cost, -1);
		}

		public static RouteInfo CreateForRemoteRG(string destinationName, long maxMessageSize, int routingGroupRelayCost, RouteInfo firstRGConnectorRouteInfo)
		{
			RoutingUtils.ThrowIfNull(firstRGConnectorRouteInfo, "firstRGConnectorRouteInfo");
			if (routingGroupRelayCost == -1)
			{
				throw new ArgumentOutOfRangeException("routingGroupRelayCost", "routingGroupRelayCost must be provided");
			}
			if (firstRGConnectorRouteInfo.DestinationProximity == Proximity.RemoteRoutingGroup)
			{
				throw new ArgumentOutOfRangeException("firstRGConnectorRouteInfo.DestinationProximity", "First RG connector for any route must be in the local RG");
			}
			if (firstRGConnectorRouteInfo.NextHop == null)
			{
				throw new ArgumentOutOfRangeException("firstRGConnectorRouteInfo.NextHop", "First RG connector next hop must not be null");
			}
			return new RouteInfo(destinationName, firstRGConnectorRouteInfo.NextHop, maxMessageSize, firstRGConnectorRouteInfo.DestinationProximity, firstRGConnectorRouteInfo.SiteRelayCost, routingGroupRelayCost);
		}

		public static RouteInfo CreateForUnroutableDestination(string destinationName, RoutingNextHop nextHop)
		{
			RoutingUtils.ThrowIfNull(nextHop, "nextHop");
			return new RouteInfo(destinationName, nextHop, long.MaxValue, Proximity.None, -1, -1);
		}

		public int CompareTo(RouteInfo other, RouteComparison options)
		{
			RoutingUtils.ThrowIfNull(other, "other");
			if (object.ReferenceEquals(this, other))
			{
				return 0;
			}
			if (this.DestinationProximity != other.DestinationProximity)
			{
				if (this.DestinationProximity >= other.DestinationProximity)
				{
					return 1;
				}
				return -1;
			}
			else
			{
				if ((options & RouteComparison.IgnoreRGCosts) == RouteComparison.None && this.RGRelayCost != other.RGRelayCost)
				{
					return this.RGRelayCost - other.RGRelayCost;
				}
				if (this.siteBasedProximity != other.siteBasedProximity)
				{
					if (this.siteBasedProximity >= other.siteBasedProximity)
					{
						return 1;
					}
					return -1;
				}
				else
				{
					if (this.SiteRelayCost != other.SiteRelayCost)
					{
						return this.SiteRelayCost - other.SiteRelayCost;
					}
					if ((options & RouteComparison.CompareRestrictions) != RouteComparison.None && this.MaxMessageSize != other.MaxMessageSize)
					{
						if (other.MaxMessageSize >= this.MaxMessageSize)
						{
							return 1;
						}
						return -1;
					}
					else
					{
						if ((options & RouteComparison.CompareNames) != RouteComparison.None)
						{
							return RoutingUtils.CompareNames(this.DestinationName, other.DestinationName);
						}
						return 0;
					}
				}
			}
		}

		public bool Match(RouteInfo other, NextHopMatch nextHopMatch)
		{
			if (this.siteBasedProximity != other.siteBasedProximity || this.SiteRelayCost != other.SiteRelayCost || this.RGRelayCost != other.RGRelayCost || this.MaxMessageSize != other.MaxMessageSize || !RoutingUtils.MatchStrings(this.DestinationName, other.DestinationName) || !RoutingUtils.NullMatch(this.NextHop, other.NextHop))
			{
				return false;
			}
			if (this.NextHop == null)
			{
				return true;
			}
			switch (nextHopMatch)
			{
			case NextHopMatch.Full:
				return this.NextHop.Match(other.NextHop);
			case NextHopMatch.GuidOnly:
				return this.NextHop.NextHopGuid == other.NextHop.NextHopGuid;
			default:
				throw new ArgumentOutOfRangeException("nextHopMatch", nextHopMatch, "Unexpected nextHopMatch: " + nextHopMatch);
			}
		}

		public RouteInfo ReplaceNextHop(RoutingNextHop newNextHop, string newName)
		{
			RoutingUtils.ThrowIfNull(newNextHop, "newNextHop");
			RoutingUtils.ThrowIfNullOrEmpty(newName, "newName");
			if (this.HasMandatoryTopologyHop)
			{
				return this;
			}
			return new RouteInfo(newName, newNextHop, this.MaxMessageSize, this.siteBasedProximity, this.SiteRelayCost, this.RGRelayCost);
		}

		public const int NoCost = -1;

		public static readonly RouteInfo LocalServerRoute = RouteInfo.CreateForLocalServer("<Local Server>", null);

		public static readonly RouteInfo LocalSiteRoute = RouteInfo.CreateForLocalSite("<Local AD Site>", null);

		public readonly string DestinationName;

		public readonly RoutingNextHop NextHop;

		public readonly long MaxMessageSize;

		public readonly int SiteRelayCost;

		public readonly int RGRelayCost;

		private readonly Proximity siteBasedProximity;

		public class Comparer : IComparer<RouteInfo>
		{
			public Comparer(RouteComparison options)
			{
				this.options = options;
			}

			public int Compare(RouteInfo x, RouteInfo y)
			{
				RoutingUtils.ThrowIfNull(x, "x");
				return x.CompareTo(y, this.options);
			}

			private RouteComparison options;
		}
	}
}
