using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.ExchangeTopology
{
	internal sealed class ExchangeTopology
	{
		internal ExchangeTopology(DateTime discoveryStarted, ExchangeTopologyScope topologyScope, ReadOnlyCollection<TopologyServer> allTopologyServers, ReadOnlyCollection<TopologySite> allTopologySites, ReadOnlyCollection<TopologySiteLink> allTopologySiteLinks, ReadOnlyCollection<MiniVirtualDirectory> allVirtualDirectories, ReadOnlyCollection<MiniEmailTransport> allEmailTransports, ReadOnlyCollection<MiniReceiveConnector> allSmtpReceiveConnectors, ReadOnlyCollection<ADServer> allAdServers, Dictionary<string, TopologySite> aDServerSiteDictionary, Dictionary<string, ReadOnlyCollection<ADServer>> siteADServerDictionary, Dictionary<string, TopologySite> siteDictionary, string localServerFqdn)
		{
			this.discoveryStarted = discoveryStarted;
			this.topologyScope = topologyScope;
			this.allTopologyServers = allTopologyServers;
			this.allTopologySites = allTopologySites;
			this.allTopologySiteLinks = allTopologySiteLinks;
			this.allVirtualDirectories = allVirtualDirectories;
			this.allEmailTransports = allEmailTransports;
			this.allSmtpReceiveConnectors = allSmtpReceiveConnectors;
			this.aDServerSiteDictionary = aDServerSiteDictionary;
			this.siteADServerDictionary = siteADServerDictionary;
			this.whenCreated = DateTime.UtcNow;
			ExTraceGlobals.ExchangeTopologyTracer.TracePfd<int>(0L, "PFD ADPEXT {0} - Creating ExchangeTopology for public consumption", 25525);
			this.exchangeServerDictionary = new Dictionary<string, TopologyServer>(this.allTopologyServers.Count, StringComparer.OrdinalIgnoreCase);
			foreach (TopologyServer topologyServer in this.allTopologyServers)
			{
				this.exchangeServerDictionary.Add(topologyServer.Id.DistinguishedName, topologyServer);
				if (this.localServer == null && string.Compare(localServerFqdn, topologyServer.Fqdn, StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.localServer = topologyServer;
				}
			}
			ExTraceGlobals.ExchangeTopologyTracer.TracePfd<int, string>(0L, "PFD ADPEXT {0} - The local server is {1}", 17333, (this.localServer != null) ? this.localServer.Fqdn : "<undefined>");
			if (this.localServer != null)
			{
				this.localSite = this.localServer.TopologySite;
				ExTraceGlobals.ExchangeTopologyTracer.TraceDebug<string>(0L, "Local site: {0}", (this.localSite != null) ? this.localSite.Name : "none");
			}
			else
			{
				string siteName = NativeHelpers.GetSiteName(false);
				if (string.IsNullOrEmpty(siteName))
				{
					ExTraceGlobals.ExchangeTopologyTracer.TraceDebug(0L, "Computer doesn't belong to any site");
				}
				else
				{
					ExTraceGlobals.ExchangeTopologyTracer.TraceDebug<string>(0L, "GetSiteName returned: {0}", siteName);
					foreach (TopologySite topologySite in this.allTopologySites)
					{
						if (string.Compare(topologySite.Name, siteName) == 0)
						{
							this.localSite = topologySite;
							ExTraceGlobals.ExchangeTopologyTracer.TraceDebug<string>(0L, "Local site: {0}", this.localSite.Name);
							break;
						}
					}
				}
			}
			ExTraceGlobals.ExchangeTopologyTracer.TracePfd<int, string>(0L, "PFD ADPEXT {0} - The local site is {1}", 31669, (this.localSite != null) ? this.localSite.Name : "<undefined>");
			if (allAdServers != null)
			{
				this.adServerDictionary = new Dictionary<string, ADServer>(allAdServers.Count, StringComparer.OrdinalIgnoreCase);
				foreach (ADServer adserver in allAdServers)
				{
					this.adServerDictionary.Add(adserver.DnsHostName, adserver);
				}
			}
			if (ExTraceGlobals.ExchangeTopologyTracer.IsTraceEnabled(TraceType.PfdTrace))
			{
				foreach (TopologyServer topologyServer2 in this.allTopologyServers)
				{
					ExTraceGlobals.ExchangeTopologyTracer.TracePfd<int, string, string>((long)this.GetHashCode(), "PFD ADPEXT {0} - Server: {1} belongs to {2}", 23477, topologyServer2.Name, (topologyServer2.TopologySite == null) ? "no site" : topologyServer2.TopologySite.Name);
					ExTraceGlobals.ExchangeTopologyTracer.TracePfd<int, string, string>((long)this.GetHashCode(), "PFD ADPEXT {0} - Server: FQDN for {1} is {2}", 22453, topologyServer2.Name, topologyServer2.Fqdn);
				}
				foreach (TopologySite topologySite2 in this.allTopologySites)
				{
					StringBuilder stringBuilder = new StringBuilder();
					foreach (ITopologySiteLink topologySiteLink in topologySite2.TopologySiteLinks)
					{
						TopologySiteLink topologySiteLink2 = (TopologySiteLink)topologySiteLink;
						stringBuilder.Append(topologySiteLink2.Name);
						stringBuilder.Append(", ");
					}
					ExTraceGlobals.ExchangeTopologyTracer.TracePfd<int, string, StringBuilder>((long)this.GetHashCode(), "PFD ADPEXT {0} - Site: {1} links to {2}", 30645, topologySite2.Name, stringBuilder);
				}
				foreach (TopologySiteLink topologySiteLink3 in this.allTopologySiteLinks)
				{
					StringBuilder stringBuilder2 = new StringBuilder();
					foreach (ITopologySite topologySite3 in topologySiteLink3.TopologySites)
					{
						TopologySite topologySite4 = (TopologySite)topologySite3;
						stringBuilder2.Append(topologySite4.Name);
						stringBuilder2.Append(", ");
					}
					ExTraceGlobals.ExchangeTopologyTracer.TracePfd<int, string, StringBuilder>((long)this.GetHashCode(), "PFD ADPEXT {0} - SiteLink: {1} connects {2}", 19381, topologySiteLink3.Name, stringBuilder2);
				}
				if (this.allVirtualDirectories != null)
				{
					foreach (MiniVirtualDirectory miniVirtualDirectory in this.allVirtualDirectories)
					{
						ExTraceGlobals.ExchangeTopologyTracer.TracePfd<int, string, ADObjectId>((long)this.GetHashCode(), "PFD ADPEXT {0} - VirtualDirectory: {1} on {2}", 27573, miniVirtualDirectory.Name, miniVirtualDirectory.Server);
					}
				}
				if (this.allEmailTransports != null)
				{
					foreach (MiniEmailTransport miniEmailTransport in this.allEmailTransports)
					{
						ExTraceGlobals.ExchangeTopologyTracer.TracePfd<int, string>((long)this.GetHashCode(), "PFD ADPEXT {0} - Email Transport: {1}", 63987, miniEmailTransport.Name);
					}
				}
				if (this.allSmtpReceiveConnectors != null)
				{
					foreach (MiniReceiveConnector miniReceiveConnector in this.allSmtpReceiveConnectors)
					{
						ExTraceGlobals.ExchangeTopologyTracer.TracePfd<int, string>((long)this.GetHashCode(), "PFD ADPEXT {0} - SMTP Receive Connector: {1}", 47603, miniReceiveConnector.Name);
					}
				}
				if (allAdServers != null)
				{
					foreach (ADServer adserver2 in allAdServers)
					{
						ExTraceGlobals.ExchangeTopologyTracer.TracePfd<int, string, AdName>((long)this.GetHashCode(), "PFD ADPEXT {0} - Domain Controller: {1} on Site {2}", 54149, adserver2.DnsHostName, adserver2.Id.Parent.Parent.Rdn);
					}
				}
			}
		}

		public static ExchangeTopology RsoTopology
		{
			get
			{
				ExchangeTopology.RefreshRsoTopology();
				return ExchangeTopology.rsoTopology;
			}
		}

		public DateTime WhenCreated
		{
			get
			{
				return this.whenCreated;
			}
		}

		public DateTime DiscoveryStarted
		{
			get
			{
				return this.discoveryStarted;
			}
		}

		public ReadOnlyCollection<TopologyServer> AllTopologyServers
		{
			get
			{
				return this.allTopologyServers;
			}
		}

		public ReadOnlyCollection<TopologySite> AllTopologySites
		{
			get
			{
				return this.allTopologySites;
			}
		}

		public ReadOnlyCollection<TopologySiteLink> AllTopologySiteLinks
		{
			get
			{
				return this.allTopologySiteLinks;
			}
		}

		public ReadOnlyCollection<MiniVirtualDirectory> AllVirtualDirectories
		{
			get
			{
				return this.allVirtualDirectories;
			}
		}

		public ReadOnlyCollection<MiniEmailTransport> AllEmailTransports
		{
			get
			{
				return this.allEmailTransports;
			}
		}

		public ReadOnlyCollection<MiniReceiveConnector> AllSmtpReceiveConnectors
		{
			get
			{
				return this.allSmtpReceiveConnectors;
			}
		}

		public TopologyServer LocalServer
		{
			get
			{
				return this.localServer;
			}
		}

		public TopologySite LocalSite
		{
			get
			{
				return this.localSite;
			}
		}

		public static ExchangeTopology Discover()
		{
			return ExchangeTopology.Discover(null, ExchangeTopologyScope.Complete);
		}

		public static ExchangeTopology Discover(ITopologyConfigurationSession session)
		{
			return ExchangeTopology.Discover(session, ExchangeTopologyScope.Complete);
		}

		public static ExchangeTopology Discover(ExchangeTopologyScope scope)
		{
			return ExchangeTopology.Discover(null, scope);
		}

		public static ExchangeTopology Discover(ITopologyConfigurationSession session, ExchangeTopologyScope scope)
		{
			ExchangeTopologyDiscovery topologyDiscovery = ExchangeTopologyDiscovery.Create(session, scope);
			return ExchangeTopologyDiscovery.Populate(topologyDiscovery);
		}

		public TopologySite FindClosestDestinationSite(TopologySite sourceSite, ICollection<TopologySite> destinationSites)
		{
			if (destinationSites == null || destinationSites.Count == 0)
			{
				return null;
			}
			if (sourceSite == null || destinationSites.Contains(sourceSite))
			{
				return sourceSite;
			}
			ReadOnlyCollection<TopologySite> sitesSortedByCostFromSite = this.GetSitesSortedByCostFromSite(sourceSite);
			foreach (TopologySite topologySite in sitesSortedByCostFromSite)
			{
				if (destinationSites.Contains(topologySite))
				{
					return topologySite;
				}
			}
			return null;
		}

		public TopologyServer GetTopologyServer(ADObjectId serverId)
		{
			TopologyServer topologyServer = null;
			this.exchangeServerDictionary.TryGetValue(serverId.DistinguishedName, out topologyServer);
			ExTraceGlobals.ExchangeTopologyTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Mapped {0} to {1}", serverId.DistinguishedName, (topologyServer == null) ? "<null>" : topologyServer.Name);
			return topologyServer;
		}

		public ADServer GetAdServer(string fqdn)
		{
			if (this.topologyScope != ExchangeTopologyScope.Complete && this.topologyScope != ExchangeTopologyScope.ADAndExchangeServerAndSiteTopology)
			{
				throw new InvalidOperationException("GetAdSever is only supported for Complete and ADAndExchangeServerAndSiteTopology scopes");
			}
			ADServer adserver = null;
			this.adServerDictionary.TryGetValue(fqdn, out adserver);
			ExTraceGlobals.ExchangeTopologyTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Mapped {0} to {1}", fqdn, (adserver == null) ? "<null>" : adserver.DistinguishedName);
			return adserver;
		}

		public override string ToString()
		{
			return string.Format("ExchangeTopology generated at {0} with {1} servers", this.whenCreated, this.allTopologyServers.Count);
		}

		public TopologySite SiteFromADServer(string fqdn)
		{
			if (this.topologyScope != ExchangeTopologyScope.Complete && this.topologyScope != ExchangeTopologyScope.ADAndExchangeServerAndSiteTopology)
			{
				throw new InvalidOperationException("SiteFromADServer is only supported for Complete and ADAndExchangeServerAndSiteTopology scopes");
			}
			TopologySite topologySite;
			if (!this.aDServerSiteDictionary.TryGetValue(fqdn, out topologySite))
			{
				ExTraceGlobals.ExchangeTopologyTracer.TraceDebug<string>(0L, "ADServer {0} is not in list of ADServers with sites.", fqdn);
				return null;
			}
			ExTraceGlobals.ExchangeTopologyTracer.TraceDebug<string, string>(0L, "Found site {0} for ADServer {1}.", topologySite.Name, fqdn);
			return topologySite;
		}

		public ReadOnlyCollection<ADServer> ADServerFromSite(string siteDN)
		{
			if (this.topologyScope != ExchangeTopologyScope.Complete && this.topologyScope != ExchangeTopologyScope.ADAndExchangeServerAndSiteTopology)
			{
				throw new InvalidOperationException("ADSeverFromSite is only supported for Complete and ADAndExchangeServerAndSiteTopology scopes");
			}
			ReadOnlyCollection<ADServer> readOnlyCollection = null;
			if (!this.siteADServerDictionary.TryGetValue(siteDN, out readOnlyCollection))
			{
				ExTraceGlobals.ExchangeTopologyTracer.TraceDebug<string>(0L, "{0} is not a valid key in the ADSite-ADServer list.", siteDN);
				return new ReadOnlyCollection<ADServer>(new List<ADServer>());
			}
			ExTraceGlobals.ExchangeTopologyTracer.TraceDebug<string, string>(0L, "Found ADServers from ADSite {0}. First DC is {1}", siteDN, (readOnlyCollection.Count > 0) ? readOnlyCollection[0].DnsHostName : "<null>");
			return readOnlyCollection;
		}

		public ReadOnlyCollection<TopologySite> GetSitesSortedByCostFromSite(TopologySite sourceSite)
		{
			return new ReadOnlyCollection<TopologySite>(ExchangeTopologyDiscovery.OrderDestinationSites(this.allTopologySites, sourceSite, this.allTopologySites));
		}

		private static void RefreshRsoTopology()
		{
			if (ExchangeTopology.rsoTopology == null)
			{
				lock (ExchangeTopology.rsoTopologyLock)
				{
					if (ExchangeTopology.rsoTopology == null)
					{
						ExchangeTopology.rsoTopology = ExchangeTopology.Discover(ExchangeTopologyScope.ADAndExchangeServerAndSiteTopology);
					}
					return;
				}
			}
			if (ExchangeTopology.rsoTopology.whenCreated.AddMinutes(10.0).CompareTo(DateTime.UtcNow) < 0)
			{
				lock (ExchangeTopology.rsoTopologyLock)
				{
					if (ExchangeTopology.rsoTopology.whenCreated.AddMinutes(10.0).CompareTo(DateTime.UtcNow) < 0)
					{
						ExchangeTopology exchangeTopology = ExchangeTopology.Discover(ExchangeTopologyScope.ADAndExchangeServerAndSiteTopology);
						ExchangeTopology.rsoTopology = exchangeTopology;
					}
				}
			}
		}

		private static ExchangeTopology rsoTopology = null;

		private static object rsoTopologyLock = new object();

		private readonly DateTime whenCreated;

		private readonly DateTime discoveryStarted;

		private readonly ExchangeTopologyScope topologyScope;

		private ReadOnlyCollection<TopologyServer> allTopologyServers;

		private ReadOnlyCollection<TopologySite> allTopologySites;

		private ReadOnlyCollection<TopologySiteLink> allTopologySiteLinks;

		private ReadOnlyCollection<MiniVirtualDirectory> allVirtualDirectories;

		private ReadOnlyCollection<MiniEmailTransport> allEmailTransports;

		private ReadOnlyCollection<MiniReceiveConnector> allSmtpReceiveConnectors;

		private TopologyServer localServer;

		private TopologySite localSite;

		private Dictionary<string, TopologyServer> exchangeServerDictionary;

		private Dictionary<string, ADServer> adServerDictionary;

		private Dictionary<string, TopologySite> aDServerSiteDictionary;

		private Dictionary<string, ReadOnlyCollection<ADServer>> siteADServerDictionary;
	}
}
