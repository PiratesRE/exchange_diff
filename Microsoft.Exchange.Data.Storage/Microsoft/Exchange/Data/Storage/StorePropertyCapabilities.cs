using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	[Serializable]
	internal enum StorePropertyCapabilities
	{
		None = 0,
		CanQuery = 1,
		CanSortBy = 2,
		CanGroupBy = 4,
		All = 7
	}
}
