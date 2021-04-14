using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	public enum MessageRateSourceFlags
	{
		[LocDescription(DirectoryStrings.IDs.MessageRateSourceFlagsNone)]
		None = 0,
		[LocDescription(DirectoryStrings.IDs.MessageRateSourceFlagsIPAddress)]
		IPAddress = 1,
		[LocDescription(DirectoryStrings.IDs.MessageRateSourceFlagsUser)]
		User = 2,
		[LocDescription(DirectoryStrings.IDs.MessageRateSourceFlagsAll)]
		All = 3
	}
}
