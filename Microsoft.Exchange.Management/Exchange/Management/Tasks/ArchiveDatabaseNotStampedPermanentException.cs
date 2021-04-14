using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ArchiveDatabaseNotStampedPermanentException : MailboxReplicationPermanentException
	{
		public ArchiveDatabaseNotStampedPermanentException() : base(Strings.ArchiveDatabaseNotExplicitlyStampedError)
		{
		}

		public ArchiveDatabaseNotStampedPermanentException(Exception innerException) : base(Strings.ArchiveDatabaseNotExplicitlyStampedError, innerException)
		{
		}

		protected ArchiveDatabaseNotStampedPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
