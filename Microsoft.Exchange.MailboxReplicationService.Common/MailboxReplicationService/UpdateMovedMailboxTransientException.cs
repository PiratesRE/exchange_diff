using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UpdateMovedMailboxTransientException : MailboxReplicationTransientException
	{
		public UpdateMovedMailboxTransientException() : base(MrsStrings.ErrorWhileUpdatingMovedMailbox)
		{
		}

		public UpdateMovedMailboxTransientException(Exception innerException) : base(MrsStrings.ErrorWhileUpdatingMovedMailbox, innerException)
		{
		}

		protected UpdateMovedMailboxTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
