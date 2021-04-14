using System;
using Microsoft.Exchange.Core;

namespace Microsoft.Exchange.Management.FfoReporting
{
	public enum JobStatus
	{
		[LocDescription(CoreStrings.IDs.JobStatusNotStarted)]
		NotStarted,
		[LocDescription(CoreStrings.IDs.JobStatusInProgress)]
		InProgress,
		[LocDescription(CoreStrings.IDs.JobStatusCancelled)]
		Cancelled,
		[LocDescription(CoreStrings.IDs.JobStatusFailed)]
		Failed,
		[LocDescription(CoreStrings.IDs.JobStatusDone)]
		Done
	}
}
