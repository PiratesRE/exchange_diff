using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public enum JobPickupResult
	{
		JobPickedUp = 1,
		CompletedJobCleanedUp,
		CompletedJobSkipped = 100,
		JobIsPostponed,
		JobAlreadyActive,
		DisabledJobPickup,
		PostponeCancel,
		ReservationFailure = 200,
		ProxyBackoff,
		PickupFailure,
		UnknownJobType = 300,
		InvalidJob,
		PoisonedJob,
		JobOwnedByTransportSync
	}
}
