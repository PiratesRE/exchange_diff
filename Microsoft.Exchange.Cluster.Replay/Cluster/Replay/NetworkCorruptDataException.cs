using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NetworkCorruptDataException : NetworkTransportException
	{
		public NetworkCorruptDataException(string srcNode) : base(ReplayStrings.NetworkCorruptData(srcNode))
		{
			this.srcNode = srcNode;
		}

		public NetworkCorruptDataException(string srcNode, Exception innerException) : base(ReplayStrings.NetworkCorruptData(srcNode), innerException)
		{
			this.srcNode = srcNode;
		}

		protected NetworkCorruptDataException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.srcNode = (string)info.GetValue("srcNode", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("srcNode", this.srcNode);
		}

		public string SrcNode
		{
			get
			{
				return this.srcNode;
			}
		}

		private readonly string srcNode;
	}
}
