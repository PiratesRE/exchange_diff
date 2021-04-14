using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MultiplePublicFolderMigrationTypesNotAllowedException : MailboxReplicationPermanentException
	{
		public MultiplePublicFolderMigrationTypesNotAllowedException() : base(Strings.ErrorAnotherPublicFolderMigrationTypeAlreadyInProgress)
		{
		}

		public MultiplePublicFolderMigrationTypesNotAllowedException(Exception innerException) : base(Strings.ErrorAnotherPublicFolderMigrationTypeAlreadyInProgress, innerException)
		{
		}

		protected MultiplePublicFolderMigrationTypesNotAllowedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
