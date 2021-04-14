using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayServiceSuspendBlockedResynchronizingException : TaskServerException
	{
		public ReplayServiceSuspendBlockedResynchronizingException() : base(ReplayStrings.ReplayServiceSuspendBlockedResynchronizingException)
		{
		}

		public ReplayServiceSuspendBlockedResynchronizingException(Exception innerException) : base(ReplayStrings.ReplayServiceSuspendBlockedResynchronizingException, innerException)
		{
		}

		protected ReplayServiceSuspendBlockedResynchronizingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
