using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum InfoAnnouncementEnabledEnum
	{
		[LocDescription(DirectoryStrings.IDs.True)]
		True,
		[LocDescription(DirectoryStrings.IDs.False)]
		False,
		[LocDescription(DirectoryStrings.IDs.Uninterruptible)]
		Uninterruptible
	}
}
