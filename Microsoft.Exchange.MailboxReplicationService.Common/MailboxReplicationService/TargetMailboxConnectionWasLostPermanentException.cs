using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TargetMailboxConnectionWasLostPermanentException : MailboxReplicationPermanentException
	{
		public TargetMailboxConnectionWasLostPermanentException() : base(MrsStrings.TargetMailboxConnectionWasLost)
		{
		}

		public TargetMailboxConnectionWasLostPermanentException(Exception innerException) : base(MrsStrings.TargetMailboxConnectionWasLost, innerException)
		{
		}

		protected TargetMailboxConnectionWasLostPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
