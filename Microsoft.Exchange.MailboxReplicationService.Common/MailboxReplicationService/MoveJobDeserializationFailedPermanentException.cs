using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MoveJobDeserializationFailedPermanentException : MailboxReplicationPermanentException
	{
		public MoveJobDeserializationFailedPermanentException() : base(MrsStrings.MoveJobDeserializationFailed)
		{
		}

		public MoveJobDeserializationFailedPermanentException(Exception innerException) : base(MrsStrings.MoveJobDeserializationFailed, innerException)
		{
		}

		protected MoveJobDeserializationFailedPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
