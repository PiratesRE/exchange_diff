using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IpResourceCreationOnWrongTypeOfNetworkException : ClusCommonValidationFailedException
	{
		public IpResourceCreationOnWrongTypeOfNetworkException(string network) : base(Strings.IpResourceCreationOnWrongTypeOfNetworkException(network))
		{
			this.network = network;
		}

		public IpResourceCreationOnWrongTypeOfNetworkException(string network, Exception innerException) : base(Strings.IpResourceCreationOnWrongTypeOfNetworkException(network), innerException)
		{
			this.network = network;
		}

		protected IpResourceCreationOnWrongTypeOfNetworkException(SerializationInfo info, StreamingContext context) : base(info, context)
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
