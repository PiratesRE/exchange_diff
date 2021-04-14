using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class FolderCreationBlockedException : StoragePermanentException
	{
		public FolderCreationBlockedException() : base(ServerStrings.ErrorFolderCreationIsBlocked)
		{
		}

		public FolderCreationBlockedException(Exception innerException) : base(ServerStrings.ErrorFolderCreationIsBlocked, innerException)
		{
		}

		protected FolderCreationBlockedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
