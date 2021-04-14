using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PublicFolderMailboxesNotProvisionedForMigrationException : MailboxReplicationPermanentException
	{
		public PublicFolderMailboxesNotProvisionedForMigrationException() : base(MrsStrings.PublicFolderMailboxesNotProvisionedForMigration)
		{
		}

		public PublicFolderMailboxesNotProvisionedForMigrationException(Exception innerException) : base(MrsStrings.PublicFolderMailboxesNotProvisionedForMigration, innerException)
		{
		}

		protected PublicFolderMailboxesNotProvisionedForMigrationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
