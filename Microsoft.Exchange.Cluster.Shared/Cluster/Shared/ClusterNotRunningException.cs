using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ClusterNotRunningException : ClusterException
	{
		public ClusterNotRunningException() : base(Strings.ClusterNotRunningException)
		{
		}

		public ClusterNotRunningException(Exception innerException) : base(Strings.ClusterNotRunningException, innerException)
		{
		}

		protected ClusterNotRunningException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
