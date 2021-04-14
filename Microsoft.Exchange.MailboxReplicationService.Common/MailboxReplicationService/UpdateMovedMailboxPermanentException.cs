using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UpdateMovedMailboxPermanentException : MailboxReplicationPermanentException
	{
		public UpdateMovedMailboxPermanentException() : base(MrsStrings.ErrorWhileUpdatingMovedMailbox)
		{
		}

		public UpdateMovedMailboxPermanentException(Exception innerException) : base(MrsStrings.ErrorWhileUpdatingMovedMailbox, innerException)
		{
		}

		protected UpdateMovedMailboxPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
