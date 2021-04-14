using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum UMUriType
	{
		[LocDescription(DirectoryStrings.IDs.TelExtn)]
		TelExtn = 1,
		[LocDescription(DirectoryStrings.IDs.E164)]
		E164,
		[LocDescription(DirectoryStrings.IDs.SipName)]
		SipName
	}
}
