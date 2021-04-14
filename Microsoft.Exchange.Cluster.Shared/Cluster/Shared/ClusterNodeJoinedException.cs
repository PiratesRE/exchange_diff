using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ClusterNodeJoinedException : ClusterException
	{
		public ClusterNodeJoinedException(string nodeName) : base(Strings.ClusterNodeJoinedException(nodeName))
		{
			this.nodeName = nodeName;
		}

		public ClusterNodeJoinedException(string nodeName, Exception innerException) : base(Strings.ClusterNodeJoinedException(nodeName), innerException)
		{
			this.nodeName = nodeName;
		}

		protected ClusterNodeJoinedException(SerializationInfo info, StreamingContext context) : base(info, context)
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
