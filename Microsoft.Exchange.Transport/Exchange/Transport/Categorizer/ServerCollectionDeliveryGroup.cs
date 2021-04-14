using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal abstract class ServerCollectionDeliveryGroup : DeliveryGroup
	{
		protected ServerCollectionDeliveryGroup(RoutedServerCollection routedServerCollection)
		{
			RoutingUtils.ThrowIfNull(routedServerCollection, "routedServerCollection");
			this.routedServerCollection = routedServerCollection;
		}

		public override IEnumerable<RoutingServerInfo> AllServersNoFallback
		{
			get
			{
				return this.routedServerCollection.AllServers;
			}
		}

		public override RouteInfo PrimaryRoute
		{
			get
			{
				return this.routedServerCollection.PrimaryRoute;
			}
		}

		protected RoutedServerCollection RoutedServerCollection
		{
			get
			{
				return this.routedServerCollection;
			}
		}

		public override IEnumerable<INextHopServer> GetLoadBalancedNextHopServers(string nextHopDomain)
		{
			return this.routedServerCollection.AllServersLoadBalanced;
		}

		public override IEnumerable<RoutingServerInfo> GetServersForProxyTarget(ProxyRoutingEnumeratorContext context)
		{
			return this.routedServerCollection.GetServersForProxyTarget(context);
		}

		public override IEnumerable<RoutingServerInfo> GetServersForShadowTarget(ProxyRoutingEnumeratorContext context, ShadowRoutingConfiguration shadowRoutingConfig)
		{
			return this.routedServerCollection.GetServersForShadowTarget(context, shadowRoutingConfig);
		}

		public override bool Match(RoutingNextHop other)
		{
			return !(base.GetType() != other.GetType()) && this.MatchServers((ServerCollectionDeliveryGroup)other);
		}

		protected bool MatchServers(ServerCollectionDeliveryGroup other)
		{
			return this.routedServerCollection.MatchServers(other.routedServerCollection);
		}

		private RoutedServerCollection routedServerCollection;
	}
}
