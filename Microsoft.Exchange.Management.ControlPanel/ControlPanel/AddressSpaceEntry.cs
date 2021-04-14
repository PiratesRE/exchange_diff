using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class AddressSpaceEntry
	{
		public AddressSpaceEntry(AddressSpace addressSpace)
		{
			this.type = addressSpace.Type;
			this.address = addressSpace.Address;
			this.cost = addressSpace.Cost;
			this.addressSpaceKey = string.Concat(new object[]
			{
				this.type,
				":",
				this.address,
				";",
				this.cost
			});
		}

		[DataMember]
		public string Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
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
		public int Cost
		{
			get
			{
				return this.cost;
			}
			set
			{
				this.cost = value;
			}
		}

		[DataMember]
		public string AddressSpaceKey
		{
			get
			{
				return this.addressSpaceKey;
			}
			set
			{
				this.addressSpaceKey = value;
			}
		}

		private string type;

		private string address;

		private int cost;

		private string addressSpaceKey;
	}
}
