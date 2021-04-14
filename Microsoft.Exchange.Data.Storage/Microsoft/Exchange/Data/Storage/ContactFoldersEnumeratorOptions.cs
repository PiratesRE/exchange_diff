using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum ContactFoldersEnumeratorOptions
	{
		None = 0,
		SkipHiddenFolders = 1,
		SkipDeletedFolders = 2,
		IncludeParentFolder = 4
	}
}
