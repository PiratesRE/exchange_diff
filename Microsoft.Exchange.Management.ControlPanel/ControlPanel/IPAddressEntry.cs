using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class IPAddressEntry
	{
		public IPAddressEntry(IPRange range)
		{
			this.address = range.ToString();
		}

		[DataMember]
		public string Address
		{
			get
			{
				return this.address;
			}
			set
			{
				this.address = value;
			}
		}

		public override bool Equals(object obj)
		{
			IPAddressEntry ipaddressEntry = obj as IPAddressEntry;
			return ipaddressEntry != null && string.Compare(ipaddressEntry.Address, this.Address, true) == 0;
		}

		public override int GetHashCode()
		{
			return this.Address.GetHashCode();
		}

		private string address;
	}
}
