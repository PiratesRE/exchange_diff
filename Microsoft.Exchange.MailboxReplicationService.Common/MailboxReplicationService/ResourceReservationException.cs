using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ResourceReservationException : MailboxReplicationTransientException
	{
		public ResourceReservationException(LocalizedString message) : base(message)
		{
		}

		public ResourceReservationException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected ResourceReservationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
