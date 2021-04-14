using System;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class Hostname
	{
		public Hostname(string address) : this(new RoutingHostName(address))
		{
		}

		internal Hostname(RoutingHostName routingHostName)
		{
			if (routingHostName.Equals(RoutingHostName.Empty))
			{
				throw new ArgumentException("routingHostName");
			}
			this.routingHostName = routingHostName;
		}

		public string HostnameString
		{
			get
			{
				return this.routingHostName.HostnameString;
			}
		}

		public static Hostname Parse(string address)
		{
			return new Hostname(address);
		}

		public static bool TryParse(string address, out Hostname hostname)
		{
			hostname = null;
			RoutingHostName routingHostName;
			if (RoutingHostName.TryParse(address, out routingHostName))
			{
				hostname = new Hostname(routingHostName);
				return true;
			}
			return false;
		}

		public override string ToString()
		{
			return this.routingHostName.ToString();
		}

		public bool Equals(Hostname rhs)
		{
			return rhs != null && this.routingHostName.Equals(rhs.routingHostName);
		}

		public override bool Equals(object comparand)
		{
			Hostname hostname = comparand as Hostname;
			return hostname != null && this.Equals(hostname);
		}

		public override int GetHashCode()
		{
			return this.routingHostName.GetHashCode();
		}

		private readonly RoutingHostName routingHostName;
	}
}
