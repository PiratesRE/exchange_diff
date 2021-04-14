using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RehomeRequestTransientException : MailboxReplicationTransientException
	{
		public RehomeRequestTransientException() : base(MrsStrings.RehomeRequestFailure)
		{
		}

		public RehomeRequestTransientException(Exception innerException) : base(MrsStrings.RehomeRequestFailure, innerException)
		{
		}

		protected RehomeRequestTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
