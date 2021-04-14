using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum FolderMappingFlags
	{
		None = 0,
		Include = 1,
		Exclude = 2,
		Inherit = 4,
		Root = 8,
		InheritedInclude = 16,
		InheritedExclude = 32,
		InclusionFlags = 3,
		InheritanceFlags = 4
	}
}
