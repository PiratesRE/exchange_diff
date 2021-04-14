using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DocumentLibraryInfo
	{
		public StoreObjectId FolderId { get; private set; }

		public Guid SharepointId { get; private set; }

		public string SharepointUrl { get; private set; }

		public DocumentLibraryInfo(StoreObjectId folderId, Guid sharepointId, string sharepointUrl)
		{
			this.FolderId = folderId;
			this.SharepointId = sharepointId;
			this.SharepointUrl = sharepointUrl;
		}
	}
}
