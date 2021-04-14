using System;

namespace System.Security.Util
{
	[Flags]
	[Serializable]
	internal enum QuickCacheEntryType
	{
		FullTrustZoneMyComputer = 16777216,
		FullTrustZoneIntranet = 33554432,
		FullTrustZoneInternet = 67108864,
		FullTrustZoneTrusted = 134217728,
		FullTrustZoneUntrusted = 268435456,
		FullTrustAll = 536870912
	}
}
