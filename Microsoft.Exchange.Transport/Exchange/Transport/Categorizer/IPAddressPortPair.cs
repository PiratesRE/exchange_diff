using System;
using System.Net;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class IPAddressPortPair
	{
		public IPAddressPortPair(IPAddress address, ushort port)
		{
			this.address = address;
			this.port = port;
		}

		public override bool Equals(object obj)
		{
			IPAddressPortPair ipaddressPortPair = obj as IPAddressPortPair;
			return ipaddressPortPair != null && this.address.Equals(ipaddressPortPair.address) && this.port == ipaddressPortPair.port;
		}

		public override int GetHashCode()
		{
			return this.address.GetHashCode() ^ (int)this.port;
		}

		public override string ToString()
		{
			return string.Format("{0}:{1}", this.address, this.port.ToString());
		}

		private readonly IPAddress address;

		private readonly ushort port;
	}
}
