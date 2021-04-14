using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToFindNetwork : ClusCommonValidationFailedException
	{
		public FailedToFindNetwork(string network) : base(Strings.FailedToFindNetwork(network))
		{
			this.network = network;
		}

		public FailedToFindNetwork(string network, Exception innerException) : base(Strings.FailedToFindNetwork(network), innerException)
		{
			this.network = network;
		}

		protected FailedToFindNetwork(SerializationInfo info, StreamingContext context) : base(info, context)
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
