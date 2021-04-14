using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum CmdletExtensionFlagValues
	{
		None = 0,
		Enabled = 1,
		System = 16
	}
}
