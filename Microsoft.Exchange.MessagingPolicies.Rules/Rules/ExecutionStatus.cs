using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal enum ExecutionStatus
	{
		Success,
		SuccessMailItemDeleted,
		SuccessMailItemDeferred,
		TransientError,
		PermanentError
	}
}
