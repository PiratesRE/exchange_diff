using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum EmptyFolderFlags
	{
		None = 0,
		DeleteAssociatedMessages = 8,
		ForceHardDelete = 16,
		Force = 32
	}
}
