using System;

namespace Microsoft.Office.CompliancePolicy
{
	public enum SyncAgentErrorCode
	{
		Generic,
		EnqueueErrorShutDown,
		EnqueueErrorQueueFull,
		EnqueueErrorItemFailure,
		EnqueueErrorMergeConflict
	}
}
