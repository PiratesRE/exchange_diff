using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OscFolderCreateResult
	{
		public OscFolderCreateResult(StoreObjectId folderId, bool created)
		{
			ArgumentValidator.ThrowIfNull("folderId", folderId);
			this.FolderId = folderId;
			this.Created = created;
		}

		public StoreObjectId FolderId { get; private set; }

		public bool Created { get; private set; }

		public override string ToString()
		{
			return string.Format("{{ FolderId: {0};  Created: {1} }}", this.FolderId, this.Created);
		}
	}
}
