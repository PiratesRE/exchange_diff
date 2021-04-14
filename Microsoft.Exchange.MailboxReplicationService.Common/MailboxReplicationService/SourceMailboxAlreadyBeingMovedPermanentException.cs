using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SourceMailboxAlreadyBeingMovedPermanentException : MailboxReplicationPermanentException
	{
		public SourceMailboxAlreadyBeingMovedPermanentException() : base(MrsStrings.SourceMailboxAlreadyBeingMoved)
		{
		}

		public SourceMailboxAlreadyBeingMovedPermanentException(Exception innerException) : base(MrsStrings.SourceMailboxAlreadyBeingMoved, innerException)
		{
		}

		protected SourceMailboxAlreadyBeingMovedPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
