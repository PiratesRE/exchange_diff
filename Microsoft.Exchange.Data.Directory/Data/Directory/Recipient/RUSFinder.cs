using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class RUSFinder
	{
		private RUSFinder()
		{
		}

		public RUSFinder(ExchangeTopology topology)
		{
			this.topo = topology;
			this.siteDictionary = new Dictionary<TopologySite, List<TopologyServer>>();
			this.siteList = new List<TopologySite>();
			ReadOnlyCollection<TopologyServer> allTopologyServers = this.topo.AllTopologyServers;
			int num = 0;
			int num2 = 0;
			foreach (TopologyServer topologyServer in allTopologyServers)
			{
				if (topologyServer.TopologySite != null && topologyServer.IsExchange2007OrLater && topologyServer.IsMailboxServer)
				{
					List<TopologyServer> list;
					if (!this.siteDictionary.TryGetValue(topologyServer.TopologySite, out list))
					{
						this.siteList.Add(topologyServer.TopologySite);
						list = new List<TopologyServer>();
						this.siteDictionary.Add(topologyServer.TopologySite, list);
						num++;
					}
					list.Add(topologyServer);
					num2++;
					if (ExTraceGlobals.RecipientUpdateServiceTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.RecipientUpdateServiceTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "Found RUS Server: {0} ({1}) in site {2}", topologyServer.Name, topologyServer.Fqdn, (topologyServer.TopologySite == null) ? "no site" : topologyServer.TopologySite.Name);
					}
				}
			}
			ExTraceGlobals.RecipientUpdateServiceTracer.TraceDebug<int, int>((long)this.GetHashCode(), "Found {0} potential RUS Servers in {1} sites.", num2, num);
		}

		public TopologySite ClosestSite(TopologySite sourceSite)
		{
			TopologySite topologySite = this.topo.FindClosestDestinationSite(sourceSite, this.siteList);
			if (topologySite != null)
			{
				this.siteList.Remove(topologySite);
			}
			ExTraceGlobals.RecipientUpdateServiceTracer.TraceDebug<string>((long)this.GetHashCode(), "ClosestSite returned site {0}.", (topologySite == null) ? "no site" : topologySite.Name);
			return topologySite;
		}

		public List<TopologyServer> ServerList(TopologySite site)
		{
			List<TopologyServer> result;
			if (!this.siteDictionary.TryGetValue(site, out result))
			{
				result = new List<TopologyServer>();
			}
			return result;
		}

		private ExchangeTopology topo;

		private Dictionary<TopologySite, List<TopologyServer>> siteDictionary;

		private List<TopologySite> siteList;
	}
}
