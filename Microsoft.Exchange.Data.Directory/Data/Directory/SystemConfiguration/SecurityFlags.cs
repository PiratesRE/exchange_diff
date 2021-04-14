using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum SecurityFlags
	{
		RequireTLS = 4,
		AuthSpNego = 128,
		AuthLogin = 256,
		AuthKerberos = 2097152,
		ForceHELO = 4194304,
		IgnoreSTARTTLS = 8388608
	}
}
