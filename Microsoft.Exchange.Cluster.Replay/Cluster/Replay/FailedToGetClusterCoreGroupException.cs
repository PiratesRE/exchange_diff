using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToGetClusterCoreGroupException : TransientException
	{
		public FailedToGetClusterCoreGroupException() : base(ReplayStrings.ErrorFailedToGetClusterCoreGroup)
		{
		}

		public FailedToGetClusterCoreGroupException(Exception innerException) : base(ReplayStrings.ErrorFailedToGetClusterCoreGroup, innerException)
		{
		}

		protected FailedToGetClusterCoreGroupException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
