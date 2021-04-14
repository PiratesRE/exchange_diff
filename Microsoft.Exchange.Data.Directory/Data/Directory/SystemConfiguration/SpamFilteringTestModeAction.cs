using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum SpamFilteringTestModeAction
	{
		[LocDescription(DirectoryStrings.IDs.SpamFilteringTestActionNone)]
		None,
		[LocDescription(DirectoryStrings.IDs.SpamFilteringTestActionAddXHeader)]
		AddXHeader,
		[LocDescription(DirectoryStrings.IDs.SpamFilteringTestActionBccMessage)]
		BccMessage
	}
}
