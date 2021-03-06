using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SyncProtocolNotSpecifiedPermanentException : MailboxReplicationPermanentException
	{
		public SyncProtocolNotSpecifiedPermanentException() : base(Strings.ErrorSyncProtocolNotSpecified)
		{
		}

		public SyncProtocolNotSpecifiedPermanentException(Exception innerException) : base(Strings.ErrorSyncProtocolNotSpecified, innerException)
		{
		}

		protected SyncProtocolNotSpecifiedPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
