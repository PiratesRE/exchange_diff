using System;

namespace Microsoft.Exchange.Data.Transport
{
	[Serializable]
	internal struct RoutingHostName
	{
		public RoutingHostName(string address)
		{
			if (!RoutingHostName.IsValidName(address))
			{
				throw new ArgumentException(string.Format("The specified host name '{0}' isn't valid", address));
			}
			this.hostnamestring = address;
		}

		public string HostnameString
		{
			get
			{
				return this.hostnamestring;
			}
		}

		public static RoutingHostName Parse(string address)
		{
			return new RoutingHostName(address);
		}

		public static bool TryParse(string address, out RoutingHostName hostname)
		{
			hostname = RoutingHostName.Empty;
			if (string.IsNullOrEmpty(address))
			{
				return false;
			}
			if (!RoutingHostName.IsValidName(address))
			{
				return false;
			}
			hostname = new RoutingHostName(address);
			return true;
		}

		internal static bool IsValidName(string name)
		{
			return RoutingHostName.ValidateName(DNSNameFormat.Domain, name) == DNSNameStatus.Valid;
		}

		internal static DNSNameStatus ValidateName(DNSNameFormat format, string name)
		{
			if (!string.IsNullOrEmpty(name))
			{
				return (DNSNameStatus)DnsNativeMethods2.ValidateName(name, (int)format);
			}
			return DNSNameStatus.InvalidName;
		}

		public override string ToString()
		{
			if (this.hostnamestring != null)
			{
				return this.hostnamestring;
			}
			return string.Empty;
		}

		public bool Equals(RoutingHostName rhs)
		{
			if (this.hostnamestring != null)
			{
				return this.hostnamestring.Equals(rhs.hostnamestring, StringComparison.OrdinalIgnoreCase);
			}
			return rhs.hostnamestring == null;
		}

		public override bool Equals(object comparand)
		{
			RoutingHostName? routingHostName = comparand as RoutingHostName?;
			return routingHostName != null && this.Equals(routingHostName.Value);
		}

		public override int GetHashCode()
		{
			if (this.hostnamestring != null)
			{
				return this.hostnamestring.GetHashCode();
			}
			return 0;
		}

		private readonly string hostnamestring;

		public static RoutingHostName Empty = default(RoutingHostName);
	}
}
