using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RecoverySyncNotImplementedException : MailboxReplicationPermanentException
	{
		public RecoverySyncNotImplementedException() : base(MrsStrings.RecoverySyncNotImplemented)
		{
		}

		public RecoverySyncNotImplementedException(Exception innerException) : base(MrsStrings.RecoverySyncNotImplemented, innerException)
		{
		}

		protected RecoverySyncNotImplementedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
