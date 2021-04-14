using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum EnumHierarchyFlags
	{
		None = 0,
		NormalFolders = 1,
		SearchFolders = 2,
		RootFolder = 4,
		AllFolders = 7
	}
}
