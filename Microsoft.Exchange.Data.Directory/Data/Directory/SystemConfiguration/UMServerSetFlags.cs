using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum UMServerSetFlags
	{
		IPAddressFamilyConfigurable = 1,
		IPv4Enabled = 2,
		IPv6Enabled = 4,
		Default = 7
	}
}
