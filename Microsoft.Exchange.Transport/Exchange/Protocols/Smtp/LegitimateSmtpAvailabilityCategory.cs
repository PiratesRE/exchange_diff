using System;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal enum LegitimateSmtpAvailabilityCategory
	{
		SuccessfulSubmission,
		RejectDueToMaxInboundConnectionLimit,
		RejectDueToWLIDDown,
		RejectDueToADDown,
		RejectDueToBackPressure,
		RejectDueToIOException,
		RejectDueToTLSError,
		RejectDueToMaxLocalLoopCount
	}
}
