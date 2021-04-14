using System;

namespace Microsoft.Exchange.Data.Storage.ActiveMonitoring
{
	public enum MonitorServerComponentState
	{
		Unknown,
		NotApplicable,
		Online,
		PartiallyOnline,
		Offline,
		Functional,
		Sidelined
	}
}
