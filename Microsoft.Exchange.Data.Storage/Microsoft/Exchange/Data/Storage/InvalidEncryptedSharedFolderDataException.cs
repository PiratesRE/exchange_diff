using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidEncryptedSharedFolderDataException : StoragePermanentException
	{
		public InvalidEncryptedSharedFolderDataException() : base(ServerStrings.InvalidEncryptedSharedFolderDataException)
		{
		}

		public InvalidEncryptedSharedFolderDataException(Exception innerException) : base(ServerStrings.InvalidEncryptedSharedFolderDataException, innerException)
		{
		}

		protected InvalidEncryptedSharedFolderDataException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
