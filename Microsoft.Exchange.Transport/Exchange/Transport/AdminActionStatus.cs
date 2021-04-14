using System;

namespace Microsoft.Exchange.Transport
{
	internal enum AdminActionStatus : byte
	{
		None,
		Suspended,
		PendingDeleteWithNDR,
		PendingDeleteWithOutNDR,
		SuspendedInSubmissionQueue
	}
}
