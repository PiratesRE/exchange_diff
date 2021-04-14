using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayServiceSuspendReseedBlockedException : TaskServerException
	{
		public ReplayServiceSuspendReseedBlockedException() : base(ReplayStrings.ReplayServiceSuspendReseedBlockedException)
		{
		}

		public ReplayServiceSuspendReseedBlockedException(Exception innerException) : base(ReplayStrings.ReplayServiceSuspendReseedBlockedException, innerException)
		{
		}

		protected ReplayServiceSuspendReseedBlockedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
