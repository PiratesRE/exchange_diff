using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SourceMailboxConnectionStalePermanentException : MailboxReplicationPermanentException
	{
		public SourceMailboxConnectionStalePermanentException() : base(MrsStrings.CouldNotConnectToSourceMailbox)
		{
		}

		public SourceMailboxConnectionStalePermanentException(Exception innerException) : base(MrsStrings.CouldNotConnectToSourceMailbox, innerException)
		{
		}

		protected SourceMailboxConnectionStalePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
