using System;

namespace Microsoft.Exchange.BITS
{
	[Flags]
	internal enum BG_JOB_NOTIFICATION_TYPE : uint
	{
		BG_NOTIFY_JOB_TRANSFERRED = 1U,
		BG_NOTIFY_JOB_ERROR = 2U,
		BG_NOTIFY_DISABLE = 4U,
		BG_NOTIFY_JOB_MODIFICATION = 8U
	}
}
