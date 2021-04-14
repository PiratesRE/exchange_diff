using System;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[Flags]
	internal enum DocumentLibraryQueryOptions
	{
		Folders = 1,
		Files = 2,
		FoldersAndFiles = 3
	}
}
