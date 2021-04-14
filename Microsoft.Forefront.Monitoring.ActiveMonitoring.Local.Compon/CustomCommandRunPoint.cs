using System;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public enum CustomCommandRunPoint
	{
		None,
		AfterConnect,
		AfterHelo,
		AfterAuthenticate,
		AfterStartTls,
		AfterHeloAfterStartTls,
		AfterMailFrom,
		BeforeRcptTo,
		AfterRcptTo,
		BeforeData,
		AfterData
	}
}
