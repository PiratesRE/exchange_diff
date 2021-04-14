using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	public enum ArchiveState
	{
		[LocDescription(DirectoryStrings.IDs.ArchiveStateNone)]
		None,
		[LocDescription(DirectoryStrings.IDs.ArchiveStateLocal)]
		Local,
		[LocDescription(DirectoryStrings.IDs.ArchiveStateHostedProvisioned)]
		HostedProvisioned,
		[LocDescription(DirectoryStrings.IDs.ArchiveStateHostedPending)]
		HostedPending,
		[LocDescription(DirectoryStrings.IDs.ArchiveStateOnPremise)]
		OnPremise
	}
}
