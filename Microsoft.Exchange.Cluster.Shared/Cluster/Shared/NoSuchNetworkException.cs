using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoSuchNetworkException : ClusCommonValidationFailedException
	{
		public NoSuchNetworkException(string netName) : base(Strings.NoSuchNetwork(netName))
		{
			this.netName = netName;
		}

		public NoSuchNetworkException(string netName, Exception innerException) : base(Strings.NoSuchNetwork(netName), innerException)
		{
			this.netName = netName;
		}

		protected NoSuchNetworkException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.netName = (string)info.GetValue("netName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("netName", this.netName);
		}

		public string NetName
		{
			get
			{
				return this.netName;
			}
		}

		private readonly string netName;
	}
}
