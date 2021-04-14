using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum DialPlanFlagBits2
	{
		[LocDescription(DirectoryStrings.IDs.None)]
		None = 0,
		[LocDescription(DirectoryStrings.IDs.PAAEnabled)]
		PAAEnabled = 2,
		[LocDescription(DirectoryStrings.IDs.SipResourceIdRequired)]
		SipResourceIdRequired = 4,
		All = -1
	}
}
