using System;

namespace Microsoft.Exchange.Data.Storage.UnifiedPolicy
{
	internal enum PolicyLoadStatus
	{
		Unknown,
		NotExist,
		FailedToLoad,
		Loaded,
		Count
	}
}
