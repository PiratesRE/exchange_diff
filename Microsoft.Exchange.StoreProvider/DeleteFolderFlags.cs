using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum DeleteFolderFlags
	{
		None = 0,
		DeleteMessages = 1,
		DelSubFolders = 4,
		ForceHardDelete = 16
	}
}
