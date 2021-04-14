using System;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel
{
	public enum ComplianceJobStatus : byte
	{
		StatusUnknown,
		NotStarted,
		Starting,
		InProgress,
		Succeeded,
		Failed,
		PartiallySucceeded,
		Stopped
	}
}
