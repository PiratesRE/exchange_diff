using System;

namespace Microsoft.Exchange.Data.Storage.Approval
{
	[Flags]
	internal enum ApprovalStatus
	{
		Unhandled = 1,
		Cancelled = 2,
		Ndred = 4,
		Expired = 8,
		Approved = 16,
		Rejected = 32,
		Succeeded = 64,
		Failed = 128,
		Oofed = 256,
		OofOrNdrHandled = 512,
		DecisionIndepedentFlags = 1048320
	}
}
