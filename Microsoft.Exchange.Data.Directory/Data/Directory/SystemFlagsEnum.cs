using System;

namespace Microsoft.Exchange.Data.Directory
{
	[Flags]
	internal enum SystemFlagsEnum
	{
		None = 0,
		NotReplicate = 1,
		NtdsNamingContext = 1,
		Replicate = 2,
		NtdsDomain = 2,
		ConstructAttribute = 4,
		Category1 = 16,
		DeleteImmediately = 33554432,
		Unmovable = 67108864,
		Unrenameable = 134217728,
		MovableWithRestrictions = 268435456,
		Movable = 536870912,
		Renamable = 1073741824,
		Indispensable = -2147483648
	}
}
