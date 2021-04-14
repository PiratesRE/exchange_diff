using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayServiceSuspendWantedClearedException : TaskServerException
	{
		public ReplayServiceSuspendWantedClearedException() : base(ReplayStrings.ReplayServiceSuspendWantedClearedException)
		{
		}

		public ReplayServiceSuspendWantedClearedException(Exception innerException) : base(ReplayStrings.ReplayServiceSuspendWantedClearedException, innerException)
		{
		}

		protected ReplayServiceSuspendWantedClearedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
