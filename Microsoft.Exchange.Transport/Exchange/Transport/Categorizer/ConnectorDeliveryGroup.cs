using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class ConnectorDeliveryGroup : ServerCollectionDeliveryGroup
	{
		public ConnectorDeliveryGroup(ADObjectId connectorId, RoutedServerCollection routedSourceServers, bool isEdgeConnector) : base(routedSourceServers)
		{
			RoutingUtils.ThrowIfNullOrEmpty(connectorId, "connectorId");
			if (routedSourceServers.ClosestProximity != Proximity.LocalADSite && routedSourceServers.ClosestProximity != Proximity.RemoteADSite)
			{
				throw new ArgumentOutOfRangeException("routedSourceServers.ClosestProximity", routedSourceServers.ClosestProximity, "routedSourceServers.ClosestProximity must be Local or Remote AD site");
			}
			if (isEdgeConnector && routedSourceServers.ClosestProximity != Proximity.LocalADSite)
			{
				throw new ArgumentException("This delivery group cannot be used for Edge connectors in remote AD sites", "isEdgeConnector");
			}
			this.connectorId = connectorId;
			this.deliveryType = (isEdgeConnector ? DeliveryType.SmtpRelayWithinAdSiteToEdge : DeliveryType.SmtpRelayToConnectorSourceServers);
		}

		public override string Name
		{
			get
			{
				return this.connectorId.Name;
			}
		}

		public override DeliveryType DeliveryType
		{
			get
			{
				return this.deliveryType;
			}
		}

		public override Guid NextHopGuid
		{
			get
			{
				return this.connectorId.ObjectGuid;
			}
		}

		public override bool Match(RoutingNextHop other)
		{
			ConnectorDeliveryGroup connectorDeliveryGroup = other as ConnectorDeliveryGroup;
			return connectorDeliveryGroup != null && !(this.connectorId.ObjectGuid != connectorDeliveryGroup.connectorId.ObjectGuid) && this.deliveryType == connectorDeliveryGroup.deliveryType && base.MatchServers(connectorDeliveryGroup);
		}

		private ADObjectId connectorId;

		private DeliveryType deliveryType;
	}
}
