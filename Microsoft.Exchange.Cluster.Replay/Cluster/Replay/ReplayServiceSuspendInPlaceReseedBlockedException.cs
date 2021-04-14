using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayServiceSuspendInPlaceReseedBlockedException : TaskServerException
	{
		public ReplayServiceSuspendInPlaceReseedBlockedException() : base(ReplayStrings.ReplayServiceSuspendInPlaceReseedBlockedException)
		{
		}

		public ReplayServiceSuspendInPlaceReseedBlockedException(Exception innerException) : base(ReplayStrings.ReplayServiceSuspendInPlaceReseedBlockedException, innerException)
		{
		}

		protected ReplayServiceSuspendInPlaceReseedBlockedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
