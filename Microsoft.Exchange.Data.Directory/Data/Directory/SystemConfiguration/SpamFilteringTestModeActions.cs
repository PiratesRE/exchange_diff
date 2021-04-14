using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	public enum SpamFilteringTestModeActions
	{
		[LocDescription(DirectoryStrings.IDs.SpamFilteringTestActionNone)]
		None = 0,
		[LocDescription(DirectoryStrings.IDs.SpamFilteringTestActionAddXHeader)]
		AddXHeader = 1,
		[LocDescription(DirectoryStrings.IDs.SpamFilteringTestActionBccMessage)]
		BccMessage = 2
	}
}
