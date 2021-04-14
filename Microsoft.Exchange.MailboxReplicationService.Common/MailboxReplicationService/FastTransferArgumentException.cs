using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FastTransferArgumentException : MailboxReplicationPermanentException
	{
		public FastTransferArgumentException() : base(MrsStrings.FastTransferArgumentError)
		{
		}

		public FastTransferArgumentException(Exception innerException) : base(MrsStrings.FastTransferArgumentError, innerException)
		{
		}

		protected FastTransferArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
