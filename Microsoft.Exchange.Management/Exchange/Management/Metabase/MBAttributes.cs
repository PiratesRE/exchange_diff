using System;

namespace Microsoft.Exchange.Management.Metabase
{
	[Flags]
	internal enum MBAttributes
	{
		None = 0,
		Inherit = 1,
		InsertPath = 64,
		IsInherited = 32,
		PartialPath = 2,
		Reference = 8,
		Secure = 4,
		Volatile = 16,
		StoreAsExpandSz = 4096,
		ReadOnly = 8192
	}
}
