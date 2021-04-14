using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RequestedNetworkIsNotDhcpEnabled : ClusCommonValidationFailedException
	{
		public RequestedNetworkIsNotDhcpEnabled(string network) : base(Strings.RequestedNetworkIsNotDhcpEnabled(network))
		{
			this.network = network;
		}

		public RequestedNetworkIsNotDhcpEnabled(string network, Exception innerException) : base(Strings.RequestedNetworkIsNotDhcpEnabled(network), innerException)
		{
			this.network = network;
		}

		protected RequestedNetworkIsNotDhcpEnabled(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.network = (string)info.GetValue("network", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("network", this.network);
		}

		public string Network
		{
			get
			{
				return this.network;
			}
		}

		private readonly string network;
	}
}
