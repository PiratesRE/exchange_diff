using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class FolderNotPublishedException : StoragePermanentException
	{
		public FolderNotPublishedException() : base(ServerStrings.FolderNotPublishedException)
		{
		}

		public FolderNotPublishedException(Exception innerException) : base(ServerStrings.FolderNotPublishedException, innerException)
		{
		}

		protected FolderNotPublishedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
