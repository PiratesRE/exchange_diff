using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum EmptyFolderFlags
	{
		None = 0,
		DeleteAssociatedMessages = 1,
		Force = 2,
		HardDelete = 4
	}
}
