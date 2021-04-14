using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NotEnoughInformationToFindMoveRequestPermanentException : MailboxReplicationPermanentException
	{
		public NotEnoughInformationToFindMoveRequestPermanentException() : base(MrsStrings.NotEnoughInformationToFindMoveRequest)
		{
		}

		public NotEnoughInformationToFindMoveRequestPermanentException(Exception innerException) : base(MrsStrings.NotEnoughInformationToFindMoveRequest, innerException)
		{
		}

		protected NotEnoughInformationToFindMoveRequestPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
