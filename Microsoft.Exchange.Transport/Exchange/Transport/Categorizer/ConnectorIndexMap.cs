using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Transport.Common;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class ConnectorIndexMap
	{
		public ConnectorIndexMap(DateTime timestamp)
		{
			this.timestamp = timestamp;
			this.typeToIndexMap = new Dictionary<string, ConnectorIndex>(StringComparer.OrdinalIgnoreCase);
			this.connectorRouteMap = new Dictionary<Guid, RouteInfo>();
		}

		public ConnectorMatchResult TryFindBestConnector(string addressType, string address, long messageSize, out ConnectorRoutingDestination connectorDestination)
		{
			connectorDestination = null;
			RoutingUtils.ThrowIfNullOrEmpty(addressType, "addressType");
			ConnectorIndex connectorIndex;
			if (!this.typeToIndexMap.TryGetValue(addressType, out connectorIndex))
			{
				RoutingDiag.Tracer.TraceDebug<DateTime, string, string>((long)this.GetHashCode(), "[{0}] Connector index not found for address type '{1}'. (Address is {2})", this.timestamp, addressType, address);
				return ConnectorMatchResult.NoAddressMatch;
			}
			return connectorIndex.TryFindBestConnector(address, messageSize, out connectorDestination);
		}

		public bool TryGetConnectorNextHop(Guid connectorGuid, out RoutingNextHop nextHop)
		{
			nextHop = null;
			RouteInfo routeInfo;
			if (this.connectorRouteMap.TryGetValue(connectorGuid, out routeInfo))
			{
				nextHop = routeInfo.NextHop;
				return true;
			}
			return false;
		}

		public bool TryGetLocalSendConnector<ConnectorType>(Guid connectorGuid, out ConnectorType connector) where ConnectorType : MailGateway
		{
			connector = default(ConnectorType);
			RouteInfo routeInfo;
			return this.connectorRouteMap.TryGetValue(connectorGuid, out routeInfo) && ConnectorIndexMap.TryGetLocalSendConnector<ConnectorType>(routeInfo, out connector);
		}

		public IList<ConnectorType> GetLocalSendConnectors<ConnectorType>() where ConnectorType : MailGateway
		{
			IList<ConnectorType> list = new List<ConnectorType>();
			foreach (RouteInfo routeInfo in this.connectorRouteMap.Values)
			{
				ConnectorType item;
				if (ConnectorIndexMap.TryGetLocalSendConnector<ConnectorType>(routeInfo, out item))
				{
					list.Add(item);
				}
			}
			return list;
		}

		public void AddConnector(ConnectorRoutingDestination connectorDestination)
		{
			foreach (AddressSpace addressSpace in connectorDestination.AddressSpaces)
			{
				this.AddConnector(addressSpace, connectorDestination);
			}
			this.AddNonAddressSpaceConnector(connectorDestination.ConnectorGuid, connectorDestination.RouteInfo);
		}

		public void AddNonAddressSpaceConnector(Guid connectorGuid, RouteInfo routeInfo)
		{
			TransportHelpers.AttemptAddToDictionary<Guid, RouteInfo>(this.connectorRouteMap, connectorGuid, routeInfo, new TransportHelpers.DiagnosticsHandler<Guid, RouteInfo>(RoutingUtils.LogErrorWhenAddToDictionaryFails<Guid, RouteInfo>));
		}

		public bool QuickMatch(ConnectorIndexMap other)
		{
			return this.connectorRouteMap.Count == other.connectorRouteMap.Count && this.typeToIndexMap.Count == other.typeToIndexMap.Count;
		}

		public bool FullMatch(ConnectorIndexMap other)
		{
			return RoutingUtils.MatchDictionaries<string, ConnectorIndex>(this.typeToIndexMap, other.typeToIndexMap, (ConnectorIndex index1, ConnectorIndex index2) => index1.Match(index2));
		}

		private static bool TryGetLocalSendConnector<ConnectorType>(RouteInfo routeInfo, out ConnectorType connector) where ConnectorType : MailGateway
		{
			connector = default(ConnectorType);
			if (routeInfo.DestinationProximity == Proximity.LocalServer)
			{
				ConnectorDeliveryHop connectorDeliveryHop = (ConnectorDeliveryHop)routeInfo.NextHop;
				connector = (connectorDeliveryHop.Connector as ConnectorType);
			}
			return connector != null;
		}

		private void AddConnector(AddressSpace addressSpace, ConnectorRoutingDestination connectorDestination)
		{
			RoutingDiag.Tracer.TraceDebug<DateTime, string, AddressSpace>((long)this.GetHashCode(), "[{0}] Indexing connector {1} for address space '{2}'", this.timestamp, connectorDestination.StringIdentity, addressSpace);
			ConnectorIndex connectorIndex;
			if (!this.typeToIndexMap.TryGetValue(addressSpace.Type, out connectorIndex))
			{
				if (addressSpace.IsSmtpType)
				{
					connectorIndex = new SmtpConnectorIndex(this.timestamp);
				}
				else if (addressSpace.IsX400Type)
				{
					connectorIndex = new X400ConnectorIndex(this.timestamp);
				}
				else
				{
					connectorIndex = new GenericConnectorIndex(addressSpace.Type, this.timestamp);
				}
				this.typeToIndexMap.Add(addressSpace.Type, connectorIndex);
				RoutingDiag.Tracer.TraceDebug<DateTime, string>((long)this.GetHashCode(), "[{0}] Created connector index for address type {1}", this.timestamp, addressSpace.Type);
			}
			connectorIndex.AddConnector(addressSpace, connectorDestination);
			RoutingDiag.Tracer.TraceDebug<DateTime, string, AddressSpace>((long)this.GetHashCode(), "[{0}] Indexed connector {1} for address space '{2}'", this.timestamp, connectorDestination.StringIdentity, addressSpace);
		}

		private readonly DateTime timestamp;

		private Dictionary<string, ConnectorIndex> typeToIndexMap;

		private Dictionary<Guid, RouteInfo> connectorRouteMap;
	}
}
