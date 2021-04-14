using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum CopyFolderFlags
	{
		None = 0,
		FolderMove = 1,
		CopySubfolders = 16,
		UnicodeStrings = -2147483648,
		SendEntryId = 32
	}
}
