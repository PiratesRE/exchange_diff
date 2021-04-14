using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum ReplicationType
	{
		[LocDescription(DirectoryStrings.IDs.ReplicationTypeNone)]
		None,
		[LocDescription(DirectoryStrings.IDs.ReplicationTypeRemote)]
		Remote = 2,
		[LocDescription(DirectoryStrings.IDs.ReplicationTypeUnknown)]
		Unknown
	}
}
