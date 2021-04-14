using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class DagDeliveryGroup : MailboxDeliveryGroupBase
	{
		public DagDeliveryGroup(ADObjectId dagId, RouteInfo routeInfo, RoutingServerInfo serverInfo, bool isLocalDeliveryGroup, RoutingContextCore contextCore) : base(new RoutedServerCollection(routeInfo, serverInfo, contextCore), DeliveryType.SmtpRelayToDag, dagId.Name, dagId.ObjectGuid, serverInfo.MajorVersion, isLocalDeliveryGroup)
		{
			RoutingUtils.ThrowIfNullOrEmpty(dagId, "dagId");
		}

		public void AddServer(RouteInfo routeInfo, RoutingServerInfo serverInfo, RoutingContextCore contextCore)
		{
			base.AddServerInternal(routeInfo, serverInfo, contextCore);
		}

		public override bool TryGetDatabaseRouteInfo(MiniDatabase database, RoutingServerInfo owningServerInfo, RouteCalculatorContext context, out RouteInfo databaseRouteInfo)
		{
			databaseRouteInfo = null;
			RouteInfo primaryRoute = base.RoutedServerCollection.PrimaryRoute;
			if (primaryRoute.DestinationProximity != Proximity.RemoteADSite || base.RoutedServerCollection.ServerGroupCount <= 1 || (!primaryRoute.HasMandatoryTopologyHop && context.Core.Settings.DestinationRoutingToRemoteSitesEnabled))
			{
				return base.TryGetDatabaseRouteInfo(database, owningServerInfo, context, out databaseRouteInfo);
			}
			if (context.ServerMap.TryGetServerRoute(owningServerInfo, out databaseRouteInfo))
			{
				RoutingDiag.Tracer.TraceDebug<DateTime, string, string>((long)this.GetHashCode(), "[{0}] Broke up the DAG for Database '{1}'; the route destination name is '{2}'", context.Timestamp, database.DistinguishedName, databaseRouteInfo.DestinationName);
				return true;
			}
			RoutingDiag.Tracer.TraceError<DateTime, string, string>((long)this.GetHashCode(), "[{0}] Owning server '{1}' for Database '{2}' does not have a route", context.Timestamp, owningServerInfo.Id.DistinguishedName, database.DistinguishedName);
			RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingNoRouteToOwningServer, owningServerInfo.Id.DistinguishedName, new object[]
			{
				owningServerInfo.Id.DistinguishedName,
				database.DistinguishedName,
				context.Timestamp
			});
			databaseRouteInfo = null;
			return false;
		}
	}
}
