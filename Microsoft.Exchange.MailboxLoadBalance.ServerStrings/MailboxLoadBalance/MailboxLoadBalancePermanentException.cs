using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MailboxLoadBalancePermanentException : AnchorPermanentException
	{
		public MailboxLoadBalancePermanentException(LocalizedString message) : base(message)
		{
		}

		public MailboxLoadBalancePermanentException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected MailboxLoadBalancePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
