using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ArchiveDisabledPermanentException : MailboxReplicationPermanentException
	{
		public ArchiveDisabledPermanentException() : base(Strings.ArchiveDisabledError)
		{
		}

		public ArchiveDisabledPermanentException(Exception innerException) : base(Strings.ArchiveDisabledError, innerException)
		{
		}

		protected ArchiveDisabledPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
