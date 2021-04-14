using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ClusterEvictWithoutCleanupException : ClusterException
	{
		public ClusterEvictWithoutCleanupException(string nodeName) : base(Strings.ClusterEvictWithoutCleanupException(nodeName))
		{
			this.nodeName = nodeName;
		}

		public ClusterEvictWithoutCleanupException(string nodeName, Exception innerException) : base(Strings.ClusterEvictWithoutCleanupException(nodeName), innerException)
		{
			this.nodeName = nodeName;
		}

		protected ClusterEvictWithoutCleanupException(SerializationInfo info, StreamingContext context) : base(info, context)
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
