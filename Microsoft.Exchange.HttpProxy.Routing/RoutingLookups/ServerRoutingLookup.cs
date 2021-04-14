using System;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.HttpProxy.Routing.RoutingDestinations;
using Microsoft.Exchange.HttpProxy.Routing.RoutingEntries;
using Microsoft.Exchange.HttpProxy.Routing.RoutingKeys;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingLookups
{
	internal class ServerRoutingLookup : IRoutingLookup, IServerVersionLookup
	{
		public ServerRoutingLookup()
		{
			this.versionLookup = this;
		}

		public ServerRoutingLookup(IServerVersionLookup versionLookup)
		{
			this.versionLookup = versionLookup;
		}

		IRoutingEntry IRoutingLookup.GetRoutingEntry(IRoutingKey routingKey, IRoutingDiagnostics diagnostics)
		{
			if (routingKey == null)
			{
				throw new ArgumentNullException("routingKey");
			}
			if (diagnostics == null)
			{
				throw new ArgumentNullException("diagnostics");
			}
			ServerRoutingKey serverRoutingKey = routingKey as ServerRoutingKey;
			if (serverRoutingKey == null)
			{
				string message = string.Format("Routing key type {0} is not supported", routingKey.GetType());
				throw new ArgumentException(message, "routingKey");
			}
			if (!string.IsNullOrEmpty(serverRoutingKey.Server))
			{
				int? version = null;
				if (serverRoutingKey.Version != null)
				{
					version = serverRoutingKey.Version;
				}
				else
				{
					version = this.versionLookup.LookupVersion(serverRoutingKey.Server);
				}
				return new SuccessfulServerRoutingEntry(serverRoutingKey, new ServerRoutingDestination(serverRoutingKey.Server, version), DateTime.UtcNow.ToFileTimeUtc());
			}
			ErrorRoutingDestination destination = new ErrorRoutingDestination("Could not extract server from ServerRoutingKey");
			return new FailedServerRoutingEntry(serverRoutingKey, destination, DateTime.UtcNow.ToFileTimeUtc());
		}

		int? IServerVersionLookup.LookupVersion(string server)
		{
			return ServerLookup.LookupVersion(server);
		}

		private IServerVersionLookup versionLookup;
	}
}
