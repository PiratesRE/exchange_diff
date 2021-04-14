using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidProxyOperationOrderPermanentException : MailboxReplicationPermanentException
	{
		public InvalidProxyOperationOrderPermanentException() : base(MrsStrings.InvalidProxyOperationOrder)
		{
		}

		public InvalidProxyOperationOrderPermanentException(Exception innerException) : base(MrsStrings.InvalidProxyOperationOrder, innerException)
		{
		}

		protected InvalidProxyOperationOrderPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
