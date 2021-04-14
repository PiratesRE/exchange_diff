using System;
using System.Net;

namespace Microsoft.Exchange.Net
{
	internal class DnsAddressRecord : DnsRecord
	{
		protected DnsAddressRecord(string name) : base(name)
		{
		}

		protected DnsAddressRecord(string name, IPAddress address) : base(name)
		{
			this.address = address;
		}

		protected DnsAddressRecord(Win32DnsRecordHeader header) : base(header)
		{
		}

		public IPAddress IPAddress
		{
			get
			{
				return this.address;
			}
		}

		public override string ToString()
		{
			return base.ToString() + " " + this.address.ToString();
		}

		protected IPAddress address;
	}
}
