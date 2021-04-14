using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring
{
	[Flags]
	internal enum ObserverSelectionReason
	{
		None = 0,
		NoneInMaintenance = 1,
		NoSelectionTimestamp = 2,
		OldSelectionTimestamp = 4,
		NotEnoughObservers = 8,
		NoObserverTimestamp = 16,
		OldObserverTimestamp = 32
	}
}
