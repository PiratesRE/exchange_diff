using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SuspendWantedWriteFailedException : TransientException
	{
		public SuspendWantedWriteFailedException() : base(ReplayStrings.SuspendWantedWriteFailedException)
		{
		}

		public SuspendWantedWriteFailedException(Exception innerException) : base(ReplayStrings.SuspendWantedWriteFailedException, innerException)
		{
		}

		protected SuspendWantedWriteFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
