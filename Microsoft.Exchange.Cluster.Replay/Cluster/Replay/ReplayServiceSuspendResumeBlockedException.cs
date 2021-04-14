using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayServiceSuspendResumeBlockedException : TaskServerException
	{
		public ReplayServiceSuspendResumeBlockedException() : base(ReplayStrings.ReplayServiceSuspendResumeBlockedException)
		{
		}

		public ReplayServiceSuspendResumeBlockedException(Exception innerException) : base(ReplayStrings.ReplayServiceSuspendResumeBlockedException, innerException)
		{
		}

		protected ReplayServiceSuspendResumeBlockedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
