using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	public enum ExtendedProtectionPolicySetting
	{
		[LocDescription(DirectoryStrings.IDs.ReceiveExtendedProtectionPolicyNone)]
		None = 0,
		[LocDescription(DirectoryStrings.IDs.ReceiveExtendedProtectionPolicyAllow)]
		Allow = 1,
		[LocDescription(DirectoryStrings.IDs.ReceiveExtendedProtectionPolicyRequire)]
		Require = 2
	}
}
