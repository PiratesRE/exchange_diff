using System;

namespace Microsoft.Exchange.BITS
{
	internal enum BG_JOB_STATE
	{
		BG_JOB_STATE_QUEUED,
		BG_JOB_STATE_CONNECTING,
		BG_JOB_STATE_TRANSFERRING,
		BG_JOB_STATE_SUSPENDED,
		BG_JOB_STATE_ERROR,
		BG_JOB_STATE_TRANSIENT_ERROR,
		BG_JOB_STATE_TRANSFERRED,
		BG_JOB_STATE_ACKNOWLEDGED,
		BG_JOB_STATE_CANCELLED,
		BG_JOB_STATE_UPDATE_AVAILABLE = 1001,
		BG_JOB_STATE_VALIDATION_SUCCESS,
		BG_JOB_STATE_VALIDATION_FAILED
	}
}
