using System;

namespace Microsoft.Exchange.EdgeSync.Validation
{
	public enum ValidationStatus
	{
		NoSyncConfigured,
		Normal,
		Warning,
		Failed,
		Inconclusive,
		FailedUrgent
	}
}
