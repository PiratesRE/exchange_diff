using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class DagNetwork
	{
		public DagNetwork(DatabaseAvailabilityGroupNetwork network)
		{
			this.Identity = network.Identity.ToIdentity(network.Name);
			this.ReplicationEnabled = network.ReplicationEnabled;
		}

		[DataMember]
		public Identity Identity { get; set; }

		[DataMember]
		public bool ReplicationEnabled { get; set; }
	}
}
