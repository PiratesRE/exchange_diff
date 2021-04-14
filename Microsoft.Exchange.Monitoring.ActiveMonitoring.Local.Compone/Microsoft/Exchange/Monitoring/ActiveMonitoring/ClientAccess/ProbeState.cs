using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ClientAccess
{
	internal enum ProbeState
	{
		PreparingRequest,
		WaitingResponse,
		Passed,
		FailedRequest,
		FailedResponse,
		TimedOut
	}
}
