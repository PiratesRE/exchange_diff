using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum ServerAutoDagFlags
	{
		None = 0,
		DatabaseCopyLocationAgilityDisabled = 1,
		ActivationDisabled = 2,
		ServerConfigured = 4
	}
}
