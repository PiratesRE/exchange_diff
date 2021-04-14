using System;

namespace Microsoft.Exchange.Data.Storage.ActiveManager
{
	internal enum AmDbActionReason
	{
		None,
		Startup,
		Cmdlet,
		FailureItem,
		StoreStarted,
		StoreStopped,
		NodeUp,
		NodeDown,
		SystemShutdown,
		PeriodicAction,
		MapiNetFailure,
		CatalogFailureItem,
		NodeDownConfirmed,
		ManagedAvailability,
		Rebalance,
		ActivationDisabled,
		TimeoutFailure,
		ReplayDown
	}
}
