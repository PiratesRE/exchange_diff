using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ActionNotSupportedException : MailboxReplicationPermanentException
	{
		public ActionNotSupportedException() : base(MrsStrings.ActionNotSupported)
		{
		}

		public ActionNotSupportedException(Exception innerException) : base(MrsStrings.ActionNotSupported, innerException)
		{
		}

		protected ActionNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
