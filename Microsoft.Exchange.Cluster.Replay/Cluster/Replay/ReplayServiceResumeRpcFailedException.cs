using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayServiceResumeRpcFailedException : TaskServerException
	{
		public ReplayServiceResumeRpcFailedException() : base(ReplayStrings.ReplayServiceResumeRpcFailedException)
		{
		}

		public ReplayServiceResumeRpcFailedException(Exception innerException) : base(ReplayStrings.ReplayServiceResumeRpcFailedException, innerException)
		{
		}

		protected ReplayServiceResumeRpcFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
