using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class RemoteMailboxLoadBalanceTransientException : MailboxLoadBalanceTransientException
	{
		public RemoteMailboxLoadBalanceTransientException(LocalizedString message) : base(message)
		{
		}

		public RemoteMailboxLoadBalanceTransientException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected RemoteMailboxLoadBalanceTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
