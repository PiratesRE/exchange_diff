using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayServiceSuspendRpcFailedException : TaskServerException
	{
		public ReplayServiceSuspendRpcFailedException() : base(ReplayStrings.ReplayServiceSuspendRpcFailedException)
		{
		}

		public ReplayServiceSuspendRpcFailedException(Exception innerException) : base(ReplayStrings.ReplayServiceSuspendRpcFailedException, innerException)
		{
		}

		protected ReplayServiceSuspendRpcFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
