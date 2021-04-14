using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingDestinations
{
	internal class ServerRoutingDestination : RoutingDestinationBase
	{
		public ServerRoutingDestination(string fqdn, int? version)
		{
			if (fqdn == null)
			{
				throw new ArgumentNullException("fqdn");
			}
			this.fqdn = fqdn;
			this.version = version;
		}

		public string Fqdn
		{
			get
			{
				return this.fqdn;
			}
		}

		public int? Version
		{
			get
			{
				return this.version;
			}
		}

		public override RoutingItemType RoutingItemType
		{
			get
			{
				return RoutingItemType.Server;
			}
		}

		public override string Value
		{
			get
			{
				return this.fqdn;
			}
		}

		public override IList<string> Properties
		{
			get
			{
				if (this.version != null)
				{
					return new string[]
					{
						this.version.ToString()
					};
				}
				return Array<string>.Empty;
			}
		}

		public static bool TryParse(string value, IList<string> properties, out ServerRoutingDestination destination)
		{
			destination = null;
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (properties == null)
			{
				throw new ArgumentNullException("properties");
			}
			if (properties.Count == 0)
			{
				destination = new ServerRoutingDestination(value, null);
				return true;
			}
			int value2;
			if (properties.Count > 0 && int.TryParse(properties[0], out value2))
			{
				destination = new ServerRoutingDestination(value, new int?(value2));
				return true;
			}
			return false;
		}

		private readonly string fqdn;

		private readonly int? version;
	}
}
