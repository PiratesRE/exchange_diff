using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OfflinePublicFolderMigrationNotSupportedException : MailboxReplicationPermanentException
	{
		public OfflinePublicFolderMigrationNotSupportedException() : base(MrsStrings.OfflinePublicFolderMigrationNotSupported)
		{
		}

		public OfflinePublicFolderMigrationNotSupportedException(Exception innerException) : base(MrsStrings.OfflinePublicFolderMigrationNotSupported, innerException)
		{
		}

		protected OfflinePublicFolderMigrationNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
