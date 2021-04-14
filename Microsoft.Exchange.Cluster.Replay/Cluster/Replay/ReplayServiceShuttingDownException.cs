using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayServiceShuttingDownException : TaskServerException
	{
		public ReplayServiceShuttingDownException() : base(ReplayStrings.ReplayServiceShuttingDownException)
		{
		}

		public ReplayServiceShuttingDownException(Exception innerException) : base(ReplayStrings.ReplayServiceShuttingDownException, innerException)
		{
		}

		protected ReplayServiceShuttingDownException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
