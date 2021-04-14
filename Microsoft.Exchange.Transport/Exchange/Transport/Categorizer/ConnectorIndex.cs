using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal abstract class ConnectorIndex
	{
		public ConnectorIndex(DateTime timestamp)
		{
			this.Timestamp = timestamp;
		}

		public abstract ConnectorMatchResult TryFindBestConnector(string address, long messageSize, out ConnectorRoutingDestination connectorDestination);

		public abstract void AddConnector(AddressSpace addressSpace, ConnectorRoutingDestination connectorDestination);

		public abstract bool Match(ConnectorIndex other);

		protected readonly DateTime Timestamp;

		protected sealed class ConnectorWithCost
		{
			public ConnectorWithCost(ConnectorRoutingDestination connectorDestination, AddressSpace addressSpace)
			{
				RoutingUtils.ThrowIfNull(connectorDestination, "connectorDestination");
				RoutingUtils.ThrowIfNull(addressSpace, "addressSpace");
				this.ConnectorDestination = connectorDestination;
				this.AddressSpace = addressSpace;
			}

			public int Cost
			{
				get
				{
					return this.AddressSpace.Cost;
				}
			}

			public static void InsertConnector(ConnectorRoutingDestination connectorDestination, AddressSpace addressSpace, IList<ConnectorIndex.ConnectorWithCost> connectorList)
			{
				ConnectorIndex.ConnectorWithCost connectorWithCost = new ConnectorIndex.ConnectorWithCost(connectorDestination, addressSpace);
				int num = 0;
				while (num < connectorList.Count && connectorWithCost.CompareTo(connectorList[num]) >= 0)
				{
					num++;
				}
				connectorList.Insert(num, connectorWithCost);
			}

			public static int GetConnectorForMessageSize(IList<ConnectorIndex.ConnectorWithCost> connectorList, long messageSize, DateTime timestamp, string addressType, string address)
			{
				for (int i = 0; i < connectorList.Count; i++)
				{
					long maxMessageSize = connectorList[i].ConnectorDestination.MaxMessageSize;
					if (messageSize <= maxMessageSize)
					{
						return i;
					}
					RoutingDiag.Tracer.TraceDebug(0L, "[{0}] Skipped connector {1} for address '{2}:{3}' because the message size {4} is over the limit of {5}", new object[]
					{
						timestamp,
						connectorList[i].ConnectorDestination.StringIdentity,
						addressType,
						address,
						messageSize,
						maxMessageSize
					});
				}
				return -1;
			}

			public static ConnectorMatchResult TryGetConnectorForMessageSize(IList<ConnectorIndex.ConnectorWithCost> connectorList, long messageSize, DateTime timestamp, string addressType, string address, out ConnectorRoutingDestination matchingConnector)
			{
				matchingConnector = null;
				if (connectorList == null)
				{
					return ConnectorMatchResult.NoAddressMatch;
				}
				int connectorForMessageSize = ConnectorIndex.ConnectorWithCost.GetConnectorForMessageSize(connectorList, messageSize, timestamp, addressType, address);
				if (connectorForMessageSize < 0)
				{
					return ConnectorMatchResult.MaxMessageSizeExceeded;
				}
				matchingConnector = connectorList[connectorForMessageSize].ConnectorDestination;
				return ConnectorMatchResult.Success;
			}

			public static bool MatchLists(List<ConnectorIndex.ConnectorWithCost> l1, List<ConnectorIndex.ConnectorWithCost> l2)
			{
				return RoutingUtils.MatchOrderedLists<ConnectorIndex.ConnectorWithCost>(l1, l2, (ConnectorIndex.ConnectorWithCost c1, ConnectorIndex.ConnectorWithCost c2) => c1.Match(c2));
			}

			public int CompareTo(ConnectorIndex.ConnectorWithCost other)
			{
				RouteInfo routeInfo = this.ConnectorDestination.RouteInfo;
				RouteInfo routeInfo2 = other.ConnectorDestination.RouteInfo;
				Proximity destinationProximity = routeInfo.DestinationProximity;
				Proximity destinationProximity2 = routeInfo2.DestinationProximity;
				int num = this.Cost;
				int num2 = other.Cost;
				if (destinationProximity != destinationProximity2)
				{
					if (destinationProximity == Proximity.RemoteRoutingGroup)
					{
						return 1;
					}
					if (destinationProximity2 == Proximity.RemoteRoutingGroup)
					{
						return -1;
					}
				}
				else if (destinationProximity == Proximity.RemoteRoutingGroup)
				{
					num += routeInfo.RGRelayCost;
					num2 += routeInfo2.RGRelayCost;
					if (num != num2)
					{
						return num - num2;
					}
					int num3 = routeInfo.CompareTo(routeInfo2, RouteComparison.IgnoreRGCosts);
					if (num3 != 0)
					{
						return num3;
					}
					return RoutingUtils.CompareNames(this.ConnectorDestination.ConnectorName, other.ConnectorDestination.ConnectorName);
				}
				if (destinationProximity == Proximity.RemoteADSite)
				{
					num += routeInfo.SiteRelayCost;
				}
				if (destinationProximity2 == Proximity.RemoteADSite)
				{
					num2 += routeInfo2.SiteRelayCost;
				}
				if (num != num2)
				{
					return num - num2;
				}
				if (destinationProximity == destinationProximity2)
				{
					return RoutingUtils.CompareNames(this.ConnectorDestination.ConnectorName, other.ConnectorDestination.ConnectorName);
				}
				if (destinationProximity >= destinationProximity2)
				{
					return 1;
				}
				return -1;
			}

			private bool Match(ConnectorIndex.ConnectorWithCost other)
			{
				return this.Cost == other.Cost && this.ConnectorDestination.Match(other.ConnectorDestination);
			}

			public readonly ConnectorRoutingDestination ConnectorDestination;

			public readonly AddressSpace AddressSpace;
		}
	}
}
