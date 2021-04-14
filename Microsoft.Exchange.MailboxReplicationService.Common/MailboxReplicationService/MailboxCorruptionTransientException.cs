using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxCorruptionTransientException : MailboxReplicationTransientException
	{
		public MailboxCorruptionTransientException() : base(MrsStrings.MoveRestartDueToIsIntegCheck)
		{
		}

		public MailboxCorruptionTransientException(Exception innerException) : base(MrsStrings.MoveRestartDueToIsIntegCheck, innerException)
		{
		}

		protected MailboxCorruptionTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
