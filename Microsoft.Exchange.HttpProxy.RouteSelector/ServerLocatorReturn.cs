using System;
using System.Collections.Generic;
using Microsoft.Exchange.HttpProxy.Routing;

namespace Microsoft.Exchange.HttpProxy.RouteSelector
{
	internal class ServerLocatorReturn
	{
		internal ServerLocatorReturn()
		{
			this.ServerFqdn = string.Empty;
			this.ServerVersion = null;
			this.SuccessKey = null;
			this.RoutingEntries = null;
		}

		internal ServerLocatorReturn(string serverFqdn, int? serverVersion, IRoutingKey successKey, IList<IRoutingEntry> routingEntries)
		{
			this.ServerFqdn = serverFqdn;
			this.ServerVersion = serverVersion;
			this.SuccessKey = successKey;
			this.RoutingEntries = routingEntries;
		}

		public string ServerFqdn { get; set; }

		public int? ServerVersion { get; set; }

		public IRoutingKey SuccessKey { get; set; }

		public IList<IRoutingEntry> RoutingEntries { get; set; }
	}
}
