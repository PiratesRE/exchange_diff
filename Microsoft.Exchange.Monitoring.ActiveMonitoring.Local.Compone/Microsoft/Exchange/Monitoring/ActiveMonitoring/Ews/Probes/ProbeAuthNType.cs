using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Ews.Probes
{
	public enum ProbeAuthNType
	{
		LiveIdOrNegotiate,
		LiveId,
		Negotiate,
		Cafe,
		Web,
		LiveIdOrCafe,
		Unused
	}
}
