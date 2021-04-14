using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FinalizationIsBlockedPermanentException : MailboxReplicationPermanentException
	{
		public FinalizationIsBlockedPermanentException() : base(MrsStrings.ErrorFinalizationIsBlocked)
		{
		}

		public FinalizationIsBlockedPermanentException(Exception innerException) : base(MrsStrings.ErrorFinalizationIsBlocked, innerException)
		{
		}

		protected FinalizationIsBlockedPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
