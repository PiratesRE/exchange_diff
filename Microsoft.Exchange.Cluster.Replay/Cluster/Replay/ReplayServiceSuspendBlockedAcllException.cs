using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayServiceSuspendBlockedAcllException : TaskServerException
	{
		public ReplayServiceSuspendBlockedAcllException() : base(ReplayStrings.ReplayServiceSuspendBlockedAcllException)
		{
		}

		public ReplayServiceSuspendBlockedAcllException(Exception innerException) : base(ReplayStrings.ReplayServiceSuspendBlockedAcllException, innerException)
		{
		}

		protected ReplayServiceSuspendBlockedAcllException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
