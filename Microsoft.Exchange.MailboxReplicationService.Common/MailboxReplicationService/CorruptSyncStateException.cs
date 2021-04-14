using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CorruptSyncStateException : MailboxReplicationPermanentException
	{
		public CorruptSyncStateException() : base(MrsStrings.CorruptSyncState)
		{
		}

		public CorruptSyncStateException(Exception innerException) : base(MrsStrings.CorruptSyncState, innerException)
		{
		}

		protected CorruptSyncStateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
