using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TooManyLargeItemsPermanentException : MailboxReplicationPermanentException
	{
		public TooManyLargeItemsPermanentException() : base(MrsStrings.TooManyLargeItems)
		{
		}

		public TooManyLargeItemsPermanentException(Exception innerException) : base(MrsStrings.TooManyLargeItems, innerException)
		{
		}

		protected TooManyLargeItemsPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
