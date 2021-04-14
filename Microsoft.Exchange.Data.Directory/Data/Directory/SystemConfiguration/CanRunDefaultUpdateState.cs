using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum CanRunDefaultUpdateState
	{
		[LocDescription(DirectoryStrings.IDs.CanRunDefaultUpdateState_Invalid)]
		Invalid,
		[LocDescription(DirectoryStrings.IDs.CanRunDefaultUpdateState_NotLocal)]
		NotLocal,
		[LocDescription(DirectoryStrings.IDs.CanRunDefaultUpdateState_NotSuspended)]
		NotSuspended,
		[LocDescription(DirectoryStrings.IDs.CanRunDefaultUpdateState_Allowed)]
		Allowed
	}
}
