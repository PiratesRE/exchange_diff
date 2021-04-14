using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ClusterNodeNotFoundException : ClusterException
	{
		public ClusterNodeNotFoundException(string nodeName) : base(Strings.ClusterNodeNotFoundException(nodeName))
		{
			this.nodeName = nodeName;
		}

		public ClusterNodeNotFoundException(string nodeName, Exception innerException) : base(Strings.ClusterNodeNotFoundException(nodeName), innerException)
		{
			this.nodeName = nodeName;
		}

		protected ClusterNodeNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
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
