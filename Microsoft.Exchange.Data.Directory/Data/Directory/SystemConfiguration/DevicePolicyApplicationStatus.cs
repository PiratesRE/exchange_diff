using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum DevicePolicyApplicationStatus
	{
		[LocDescription(DirectoryStrings.IDs.Unknown)]
		Unknown,
		[LocDescription(DirectoryStrings.IDs.NotApplied)]
		NotApplied,
		[LocDescription(DirectoryStrings.IDs.AppliedInFull)]
		AppliedInFull,
		[LocDescription(DirectoryStrings.IDs.PartiallyApplied)]
		PartiallyApplied,
		[LocDescription(DirectoryStrings.IDs.ExternallyManaged)]
		ExternallyManaged
	}
}
