using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MailboxLoadBalanceTransientException : AnchorTransientException
	{
		public MailboxLoadBalanceTransientException(LocalizedString message) : base(message)
		{
		}

		public MailboxLoadBalanceTransientException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected MailboxLoadBalanceTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
