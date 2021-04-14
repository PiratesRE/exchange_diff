using System;

namespace Microsoft.Exchange.Data.Storage.Approval
{
	internal enum DecisionConflict
	{
		NoConflict,
		DifferentApproverDifferentDecision,
		DifferentApproverSameDecision,
		SameApproverAndDecision,
		SameApproverDifferentDecision,
		HasApproverMissingDecision,
		AlreadyCancelled,
		AlreadyExpired,
		Unauthorized,
		MissingItem,
		Unknown
	}
}
