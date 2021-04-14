using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotPreventCompletionForCompletingMovePermanentException : MailboxReplicationPermanentException
	{
		public CannotPreventCompletionForCompletingMovePermanentException() : base(MrsStrings.ErrorCannotPreventCompletionForCompletingMove)
		{
		}

		public CannotPreventCompletionForCompletingMovePermanentException(Exception innerException) : base(MrsStrings.ErrorCannotPreventCompletionForCompletingMove, innerException)
		{
		}

		protected CannotPreventCompletionForCompletingMovePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
