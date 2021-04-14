using System;

namespace Microsoft.Exchange.EdgeSync.Validation
{
	public enum SyncStatus
	{
		Synchronized = 1,
		NotSynchronized,
		NotStarted,
		InProgress,
		Skipped,
		DirectoryError,
		Expired
	}
}
