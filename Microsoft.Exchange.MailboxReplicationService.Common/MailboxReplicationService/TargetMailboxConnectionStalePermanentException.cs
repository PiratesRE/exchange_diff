using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TargetMailboxConnectionStalePermanentException : MailboxReplicationPermanentException
	{
		public TargetMailboxConnectionStalePermanentException() : base(MrsStrings.CouldNotConnectToTargetMailbox)
		{
		}

		public TargetMailboxConnectionStalePermanentException(Exception innerException) : base(MrsStrings.CouldNotConnectToTargetMailbox, innerException)
		{
		}

		protected TargetMailboxConnectionStalePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
