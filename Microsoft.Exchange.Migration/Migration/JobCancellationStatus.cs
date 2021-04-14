using System;

namespace Microsoft.Exchange.Migration
{
	internal enum JobCancellationStatus
	{
		NotCancelled,
		CancelledByUserRequest,
		CancelledDueToHighFailureCount
	}
}
