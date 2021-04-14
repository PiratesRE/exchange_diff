using System;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public enum ConnectionLostPoint
	{
		None,
		OnConnect,
		OnAuthenticate,
		OnHelo,
		OnStartTls,
		OnHeloAfterStartTls,
		OnMailFrom,
		OnRcptTo,
		OnData
	}
}
