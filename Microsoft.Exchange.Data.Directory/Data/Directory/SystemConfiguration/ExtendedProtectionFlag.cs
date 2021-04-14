using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	public enum ExtendedProtectionFlag
	{
		None = 0,
		Proxy = 1,
		NoServiceNameCheck = 2,
		AllowDotlessSpn = 4,
		ProxyCohosting = 32
	}
}
