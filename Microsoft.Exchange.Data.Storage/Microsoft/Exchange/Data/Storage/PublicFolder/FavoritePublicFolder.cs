using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.PublicFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FavoritePublicFolder : IFavoritePublicFolder
	{
		public FavoritePublicFolder(StoreObjectId folderId, string displayName)
		{
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			if (displayName == null)
			{
				throw new ArgumentNullException("displayName");
			}
			this.FolderId = folderId;
			this.DisplayName = displayName;
		}

		public StoreObjectId FolderId { get; private set; }

		public string DisplayName { get; private set; }

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				base.GetType().Name,
				": FolderId=",
				this.FolderId.ToBase64String(),
				", DisplayName=",
				this.DisplayName
			});
		}
	}
}
