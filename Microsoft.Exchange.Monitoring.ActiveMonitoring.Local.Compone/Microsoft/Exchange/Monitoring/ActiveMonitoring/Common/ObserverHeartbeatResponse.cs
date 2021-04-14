using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	[Serializable]
	internal enum ObserverHeartbeatResponse
	{
		NoResponse,
		UnknownObserver,
		Success
	}
}
