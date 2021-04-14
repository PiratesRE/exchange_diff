using System;

namespace Microsoft.Exchange.Security
{
	[Flags]
	public enum ExtendedProtectionPolicySetting
	{
		None = 0,
		Allow = 1,
		Require = 2
	}
}
