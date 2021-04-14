using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum MailTipsAccessLevel
	{
		[LocDescription(DirectoryStrings.IDs.MailTipsAccessLevelNone)]
		None,
		[LocDescription(DirectoryStrings.IDs.MailTipsAccessLevelLimited)]
		Limited,
		[LocDescription(DirectoryStrings.IDs.MailTipsAccessLevelAll)]
		All
	}
}
