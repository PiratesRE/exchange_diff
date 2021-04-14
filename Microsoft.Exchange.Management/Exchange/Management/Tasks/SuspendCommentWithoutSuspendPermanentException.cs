using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SuspendCommentWithoutSuspendPermanentException : MailboxReplicationPermanentException
	{
		public SuspendCommentWithoutSuspendPermanentException() : base(Strings.ErrorCannotSpecifySuspendCommentWithoutSuspend)
		{
		}

		public SuspendCommentWithoutSuspendPermanentException(Exception innerException) : base(Strings.ErrorCannotSpecifySuspendCommentWithoutSuspend, innerException)
		{
		}

		protected SuspendCommentWithoutSuspendPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
