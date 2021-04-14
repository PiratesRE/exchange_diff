using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DestMailboxAlreadyBeingMovedTransientException : MailboxReplicationTransientException
	{
		public DestMailboxAlreadyBeingMovedTransientException() : base(MrsStrings.DestMailboxAlreadyBeingMoved)
		{
		}

		public DestMailboxAlreadyBeingMovedTransientException(Exception innerException) : base(MrsStrings.DestMailboxAlreadyBeingMoved, innerException)
		{
		}

		protected DestMailboxAlreadyBeingMovedTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
