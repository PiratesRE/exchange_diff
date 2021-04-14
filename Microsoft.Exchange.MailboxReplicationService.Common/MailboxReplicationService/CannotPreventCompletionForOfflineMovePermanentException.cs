using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotPreventCompletionForOfflineMovePermanentException : MailboxReplicationPermanentException
	{
		public CannotPreventCompletionForOfflineMovePermanentException() : base(MrsStrings.ErrorCannotPreventCompletionForOfflineMove)
		{
		}

		public CannotPreventCompletionForOfflineMovePermanentException(Exception innerException) : base(MrsStrings.ErrorCannotPreventCompletionForOfflineMove, innerException)
		{
		}

		protected CannotPreventCompletionForOfflineMovePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
