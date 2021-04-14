using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.PopImap.Probes
{
	internal enum ProbeState
	{
		Capability1,
		Capability2,
		Capability3,
		Login1,
		Login2,
		User1,
		User2,
		Pass1,
		Pass2,
		StartTls1,
		Failure,
		Finish
	}
}
