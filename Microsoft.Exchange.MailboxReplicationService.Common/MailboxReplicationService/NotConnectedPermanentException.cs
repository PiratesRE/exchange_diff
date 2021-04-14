using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NotConnectedPermanentException : MailboxReplicationPermanentException
	{
		public NotConnectedPermanentException() : base(MrsStrings.NotConnected)
		{
		}

		public NotConnectedPermanentException(Exception innerException) : base(MrsStrings.NotConnected, innerException)
		{
		}

		protected NotConnectedPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
