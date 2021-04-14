using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum PreferredInternetCodePageForShiftJisEnum
	{
		[LocDescription(DirectoryStrings.IDs.PreferredInternetCodePageUndefined)]
		Undefined,
		[LocDescription(DirectoryStrings.IDs.PreferredInternetCodePageIso2022Jp)]
		Iso2022Jp = 50220,
		[LocDescription(DirectoryStrings.IDs.PreferredInternetCodePageEsc2022Jp)]
		Esc2022Jp,
		[LocDescription(DirectoryStrings.IDs.PreferredInternetCodePageSio2022Jp)]
		Sio2022Jp
	}
}
