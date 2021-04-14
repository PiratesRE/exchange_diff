using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TooManyMissingItemsPermanentException : MailboxReplicationPermanentException
	{
		public TooManyMissingItemsPermanentException() : base(MrsStrings.TooManyMissingItems)
		{
		}

		public TooManyMissingItemsPermanentException(Exception innerException) : base(MrsStrings.TooManyMissingItems, innerException)
		{
		}

		protected TooManyMissingItemsPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
