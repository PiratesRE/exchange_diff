using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TooManyCleanupRetriesPermanentException : MailboxReplicationPermanentException
	{
		public TooManyCleanupRetriesPermanentException() : base(MrsStrings.ErrorTooManyCleanupRetries)
		{
		}

		public TooManyCleanupRetriesPermanentException(Exception innerException) : base(MrsStrings.ErrorTooManyCleanupRetries, innerException)
		{
		}

		protected TooManyCleanupRetriesPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
