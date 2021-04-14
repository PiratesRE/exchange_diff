using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MRSProxyConnectionLimitReachedTransientException : MailboxReplicationTransientException
	{
		public MRSProxyConnectionLimitReachedTransientException(LocalizedString message) : base(message)
		{
		}

		public MRSProxyConnectionLimitReachedTransientException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected MRSProxyConnectionLimitReachedTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
