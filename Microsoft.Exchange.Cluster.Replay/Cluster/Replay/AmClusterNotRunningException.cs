using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmClusterNotRunningException : ClusterException
	{
		public AmClusterNotRunningException() : base(ReplayStrings.AmClusterNotRunningException)
		{
		}

		public AmClusterNotRunningException(Exception innerException) : base(ReplayStrings.AmClusterNotRunningException, innerException)
		{
		}

		protected AmClusterNotRunningException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
