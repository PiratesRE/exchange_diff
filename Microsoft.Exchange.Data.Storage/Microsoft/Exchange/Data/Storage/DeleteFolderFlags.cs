using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum DeleteFolderFlags
	{
		None = 0,
		DeleteMessages = 1,
		DeleteSubFolders = 4,
		HardDelete = 16
	}
}
