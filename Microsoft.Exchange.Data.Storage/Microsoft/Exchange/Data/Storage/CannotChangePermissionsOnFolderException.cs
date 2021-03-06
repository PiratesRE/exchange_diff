using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CannotChangePermissionsOnFolderException : StoragePermanentException
	{
		public CannotChangePermissionsOnFolderException() : base(ServerStrings.CannotChangePermissionsOnFolder)
		{
		}

		public CannotChangePermissionsOnFolderException(Exception innerException) : base(ServerStrings.CannotChangePermissionsOnFolder, innerException)
		{
		}

		protected CannotChangePermissionsOnFolderException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
