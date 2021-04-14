using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class DAGNetworkSubnetItem
	{
		public DAGNetworkSubnetItem(DatabaseAvailabilityGroupNetworkSubnet subnet)
		{
			this.state = subnet.State.ToString();
			this.ipAddress = subnet.SubnetId.ToString();
		}

		[DataMember]
		public string SubnetIPAddress
		{
			get
			{
				return this.ipAddress;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember]
		public string SubnetState
		{
			get
			{
				return this.state;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public override string ToString()
		{
			return string.Format("Subnet IP: {0}, Subnet State: {1}", this.ipAddress, this.state);
		}

		private readonly string state;

		private readonly string ipAddress;
	}
}
