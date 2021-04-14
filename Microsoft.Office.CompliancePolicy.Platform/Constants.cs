using System;

namespace Microsoft.Office.CompliancePolicy
{
	public static class Constants
	{
		public const string SyncAgentComponentName = "UnifiedPolicySyncAgent";

		public const string EnqueueErrorShutDown = "Queue is shutting down";

		public const string EnqueueErrorQueueFull = "Queue is full";

		public const string EnqueueErrorItemFailure = "The item insertion has failed";

		public const string EnqueueErrorMergeConflict = "The item insertion has failed due to conflict";
	}
}
