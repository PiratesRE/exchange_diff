using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class PublicFolderMailboxesNotProvisionedException : MigrationPermanentException
	{
		public PublicFolderMailboxesNotProvisionedException() : base(Strings.PublicFolderMailboxesNotProvisionedError)
		{
		}

		public PublicFolderMailboxesNotProvisionedException(Exception innerException) : base(Strings.PublicFolderMailboxesNotProvisionedError, innerException)
		{
		}

		protected PublicFolderMailboxesNotProvisionedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
