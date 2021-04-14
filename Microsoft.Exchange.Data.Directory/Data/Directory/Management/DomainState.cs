using System;

namespace Microsoft.Exchange.Data.Directory.Management
{
	public enum DomainState
	{
		[LocDescription(DirectoryStrings.IDs.DomainStateUnknown)]
		Unknown,
		[LocDescription(DirectoryStrings.IDs.DomainStateCustomProvision)]
		CustomProvision,
		[LocDescription(DirectoryStrings.IDs.DomainStatePendingActivation)]
		PendingActivation,
		[LocDescription(DirectoryStrings.IDs.DomainStatePendingRelease)]
		PendingRelease,
		[LocDescription(DirectoryStrings.IDs.DomainStateActive)]
		Active
	}
}
