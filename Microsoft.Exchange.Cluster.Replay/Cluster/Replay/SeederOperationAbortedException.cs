using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SeederOperationAbortedException : SeederServerException
	{
		public SeederOperationAbortedException() : base(ReplayStrings.SeederOperationAborted)
		{
		}

		public SeederOperationAbortedException(Exception innerException) : base(ReplayStrings.SeederOperationAborted, innerException)
		{
		}

		protected SeederOperationAbortedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
