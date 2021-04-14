using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxSignatureChangedTransientException : MailboxReplicationTransientException
	{
		public MailboxSignatureChangedTransientException() : base(MrsStrings.MoveRestartedDueToSignatureChange)
		{
		}

		public MailboxSignatureChangedTransientException(Exception innerException) : base(MrsStrings.MoveRestartedDueToSignatureChange, innerException)
		{
		}

		protected MailboxSignatureChangedTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
