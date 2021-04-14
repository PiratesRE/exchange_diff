using System;

namespace Microsoft.Exchange.AirSync
{
	[Flags]
	internal enum ProviderSyncType
	{
		None = 0,
		N = 1,
		IQ = 2,
		ICS = 4,
		FCS = 8
	}
}
