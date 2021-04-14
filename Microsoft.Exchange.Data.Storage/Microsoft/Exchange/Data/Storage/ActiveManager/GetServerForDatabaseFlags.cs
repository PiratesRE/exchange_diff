using System;

namespace Microsoft.Exchange.Data.Storage.ActiveManager
{
	[Flags]
	internal enum GetServerForDatabaseFlags
	{
		None = 0,
		ThrowServerForDatabaseNotFoundException = 1,
		IgnoreAdSiteBoundary = 2,
		ReadThrough = 4,
		BasicQuery = 8
	}
}
