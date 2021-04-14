using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayServiceRpcUnknownInstanceException : TaskServerException
	{
		public ReplayServiceRpcUnknownInstanceException() : base(ReplayStrings.ReplayServiceRpcUnknownInstanceException)
		{
		}

		public ReplayServiceRpcUnknownInstanceException(Exception innerException) : base(ReplayStrings.ReplayServiceRpcUnknownInstanceException, innerException)
		{
		}

		protected ReplayServiceRpcUnknownInstanceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
