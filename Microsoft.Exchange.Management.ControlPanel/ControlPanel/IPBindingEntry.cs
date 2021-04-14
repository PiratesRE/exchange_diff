using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class IPBindingEntry
	{
		public IPBindingEntry(IPBinding ipBinding)
		{
			this.address = ipBinding.Address.ToString();
			if (string.Equals(this.address, "0.0.0.0"))
			{
				this.displayAddress = Strings.ConnectorAllAvailableIPv4;
			}
			else if (string.Equals(this.address, "::"))
			{
				this.displayAddress = Strings.ConnectorAllAvailableIPv6;
			}
			else
			{
				this.displayAddress = this.address;
			}
			this.port = ipBinding.Port;
			this.ipBindingKey = this.address + ":" + this.port;
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

		[DataMember]
		public string DisplayAddress
		{
			get
			{
				return this.displayAddress;
			}
			set
			{
				this.displayAddress = value;
			}
		}

		[DataMember]
		public int Port
		{
			get
			{
				return this.port;
			}
			set
			{
				this.port = value;
			}
		}

		[DataMember]
		public string IPBindingKey
		{
			get
			{
				return this.ipBindingKey;
			}
			set
			{
				this.ipBindingKey = value;
			}
		}

		public override bool Equals(object obj)
		{
			IPBindingEntry ipbindingEntry = obj as IPBindingEntry;
			return ipbindingEntry != null && string.Compare(ipbindingEntry.Address, this.Address, false) == 0 && ipbindingEntry.Port == this.Port;
		}

		public override int GetHashCode()
		{
			return this.Address.GetHashCode() + this.Port;
		}

		private string address;

		private string displayAddress;

		private int port;

		private string ipBindingKey;
	}
}
