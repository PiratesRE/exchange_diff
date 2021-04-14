using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToDeleteMoveRequestMessagePermanentException : MailboxReplicationPermanentException
	{
		public UnableToDeleteMoveRequestMessagePermanentException() : base(MrsStrings.UnableToDeleteMoveRequestMessage)
		{
		}

		public UnableToDeleteMoveRequestMessagePermanentException(Exception innerException) : base(MrsStrings.UnableToDeleteMoveRequestMessage, innerException)
		{
		}

		protected UnableToDeleteMoveRequestMessagePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
