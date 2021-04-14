using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ClusterException : TransientException
	{
		public ClusterException(string clusterError) : base(Strings.ClusterException(clusterError))
		{
			this.clusterError = clusterError;
		}

		public ClusterException(string clusterError, Exception innerException) : base(Strings.ClusterException(clusterError), innerException)
		{
			this.clusterError = clusterError;
		}

		protected ClusterException(SerializationInfo info, StreamingContext context) : base(info, context)
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
