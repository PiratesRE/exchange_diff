using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.PublicFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IFavoritePublicFoldersManager
	{
		IEnumerable<IFavoritePublicFolder> EnumerateCalendarFolders();

		IEnumerable<IFavoritePublicFolder> EnumerateContactsFolders();

		IEnumerable<IFavoritePublicFolder> EnumerateMailAndPostsFolders();
	}
}
