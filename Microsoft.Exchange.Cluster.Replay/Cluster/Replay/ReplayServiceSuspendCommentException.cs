using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayServiceSuspendCommentException : TaskServerException
	{
		public ReplayServiceSuspendCommentException() : base(ReplayStrings.ReplayServiceSuspendCommentException)
		{
		}

		public ReplayServiceSuspendCommentException(Exception innerException) : base(ReplayStrings.ReplayServiceSuspendCommentException, innerException)
		{
		}

		protected ReplayServiceSuspendCommentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
