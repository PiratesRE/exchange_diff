using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RehomeRequestPermanentException : MailboxReplicationPermanentException
	{
		public RehomeRequestPermanentException() : base(MrsStrings.RehomeRequestFailure)
		{
		}

		public RehomeRequestPermanentException(Exception innerException) : base(MrsStrings.RehomeRequestFailure, innerException)
		{
		}

		protected RehomeRequestPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
