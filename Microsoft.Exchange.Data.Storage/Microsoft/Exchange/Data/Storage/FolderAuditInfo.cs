using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FolderAuditInfo
	{
		public StoreObjectId Id { get; private set; }

		public string PathName { get; private set; }

		public FolderAuditInfo(StoreObjectId folderId, string pathName)
		{
			Util.ThrowOnNullArgument(folderId, "folderId");
			this.Id = folderId;
			this.PathName = pathName;
		}
	}
}
