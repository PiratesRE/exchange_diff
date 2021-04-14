using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum FolderTreeDataSection
	{
		None,
		First,
		Favorites = 1,
		Mail,
		Calendar,
		Contacts,
		Tasks,
		Notes,
		Journal,
		FolderTreeAll,
		Today,
		Max
	}
}
