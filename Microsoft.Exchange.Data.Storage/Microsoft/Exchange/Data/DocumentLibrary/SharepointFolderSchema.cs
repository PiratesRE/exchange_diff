using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SharepointFolderSchema : SharepointDocumentLibraryItemSchema
	{
		public new static SharepointFolderSchema Instance
		{
			get
			{
				if (SharepointFolderSchema.instance == null)
				{
					SharepointFolderSchema.instance = new SharepointFolderSchema();
				}
				return SharepointFolderSchema.instance;
			}
		}

		private static SharepointFolderSchema instance;
	}
}
