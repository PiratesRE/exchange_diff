using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RequestedNetworkIsNotIPv6Enabled : ClusCommonValidationFailedException
	{
		public RequestedNetworkIsNotIPv6Enabled(string network) : base(Strings.RequestedNetworkIsNotIPv6Enabled(network))
		{
			this.network = network;
		}

		public RequestedNetworkIsNotIPv6Enabled(string network, Exception innerException) : base(Strings.RequestedNetworkIsNotIPv6Enabled(network), innerException)
		{
			this.network = network;
		}

		protected RequestedNetworkIsNotIPv6Enabled(SerializationInfo info, StreamingContext context) : base(info, context)
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
