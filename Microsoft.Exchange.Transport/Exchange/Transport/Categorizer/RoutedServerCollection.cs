using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class RoutedServerCollection
	{
		public RoutedServerCollection(RouteInfo routeInfo, RoutingServerInfo serverInfo, RoutingContextCore contextCore)
		{
			RoutingUtils.ThrowIfNull(routeInfo, "routeInfo");
			RoutingUtils.ThrowIfNull(serverInfo, "serverInfo");
			RoutingUtils.ThrowIfNull(contextCore, "contextCore");
			this.serversByRoutes = new SortedList<RouteInfo, ListLoadBalancer<RoutingServerInfo>>(RoutedServerCollection.routeComparer);
			this.AddServerForRoute(routeInfo, serverInfo, contextCore);
			this.ServerSelectStrategyForProxyTarget = contextCore.Settings.ProxyRoutingServerSelectStrategy;
		}

		public Proximity ClosestProximity
		{
			get
			{
				return this.PrimaryRoute.DestinationProximity;
			}
		}

		public RoutedServerSelectStrategy ServerSelectStrategyForProxyTarget
		{
			get
			{
				return this.serverSelectStrategyForProxyTarget;
			}
			private set
			{
				this.serverSelectStrategyForProxyTarget = value;
			}
		}

		public RouteInfo PrimaryRoute
		{
			get
			{
				return this.serversByRoutes.Keys[0];
			}
		}

		public int ServerGroupCount
		{
			get
			{
				return this.serversByRoutes.Count;
			}
		}

		public IList<RoutingServerInfo> PrimaryRouteServers
		{
			get
			{
				return this.serversByRoutes.Values[0].NonLoadBalancedList;
			}
		}

		public IEnumerable<RoutingServerInfo> AllServers
		{
			get
			{
				foreach (ListLoadBalancer<RoutingServerInfo> serverList in this.AllServerLists)
				{
					foreach (RoutingServerInfo serverInfo in serverList.NonLoadBalancedList)
					{
						yield return serverInfo;
					}
				}
				yield break;
			}
		}

		public IEnumerable<RoutingServerInfo> AllServersLoadBalanced
		{
			get
			{
				foreach (ListLoadBalancer<RoutingServerInfo> serverList in this.AllServerLists)
				{
					foreach (RoutingServerInfo serverInfo in serverList.LoadBalancedCollection)
					{
						yield return serverInfo;
					}
				}
				yield break;
			}
		}

		private IEnumerable<ListLoadBalancer<RoutingServerInfo>> AllServerLists
		{
			get
			{
				return this.serversByRoutes.Values;
			}
		}

		private RoutingServerInfo LocalServer
		{
			get
			{
				if (this.PrimaryRoute.DestinationProximity != Proximity.LocalServer)
				{
					throw new InvalidOperationException("Local server is not present in this RoutedServerCollection");
				}
				return this.PrimaryRouteServers[0];
			}
		}

		private int LocalSiteRouteCount
		{
			get
			{
				int num = 0;
				while (num < this.serversByRoutes.Count && this.serversByRoutes.Keys[num].InLocalADSite)
				{
					num++;
				}
				if (num > 2)
				{
					throw new InvalidOperationException("LocalSiteRouteCount must be between 0 and 2; actual value: " + num);
				}
				return num;
			}
		}

		public IEnumerable<RoutingServerInfo> GetServersForProxyTarget(ProxyRoutingEnumeratorContext context)
		{
			switch (this.ServerSelectStrategyForProxyTarget)
			{
			case RoutedServerSelectStrategy.FavorCloserProximity:
				return this.GetServersByProximity(context);
			case RoutedServerSelectStrategy.FavorLoadBalance:
				return this.GetAllServersLoadBalanced(context);
			default:
				throw new NotImplementedException("Only FavorCloserProximity and FavorLoadBalance are supported.");
			}
		}

		public IEnumerable<RoutingServerInfo> GetServersForShadowTarget(ProxyRoutingEnumeratorContext context, ShadowRoutingConfiguration shadowRoutingConfig)
		{
			if (context.RemainingServerCount != 0)
			{
				int localSiteRouteCount = this.LocalSiteRouteCount;
				int remoteSiteRouteCount = this.serversByRoutes.Count - localSiteRouteCount;
				if (shadowRoutingConfig.ShadowMessagePreference != ShadowMessagePreference.LocalOnly && remoteSiteRouteCount > 0)
				{
					foreach (RoutingServerInfo serverInfo in context.PostLoadbalanceFilter(this.GetRemoteSiteServersForProxyTarget(localSiteRouteCount, context), new bool?(true)))
					{
						yield return serverInfo;
					}
				}
				if (shadowRoutingConfig.ShadowMessagePreference != ShadowMessagePreference.RemoteOnly)
				{
					foreach (RoutingServerInfo serverInfo2 in context.PostLoadbalanceFilter(this.GetLocalSiteServersForProxyTarget(localSiteRouteCount, context), new bool?(false)))
					{
						yield return serverInfo2;
					}
				}
			}
			yield break;
		}

		public void AddServerForRoute(RouteInfo routeInfo, RoutingServerInfo serverInfo, RoutingContextCore contextCore)
		{
			this.AddServerForRoute(routeInfo, serverInfo, false, contextCore);
		}

		public void AddServerForRoute(RouteInfo routeInfo, RoutingServerInfo serverInfo, bool replaceExistingData, RoutingContextCore contextCore)
		{
			RoutingUtils.ThrowIfNull(routeInfo, "routeInfo");
			RoutingUtils.ThrowIfNull(serverInfo, "serverInfo");
			RoutingUtils.ThrowIfNull(contextCore, "contextCore");
			if (replaceExistingData)
			{
				this.serversByRoutes.Clear();
			}
			int num = this.serversByRoutes.IndexOfKey(routeInfo);
			if (num == -1)
			{
				ListLoadBalancer<RoutingServerInfo> listLoadBalancer = new ListLoadBalancer<RoutingServerInfo>(contextCore.Settings.RandomLoadBalancingOffsetEnabled);
				listLoadBalancer.AddItem(serverInfo);
				this.serversByRoutes.Add(routeInfo, listLoadBalancer);
				return;
			}
			this.serversByRoutes.Values[num].AddItem(serverInfo);
		}

		public int TrimRoutes(Proximity maxProximity, int maxCost, long minMaxMessageSize)
		{
			int num = this.serversByRoutes.Count;
			int num2 = 0;
			while (--num > 0)
			{
				RouteInfo routeInfo = this.serversByRoutes.Keys[num];
				if (routeInfo.HasMandatoryTopologyHop || routeInfo.DestinationProximity > maxProximity || routeInfo.SiteRelayCost > maxCost || routeInfo.MaxMessageSize < minMaxMessageSize)
				{
					this.serversByRoutes.RemoveAt(num);
					num2++;
				}
			}
			return num2;
		}

		public bool MatchServers(RoutedServerCollection other)
		{
			if (this.serversByRoutes.Count != other.serversByRoutes.Count)
			{
				return false;
			}
			for (int i = 0; i < this.serversByRoutes.Count; i++)
			{
				if (!RoutingUtils.MatchLists<RoutingServerInfo>(this.serversByRoutes.Values[i], other.serversByRoutes.Values[i], (RoutingServerInfo server1, RoutingServerInfo server2) => server1.Id.ObjectGuid == server2.Id.ObjectGuid))
				{
					return false;
				}
			}
			return true;
		}

		public bool GetStateAndRemoveInactiveServersIfStateIsActive(RoutingContextCore context)
		{
			List<RouteInfo> list = null;
			List<KeyValuePair<RouteInfo, List<RoutingServerInfo>>> list2 = null;
			bool flag = false;
			foreach (KeyValuePair<RouteInfo, ListLoadBalancer<RoutingServerInfo>> keyValuePair in this.serversByRoutes)
			{
				bool flag2 = false;
				List<RoutingServerInfo> value = null;
				foreach (RoutingServerInfo routingServerInfo in keyValuePair.Value.NonLoadBalancedCollection)
				{
					if (context.VerifyHubComponentStateRestriction(routingServerInfo))
					{
						flag = true;
						flag2 = true;
					}
					else
					{
						RoutingUtils.AddItemToLazyList<RoutingServerInfo>(routingServerInfo, ref value);
					}
				}
				if (flag2)
				{
					RoutingUtils.AddItemToLazyList<KeyValuePair<RouteInfo, List<RoutingServerInfo>>>(new KeyValuePair<RouteInfo, List<RoutingServerInfo>>(keyValuePair.Key, value), ref list2);
				}
				else
				{
					RoutingUtils.AddItemToLazyList<RouteInfo>(keyValuePair.Key, ref list);
				}
			}
			if (flag)
			{
				if (list != null)
				{
					foreach (RouteInfo key in list)
					{
						this.serversByRoutes.Remove(key);
					}
				}
				if (list2 != null)
				{
					foreach (KeyValuePair<RouteInfo, List<RoutingServerInfo>> keyValuePair2 in list2)
					{
						if (keyValuePair2.Value != null)
						{
							foreach (RoutingServerInfo item in keyValuePair2.Value)
							{
								this.serversByRoutes[keyValuePair2.Key].RemoveItem(item);
							}
						}
					}
				}
			}
			return flag;
		}

		private static bool TryGetNextServerIndex(LoadBalancedCollection<RoutingServerInfo> serverList, int offset, ProxyRoutingEnumeratorContext context, out int index)
		{
			index = -1;
			for (int i = offset; i < serverList.Count; i++)
			{
				if (context.PreLoadbalanceFilter(serverList[i]))
				{
					index = i;
					return true;
				}
			}
			return false;
		}

		private IEnumerable<RoutingServerInfo> GetAllServersLoadBalanced(ProxyRoutingEnumeratorContext context)
		{
			return context.PostLoadbalanceFilter(this.GetAllSitesServersForProxyTarget(context), null);
		}

		private IEnumerable<RoutingServerInfo> GetServersByProximity(ProxyRoutingEnumeratorContext context)
		{
			if (context.RemainingServerCount != 0)
			{
				int localSiteRouteCount = this.LocalSiteRouteCount;
				foreach (RoutingServerInfo serverInfo in context.PostLoadbalanceFilter(this.GetLocalSiteServersForProxyTarget(localSiteRouteCount, context), new bool?(false)))
				{
					yield return serverInfo;
				}
				if (localSiteRouteCount != this.serversByRoutes.Count && context.RemoteSiteRemainingServerCount != 0)
				{
					foreach (RoutingServerInfo serverInfo2 in context.PostLoadbalanceFilter(this.GetRemoteSiteServersForProxyTarget(localSiteRouteCount, context), new bool?(true)))
					{
						yield return serverInfo2;
					}
				}
			}
			yield break;
		}

		private IEnumerable<RoutingServerInfo> GetAllSitesServersForProxyTarget(ProxyRoutingEnumeratorContext context)
		{
			if (context.RemainingServerCount != 0)
			{
				List<RoutingServerInfo> allServerList = new List<RoutingServerInfo>(this.AllServers);
				foreach (RoutingServerInfo serverInfo in RoutingUtils.RandomShuffleEnumerate<RoutingServerInfo>(allServerList))
				{
					if (context.PreLoadbalanceFilter(serverInfo))
					{
						yield return serverInfo;
					}
				}
			}
			yield break;
		}

		private IEnumerable<RoutingServerInfo> GetLocalSiteServersForProxyTarget(int localSiteRouteCount, ProxyRoutingEnumeratorContext context)
		{
			if (localSiteRouteCount != 0 && context.RemainingServerCount != 0)
			{
				LoadBalancedCollection<RoutingServerInfo> localSiteServerList = this.serversByRoutes.Values[localSiteRouteCount - 1].LoadBalancedCollection;
				int indexForLocalServer = -1;
				if (localSiteRouteCount == 2)
				{
					indexForLocalServer = RoutingUtils.GetRandomNumber(localSiteServerList.Count + 1);
				}
				for (int i = 0; i < localSiteServerList.Count; i++)
				{
					if (i == indexForLocalServer && context.PreLoadbalanceFilter(this.LocalServer))
					{
						yield return this.LocalServer;
					}
					if (context.PreLoadbalanceFilter(localSiteServerList[i]))
					{
						yield return localSiteServerList[i];
					}
				}
				if (indexForLocalServer == localSiteServerList.Count && context.PreLoadbalanceFilter(this.LocalServer))
				{
					yield return this.LocalServer;
				}
			}
			yield break;
		}

		private IEnumerable<RoutingServerInfo> GetRemoteSiteServersForProxyTarget(int localSiteRouteCount, ProxyRoutingEnumeratorContext context)
		{
			int remoteSiteCount = this.serversByRoutes.Count - localSiteRouteCount;
			LoadBalancedCollection<RoutingServerInfo>[] loadBalancedServerLists = new LoadBalancedCollection<RoutingServerInfo>[remoteSiteCount];
			int[] serverListIndices = new int[remoteSiteCount];
			int i = 0;
			foreach (ListLoadBalancer<RoutingServerInfo> listLoadBalancer in RoutingUtils.RandomShiftEnumerate<ListLoadBalancer<RoutingServerInfo>>(this.serversByRoutes.Values, localSiteRouteCount))
			{
				LoadBalancedCollection<RoutingServerInfo>[] array = loadBalancedServerLists;
				int num;
				i = (num = i) + 1;
				array[num] = listLoadBalancer.LoadBalancedCollection;
			}
			bool haveMoreServers;
			do
			{
				haveMoreServers = false;
				for (i = 0; i < remoteSiteCount; i++)
				{
					if (loadBalancedServerLists[i] != null)
					{
						int serverIndex = -1;
						if (RoutedServerCollection.TryGetNextServerIndex(loadBalancedServerLists[i], serverListIndices[i], context, out serverIndex))
						{
							yield return loadBalancedServerLists[i][serverIndex];
						}
						if (serverIndex != -1 && serverIndex < loadBalancedServerLists[i].Count - 1)
						{
							serverListIndices[i] = serverIndex + 1;
							haveMoreServers = true;
						}
						else
						{
							loadBalancedServerLists[i] = null;
						}
					}
				}
			}
			while (haveMoreServers);
			yield break;
		}

		private static readonly RouteInfo.Comparer routeComparer = new RouteInfo.Comparer(RouteComparison.CompareNames | RouteComparison.CompareRestrictions);

		private SortedList<RouteInfo, ListLoadBalancer<RoutingServerInfo>> serversByRoutes;

		private RoutedServerSelectStrategy serverSelectStrategyForProxyTarget;
	}
}
