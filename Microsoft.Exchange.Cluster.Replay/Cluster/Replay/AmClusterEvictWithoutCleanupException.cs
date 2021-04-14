using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmClusterEvictWithoutCleanupException : AmClusterException
	{
		public AmClusterEvictWithoutCleanupException(string nodeName) : base(ReplayStrings.AmClusterEvictWithoutCleanupException(nodeName))
		{
			this.nodeName = nodeName;
		}

		public AmClusterEvictWithoutCleanupException(string nodeName, Exception innerException) : base(ReplayStrings.AmClusterEvictWithoutCleanupException(nodeName), innerException)
		{
			this.nodeName = nodeName;
		}

		protected AmClusterEvictWithoutCleanupException(SerializationInfo info, StreamingContext context) : base(info, context)
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
