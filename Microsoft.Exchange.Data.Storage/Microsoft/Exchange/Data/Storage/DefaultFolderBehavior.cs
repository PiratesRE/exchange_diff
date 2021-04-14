using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum DefaultFolderBehavior
	{
		None = 0,
		CanCreate = 1,
		CreateIfMissing = 3,
		CanNotRename = 4,
		AlwaysDeferInitialization = 32,
		CanHideFolderFromOutlook = 64,
		RefreshIfMissing = 128
	}
}
