using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExpiredReservationException : ResourceReservationException
	{
		public ExpiredReservationException() : base(MrsStrings.ErrorReservationExpired)
		{
		}

		public ExpiredReservationException(Exception innerException) : base(MrsStrings.ErrorReservationExpired, innerException)
		{
		}

		protected ExpiredReservationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
