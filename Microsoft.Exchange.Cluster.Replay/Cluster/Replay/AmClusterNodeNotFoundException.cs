using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmClusterNodeNotFoundException : AmClusterException
	{
		public AmClusterNodeNotFoundException(string nodeName) : base(ReplayStrings.AmClusterNodeNotFoundException(nodeName))
		{
			this.nodeName = nodeName;
		}

		public AmClusterNodeNotFoundException(string nodeName, Exception innerException) : base(ReplayStrings.AmClusterNodeNotFoundException(nodeName), innerException)
		{
			this.nodeName = nodeName;
		}

		protected AmClusterNodeNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.nodeName = (string)info.GetValue("nodeName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("nodeName", this.nodeName);
		}

		public string NodeName
		{
			get
			{
				return this.nodeName;
			}
		}

		private readonly string nodeName;
	}
}
