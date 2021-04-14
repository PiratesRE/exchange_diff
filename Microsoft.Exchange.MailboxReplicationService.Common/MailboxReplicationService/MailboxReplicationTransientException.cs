using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxReplicationTransientException : TransientException
	{
		public MailboxReplicationTransientException(LocalizedString message) : base(message)
		{
		}

		public MailboxReplicationTransientException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected MailboxReplicationTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
