using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum OrganizationStatus
	{
		Invalid,
		Active,
		PendingCompletion,
		PendingRemoval,
		PendingAcceptedDomainAddition,
		PendingAcceptedDomainRemoval,
		ReadyForRemoval = 8,
		ReadOnly = 10,
		PendingArrival,
		Suspended,
		LockedOut,
		Retired,
		SoftDeleted
	}
}
