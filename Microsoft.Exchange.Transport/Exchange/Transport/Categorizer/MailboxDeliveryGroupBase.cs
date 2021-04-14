using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal abstract class MailboxDeliveryGroupBase : ServerCollectionDeliveryGroup
	{
		protected MailboxDeliveryGroupBase(RoutedServerCollection routedServerCollection, DeliveryType deliveryType, string nextHopDomain, Guid nextHopGuid, int version, bool isLocalDeliveryGroup) : base(routedServerCollection)
		{
			this.key = new NextHopSolutionKey(deliveryType, nextHopDomain, nextHopGuid, isLocalDeliveryGroup);
			this.version = version;
		}

		public override string Name
		{
			get
			{
				return this.key.NextHopDomain;
			}
		}

		public override bool IsActive
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

		public override DeliveryType DeliveryType
		{
			get
			{
				return this.key.NextHopType.DeliveryType;
			}
		}

		public override Guid NextHopGuid
		{
			get
			{
				return this.key.NextHopConnector;
			}
		}

		public bool IsLocalDeliveryGroup
		{
			get
			{
				return this.key.IsLocalDeliveryGroupRelay;
			}
		}

		public virtual bool TryGetDatabaseRouteInfo(MiniDatabase database, RoutingServerInfo owningServerInfo, RouteCalculatorContext context, out RouteInfo databaseRouteInfo)
		{
			databaseRouteInfo = null;
			if (this.databaseRouteInfo == null)
			{
				RouteInfo primaryRoute = base.RoutedServerCollection.PrimaryRoute;
				if (primaryRoute.DestinationProximity == Proximity.RemoteADSite && !context.Core.Settings.DestinationRoutingToRemoteSitesEnabled)
				{
					this.databaseRouteInfo = primaryRoute;
				}
				else
				{
					this.databaseRouteInfo = primaryRoute.ReplaceNextHop(this, this.key.NextHopDomain);
				}
			}
			databaseRouteInfo = this.databaseRouteInfo;
			return true;
		}

		public override bool MayContainServersOfVersions(IList<int> majorVersions)
		{
			return majorVersions.Contains(this.version);
		}

		public override bool MayContainServersOfVersion(int majorVersion)
		{
			return this.version == majorVersion;
		}

		public void UpdateIfGroupIsActiveAndRemoveInactiveServers(RoutingContextCore context)
		{
			this.isActive = new bool?(base.RoutedServerCollection.GetStateAndRemoveInactiveServersIfStateIsActive(context));
		}

		protected override NextHopSolutionKey GetNextHopSolutionKey(RoutingContext context)
		{
			return this.key;
		}

		protected void AddServerInternal(RouteInfo routeInfo, RoutingServerInfo serverInfo, RoutingContextCore contextCore)
		{
			if (this.databaseRouteInfo != null)
			{
				throw new InvalidOperationException("Cannot add servers after database route is calculated");
			}
			base.RoutedServerCollection.AddServerForRoute(routeInfo, serverInfo, contextCore);
		}

		private readonly int version;

		private NextHopSolutionKey key;

		private RouteInfo databaseRouteInfo;

		private bool? isActive;
	}
}
