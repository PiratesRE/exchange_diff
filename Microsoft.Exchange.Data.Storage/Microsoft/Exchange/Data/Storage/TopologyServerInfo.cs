using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TopologyServerInfo
	{
		public TopologyServerInfo(Site site, TopologyServer server)
		{
			this.Site = site;
			this.DistinguishedName = server.DistinguishedName;
			this.ServerFullyQualifiedDomainName = server.Fqdn;
			this.VersionNumber = server.VersionNumber;
			this.AdminDisplayVersionNumber = server.AdminDisplayVersion;
			this.Role = server.CurrentServerRole;
			this.IsOutOfService = ((bool)server[ActiveDirectoryServerSchema.IsOutOfService] || !ServerComponentStates.IsServerOnline(server.ComponentStates));
		}

		public string DistinguishedName { get; private set; }

		public Site Site { get; private set; }

		public string ServerFullyQualifiedDomainName { get; private set; }

		public int VersionNumber { get; private set; }

		public ServerVersion AdminDisplayVersionNumber { get; private set; }

		public ServerRole Role { get; private set; }

		public bool IsOutOfService { get; private set; }

		internal static TopologyServerInfo Get(TopologyServer server, ServiceTopology.All all)
		{
			TopologyServerInfo topologyServerInfo;
			if (!all.Servers.TryGetValue(server.DistinguishedName, out topologyServerInfo))
			{
				Site site = Site.Get(server.TopologySite, all);
				topologyServerInfo = new TopologyServerInfo(site, server);
				all.Servers.Add(topologyServerInfo.DistinguishedName, topologyServerInfo);
			}
			return topologyServerInfo;
		}
	}
}
