using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class PublishedFolderAccessDeniedException : StoragePermanentException
	{
		public PublishedFolderAccessDeniedException() : base(ServerStrings.PublishedFolderAccessDeniedException)
		{
		}

		public PublishedFolderAccessDeniedException(Exception innerException) : base(ServerStrings.PublishedFolderAccessDeniedException, innerException)
		{
		}

		protected PublishedFolderAccessDeniedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
