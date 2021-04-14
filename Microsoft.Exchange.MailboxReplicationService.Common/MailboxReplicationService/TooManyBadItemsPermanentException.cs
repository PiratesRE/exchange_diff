using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TooManyBadItemsPermanentException : MailboxReplicationPermanentException
	{
		public TooManyBadItemsPermanentException() : base(MrsStrings.TooManyBadItems)
		{
		}

		public TooManyBadItemsPermanentException(Exception innerException) : base(MrsStrings.TooManyBadItems, innerException)
		{
		}

		protected TooManyBadItemsPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
