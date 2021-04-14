using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmClusterException : AmCommonException
	{
		public AmClusterException(string clusterError) : base(ReplayStrings.AmClusterException(clusterError))
		{
			this.clusterError = clusterError;
		}

		public AmClusterException(string clusterError, Exception innerException) : base(ReplayStrings.AmClusterException(clusterError), innerException)
		{
			this.clusterError = clusterError;
		}

		protected AmClusterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.clusterError = (string)info.GetValue("clusterError", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("clusterError", this.clusterError);
		}

		public string ClusterError
		{
			get
			{
				return this.clusterError;
			}
		}

		private readonly string clusterError;
	}
}
