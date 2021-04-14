using System;
using System.Net;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class SmartHost
	{
		public SmartHost(string address) : this(new RoutingHost(address))
		{
		}

		public SmartHost(RoutingHost routingHost)
		{
			this.routingHost = routingHost;
			if (!routingHost.Domain.Equals(RoutingHostName.Empty))
			{
				this.hostName = new Hostname(routingHost.Domain);
			}
		}

		public IPAddress Address
		{
			get
			{
				return this.routingHost.Address;
			}
		}

		public bool IsIPAddress
		{
			get
			{
				return this.routingHost.IsIPAddress;
			}
		}

		public Hostname Domain
		{
			get
			{
				return this.hostName;
			}
		}

		internal RoutingHost InnerRoutingHost
		{
			get
			{
				return this.routingHost;
			}
		}

		public bool Equals(SmartHost value)
		{
			return object.ReferenceEquals(this, value) || this.routingHost.Equals(value.routingHost);
		}

		public override bool Equals(object comparand)
		{
			SmartHost smartHost = comparand as SmartHost;
			return smartHost != null && this.Equals(smartHost);
		}

		public override int GetHashCode()
		{
			return this.routingHost.GetHashCode();
		}

		public override string ToString()
		{
			return this.routingHost.ToString();
		}

		public static SmartHost Parse(string address)
		{
			return new SmartHost(address);
		}

		public static bool TryParse(string address, out SmartHost smarthost)
		{
			smarthost = null;
			RoutingHost routingHost;
			if (RoutingHost.TryParse(address, out routingHost))
			{
				smarthost = new SmartHost(routingHost);
				return true;
			}
			return false;
		}

		private readonly RoutingHost routingHost;

		private readonly Hostname hostName;
	}
}
