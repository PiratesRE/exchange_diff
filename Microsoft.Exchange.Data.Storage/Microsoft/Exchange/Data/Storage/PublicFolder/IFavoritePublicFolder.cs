using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.PublicFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IFavoritePublicFolder
	{
		StoreObjectId FolderId { get; }

		string DisplayName { get; }
	}
}
