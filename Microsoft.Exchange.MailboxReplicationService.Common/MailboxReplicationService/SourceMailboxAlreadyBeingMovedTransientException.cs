using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SourceMailboxAlreadyBeingMovedTransientException : MailboxReplicationTransientException
	{
		public SourceMailboxAlreadyBeingMovedTransientException() : base(MrsStrings.SourceMailboxAlreadyBeingMoved)
		{
		}

		public SourceMailboxAlreadyBeingMovedTransientException(Exception innerException) : base(MrsStrings.SourceMailboxAlreadyBeingMoved, innerException)
		{
		}

		protected SourceMailboxAlreadyBeingMovedTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
