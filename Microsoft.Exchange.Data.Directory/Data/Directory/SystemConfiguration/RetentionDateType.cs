using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum RetentionDateType
	{
		[LocDescription(DirectoryStrings.IDs.WhenDelivered)]
		WhenDelivered,
		[LocDescription(DirectoryStrings.IDs.WhenMoved)]
		WhenMoved
	}
}
