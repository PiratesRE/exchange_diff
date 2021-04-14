using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class DAGNetworkInterfaceItem
	{
		public DAGNetworkInterfaceItem(DatabaseAvailabilityGroupNetworkInterface networkInterface)
		{
			this.state = networkInterface.State.ToString();
			this.ipAddress = networkInterface.IPAddress.ToString();
		}

		[DataMember]
		public string InterfaceState
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

		[DataMember]
		public string InterfaceIPAddress
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

		public override string ToString()
		{
			return string.Format("Interface IP address: {0}, Interface State: {1}", this.ipAddress, this.state);
		}

		private readonly string state;

		private readonly string ipAddress;
	}
}
