using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum CanRunRestoreState
	{
		[LocDescription(DirectoryStrings.IDs.CanRunRestoreState_Invalid)]
		Invalid,
		[LocDescription(DirectoryStrings.IDs.CanRunRestoreState_NotLocal)]
		NotLocal,
		[LocDescription(DirectoryStrings.IDs.CanRunRestoreState_Allowed)]
		Allowed
	}
}
