using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class FolderNoCreator : DefaultFolderCreator
	{
		internal FolderNoCreator() : base(DefaultFolderType.None, StoreObjectType.Folder, true)
		{
		}

		internal override Folder Create(DefaultFolderContext context, string folderName, out bool hasCreatedNew)
		{
			throw new NotSupportedException("The defaultFolder does not support Create.");
		}

		internal override AggregateOperationResult Delete(DefaultFolderContext context, DeleteItemFlags deleteItemFlags, StoreObjectId id)
		{
			throw new NotSupportedException("The defaultFolder does not support deletion.");
		}
	}
}
