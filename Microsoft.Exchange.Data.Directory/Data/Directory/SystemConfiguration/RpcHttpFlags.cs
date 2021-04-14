using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum RpcHttpFlags
	{
		None = 0,
		SSLOffloading = 1
	}
}
