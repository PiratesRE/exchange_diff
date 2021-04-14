using System;

namespace Microsoft.Exchange.Data.Storage.OfflineRms
{
	internal enum OverallRpcStatus : uint
	{
		Success,
		TransientFailure,
		PermanentFailure
	}
}
