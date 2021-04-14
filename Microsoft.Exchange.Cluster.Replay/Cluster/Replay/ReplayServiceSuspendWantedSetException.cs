using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayServiceSuspendWantedSetException : TaskServerException
	{
		public ReplayServiceSuspendWantedSetException() : base(ReplayStrings.ReplayServiceSuspendWantedSetException)
		{
		}

		public ReplayServiceSuspendWantedSetException(Exception innerException) : base(ReplayStrings.ReplayServiceSuspendWantedSetException, innerException)
		{
		}

		protected ReplayServiceSuspendWantedSetException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
