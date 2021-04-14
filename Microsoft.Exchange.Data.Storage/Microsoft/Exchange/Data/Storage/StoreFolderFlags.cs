using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum StoreFolderFlags
	{
		FolderIPM = 1,
		FolderSearch = 2,
		FolderNormal = 4,
		FolderRules = 8
	}
}
