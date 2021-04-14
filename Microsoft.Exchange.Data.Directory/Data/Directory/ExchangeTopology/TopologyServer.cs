using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.ExchangeTopology
{
	internal sealed class TopologyServer : MiniTopologyServer
	{
		internal TopologyServer(MiniTopologyServer server)
		{
			this.propertyBag = server.propertyBag;
			this.m_Session = server.Session;
		}

		internal TopologyServer(Server server)
		{
			base.SetProperties(server);
			this.m_Session = server.Session;
		}

		public TopologySite TopologySite
		{
			get
			{
				return this.topologySite;
			}
			internal set
			{
				this.topologySite = value;
			}
		}

		private TopologySite topologySite;
	}
}
