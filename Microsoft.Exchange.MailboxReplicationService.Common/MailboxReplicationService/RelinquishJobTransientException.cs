using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RelinquishJobTransientException : MailboxReplicationTransientException
	{
		public RelinquishJobTransientException(LocalizedString message) : base(message)
		{
		}

		public RelinquishJobTransientException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected RelinquishJobTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
